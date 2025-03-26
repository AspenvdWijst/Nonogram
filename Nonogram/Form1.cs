namespace Nonogram
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Drawing;
    using System.Windows.Forms;


    public partial class Form1 : Form
    {
        private int CellSize = 100;
        private int ClueSize = 100;
        private const int CluePadding = 10;
        private const int AnimationSpeed = 10;
        private int GridSize = 5;
        private Settings settings;
        private DateTime _start;

        private bool[,] solutionGrid;
        private int[,] playerGrid;

        public Form1()
        {
            InitializeComponent();
            settings = Settings.Load();
            this.Paint += new PaintEventHandler(this.OnPaint);
            this.MouseClick += new MouseEventHandler(this.OnMouseClick);
            this.Size = new Size(1920, 1080);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);
            ComboBoxInfo();
            InitializeGrids();
        }

        private void InitializeGrids()
        {
            timer1.Start();
            _start = DateTime.Now;
            solutionGrid = GenerateRandomSolution(GridSize, GridSize);
            playerGrid = new int[GridSize, GridSize];
        }

        private void ComboBoxInfo()
        {
            SizeComboBox.Items.AddRange(new string[] { "5x5", "10x10", "15x15", "20x20" });
            SizeComboBox.SelectedIndex = 0;
            SizeComboBox.SelectedIndexChanged += SizeComboBox_SelectedIndexChanged;
        }

        private void SizeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedSize = SizeComboBox.SelectedItem.ToString();

            switch (selectedSize)
            {
                case "5x5":
                    GridSize = 5;
                    ClueSize = 100;
                    CellSize = 100;
                    break;
                case "10x10":
                    GridSize = 10;
                    ClueSize = 100;
                    CellSize = 50;
                    break;
                case "15x15":
                    GridSize = 15;
                    ClueSize = 150;
                    CellSize = 50;
                    break;
                case "20x20":
                    GridSize = 20;
                    ClueSize = 175;
                    CellSize = 40;
                    break;
            }

            InitializeGrids(); // Reset grid data
            this.Invalidate(); // Repaint the form
        }

        private bool[,] GenerateRandomSolution(int rows, int cols)
        {
            bool[,] grid = new bool[rows, cols];
            Random rand = new Random();

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = rand.Next(2) == 1; // Randomly assign true (1) or false (0)
                }
            }

            return grid;
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Bitmap offscreenBitmap = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(offscreenBitmap))
            {
                DrawGame(g);
            }
            e.Graphics.DrawImage(offscreenBitmap, 0, 0);
        }

        private void DrawGame(Graphics g)
        {
            Pen gridPen = new Pen(Color.Black, 2);
            Font clueFont = new Font("Arial", 14);
            Brush textBrush = Brushes.Black;

            int gridStartX = ClueSize;
            int gridStartY = ClueSize;



            for (int i = 0; i <= GridSize; i++)
            {
                g.DrawLine(gridPen, gridStartX + i * CellSize, 0, gridStartX + i * CellSize, ClueSize);
                g.DrawLine(gridPen, 0, gridStartY + i * CellSize, ClueSize, gridStartY + i * CellSize);
            }

            // Draw row clues
            for (int row = 0; row < GridSize; row++)
            {
                string clue = GetRowClue(row);
                g.DrawString(clue, clueFont, textBrush, ClueSize - CluePadding - g.MeasureString(clue, clueFont).Width, gridStartY + row * CellSize + 10);
            }

            // Draw column clues
            for (int col = 0; col < GridSize; col++)
            {
                string clue = GetColumnClue(col);
                string[] numbers = clue.Split(' ');
                for (int i = 0; i < numbers.Length; i++)
                {
                    g.DrawString(numbers[i], clueFont, textBrush, gridStartX + col * CellSize + 10, ClueSize - CluePadding - ((numbers.Length - i) * 20));
                }
            }

            // Draw grid
            for (int i = 0; i <= GridSize; i++)
            {
                // Vertical lines
                g.DrawLine(gridPen, gridStartX + i * CellSize, gridStartY, gridStartX + i * CellSize, gridStartY + GridSize * CellSize);

                // Horizontal lines
                g.DrawLine(gridPen, gridStartX, gridStartY + i * CellSize, gridStartX + GridSize * CellSize, gridStartY + i * CellSize);
            }

            // Draw player-filled cells
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int x = gridStartX + col * CellSize;
                    int y = gridStartY + row * CellSize;

                    if (playerGrid[row, col] == 1)
                    {
                        g.FillRectangle(Brushes.Black, x + 2, y + 2, CellSize - 4, CellSize - 4);
                    }
                    else if (playerGrid[row, col] == 2)
                    {
                        g.DrawString("X", clueFont, Brushes.Black, x + 15, y + 10);
                    }
                }
            }
        }

        private string GetRowClue(int row)
        {
            string clue = "";
            int count = 0;

            for (int col = 0; col < GridSize; col++)
            {
                if (solutionGrid[row, col]) count++;
                else if (count > 0)
                {
                    clue += count + " ";
                    count = 0;
                }
            }
            if (count > 0) clue += count;
            return string.IsNullOrEmpty(clue) ? "0" : clue.Trim();
        }

        private string GetColumnClue(int col)
        {
            string clue = "";
            int count = 0;

            for (int row = 0; row < GridSize; row++)
            {
                if (solutionGrid[row, col]) count++;
                else if (count > 0)
                {
                    clue += count + " ";
                    count = 0;
                }
            }
            if (count > 0) clue += count;
            return string.IsNullOrEmpty(clue) ? "0" : clue.Trim();
        }

        private async void OnMouseClick(object sender, MouseEventArgs e)
        {
            int gridStartX = ClueSize;
            int gridStartY = ClueSize;

            int col = (e.X - gridStartX) / CellSize;
            int row = (e.Y - gridStartY) / CellSize;

            if (row >= 0 && row < GridSize && col >= 0 && col < GridSize)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Toggle between empty and filled
                    if (playerGrid[row, col] == 1)
                        playerGrid[row, col] = 0; // If already filled, empty it
                    else
                    {
                        if (settings.animationsEnabled)
                            await AnimateFillCell(row, col);
                        else
                            playerGrid[row, col] = 1;
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // Toggle between empty and X
                    playerGrid[row, col] = (playerGrid[row, col] == 2) ? 0 : 2;
                }

                this.Invalidate(); // Redraw the form
                CheckWinCondition();
            }
        }

        private string GetRowClueFromPlayer(int row)
        {
            string clue = "";
            int count = 0;

            for (int col = 0; col < GridSize; col++)
            {
                if (playerGrid[row, col] == 1) count++;
                else if (count > 0)
                {
                    clue += count + " ";
                    count = 0;
                }
            }
            if (count > 0) clue += count;

            return string.IsNullOrEmpty(clue) ? "0" : clue.Trim();
        }

        private string GetColumnClueFromPlayer(int col)
        {
            string clue = "";
            int count = 0;

            for (int row = 0; row < GridSize; row++)
            {
                if (playerGrid[row, col] == 1) count++;
                else if (count > 0)
                {
                    clue += count + " ";
                    count = 0;
                }
            }
            if (count > 0) clue += count;

            return string.IsNullOrEmpty(clue) ? "0" : clue.Trim();
        }

        private async Task AnimateFillCell(int row, int col)
        {
            int steps = 10; // Number of animation steps
            int delay = AnimationSpeed; // Speed of animation

            for (int i = 0; i <= steps; i++)
            {
                float scale = (float)i / steps;
                using (Graphics g = this.CreateGraphics())
                {
                    int gridStartX = ClueSize;
                    int gridStartY = ClueSize;
                    int x = gridStartX + col * CellSize;
                    int y = gridStartY + row * CellSize;

                    g.FillRectangle(Brushes.Black, x + (CellSize * (1 - scale)) / 2,
                                    y + (CellSize * (1 - scale)) / 2,
                                    CellSize * scale, CellSize * scale);
                }
                await Task.Delay(delay);
            }
            playerGrid[row, col] = 1;
            this.Invalidate(); // Finalize drawing
        }

        private async Task AnimateBoardReset()
        {
            int steps = 10; // Number of fade-out steps
            int delay = AnimationSpeed; // Speed of animation

            for (int i = steps; i >= 0; i--)
            {
                float alpha = (float)i / steps;
                using (Graphics g = this.CreateGraphics())
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb((int)(255 * alpha), Color.White)), ClueSize, ClueSize, GridSize * CellSize, GridSize * CellSize);
                }
                await Task.Delay(delay);
            }

            playerGrid = new int[GridSize, GridSize];
            this.Invalidate(); // Redraw after animation
        }

        private void CheckWinCondition()
        {
            for (int row = 0; row < GridSize; row++)
            {
                if (GetRowClueFromPlayer(row) != GetRowClue(row))
                    return;
            }

            for (int col = 0; col < GridSize; col++)
            {
                if (GetColumnClueFromPlayer(col) != GetColumnClue(col))
                    return;
            }

            MessageBox.Show("Puzzle Solved!", "Nonogram", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void SolvePuzzle()
        {
            DisableUI();
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    bool shouldBeFilled = solutionGrid[row, col];

                    if (shouldBeFilled && playerGrid[row, col] != 1)
                    {
                        SimulateClick(row, col, MouseButtons.Left);
                    }
                    else if (!shouldBeFilled && playerGrid[row, col] != 2)
                    {
                        SimulateClick(row, col, MouseButtons.Right);
                    }

                    await Task.Delay(100); // Smooth animation effect
                }
            }
            EnableUI();
        }

        private void SimulateClick(int row, int col, MouseButtons button)
        {
            int gridStartX = ClueSize;
            int gridStartY = ClueSize;

            int x = gridStartX + col * CellSize + CellSize / 2;
            int y = gridStartY + row * CellSize + CellSize / 2;

            MouseEventArgs clickEvent = new MouseEventArgs(button, 1, x, y, 0);
            OnMouseClick(this, clickEvent);
        }

        private void DisableUI()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button || ctrl is Panel || ctrl is ComboBox)
                {
                    ctrl.Enabled = false;
                }
            }
        }

        private void EnableUI()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button || ctrl is Panel || ctrl is ComboBox)
                {
                    ctrl.Enabled = true;
                }
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit? Unsaved progress will be lost", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void ResetButton_Click(object sender, EventArgs e)
        {
            if (settings.animationsEnabled)
            {
                await AnimateBoardReset();
            }
            else
            {
                playerGrid = new int[GridSize, GridSize];
                this.Invalidate();
            }
        }

        private async void NewPuzzleBtn_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                InitializeGrids();
            });

            this.Invalidate();
        }

        private void SolveBtn_Click(object sender, EventArgs e)
        {
            ResetButton.Enabled = false;
            NewPuzzleBtn.Enabled = false;
            SolvePuzzle();
        }

        private void SizeComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            labelTime.Text = (DateTime.Now - _start).ToString(@"mm\:ss");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}

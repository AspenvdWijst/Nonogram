using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nonogram
{
    public partial class Form1 : Form
    {
        // Grid and clue display settings
        private int CellSize = 100;          // Size of each cell in the puzzle grid
        private int ClueSize = 100;          // Space for the clues around the grid
        private const int CluePadding = 10;  // Padding between clues and grid
        private const int AnimationSpeed = 10; // Speed for the animations
        private int GridSize = 5;            // Size of the grid width+height
        private Settings settings;           // Placeholder for settings
        private DateTime _start;             // Start moment for the timer

        // Game state variables
        private bool[,] solutionGrid;        // The correct solution grid
        private int[,] playerGrid;           // The player's current progress on the grid
        private bool PlayerSolved = true;    // Check if the player has solved the puzzle
        private int solvedPuzzlesCount = 0;  // Keeps track of the amount of puzzles solved
        private Label solvedPuzzlesLabel;    // Label to display how many puzzles have been solved

        // Constructor for Form1
        public Form1(int gridSize, int[,] savedPlayerGrid = null)
        {
            InitializeComponent();               
            InitializeSolvedPuzzlesLabel();      // Initialize the label that tracks solved puzzles
            this.GridSize = gridSize;            // Set grid size based on input

            // Custom sizes based on different gridsizes
            switch (gridSize)
            {
                case 5:
                    GridSize = 5;
                    ClueSize = 100;
                    CellSize = 100;
                    break;
                case 10:
                    GridSize = 10;
                    ClueSize = 100;
                    CellSize = 50;
                    break;
                case 15:
                    GridSize = 15;
                    ClueSize = 150;
                    CellSize = 50;
                    break;
                case 20:
                    GridSize = 20;
                    ClueSize = 175;
                    CellSize = 40;
                    break;
            }

            this.playerGrid = playerGrid;                     // Assign player grid reference
            InitializeGrids(savedPlayerGrid);                 // Initialize the grid, with save data if that exists
            this.Invalidate();                                // Repaint the form
            settings = Settings.Load();                       // Load game settings
            this.Paint += new PaintEventHandler(this.OnPaint);        // Custom paint event
            this.MouseClick += new MouseEventHandler(this.OnMouseClick); // Custom mouse click handler
            this.Size = new Size(1920, 1080);                 // Set window size

            // Enable double buffering for smooth drawing and prevent flickering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);

            // Logic for the timer
            timer1 = new System.Windows.Forms.Timer();
            timer1.Tick += new EventHandler(timer1_Tick);

            this.FormClosed += (s, e) => Application.Exit();
            Button backButton = new Button
            {
                Text = "Back to Main Menu",
                Location = new Point(1700, 900),
                AutoSize = true
            };
            backButton.Click += BackButton_Click;
            this.Controls.Add(backButton);
            
        }

        public Form1()
        {
        }

        //hombutton click event
        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            MainMenuForm mainMenu = new MainMenuForm();
            mainMenu.ShowDialog();
            this.Close();
        }

        private void InitializeGrids(int[,] savedPlayerGrid = null)
        {
            if (savedPlayerGrid != null)
            {
                // Use the saved player grid if available
                playerGrid = savedPlayerGrid;
            }
            else
            {
                // Initialize a new empty player grid
                playerGrid = new int[GridSize, GridSize];

                // If no saved grid, set all cells to 0 for a fresh start.
                for (int i = 0; i < GridSize; i++)
                {
                    for (int j = 0; j < GridSize; j++)
                    {
                        playerGrid[i, j] = 0;
                    }
                }
            }

            timer1.Start();
            _start = DateTime.Now;

            // Generate the solution grid
            solutionGrid = GenerateRandomSolution(GridSize, GridSize);
        }

        private void InitializeSolvedPuzzlesLabel()
        {
            solvedPuzzlesLabel = new Label();
            solvedPuzzlesLabel.Text = $"Puzzles Solved: {solvedPuzzlesCount}";
            solvedPuzzlesLabel.Font = new Font("Arial", 14, FontStyle.Bold);
            solvedPuzzlesLabel.Location = new Point(900, 20);
            solvedPuzzlesLabel.AutoSize = true;
            this.Controls.Add(solvedPuzzlesLabel);
        }

        private void UpdateSolvedPuzzlesLabel()
        {
            if (solvedPuzzlesLabel != null)
            {
                solvedPuzzlesLabel.Text = $"Puzzles Solved: {solvedPuzzlesCount}";
            }
        }

        private bool[,] GenerateRandomSolution(int rows, int cols)
        {
            bool[,] grid = new bool[rows, cols];
            Random rand = new Random();

            // Fill each cell randomly with true (filled) or false (empty)
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = rand.Next(2) == 1;
                }
            }

            return grid; // Return the randomly generated solution grid
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            // Create a bitmap to draw on, to reduce flickering (off-screen rendering)
            Bitmap offscreenBitmap = new Bitmap(this.Width, this.Height);
            using (Graphics g = Graphics.FromImage(offscreenBitmap))
            {
                DrawGame(g); // Draw the full game state on the bitmap
            }

            // Draw the off-screen bitmap to the actual screen
            e.Graphics.DrawImage(offscreenBitmap, 0, 0);
        }

        private void DrawGame(Graphics g)
        {
            Pen gridPen = new Pen(Color.Black, 2); // Pen for grid lines
            Font clueFont = new Font("Arial", 14); // Font for clue numbers
            Brush textBrush = Brushes.Black;       // Brush for drawing text

            int gridStartX = ClueSize; // X start position of the actual grid (leaves space for column clues)
            int gridStartY = ClueSize; // Y start position of the actual grid (leaves space for row clues)

            // Draw column lines above the grid for clues
            for (int i = 0; i <= GridSize; i++)
            {
                g.DrawLine(gridPen, gridStartX + i * CellSize, 0, gridStartX + i * CellSize, ClueSize);
                g.DrawLine(gridPen, 0, gridStartY + i * CellSize, ClueSize, gridStartY + i * CellSize);
            }

            // Draw row clues to the left of the grid
            for (int row = 0; row < GridSize; row++)
            {
                string clue = GetRowClue(row); // Get clue string for this row
               
                // Draw the clue right-aligned inside the clue area
                g.DrawString(clue, clueFont, textBrush,
                    ClueSize - CluePadding - g.MeasureString(clue, clueFont).Width,
                    gridStartY + row * CellSize + 10);
            }

            // Draw column clues above the grid
            for (int col = 0; col < GridSize; col++)
            {
                string clue = GetColumnClue(col); // Get clue string for this column
                string[] numbers = clue.Split(' '); // Split clue into individual numbers
                for (int i = 0; i < numbers.Length; i++)
                {
                    // Draw each clue number stacked vertically
                    g.DrawString(numbers[i], clueFont, textBrush,
                        gridStartX + col * CellSize + 10,
                        ClueSize - CluePadding - ((numbers.Length - i) * 20));
                }
            }

            // Draw main grid lines
            for (int i = 0; i <= GridSize; i++)
            {
                // Vertical lines
                g.DrawLine(gridPen, gridStartX + i * CellSize, gridStartY,
                                     gridStartX + i * CellSize, gridStartY + GridSize * CellSize);
                // Horizontal lines
                g.DrawLine(gridPen, gridStartX, gridStartY + i * CellSize,
                                     gridStartX + GridSize * CellSize, gridStartY + i * CellSize);
            }

            // Draw the player's filled or marked cells
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int x = gridStartX + col * CellSize;
                    int y = gridStartY + row * CellSize;

                    if (playerGrid[row, col] == 1)
                    {
                        // Filled cell (black)
                        g.FillRectangle(Brushes.Black, x + 2, y + 2, CellSize - 4, CellSize - 4);
                    }
                    else if (playerGrid[row, col] == 2)
                    {
                        // Marked as empty (with an "X")
                        g.DrawString("X", clueFont, Brushes.Black, x + 15, y + 10);
                    }
                }
            }
        }

        // Generates the clue string for a given row based on the solution grid
        private string GetRowClue(int row)
        {
            string clue = "";
            int count = 0;

            for (int col = 0; col < GridSize; col++)
            {
                if (solutionGrid[row, col]) count++; // Count consecutive filled cells
                else if (count > 0)
                {
                    clue += count + " "; // Add group size to clue string
                    count = 0;
                }
            }
            if (count > 0) clue += count; // Add last group if it ends at the row's end

            return string.IsNullOrEmpty(clue) ? "0" : clue.Trim(); // Return "0" if no filled cells
        }

        // Generates the clue string for a given column based on the solution grid
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

        // Handles mouse click events on the game board
        public async void OnMouseClick(object sender, MouseEventArgs e)
        {
            int gridStartX = ClueSize;
            int gridStartY = ClueSize;

            // Determine which cell was clicked
            int col = (e.X - gridStartX) / CellSize;
            int row = (e.Y - gridStartY) / CellSize;

            // Make sure the click is within the grid bounds
            if (row >= 0 && row < GridSize && col >= 0 && col < GridSize)
            {
                if (e.Button == MouseButtons.Left)
                {
                    // Toggle between empty and filled
                    if (playerGrid[row, col] == 1)
                        playerGrid[row, col] = 0; // Unfill the cell
                    else
                    {
                        if (settings.animationsEnabled)
                            await AnimateFillCell(row, col); // Animate fill if enabled
                        else
                            playerGrid[row, col] = 1; // Fill the cell
                    }
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // Toggle between empty and marked (X)
                    playerGrid[row, col] = (playerGrid[row, col] == 2) ? 0 : 2;
                }

                SaveGrid();         // Save the current state of the grid
                this.Invalidate();  // Redraw the grid
                CheckWinCondition(); // Check if the puzzle is solved
            }
        }

        // Generate clue string for a row based on the player's current grid
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

        // Generate clue string for a column based on the player's current grid
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

        // Animate a cell being filled by gradually increasing the size of the black square
        private async Task AnimateFillCell(int row, int col)
        {
            int steps = 10; // Number of animation steps
            int delay = AnimationSpeed; // Delay between frames

            for (int i = 0; i <= steps; i++)
            {
                float scale = (float)i / steps;

                using (Graphics g = this.CreateGraphics())
                {
                    int gridStartX = ClueSize;
                    int gridStartY = ClueSize;
                    int x = gridStartX + col * CellSize;
                    int y = gridStartY + row * CellSize;

                    // Calculate size and position based on animation step
                    g.FillRectangle(Brushes.Black,
                                    x + (CellSize * (1 - scale)) / 2,
                                    y + (CellSize * (1 - scale)) / 2,
                                    CellSize * scale,
                                    CellSize * scale);
                }

                await Task.Delay(delay); // Wait for the next frame
            }

            playerGrid[row, col] = 1; // Finalize the cell as filled
            this.Invalidate();        // Redraw the form to reflect final state
        }

        // Fades out the board and resets the player grid
        private async Task AnimateBoardReset()
        {
            int steps = 10; // Number of fade-out steps
            int delay = AnimationSpeed; // Delay between frames

            for (int i = steps; i >= 0; i--)
            {
                float alpha = (float)i / steps; // Calculate transparency level
                using (Graphics g = this.CreateGraphics())
                {
                    // Fill the grid area with a white overlay of decreasing opacity
                    g.FillRectangle(
                        new SolidBrush(Color.FromArgb((int)(255 * alpha), Color.White)),
                        ClueSize, ClueSize, GridSize * CellSize, GridSize * CellSize
                    );
                }
                await Task.Delay(delay);
            }

            playerGrid = new int[GridSize, GridSize]; // Clear the grid
            this.Invalidate(); // Trigger a redraw
        }

        // Checks if the player's grid matches the solution grid
        private bool CheckWinCondition()
        {
            // Compare row clues
            for (int row = 0; row < GridSize; row++)
            {
                if (GetRowClueFromPlayer(row) != GetRowClue(row))
                    return false;
            }

            // Compare column clues
            for (int col = 0; col < GridSize; col++)
            {
                if (GetColumnClueFromPlayer(col) != GetColumnClue(col))
                    return false;
            }

            // If puzzle was solved, update progress
            if (PlayerSolved == true)
            {
                solvedPuzzlesCount++;
                UpdateSolvedPuzzlesLabel();
                PlayerSolved = false;
            }

            MessageBox.Show("Puzzle Solved!", "Nonogram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        // Opens a settings window for toggling animations
        private void OpenSettingsWindow()
        {
            Form settingsForm = new Form()
            {
                Text = "Settings",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent
            };

            CheckBox animationCheckBox = new CheckBox()
            {
                Text = "Enable/disable Animations",
                Checked = settings.animationsEnabled,
                Location = new Point(50, 50),
                AutoSize = true
            };

            Button saveButton = new Button()
            {
                Text = "Save",
                Location = new Point(50, 100)
            };

            // Save the settings when the button is clicked
            saveButton.Click += (sender, e) =>
            {
                settings.animationsEnabled = animationCheckBox.Checked;
                settings.Save();
                settingsForm.Close();
            };

            settingsForm.Controls.Add(animationCheckBox);
            settingsForm.Controls.Add(saveButton);
            settingsForm.ShowDialog();
        }

        // Reveals a correct cell as a hint
        private async void GiveHint()
        {
            Random rand = new Random();
            List<(int, int)> possibleHints = new List<(int, int)>();

            // Find cells that are incorrect or unmarked
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (playerGrid[row, col] != (solutionGrid[row, col] ? 1 : 2))
                    {
                        possibleHints.Add((row, col));
                    }
                }
            }

            // Show a random hint if possible
            if (possibleHints.Count > 0)
            {
                var (hintRow, hintCol) = possibleHints[rand.Next(possibleHints.Count)];
                playerGrid[hintRow, hintCol] = solutionGrid[hintRow, hintCol] ? 1 : 2;
                this.Invalidate(); // Redraw to show the hint
                await Task.Delay(500); // Delay to prevent spam
            }
            else
            {
                MessageBox.Show("No hints available, the puzzle is almost complete!", "Hint", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Automatically solves the puzzle cell by cell with animation
        private async void SolvePuzzle()
        {
            DisableUI(); // Prevent user interaction
            PlayerSolved = false;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    // Stop solving if user already completed the puzzle
                    if (CheckWinCondition())
                    {
                        EnableUI();
                        return;
                    }

                    bool shouldBeFilled = solutionGrid[row, col];

                    // Simulate correct click depending on solution
                    if (shouldBeFilled && playerGrid[row, col] != 1)
                    {
                        SimulateClick(row, col, MouseButtons.Left);
                    }
                    else if (!shouldBeFilled && playerGrid[row, col] != 2)
                    {
                        SimulateClick(row, col, MouseButtons.Right);
                    }

                    await Task.Delay(100); // Smooth animation
                }
            }

            EnableUI(); // Re-enable user input
        }

        // Simulates a mouse click on the given cell
        private void SimulateClick(int row, int col, MouseButtons button)
        {
            int gridStartX = ClueSize;
            int gridStartY = ClueSize;

            // Calculate coordinates for center of the cell
            int x = gridStartX + col * CellSize + CellSize / 2;
            int y = gridStartY + row * CellSize + CellSize / 2;

            // Create a fake MouseEventArgs and pass to OnMouseClick
            MouseEventArgs clickEvent = new MouseEventArgs(button, 1, x, y, 0);
            OnMouseClick(this, clickEvent);
        }

        // Disables all interactive controls and mouse clicks
        private void DisableUI()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button || ctrl is Panel || ctrl is ComboBox)
                {
                    ctrl.Enabled = false;
                }
            }

            this.MouseClick -= OnMouseClick; // Detach click handler
        }

        // Enables controls and mouse input
        private void EnableUI()
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is Button || ctrl is Panel || ctrl is ComboBox)
                {
                    ctrl.Enabled = true;
                }
            }

            this.MouseClick += OnMouseClick; // Reattach click handler
        }

        // Convert the 2d array to a jagged array, so it can be saved in the JSON
        private int[][] ConvertToJaggedArray(int[,] twoDArray)
        {
            int rows = twoDArray.GetLength(0);
            int cols = twoDArray.GetLength(1);

            int[][] jaggedArray = new int[rows][];
            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new int[cols];
                for (int j = 0; j < cols; j++)
                {
                    jaggedArray[i][j] = twoDArray[i, j];
                }
            }

            return jaggedArray;
        }

        // Saves the player's grid as a JSON file
        private async Task SaveGrid()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SavedGrid.json");
            int[][] jaggedGrid = ConvertToJaggedArray(playerGrid);
            string jsonGrid = System.Text.Json.JsonSerializer.Serialize(jaggedGrid);

            await File.WriteAllTextAsync(filePath, jsonGrid); // Write async
        }

        // Ask the user for confirmation before closing the app
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit? Unsaved progress will be lost", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancel the close event
            }

            base.OnFormClosing(e); // Continue with default close behavior
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private async void ResetButton_Click(object sender, EventArgs e)
        {
            PlayerSolved = false; // Reset solved state

            // Use animation if enabled in settings
            if (settings.animationsEnabled)
            {
                await AnimateBoardReset();
            }
            else
            {
                // Instantly reset the player grid without animation
                playerGrid = new int[GridSize, GridSize];
                this.Invalidate(); // Redraw the form
            }
        }

        public async void NewPuzzleBtn_Click(object sender, EventArgs e)
        {
            PlayerSolved = true; // Mark current puzzle as completed
            timer1.Start();
            _start = DateTime.Now;
            // Run puzzle initialization in a background thread
            await Task.Run(() =>
            {
                InitializeGrids(); // Set up a new puzzle
            });

            this.Invalidate(); // Redraw the new puzzle
        }

        private void SolveBtn_Click(object sender, EventArgs e)
        {
            // Disable buttons to prevent interaction during solving
            ResetButton.Enabled = false;
            NewPuzzleBtn.Enabled = false;

            SolvePuzzle(); // Start solving animation
        }

        private void SizeComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Show elapsed time in the correct  format
            labelTime.Text = (DateTime.Now - _start).ToString(@"hh\:mm\:ss");
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            GiveHint(); // Provide a hint to the user
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenSettingsWindow(); // Open the settings dialog
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        public static class GameSettings
        {
            private static string settingsFilePath = "settings.txt";

            public static void SaveGridSize(int gridSize)
            {
                File.WriteAllText(settingsFilePath, gridSize.ToString());
            }
        }
    }
}

using Newtonsoft.Json;
using static Nonogram.Form1;

namespace Nonogram
{
    public partial class MainMenuForm : Form
    {
        public int SelectedGridSize { get; private set; } = 5;

        public MainMenuForm()
        {
            InitializeComponent();
            this.Text = "Nonogram - Main Menu";
            this.Size = new System.Drawing.Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;

            Button loadButton = new Button
            {
                Text = "Load Last Game",
                Size = new Size(200, 50),
                Location = new Point(150, 200)
            };
            loadButton.Click += LoadButton_Click;
            Controls.Add(loadButton);

            Label titleLabel = new Label()
            {
                Text = "Select Grid Size:",
                Location = new System.Drawing.Point(175, 60),
                AutoSize = true
            };
            this.Controls.Add(titleLabel);
                Label homeLabel = new Label()
                {
                    Text = "Nonogram",
                    Location = new System.Drawing.Point(175, 20),
                    Size = new Size (200,50),
                    AutoSize = true
                };
            this.Controls.Add(homeLabel);

            ComboBox sizeComboBox = new ComboBox()
            {
                Location = new System.Drawing.Point(175, 90),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            sizeComboBox.Items.AddRange(new string[] { "5x5", "10x10", "15x15", "20x20" });
            sizeComboBox.SelectedIndex = 0;
            sizeComboBox.SelectedIndexChanged += (sender, e) =>
            {
                switch (sizeComboBox.SelectedItem.ToString())
                {
                    case "5x5": SelectedGridSize = 5; break;
                    case "10x10": SelectedGridSize = 10; break;
                    case "15x15": SelectedGridSize = 15; break;
                    case "20x20": SelectedGridSize = 20; break;
                }
            };
            this.Controls.Add(sizeComboBox);

            Button startGameButton = new Button()
            {
                Text = "Start New Game",
                Location = new System.Drawing.Point(175, 120),
                AutoSize = true
            };
            startGameButton.Click += StartGameButton_Click;
            this.Controls.Add(startGameButton);
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 gameForm = new Form1(SelectedGridSize);  // Pass selected grid size to Form1
            gameForm.Show();
            this.Hide();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            // Load saved game data and grid size from the file
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\SavedGrid.json");

            if (File.Exists(filePath))
            {
                string savedData = File.ReadAllText(filePath);
                int[][] jaggedPlayerGrid = JsonConvert.DeserializeObject<int[][]>(savedData);

                // Convert jagged array to multidimensional array
                int[,] playerGrid = ConvertJaggedToMultidimensionalArray(jaggedPlayerGrid);

                // Dynamically get grid size from loaded data
                int savedGridSize = playerGrid.GetLength(0); // Assuming square grid

                // Pass the loaded grid and size to Form1
                this.Hide();
                Form1 gameForm = new Form1(savedGridSize, playerGrid);
                gameForm.Show();
                this.Show();
            }
            else
            {
                MessageBox.Show("No saved game found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private int[,] ConvertJaggedToMultidimensionalArray(int[][] jaggedArray)
        {
            int rows = jaggedArray.Length;
            int cols = jaggedArray[0].Length;
            int[,] multiArray = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    multiArray[i, j] = jaggedArray[i][j];
                }
            }

            return multiArray;
        }

        private void MainMenuForm_Load(object sender, EventArgs e)
        {

        }
    }
}
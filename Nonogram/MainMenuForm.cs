using System;
using System.Windows.Forms;

namespace Nonogram
{
    public partial class MainMenuForm : Form
    {
        public int SelectedGridSize { get; private set; } = 5;

        public MainMenuForm()
        {
            InitializeComponent();
            this.Text = "Nonogram - Main Menu";
            this.Size = new System.Drawing.Size(300, 200);
            this.StartPosition = FormStartPosition.CenterScreen;

            Label titleLabel = new Label()
            {
                Text = "Select Grid Size:",
                Location = new System.Drawing.Point(50, 30),
                AutoSize = true
            };
            this.Controls.Add(titleLabel);

            ComboBox sizeComboBox = new ComboBox()
            {
                Location = new System.Drawing.Point(50, 60),
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
                Location = new System.Drawing.Point(50, 100),
                AutoSize = true
            };
            startGameButton.Click += StartGameButton_Click;
            this.Controls.Add(startGameButton);
        }

        private void StartGameButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 gameForm = new Form1(SelectedGridSize);  // Pass selected grid size to Form1
            gameForm.ShowDialog();
            this.Close();
        }

    }
}
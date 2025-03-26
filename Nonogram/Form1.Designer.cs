namespace Nonogram
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            label2 = new Label();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            ResetButton = new Button();
            NewPuzzleBtn = new Button();
            SolveBtn = new Button();
            SizeComboBox = new ComboBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(1691, 109);
            label1.Name = "label1";
            label1.Size = new Size(96, 21);
            label1.TabIndex = 0;
            label1.Text = "Right click =";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label2.Location = new Point(1691, 165);
            label2.Name = "label2";
            label2.Size = new Size(89, 21);
            label2.TabIndex = 2;
            label2.Text = "Left click = ";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(1797, 156);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(40, 40);
            pictureBox1.TabIndex = 3;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(1797, 100);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(40, 40);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // ResetButton
            // 
            ResetButton.Location = new Point(1716, 222);
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(75, 72);
            ResetButton.TabIndex = 5;
            ResetButton.Text = "Reset";
            ResetButton.UseVisualStyleBackColor = true;
            ResetButton.Click += ResetButton_Click;
            // 
            // NewPuzzleBtn
            // 
            NewPuzzleBtn.Location = new Point(1716, 316);
            NewPuzzleBtn.Name = "NewPuzzleBtn";
            NewPuzzleBtn.Size = new Size(75, 71);
            NewPuzzleBtn.TabIndex = 6;
            NewPuzzleBtn.Text = "New Puzzle";
            NewPuzzleBtn.UseVisualStyleBackColor = true;
            NewPuzzleBtn.Click += NewPuzzleBtn_Click;
            // 
            // SolveBtn
            // 
            SolveBtn.Location = new Point(1716, 406);
            SolveBtn.Name = "SolveBtn";
            SolveBtn.Size = new Size(75, 69);
            SolveBtn.TabIndex = 7;
            SolveBtn.Text = "Solve puzzle";
            SolveBtn.UseVisualStyleBackColor = true;
            SolveBtn.Click += SolveBtn_Click;
            // 
            // SizeComboBox
            // 
            SizeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            SizeComboBox.FormattingEnabled = true;
            SizeComboBox.Location = new Point(1691, 59);
            SizeComboBox.Name = "SizeComboBox";
            SizeComboBox.Size = new Size(121, 23);
            SizeComboBox.Sorted = true;
            SizeComboBox.TabIndex = 8;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(1691, 41);
            label3.Name = "label3";
            label3.Size = new Size(62, 15);
            label3.TabIndex = 9;
            label3.Text = "Puzzle size";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(label3);
            Controls.Add(SizeComboBox);
            Controls.Add(SolveBtn);
            Controls.Add(NewPuzzleBtn);
            Controls.Add(ResetButton);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button ResetButton;
        private Button NewPuzzleBtn;
        private Button SolveBtn;
        private ComboBox SizeComboBox;
        private Label label3;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Nonogram;
using Newtonsoft.Json;

namespace Nonogram
{
    public partial class LoginForm : Form
    {
        private string filePath = "users.json"; // Path to user database json file

        public LoginForm()
        {
            InitializeComponent();
            this.Load += new EventHandler(LoginForm_Load);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            //
        }

        private void btnRegister_Click_1(object sender, EventArgs e)
        {
            // Saves entered text from the textboxes
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // Check if the textboxes are empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Empty username or password");
                return;
            }

            // Load the users from the json file
            List<User> users = LoadUsers();

            // Check if the username is already taken
            if (users.Exists(u => u.Username == username))
            {
                MessageBox.Show("Username already exists");
                return;
            }

            // Add the new user to the list
            users.Add(new User { Username = username, Password = HashPassword(password) });

            SaveUsers(users);
            MessageBox.Show("Registration complete!");
        }


        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            string hashedPassword = HashPassword(password);

            List<User> users = LoadUsers();

            // Debugging: Show loaded users
            // MessageBox.Show($"Loaded Users:\n" + JsonConvert.SerializeObject(users, Formatting.Indented));

            // Find the user with the entered username and password
            User user = users.Find(u => u.Username == username && u.Password == hashedPassword);

            if (user != null) // If the user is found
            {
                MessageBox.Show("Login successful!");
                this.Hide();
                Form1 mainForm = new Form1();
                mainForm.Show();
            }
            else
            {
                MessageBox.Show("Invalid credentials!\n\nEntered Username: " + username + "\nEntered Password (hashed): " + hashedPassword);
            }
        }


        private List<User> LoadUsers()
        {
            // If the file doesn't exist, create it
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "[]");
                return new List<User>();
            }

            // Load the users from the json file
            try
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading user database: " + ex.Message);
                return new List<User>();
            }
        }

        private void SaveUsers(List<User> users)
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented); 
            File.WriteAllText(filePath, json); // Save the users to the json file
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create()) // Create a SHA256 hash
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // Consistant hashing
            }
        }

        public class User
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}

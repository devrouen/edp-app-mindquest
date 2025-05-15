using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;

namespace mindquest
{
    public partial class signupForm : Form
    {
        public signupForm()
        {
            InitializeComponent();
        }

        private void btnSignup_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string recoveryPhrase = txtRecoveryPhrase.Text.Trim();

            // Validation
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(recoveryPhrase))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Email validation
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Password validation (at least 8 characters)
            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Check if username already exists
                    string checkUsernameQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
                    MySqlCommand checkUsernameCmd = new MySqlCommand(checkUsernameQuery, conn);
                    checkUsernameCmd.Parameters.AddWithValue("@username", username);
                    int usernameCount = Convert.ToInt32(checkUsernameCmd.ExecuteScalar());

                    if (usernameCount > 0)
                    {
                        MessageBox.Show("Username already exists. Please choose a different username.",
                            "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if email already exists
                    string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @email";
                    MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, conn);
                    checkEmailCmd.Parameters.AddWithValue("@email", email);
                    int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                    if (emailCount > 0)
                    {
                        MessageBox.Show("Email already registered. Please use a different email address.",
                            "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create user
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
                    string insertQuery = @"INSERT INTO users (username, email, password_hash, recovery_phrase, created_at) 
                                        VALUES (@username, @email, @password, @recoveryPhrase, NOW())";

                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@username", username);
                    insertCmd.Parameters.AddWithValue("@email", email);
                    insertCmd.Parameters.AddWithValue("@password", hashedPassword);
                    insertCmd.Parameters.AddWithValue("@recoveryPhrase", recoveryPhrase);

                    int rowsAffected = insertCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Account created successfully! You can now log in.",
                            "Registration Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        new loginForm().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create account. Please try again.",
                            "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating account: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Simple email validation
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            new loginForm().Show();
            this.Close();
        }
    }
}
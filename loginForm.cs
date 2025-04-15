using System;
using System.Data;
using System.Windows.Forms;
using MindQuest;
using MySql.Data.MySqlClient;

namespace mindquest
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Validate credentials and fetch userId + role
            (bool isValid, int userId, string role) = ValidateUser(username, password);

            if (isValid)
            {
                if (role == "admin")
                {
                    MessageBox.Show("Admin login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AdminDashboard adminDashboard = new AdminDashboard(username);
                    adminDashboard.Show();
                }
                else
                {
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    userDashboard userDashboard = new userDashboard(userId, username);
                    userDashboard.Show();
                }

                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private (bool, int, string) ValidateUser(string username, string password)
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT user_id, role, password_hash FROM users WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string storedHash = reader.GetString("password_hash");
                            if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                            {
                                int userId = reader.GetInt32("user_id");
                                string role = reader.GetString("role");
                                return (true, userId, role);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error validating user: " + ex.Message);
            }

            return (false, 0, string.Empty);
        }



        private string GetUserRole(string username)
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT role FROM users WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", username);

                    string role = Convert.ToString(cmd.ExecuteScalar());
                    return role;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching user role: " + ex.Message);
                return string.Empty;
            }
        }   

        private void lnkSignup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open the signup form and hide the login form
            signupForm signup = new signupForm();
            signup.Show();
            this.Hide();
        }
    }
}

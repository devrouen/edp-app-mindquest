using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace mindquest
{
    public partial class passwordRecoveryForm : Form
    {
        public passwordRecoveryForm()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string recoveryPhrase = txtRecoveryPhrase.Text.Trim();
            string newPassword = txtNewPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(recoveryPhrase) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    string selectQuery = "SELECT user_id FROM users WHERE username = @username AND recovery_phrase = @phrase";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@username", username);
                    selectCmd.Parameters.AddWithValue("@phrase", recoveryPhrase);

                    object result = selectCmd.ExecuteScalar();

                    if (result != null)
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                        string updateQuery = "UPDATE users SET password_hash = @password WHERE username = @username";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@password", hashedPassword);
                        updateCmd.Parameters.AddWithValue("@username", username);
                        updateCmd.ExecuteNonQuery();

                        MessageBox.Show("Password has been reset successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        new loginForm().Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Invalid username or recovery phrase.", "Reset Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error resetting password: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            new loginForm().Show();
            this.Close();
        }
    }
}

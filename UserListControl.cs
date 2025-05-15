using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class UserListControl : UserControl
    {
        private int selectedUserId = -1;
        private string selectedPasswordHash = string.Empty;
        private string selectedRecoveryPhrase = string.Empty;

        public UserListControl()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers(string searchTerm = "")
        {
            try
            {
                dgvUsers.Rows.Clear();

                string query = "SELECT user_id, username, email, password_hash, recovery_phrase, role, created_at FROM users";

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query += " WHERE username LIKE @search OR email LIKE @search";
                }

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        if (!string.IsNullOrWhiteSpace(searchTerm))
                        {
                            cmd.Parameters.AddWithValue("@search", "%" + searchTerm + "%");
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                // Get values from the reader
                                int userId = Convert.ToInt32(reader["user_id"]);
                                string username = reader["username"].ToString();
                                string email = reader["email"].ToString();
                                string passwordHash = reader["password_hash"].ToString();

                                // Handle recovery_phrase (it might be null in existing records)
                                string recoveryPhrase = "";
                                if (!reader.IsDBNull(reader.GetOrdinal("recovery_phrase")))
                                {
                                    recoveryPhrase = reader["recovery_phrase"].ToString();
                                }

                                string role = reader["role"].ToString();
                                string createdAt = reader["created_at"].ToString();

                                int rowId = dgvUsers.Rows.Add(
                                    userId,
                                    username,
                                    email,
                                    passwordHash,
                                    recoveryPhrase,
                                    role,
                                    createdAt
                                );
                            }
                        }
                    }
                }

                lblTotalUsers.Text = $"Total Users: {dgvUsers.Rows.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUsers(txtSearch.Text.Trim());
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSearch_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadUsers();
        }

        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedUserId = Convert.ToInt32(dgvUsers.Rows[e.RowIndex].Cells["ColUserId"].Value);
                string username = dgvUsers.Rows[e.RowIndex].Cells["ColUsername"].Value.ToString();
                string email = dgvUsers.Rows[e.RowIndex].Cells["ColEmail"].Value.ToString();
                selectedPasswordHash = dgvUsers.Rows[e.RowIndex].Cells["ColPasswordHash"].Value?.ToString() ?? "";
                selectedRecoveryPhrase = dgvUsers.Rows[e.RowIndex].Cells["ColRecoveryPhrase"].Value?.ToString() ?? "";
                string role = dgvUsers.Rows[e.RowIndex].Cells["ColRole"].Value.ToString();

                txtUsername.Text = username;
                txtEmail.Text = email;
                txtPassword.Clear(); // Clear password field since we're displaying hash in the grid
                txtRecoveryPhrase.Text = selectedRecoveryPhrase;
                cmbRole.Text = role;

                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Check if username or email already exists
                if (IsUsernameTaken(txtUsername.Text) || IsEmailTaken(txtEmail.Text))
                {
                    MessageBox.Show("Username or email already exists.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text);

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO users (username, email, password_hash, recovery_phrase, role) 
                                    VALUES (@username, @email, @passwordHash, @recoveryPhrase, @role)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                        cmd.Parameters.AddWithValue("@recoveryPhrase", txtRecoveryPhrase.Text.Trim());
                        cmd.Parameters.AddWithValue("@role", cmbRole.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User added successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                        LoadUsers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding user: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedUserId < 0)
            {
                MessageBox.Show("Please select a user to update.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Username and Email are required fields.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Check if username or email already exists for a different user
                if (IsUsernameTaken(txtUsername.Text, selectedUserId) || IsEmailTaken(txtEmail.Text, selectedUserId))
                {
                    MessageBox.Show("Username or email already exists for another user.", "Validation Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query;

                    // Check if password needs to be updated
                    if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        string passwordHash = BCrypt.Net.BCrypt.HashPassword(txtPassword.Text);
                        query = @"UPDATE users SET 
                                username = @username, 
                                email = @email, 
                                password_hash = @passwordHash, 
                                recovery_phrase = @recoveryPhrase,
                                role = @role 
                                WHERE user_id = @userId";
                    }
                    else
                    {
                        // Keep existing password hash
                        query = @"UPDATE users SET 
                                username = @username, 
                                email = @email, 
                                recovery_phrase = @recoveryPhrase,
                                role = @role 
                                WHERE user_id = @userId";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@recoveryPhrase", txtRecoveryPhrase.Text.Trim());
                        cmd.Parameters.AddWithValue("@role", cmbRole.Text);
                        cmd.Parameters.AddWithValue("@userId", selectedUserId);

                        if (!string.IsNullOrWhiteSpace(txtPassword.Text))
                        {
                            cmd.Parameters.AddWithValue("@passwordHash", BCrypt.Net.BCrypt.HashPassword(txtPassword.Text));
                        }

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearForm();
                        LoadUsers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating user: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedUserId < 0)
            {
                MessageBox.Show("Please select a user to delete.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to delete this user?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = DBHelper.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM users WHERE user_id = @userId";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@userId", selectedUserId);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("User deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearForm();
                            LoadUsers();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting user: " + ex.Message, "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtUsername.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            txtRecoveryPhrase.Clear();
            cmbRole.SelectedIndex = 0; // Default to "user"
            selectedUserId = -1;
            selectedPasswordHash = string.Empty;
            selectedRecoveryPhrase = string.Empty;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private bool IsUsernameTaken(string username, int excludeUserId = -1)
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE username = @username";

                    if (excludeUserId != -1)
                    {
                        query += " AND user_id != @userId";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);

                        if (excludeUserId != -1)
                        {
                            cmd.Parameters.AddWithValue("@userId", excludeUserId);
                        }

                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking username: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true; // Assume taken in case of error
            }
        }

        private bool IsEmailTaken(string email, int excludeUserId = -1)
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE email = @email";

                    if (excludeUserId != -1)
                    {
                        query += " AND user_id != @userId";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);

                        if (excludeUserId != -1)
                        {
                            cmd.Parameters.AddWithValue("@userId", excludeUserId);
                        }

                        long count = (long)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking email: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true; // Assume taken in case of error
            }
        }

        private void groupBoxSearch_Enter(object sender, EventArgs e)
        {
            // This is an empty event handler
        }
    }
}
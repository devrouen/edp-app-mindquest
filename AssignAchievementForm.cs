using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class AssignAchievementForm : Form
    {
        private int _achievementId;
        private string _achievementName;

        public AssignAchievementForm(int achievementId)
        {
            InitializeComponent();
            _achievementId = achievementId;
            LoadAchievementInfo();
            LoadUsers();
        }

        private void LoadAchievementInfo()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT achievement_name FROM achievements WHERE achievement_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", _achievementId);
                    _achievementName = cmd.ExecuteScalar().ToString();

                    lblAchievementName.Text = _achievementName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading achievement information: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadUsers()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    // Get users who don't already have this achievement
                    string query = @"
                        SELECT u.user_id, u.username, u.email, u.role
                        FROM users u
                        WHERE u.user_id NOT IN (
                            SELECT ua.user_id FROM userachievements ua 
                            WHERE ua.achievement_id = @achievementId
                        )
                        ORDER BY u.username";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@achievementId", _achievementId);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvUsers.DataSource = dt;

                    // Configure columns
                    if (dgvUsers.Columns.Contains("user_id"))
                        dgvUsers.Columns["user_id"].Visible = false;

                    if (dgvUsers.Columns.Contains("username"))
                        dgvUsers.Columns["username"].HeaderText = "Username";

                    if (dgvUsers.Columns.Contains("email"))
                        dgvUsers.Columns["email"].HeaderText = "Email";

                    if (dgvUsers.Columns.Contains("role"))
                        dgvUsers.Columns["role"].HeaderText = "Role";

                    lblEligibleUsers.Text = $"Eligible users (without this achievement): {dt.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading users: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a user to assign the achievement to.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int userId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["user_id"].Value);
            string username = dgvUsers.SelectedRows[0].Cells["username"].Value.ToString();

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"INSERT INTO userachievements (user_id, achievement_id) 
                                  VALUES (@userId, @achievementId)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@achievementId", _achievementId);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show($"Achievement '{_achievementName}' assigned to {username} successfully!",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Refresh the users list
                    LoadUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error assigning achievement: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvUsers.DataSource != null)
                {
                    string searchText = txtSearch.Text.Trim().ToLower();

                    if (string.IsNullOrEmpty(searchText))
                    {
                        // Reset the filter
                        ((DataTable)dgvUsers.DataSource).DefaultView.RowFilter = "";
                    }
                    else
                    {
                        // Apply filter
                        ((DataTable)dgvUsers.DataSource).DefaultView.RowFilter =
                            $"username LIKE '%{searchText}%' OR email LIKE '%{searchText}%'";
                    }

                    lblEligibleUsers.Text = $"Eligible users (without this achievement): {dgvUsers.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering data: " + ex.Message, "Filter Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSearch.Clear();
            }
        }
    }
}
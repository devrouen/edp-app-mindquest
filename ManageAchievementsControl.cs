using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class ManageAchievementsControl : UserControl
    {
        public ManageAchievementsControl()
        {
            InitializeComponent();
            LoadAchievements();
        }

        private void LoadAchievements()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM achievements";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAchievements.DataSource = dt;
                    dgvAchievements.Columns["achievement_id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading achievements: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Achievement name cannot be empty.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO achievements (name, description) VALUES (@name, @description)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.ExecuteNonQuery();
                    LoadAchievements();
                    txtName.Clear();
                    txtDescription.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding achievement: " + ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAchievements.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvAchievements.SelectedRows[0].Cells["achievement_id"].Value);
            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Achievement name cannot be empty.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE achievements SET name = @name, description = @description WHERE achievement_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    LoadAchievements();
                    txtName.Clear();
                    txtDescription.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating achievement: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAchievements.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvAchievements.SelectedRows[0].Cells["achievement_id"].Value);
            if (MessageBox.Show("Are you sure you want to delete this achievement?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = DBHelper.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM achievements WHERE achievement_id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        LoadAchievements();
                        txtName.Clear();
                        txtDescription.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting achievement: " + ex.Message);
                }
            }
        }

        private void dgvAchievements_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtName.Text = dgvAchievements.Rows[e.RowIndex].Cells["achievement_name"].Value.ToString();
                txtDescription.Text = dgvAchievements.Rows[e.RowIndex].Cells["description"].Value.ToString();
            }
        }   
    }
}

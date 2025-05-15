using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class ManageAchievementsControl : UserControl
    {
        private int selectedAchievementId = -1;

        public ManageAchievementsControl()
        {
            InitializeComponent();
            LoadAchievements();
            ConfigureDataGridView();
            ClearInputs();
        }

        private void ConfigureDataGridView()
        {
            // Set properties for better UX
            dgvAchievements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAchievements.MultiSelect = false;
            dgvAchievements.ReadOnly = true;
            dgvAchievements.AllowUserToAddRows = false;
            dgvAchievements.AllowUserToDeleteRows = false;
            dgvAchievements.AllowUserToResizeRows = false;
            dgvAchievements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAchievements.RowHeadersVisible = false;
            dgvAchievements.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
        }

        private void LoadAchievements()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM achievements ORDER BY achievement_name";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAchievements.DataSource = dt;

                    // Hide the achievement_id column
                    if (dgvAchievements.Columns["achievement_id"] != null)
                        dgvAchievements.Columns["achievement_id"].Visible = false;

                    // Format column headers
                    if (dgvAchievements.Columns["achievement_name"] != null)
                        dgvAchievements.Columns["achievement_name"].HeaderText = "Achievement";

                    if (dgvAchievements.Columns["description"] != null)
                        dgvAchievements.Columns["description"].HeaderText = "Description";

                    if (dgvAchievements.Columns["badge_image"] != null)
                        dgvAchievements.Columns["badge_image"].HeaderText = "Badge Image Path";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading achievements: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputs()
        {
            txtName.Clear();
            txtDescription.Clear();
            txtImagePath.Clear();
            selectedAchievementId = -1;
            btnAdd.Enabled = true;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            txtName.Focus();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text.Trim()))
            {
                MessageBox.Show("Achievement name cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return false;
            }
            return true;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();
            string imagePath = txtImagePath.Text.Trim();

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO achievements (achievement_name, description, badge_image) VALUES (@name, @description, @badge_image)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@badge_image", string.IsNullOrWhiteSpace(imagePath) ? (object)DBNull.Value : imagePath);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Achievement added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadAchievements();
                    ClearInputs();
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate key error
                {
                    MessageBox.Show("An achievement with this name already exists!", "Duplicate Entry",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding achievement: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (selectedAchievementId < 0 || !ValidateInputs()) return;

            string name = txtName.Text.Trim();
            string description = txtDescription.Text.Trim();
            string imagePath = txtImagePath.Text.Trim();

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"UPDATE achievements 
                                    SET achievement_name = @name, 
                                        description = @description, 
                                        badge_image = @badge_image 
                                    WHERE achievement_id = @id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@description", description);
                    cmd.Parameters.AddWithValue("@badge_image", string.IsNullOrWhiteSpace(imagePath) ? (object)DBNull.Value : imagePath);
                    cmd.Parameters.AddWithValue("@id", selectedAchievementId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Achievement updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadAchievements();
                    ClearInputs();
                }
            }
            catch (MySqlException ex)
            {
                if (ex.Number == 1062) // Duplicate key error
                {
                    MessageBox.Show("An achievement with this name already exists!", "Duplicate Entry",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating achievement: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedAchievementId < 0) return;

            // Check if the achievement is already assigned to users
            bool isAssigned = false;
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string checkQuery = "SELECT COUNT(*) FROM userachievements WHERE achievement_id = @id";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", selectedAchievementId);
                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    isAssigned = count > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking achievement usage: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string warningMessage = isAssigned
                ? "This achievement has been assigned to users. Deleting it will remove all user achievements. Are you sure you want to delete this achievement?"
                : "Are you sure you want to delete this achievement?";

            if (MessageBox.Show(warningMessage, "Confirm Delete",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = DBHelper.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM achievements WHERE achievement_id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", selectedAchievementId);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Achievement deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadAchievements();
                        ClearInputs();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting achievement: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvAchievements_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedAchievementId = Convert.ToInt32(dgvAchievements.Rows[e.RowIndex].Cells["achievement_id"].Value);
                txtName.Text = dgvAchievements.Rows[e.RowIndex].Cells["achievement_name"].Value.ToString();
                txtDescription.Text = dgvAchievements.Rows[e.RowIndex].Cells["description"].Value?.ToString() ?? "";
                txtImagePath.Text = dgvAchievements.Rows[e.RowIndex].Cells["badge_image"].Value?.ToString() ?? "";

                btnAdd.Enabled = false;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select Badge Image";
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtImagePath.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearInputs();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            string imagePath = txtImagePath.Text.Trim();

            if (string.IsNullOrWhiteSpace(imagePath))
            {
                MessageBox.Show("No image path specified.", "Preview",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!File.Exists(imagePath))
            {
                MessageBox.Show("The specified image file does not exist.", "File Not Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Create a simple form to display the image
                using (Form previewForm = new Form())
                {
                    previewForm.Text = "Badge Image Preview";
                    previewForm.Size = new Size(400, 400);
                    previewForm.StartPosition = FormStartPosition.CenterParent;
                    previewForm.MinimizeBox = false;
                    previewForm.MaximizeBox = false;
                    previewForm.FormBorderStyle = FormBorderStyle.FixedDialog;

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.Dock = DockStyle.Fill;
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox.Image = Image.FromFile(imagePath);

                    previewForm.Controls.Add(pictureBox);
                    previewForm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error previewing image: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();

            try
            {
                if (dgvAchievements.DataSource is DataTable dataTable)
                {
                    DataView dv = dataTable.DefaultView;
                    dv.RowFilter = string.IsNullOrEmpty(searchText)
                        ? ""
                        : $"achievement_name LIKE '%{searchText}%' OR description LIKE '%{searchText}%'";
                }
            }
            catch (Exception ex)
            {
                // Just ignore filtering errors - they're usually due to special characters in the search
                txtSearch.BackColor = Color.MistyRose;
            }
            finally
            {
                if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
                {
                    txtSearch.BackColor = SystemColors.Window;
                }
            }
        }
    }
}
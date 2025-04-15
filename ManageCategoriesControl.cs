using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class ManageCategoriesControl : UserControl
    {
        public ManageCategoriesControl()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT * FROM categories";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvCategories.DataSource = dt;
                    dgvCategories.Columns["category_id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string name = txtCategoryName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Category name cannot be empty.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO categories (category_name) VALUES (@name)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.ExecuteNonQuery();
                    LoadCategories();
                    txtCategoryName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding category: " + ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["category_id"].Value);
            string name = txtCategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Category name cannot be empty.");
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "UPDATE categories SET name = @name WHERE category_id = @id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                    LoadCategories();
                    txtCategoryName.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error editing category: " + ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["category_id"].Value);
            if (MessageBox.Show("Are you sure you want to delete this category?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = DBHelper.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM categories WHERE category_id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                        LoadCategories();
                        txtCategoryName.Clear();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting category: " + ex.Message);
                }
            }
        }

        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtCategoryName.Text = dgvCategories.Rows[e.RowIndex].Cells["category_name"].Value.ToString();
            }
        }
    }
}

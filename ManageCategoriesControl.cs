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
            dgvCategories.DataBindingComplete += dgvCategories_DataBindingComplete;
            LoadCategories();
        }


        private void LoadCategories()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT category_id, category_name, " +
                                  "(SELECT COUNT(*) FROM quizzes WHERE quizzes.category_id = categories.category_id) AS quiz_count " +
                                  "FROM categories ORDER BY category_name";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dgvCategories.AutoGenerateColumns = true;
                    dgvCategories.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvCategories_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Safe to modify columns here
            if (dgvCategories.Columns.Contains("category_id"))
                dgvCategories.Columns["category_id"].Visible = false;

            if (dgvCategories.Columns.Contains("category_name"))
            {
                dgvCategories.Columns["category_name"].HeaderText = "Category Name";
                dgvCategories.Columns["category_name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            if (dgvCategories.Columns.Contains("quiz_count"))
            {
                dgvCategories.Columns["quiz_count"].HeaderText = "Number of Quizzes";
                dgvCategories.Columns["quiz_count"].Width = 150;
            }
        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (CategoryForm form = new CategoryForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int categoryId = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["category_id"].Value);
            string categoryName = dgvCategories.SelectedRows[0].Cells["category_name"].Value.ToString();

            using (CategoryForm form = new CategoryForm(categoryId, categoryName))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadCategories();
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a category to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int categoryId = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["category_id"].Value);
            string categoryName = dgvCategories.SelectedRows[0].Cells["category_name"].Value.ToString();
            int quizCount = Convert.ToInt32(dgvCategories.SelectedRows[0].Cells["quiz_count"].Value);

            if (quizCount > 0)
            {
                MessageBox.Show($"Cannot delete category '{categoryName}' because it contains {quizCount} quiz(es).\n\n" +
                                "Please reassign or delete the quizzes first.",
                                "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete the category '{categoryName}'?",
                               "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection conn = DBHelper.GetConnection())
                    {
                        conn.Open();
                        string query = "DELETE FROM categories WHERE category_id = @id";
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@id", categoryId);
                        cmd.ExecuteNonQuery();

                        LoadCategories();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting category: " + ex.Message, "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }
    }
}
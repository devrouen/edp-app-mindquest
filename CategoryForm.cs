using System;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class CategoryForm : Form
    {
        private int categoryId;
        private bool isEditMode;

        // Constructor for Add mode
        public CategoryForm()
        {
            InitializeComponent();
            this.isEditMode = false;
            this.Text = "Add New Category";
        }

        // Constructor for Edit mode
        public CategoryForm(int categoryId, string categoryName)
        {
            InitializeComponent();
            this.categoryId = categoryId;
            this.isEditMode = true;
            this.Text = "Edit Category";
            txtCategoryName.Text = categoryName;
        }

        private void CategoryForm_Load(object sender, EventArgs e)
        {
            // Set focus to the textbox
            txtCategoryName.Focus();

            // If in edit mode, select all text for easy editing
            if (isEditMode)
            {
                txtCategoryName.SelectAll();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string categoryName = txtCategoryName.Text.Trim();

            // Validate inputs
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("Please enter a category name.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCategoryName.Focus();
                return;
            }

            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    // Check if category name already exists (except for the current category in edit mode)
                    string checkQuery = "SELECT COUNT(*) FROM categories WHERE category_name = @name";
                    if (isEditMode)
                    {
                        checkQuery += " AND category_id != @id";
                    }

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@name", categoryName);
                    if (isEditMode)
                    {
                        checkCmd.Parameters.AddWithValue("@id", categoryId);
                    }

                    int existingCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (existingCount > 0)
                    {
                        MessageBox.Show($"A category with the name '{categoryName}' already exists.",
                            "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCategoryName.Focus();
                        txtCategoryName.SelectAll();
                        return;
                    }

                    // Perform the appropriate operation (insert or update)
                    if (isEditMode)
                    {
                        string updateQuery = "UPDATE categories SET category_name = @name WHERE category_id = @id";
                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                        updateCmd.Parameters.AddWithValue("@name", categoryName);
                        updateCmd.Parameters.AddWithValue("@id", categoryId);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO categories (category_name) VALUES (@name)";
                        MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                        insertCmd.Parameters.AddWithValue("@name", categoryName);
                        insertCmd.ExecuteNonQuery();
                    }

                    // Close the form with success result
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                string operation = isEditMode ? "updating" : "adding";
                MessageBox.Show($"Error {operation} category: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace mindquest
{
    public partial class editQuizForm : Form
    {
        private string adminUsername;
        private int userId;
        private int quizId;
        private string connectionString = "server=localhost;user=root;password=12345678;database=mindquest;";

        public editQuizForm(string adminUsername, int quizId)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
            this.quizId = quizId;
        }

        private void editQuizForm_Load(object sender, EventArgs e)
        {
            // Get user_id for the admin username
            GetUserId();

            // Load categories
            LoadCategories();

            // Load quiz details
            LoadQuizDetails();
        }

        private void GetUserId()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT user_id FROM users WHERE username = @username";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@username", adminUsername);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                    else
                    {
                        MessageBox.Show("Admin user not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.DialogResult = DialogResult.Cancel;
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT category_id, category_name FROM categories ORDER BY category_name";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        cmbCategory.Items.Clear();
                        cmbCategory.DisplayMember = "Text";
                        cmbCategory.ValueMember = "Value";

                        while (reader.Read())
                        {
                            cmbCategory.Items.Add(new { Text = reader["category_name"].ToString(), Value = reader["category_id"] });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadQuizDetails()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Get quiz details
                    string quizQuery = @"
                        SELECT title, description, category_id
                        FROM quizzes
                        WHERE quiz_id = @quizId";

                    MySqlCommand cmd = new MySqlCommand(quizQuery, conn);
                    cmd.Parameters.AddWithValue("@quizId", quizId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtTitle.Text = reader["title"].ToString();
                            txtDescription.Text = reader["description"].ToString();
                            int categoryId = Convert.ToInt32(reader["category_id"]);

                            // Select the correct category in the combo box
                            for (int i = 0; i < cmbCategory.Items.Count; i++)
                            {
                                dynamic item = cmbCategory.Items[i];
                                if (Convert.ToInt32(item.Value) == categoryId)
                                {
                                    cmbCategory.SelectedIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Quiz not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            this.DialogResult = DialogResult.Cancel;
                            this.Close();
                        }
                    }

                    // Get question count
                    string countQuery = "SELECT COUNT(*) FROM questions WHERE quiz_id = @quizId";
                    cmd = new MySqlCommand(countQuery, conn);
                    cmd.Parameters.AddWithValue("@quizId", quizId);
                    int questionCount = Convert.ToInt32(cmd.ExecuteScalar());

                    lblQuestionCount.Text = $"Number of Questions: {questionCount}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading quiz details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveQuizChanges();
            }
        }

        private bool ValidateForm()
        {
            // Validate quiz details
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a quiz title.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus();
                return false;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return false;
            }

            return true;
        }

        private void SaveQuizChanges()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Get category ID
                        dynamic selectedCategory = cmbCategory.SelectedItem;
                        int categoryId = Convert.ToInt32(selectedCategory.Value);

                        // Update quiz
                        string updateQuizQuery = @"
                            UPDATE quizzes 
                            SET category_id = @categoryId, title = @title, description = @description
                            WHERE quiz_id = @quizId";

                        MySqlCommand cmd = new MySqlCommand(updateQuizQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@categoryId", categoryId);
                        cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                        cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());
                        cmd.Parameters.AddWithValue("@quizId", quizId);

                        cmd.ExecuteNonQuery();

                        // Create log entry
                        string logQuery = "INSERT INTO quiz_logs (quiz_id, user_id, action) VALUES (@quizId, @userId, 'updated')";
                        cmd = new MySqlCommand(logQuery, conn, transaction);
                        cmd.Parameters.AddWithValue("@quizId", quizId);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.ExecuteNonQuery();

                        transaction.Commit();
                        MessageBox.Show("Quiz updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quiz: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
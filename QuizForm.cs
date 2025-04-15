using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using mindquest;

namespace MindQuest
{
    public partial class QuizForm : Form
    {
        private int quizId = -1;
        private bool isEditMode = false;
        private string adminUsername; // Store the passed admin username

        // Constructor for creating a new quiz (with username)
        public QuizForm(string username)
        {
            InitializeComponent();
            this.adminUsername = username; // Store the username
        }

        // Constructor for editing an existing quiz (with existing quiz data)
        public QuizForm(int quizId, string title, string description, int categoryId, string username)
        {
            InitializeComponent();
            this.quizId = quizId;
            this.adminUsername = username; // Store the username
            txtTitle.Text = title;
            txtDescription.Text = description;
            isEditMode = true;
        }

        private void QuizForm_Load(object sender, EventArgs e)
        {
            LoadCategories();
            if (isEditMode)
            {
                SelectCurrentCategory();
            }
        }

        private void LoadCategories()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = "SELECT category_id, category_name FROM categories";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        cmbCategory.DataSource = dt;
                        cmbCategory.DisplayMember = "category_name";
                        cmbCategory.ValueMember = "category_id";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading categories: " + ex.Message);
            }
        }

        private void SelectCurrentCategory()
        {
            for (int i = 0; i < cmbCategory.Items.Count; i++)
            {
                DataRowView row = cmbCategory.Items[i] as DataRowView;
                if (Convert.ToInt32(row["category_id"]) == quizId)
                {
                    cmbCategory.SelectedIndex = i;
                    break;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string description = txtDescription.Text.Trim();
            int categoryId = Convert.ToInt32(cmbCategory.SelectedValue);

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Title is required.");
                return;
            }

            try
            {
                // Get the user_id of the admin based on username
                int userId = GetUserIdFromUsername(adminUsername);

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = isEditMode
                        ? "UPDATE quizzes SET title = @title, description = @desc, category_id = @cat WHERE quiz_id = @id"
                        : "INSERT INTO quizzes (title, description, category_id, user_id) VALUES (@title, @desc, @cat, @user_id)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description);
                    cmd.Parameters.AddWithValue("@cat", categoryId);
                    cmd.Parameters.AddWithValue("@user_id", userId); // Pass the user_id here

                    if (isEditMode)
                        cmd.Parameters.AddWithValue("@id", quizId);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show(isEditMode ? "Quiz updated!" : "Quiz added!");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving quiz: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Helper method to get user_id from username
        private int GetUserIdFromUsername(string username)
        {
            int userId = 0;
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT user_id FROM users WHERE username = @username";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    userId = Convert.ToInt32(result);
                }
            }
            return userId;
        }
    }
}

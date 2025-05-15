using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class ManageQuizzesControl : UserControl
    {

        private string adminUsername;

        public ManageQuizzesControl(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
            LoadQuizzes();
        }

        public ManageQuizzesControl()
        {
            InitializeComponent();
            LoadQuizzes();
        }

        private void LoadQuizzes()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    // Join with categories to get category_name instead of category_id
                    string query = @"
                        SELECT 
                            q.quiz_id AS 'Quiz ID',
                            q.title AS 'Title',
                            q.description AS 'Description',
                            c.category_name AS 'Category',
                            q.category_id
                        FROM quizzes q
                        INNER JOIN categories c ON q.category_id = c.category_id";


                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvQuizzes.DataSource = dt;
                    if (dgvQuizzes.Columns.Contains("category_id"))
                        dgvQuizzes.Columns["category_id"].Visible = false;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load quizzes: " + ex.Message);
            }
        }

        private void btnAddQuiz_Click(object sender, EventArgs e)
        {
            addQuizForm quizForm = new addQuizForm(adminUsername); // Pass the username to the QuizForm
            if (quizForm.ShowDialog() == DialogResult.OK)
                LoadQuizzes();
        }


        private void btnEditQuiz_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                int quizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["Quiz ID"].Value);

                // Create and show the new editQuizForm
                editQuizForm editForm = new editQuizForm(adminUsername, quizId);
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Reload quizzes to reflect changes
                    LoadQuizzes();
                }
            }
            else
            {
                MessageBox.Show("Please select a quiz to edit.");
            }
        }


        private void btnDeleteQuiz_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                int quizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["Quiz ID"].Value);

                DialogResult confirm = MessageBox.Show("Are you sure you want to delete this quiz?", "Confirm Delete", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        using (MySqlConnection conn = DBHelper.GetConnection())
                        {
                            conn.Open();
                            string query = "DELETE FROM quizzes WHERE quiz_id = @id";
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@id", quizId);
                            cmd.ExecuteNonQuery();
                        }

                        LoadQuizzes();
                        MessageBox.Show("Quiz deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to delete quiz: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a quiz to delete.");
            }
        }

    }
}

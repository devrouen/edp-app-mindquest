using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using mindquest;

namespace mindquest
{
    public partial class userDashboard : Form
    {
        private int userId;
        private string username;

        public userDashboard(int userId, string username)
        {
            InitializeComponent();
            this.userId = userId;
            this.username = username;
        }

        private void userDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {username}!";
            LoadQuizzes();
        }

        private void LoadQuizzes()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT q.quiz_id, q.title AS 'Title', c.category_name AS 'Category', q.description AS 'Description'
                        FROM quizzes q
                        JOIN categories c ON q.category_id = c.category_id
                    ";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvQuizzes.DataSource = dt;
                    dgvQuizzes.Columns["quiz_id"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading quizzes: " + ex.Message);
            }
        }

        private void btnTakeQuiz_Click(object sender, EventArgs e)
        {
            if (dgvQuizzes.SelectedRows.Count > 0)
            {
                int selectedQuizId = Convert.ToInt32(dgvQuizzes.SelectedRows[0].Cells["quiz_id"].Value);
                string quizTitle = dgvQuizzes.SelectedRows[0].Cells["Title"].Value.ToString();

                MessageBox.Show($"Starting quiz: {quizTitle} (ID: {selectedQuizId})");
                // new QuizForm(selectedQuizId, userId).Show(); this.Hide();
            }
            else
            {
                MessageBox.Show("Please select a quiz first.");
            }
        }

        private void btnViewAttempts_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature coming soon: View attempts!");
        }
    }
}

using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class ViewAttemptsControl : UserControl
    {
        public ViewAttemptsControl()
        {
            InitializeComponent();
        }

        private void ViewAttemptsControl_Load(object sender, EventArgs e)
        {
            LoadAttempts();
        }

        private void LoadAttempts()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            qa.quiz_attempt_id AS 'Attempt ID',
                            u.username AS 'Username',
                            q.title AS 'Quiz Title',
                            qa.attempt_date AS 'Date Taken',
                            qa.score AS 'Score'
                        FROM quizattempts qa
                        INNER JOIN users u ON qa.user_id = u.user_id
                        INNER JOIN quizzes q ON qa.quiz_id = q.quiz_id
                        ORDER BY qa.attempt_date DESC;
                    ";

                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAttempts.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load quiz attempts:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}

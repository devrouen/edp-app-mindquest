// QuizForm.cs
using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using mindquest;

namespace MindQuest
{
    public partial class QuizForm : Form
    {
        private int quizId = -1;
        private bool isEditMode = false;
        private string adminUsername;
        private List<QuestionItem> questions = new List<QuestionItem>();

        public QuizForm(string username)
        {
            InitializeComponent();
            this.adminUsername = username;
        }

        public QuizForm(int quizId, string title, string description, int categoryId, string username)
        {
            InitializeComponent();
            this.quizId = quizId;
            this.adminUsername = username;
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
                int userId = GetUserIdFromUsername(adminUsername);

                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    if (isEditMode)
                    {
                        string updateQuery = "UPDATE quizzes SET title = @title, description = @desc, category_id = @cat WHERE quiz_id = @id";
                        MySqlCommand cmd = new MySqlCommand(updateQuery, conn);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@cat", categoryId);
                        cmd.Parameters.AddWithValue("@id", quizId);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        string insertQuery = "INSERT INTO quizzes (title, description, category_id, user_id) VALUES (@title, @desc, @cat, @user_id)";
                        MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@desc", description);
                        cmd.Parameters.AddWithValue("@cat", categoryId);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.ExecuteNonQuery();
                        quizId = (int)cmd.LastInsertedId;
                    }

                    SaveQuestionsAndAnswers(conn);
                }

                MessageBox.Show(isEditMode ? "Quiz updated!" : "Quiz added!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving quiz: " + ex.Message);
            }
        }

        private void SaveQuestionsAndAnswers(MySqlConnection conn)
        {
            foreach (var q in questions)
            {
                string questionQuery = "INSERT INTO questions (quiz_id, question_type, question_text, points) VALUES (@quizId, @type, @text, @points)";
                MySqlCommand cmd = new MySqlCommand(questionQuery, conn);
                cmd.Parameters.AddWithValue("@quizId", quizId);
                cmd.Parameters.AddWithValue("@type", q.QuestionType);
                cmd.Parameters.AddWithValue("@text", q.QuestionText);
                cmd.Parameters.AddWithValue("@points", q.Points);
                cmd.ExecuteNonQuery();

                int questionId = (int)cmd.LastInsertedId;

                foreach (var a in q.Answers)
                {
                    string answerQuery = "INSERT INTO answers (question_id, answer_text, is_correct) VALUES (@qid, @text, @correct)";
                    MySqlCommand aCmd = new MySqlCommand(answerQuery, conn);
                    aCmd.Parameters.AddWithValue("@qid", questionId);
                    aCmd.Parameters.AddWithValue("@text", a.AnswerText);
                    aCmd.Parameters.AddWithValue("@correct", a.IsCorrect);
                    aCmd.ExecuteNonQuery();
                }
            }
        }

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

    public class QuestionItem
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int Points { get; set; }
        public List<AnswerItem> Answers { get; set; } = new List<AnswerItem>();
    }

    public class AnswerItem
    {
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; }
    }
}

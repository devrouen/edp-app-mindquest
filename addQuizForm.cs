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
    public partial class addQuizForm : Form
    {
        private string adminUsername;
        private int userId;
        private List<QuestionPanel> questionPanels = new List<QuestionPanel>();
        private string connectionString = "server=localhost;user=root;password=12345678;database=mindquest;";

        public addQuizForm(string adminUsername)
        {
            InitializeComponent();
            this.adminUsername = adminUsername;
        }

        private void addQuizForm_Load(object sender, EventArgs e)
        {
            // Get user_id for the admin username
            GetUserId();

            // Load categories
            LoadCategories();
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

                        if (cmbCategory.Items.Count > 0)
                            cmbCategory.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddQuestion_Click(object sender, EventArgs e)
        {
            AddQuestionPanel();
        }

        private void AddQuestionPanel()
        {
            QuestionPanel panel = new QuestionPanel(questionPanels.Count + 1);
            panel.OnRemoveQuestion += Panel_OnRemoveQuestion;
            questionPanels.Add(panel);
            pnlQuestions.Controls.Add(panel);
            panel.Width = pnlQuestions.Width - 30;
        }

        private void Panel_OnRemoveQuestion(QuestionPanel panel)
        {
            questionPanels.Remove(panel);
            pnlQuestions.Controls.Remove(panel);

            // Renumber remaining questions
            for (int i = 0; i < questionPanels.Count; i++)
            {
                questionPanels[i].UpdateQuestionNumber(i + 1);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                SaveQuizAndQuestions();
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

            // Validate questions
            if (questionPanels.Count == 0)
            {
                MessageBox.Show("Please add at least one question to the quiz.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnAddQuestion.Focus();
                return false;
            }

            // Validate each question
            foreach (QuestionPanel panel in questionPanels)
            {
                if (!panel.ValidateQuestion())
                {
                    return false;
                }
            }

            return true;
        }

        private void SaveQuizAndQuestions()
        {
            MySqlConnection conn = null;
            MySqlTransaction transaction = null;

            try
            {
                conn = new MySqlConnection(connectionString);
                conn.Open();
                transaction = conn.BeginTransaction();

                // Get category ID
                dynamic selectedCategory = cmbCategory.SelectedItem;
                int categoryId = Convert.ToInt32(selectedCategory.Value);

                // Insert quiz
                string insertQuizQuery = @"
                    INSERT INTO quizzes (user_id, category_id, title, description) 
                    VALUES (@userId, @categoryId, @title, @description);
                    SELECT LAST_INSERT_ID();";

                MySqlCommand cmd = new MySqlCommand(insertQuizQuery, conn, transaction);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@categoryId", categoryId);
                cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                cmd.Parameters.AddWithValue("@description", txtDescription.Text.Trim());

                int quizId = Convert.ToInt32(cmd.ExecuteScalar());

                // Insert questions and answers
                foreach (QuestionPanel panel in questionPanels)
                {
                    panel.SaveQuestion(conn, transaction, quizId);
                }

                // Create log entry
                string logQuery = "INSERT INTO quiz_logs (quiz_id, user_id, action) VALUES (@quizId, @userId, 'created')";
                cmd = new MySqlCommand(logQuery, conn, transaction);
                cmd.Parameters.AddWithValue("@quizId", quizId);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.ExecuteNonQuery();

                transaction.Commit();
                MessageBox.Show("Quiz saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                transaction?.Rollback();
                MessageBox.Show($"Error saving quiz: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn?.Close();
            }
        }
    }

    // Custom panel for questions
    public class QuestionPanel : Panel
    {
        public event Action<QuestionPanel> OnRemoveQuestion;

        private Label lblQuestionNumber;
        private TextBox txtQuestion;
        private Label lblPoints;
        private NumericUpDown numPoints;
        private Label lblQuestionType;
        private ComboBox cmbQuestionType;
        private FlowLayoutPanel pnlAnswers;
        private Button btnAddAnswer;
        private Button btnRemove;

        private List<AnswerPanel> answerPanels = new List<AnswerPanel>();
        private int questionNumber;

        public QuestionPanel(int questionNumber)
        {
            this.questionNumber = questionNumber;
            InitializeControls();
        }

        private void InitializeControls()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(0, 0, 0, 10);
            this.Size = new Size(720, 245);
            this.Padding = new Padding(10);

            // Question number label
            lblQuestionNumber = new Label();
            lblQuestionNumber.AutoSize = true;
            lblQuestionNumber.Font = new Font(lblQuestionNumber.Font, FontStyle.Bold);
            lblQuestionNumber.Text = $"Question {questionNumber}";
            lblQuestionNumber.Location = new Point(10, 10);
            this.Controls.Add(lblQuestionNumber);

            // Remove button
            btnRemove = new Button();
            btnRemove.Text = "Remove";
            btnRemove.Size = new Size(80, 28);
            btnRemove.Location = new Point(620, 5);
            btnRemove.Click += BtnRemove_Click;
            this.Controls.Add(btnRemove);

            // Question text
            txtQuestion = new TextBox();
            txtQuestion.Multiline = true;
            txtQuestion.Size = new Size(510, 50);
            txtQuestion.Location = new Point(10, 35);
            this.Controls.Add(txtQuestion);

            // Question type
            lblQuestionType = new Label();
            lblQuestionType.AutoSize = true;
            lblQuestionType.Text = "Type:";
            lblQuestionType.Location = new Point(10, 95);
            this.Controls.Add(lblQuestionType);

            cmbQuestionType = new ComboBox();
            cmbQuestionType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbQuestionType.Size = new Size(200, 24);
            cmbQuestionType.Location = new Point(60, 92);
            cmbQuestionType.Items.AddRange(new object[] {
                "multiple_choice",
                "true_false",
                "identification",
                "multiple_answers"
            });
            cmbQuestionType.SelectedIndex = 0;
            cmbQuestionType.SelectedIndexChanged += CmbQuestionType_SelectedIndexChanged;
            this.Controls.Add(cmbQuestionType);

            // Points
            lblPoints = new Label();
            lblPoints.AutoSize = true;
            lblPoints.Text = "Points:";
            lblPoints.Location = new Point(280, 95);
            this.Controls.Add(lblPoints);

            numPoints = new NumericUpDown();
            numPoints.Minimum = 1;
            numPoints.Maximum = 100;
            numPoints.Value = 1;
            numPoints.Size = new Size(80, 22);
            numPoints.Location = new Point(330, 92);
            this.Controls.Add(numPoints);

            // Answers panel
            pnlAnswers = new FlowLayoutPanel();
            pnlAnswers.FlowDirection = FlowDirection.TopDown;
            pnlAnswers.AutoScroll = true;
            pnlAnswers.WrapContents = false;
            pnlAnswers.Size = new Size(540, 100);
            pnlAnswers.Location = new Point(10, 125);
            pnlAnswers.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pnlAnswers);

            // Add answer button
            btnAddAnswer = new Button();
            btnAddAnswer.Text = "Add Answer";
            btnAddAnswer.Size = new Size(100, 28);
            btnAddAnswer.Location = new Point(10, 230);
            btnAddAnswer.Click += BtnAddAnswer_Click;
            this.Controls.Add(btnAddAnswer);

            // Add initial answers based on default question type
            SetupAnswersForQuestionType("multiple_choice");
        }

        private void CmbQuestionType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string questionType = cmbQuestionType.SelectedItem.ToString();
            SetupAnswersForQuestionType(questionType);
        }

        private void SetupAnswersForQuestionType(string questionType)
        {
            // Clear existing answers
            foreach (AnswerPanel panel in answerPanels)
            {
                pnlAnswers.Controls.Remove(panel);
            }
            answerPanels.Clear();

            // Add appropriate answers based on question type
            switch (questionType)
            {
                case "multiple_choice":
                    btnAddAnswer.Visible = true;
                    // Add 4 options by default
                    for (int i = 0; i < 4; i++)
                    {
                        AddAnswerPanel(i == 0); // First one is correct by default
                    }
                    break;
                case "true_false":
                    btnAddAnswer.Visible = false;
                    // Add True and False options
                    AddAnswerPanel(true, "True");
                    AddAnswerPanel(false, "False");
                    break;
                case "identification":
                    btnAddAnswer.Visible = true;
                    // Add one correct answer option
                    AddAnswerPanel(true);
                    break;
                case "multiple_answers":
                    btnAddAnswer.Visible = true;
                    // Add 4 options by default, multiple can be marked correct
                    for (int i = 0; i < 4; i++)
                    {
                        AddAnswerPanel(false);
                    }
                    break;
            }
        }

        private void BtnAddAnswer_Click(object sender, EventArgs e)
        {
            AddAnswerPanel(false);
        }

        private void AddAnswerPanel(bool isCorrect, string initialText = "")
        {
            AnswerPanel panel = new AnswerPanel(answerPanels.Count + 1, isCorrect, initialText);
            panel.OnRemoveAnswer += Panel_OnRemoveAnswer;
            answerPanels.Add(panel);
            pnlAnswers.Controls.Add(panel);
            panel.Width = pnlAnswers.Width - 20;

            // Update panel height based on number of answers
            UpdatePanelHeight();
        }

        private void Panel_OnRemoveAnswer(AnswerPanel panel)
        {
            // Don't allow removing if it's the last answer or it's true/false
            if (cmbQuestionType.Text == "true_false" ||
                (answerPanels.Count <= 1 && (cmbQuestionType.Text == "multiple_choice" || cmbQuestionType.Text == "identification")))
            {
                MessageBox.Show("Cannot remove this answer. At least one answer is required.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            answerPanels.Remove(panel);
            pnlAnswers.Controls.Remove(panel);

            // Renumber remaining answers
            for (int i = 0; i < answerPanels.Count; i++)
            {
                answerPanels[i].UpdateAnswerNumber(i + 1);
            }

            // Update panel height based on number of answers
            UpdatePanelHeight();
        }

        private void UpdatePanelHeight()
        {
            int height = 245 + (answerPanels.Count > 3 ? (answerPanels.Count - 3) * 35 : 0);
            this.Size = new Size(this.Width, height);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            OnRemoveQuestion?.Invoke(this);
        }

        public void UpdateQuestionNumber(int newNumber)
        {
            this.questionNumber = newNumber;
            lblQuestionNumber.Text = $"Question {questionNumber}";
        }

        public bool ValidateQuestion()
        {
            // Validate question text
            if (string.IsNullOrWhiteSpace(txtQuestion.Text))
            {
                MessageBox.Show($"Please enter text for Question {questionNumber}.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuestion.Focus();
                return false;
            }

            // Validate answers
            if (answerPanels.Count == 0)
            {
                MessageBox.Show($"Please add at least one answer for Question {questionNumber}.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            // Validate each answer
            foreach (AnswerPanel panel in answerPanels)
            {
                if (!panel.ValidateAnswer())
                {
                    MessageBox.Show($"Please enter text for all answers in Question {questionNumber}.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            // Ensure at least one answer is marked as correct
            // (except for identification questions where we might allow no correct answers)
            string questionType = cmbQuestionType.SelectedItem.ToString();
            if (questionType != "identification")
            {
                bool hasCorrectAnswer = answerPanels.Any(a => a.IsCorrect);
                if (!hasCorrectAnswer)
                {
                    MessageBox.Show($"Please mark at least one answer as correct for Question {questionNumber}.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

            return true;
        }

        public void SaveQuestion(MySqlConnection conn, MySqlTransaction transaction, int quizId)
        {
            // Insert question
            string insertQuestionQuery = @"
                INSERT INTO questions (quiz_id, question_type, question_text, points) 
                VALUES (@quizId, @questionType, @questionText, @points);
                SELECT LAST_INSERT_ID();";

            MySqlCommand cmd = new MySqlCommand(insertQuestionQuery, conn, transaction);
            cmd.Parameters.AddWithValue("@quizId", quizId);
            cmd.Parameters.AddWithValue("@questionType", cmbQuestionType.Text);
            cmd.Parameters.AddWithValue("@questionText", txtQuestion.Text.Trim());
            cmd.Parameters.AddWithValue("@points", numPoints.Value);

            int questionId = Convert.ToInt32(cmd.ExecuteScalar());

            // Insert answers
            foreach (AnswerPanel panel in answerPanels)
            {
                panel.SaveAnswer(conn, transaction, questionId);
            }
        }
    }

    // Custom panel for answers
    public class AnswerPanel : Panel
    {
        public event Action<AnswerPanel> OnRemoveAnswer;

        private Label lblAnswerNumber;
        private TextBox txtAnswer;
        private CheckBox chkCorrect;
        private Button btnRemove;

        private int answerNumber;

        public bool IsCorrect => chkCorrect.Checked;

        public AnswerPanel(int answerNumber, bool isCorrect, string initialText = "")
        {
            this.answerNumber = answerNumber;
            InitializeControls(isCorrect, initialText);
        }

        private void InitializeControls(bool isCorrect, string initialText)
        {
            this.Size = new Size(680, 35);
            this.Margin = new Padding(0, 0, 0, 5);

            // Answer number label
            lblAnswerNumber = new Label();
            lblAnswerNumber.AutoSize = true;
            lblAnswerNumber.Text = $"{answerNumber}.";
            lblAnswerNumber.Location = new Point(5, 7);
            this.Controls.Add(lblAnswerNumber);

            // Answer text
            txtAnswer = new TextBox();
            txtAnswer.Size = new Size(400, 24);
            txtAnswer.Location = new Point(30, 5);
            txtAnswer.Text = initialText;
            this.Controls.Add(txtAnswer);

            // Correct checkbox
            chkCorrect = new CheckBox();
            chkCorrect.Text = "Correct";
            chkCorrect.AutoSize = true;
            chkCorrect.Checked = isCorrect;
            chkCorrect.Location = new Point(440, 6);
            this.Controls.Add(chkCorrect);

            // Remove button
            btnRemove = new Button();
            btnRemove.Text = "X";
            btnRemove.Size = new Size(25, 25);
            btnRemove.Location = new Point(650, 5);
            btnRemove.Click += BtnRemove_Click;
            this.Controls.Add(btnRemove);
        }

        private void BtnRemove_Click(object sender, EventArgs e)
        {
            OnRemoveAnswer?.Invoke(this);
        }

        public void UpdateAnswerNumber(int newNumber)
        {
            this.answerNumber = newNumber;
            lblAnswerNumber.Text = $"{answerNumber}.";
        }

        public bool ValidateAnswer()
        {
            return !string.IsNullOrWhiteSpace(txtAnswer.Text);
        }

        public void SaveAnswer(MySqlConnection conn, MySqlTransaction transaction, int questionId)
        {
            string insertAnswerQuery = @"
                INSERT INTO answers (question_id, answer_text, is_correct) 
                VALUES (@questionId, @answerText, @isCorrect)";

            MySqlCommand cmd = new MySqlCommand(insertAnswerQuery, conn, transaction);
            cmd.Parameters.AddWithValue("@questionId", questionId);
            cmd.Parameters.AddWithValue("@answerText", txtAnswer.Text.Trim());
            cmd.Parameters.AddWithValue("@isCorrect", chkCorrect.Checked);

            cmd.ExecuteNonQuery();
        }
    }
}
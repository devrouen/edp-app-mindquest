using System.Windows.Forms;
using System;

namespace MindQuest
{
    partial class QuizForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ListBox lstQuestions;
        private System.Windows.Forms.TextBox txtQuestionText;
        private System.Windows.Forms.ComboBox cmbQuestionType;
        private System.Windows.Forms.NumericUpDown numPoints;
        private System.Windows.Forms.DataGridView dgvAnswers;
        private System.Windows.Forms.Button btnAddQuestion;
        private System.Windows.Forms.Button btnSaveQuestion;
        private System.Windows.Forms.Button btnAddAnswer;
        private System.Windows.Forms.Button btnRemoveAnswer;
        private System.Windows.Forms.Label lblQuestionText;
        private System.Windows.Forms.Label lblQuestionType;
        private System.Windows.Forms.Label lblPoints;
        private System.Windows.Forms.Label lblAnswers;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtTitle = new TextBox();
            this.txtDescription = new TextBox();
            this.cmbCategory = new ComboBox();
            this.btnSave = new Button();
            this.btnCancel = new Button();
            this.lblTitle = new Label();
            this.lblDescription = new Label();
            this.lblCategory = new Label();
            this.txtQuestionText = new TextBox();
            this.cmbQuestionType = new ComboBox();
            this.numPoints = new NumericUpDown();
            this.dgvAnswers = new DataGridView();
            this.btnAddAnswer = new Button();
            this.btnRemoveAnswer = new Button();
            this.lblQuestionText = new Label();
            this.lblQuestionType = new Label();
            this.lblPoints = new Label();
            this.lblAnswers = new Label();

            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnswers)).BeginInit();
            this.SuspendLayout();

            // Labels
            lblTitle.Text = "Title:";
            lblTitle.Location = new System.Drawing.Point(30, 20);
            lblDescription.Text = "Description:";
            lblDescription.Location = new System.Drawing.Point(30, 60);
            lblCategory.Text = "Category:";
            lblCategory.Location = new System.Drawing.Point(30, 110);

            // Text Inputs
            txtTitle.Location = new System.Drawing.Point(150, 20);
            txtDescription.Location = new System.Drawing.Point(150, 60);
            txtDescription.Multiline = true;
            txtDescription.Height = 40;
            cmbCategory.Location = new System.Drawing.Point(150, 110);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            // Question Section
            lblQuestionText.Text = "Question Text:";
            lblQuestionText.Location = new System.Drawing.Point(30, 160);
            txtQuestionText.Location = new System.Drawing.Point(150, 160);
            txtQuestionText.Width = 400;

            lblQuestionType.Text = "Question Type:";
            lblQuestionType.Location = new System.Drawing.Point(30, 200);
            cmbQuestionType.Location = new System.Drawing.Point(150, 200);
            cmbQuestionType.Items.AddRange(new string[] { "multiple_choice", "true_false", "identification", "multiple_answers" });

            lblPoints.Text = "Points:";
            lblPoints.Location = new System.Drawing.Point(30, 240);
            numPoints.Location = new System.Drawing.Point(150, 240);

            lblAnswers.Text = "Answers:";
            lblAnswers.Location = new System.Drawing.Point(30, 280);
            dgvAnswers.Location = new System.Drawing.Point(150, 280);
            dgvAnswers.Size = new System.Drawing.Size(400, 120);
            dgvAnswers.Columns.Add("AnswerText", "Answer Text");
            dgvAnswers.Columns.Add(new DataGridViewCheckBoxColumn() { Name = "IsCorrect", HeaderText = "Correct" });

            btnAddAnswer.Text = "Add Answer";
            btnAddAnswer.Location = new System.Drawing.Point(150, 410);
            //btnAddAnswer.Click += new EventHandler(btnAddAnswer_Click);

            btnRemoveAnswer.Text = "Remove Answer";
            btnRemoveAnswer.Location = new System.Drawing.Point(250, 410);
            //btnRemoveAnswer.Click += new EventHandler(btnRemoveAnswer_Click);

            // Buttons
            btnSave.Text = "Save";
            btnSave.Location = new System.Drawing.Point(150, 460);
            btnSave.Click += new EventHandler(btnSave_Click);

            btnCancel.Text = "Cancel";
            btnCancel.Location = new System.Drawing.Point(250, 460);
            //btnCancel.Click += new EventHandler(btnCancel_Click);

            // Form setup
            this.Text = "Quiz Editor";
            this.ClientSize = new System.Drawing.Size(600, 520);
            this.Controls.AddRange(new Control[] {
        lblTitle, txtTitle, lblDescription, txtDescription, lblCategory, cmbCategory,
        lblQuestionText, txtQuestionText, lblQuestionType, cmbQuestionType,
        lblPoints, numPoints, lblAnswers, dgvAnswers, btnAddAnswer, btnRemoveAnswer,
        btnSave, btnCancel
    });

            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnswers)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    }
}

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
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblCategory = new System.Windows.Forms.Label();

            this.SuspendLayout();

            // lblTitle
            this.lblTitle.Text = "Title:";
            this.lblTitle.Location = new System.Drawing.Point(30, 30);

            // txtTitle
            this.txtTitle.Location = new System.Drawing.Point(150, 30);
            this.txtTitle.Width = 200;

            // lblDescription
            this.lblDescription.Text = "Description:";
            this.lblDescription.Location = new System.Drawing.Point(30, 70);

            // txtDescription
            this.txtDescription.Location = new System.Drawing.Point(150, 70);
            this.txtDescription.Width = 200;
            this.txtDescription.Height = 60;
            this.txtDescription.Multiline = true;

            // lblCategory
            this.lblCategory.Text = "Category:";
            this.lblCategory.Location = new System.Drawing.Point(30, 150);

            // cmbCategory
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Location = new System.Drawing.Point(150, 150);
            this.cmbCategory.Width = 200;

            // btnSave
            this.btnSave.Text = "Save";
            this.btnSave.Location = new System.Drawing.Point(150, 200);
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // btnCancel
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Location = new System.Drawing.Point(250, 200);
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // QuizForm
            this.ClientSize = new System.Drawing.Size(400, 270);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Text = "Quiz Details";
            this.Load += new System.EventHandler(this.QuizForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();




        }
    }
}

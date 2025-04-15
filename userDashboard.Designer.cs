using System.Windows.Forms;

namespace mindquest
{
    partial class userDashboard
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblWelcome = new System.Windows.Forms.Label();
            this.dgvQuizzes = new System.Windows.Forms.DataGridView();
            this.btnTakeQuiz = new System.Windows.Forms.Button();
            this.btnViewAttempts = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuizzes)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(30, 20);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(135, 25);
            this.lblWelcome.Text = "Welcome, {username}!";
            // 
            // dgvQuizzes
            // 
            this.dgvQuizzes.AllowUserToAddRows = false;
            this.dgvQuizzes.AllowUserToDeleteRows = false;
            this.dgvQuizzes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuizzes.Location = new System.Drawing.Point(30, 60);
            this.dgvQuizzes.Name = "dgvQuizzes";
            this.dgvQuizzes.ReadOnly = true;
            this.dgvQuizzes.Size = new System.Drawing.Size(500, 250);
            // 
            // btnTakeQuiz
            // 
            this.btnTakeQuiz.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnTakeQuiz.Location = new System.Drawing.Point(550, 60);
            this.btnTakeQuiz.Name = "btnTakeQuiz";
            this.btnTakeQuiz.Size = new System.Drawing.Size(120, 40);
            this.btnTakeQuiz.Text = "Take Quiz";
            this.btnTakeQuiz.UseVisualStyleBackColor = true;
            this.btnTakeQuiz.Click += new System.EventHandler(this.btnTakeQuiz_Click);
            // 
            // btnViewAttempts
            // 
            this.btnViewAttempts.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnViewAttempts.Location = new System.Drawing.Point(550, 110);
            this.btnViewAttempts.Name = "btnViewAttempts";
            this.btnViewAttempts.Size = new System.Drawing.Size(120, 40);
            this.btnViewAttempts.Text = "My Attempts";
            this.btnViewAttempts.UseVisualStyleBackColor = true;
            this.btnViewAttempts.Click += new System.EventHandler(this.btnViewAttempts_Click);
            // 
            // userDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.ClientSize = new System.Drawing.Size(700, 350);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.dgvQuizzes);
            this.Controls.Add(this.btnTakeQuiz);
            this.Controls.Add(this.btnViewAttempts);
            this.Name = "userDashboard";
            this.Text = "User Dashboard";
            this.Load += new System.EventHandler(this.userDashboard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuizzes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblWelcome;
        private DataGridView dgvQuizzes;
        private Button btnTakeQuiz;
        private Button btnViewAttempts;
    }
}

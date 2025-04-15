namespace MindQuest
{
    partial class ManageQuizzesControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvQuizzes;
        private System.Windows.Forms.Button btnAddQuiz;
        private System.Windows.Forms.Button btnEditQuiz;
        private System.Windows.Forms.Button btnDeleteQuiz;
        private System.Windows.Forms.Label lblHeader;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvQuizzes = new System.Windows.Forms.DataGridView();
            this.btnAddQuiz = new System.Windows.Forms.Button();
            this.btnEditQuiz = new System.Windows.Forms.Button();
            this.btnDeleteQuiz = new System.Windows.Forms.Button();
            this.lblHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuizzes)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvQuizzes
            // 
            this.dgvQuizzes.AllowUserToAddRows = false;
            this.dgvQuizzes.AllowUserToDeleteRows = false;
            this.dgvQuizzes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                        | System.Windows.Forms.AnchorStyles.Left)
                                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvQuizzes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvQuizzes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvQuizzes.Location = new System.Drawing.Point(20, 70);
            this.dgvQuizzes.Name = "dgvQuizzes";
            this.dgvQuizzes.ReadOnly = true;
            this.dgvQuizzes.RowHeadersWidth = 62;
            this.dgvQuizzes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvQuizzes.Size = new System.Drawing.Size(920, 380);
            this.dgvQuizzes.TabIndex = 0;
            // 
            // btnAddQuiz
            // 
            this.btnAddQuiz.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnAddQuiz.Location = new System.Drawing.Point(20, 470);
            this.btnAddQuiz.Name = "btnAddQuiz";
            this.btnAddQuiz.Size = new System.Drawing.Size(100, 30);
            this.btnAddQuiz.TabIndex = 1;
            this.btnAddQuiz.Text = "Add Quiz";
            this.btnAddQuiz.UseVisualStyleBackColor = true;
            this.btnAddQuiz.Click += new System.EventHandler(this.btnAddQuiz_Click);
            // 
            // btnEditQuiz
            // 
            this.btnEditQuiz.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnEditQuiz.Location = new System.Drawing.Point(130, 470);
            this.btnEditQuiz.Name = "btnEditQuiz";
            this.btnEditQuiz.Size = new System.Drawing.Size(100, 30);
            this.btnEditQuiz.TabIndex = 2;
            this.btnEditQuiz.Text = "Edit Quiz";
            this.btnEditQuiz.UseVisualStyleBackColor = true;
            this.btnEditQuiz.Click += new System.EventHandler(this.btnEditQuiz_Click);
            // 
            // btnDeleteQuiz
            // 
            this.btnDeleteQuiz.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.btnDeleteQuiz.Location = new System.Drawing.Point(240, 470);
            this.btnDeleteQuiz.Name = "btnDeleteQuiz";
            this.btnDeleteQuiz.Size = new System.Drawing.Size(100, 30);
            this.btnDeleteQuiz.TabIndex = 3;
            this.btnDeleteQuiz.Text = "Delete Quiz";
            this.btnDeleteQuiz.UseVisualStyleBackColor = true;
            this.btnDeleteQuiz.Click += new System.EventHandler(this.btnDeleteQuiz_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(20, 20);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(229, 38);
            this.lblHeader.TabIndex = 4;
            this.lblHeader.Text = "Manage Quizzes";
            // 
            // ManageQuizzesControl
            // 
            this.Controls.Add(this.dgvQuizzes);
            this.Controls.Add(this.btnAddQuiz);
            this.Controls.Add(this.btnEditQuiz);
            this.Controls.Add(this.btnDeleteQuiz);
            this.Controls.Add(this.lblHeader);
            this.Name = "ManageQuizzesControl";
            this.Size = new System.Drawing.Size(960, 520); // 🔧 Increased width/height
            ((System.ComponentModel.ISupportInitialize)(this.dgvQuizzes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

    }
}

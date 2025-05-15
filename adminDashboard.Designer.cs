namespace MindQuest
{
    partial class AdminDashboard
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelMainContent;
        private System.Windows.Forms.Button btnManageQuizzes;
        private System.Windows.Forms.Button btnManageCategories;
        private System.Windows.Forms.Button btnManageAchievements;
        private System.Windows.Forms.Button btnViewAttempts;
        private System.Windows.Forms.Button btnViewLogs;
        private System.Windows.Forms.Button btnManageUsers;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblWelcome;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnViewLogs = new System.Windows.Forms.Button();
            this.btnViewAttempts = new System.Windows.Forms.Button();
            this.btnManageCategories = new System.Windows.Forms.Button();
            this.btnManageQuizzes = new System.Windows.Forms.Button();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnManageAchievements = new System.Windows.Forms.Button();
            this.btnManageUsers = new System.Windows.Forms.Button();
            this.panelMainContent = new System.Windows.Forms.Panel();
            this.panelSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.SteelBlue;
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.Controls.Add(this.btnViewLogs);
            this.panelSidebar.Controls.Add(this.btnViewAttempts);
            this.panelSidebar.Controls.Add(this.btnManageCategories);
            this.panelSidebar.Controls.Add(this.btnManageQuizzes);
            this.panelSidebar.Controls.Add(this.lblWelcome);
            this.panelSidebar.Controls.Add(this.btnManageAchievements);
            this.panelSidebar.Controls.Add(this.btnManageUsers);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 0);
            this.panelSidebar.Margin = new System.Windows.Forms.Padding(4);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(257, 748);
            this.panelSidebar.TabIndex = 0;
            // 
            // btnLogout
            // 
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(15, 587);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(4);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(225, 47);
            this.btnLogout.TabIndex = 5;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnViewLogs
            // 
            this.btnViewLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewLogs.ForeColor = System.Drawing.Color.White;
            this.btnViewLogs.Location = new System.Drawing.Point(15, 260);
            this.btnViewLogs.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewLogs.Name = "btnViewLogs";
            this.btnViewLogs.Size = new System.Drawing.Size(225, 47);
            this.btnViewLogs.TabIndex = 4;
            this.btnViewLogs.Text = "View Logs";
            this.btnViewLogs.UseVisualStyleBackColor = true;
            this.btnViewLogs.Click += new System.EventHandler(this.btnViewLogs_Click);
            // 
            // btnViewAttempts
            // 
            this.btnViewAttempts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewAttempts.ForeColor = System.Drawing.Color.White;
            this.btnViewAttempts.Location = new System.Drawing.Point(15, 200);
            this.btnViewAttempts.Margin = new System.Windows.Forms.Padding(4);
            this.btnViewAttempts.Name = "btnViewAttempts";
            this.btnViewAttempts.Size = new System.Drawing.Size(225, 47);
            this.btnViewAttempts.TabIndex = 3;
            this.btnViewAttempts.Text = "View Attempts";
            this.btnViewAttempts.UseVisualStyleBackColor = true;
            this.btnViewAttempts.Click += new System.EventHandler(this.btnViewAttempts_Click);
            // 
            // btnManageCategories
            // 
            this.btnManageCategories.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageCategories.ForeColor = System.Drawing.Color.White;
            this.btnManageCategories.Location = new System.Drawing.Point(15, 140);
            this.btnManageCategories.Margin = new System.Windows.Forms.Padding(4);
            this.btnManageCategories.Name = "btnManageCategories";
            this.btnManageCategories.Size = new System.Drawing.Size(225, 47);
            this.btnManageCategories.TabIndex = 2;
            this.btnManageCategories.Text = "Manage Categories";
            this.btnManageCategories.UseVisualStyleBackColor = true;
            this.btnManageCategories.Click += new System.EventHandler(this.btnManageCategories_Click);
            // 
            // btnManageQuizzes
            // 
            this.btnManageQuizzes.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageQuizzes.ForeColor = System.Drawing.Color.White;
            this.btnManageQuizzes.Location = new System.Drawing.Point(15, 80);
            this.btnManageQuizzes.Margin = new System.Windows.Forms.Padding(4);
            this.btnManageQuizzes.Name = "btnManageQuizzes";
            this.btnManageQuizzes.Size = new System.Drawing.Size(225, 47);
            this.btnManageQuizzes.TabIndex = 1;
            this.btnManageQuizzes.Text = "Manage Quizzes";
            this.btnManageQuizzes.UseVisualStyleBackColor = true;
            this.btnManageQuizzes.Click += new System.EventHandler(this.btnManageQuizzes_Click);
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Location = new System.Drawing.Point(19, 27);
            this.lblWelcome.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(169, 28);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "Welcome, admin";
            // 
            // btnManageAchievements
            // 
            this.btnManageAchievements.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageAchievements.ForeColor = System.Drawing.Color.White;
            this.btnManageAchievements.Location = new System.Drawing.Point(15, 320);
            this.btnManageAchievements.Margin = new System.Windows.Forms.Padding(4);
            this.btnManageAchievements.Name = "btnManageAchievements";
            this.btnManageAchievements.Size = new System.Drawing.Size(225, 47);
            this.btnManageAchievements.TabIndex = 5;
            this.btnManageAchievements.Text = "Manage Achievements";
            this.btnManageAchievements.UseVisualStyleBackColor = true;
            this.btnManageAchievements.Click += new System.EventHandler(this.btnManageAchievements_Click);
            // 
            // btnManageUsers
            // 
            this.btnManageUsers.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManageUsers.ForeColor = System.Drawing.Color.White;
            this.btnManageUsers.Location = new System.Drawing.Point(15, 380);
            this.btnManageUsers.Margin = new System.Windows.Forms.Padding(4);
            this.btnManageUsers.Name = "btnManageUsers";
            this.btnManageUsers.Size = new System.Drawing.Size(225, 47);
            this.btnManageUsers.TabIndex = 6;
            this.btnManageUsers.Text = "Manage Users";
            this.btnManageUsers.UseVisualStyleBackColor = true;
            this.btnManageUsers.Click += new System.EventHandler(this.btnManageUsers_Click);
            // 
            // panelMainContent
            // 
            this.panelMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMainContent.Location = new System.Drawing.Point(257, 0);
            this.panelMainContent.Margin = new System.Windows.Forms.Padding(4);
            this.panelMainContent.Name = "panelMainContent";
            this.panelMainContent.Size = new System.Drawing.Size(1008, 748);
            this.panelMainContent.TabIndex = 1;
            // 
            // AdminDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1265, 748);
            this.Controls.Add(this.panelMainContent);
            this.Controls.Add(this.panelSidebar);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AdminDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin Dashboard";
            this.panelSidebar.ResumeLayout(false);
            this.panelSidebar.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
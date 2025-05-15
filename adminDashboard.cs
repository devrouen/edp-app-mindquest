using mindquest;
using System;
using System.Windows.Forms;

namespace MindQuest
{
    public partial class AdminDashboard : Form
    {
        private string adminUsername;

        public AdminDashboard(string username)
        {
            InitializeComponent();
            adminUsername = username;
            lblWelcome.Text = $"Welcome, {adminUsername}!";
        }

        private void LoadControl(UserControl control)
        {
            panelMainContent.Controls.Clear();
            control.Dock = DockStyle.Fill;
            panelMainContent.Controls.Add(control);
        }

        private void btnManageQuizzes_Click(object sender, EventArgs e)
        {
            LoadControl(new ManageQuizzesControl(adminUsername));
        }

        private void btnManageCategories_Click(object sender, EventArgs e)
        {
            LoadControl(new ManageCategoriesControl());
        }

        private void btnViewAttempts_Click(object sender, EventArgs e)
        {
            LoadControl(new ViewAttemptsControl());
        }

        private void btnViewLogs_Click(object sender, EventArgs e)
        {
            LoadControl(new AdminLogsControl());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Hide();
                loginForm loginForm = new loginForm();
                loginForm.Show();
            }
        }

        private void btnManageAchievements_Click(object sender, EventArgs e)
        {
            LoadControl(new ManageAchievementsControl());
        }

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            LoadControl(new UserListControl());
        }

    }
}
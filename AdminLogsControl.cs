using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class AdminLogsControl : UserControl
    {
        public AdminLogsControl()
        {
            InitializeComponent();
        }

        private void AdminLogsControl_Load(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();
                    string query = @"
                        SELECT 
                            log_id AS 'Log ID',
                            log_message AS 'Message',
                            log_date AS 'Date'
                        FROM logs
                        ORDER BY log_date DESC";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvLogs.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading logs: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }
    }
}

using System;
using System.Data;
using System.Windows.Forms;
using mindquest;
using MindQuest;
using MySql.Data.MySqlClient;

namespace MindQuest
{
    public partial class AdminLogsControl : UserControl
    {
        private DataTable logsTable;

        public AdminLogsControl()
        {
            InitializeComponent();
        }

        private void AdminLogsControl_Load(object sender, EventArgs e)
        {
            ConfigureDataGridView();
            LoadLogs();
        }

        private void ConfigureDataGridView()
        {
            dgvLogs.RowHeadersVisible = false;
            dgvLogs.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void LoadLogs()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

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

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn))
                    {
                        logsTable = new DataTable();
                        adapter.Fill(logsTable);
                        dgvLogs.DataSource = logsTable;
                    }
                }

                FormatDataGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading logs: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void FormatDataGridView()
        {
            if (dgvLogs.Columns.Count > 0)
            {
                // Set date column format
                if (dgvLogs.Columns.Contains("Date"))
                {
                    dgvLogs.Columns["Date"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
                }

                // Auto-size columns for better content display
                dgvLogs.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }
    }
}
using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using MySql.Data.MySqlClient;
using System.IO;
using mindquest;

namespace MindQuest
{
    public partial class ViewAttemptsControl : UserControl
    {
        private DataTable originalData;
        private string currentSearchText = string.Empty;
        private string currentFilterColumn = string.Empty;
        private DateTime? startDate = null;
        private DateTime? endDate = null;

        public ViewAttemptsControl()
        {
            InitializeComponent();
            SetupControls();
        }

        private void SetupControls()
        {
            // Setup the filter dropdown
            cmbFilterBy.Items.Add("All");
            cmbFilterBy.Items.Add("Username");
            cmbFilterBy.Items.Add("Quiz Title");
            cmbFilterBy.Items.Add("Score");
            cmbFilterBy.SelectedIndex = 0;

            // Set default date range (last 30 days)
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Today;

            // Style the grid
            dgvAttempts.RowHeadersVisible = false;
            dgvAttempts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvAttempts.EnableHeadersVisualStyles = false;
            dgvAttempts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(50, 120, 200);
            dgvAttempts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAttempts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgvAttempts.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            dgvAttempts.MultiSelect = false;
            dgvAttempts.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvAttempts.ColumnHeadersHeight = 40;
        }

        private void ViewAttemptsControl_Load(object sender, EventArgs e)
        {
            LoadAttempts();
        }

        private void LoadAttempts()
        {
            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                       SELECT 
                            qa.quiz_attempt_id AS 'Attempt ID',
                            u.username AS 'Username',
                            q.title AS 'Quiz Title',
                            qa.attempt_date AS 'Date Taken',
                            qa.score AS 'Score',
                            CONCAT(
                                SUM(CASE WHEN a.is_correct = 1 THEN 1 ELSE 0 END), 
                                '/', 
                                COUNT(ur.response_id)
                            ) AS 'Correct/Total'
                        FROM quizattempts qa
                        JOIN users u ON qa.user_id = u.user_id
                        JOIN quizzes q ON qa.quiz_id = q.quiz_id
                        LEFT JOIN userresponses ur ON qa.quiz_attempt_id = ur.quiz_attempt_id
                        LEFT JOIN answers a ON ur.answer_id = a.answer_id
                        GROUP BY qa.quiz_attempt_id, u.username, q.title, qa.attempt_date, qa.score
                        ORDER BY qa.attempt_date DESC;
    
                    ";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    originalData = new DataTable();
                    adapter.Fill(originalData);
                    dgvAttempts.DataSource = originalData;

                    // Format the Date column
                    if (dgvAttempts.Columns["Date Taken"] != null)
                        dgvAttempts.Columns["Date Taken"].DefaultCellStyle.Format = "dd-MMM-yyyy HH:mm";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load quiz attempts:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            currentSearchText = txtSearch.Text.Trim();
            ApplyFilters();
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            startDate = dtpStartDate.Value.Date;
            endDate = dtpEndDate.Value.Date.AddDays(1).AddSeconds(-1); // End of the selected day

            switch (cmbFilterBy.SelectedItem.ToString())
            {
                case "Username":
                    currentFilterColumn = "Username";
                    break;
                case "Quiz Title":
                    currentFilterColumn = "Quiz Title";
                    break;
                case "Score":
                    currentFilterColumn = "Score";
                    break;
                default:
                    currentFilterColumn = string.Empty; // All columns
                    break;
            }

            ApplyFilters();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            cmbFilterBy.SelectedIndex = 0;
            dtpStartDate.Value = DateTime.Today.AddDays(-30);
            dtpEndDate.Value = DateTime.Today;

            currentSearchText = string.Empty;
            currentFilterColumn = string.Empty;
            startDate = null;
            endDate = null;

            dgvAttempts.DataSource = originalData;
        }

        private void ApplyFilters()
        {
            if (originalData == null) return;

            try
            {
                // Create a view with the original data
                DataView dv = new DataView(originalData);

                // Build filter expression
                string filterExpression = string.Empty;

                // Date filter - always apply if dates are set
                if (startDate.HasValue && endDate.HasValue)
                {
                    string dateFormat = "yyyy-MM-dd HH:mm:ss";
                    filterExpression = $"[Date Taken] >= #{startDate.Value.ToString(dateFormat)}# AND [Date Taken] <= #{endDate.Value.ToString(dateFormat)}#";
                }

                // Text search filter
                if (!string.IsNullOrWhiteSpace(currentSearchText))
                {
                    string searchFilter;

                    if (string.IsNullOrEmpty(currentFilterColumn) || currentFilterColumn == "All")
                    {
                        // Search across multiple columns
                        searchFilter = $"[Username] LIKE '%{currentSearchText}%' OR [Quiz Title] LIKE '%{currentSearchText}%'";
                    }
                    else
                    {
                        // Search in specific column
                        searchFilter = $"[{currentFilterColumn}] LIKE '%{currentSearchText}%'";
                    }

                    // Combine with date filter if exists
                    if (!string.IsNullOrEmpty(filterExpression))
                        filterExpression += " AND ";

                    filterExpression += searchFilter;
                }

                // Apply filter
                dv.RowFilter = filterExpression;
                dgvAttempts.DataSource = dv.ToTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error applying filters: " + ex.Message, "Filter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void generateReport_Click(object sender, EventArgs e)
        {
            try
            {
                // Reference to the current data (either original or filtered)
                DataTable currentData = dgvAttempts.DataSource as DataTable;
                if (currentData == null || currentData.Rows.Count == 0)
                {
                    MessageBox.Show("No data available to generate report.", "Report Generation",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Find the solution directory (mindquest folder)
                string currentDir = Application.StartupPath;
                string solutionDir = currentDir;

                // Navigate up until we find the mindquest folder
                while (!Directory.Exists(Path.Combine(solutionDir, "reports")) &&
                       Path.GetDirectoryName(solutionDir) != null)
                {
                    solutionDir = Path.GetDirectoryName(solutionDir);
                }

                // If we couldn't locate the reports directory, try the parent of the executable directory
                if (!Directory.Exists(Path.Combine(solutionDir, "reports")))
                {
                    solutionDir = Path.GetDirectoryName(currentDir);
                    if (!Directory.Exists(Path.Combine(solutionDir, "reports")))
                    {
                        // If still not found, check if we're in debug/release folder and need to go up one more level
                        solutionDir = Path.GetDirectoryName(solutionDir);
                        if (!Directory.Exists(Path.Combine(solutionDir, "reports")))
                        {
                            MessageBox.Show("Could not locate the reports folder in the solution directory.",
                                "Folder Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                // Now we should have the correct path to the solution directory
                string reportsRootFolder = Path.Combine(solutionDir, "reports");
                string todayFolder = Path.Combine(reportsRootFolder, DateTime.Today.ToString("yyyy-MM-dd"));

                // Create the folder if it doesn't exist
                if (!Directory.Exists(todayFolder))
                {
                    Directory.CreateDirectory(todayFolder);
                }

                // Template file path
                string templatePath = Path.Combine(reportsRootFolder, "report-template.xlsx");
                if (!File.Exists(templatePath))
                {
                    MessageBox.Show($"Report template not found. Looking for file at: {templatePath}",
                        "Template Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Log the paths for debugging
                Console.WriteLine($"Solution directory: {solutionDir}");
                Console.WriteLine($"Reports folder: {reportsRootFolder}");
                Console.WriteLine($"Template path: {templatePath}");

                // Create new report filename with timestamp
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string reportFilename = $"QuizAttempts_Report_{timestamp}.xlsx";
                string reportPath = Path.Combine(todayFolder, reportFilename);

                // Copy the template
                File.Copy(templatePath, reportPath, true);

                // Initialize Excel application using Microsoft.Office.Interop.Excel
                var excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.Visible = false;

                // Open the workbook
                var workbook = excelApp.Workbooks.Open(reportPath);
                var worksheet = workbook.Sheets[1] as Microsoft.Office.Interop.Excel.Worksheet;

                // Update the filter information in the existing header section
                // (assuming the template already has headers in the first 3 rows)
                if (!string.IsNullOrEmpty(currentSearchText) || startDate.HasValue || endDate.HasValue)
                {
                    string filterInfo = "Filters applied: ";
                    if (!string.IsNullOrEmpty(currentSearchText))
                        filterInfo += $"Search text: '{currentSearchText}' ";

                    if (!string.IsNullOrEmpty(currentFilterColumn) && currentFilterColumn != "All")
                        filterInfo += $"Column: {currentFilterColumn} ";

                    if (startDate.HasValue && endDate.HasValue)
                        filterInfo += $"Date range: {startDate.Value.ToShortDateString()} to {endDate.Value.ToShortDateString()}";

                    worksheet.Cells[3, 1] = filterInfo;
                }
                else
                {
                    worksheet.Cells[3, 1] = "No filters applied";
                }

                // Update the generation date in the header
                worksheet.Cells[2, 1] = $"Generated on: {DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss")}";

                // Find the first empty row after the header (row 4 is where data should start)
                const int startRow = 4;

                // Write column headers
                for (int col = 0; col < currentData.Columns.Count; col++)
                {
                    worksheet.Cells[startRow, col + 1] = currentData.Columns[col].ColumnName;

                    // Format header cells
                    var headerCell = worksheet.Cells[startRow, col + 1];
                    headerCell.Font.Bold = true;
                    headerCell.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(50, 120, 200));
                    headerCell.Font.Color = System.Drawing.ColorTranslator.ToOle(Color.White);
                }

                // Write data rows
                for (int row = 0; row < currentData.Rows.Count; row++)
                {
                    for (int col = 0; col < currentData.Columns.Count; col++)
                    {
                        // Excel rows are 1-based, and we're starting after the headers
                        var cell = worksheet.Cells[row + startRow + 1, col + 1];

                        // Special handling for "Correct/Total" column to prevent date formatting
                        if (currentData.Columns[col].ColumnName == "Correct/Total")
                        {
                            // Format as text before setting the value
                            cell.NumberFormat = "@";
                            cell.Value = currentData.Rows[row][col].ToString();
                        }
                        else
                        {
                            cell.Value = currentData.Rows[row][col].ToString();
                        }
                    }

                    // Add alternating row colors
                    if (row % 2 == 1)
                    {
                        var rowRange = worksheet.Range[
                            worksheet.Cells[row + startRow + 1, 1],
                            worksheet.Cells[row + startRow + 1, currentData.Columns.Count]];
                        rowRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(Color.FromArgb(240, 240, 240));
                    }
                }

                // Auto-fit columns
                var usedRange = worksheet.UsedRange;
                usedRange.Columns.AutoFit();

                // Save the workbook
                workbook.Save();

                // Close and release resources
                workbook.Close();
                excelApp.Quit();

                System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                MessageBox.Show($"Report successfully generated and saved to:\n{reportPath}",
                    "Report Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Open the containing folder
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{reportPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}",
                    "Report Generation Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            // Empty event handler
        }

        private void dtpEndDate_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
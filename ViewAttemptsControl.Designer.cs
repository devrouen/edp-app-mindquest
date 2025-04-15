namespace MindQuest
{
    partial class ViewAttemptsControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dgvAttempts;
        private System.Windows.Forms.Label lblTitle;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dgvAttempts = new System.Windows.Forms.DataGridView();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttempts)).BeginInit();
            this.SuspendLayout();

            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(400, 40);
            this.lblTitle.Text = "User Quiz Attempts";

            // 
            // dgvAttempts
            // 
            this.dgvAttempts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                | System.Windows.Forms.AnchorStyles.Left)
                                                | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAttempts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAttempts.Location = new System.Drawing.Point(20, 80);
            this.dgvAttempts.Name = "dgvAttempts";
            this.dgvAttempts.Size = new System.Drawing.Size(900, 500);
            this.dgvAttempts.TabIndex = 1;

            // 
            // ViewAttemptsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.dgvAttempts);
            this.Name = "ViewAttemptsControl";
            this.Size = new System.Drawing.Size(950, 600);
            this.Load += new System.EventHandler(this.ViewAttemptsControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAttempts)).EndInit();
            this.ResumeLayout(false);
        }
    }
}

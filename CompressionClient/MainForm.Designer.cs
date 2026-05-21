using System;
using System.Drawing;
using System.Windows.Forms;

namespace CompressionClient
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        private Label lblTitle;
        private Label lblConnStatus;
        private GroupBox grpServer;
        private Label lblHost;
        private TextBox txtHost;
        private Label lblPort;
        private TextBox txtPort;
        private Button btnConnect;
        private GroupBox grpFile;
        private TextBox txtFilePath;
        private Button btnBrowse;
        private Button btnSend;
        private GroupBox grpLog;
        private RichTextBox rtbLog;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Label lblOrigSize;
        private Label lblCompSize;
        private Label lblRatio;
        private Panel pnlStats;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.Text          = "File Compression Client";
            this.Size          = new Size(650, 660);
            this.MinimumSize   = new Size(650, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font          = new Font("Segoe UI", 9.5f);
            this.BackColor     = Color.FromArgb(240, 242, 245);

            // ── Title ──────────────────────────────────────────────────────
            lblTitle = new Label();
            lblTitle.Text      = "File Compression Client";
            lblTitle.Font      = new Font("Segoe UI Semibold", 15f);
            lblTitle.ForeColor = Color.FromArgb(30, 80, 160);
            lblTitle.AutoSize  = true;
            lblTitle.Location  = new Point(20, 15);

            lblConnStatus = new Label();
            lblConnStatus.Text      = "● Disconnected";
            lblConnStatus.Font      = new Font("Segoe UI Semibold", 9.5f);
            lblConnStatus.ForeColor = Color.FromArgb(220, 80, 80);
            lblConnStatus.AutoSize  = true;
            lblConnStatus.Location  = new Point(470, 22);

            // ── Server Settings ───────────────────────────────────────────
            grpServer          = new GroupBox();
            grpServer.Text     = "Server Settings";
            grpServer.Location = new Point(15, 55);
            grpServer.Size     = new Size(610, 75);
            grpServer.Font     = new Font("Segoe UI Semibold", 9.5f);

            lblHost          = new Label();
            lblHost.Text     = "Host:";
            lblHost.Location = new Point(12, 34);
            lblHost.AutoSize = true;

            txtHost          = new TextBox();
            txtHost.Text     = "127.0.0.1";
            txtHost.Location = new Point(52, 30);
            txtHost.Width    = 150;

            lblPort          = new Label();
            lblPort.Text     = "Port:";
            lblPort.Location = new Point(220, 34);
            lblPort.AutoSize = true;

            txtPort          = new TextBox();
            txtPort.Text     = "9000";
            txtPort.Location = new Point(258, 30);
            txtPort.Width    = 70;

            btnConnect           = new Button();
            btnConnect.Text      = "Connect";
            btnConnect.Location  = new Point(350, 28);
            btnConnect.Size      = new Size(110, 32);
            btnConnect.BackColor = Color.FromArgb(30, 130, 200);
            btnConnect.ForeColor = Color.White;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.Cursor    = Cursors.Hand;
            btnConnect.Font      = new Font("Segoe UI Semibold", 9.5f);
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Click    += BtnConnect_Click;

            grpServer.Controls.Add(lblHost);
            grpServer.Controls.Add(txtHost);
            grpServer.Controls.Add(lblPort);
            grpServer.Controls.Add(txtPort);
            grpServer.Controls.Add(btnConnect);

            // ── File Selection ────────────────────────────────────────────
            grpFile          = new GroupBox();
            grpFile.Text     = "Select File";
            grpFile.Location = new Point(15, 140);
            grpFile.Size     = new Size(610, 70);
            grpFile.Font     = new Font("Segoe UI Semibold", 9.5f);

            txtFilePath          = new TextBox();
            txtFilePath.Text     = "";
            txtFilePath.Location = new Point(12, 28);
            txtFilePath.Width    = 460;
            txtFilePath.ReadOnly = true;
            txtFilePath.BackColor = Color.White;

            btnBrowse           = new Button();
            btnBrowse.Text      = "Browse...";
            btnBrowse.Location  = new Point(482, 26);
            btnBrowse.Size      = new Size(110, 30);
            btnBrowse.BackColor = Color.FromArgb(100, 100, 120);
            btnBrowse.ForeColor = Color.White;
            btnBrowse.FlatStyle = FlatStyle.Flat;
            btnBrowse.Cursor    = Cursors.Hand;
            btnBrowse.FlatAppearance.BorderSize = 0;
            btnBrowse.Click    += BtnBrowse_Click;

            grpFile.Controls.Add(txtFilePath);
            grpFile.Controls.Add(btnBrowse);

            // ── Send Button ───────────────────────────────────────────────
            btnSend           = new Button();
            btnSend.Text      = "Send & Compress";
            btnSend.Location  = new Point(15, 222);
            btnSend.Size      = new Size(610, 44);
            btnSend.Font      = new Font("Segoe UI Semibold", 12f);
            btnSend.BackColor = Color.FromArgb(30, 160, 100);
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.Cursor    = Cursors.Hand;
            btnSend.Enabled   = false;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click    += BtnSend_Click;

            // ── Progress ──────────────────────────────────────────────────
            progressBar          = new ProgressBar();
            progressBar.Location = new Point(15, 278);
            progressBar.Size     = new Size(610, 16);
            progressBar.Style    = ProgressBarStyle.Continuous;

            lblStatus          = new Label();
            lblStatus.Text     = "Not connected.";
            lblStatus.Location = new Point(15, 299);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.FromArgb(80, 80, 80);

            // ── Stats Panel ───────────────────────────────────────────────
            pnlStats           = new Panel();
            pnlStats.Location  = new Point(15, 322);
            pnlStats.Size      = new Size(610, 38);
            pnlStats.BackColor = Color.FromArgb(220, 235, 255);
            pnlStats.Visible   = false;

            lblOrigSize          = new Label();
            lblOrigSize.Location = new Point(10, 11);
            lblOrigSize.AutoSize = true;
            lblOrigSize.ForeColor = Color.FromArgb(50, 50, 120);

            lblCompSize          = new Label();
            lblCompSize.Location = new Point(200, 11);
            lblCompSize.AutoSize = true;
            lblCompSize.ForeColor = Color.FromArgb(20, 120, 60);

            lblRatio          = new Label();
            lblRatio.Location = new Point(415, 11);
            lblRatio.AutoSize = true;
            lblRatio.Font     = new Font("Segoe UI Semibold", 9.5f);
            lblRatio.ForeColor = Color.FromArgb(180, 60, 0);

            pnlStats.Controls.Add(lblOrigSize);
            pnlStats.Controls.Add(lblCompSize);
            pnlStats.Controls.Add(lblRatio);

            // ── Log ───────────────────────────────────────────────────────
            grpLog          = new GroupBox();
            grpLog.Text     = "Activity Log";
            grpLog.Location = new Point(15, 370);
            grpLog.Size     = new Size(610, 240);
            grpLog.Font     = new Font("Segoe UI Semibold", 9.5f);

            rtbLog            = new RichTextBox();
            rtbLog.Location   = new Point(10, 22);
            rtbLog.Size       = new Size(588, 208);
            rtbLog.ReadOnly   = true;
            rtbLog.BackColor  = Color.FromArgb(18, 22, 30);
            rtbLog.ForeColor  = Color.FromArgb(180, 220, 180);
            rtbLog.Font       = new Font("Consolas", 9f);
            rtbLog.BorderStyle = BorderStyle.None;
            rtbLog.ScrollBars = RichTextBoxScrollBars.Vertical;

            grpLog.Controls.Add(rtbLog);

            // ── Add all to form ───────────────────────────────────────────
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblConnStatus);
            this.Controls.Add(grpServer);
            this.Controls.Add(grpFile);
            this.Controls.Add(btnSend);
            this.Controls.Add(progressBar);
            this.Controls.Add(lblStatus);
            this.Controls.Add(pnlStats);
            this.Controls.Add(grpLog);

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

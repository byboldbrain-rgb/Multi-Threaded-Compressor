using System;
using System.Drawing;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CompressionClient
{
    public partial class MainForm : Form
    {
        private bool _isConnected = false;
        private string _host = "";
        private int _port = 9000;

        public MainForm()
        {
            InitializeComponent();
            Log("Welcome! Enter server IP and port, then click Connect.", Color.FromArgb(150, 200, 255));
            UpdateConnectionUI(false);
        }

        // ── Connect ───────────────────────────────────────────────────────────
        private async void BtnConnect_Click(object sender, EventArgs e)
        {
            if (_isConnected)
            {
                _isConnected = false;
                UpdateConnectionUI(false);
                Log("Disconnected.", Color.FromArgb(255, 160, 100));
                return;
            }

            _host = txtHost.Text.Trim();
            if (!int.TryParse(txtPort.Text.Trim(), out _port))
            {
                MessageBox.Show("Invalid port number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Log($"Testing connection to {_host}:{_port}...", Color.FromArgb(255, 220, 120));
            btnConnect.Enabled = false;

            bool ok = await Task.Run(() =>
            {
                try
                {
                    using var t = new TcpClient();
                    t.Connect(_host, _port);
                    return true;
                }
                catch { return false; }
            });

            btnConnect.Enabled = true;

            if (ok)
            {
                _isConnected = true;
                UpdateConnectionUI(true);
                Log($"Connected to {_host}:{_port} successfully!", Color.FromArgb(100, 255, 150));
            }
            else
            {
                Log($"Could not connect to {_host}:{_port}. Is the server running?", Color.FromArgb(255, 100, 100));
                MessageBox.Show($"Cannot connect to {_host}:{_port}\nMake sure the server is running.",
                    "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── Browse ────────────────────────────────────────────────────────────
        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Select a file to compress",
                Filter = "All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = dlg.FileName;
                long size = new FileInfo(dlg.FileName).Length;
                Log($"File selected: {Path.GetFileName(dlg.FileName)}  ({FormatBytes(size)})", Color.FromArgb(200, 230, 200));
            }
        }

        // ── Send & Compress ───────────────────────────────────────────────────
        private async void BtnSend_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Please connect to the server first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("Please select a file first.", "No File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetBusy(true);
            pnlStats.Visible = false;

            try
            {
                byte[] fileData = await Task.Run(() => File.ReadAllBytes(txtFilePath.Text));
                long originalSize = fileData.Length;
                Log($"Sending {Path.GetFileName(txtFilePath.Text)} ({FormatBytes(originalSize)}) to server...", Color.FromArgb(255, 220, 120));

                byte[] compressed = await Task.Run(() => SendAndReceive(fileData));

                string savePath = txtFilePath.Text + ".gz";
                await Task.Run(() => File.WriteAllBytes(savePath, compressed));

                double ratio = (1.0 - (double)compressed.Length / originalSize) * 100;
                lblOrigSize.Text = $"Original:     {FormatBytes(originalSize)}";
                lblCompSize.Text = $"Compressed:  {FormatBytes(compressed.Length)}";
                lblRatio.Text    = $"Saved {ratio:F1}%";
                pnlStats.Visible = true;

                Log($"Done! Saved as: {Path.GetFileName(savePath)}", Color.FromArgb(100, 255, 150));
                Log($"  {FormatBytes(originalSize)}  ->  {FormatBytes(compressed.Length)}  (saved {ratio:F1}%)", Color.FromArgb(100, 255, 150));
                lblStatus.Text = "Compression complete!";
            }
            catch (Exception ex)
            {
                Log($"Error: {ex.Message}", Color.FromArgb(255, 100, 100));
                lblStatus.Text = "Failed. See log.";
                _isConnected = false;
                UpdateConnectionUI(false);
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        // ── Core Network Logic ────────────────────────────────────────────────
        private byte[] SendAndReceive(byte[] fileData)
        {
            using var client = new TcpClient();
            client.ReceiveTimeout = 30000;
            client.SendTimeout    = 30000;
            client.Connect(_host, _port);

            using NetworkStream stream = client.GetStream();
            using BinaryWriter writer  = new BinaryWriter(stream);
            using BinaryReader reader  = new BinaryReader(stream);

            // Send file size then file data
            writer.Write((long)fileData.Length);
            writer.Write(fileData);
            writer.Flush();

            // Receive compressed size then compressed data
            long compSize      = reader.ReadInt64();
            byte[] compressed  = reader.ReadBytes((int)compSize);
            return compressed;
        }

        // ── UI Helpers ────────────────────────────────────────────────────────
        private void UpdateConnectionUI(bool connected)
        {
            _isConnected         = connected;
            btnConnect.Text      = connected ? "Disconnect" : "Connect";
            btnConnect.BackColor = connected ? Color.FromArgb(180, 60, 60) : Color.FromArgb(30, 130, 200);
            txtHost.Enabled      = !connected;
            txtPort.Enabled      = !connected;
            btnSend.Enabled      = connected;
            lblStatus.Text       = connected ? $"Connected to {_host}:{_port}" : "Not connected.";
            lblConnStatus.Text   = connected ? "● Connected" : "● Disconnected";
            lblConnStatus.ForeColor = connected ? Color.FromArgb(80, 220, 100) : Color.FromArgb(220, 80, 80);
        }

        private void SetBusy(bool busy)
        {
            btnSend.Enabled    = !busy && _isConnected;
            btnBrowse.Enabled  = !busy;
            btnConnect.Enabled = !busy;
            progressBar.Style  = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            progressBar.MarqueeAnimationSpeed = busy ? 30 : 0;
            if (busy) lblStatus.Text = "Processing...";
        }

        private void Log(string message, Color color)
        {
            if (InvokeRequired) { Invoke(() => Log(message, color)); return; }
            rtbLog.SelectionStart  = rtbLog.TextLength;
            rtbLog.SelectionLength = 0;
            rtbLog.SelectionColor  = Color.FromArgb(100, 120, 100);
            rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] ");
            rtbLog.SelectionColor  = color;
            rtbLog.AppendText(message + "\n");
            rtbLog.ScrollToCaret();
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            return $"{bytes / (1024.0 * 1024):F2} MB";
        }
    }
}

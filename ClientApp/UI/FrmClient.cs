using ClientApp.Core;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ClientApp.UI
{
    public partial class FrmClient : Form
    {
        private ClientCore _client;
        private string _serverFolder;

        public FrmClient()
        {
            InitializeComponent();
            lblStatus.Text = "Waiting...";
            lblStatus.ForeColor = Color.Gray;
            btnDisconnect.Enabled = false;
        }

        // ── Nút Connect ─────────────────────────────────────────────────────
        private void btnConnect_Click(object sender, EventArgs e)
        {
            // Validate IP
            if (string.IsNullOrWhiteSpace(txtIP.Text))
            {
                MessageBox.Show("Nhập địa chỉ IP của server!", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate Port
            if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Port không hợp lệ! (1 – 65535)", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cập nhật UI trước khi kết nối
            SetStatus("Connecting...", Color.Orange);
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;

            // Khởi tạo ClientCore
            _client = new ClientCore();
            _client.OnFolderReceived = (folder) =>
            {
                _serverFolder = folder;
            };
            // Callback nhận thay đổi file từ server
            _client.OnChangeReceived = (change) =>
            {
                Invoke(new Action(() =>
                {
                    string icon = GetActionIcon(change.Action);
                    string msg = $"{change.Time:HH:mm:ss}  {icon}  {change.Action,-10}  {change.FileName}";
                    lstLog.Items.Add(msg);
                    lstLog.TopIndex = lstLog.Items.Count - 1; // auto-scroll

                    // Flash thông báo trên TaskBar
                    //FlashTaskbar();
                }));
            };

            // Callback cập nhật trạng thái kết nối
            _client.OnStatusChange = (msg) =>
            {
                Invoke(new Action(() =>
                {
                    if (msg.StartsWith("Connected"))
                    {
                        SetStatus(msg, Color.Green);
                        _client.RequestFolder();
                        btnLoadHistory.Enabled = true;
                    }
                    else if (msg.StartsWith("Lost") || msg.StartsWith("Retry"))
                    {
                        SetStatus(msg, Color.Orange);
                    }
                    else // Disconnected
                    {
                        SetStatus(msg, Color.Red);
                        btnConnect.Enabled = true;
                        btnDisconnect.Enabled = false;
                        btnLoadHistory.Enabled = false;
                    }
                }));
            };

            _client.Connect(txtIP.Text.Trim(), port);
        }

        // ── Nút Disconnect ───────────────────────────────────────────────────
        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _client?.Disconnect();
            SetStatus("Disconnected", Color.Red);
            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        // ── Nút Clear Log ────────────────────────────────────────────────────
        private void btnClear_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        // ── Đóng form: ngắt kết nối trước khi thoát ─────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _client?.Disconnect();
            base.OnFormClosing(e);
        }

        // ── Helper: cập nhật label trạng thái ───────────────────────────────
        private void SetStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        // ── Helper: icon theo loại hành động ────────────────────────────────
        private static string GetActionIcon(string action)
        {
            switch (action)
            {
                case "Created": return "[+]";
                case "Deleted": return "[-]";
                case "Modified": return "[~]";
                case "Renamed": return "[→]";
                default: return "[?]";
            }
        }

        private void btnLoadHistory_Click(object sender, EventArgs e)
        {
            if (_client == null)
            {
                MessageBox.Show("Chưa connect tới server!");
                return;
            }

            if (string.IsNullOrEmpty(_serverFolder))
            {
                MessageBox.Show("Chưa nhận được folder từ server!");
                return;
            }

            lstLog.Items.Clear();
            _client.RequestHistory(_serverFolder);
        }

    }
}
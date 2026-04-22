using ClientApp.Core;
using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace ClientApp.UI
{
    public partial class FrmClient : Form
    {
        private ClientCore _client;
        private string _serverFolder;

        // ── Cảnh báo: trạng thái bật/tắt và loại action cần cảnh báo ─────────
        private bool _alertEnabled = true;
        private bool _alertCreated = true;
        private bool _alertDeleted = true;
        private bool _alertModified = false;
        private bool _alertRenamed = false;

        public FrmClient()
        {
            InitializeComponent();
            lblStatus.Text = "Waiting...";
            lblStatus.ForeColor = Color.Gray;
            btnDisconnect.Enabled = false;
            btnLoadHistory.Enabled = false;

            // Đồng bộ trạng thái checkbox với biến
            chkAlertEnable.Checked = _alertEnabled;
            chkAlertCreated.Checked = _alertCreated;
            chkAlertDeleted.Checked = _alertDeleted;
            chkAlertModified.Checked = _alertModified;
            chkAlertRenamed.Checked = _alertRenamed;
            UpdateAlertCheckboxState();
        }

        // ── Nút Connect ─────────────────────────────────────────────────────
        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIP.Text))
            {
                MessageBox.Show("Nhập địa chỉ IP của server!", "Thiếu thông tin",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(txtPort.Text, out int port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Port không hợp lệ! (1 – 65535)", "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SetStatus("Connecting...", Color.Orange);
            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;

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
                    lstLog.TopIndex = lstLog.Items.Count - 1;

                    // ── CẢNH BÁO ────────────────────────────────────────────
                    TriggerAlert(change);
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
                    else
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
            btnLoadHistory.Enabled = false;
        }

        // ── Nút Clear ────────────────────────────────────────────────────────
        private void btnClear_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        // ── Nút Load History ─────────────────────────────────────────────────
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

        // ── Checkbox bật/tắt toàn bộ cảnh báo ───────────────────────────────
        private void chkAlertEnable_CheckedChanged(object sender, EventArgs e)
        {
            _alertEnabled = chkAlertEnable.Checked;
            UpdateAlertCheckboxState();
        }

        // ── Checkbox từng loại action ────────────────────────────────────────
        private void chkAlertCreated_CheckedChanged(object sender, EventArgs e)
            => _alertCreated = chkAlertCreated.Checked;

        private void chkAlertDeleted_CheckedChanged(object sender, EventArgs e)
            => _alertDeleted = chkAlertDeleted.Checked;

        private void chkAlertModified_CheckedChanged(object sender, EventArgs e)
            => _alertModified = chkAlertModified.Checked;

        private void chkAlertRenamed_CheckedChanged(object sender, EventArgs e)
            => _alertRenamed = chkAlertRenamed.Checked;

        // ── Đóng form ────────────────────────────────────────────────────────
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _client?.Disconnect();
            base.OnFormClosing(e);
        }

        // ════════════════════════════════════════════════════════════════════
        // LOGIC CẢNH BÁO
        // ════════════════════════════════════════════════════════════════════

        // Kiểm tra điều kiện và kích hoạt cảnh báo
        private void TriggerAlert(FileChange change)
        {
            if (!_alertEnabled) return;

            bool shouldAlert = false;
            switch (change.Action)
            {
                case "Created": shouldAlert = _alertCreated; break;
                case "Deleted": shouldAlert = _alertDeleted; break;
                case "Modified": shouldAlert = _alertModified; break;
                case "Renamed": shouldAlert = _alertRenamed; break;
            }
            if (!shouldAlert) return;

            // 1. Phát âm thanh cảnh báo hệ thống
            SystemSounds.Exclamation.Play();

            // 2. Nhấp nháy icon trên Taskbar
            NativeMethods.FlashWindow(this.Handle);

            // 3. Hiện balloon tooltip ở System Tray (nếu có NotifyIcon)
            //    — hoặc hiện FrmAlert popup nhỏ không chặn UI
            ShowAlertPopup(change);
        }

        // Hiện cửa sổ cảnh báo nhỏ góc phải màn hình, tự động đóng sau 4 giây
        private void ShowAlertPopup(FileChange change)
        {
            var alert = new FrmAlert(change);

            // Tính vị trí góc phải dưới màn hình
            var screen = Screen.PrimaryScreen.WorkingArea;
            alert.Left = screen.Right - alert.Width - 10;
            alert.Top = screen.Bottom - alert.Height - 10;

            alert.Show(this); // non-blocking, không chặn UI
        }

        // ════════════════════════════════════════════════════════════════════
        // HELPER
        // ════════════════════════════════════════════════════════════════════

        private void UpdateAlertCheckboxState()
        {
            // Khi tắt cảnh báo tổng thì disable các checkbox con để rõ ràng
            bool on = _alertEnabled;
            chkAlertCreated.Enabled = on;
            chkAlertDeleted.Enabled = on;
            chkAlertModified.Enabled = on;
            chkAlertRenamed.Enabled = on;
        }

        private void SetStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

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
    }
}
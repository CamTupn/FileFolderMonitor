using ClientApp.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClientApp.UI
{
    // Cửa sổ popup cảnh báo nhỏ, hiện ở góc phải màn hình, tự đóng sau 4 giây.
    // Không chặn UI chính vì dùng Show() thay vì ShowDialog().
    public class FrmAlert : Form
    {
        private System.Windows.Forms.Timer _timer;

        public FrmAlert(FileChange change)
        {
           
            this.FormBorderStyle = FormBorderStyle.None;       
            this.TopMost = true;                        
            this.ShowInTaskbar = false;                       
            this.Size = new Size(300, 80);
            this.BackColor = GetAlertColor(change.Action);
            this.Opacity = 0.93;
            this.Cursor = Cursors.Hand;

            // ── Icon + tiêu đề ───────────────────────────────────────────────
            var lblTitle = new Label
            {
                Text = $"  {GetActionIcon(change.Action)}  File {change.Action}!",
                Font = new Font("Microsoft Sans Serif", 10.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, 8),
                Size = new Size(300, 24),
            };

            // ── Tên file ─────────────────────────────────────────────────────
            string shortName = TruncatePath(change.FileName, 38);
            var lblFile = new Label
            {
                Text = $"  {shortName}",
                Font = new Font("Consolas", 8.5f),
                ForeColor = Color.White,
                Location = new Point(0, 34),
                Size = new Size(300, 18),
            };

            // ── Thời gian ────────────────────────────────────────────────────
            var lblTime = new Label
            {
                Text = $"  {change.Time:HH:mm:ss}",
                Font = new Font("Microsoft Sans Serif", 8f),
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                Location = new Point(0, 54),
                Size = new Size(150, 16),
            };

            // ── Nút đóng nhỏ ─────────────────────────────────────────────────
            var btnClose = new Label
            {
                Text = "✕",
                Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(275, 5),
                Size = new Size(20, 20),
                Cursor = Cursors.Hand,
            };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblFile);
            this.Controls.Add(lblTime);
            this.Controls.Add(btnClose);

         
            this.Click += (s, e) => this.Close();
            lblTitle.Click += (s, e) => this.Close();
            lblFile.Click += (s, e) => this.Close();

            // ── Timer tự đóng sau 4 giây ─────────────────────────────────────
            _timer = new System.Windows.Forms.Timer { Interval = 4000 };
            _timer.Tick += (s, e) =>
            {
                _timer.Stop();
                this.Close();
            };
            _timer.Start();
        }

      
        private static Color GetAlertColor(string action)
        {
            switch (action)
            {
                case "Created": return Color.FromArgb(40, 167, 69);   
                case "Deleted": return Color.FromArgb(220, 53, 69);   
                case "Modified": return Color.FromArgb(0, 123, 255);  
                case "Renamed": return Color.FromArgb(255, 140, 0);  
                default: return Color.FromArgb(108, 117, 125); 
            }
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

      
        private static string TruncatePath(string path, int maxLen)
        {
            if (string.IsNullOrEmpty(path) || path.Length <= maxLen) return path;
            return "..." + path.Substring(path.Length - (maxLen - 3));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _timer?.Dispose();
            base.Dispose(disposing);
        }
    }
}
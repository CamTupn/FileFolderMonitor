using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerApp.Core;

namespace ServerApp.UI
{
    public partial class FrmServer : Form
    {
        // Khai báo biến để quản lý ServerCore và FileWatcher
        private ServerCore server;
        private FileWatcher watcher;
        public FrmServer()
        {
            InitializeComponent();
            DbHelper.Init();
            lblStatus.Text = "Waiting...";
            btnStop.Enabled = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // Mo dialog để chọn thư mục cần theo dõi
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            // Nếu người dùng chọn thư mục và nhấn OK, cập nhật đường dẫn vào textbox
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // Kiểm tra nếu port hoặc folder chưa được nhập, hiển thị thông báo lỗi và dừng quá trình khởi động
            if (string.IsNullOrEmpty(txtPort.Text) || string.IsNullOrEmpty(txtFolder.Text))
            {
                MessageBox.Show("Nhập port và chọn folder!");
                return;
            }
            // Kiểm tra nếu thư mục không tồn tại, hiển thị thông báo lỗi và dừng quá trình khởi động
            int port;
            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Port không hợp lệ!");
                return;
            }
            // Khởi tạo ServerCore và bắt đầu lắng nghe trên cổng đã nhập
            server = new ServerCore();
            server.OnStatusChange = (msg) =>
            {
                Invoke(new Action(() =>
                {
                    lblStatus.Text = msg;
                    // Đổi màu status: nếu msg chứa "0" thì đỏ, ngược lại xanh
                    if (msg.Contains("0"))
                        lblStatus.ForeColor = Color.Red;
                    else
                        lblStatus.ForeColor = Color.Green;
                }));
            };
            server.Start(port);
            server.SetFolder(txtFolder.Text);
            watcher = new FileWatcher(server);
            watcher.Start(txtFolder.Text);
            lblStatus.Text = "Server running...";
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // Dừng FileWatcher và Server   
            watcher?.Stop();
            server?.Stop();
            lblStatus.Text = "Server stopped";
            lblStatus.ForeColor = Color.Red;
            // Cập nhật trạng thái và nút bấm
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
}

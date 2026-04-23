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
            // Kiểm tra nếu port hoặc folder đã được nhập chưa
            if (string.IsNullOrEmpty(txtPort.Text) || string.IsNullOrEmpty(txtFolder.Text))
            {
                MessageBox.Show("Nhập port và chọn folder!");
                return;
            }
            int port;
            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("Port không hợp lệ!");
                return;
            }
            // Khởi tạo ServerCore 
            server = new ServerCore();
            server.OnStatusChange = (msg) =>
            {
                Invoke(new Action(() =>
                {
                    lblStatus.Text = msg;
                    // Đổi màu status
                    if (msg.Contains("0"))
                        lblStatus.ForeColor = Color.Red;
                    else
                        lblStatus.ForeColor = Color.Green;
                }));
            };
            server.Start(port);
            server.SetFolder(txtFolder.Text); // Cập nhật thư mục theo dõi cho ServerCore
            watcher = new FileWatcher(server); // Khởi tạo FileWatcher và truyền ServerCore vào
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

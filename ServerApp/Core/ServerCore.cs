using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace ServerApp.Core
{
    // Lớp này quản lý server TCP, bao gồm việc lắng nghe kết nối từ client, gửi thông tin thay đổi file đến các client đã kết nối và quản lý trạng thái của server.
    public class ServerCore
    {
        // Lắng nghe kết nối từ client trên cổng đã chỉ định
        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>(); // danh sách các client đã kết nối
        private bool isRunning = false;
        public Action<string> OnStatusChange; 
        
        public void Start(int port)
        {
            // Khởi tạo TcpListener để lắng nghe trên tất cả các địa chỉ IP và cổng đã chỉ định
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            isRunning = true;
            // Bắt đầu một task để chấp nhận kết nối từ client trong khi server đang chạy
            Task.Run(() =>
            {
                while (isRunning)
                {
                    try
                    {
                        // Chấp nhận kết nối từ client
                        var client = server.AcceptTcpClient(); 
                        clients.Add(client);
                        OnStatusChange?.Invoke($"Client Connected ({clients.Count})");
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }
        public void Stop()
        {
            // Dừng server và đóng tất cả kết nối với client
            isRunning = false;

            try
            {
                server.Stop();
                foreach (var client in clients)
                {
                    client.Close();
                }
                clients.Clear(); 
                OnStatusChange?.Invoke("Server Stopped");
            }
            catch { }
        }
        // Phương thức này được gọi để gửi thông tin thay đổi file đến tất cả các client đã kết nối.
        // Thông tin thay đổi được serialize thành JSON và gửi qua stream của mỗi client.
        public void Broadcast(FileChange change)
        {
            // Serialize đối tượng FileChange thành JSON và chuyển đổi thành byte array để gửi qua mạng
            string json = JsonConvert.SerializeObject(change);
            byte[] data = Encoding.UTF8.GetBytes(json);
            // Gửi dữ liệu đến tất cả các client đã kết nối.
            foreach (var client in clients.ToList())
            {
                try
                {
                    var stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
                catch
                {
                    clients.Remove(client);
                    OnStatusChange?.Invoke($"Clients connected: {clients.Count}");
                }
            }
        }
    }
}

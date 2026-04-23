using Newtonsoft.Json;
using ServerApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerApp.Core
{
    // Lớp này quản lý server TCP, bao gồm việc lắng nghe kết nối từ client, gửi thông tin thay đổi file đến các client đã kết nối và quản lý trạng thái của server.
    public class ServerCore
    {
        private readonly object _lock = new object();
        // Lắng nghe kết nối từ client trên cổng đã chỉ định
        private TcpListener server;
        private List<TcpClient> clients = new List<TcpClient>(); // danh sách các client đã kết nối
        private bool isRunning = false;
        public Action<string> OnStatusChange;
        private string currentFolder;
        public void SetFolder(string folder)
        {
            currentFolder = folder;
        }
        private void SendFolder(TcpClient client)
        {
            var stream = client.GetStream();
            string msg = $"FOLDER|{currentFolder}\n";
            byte[] data = Encoding.UTF8.GetBytes(msg);

            stream.Write(data, 0, data.Length);
        }
        private void HandleClient(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();
                byte[] buffer = new byte[4096];

                while (isRunning)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string msg = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    foreach (var line in msg.Split('\n'))
                    {
                        string cmd = line.Trim();
                        if (cmd == "GET_FOLDER")
                        {
                            SendFolder(client);
                        }
                        if (cmd.StartsWith("GET_HISTORY|"))
                        {
                            string folder = cmd.Substring("GET_HISTORY|".Length);
                            SendHistory(client, folder);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                clients.Remove(client);
                client.Close();
                OnStatusChange?.Invoke($"Client Connected ({clients.Count})");
            }
        }
        private void SendHistory(TcpClient client, string folder)
        {
            var logs = DbHelper.GetByFolder(folder);
            var stream = client.GetStream();
            foreach (var log in logs)
            {
                string json = JsonConvert.SerializeObject(log) + "\n";
                byte[] data = Encoding.UTF8.GetBytes(json);
                lock (_lock)
                {
                    stream.Write(data, 0, data.Length);
                }
                Thread.Sleep(2);
            }
        }
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
                        Task.Run(() => HandleClient(client));
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
            //DbHelper.Insert(change.FileName, change.Action, change.Time.ToString());
            // Serialize đối tượng FileChange thành JSON và chuyển đổi thành byte array để gửi qua mạng
            string json = JsonConvert.SerializeObject(change) + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);
            // Gửi dữ liệu đến tất cả các client đã kết nối.
            foreach (var client in clients.ToList())
            {
                try
                {
                    var stream = client.GetStream();
                    lock (_lock)
                    {
                        stream.Write(data, 0, data.Length);
                    }
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

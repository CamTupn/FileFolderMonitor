using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp.Core
{
    // Quản lý kết nối TCP đến server, nhận dữ liệu JSON và deserialize thành FileChange.
    // Hỗ trợ tự động reconnect khi mất kết nối.
    public class ClientCore
    {
        // ── Sự kiện / Callback ──────────────────────────────────────────────
        // Gọi khi nhận được thông báo thay đổi file từ server
        public Action<FileChange> OnChangeReceived;
        // Gọi khi trạng thái kết nối thay đổi (truyền chuỗi mô tả trạng thái)
        public Action<string> OnStatusChange;
        public Action<string> OnFolderReceived;
        // ── Trạng thái nội bộ ────────────────────────────────────────────────
        private TcpClient _client;
        private bool _isRunning;
        private string _host;
        private int _port;

        // Buffer để ghép các TCP chunk thành message hoàn chỉnh
        private readonly StringBuilder _buffer = new StringBuilder();

        // ── Kết nối / Ngắt kết nối ───────────────────────────────────────────
        public void Connect(string host, int port)
        {
            if (_isRunning) return;
            _host = host;
            _port = port;
            _isRunning = true;

            Task.Run(() => ReceiveLoop());
        }

        public void Disconnect()
        {
            _isRunning = false;
            try
            {
                _client?.Close();
            }
            catch { }

            _client = null; 
            OnStatusChange?.Invoke("Disconnected");
        }

        // ── Vòng lặp nhận dữ liệu (có tự động reconnect) ────────────────────
        private void ReceiveLoop()
        {
            while (_isRunning)
            {
                try
                {
                    _client = new TcpClient();
                    _client.Connect(_host, _port);
                    OnStatusChange?.Invoke($"Connected to {_host}:{_port}");

                    var stream = _client.GetStream();
                    byte[] chunk = new byte[4096];

                    // Đọc liên tục từ stream cho đến khi mất kết nối
                    while (_isRunning)
                    {
                        int bytesRead = stream.Read(chunk, 0, chunk.Length);
                        if (bytesRead == 0) break; // Server đóng kết nối

                        // Thêm chunk vào buffer và xử lý message hoàn chỉnh
                        _buffer.Append(Encoding.UTF8.GetString(chunk, 0, bytesRead));
                        ProcessBuffer();
                    }
                }
                catch (Exception ex)
                {
                    if (!_isRunning) break;  // Ngắt kết nối chủ động → không reconnect
                    OnStatusChange?.Invoke($"Lost connection.({ex.Message})");
                }
                finally
                {
                    try { _client?.Close(); } catch { }
                }

                for (int i = 0; i < 30 && _isRunning; i++)
                {
                    Thread.Sleep(100); 
                }
            }
        }

        // ── Xử lý buffer: tách từng JSON object hoàn chỉnh theo delimiter '\n' ──
        // Server phải gửi mỗi message kết thúc bằng '\n'.
        // Nếu server chưa gửi '\n', client vẫn hoạt động được nhờ thử parse trực tiếp.
        private void ProcessBuffer()
        {
            while (true)
            {
                string bufferStr = _buffer.ToString();
                int idx = bufferStr.IndexOf('\n');
                // chưa đủ 1 message
                if (idx < 0) break;
                // lấy 1 dòng hoàn chỉnh
                string raw = bufferStr.Substring(0, idx).Trim();
                // xóa khỏi buffer
                _buffer.Remove(0, idx + 1);
                if (string.IsNullOrEmpty(raw)) continue;
                // xử lý command trước (không phải JSON)
                if (raw.StartsWith("FOLDER|"))
                {
                    string folder = raw.Substring("FOLDER|".Length);
                    OnFolderReceived?.Invoke(folder);
                    continue;
                }
                //xử lý JSON (history + realtime)
                if (!TryDeserializeAndNotify(raw))
                {
                    Console.WriteLine("Parse lỗi: " + raw);
                }
            }
        }
        public void RequestFolder()
        {
            if (_client == null || !_client.Connected) return;

            string cmd = "GET_FOLDER\n";
            byte[] data = Encoding.UTF8.GetBytes(cmd);

            var stream = _client.GetStream();
            stream.Write(data, 0, data.Length);
        }
        private bool TryDeserializeAndNotify(string json)
        {
            try
            {
                var list = JsonConvert.DeserializeObject<List<FileChange>>(json);

                if (list != null)
                {
                    foreach (var item in list)
                        OnChangeReceived?.Invoke(item);

                    return true;
                }
            }
            catch { }

            try
            {
                var change = JsonConvert.DeserializeObject<FileChange>(json);
                if (change != null)
                {
                    OnChangeReceived?.Invoke(change);
                    return true;
                }
            }
            catch { }

            return false;
        }
        public void RequestHistory(string folder)
        {
            if (_client == null || !_client.Connected) return;
            string cmd = $"GET_HISTORY|{folder}\n";
            byte[] data = Encoding.UTF8.GetBytes(cmd);

            var stream = _client.GetStream();
            stream.Write(data, 0, data.Length);
        }
    }
}
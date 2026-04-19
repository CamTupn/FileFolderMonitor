using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            _host = host;
            _port = port;
            _isRunning = true;

            // Chạy vòng lặp nhận dữ liệu trên background thread để không block UI
            Task.Run(() => ReceiveLoop());
        }

        public void Disconnect()
        {
            _isRunning = false;
            try { _client?.Close(); } catch { }
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
                    OnStatusChange?.Invoke($"Lost connection. Retrying... ({ex.Message})");
                }
                finally
                {
                    try { _client?.Close(); } catch { }
                    _buffer.Clear();
                }

                // Chờ 3 giây trước khi thử kết nối lại
                if (_isRunning)
                    Thread.Sleep(3000);
            }
        }

        // ── Xử lý buffer: tách từng JSON object hoàn chỉnh theo delimiter '\n' ──
        // Server phải gửi mỗi message kết thúc bằng '\n'.
        // Nếu server chưa gửi '\n', client vẫn hoạt động được nhờ thử parse trực tiếp.
        private void ProcessBuffer()
        {
            string raw = _buffer.ToString();

            // Trường hợp server gửi message kết thúc bằng '\n' (delimiter chuẩn)
            int newlineIdx;
            while ((newlineIdx = raw.IndexOf('\n')) >= 0)
            {
                string jsonPart = raw.Substring(0, newlineIdx).Trim();
                raw = raw.Substring(newlineIdx + 1);

                if (!string.IsNullOrEmpty(jsonPart))
                    TryDeserializeAndNotify(jsonPart);
            }

            // Trường hợp server KHÔNG dùng delimiter (code server hiện tại):
            // Thử parse toàn bộ buffer như 1 JSON object. Nếu thành công thì xử lý và xóa buffer.
            if (raw.Length > 0)
            {
                string trimmed = raw.Trim();
                if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
                {
                    if (TryDeserializeAndNotify(trimmed))
                        raw = string.Empty;
                }
            }

            _buffer.Clear();
            _buffer.Append(raw);
        }

        private bool TryDeserializeAndNotify(string json)
        {
            try
            {
                var change = JsonConvert.DeserializeObject<FileChange>(json);
                if (change != null)
                {
                    OnChangeReceived?.Invoke(change);
                    return true;
                }
            }
            catch { /* JSON chưa hoàn chỉnh hoặc lỗi – giữ lại trong buffer */ }
            return false;
        }
    }
}
using ServerApp.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Core
{
    // Lớp này sử dụng FileSystemWatcher để theo dõi các thay đổi trong thư mục được chỉ định, bao gồm tạo, xóa, sửa và đổi tên file hoặc thư mục.
    // Khi có sự kiện xảy ra, nó sẽ gửi thông tin về thay đổi đó đến ServerCore để broadcast cho các client.
    public class FileWatcher
    {
        private FileSystemWatcher watcher; // đối tượng để theo dõi thư mục
        private ServerCore server; // tham chiếu đến ServerCore để gửi thông tin thay đổi
        // Dictionary để lưu thời gian của sự kiện cuối cùng cho mỗi file, giúp chống spam khi có nhiều sự kiện liên tiếp xảy ra trong thời gian ngắn
        private Dictionary<string, DateTime> lastEvent = new Dictionary<string, DateTime>();
        // HashSet để lưu các file tạm thời (có tên bắt đầu bằng "~$" hoặc kết thúc bằng ".tmp"), giúp phân biệt giữa sự kiện tạo file tạm và sự kiện sửa file thực sự
        private HashSet<string> tempFiles = new HashSet<string>();
        // Callback để gửi log thay đổi file lên UI
        public Action<string> OnLog;
        public FileWatcher(ServerCore server)
        {
            this.server = server;
        }

        public void Start(string path)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.IncludeSubdirectories = true;
            // Theo dõi tất cả các loại file
            watcher.Filter = "*.*";
            // Theo dõi các sự kiện liên quan đến tên file, tên thư mục, thời gian sửa đổi cuối cùng và kích thước file
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Created += OnCreated; 
            watcher.Deleted += OnDeleted;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;
            // Bắt đầu theo dõi
            watcher.EnableRaisingEvents = true;
        }

        // ================= CREATED =================
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            // Nếu file mới tạo là file tạm (bắt đầu bằng "~$" hoặc kết thúc bằng ".tmp"), thêm vào tempFiles và không gửi thông tin lên server.
            // Khi file này được rename thành tên thực, sẽ coi là sự kiện MODIFY thay vì CREATED.
            if (IsTemp(e.Name))
            {
                tempFiles.Add(e.FullPath);
                return;
            }
            // Xác định loại đối tượng được tạo là file hay folder dựa trên việc có phần mở rộng hay không
            string type = Path.HasExtension(e.Name) ? "File" : "Folder";
            // Gửi log
            string folder = watcher.Path;
            OnLog?.Invoke($"{Now()} | {type} | {e.FullPath} | Created");
            DbHelper.Insert(e.FullPath, "Created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), folder);
            // Gửi thông tin thay đổi lên server để broadcast cho các client
            server.Broadcast(new FileChange
            {
                FileName = e.FullPath,
                Action = "Created",
                Time = DateTime.Now
            });
        }

        // ================= DELETED =================
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string folder = watcher.Path;
            // Nếu file bị xóa là file tạm, không gửi thông tin lên server.
            if (IsTemp(e.Name)) return;
            // Xác định loại đối tượng bị xóa là file hay folder dựa trên việc có phần mở rộng hay không
            string type = Path.HasExtension(e.Name) ? "File" : "Folder";
            // Gửi log
            OnLog?.Invoke($"{Now()} | {type} | {e.FullPath} | Deleted");
            DbHelper.Insert(e.FullPath, "Deleted", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), folder);
            // Gửi thông tin thay đổi lên server để broadcast cho các client
            server.Broadcast(new FileChange
            {
                FileName = e.FullPath,
                Action = "Deleted",
                Time = DateTime.Now
            });
        }

        // ================= CHANGED =================
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string folder = watcher.Path;
            // Nếu file bị thay đổi là file tạm, không gửi thông tin lên server.
            if (IsTemp(e.Name)) return;
            // Chỉ quan tâm đến sự kiện thay đổi của file, bỏ qua folder.
            if (!Path.HasExtension(e.Name)) return;
            // Sử dụng đường dẫn đầy đủ làm key để theo dõi thời gian của sự kiện cuối cùng, giúp chống spam khi có nhiều sự kiện liên tiếp xảy ra trong thời gian ngắn.
            string key = e.FullPath;
            // chống spam
            if (lastEvent.ContainsKey(key))
            {
                if ((DateTime.Now - lastEvent[key]).TotalMilliseconds < 500)
                    return;
            }
            lastEvent[key] = DateTime.Now;
            OnLog?.Invoke($"{Now()} | File | {e.FullPath} | Modified");
            DbHelper.Insert(e.FullPath, "Modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), folder);
            server.Broadcast(new FileChange
            {
                FileName = e.FullPath,
                Action = "Modified",
                Time = DateTime.Now
            });
        }

        // ================= RENAMED =================
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            string folder = watcher.Path;
            if (IsTemp(e.Name)) return;
            string key = e.FullPath;
            if (lastEvent.ContainsKey(key))
            {
                if ((DateTime.Now - lastEvent[key]).TotalMilliseconds < 500)
                    return;
            }
            lastEvent[key] = DateTime.Now;
            // nếu rename từ file temp => coi là MODIFY
            if (tempFiles.Contains(e.OldFullPath) ||
               e.OldName.Contains(".tmp") ||
               e.OldName.StartsWith("~$"))
            {
                tempFiles.Remove(e.OldFullPath);
                OnLog?.Invoke($"{Now()} | File | {e.FullPath} | Modified");
                DbHelper.Insert(e.FullPath, "Modified", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), folder);
                server.Broadcast(new FileChange
                {
                    FileName = e.FullPath,
                    Action = "Modified",
                    Time = DateTime.Now
                });

                return;
            }
            // rename bình thường
            OnLog?.Invoke($"{Now()} | {e.OldFullPath} -> {e.FullPath} | Renamed");
            DbHelper.Insert($"{e.OldFullPath} -> {e.FullPath}", "Renamed", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), folder);
            server.Broadcast(new FileChange
            {
                FileName = $"{e.OldFullPath} -> {e.FullPath}",
                Action = "Renamed",
                Time = DateTime.Now
            });
        }

        // ================= HELPER =================
        // Hàm này kiểm tra nếu tên file bắt đầu bằng "~$" hoặc kết thúc bằng ".tmp" thì coi là file tạm.
        // thường được tạo ra khi người dùng đang chỉnh sửa một file (ví dụ như file Excel), giúp phân biệt giữa sự kiện tạo file tạm và sự kiện sửa file thực sự.
        private bool IsTemp(string name) 
        {
            return name.StartsWith("~$") || name.EndsWith(".tmp");
        }

        private string Now() 
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        // Nhấn nút Stop
        public void Stop() 
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }
    }
}

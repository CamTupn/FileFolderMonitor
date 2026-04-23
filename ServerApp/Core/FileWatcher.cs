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
    //Dùng FileSystemWatcher để theo dõi các thay đổi trong thư mục và gửi thông tin về ServerCore
    public class FileWatcher
    {
        private FileSystemWatcher watcher;
        private ServerCore server; // tham chiếu đến ServerCore để gửi thông tin thay đổi
        // Dictionary để lưu thời gian của sự kiện cuối cùng cho mỗi file, giúp chống spam khi có nhiều sự kiện liên tiếp xảy ra trong thời gian ngắn
        private Dictionary<string, DateTime> lastEvent = new Dictionary<string, DateTime>();
        // HashSet để lưu các file tạm thời (bắt đầu bằng "~$" hoặc kết thúc bằng ".tmp")
        private HashSet<string> tempFiles = new HashSet<string>();
        // gửi log thay đổi file lên UI
        public Action<string> OnLog;
        public FileWatcher(ServerCore server)
        {
            this.server = server;
        }
        // bắt đầu theo dõi
        public void Start(string path)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;
            watcher.IncludeSubdirectories = true; // theo dõi cả thư mục con
            watcher.Filter = "*.*";
            watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite | NotifyFilters.Size;
            watcher.Created += OnCreated; 
            watcher.Deleted += OnDeleted;
            watcher.Changed += OnChanged;
            watcher.Renamed += OnRenamed;
            watcher.EnableRaisingEvents = true; 
        }
        // kiểm tra file tạm (bắt đầu bằng "~$" hoặc kết thúc bằng ".tmp")
        private bool IsTemp(string name)
        {
            return name.StartsWith("~$") || name.EndsWith(".tmp");
        }

        // CREATED 
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            // bỏ qua file tạm (lưu lại xử lí sau)
            if (IsTemp(e.Name))
            {
                tempFiles.Add(e.FullPath);
                return;
            }
            // Xác định đối tượng được tạo là file hay folder 
            string type = Path.HasExtension(e.Name) ? "File" : "Folder";
            string folder = watcher.Path;
            OnLog?.Invoke($"{Now()} | {type} | {e.FullPath} | Created"); // Gửi log lên UI
            DbHelper.Insert(e.FullPath, "Created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), folder); // lưu vào database
            // Gửi thông tin thay đổi lên server để broadcast cho các client
            server.Broadcast(new FileChange
            {
                FileName = e.FullPath,
                Action = "Created",
                Time = DateTime.Now
            });
        }

        // DELETED
        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            string folder = watcher.Path;
            // bỏ qua file tạm
            if (IsTemp(e.Name)) return;
            // Xác định loại đối tượng
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

        // CHANGED
        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            string folder = watcher.Path;
            // bỏ qua file tạm
            if (IsTemp(e.Name)) return;
            // bỏ qua folder.
            if (!Path.HasExtension(e.Name)) return;
            // Sử dụng đường dẫn đầy đủ làm key để theo dõi thời gian của sự kiện cuối cùng
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

        // RENAMED 
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
        private string Now() 
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        // Dừng theo dõi
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

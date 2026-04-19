using System;

namespace ClientApp.Core
{
    // Model tương ứng với FileChange bên ServerApp – dùng để deserialize JSON nhận từ server
    public class FileChange
    {
        public string FileName { get; set; }
        public string Action { get; set; }
        public DateTime Time { get; set; }
    }
}
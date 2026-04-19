using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Core
{
    // Lớp này đại diện cho một thay đổi của file, bao gồm tên file, hành động (thêm, sửa, xóa) và thời gian xảy ra
    public class FileChange
    {
        // Tên file hoặc thư mục bị thay đổi
        public string FileName { get; set; }
        // Hành động thay đổi: Created, Deleted, Modified, Renamed
        public string Action { get; set; }
        // Thời gian xảy ra thay đổi
        public DateTime Time { get; set; }
    }
}

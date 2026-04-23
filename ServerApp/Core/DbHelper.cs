using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.Core
{
    internal class DbHelper
    {
        private static string connStr = "Data Source=log.db";
        // tạo bảng
        public static void Init()
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = @"CREATE TABLE IF NOT EXISTS Logs (
                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                FileName TEXT,
                                Action TEXT,
                                Time TEXT,
                                Folder TEXT)";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }
        // lưu dữ liệu vào database
        public static void Insert(string file, string action, string time, string folder)
        {
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = "INSERT INTO Logs(FileName, Action, Time, Folder) VALUES (@f,@a,@t,@fo)";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@f", file);
                cmd.Parameters.AddWithValue("@a", action);
                cmd.Parameters.AddWithValue("@t", time);
                cmd.Parameters.AddWithValue("@fo", folder);
                cmd.ExecuteNonQuery();
            }
        }
        // lấy tất cả dữ liệu từ database
        public static List<FileChange> GetAll()
        {
            List<FileChange> list = new List<FileChange>();

            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();

                string sql = "SELECT * FROM Logs ORDER BY Id DESC";
                var cmd = new SQLiteCommand(sql, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(new FileChange
                    {
                        FileName = reader["FileName"].ToString(),
                        Action = reader["Action"].ToString(),
                        Time = DateTime.Parse(reader["Time"].ToString())
                    });
                }
            }
            return list;
        }
        // lấy dữ liệu theo folder
        public static List<FileChange> GetByFolder(string folder)
        {
            List<FileChange> list = new List<FileChange>();
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                string sql = "SELECT * FROM Logs WHERE Folder = @fo ORDER BY Id DESC";
                var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.AddWithValue("@fo", folder);

                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(new FileChange
                    {
                        FileName = reader["FileName"].ToString(),
                        Action = reader["Action"].ToString(),
                        Time = DateTime.Parse(reader["Time"].ToString())
                    });
                }
            }
            return list;
        }
    }
}

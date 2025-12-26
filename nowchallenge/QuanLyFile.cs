using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Dùng để đọc, ghi và xóa các dữ liệu trong file.
// Lưu ý đến cách đọc file vì với từng bài thì họ có cách sử dụng kiểu dữ liệu khác nhau

namespace nowchallenge
{
    public class QuanLyFile
    {
        public static void GhiFile(string path, IGhiFile obj)
        {
            using (StreamWriter sw=new StreamWriter (path, true))
            {
                sw.WriteLine(obj.ToFileString());
            }
        }

        // Hàm ghi lại toàn bộ file
        public static void GhiDeFile<T>(string path, List<T> ds) where T : IGhiFile
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                foreach (var item in ds)
                {
                    sw.WriteLine(item.ToFileString());
                }
            }
        }

        // Hàm đọc file cho class tài xế
        public static List<TaiXe> DocFile_TaiXe(string filePath)
        {
            List<TaiXe> ds = new List<TaiXe>();

            if (!File.Exists(filePath))
            {
                throw new Exception("Danh sách không tồn tại");
            }

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] p = line.Split('|');
                        if (p.Length != 9) continue;

                        ds.Add(new TaiXe(
                                int.Parse(p[0].Trim()),
                                p[1].Trim(),
                                double.Parse(p[2].Trim()),
                                double.Parse(p[3].Trim()),
                                double.Parse(p[4].Trim()),
                                p[5].Trim(),
                                int.Parse(p[6].Trim()),
                                int.Parse(p[7].Trim()),
                                bool.Parse(p[8].Trim())
                            ));
                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine(" Không thể mở file (đang bị sử dụng hoặc lỗi I/O).");
            }

            return ds;
        }

        // Hàm đọc file cho class khách hàng

        public static List<KhachHang> DocFile_KhachHang(string filePath)
        {
            List<KhachHang> ds = new List<KhachHang>();

            if (!File.Exists(filePath))
            {
                throw new Exception("Danh sách không tồn tại");
            }

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] p = line.Split('|');
                        if (p.Length != 5) continue; // bỏ dòng lỗi format

                        ds.Add(new KhachHang(
                                int.Parse(p[0].Trim()),
                                p[1].Trim(),
                                p[2].Trim(),
                                double.Parse(p[3].Trim()),
                                double.Parse(p[4].Trim())
                            ));

                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine(" Không thể mở file (đang bị sử dụng hoặc lỗi I/O).");
            }

            return ds;
        }

        public static List<ChuyenDi> DocFile_ChuyenDi(string filePath)
        {
            List<ChuyenDi> ds = new List<ChuyenDi>();

            if (!File.Exists(filePath))
            {
                throw new Exception("Danh sách không tồn tại");
            }

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] p = line.Split('|');
                        if (p.Length != 6) continue; // bỏ dòng lỗi format

                        ds.Add(new ChuyenDi(
                                int.Parse(p[0].Trim()),
                                int.Parse(p[1].Trim()),
                                int.Parse(p[2].Trim()),
                                double.Parse(p[3].Trim()),
                                DateTime.Parse(p[5].Trim()
                            )));

                    }
                }
            }
            catch (IOException)
            {
                Console.WriteLine(" Không thể mở file (đang bị sử dụng hoặc lỗi I/O).");
            }

            return ds;
        }

        public static void XoaToanBoFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File không tồn tại", filePath);

            try
            {
                // Ghi rỗng => xóa toàn bộ nội dung
                File.WriteAllText(filePath, string.Empty);
            }
            catch (IOException)
            {
                throw new IOException("Không thể xóa nội dung file (file đang được sử dụng).");
            }
        }
        
        /// Xóa toàn bộ nội dung file nhưng đăng ký thao tác vào LichSuThaoTac để hỗ trợ Undo/Redo.
        /// Program chỉ cần gọi hàm này và cung cấp các callback để cập nhật trạng thái trong bộ nhớ (nếu cần).
        /// doAfter: callback chạy ngay sau khi xóa (ví dụ clear list trong memory)
        /// undoAfter: callback chạy sau khi phục hồi file (ví dụ reload list từ file)
        public static void XoaToanBoFileWithUndo(string filePath, Action doAfter = null, Action undoAfter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File không tồn tại", filePath);

            string backup;
            try
            {
                backup = File.ReadAllText(filePath);
            }
            catch (IOException)
            {
                throw new IOException("Không thể đọc file để tạo bản sao lưu (file đang được sử dụng).");
            }

            Action doAction = () =>
            {
                try
                {
                    File.WriteAllText(filePath, string.Empty);
                }
                catch (IOException)
                {
                    throw new IOException("Không thể xóa nội dung file (file đang được sử dụng).");
                }
                doAfter?.Invoke();
            };

            Action undoAction = () =>
            {
                try
                {
                    File.WriteAllText(filePath, backup);
                }
                catch (IOException)
                {
                    throw new IOException("Không thể phục hồi file từ bản sao lưu.");
                }
                undoAfter?.Invoke();
            };

            // Đăng ký và thực thi ngay (LichSuThaoTac sẽ thực thi doAction khi AddOperation được gọi)
            LichSuThaoTac.Instance.AddOperation(doAction, undoAction);
        }
    }
}
    

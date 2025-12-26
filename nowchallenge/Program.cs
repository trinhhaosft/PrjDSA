using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nowchallenge
{
    internal class Program
    {
        // lưu trữ lịch sử thao tác để hỗ trợ Undo/Redo
        static LichSuThaoTac lichSuThaoTac = LichSuThaoTac.Instance;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            string filePathKhachHang = @"D:\nowchallenge\KhachHang.txt";
            string filePathTaiXe = @"D:\nowchallenge\TaiXe.txt";

            EnsureFileExists(filePathKhachHang);
            EnsureFileExists(filePathTaiXe);

            var qlkh = new QuanLyKhachHang();
            var qltx = new QuanLyTaiXe();
            var dv = new DichVuDatXe();

            // Tải dữ liệu từ file bằng các phương thức đã có.
            // (Các phương thức này có thể gán trực tiếp vào thuộc tính trong lớp quản lý.)
            try { qlkh.KhachHangs = QuanLyFile.DocFile_KhachHang(filePathKhachHang); } catch (Exception ex) { Console.WriteLine("Không thể đọc file khách hàng: " + ex.Message); }
            try { qltx.TaiXes = QuanLyFile.DocFile_TaiXe(filePathTaiXe); } catch (Exception ex) { Console.WriteLine("Không thể đọc file tài xế: " + ex.Message); }

            while (true)
            {
                Console.WriteLine("\n===== NOW CHALLENGE MENU =====");
                Console.WriteLine("1. Quản lý khách hàng");
                Console.WriteLine("2. Quản lý tài xế");
                Console.WriteLine("3. Dịch vụ đặt xe");
                Console.WriteLine("0. Thoát");
                Console.Write("Chọn: ");

                var choice = Console.ReadLine();
                if (choice == "0") break;

                switch (choice)
                {
                    case "1":
                        MenuManageCustomers(qlkh, filePathKhachHang);
                        break;
                    case "2":
                        MenuManageDrivers(qltx, filePathTaiXe);
                        break;
                    case "3":
                        MenuBookingServices(dv, qlkh, qltx, filePathTaiXe);
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }

            Console.WriteLine("Kết thúc chương trình.");
        }

        static void EnsureFileExists(string path)
        {
            if (!File.Exists(path))
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.Create(path).Close();
            }
        }

        // Các hàm wrapper mỏng gọi vào lớp đã có — không triển khai lại logic ở đây.
        static void MenuManageCustomers(QuanLyKhachHang qlkh, string filePath)
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ KHÁCH HÀNG ---");
                Console.WriteLine("1. Hiển thị toàn bộ");
                Console.WriteLine("2. Thêm khách hàng");
                Console.WriteLine("3. Cập nhật khách hàng (theo ID)");
                Console.WriteLine("4. Xóa khách hàng (theo ID)");
                Console.WriteLine("5. Hiển thị khách hàng theo quận");
                Console.WriteLine("6. Hiển thị Top K khách hàng");
                Console.WriteLine("7. Undo (hoàn tác)");
                Console.WriteLine("8. Redo (làm lại)");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                var ch = Console.ReadLine();
                if (ch == "0") return;

                try
                {
                    switch (ch)
                    {
                        case "1":
                            qlkh.HienThiToanBoKhachHang();
                            break;

                        case "2":
                            var newKh = qlkh.NhapKhachHang();
                            qlkh.ThemKhachHangMoi(newKh, filePath);
                            break;

                        case "3":
                            Console.Write("Nhập ID khách hàng cần cập nhật: ");
                            if (int.TryParse(Console.ReadLine(), out int idCapNhat))  
                                qlkh.CapNhatThongTinKhachHang(idCapNhat, filePath);
                            else
                                Console.WriteLine("ID không hợp lệ.");
                            break;

                        case "4":
                            Console.Write("Nhập ID khách hàng cần xóa: ");
                            if (int.TryParse(Console.ReadLine(), out int idXoa))
                                qlkh.XoaKhachHang(idXoa, filePath);
                            else
                                Console.WriteLine("ID không hợp lệ.");
                            break;

                        case "5":
                            Console.Write("Nhập quận cần hiển thị: ");
                            var quan = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(quan))
                            {
                                Console.WriteLine("Quận không hợp lệ.");
                                break;
                            }

                            int total = qlkh.DemKhachHangTheoQuan(quan);
                            if (total == 0)
                            {
                                Console.WriteLine("Không tìm thấy khách hàng ở quận này.");
                                break;
                            }

                            Console.WriteLine($"Tổng {total} khách hàng ở quận '{quan}'.");
                            const int pageSize = 10;
                            int page = 1;
                            while (true)
                            {
                                var pageList = qlkh.LayKhachHangTheoQuan_Paged(quan, page, pageSize);
                                if (pageList == null || pageList.Count == 0)
                                {
                                    Console.WriteLine("Không còn dữ liệu để hiển thị.");
                                    break;
                                }

                                Console.WriteLine(KhachHang.Tieude_khachhang());
                                Console.WriteLine(KhachHang.Separator_khachhang());

                                foreach (var k in pageList)
                                    Console.WriteLine(k.ToString());

                                int shown = page * pageSize;
                                if (shown >= total) break;

                                Console.Write($"Hiển thị {Math.Min(shown, total)}/{total}. Xem trang tiếp theo? (y/n): ");
                                var resp = Console.ReadLine();
                                if (string.IsNullOrWhiteSpace(resp) || resp.Trim().ToLower() != "y") break;
                                page++;
                            }
                            break;

                        case "6":
                            try
                            {
                                if (qlkh.KhachHangs == null || qlkh.KhachHangs.Count == 0)
                                {
                                    Console.WriteLine("Không có dữ liệu khách hàng.");
                                    break;
                                }

                                Console.Write("Nhập K (số lượng khách muốn hiển thị): ");
                                if (!int.TryParse(Console.ReadLine(), out int k) || k <= 0)
                                {
                                    Console.WriteLine("K không hợp lệ.");
                                    break;
                                }

                                Console.Write("Hiển thị (1=K đầu, 2=K cuối): ");
                                var s = Console.ReadLine();
                                int luaChon;
                                if (!int.TryParse(s, out luaChon) || (luaChon != 1 && luaChon != 2))
                                    luaChon = 1;

                                qlkh.HienThiTopK_KhachHang(k, luaChon);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi khi hiển thị Top K: " + ex.Message);
                            }
                            break;

                        case "7":
                            try
                            {
                                lichSuThaoTac.Undo();
                              
                                try { qlkh.KhachHangs = QuanLyFile.DocFile_KhachHang(filePath); } catch { /* ignore */ }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Undo thất bại: " + ex.Message);
                            }
                            break;

                        case "8":
                            try
                            {
                                lichSuThaoTac.Redo();
                              
                                try { qlkh.KhachHangs = QuanLyFile.DocFile_KhachHang(filePath); } catch { /* ignore */ }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Redo thất bại: " + ex.Message);
                            }
                            break;

                        default:
                            Console.WriteLine("Lựa chọn không hợp lệ.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi: " + ex.Message);
                }
            }
        }

        static void MenuManageDrivers(QuanLyTaiXe qltx, string filePath)
        {
            while (true)
            {
                Console.WriteLine("\n--- QUẢN LÝ TÀI XẾ ---");
                Console.WriteLine("1. Hiển thị toàn bộ");
                Console.WriteLine("2. Thêm tài xế");
                Console.WriteLine("3. Cập nhật tài xế");
                Console.WriteLine("4. Xóa tài xế");
                Console.WriteLine("5. Sắp xếp theo rating");
                Console.WriteLine("6. Hiển thị lịch sử chuyến đi");
                Console.WriteLine("7. Hiển thị Top K tài xế");
                Console.WriteLine("8. Undo (hoàn tác)");
                Console.WriteLine("9. Redo (làm lại)");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                var ch = Console.ReadLine();
                if (ch == "0") return;

                try
                {
                    switch (ch)
                    {
                        case "1":
                            qltx.HienThiToanBoTaiXe();
                            break;

                        case "2":
                            var newTx = qltx.NhapTaiXe();
                            qltx.ThemTaiXe(newTx, filePath);
                            break;

                        case "3":
                            qltx.CapNhatTaiXe(filePath);
                            break;

                        case "4":
                            Console.Write("Nhập ID tài xế cần xóa: ");
                            if (int.TryParse(Console.ReadLine(), out int idXoa))
                                qltx.XoaTaiXe(idXoa, filePath);
                            else
                                Console.WriteLine("ID không hợp lệ.");
                            break;

                        case "5":
                            Console.Write("Hiển thị (1=giảm dần, 0=tăng dần): ");
                            var dir = Console.ReadLine();
                            bool giamDan = dir == "1";
                            qltx.SapXepTheoRating(giamDan);
                            qltx.HienThiToanBoTaiXe();
                            break;

                        case "6":
                            Console.Write("Nhập ID tài xế để xem lịch sử: ");
                            if (int.TryParse(Console.ReadLine(), out int idTx))
                            {
                                var tx = qltx.TaiXes?.Find(t => t.MaTaiXe == idTx);
                                if (tx != null) qltx.HienThiLichSuChuyenDi(tx);
                                else Console.WriteLine("Không tìm thấy tài xế.");
                            }
                            else Console.WriteLine("ID không hợp lệ.");
                            break;

                        case "7":
                            try
                            {
                                if (qltx.TaiXes == null || qltx.TaiXes.Count == 0)
                                {
                                    Console.WriteLine("Không có dữ liệu tài xế.");
                                    break;
                                }

                                Console.Write("Nhập K (số lượng top tài xế muốn hiển thị): ");
                                if (!int.TryParse(Console.ReadLine(), out int k) || k <= 0)
                                {
                                    Console.WriteLine("K không hợp lệ.");
                                    break;
                                }

                                Console.Write("Hiển thị (1=Top đầu, 2=Top cuối): ");
                                var s = Console.ReadLine();
                                int luaChon;
                                if (!int.TryParse(s, out luaChon) || (luaChon != 1 && luaChon != 2))
                                    luaChon = 1;

                                qltx.HienThiTopK_Taixe(k, luaChon);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Lỗi khi hiển thị Top K: " + ex.Message);
                            }
                            break;

                        case "8":
                            try
                            {
                                lichSuThaoTac.Undo();
                              
                                try { qltx.TaiXes = QuanLyFile.DocFile_TaiXe(filePath); } catch { /* ignore */ }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Undo thất bại: " + ex.Message);
                            }
                            break;

                        case "9":
                            try
                            {
                                lichSuThaoTac.Redo();
                               
                                try { qltx.TaiXes = QuanLyFile.DocFile_TaiXe(filePath); } catch { /* ignore */ }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Redo thất bại: " + ex.Message);
                            }
                            break;

                        default:
                            Console.WriteLine("Lựa chọn không hợp lệ.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi: " + ex.Message);
                }
            }
        }

        static void MenuBookingServices(DichVuDatXe dv, QuanLyKhachHang qlkh, QuanLyTaiXe qltx, string filePathTaiXe)
        {
            while (true)
            {
                Console.WriteLine("\n--- DỊCH VỤ ĐẶT XE ---");
                Console.WriteLine("1. Tìm tài xế phù hợp");
                Console.WriteLine("2. Đặt xe tự động");
                Console.WriteLine("3. Đặt xe thủ công");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");

                var ch = Console.ReadLine();
                if (ch == "0") return;

                try
                {
                    switch (ch)
                    {
                        case "1":
                            DichVuDatXe.TaiPhuHop_TuongTac(qlkh, qltx);
                            break;

                        case "2":
                            DichVuDatXe.DatXeTuDong_TuongTac(qlkh, qltx);
                            break;

                        case "3":
                            DichVuDatXe.DatXeThuCong_TuongTac(qlkh, qltx);
                            break;

                        default:
                            Console.WriteLine("Lựa chọn không hợp lệ.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi: " + ex.Message);
                }
            }
        }
    }
}
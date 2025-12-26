using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace nowchallenge
{
    public partial class QuanLyKhachHang
    {
        // Property duy nhất để lưu danh sách khách hàng
        public List<KhachHang> KhachHangs { get; set; } = new List<KhachHang>();

        //string tepKhachHang = "D:\\DSA\\NowChallenge\\nowchallenge\\KhachHang.txt";

        // Hiển thị top k khách hàng đầu hoặc cuối
        public void HienThiTopK_KhachHang(int k, int luaChon)
        {
            int n = KhachHangs.Count;

            if (n == 0)
            {
                Console.WriteLine("Danh sách rỗng!");
                return;
            }

            if (k <= 0 || k > n)
            {
                Console.WriteLine("K không hợp lệ");
                return;
            }

            if (luaChon == 1)
            {
                Console.WriteLine($"Top {k} khách hàng đầu tiên:");
                Console.WriteLine(KhachHang.Tieude_khachhang());
                Console.WriteLine(KhachHang.Separator_khachhang());
                for (int i = 0; i < k; i++)
                    Console.WriteLine(KhachHangs[i].ToString());
            }
            else if (luaChon == 2)
            {
                Console.WriteLine($"Top {k} khách hàng cuối:");
                Console.WriteLine(KhachHang.Tieude_khachhang());
                Console.WriteLine(KhachHang.Separator_khachhang());
                for (int i = n - k; i < n; i++)
                    Console.WriteLine(KhachHangs[i].ToString());
            }
            else
            {
                Console.WriteLine("Lựa chọn không hợp lệ (1: K đầu, 2: K cuối)");
            }
        }

        // Tìm khách hàng theo tên
        public KhachHang TimKhachHangTheoTen(string key)
        {
            foreach (var kh in KhachHangs)
                if (string.Compare(key, kh.TenKhachHang) == 0)
                    return kh;

            return null;
        }

        // Tìm khách hàng theo ID
        public KhachHang TimKhachHangTheoID(int idKey)
        {
            foreach (var kh in KhachHangs)
                if (kh.MaKhachHang == idKey)
                    return kh;

            return null;
        }

        // Kiểm tra ID đã tồn tại chưa
        public bool KiemTraID(int maKhachHang)
        {
            foreach (var kh in KhachHangs)
                if (kh.MaKhachHang == maKhachHang)
                    return true;

            return false;
        }

        // Nhập khách hàng mới
        public KhachHang NhapKhachHang()
        {
            while (true)
            {
                int id = 0;
                string ten = "";
                string quan = "";
                double x = 0, y = 0;

                int step = 0; // 0=ID, 1=Ten, 2=Quan, 3=X, 4=Y

                while (true)
                {
                    if (step == 0)
                    {
                        Console.Write("Nhập mã khách hàng (Enter để hủy, / = Redo): ");
                        var input = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(input))
                        {
                            Console.WriteLine("Hủy nhập khách hàng.");
                            return null;
                        }

                        if (input == "/") { step = Math.Min(4, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                        if (!int.TryParse(input, out int tmpId))
                        {
                            Console.WriteLine("ID không hợp lệ. Vui lòng nhập số nguyên.");
                            continue;
                        }
                        if (this.KiemTraID(tmpId))
                        {
                            Console.WriteLine($"Lỗi: ID {tmpId} đã tồn tại! Vui lòng nhập lại.");
                            continue;
                        }

                        LichSuThaoTac.Instance.AddOperation(() => { id = tmpId; }, () => { id = 0; });
                        step++;
                        continue;
                    }

                    if (step == 1)
                    {
                        Console.Write("Nhập tên khách hàng (* = Undo, / = Redo): ");
                        var input = Console.ReadLine();

                        if (input == "*") { step = Math.Max(0, step - 1); LichSuThaoTac.Instance.Undo(); continue; }
                        if (input == "/") { step = Math.Min(4, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                        string tenTmp = input?.Trim() ?? "";
                        if (string.IsNullOrEmpty(tenTmp))
                        {
                            Console.WriteLine("Tên không được để trống."); continue;
                        }
                        if (!tenTmp.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                        {
                            Console.WriteLine("Tên chỉ được chứa chữ và khoảng trắng."); continue;
                        }

                        LichSuThaoTac.Instance.AddOperation(() => { ten = tenTmp; }, () => { ten = ""; });
                        step++;
                        continue;
                    }

                    if (step == 2)
                    {
                        Console.Write("Nhập quận (* = Undo, / = Redo): ");
                        var input = Console.ReadLine();

                        if (input == "*") { step = Math.Max(1, step - 1); LichSuThaoTac.Instance.Undo(); continue; }
                        if (input == "/") { step = Math.Min(4, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                        string quanTmp = input?.Trim() ?? "";
                        if (string.IsNullOrEmpty(quanTmp))
                        {
                            Console.WriteLine("Quận không được để trống."); continue;
                        }

                        LichSuThaoTac.Instance.AddOperation(() => { quan = quanTmp; }, () => { quan = ""; });
                        step++;
                        continue;
                    }

                    if (step == 3)
                    {
                        Console.Write("Nhập tọa độ X (* = Undo, / = Redo): ");
                        var input = Console.ReadLine();

                        if (input == "*") { step = Math.Max(2, step - 1); LichSuThaoTac.Instance.Undo(); continue; }
                        if (input == "/") { step = Math.Min(4, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                        if (!double.TryParse(input, out double tmpX))
                        {
                            Console.WriteLine("Tọa độ X không hợp lệ. Vui lòng nhập số."); continue;
                        }
                        if (tmpX < 0)
                        {
                            Console.WriteLine("Tọa độ X phải >= 0."); continue;
                        }

                        LichSuThaoTac.Instance.AddOperation(() => { x = tmpX; }, () => { x = 0; });
                        step++;
                        continue;
                    }

                    if (step == 4)
                    {
                        Console.Write("Nhập tọa độ Y (* = Undo, / = Redo): ");
                        var input = Console.ReadLine();

                        if (input == "*") { step = Math.Max(3, step - 1); LichSuThaoTac.Instance.Undo(); continue; }
                        if (input == "/") { step = Math.Min(4, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                        if (!double.TryParse(input, out double tmpY))
                        {
                            Console.WriteLine("Tọa độ Y không hợp lệ. Vui lòng nhập số."); continue;
                        }
                        if (tmpY < 0)
                        {
                            Console.WriteLine("Tọa độ Y phải >= 0."); continue;
                        }

                        LichSuThaoTac.Instance.AddOperation(() => { y = tmpY; }, () => { y = 0; });
                        step++;
                        continue;
                    }

                    if (step == 5)
                    {
                        try
                        {
                            return new KhachHang(id, ten, quan, x, y);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("Lỗi dữ liệu: " + ex.Message + " Vui lòng nhập lại toàn bộ thông tin.");
                            step = 0;
                            continue;
                        }
                    }
                }
            }
        }






        // danh sách khách hàng sao chép
        private static List<KhachHang> CloneList(List<KhachHang> src)
        {
            var dst = new List<KhachHang>();
            if (src == null) return dst;
            foreach (var k in src)
            {
                // tạo bản sao từng khách hàng
                dst.Add(new KhachHang(k.MaKhachHang, k.TenKhachHang, k.Quan, k.ToaDoX, k.ToaDoY));
            }
            return dst;
        }

        // Thêm khách hàng mới
        public void ThemKhachHangMoi(KhachHang khachHang, string tepKhachHang)
        {
            if (khachHang == null)
            {
                Console.WriteLine("Không thêm khách hàng.");
                return;
            }

            var before = CloneList(this.KhachHangs);

            Action doAction = () =>
            {
                if (KiemTraID(khachHang.MaKhachHang))
                {
                    Console.WriteLine($"ID: {khachHang.MaKhachHang} đã tồn tại, không thể thêm mới");
                    return;
                }
                this.KhachHangs.Add(khachHang);
                QuanLyFile.XoaToanBoFile(tepKhachHang);
                QuanLyFile.GhiDeFile(tepKhachHang, this.KhachHangs);
                Console.WriteLine("Đã thêm khách hàng mới.");
            };

            Action undoAction = () =>
            {
                this.KhachHangs = CloneList(before);
                QuanLyFile.XoaToanBoFile(tepKhachHang);
                QuanLyFile.GhiDeFile(tepKhachHang, this.KhachHangs);
            };

            LichSuThaoTac.Instance.AddOperation(doAction, undoAction);
        }

        // Cập nhật khách hàng theo ID
        public void CapNhatThongTinKhachHang(int maKhachHang, string tepKhachHang)
        {
            var khCapNhat = TimKhachHangTheoID(maKhachHang);
            while (khCapNhat == null)
            {
                Console.WriteLine($"Không tìm thấy khách hàng có ID: {maKhachHang}");
                Console.Write("Nhập lại ID khách hàng cần cập nhật (Enter để hủy): ");
                var s = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(s))
                {
                    Console.WriteLine("Hủy cập nhật.");
                    return;
                }

                if (!int.TryParse(s.Trim(), out maKhachHang))
                {
                    Console.WriteLine("ID không hợp lệ. Vui lòng nhập số nguyên.");
                    continue;
                }

                khCapNhat = TimKhachHangTheoID(maKhachHang);
            }

            InThongTinCapNhat(khCapNhat);

            bool tiepTuc = true;
            while (tiepTuc)
            {
                Console.WriteLine("\n===== MENU CẬP NHẬT =====");
                Console.WriteLine("1. Sửa tên khách hàng");
                Console.WriteLine("2. Sửa quận");
                Console.WriteLine("3. Sửa tọa độ X");
                Console.WriteLine("4. Sửa tọa độ Y");
                Console.WriteLine("5. Hoàn tất cập nhật");
                Console.Write("Chọn mục: ");
                string chon = Console.ReadLine();

                switch (chon)
                {
                    case "1":
                        Console.Write("Tên mới: ");
                        string tenMoi = Console.ReadLine();
                        if (!string.IsNullOrEmpty(tenMoi))
                            khCapNhat.TenKhachHang = tenMoi;
                        InThongTinCapNhat(khCapNhat);
                        break;

                    case "2":
                        Console.Write("Quận mới: ");
                        string quanMoi = Console.ReadLine();
                        if (!string.IsNullOrEmpty(quanMoi))
                            khCapNhat.Quan = quanMoi;
                        InThongTinCapNhat(khCapNhat);
                        break;

                    case "3":
                        Console.Write("Tọa độ X mới: ");
                        string inputX = Console.ReadLine();
                        if (!string.IsNullOrEmpty(inputX))
                            khCapNhat.ToaDoX = double.Parse(inputX);
                        InThongTinCapNhat(khCapNhat);
                        break;

                    case "4":
                        Console.Write("Tọa độ Y mới: ");
                        string inputY = Console.ReadLine();
                        if (!string.IsNullOrEmpty(inputY))
                            khCapNhat.ToaDoY = double.Parse(inputY);
                        InThongTinCapNhat(khCapNhat);
                        break;

                    case "5":
                        QuanLyFile.XoaToanBoFile(tepKhachHang);
                        QuanLyFile.GhiDeFile(tepKhachHang, KhachHangs);
                        Console.WriteLine("Đã lưu thông tin cập nhật.");
                        tiepTuc = false;
                        break;

                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        // Xóa khách hàng
        public void XoaKhachHang(int maKhachHang, string tepKhachHang)
        {
            var before = CloneList(KhachHangs);
            Action doAction = () =>
            {
                var khCanXoa = TimKhachHangTheoID(maKhachHang);
                if (khCanXoa == null)
                {
                    Console.WriteLine($"Không tìm thấy khách hàng có ID: {maKhachHang}");
                    return;
                }

                KhachHangs.Remove(khCanXoa);
                Console.WriteLine($"Đã xóa khách hàng có ID: {maKhachHang}");
                KhachHangs.RemoveAll(k => k.MaKhachHang == maKhachHang);
                QuanLyFile.XoaToanBoFile(tepKhachHang);
                QuanLyFile.GhiDeFile(tepKhachHang, KhachHangs);
            };
            Action undoAction = () =>
            {
                KhachHangs = CloneList(before);
                QuanLyFile.XoaToanBoFile(tepKhachHang);
                QuanLyFile.GhiDeFile(tepKhachHang, KhachHangs);
            };
            LichSuThaoTac.Instance.AddOperation(doAction, undoAction);
        }

        // In thông tin khách hàng
        public void InThongTinCapNhat(KhachHang kh)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-10} {4,-10}",
                              "ID", "Tên khách hàng", "Quận", "Tọa độ X", "Tọa độ Y");
            Console.WriteLine("{0,-5} {1,-20} {2,-10} {3,-10} {4,-10}",
                              kh.MaKhachHang, kh.TenKhachHang, kh.Quan, kh.ToaDoX, kh.ToaDoY);
        }

        // Trả về danh sách khách hàng theo quận (case-insensitive, trim)
        public List<KhachHang> LayKhachHangTheoQuan(string quan)
        {
            if (string.IsNullOrWhiteSpace(quan))
                return new List<KhachHang>();

            string q = quan.Trim();
            return KhachHangs
                .Where(k => !string.IsNullOrWhiteSpace(k.Quan) &&
                            string.Equals(k.Quan.Trim(), q, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public int DemKhachHangTheoQuan(string quan)
        {
            return LayKhachHangTheoQuan(quan).Count;
        }

        // Return customers in district sorted by ID ascending
        public List<KhachHang> LayKhachHangTheoQuan_Sorted(string quan)
        {
            return LayKhachHangTheoQuan(quan)
                   .OrderBy(k => k.MaKhachHang)
                   .ToList();
        }

        // Return a page (1-based) of customers in district sorted by ID
        public List<KhachHang> LayKhachHangTheoQuan_Paged(string quan, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            return LayKhachHangTheoQuan_Sorted(quan)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
        }

        public void HienThiToanBoKhachHang()
        {
            if (KhachHangs == null || KhachHangs.Count == 0)
            {
                Console.WriteLine("Danh sách rỗng!");
                return;
            }

            Console.WriteLine(KhachHang.Tieude_khachhang());
            Console.WriteLine(KhachHang.Separator_khachhang());
            foreach (var k in KhachHangs)
                Console.WriteLine(k.ToString());
        }
    }
}






using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nowchallenge
{
    public class QuanLyTaiXe
    {
        public List<TaiXe> TaiXes { get; set; } = new List<TaiXe>();

        // danh sách tài xế tạm thời để thực hiện undo redo
        private static List<TaiXe> CloneList(List<TaiXe> src)
        {
            var dst = new List<TaiXe>();
            if (src == null) return dst;
            foreach (var t in src)
            {
                dst.Add(CloneTaiXe(t));
            }
            return dst;
        }

        private static TaiXe CloneTaiXe(TaiXe src) // danh sách tài xế sao chép
        {
            if (src == null) return null;
            return new TaiXe(
                src.MaTaiXe,
                src.TenTaiXe,
                src.DanhGia,
                src.ToaDoX,
                src.ToaDoY,
                src.LichSuChuyenDi,
                src.SoChuyenDi,
                src.KinhNghiem,
                src.CoKhach
            );
        }

        // Hiển thị, tìm kiếm, sắp xếp
        // Hiển thị top K tài xế đầu hoặc cuối danh sách
        public void HienThiTopK_Taixe(int k, int luaChon)
        {
            int n = TaiXes.Count;

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
                Console.WriteLine($"Top {k} tài xế đầu tiên:");
                Console.WriteLine(TaiXe.Tieude_taixe());
                Console.WriteLine(TaiXe.Separator_taixe());
                for (int i = 0; i < k; i++)
                    Console.WriteLine(TaiXes[i]);
            }
            else if (luaChon == 2)
            {
                Console.WriteLine($"Top {k} tài xế cuối:");
                Console.WriteLine(TaiXe.Tieude_taixe());
                Console.WriteLine(TaiXe.Separator_taixe());
                for (int i = n - k; i < n; i++)
                    Console.WriteLine(TaiXes[i]);
            }
            else
            {
                Console.WriteLine("Lựa chọn không hợp lệ (1: K đầu, 2: K cuối)");
            }
        }



        public TaiXe TimKiem(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
                return null;

            for (int i = 0; i < TaiXes.Count; i++)
            {
                if (TaiXes[i].MaTaiXe.ToString() == tuKhoa ||
                    TaiXes[i].TenTaiXe.IndexOf(tuKhoa, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return TaiXes[i];
                }
            }
            return null;
        }

        public void SapXepTheoRating(bool giamDan)
        {
            if (TaiXes.Count == 0)
            {
                Console.WriteLine("Danh sách rỗng!");
                return;
            }

            int n = TaiXes.Count;

            for (int i = 0; i < n - 1; i++)
            {
                int index = i;

                for (int j = i + 1; j < n; j++)
                {
                    if (giamDan)
                    {
                        if (TaiXes[j].DanhGia > TaiXes[index].DanhGia)
                            index = j;
                    }
                    else
                    {
                        if (TaiXes[j].DanhGia < TaiXes[index].DanhGia)
                            index = j;
                    }
                }

                TaiXe temp = TaiXes[i];
                TaiXes[i] = TaiXes[index];
                TaiXes[index] = temp;
            }
        }

        /*
         . Chức năng Quản lý danh sách chuyến đi của tài xế
            Nhập ID của một tài xế để hiển thị toàn bộ các chuyến đi mà tài xế đó đã thực
            hiện, được sắp xếp theo thứ tự thời gian.
         */
        static void SapXepTheoThoiGianTaoChuyen(List<ChuyenDi> a, int left, int right)
        {
            //DÙNG QUICKSORT
            if (left >= right) return;

            ChuyenDi pivot = a[(left + right) / 2];
            int i = left;
            int j = right;

            while (i <= j)
            {
                while (a[i].ThoiGianTaoChuyen.CompareTo(pivot.ThoiGianTaoChuyen) < 0) i++;
                while (a[j].ThoiGianTaoChuyen.CompareTo(pivot.ThoiGianTaoChuyen) > 0) j--;

                if (i <= j)
                {
                    ChuyenDi temp = a[i];
                    a[i] = a[j];
                    a[j] = temp;
                    i++;
                    j--;
                }
            }

            SapXepTheoThoiGianTaoChuyen(a, left, j);
            SapXepTheoThoiGianTaoChuyen(a, i, right);
        }

        public void HienThiLichSuChuyenDi(TaiXe taiXe)
        {
            string tepChuyenDi = @"D:\nowchallenge\Ride of Driver\" + taiXe.LichSuChuyenDi + ".txt";

            List<ChuyenDi> lichSu = QuanLyFile.DocFile_ChuyenDi(tepChuyenDi);

            if (lichSu == null || lichSu.Count == 0)
            {
                Console.WriteLine("Không có lịch sử chuyến đi.");
                return;
            }

            SapXepTheoThoiGianTaoChuyen(lichSu, 0, lichSu.Count - 1);

            for (int i = 0; i < lichSu.Count; i++)
            {
                Console.WriteLine(lichSu[i].ToString());
            }
        }

        public bool KiemTraID(int maTaiXe)
        {
            foreach (var kh in TaiXes)
                if (kh.MaTaiXe == maTaiXe)
                    return true;

            return false;
        }
        // Thêm, xóa, sửa một tài xế
        // Hàm nhập tài xế
        public TaiXe NhapTaiXe()
        {
            int id = 0;
            string tenTaiXe = "";
            double danhGia = 0;
            double x = 0, y = 0;
            string lichSuChuyenDi = "";
            int kinhNghiem = 0;

            int step = 0; // 0=ID, 1=Ten, 2=DanhGia, 3=X, 4=Y, 5=LichSu, 6=KinhNghiem

            while (true)
            {
                if (step == 0)
                {
                    Console.Write("Nhập mã tài xế (Enter để hủy, / = Redo): ");
                    var input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                    {
                        Console.WriteLine("Hủy nhập tài xế.");
                        return null;
                    }

                    if (input == "/") { step = Math.Min(6, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                    if (!int.TryParse(input, out int tmpId))
                    {
                        Console.WriteLine("ID không hợp lệ. Vui lòng nhập số nguyên."); continue;
                    }
                    if (this.KiemTraID(tmpId))
                    {
                        Console.WriteLine($"Lỗi: ID {tmpId} đã tồn tại! Vui lòng nhập lại."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { id = tmpId; }, () => { id = 0; });
                    step++;
                    continue;
                }

                if (step == 1)
                {
                    Console.Write("Nhập tên tài xế (* = Undo, / = Redo): ");
                    var input = Console.ReadLine();

                    if (input == "*") { step = Math.Max(0, step - 1); LichSuThaoTac.Instance.Undo(); continue; }
                    if (input == "/") { step = Math.Min(6, step + 1); LichSuThaoTac.Instance.Redo(); continue; }

                    string tenTmp = input?.Trim() ?? "";
                    if (string.IsNullOrEmpty(tenTmp))
                    {
                        Console.WriteLine("Tên không được để trống."); continue;
                    }
                    if (!tenTmp.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                    {
                        Console.WriteLine("Tên chỉ được chứa chữ và khoảng trắng."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { tenTaiXe = tenTmp; }, () => { tenTaiXe = ""; });
                    step++;
                    continue;
                }

                if (step == 2)
                {
                    Console.Write("Nhập đánh giá (0-5) (* = Undo, / = Redo): ");
                    var input = Console.ReadLine();

                    if (input == "*") { step--; LichSuThaoTac.Instance.Undo(); continue; }
                    if (input == "/") { step++; LichSuThaoTac.Instance.Redo(); continue; }

                    if (!double.TryParse(input, out double tmpDG) || tmpDG < 0 || tmpDG > 5)
                    {
                        Console.WriteLine("Đánh giá không hợp lệ. Vui lòng nhập số từ 0 đến 5."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { danhGia = tmpDG; }, () => { danhGia = 0; });
                    step++;
                    continue;
                }

                if (step == 3)
                {
                    Console.Write("Nhập tọa độ X (* = Undo, / = Redo): ");
                    var input = Console.ReadLine();

                    if (input == "*") { step--; LichSuThaoTac.Instance.Undo(); continue; }
                    if (input == "/") { step++; LichSuThaoTac.Instance.Redo(); continue; }

                    if (!double.TryParse(input, out double tmpX) || tmpX < 0)
                    {
                        Console.WriteLine("Tọa độ X không hợp lệ. Vui lòng nhập số >= 0."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { x = tmpX; }, () => { x = 0; });
                    step++;
                    continue;
                }

                if (step == 4)
                {
                    Console.Write("Nhập tọa độ Y (* = Undo, / = Redo): ");
                    var input = Console.ReadLine();

                    if (input == "*") { step--; LichSuThaoTac.Instance.Undo(); continue; }
                    if (input == "/") { step++; LichSuThaoTac.Instance.Redo(); continue; }

                    if (!double.TryParse(input, out double tmpY) || tmpY < 0)
                    {
                        Console.WriteLine("Tọa độ Y không hợp lệ. Vui lòng nhập số >= 0."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { y = tmpY; }, () => { y = 0; });
                    step++;
                    continue;
                }

                if (step == 5)
                {
                    Console.Write("Nhập tên file lịch sử chuyến đi (* = Undo, / = Redo): ");
                    var input = Console.ReadLine();

                    if (input == "*") { step--; LichSuThaoTac.Instance.Undo(); continue; }
                    if (input == "/") { step++; LichSuThaoTac.Instance.Redo(); continue; }

                    string fileTmp = input?.Trim() ?? "";
                    if (string.IsNullOrEmpty(fileTmp))
                    {
                        Console.WriteLine("Tên file lịch sử không được để trống."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { lichSuChuyenDi = fileTmp; }, () => { lichSuChuyenDi = ""; });
                    step++;
                    continue;
                }

                if (step == 6)
                {
                    Console.Write("Nhập kinh nghiệm (số năm, 0-60) (* = Undo, / = Redo): ");
                    var input = Console.ReadLine();

                    if (input == "*") { step--; LichSuThaoTac.Instance.Undo(); continue; }
                    if (input == "/") { step++; LichSuThaoTac.Instance.Redo(); continue; }

                    if (!int.TryParse(input, out int tmpKN) || tmpKN < 0 || tmpKN > 60)
                    {
                        Console.WriteLine("Kinh nghiệm không hợp lệ. Vui lòng nhập số nguyên trong khoảng 0 đến 60."); continue;
                    }

                    LichSuThaoTac.Instance.AddOperation(() => { kinhNghiem = tmpKN; }, () => { kinhNghiem = 0; });
                    step++;
                    continue;
                }

                if (step == 7)
                {
                    try
                    {
                        return new TaiXe(id, tenTaiXe, danhGia, x, y, lichSuChuyenDi, 0, kinhNghiem, false);
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


        // Thêm tài xế mới
        public void ThemTaiXe(TaiXe taiXe, string tepTaiXe)
        {
            if (taiXe == null)
            {
                Console.WriteLine("Không thêm tài xế.");
                return;
            }

            var before = CloneList(this.TaiXes);

            Action doAction = () =>
            {
                if (KiemTraID(taiXe.MaTaiXe))
                {
                    Console.WriteLine($"ID: {taiXe.MaTaiXe} đã tồn tại, không thể thêm mới");
                    return;
                }
                this.TaiXes.Add(taiXe);
                QuanLyFile.XoaToanBoFile(tepTaiXe);
                QuanLyFile.GhiDeFile(tepTaiXe, this.TaiXes);
                Console.WriteLine("Đã thêm tài xế mới.");
                Console.WriteLine("Thêm thành công! (Trạng thái mặc định: Rảnh, Số chuyến: 0)");
            };

            Action undoAction = () =>
            {
                this.TaiXes = CloneList(before);
                QuanLyFile.XoaToanBoFile(tepTaiXe);
                QuanLyFile.GhiDeFile(tepTaiXe, this.TaiXes);
                Console.WriteLine("Đã hoàn tác thêm tài xế.");
            };

            LichSuThaoTac.Instance.AddOperation(doAction, undoAction);
        }


        // Xóa tài xế theo id
        // Xóa tài xế 
        public void XoaTaiXe(int id, string tepTaiXe)
        {
            var tx = TimKiem(id.ToString());
            if (tx == null)
            {
                Console.WriteLine("Không tìm thấy ID " + id);
                return;
            }
            // danh sách tạm thời để thực hiện undo redo
            var before = CloneList(this.TaiXes);
            var after = CloneList(this.TaiXes);
            after.RemoveAll(t => t.MaTaiXe == id);

            Action doAction = () =>
            {
                this.TaiXes = CloneList(after);
                Console.WriteLine("Đã xóa ID " + id);
                QuanLyFile.XoaToanBoFile(tepTaiXe);
                QuanLyFile.GhiDeFile(tepTaiXe, this.TaiXes);
            };

            Action undoAction = () =>
            {
                this.TaiXes = CloneList(before);
                QuanLyFile.XoaToanBoFile(tepTaiXe);
                QuanLyFile.GhiDeFile(tepTaiXe, this.TaiXes);
            };

            LichSuThaoTac.Instance.AddOperation(doAction, undoAction);
        }


        public void CapNhatTaiXe(string tepTaiXe)
        {
            // 
            var before = CloneList(this.TaiXes);

            Console.WriteLine("\n--- CAP NHAT THONG TIN ---");
            Console.Write("Nhap TEN tai xe can sua: ");
            string tenCanTim = Console.ReadLine();
            if (tenCanTim != null) tenCanTim = tenCanTim.Trim();

            // tìm các kết quả khớp (so sánh Trim + ignore case)
            List<TaiXe> ketQuaTim = TaiXes
                .Where(x => !string.IsNullOrWhiteSpace(x.TenTaiXe) &&
                            string.Equals(x.TenTaiXe.Trim(), tenCanTim, StringComparison.OrdinalIgnoreCase))
                .ToList();

            TaiXe mucTieu = null;

            if (ketQuaTim.Count == 0)
            {
                Console.WriteLine("Khong tim thay tai xe nao ten nhu vay.");
                return;
            }
            else if (ketQuaTim.Count == 1)
            {
                mucTieu = ketQuaTim[0];
            }
            else
            {
                // nhiều kết quả: in header 1 lần rồi liệt kê để user chọn chính xác
                Console.WriteLine($"\nPhat hien {ketQuaTim.Count} tai xe cung ten '{tenCanTim}':");
                Console.WriteLine(TaiXe.Tieude_taixe());
                Console.WriteLine(new string('-', 5) + "-+-" +
                                  new string('-', 25) + "-+-" +
                                  new string('-', 20) + "-+-" +
                                  new string('-', 8) + "-+-" +
                                  new string('-', 8) + "-+-" +
                                  new string('-', 10) + "-+-" +
                                  new string('-', 12) + "-+-" +
                                  new string('-', 12));

                foreach (var tx in ketQuaTim)
                {
                    Console.WriteLine(tx.ToString());
                }

                // Yêu cầu chọn ID trong danh sách kết quả — lặp cho đến khi chọn hợp lệ hoặc hủy
                while (true)
                {
                    Console.Write("Vui long nhap ID chinh xac cua nguoi muon sua (Enter de huy): ");
                    var s = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(s))
                    {
                        Console.WriteLine("Huy thao tac cap nhat.");
                        return;
                    }

                    if (!int.TryParse(s.Trim(), out int idNhap))
                    {
                        Console.WriteLine("ID không hợp lệ, thử lại.");
                        continue;
                    }

                    mucTieu = null;
                    for (int _i = 0; _i < ketQuaTim.Count; _i++)
                    {
                        if (ketQuaTim[_i].MaTaiXe == idNhap)
                        {
                            mucTieu = ketQuaTim[_i];
                            break;
                        }
                    }

                    if (mucTieu == null)
                    {
                        Console.WriteLine("ID không nằm trong danh sách kết quả, thử lại.");
                        continue;
                    }

                    break;
                }
            }

            InThongTinCapNhat(mucTieu);

            bool tiepTuc = true;
            while (tiepTuc)
            {
                Console.WriteLine("\n===== MENU CẬP NHẬT TÀI XẾ =====");
                Console.WriteLine("1. Sửa tên tài xế");
                Console.WriteLine("2. Sửa đánh giá");
                Console.WriteLine("3. Sửa tọa độ X");
                Console.WriteLine("4. Sửa tọa độ Y");
                Console.WriteLine("5. Sửa số chuyến đi");
                Console.WriteLine("6. Sửa kinh nghiệm");
                Console.WriteLine("7. Sửa trạng thái (Rảnh/Đang chạy)");
                Console.WriteLine("8. Hoàn tất cập nhật");
                Console.Write("Chọn mục: ");
                string chon = Console.ReadLine();

                switch (chon)
                {
                    case "1":
                        Console.Write("Tên mới: ");
                        string tenMoi = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(tenMoi))
                            mucTieu.TenTaiXe = tenMoi;
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "2":
                        Console.Write("Đánh giá mới (0-5): ");
                        string inputDanhGia = Console.ReadLine();
                        if (double.TryParse(inputDanhGia, out double danhGiaMoi))
                            mucTieu.DanhGia = danhGiaMoi;
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "3":
                        Console.Write("Tọa độ X mới: ");
                        string inputX = Console.ReadLine();
                        if (double.TryParse(inputX, out double xMoi))
                            mucTieu.ToaDoX = xMoi;
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "4":
                        Console.Write("Tọa độ Y mới: ");
                        string inputY = Console.ReadLine();
                        if (double.TryParse(inputY, out double yMoi))
                            mucTieu.ToaDoY = yMoi;
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "5":
                        Console.Write("Số chuyến đi mới: ");
                        string inputSoChuyen = Console.ReadLine();
                        if (int.TryParse(inputSoChuyen, out int soChuyenMoi))
                            mucTieu.SoChuyenDi = soChuyenMoi;
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "6":
                        Console.Write("Kinh nghiệm mới (năm): ");
                        string inputKinhNghiem = Console.ReadLine();
                        if (int.TryParse(inputKinhNghiem, out int kinhNghiemMoi))
                            mucTieu.KinhNghiem = kinhNghiemMoi;
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "7":
                        Console.Write("Trạng thái (0: Rảnh, 1: Đang chạy): ");
                        string inputTrangThai = Console.ReadLine();
                        if (inputTrangThai == "0")
                            mucTieu.CoKhach = false;
                        else if (inputTrangThai == "1")
                            mucTieu.CoKhach = true;
                        else
                            Console.WriteLine("Giá trị không hợp lệ.");
                        InThongTinCapNhat(mucTieu);
                        break;

                    case "8":
                        //ghi lại
                        var after = CloneList(this.TaiXes);

                        // lưu trữ 
                        QuanLyFile.XoaToanBoFile(tepTaiXe);
                        QuanLyFile.GhiDeFile(tepTaiXe, TaiXes);
                        Console.WriteLine("Đã lưu thông tin cập nhật.");
                        tiepTuc = false;

                        // đăng ký thao tác hoàn tác/làm lại (doAction sẽ áp dụng lại 'after', undoAction khôi phục 'before')
                        Action doAction = () =>
                        {
                            this.TaiXes = CloneList(after);
                            QuanLyFile.XoaToanBoFile(tepTaiXe);
                            QuanLyFile.GhiDeFile(tepTaiXe, this.TaiXes);
                        };

                        Action undoAction = () =>
                        {
                            this.TaiXes = CloneList(before);
                            QuanLyFile.XoaToanBoFile(tepTaiXe);
                            QuanLyFile.GhiDeFile(tepTaiXe, this.TaiXes);
                        };

                        LichSuThaoTac.Instance.AddOperation(doAction, undoAction);
                        break;

                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        break;
                }
            }
        }

        public void InThongTinCapNhat(TaiXe tx)
        {
            Console.WriteLine("{0,-5} {1,-20} {2,-7} {3,-7} {4,-10} {5,-10} {6,-10}",
                              "ID", "Tên tài xế", "Đánh giá", "X", "Y", "Số chuyến", "Kinh nghiệm");

            // Dòng phân cách
            Console.WriteLine(new string('-', 5) + "-+-" +
                              new string('-', 20) + "-+-" +
                              new string('-', 7) + "-+-" +
                              new string('-', 7) + "-+-" +
                              new string('-', 10) + "-+-" +
                              new string('-', 10) + "-+-" +
                              new string('-', 10));

            Console.WriteLine("{0,-5} {1,-20} {2,-7:N1} {3,-7:N2} {4,-10:N2} {5,-10} {6,-10} ({7})",
                              tx.MaTaiXe,
                              tx.TenTaiXe,
                              tx.DanhGia,
                              tx.ToaDoX,
                              tx.ToaDoY,
                              tx.SoChuyenDi,
                              tx.KinhNghiem,
                              tx.CoKhach ? "Đang chạy" : "Rảnh");
        }

        public void HienThiToanBoTaiXe()
        {
            if (TaiXes == null || TaiXes.Count == 0)
            {
                Console.WriteLine("Danh sách rỗng!");
                return;
            }

            Console.WriteLine(TaiXe.Tieude_taixe());
            Console.WriteLine(TaiXe.Separator_taixe());
            foreach (var t in TaiXes)
                Console.WriteLine(t.ToString());
        }
    }
}
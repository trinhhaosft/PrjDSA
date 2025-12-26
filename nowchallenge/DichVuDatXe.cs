using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nowchallenge
{
    public struct TaiXeUngVien
    {
        public TaiXe taiXe;
        public double khoangCach; // lưu khoảng các từ tài xế đến khách hàng

        public TaiXeUngVien(TaiXe taiXe, double khoangCach)
        {
            this.taiXe = taiXe;
            this.khoangCach = khoangCach;
        }
    }
    public partial class DichVuDatXe
    {
        /*
             4. Chức năng Tìm tài xế phù hợp
             • Nhập ID khách hàng và bán kính R.
             • Trả về danh sách các tài xế ở gần khách hàng nhất trong phạm vi R, sắp xếp
             theo khoảng cách tăng dần.
             • Tiêu chí phụ (tuỳ chọn): Rating, Số chuyến đi, Kinh nghiệm lái xe.
         */


        public static double TinhKhoangCach(KhachHang khachHang, TaiXe taiXe)
        {
            double dx = taiXe.ToaDoX - khachHang.ToaDoX;
            double dy = taiXe.ToaDoY - khachHang.ToaDoY;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        // Hàm này sẽ tính khoảng cách đồng thời chuyển list tài xế thành Dictionnary cho dễ sắp xếp
       
        public static List<TaiXeUngVien> TaoDanhSachUngVien(KhachHang khachHang ,List<TaiXe> taiXes, int R)
        {
            List<TaiXeUngVien> danhSachUngVien = new List<TaiXeUngVien>();

            for (int i = 0; i < taiXes.Count; i++)
            {
                TaiXe tx = taiXes[i];

                // Chỉ xét tài xế đang rảnh
                if (tx.CoKhach) 
                    continue;

                double khoangCach = TinhKhoangCach(khachHang, tx);

                if (khoangCach > R) continue; // bỏ qua các tài xế ở quá xa với phạm vi

                else if (taiXes[i].CoKhach) continue; // bỏ qua tài xế đang có khách
                else
                {
                    TaiXeUngVien ungVien = new TaiXeUngVien(tx, khoangCach);
                    danhSachUngVien.Add(ungVien);
                }
            }

            return danhSachUngVien;
        }
           
        // Sắp xếp theo từng tiêu chí. Kinh nghiệm -> Rating -> Khoảng cách
        static bool SoSanhKinhNghiem(TaiXeUngVien a, TaiXeUngVien b)
        {
            return a.taiXe.KinhNghiem > b.taiXe.KinhNghiem;
        }

       static bool SoSanhDanhGia (TaiXeUngVien a, TaiXeUngVien b)
        {
            return a.taiXe.DanhGia > b.taiXe.DanhGia;
        }
        
        static bool SoSanhKhoangCach(TaiXeUngVien a, TaiXeUngVien b)
        {
            return a.khoangCach < b.khoangCach;
        }

        delegate bool Compare(TaiXeUngVien a, TaiXeUngVien b);

        static void Merge(List<TaiXeUngVien> A,int left,int mid,int right,Compare cmp)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            TaiXeUngVien[] L = new TaiXeUngVien[n1];
            TaiXeUngVien[] R = new TaiXeUngVien[n2];

            for (int i = 0; i < n1; i++)
                L[i] = A[left + i];

            for (int j = 0; j < n2; j++)
                R[j] = A[mid + 1 + j];

            int i1 = 0, i2 = 0, k = left;

            while (i1 < n1 && i2 < n2)
            {
                if (cmp(L[i1], R[i2]))
                    A[k++] = L[i1++];
                else
                    A[k++] = R[i2++];
            }

            while (i1 < n1)
                A[k++] = L[i1++];

            while (i2 < n2)
                A[k++] = R[i2++];
        }

        static void MergeSort(List<TaiXeUngVien> A, int left, int right, Compare cmp)
        {
            if (left >= right) return;

            int mid = (left + right) / 2;

            MergeSort(A, left, mid, cmp);
            MergeSort(A, mid + 1, right, cmp);

            Merge(A, left, mid, right, cmp);
        }

        public static void SapXepMangUngVien(List<TaiXeUngVien> taiXeUngViens)
        {
            MergeSort(taiXeUngViens, 0, taiXeUngViens.Count - 1, SoSanhKinhNghiem);
            MergeSort(taiXeUngViens, 0, taiXeUngViens.Count - 1, SoSanhDanhGia);
            MergeSort(taiXeUngViens, 0, taiXeUngViens.Count - 1, SoSanhKhoangCach);

        }

        public List<TaiXeUngVien> TimTaiXePhuHop(KhachHang khachHang, List<TaiXe> taiXes, int R)
        {
            List<TaiXeUngVien> tmp = new List<TaiXeUngVien>();

            // Tiến hành tìm các tài xế trong cùng bán kính
            tmp = TaoDanhSachUngVien(khachHang, taiXes, R);

            // Sắp xếp mảng đó rồi trả về
            SapXepMangUngVien(tmp);

            return tmp;
        }

        /*
         5. Chức năng Đặt xe
            • Nhập: ID khách hàng, ID tài xế, quãng đường từ điểm đón đến điểm đến.
            • Tự động tính toán:
            – Distance: tổng quãng đường bao gồm cả khoảng cách tài xế đi đến vị trí khách
            hàng.
            – Fare = Distance × 12.000
            • Hỗ trợ hủy chuyến, xác nhận tất cả chuyến đi, và lưu dữ liệu vào lịch sử nếu hợp lệ
        */

        static void XacNhanChuyenDi(ChuyenDi chuyenDiHopLe, string tepChuyenDi)
        {

            QuanLyFile.GhiFile(tepChuyenDi, chuyenDiHopLe);
            Console.WriteLine("Xác nhận chuyến đi thành công");

        }

        

        public static void DatXe(KhachHang khachHang, TaiXe taiXe)
        {
            // Nếu tài xế đang có khách, hỏi người dùng có muốn chọn tài xế khác không
            if (taiXe.CoKhach)
            {
                Console.WriteLine("Tài xế đang chở khách.");
                Console.Write("Bạn có muốn chọn tài xế khác không? (y/n): ");
                var resp = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(resp) || resp.Trim().ToLower() != "y")
                    return;

                // Load danh sách tài xế từ file
                string fileTaiXe = @"D:\nowchallenge\TaiXe.txt";
                var dsTai = QuanLyFile.DocFile_TaiXe(fileTaiXe) ?? new List<TaiXe>();
                // Tạo danh sách ứng viên
                var candidates = TaoDanhSachUngVien(khachHang, dsTai, int.MaxValue);
                SapXepMangUngVien(candidates);

                if (candidates == null || candidates.Count == 0)
                {
                    Console.WriteLine("Không có tài xế rảnh để lựa chọn.");
                    return;
                }

                Console.WriteLine("\n--- DANH SÁCH TÀI XẾ KHẢ DỤNG ---");
                Console.WriteLine(TaiXe.Tieude_taixe() + " | " + "Khoảng cách");
                Console.WriteLine(TaiXe.Separator_taixe() + "-+-" + new string('-', 10));
                foreach (var u in candidates)
                    Console.WriteLine(u.taiXe.ToString() + $" | {u.khoangCach:N2}");

                // Yêu cầu người dùng chọn tài xế từ danh sách
                Console.Write("Nhập ID tài xế muốn chọn (Enter để hủy): ");
                var sId = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(sId))
                    return;

                if (!int.TryParse(sId.Trim(), out int idChosen))
                {
                    Console.WriteLine("ID không hợp lệ. Hủy thao tác.");
                    return;
                }

                var chosenCandidate = candidates.FirstOrDefault(c => c.taiXe.MaTaiXe == idChosen);
                if (chosenCandidate.taiXe == null || chosenCandidate.taiXe.MaTaiXe != idChosen)
                {
                    Console.WriteLine("Không tìm thấy tài xế trong danh sách. Hủy thao tác.");
                    return;
                }

                // Tiến hành đặt xe với tài xế được chọn
                DatXe(khachHang, chosenCandidate.taiXe);
                return;
            }

            // Tiếp tục với tài xế rảnh
            string tepChuyenDi = @"D:\nowchallenge\Ride of Driver\" + taiXe.LichSuChuyenDi + ".txt";

            Console.WriteLine("Nhập mã chuyến đi");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
                Console.WriteLine("Mã chuyến không hợp lệ. Vui lòng nhập số.");

            double quangDuong = -1;
            do
            {
                Console.WriteLine("Nhập quãng đường");
                var s = Console.ReadLine();
                if (!double.TryParse(s, out quangDuong) || quangDuong <= 0)
                {
                    Console.WriteLine("Quãng đường không hợp lệ. Vui lòng nhập số lớn hơn 0.");
                    quangDuong = -1;
                }
            }
            while (quangDuong <= 0);

            // Hiển thị thông tin tài xế trước khi yêu cầu xác nhận
            double diemden = TinhKhoangCach(khachHang, taiXe);
            Console.WriteLine("\n--- THÔNG TIN TÀI XẾ ĐƯỢC CHỌN ---");
            Console.WriteLine(TaiXe.Tieude_taixe());
            Console.WriteLine(TaiXe.Separator_taixe());
            Console.WriteLine(taiXe.ToString());
            double giaDuKien = (diemden + quangDuong )* 12000 ; 
            Console.WriteLine($"Giá ước tính: {giaDuKien:N0} VND");
            Console.WriteLine("---------------------------------\n");

            Console.WriteLine("\n===== LỰA CHỌN =====");
            Console.WriteLine("1: Xác nhận chuyến đi");
            Console.WriteLine("0: Hủy chuyến đi");
            Console.WriteLine("2: Kết thúc đặt xe");

            while (true)
            {
                var sChoice = Console.ReadLine();
                if (!int.TryParse(sChoice, out int choice))
                {
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng nhập 1, 2 hoặc 0.");
                    continue;
                }

                if (choice == 1)
                {
                    // bắt đầu chuyến: đánh dấu có khách và tăng số chuyến (tạm)
                    taiXe.CoKhach = true;
                    taiXe.SoChuyenDi = taiXe.SoChuyenDi + 1;

                    // tạo và lưu chuyến
                    ChuyenDi chuyenDiHopLe = new ChuyenDi(id, khachHang.MaKhachHang, taiXe.MaTaiXe, quangDuong, DateTime.Now);
                    XacNhanChuyenDi(chuyenDiHopLe, tepChuyenDi);

                    // Sau khi chuyến hoàn tất, hỏi khách hàng đánh giá (0-5)
                    double? rating = null;
                    while (true)
                    {
                        Console.WriteLine("Nhập đánh giá của khách hàng cho tài xế (0.0 - 5.0). Nhấn Enter để bỏ qua:");
                        string sRating = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(sRating))
                        {
                            // người dùng bỏ qua
                            break;
                        }

                        if (double.TryParse(sRating, out double r))
                        {
                            if (r >= 0.0 && r <= 5.0)
                            {
                                rating = r;
                                break;
                            }
                        }

                        Console.WriteLine("Đánh giá không hợp lệ. Vui lòng nhập số trong khoảng 0.0 - 5.0 hoặc nhấn Enter để bỏ qua.");
                    }

                    // Cập nhật file danh sách tài xế (persist SoChuyenDi, DanhGia (nếu có) và trạng thái)
                    try
                    {
                        string fileTaiXe2 = @"D:\nowchallenge\TaiXe.txt";
                        var dsTai2 = QuanLyFile.DocFile_TaiXe(fileTaiXe2) ?? new List<TaiXe>();

                        // tạo đối tượng quản lý từ file để tìm kiếm
                        var qlFromFile = new QuanLyTaiXe { TaiXes = dsTai2 };
                        var target = qlFromFile.TimKiem(taiXe.MaTaiXe.ToString());
                       
                        // ensure in-memory driver marked rảnh (chuyến đã kết thúc)
                        taiXe.CoKhach = false;

                        if (target != null)
                        {
                            // cập nhật số chuyến và trạng thái trong file copy
                            target.SoChuyenDi = taiXe.SoChuyenDi;
                            target.CoKhach = taiXe.CoKhach;

                            // nếu có đánh giá, tính trung bình mới
                            if (rating.HasValue)
                            {
                                int oldCount = Math.Max(0, target.SoChuyenDi - 1); // lượt trước khi thêm lần này
                                double oldAvg = target.DanhGia;
                                double newAvg = (oldCount == 0) ? rating.Value : (oldAvg * oldCount + rating.Value) / (oldCount + 1);

                                target.DanhGia = newAvg;
                                taiXe.DanhGia = newAvg;
                            }
                        }
                        else
                        {
                            // nếu không tìm thấy target trong file, cập nhật in-memory
                            if (rating.HasValue)
                            {
                                int oldCount = Math.Max(0, taiXe.SoChuyenDi - 1);
                                double oldAvg = taiXe.DanhGia;
                                double newAvg = (oldCount == 0) ? rating.Value : (oldAvg * oldCount + rating.Value) / (oldCount + 1);
                                taiXe.DanhGia = newAvg;
                            }
                        }

                        // ghi đè file để lưu lại thay đổi (ghi cả ds TaiXe hiện tại)
                        QuanLyFile.GhiDeFile(fileTaiXe2, dsTai2);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Cảnh báo: không thể cập nhật file tài xế: " + ex.Message);
                        // đảm bảo trạng thái in-memory được cập nhật
                        taiXe.CoKhach = false;
                    }

                    Console.WriteLine("Chuyến đi được xác nhận và (nếu có) đánh giá đã được cập nhật.");
                    break;
                }
                else if (choice == 2)
                {
                    taiXe.CoKhach = false;
                    break;
                }
                else if (choice == 0)
                {
                    taiXe.CoKhach = false;
                    break;
                }
                else
                {
                    Console.WriteLine("Không hợp lệ. Hãy chọn lại");
                }
            }
        }

        /*
         6. Tự động Ghép cặp chuyến đi
            Hệ thống tự động tìm và chỉ định tài xế phù hợp nhất cho yêu cầu của khách hàng.
         */
        public TaiXeUngVien DatXeTuDong(KhachHang khachHang, List<TaiXe> taiXes, int R)
        {
            List<TaiXeUngVien> tmp = TimTaiXePhuHop(khachHang, taiXes, R);
            if (tmp == null || tmp.Count == 0)
            {
                Console.WriteLine("Khong co tai xe phu hop!");
                return default(TaiXeUngVien);
            }

            TaiXeUngVien taiXePhuHop = tmp[0];

            // Thực hiện đặt xe cho tài xế được chọn (DatXe xử lý cập nhật, ghi file, hỏi đánh giá...)
            DatXe(khachHang, taiXePhuHop.taiXe);

            // Trả về ứng viên được chọn để caller có thể hiển thị thông tin + khoảng cách
            return taiXePhuHop;
        }

        public static void TaiPhuHop_TuongTac(QuanLyKhachHang qlkh, QuanLyTaiXe qltx)
        {
            while (true)
            {
                Console.Write("Nhập ID khách hàng để tìm (Enter để hủy): ");
                var sIdKh = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(sIdKh))
                {
                    Console.WriteLine("Hủy thao tác tìm tài xế.");
                    return;
                }

                if (!int.TryParse(sIdKh, out int idKh))
                {
                    Console.WriteLine("ID không hợp lệ. Vui lòng nhập lại.");
                    continue;
                }

                var kh = qlkh.TimKhachHangTheoID(idKh);
                if (kh == null)
                {
                    Console.WriteLine("Không tìm thấy khách hàng. Vui lòng nhập lại.");
                    continue;
                }

                int R = 0;
                while (true)
                {
                    Console.Write("Nhập bán kính R (số nguyên dương, Enter để hủy): ");
                    var sR = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(sR))
                    {
                        Console.WriteLine("Hủy thao tác tìm tài xế.");
                        return;
                    }
                    if (!int.TryParse(sR, out R) || R <= 0)
                    {
                        Console.WriteLine("R không hợp lệ. Vui lòng nhập lại.");
                        continue;
                    }
                    break;
                }

                var ds = TaoDanhSachUngVien(kh, qltx.TaiXes, R);
                SapXepMangUngVien(ds);

                Console.WriteLine(TaiXe.Tieude_taixe() + " | " + "Khoảng cách");
                Console.WriteLine(TaiXe.Separator_taixe() + "-+-" + new string('-', 10));
                foreach (var u in ds)
                    Console.WriteLine(u.taiXe.ToString() + $" | {u.khoangCach:N2}");

                return;
            }
        }

        // Hàm hỗ trợ tương tác: đặt xe tự động (yêu cầu nhập ID khách hàng + R rồi gọi DatXeTuDong)
        public static void DatXeTuDong_TuongTac(QuanLyKhachHang qlkh, QuanLyTaiXe qltx)
        {
            while (true)
            {
                Console.Write("Nhập ID khách hàng muốn đặt xe (Enter để hủy): ");
                var sIdKh = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(sIdKh))
                {
                    Console.WriteLine("Hủy thao tác đặt xe tự động.");
                    return;
                }

                if (!int.TryParse(sIdKh, out int idKh2))
                {
                    Console.WriteLine("ID không hợp lệ. Vui lòng nhập lại.");
                    continue;
                }

                var kh2 = qlkh.TimKhachHangTheoID(idKh2);
                if (kh2 == null)
                {
                    Console.WriteLine("Không tìm thấy khách hàng. Vui lòng nhập lại.");
                    continue;
                }

                int R2 = 0;
                while (true)
                {
                    Console.Write("Nhập bán kính tìm kiếm R (số nguyên dương, Enter để hủy): ");
                    var sR = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(sR))
                    {
                        Console.WriteLine("Hủy thao tác đặt xe tự động.");
                        return;
                    }
                    if (!int.TryParse(sR, out R2) || R2 <= 0)
                    {
                        Console.WriteLine("R không hợp lệ. Vui lòng nhập lại.");
                        continue;
                    }
                    break;
                }

                var dv = new DichVuDatXe();
                dv.DatXeTuDong(kh2, qltx.TaiXes, R2);
                return;
            }
        }

        // Hàm hỗ trợ tương tác: đặt xe thủ công (yêu cầu nhập ID khách hàng, sau đó nhập lại ID tài xế nếu sai)
        public static void DatXeThuCong_TuongTac(QuanLyKhachHang qlkh, QuanLyTaiXe qltx)
        {
            while (true)
            {
                Console.Write("Nhập ID khách hàng (Enter để hủy): ");
                var sKh = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(sKh))
                {
                    Console.WriteLine("Hủy thao tác đặt xe thủ công.");
                    return;
                }
                if (!int.TryParse(sKh, out int idKhManual))
                {
                    Console.WriteLine("ID không hợp lệ. Vui lòng nhập lại.");
                    continue;
                }

                var khManual = qlkh.TimKhachHangTheoID(idKhManual);
                if (khManual == null)
                {
                    Console.WriteLine("Không tìm thấy khách hàng. Vui lòng nhập lại.");
                    continue;
                }

                while (true)
                {
                    Console.Write("Nhập ID tài xế (Enter để hủy): ");
                    var sTx = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(sTx))
                    {
                        Console.WriteLine("Hủy thao tác đặt xe thủ công.");
                        return;
                    }
                    if (!int.TryParse(sTx, out int idTx))
                    {
                        Console.WriteLine("ID không hợp lệ. Vui lòng nhập lại.");
                        continue;
                    }

                    var tx = qltx.TimKiem(idTx.ToString());
                    if (tx == null)
                    {
                        Console.WriteLine("Không tìm thấy tài xế. Vui lòng nhập lại hoặc Enter để hủy.");
                        continue;
                    }

                    // Đã tìm thấy tài xế — tiến hành đặt xe
                    DatXe(khManual, tx);
                    return;
                }
            }
        }
    }
}



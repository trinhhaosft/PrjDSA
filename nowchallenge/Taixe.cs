using System;
using System.Runtime.Remoting.Messaging;

namespace nowchallenge
{
    public class TaiXe : IGhiFile
    {
        private int maTaiXe;
        private string tenTaiXe;
        private double danhGia;
        private double toaDoX;
        private double toaDoY;
        private string lichSuChuyenDi; // tên file lưu lịch sử
        private int soChuyenDi;
        private int kinhNghiem; // số năm chạy
        private bool coKhach; // true: đang chạy, false: rảnh

        public TaiXe(int maTaiXe, string tenTaiXe, double danhGia,
                     double toaDoX, double toaDoY,
                     string lichSuChuyenDi, int soChuyenDi,
                     int kinhNghiem, bool coKhach)
        {
            this.MaTaiXe = maTaiXe;
            this.TenTaiXe = tenTaiXe;
            this.DanhGia = danhGia;
            this.ToaDoX = toaDoX;
            this.ToaDoY = toaDoY;
            this.LichSuChuyenDi = lichSuChuyenDi;
            this.SoChuyenDi = soChuyenDi;
            this.KinhNghiem = kinhNghiem;
            this.CoKhach = coKhach;
        }

        public int MaTaiXe
        {
            get { return maTaiXe; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Mã tài xế phải lớn hơn 0");
                maTaiXe = value;
            }
        }

        public string TenTaiXe
        {
            get { return tenTaiXe; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên tài xế không được để trống");

                string trimmed = value.Trim();

                // Kiểm tra từng ký tự
                for (int i = 0; i < trimmed.Length; i++)
                {
                    char c = trimmed[i];
                    if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                    {
                        throw new ArgumentException("Tên tài xế chỉ được chứa chữ và khoảng trắng");
                    }
                }

                tenTaiXe = trimmed;
            }
        }

        public double DanhGia
        {
            get { return danhGia; }
            set
            {
                if (value < 0 || value > 5)
                    throw new ArgumentException("Điểm đánh giá phải nằm trong khoảng 0 đến 5");
                danhGia = value;
            }
        }

        public double ToaDoX
        {
            get { return toaDoX; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Tọa độ X không được âm");
                toaDoX = value;
            }
        }

        public double ToaDoY
        {
            get { return toaDoY; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Tọa độ Y không được âm");
                toaDoY = value;
            }
        }


        public string LichSuChuyenDi
        {
            get { return lichSuChuyenDi; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên file lịch sử không được rỗng");
                lichSuChuyenDi = value;
            }
        }

        public int SoChuyenDi
        {
            get { return soChuyenDi; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Số chuyến đi không được âm");
                soChuyenDi = value;
            }
        }

        public int KinhNghiem
        {
            get { return kinhNghiem; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Kinh nghiệm không được âm");
                if (value > 60)
                    throw new ArgumentException("Kinh nghiệm không hợp lý");
                kinhNghiem = value;
            }
        }

        public bool CoKhach
        {
            get { return coKhach; }
            set { coKhach = value; }
        }

        // Hàm ghi file
        public string ToFileString()
        {
            return $"{MaTaiXe} | {TenTaiXe} | {DanhGia} | {ToaDoX} | {ToaDoY} | {LichSuChuyenDi} | {SoChuyenDi} | {KinhNghiem} | {CoKhach}";
        }

        // Helper: trả về chuỗi sao (★ đầy, ☆ trống) tương ứng danhGia (0..5).
        // Đặt ở model để mọi nơi có thể tái sử dụng (ví dụ UI, bảng, ToString).
        public string StarRating
        {
            get
            {
                double r = Math.Max(0.0, Math.Min(5.0, danhGia)); //tìm max và min để đảm bảo r trong [0..5]
                int full = (int)Math.Floor(r);
                double frac = r - full; // phần thập phân

                // hiển thị phân số thập phân dưới dạng glyph
                char fracGlyph = '\0';
                if (frac >= 0.75) fracGlyph = '¾';
                else if (frac >= 0.50) fracGlyph = '½';
                else if (frac >= 0.25) fracGlyph = '¼';
                else fracGlyph = '\0';

                var sb = new System.Text.StringBuilder();

                // thêm sao đầy
                for (int i = 0; i < full; i++) sb.Append('★');

                // thêm sao phân số (nếu có)
                if (fracGlyph != '\0')
                {
                    sb.Append(fracGlyph);
                }

                // thêm sao trống
                int usedSlots = full + (fracGlyph != '\0' ? 1 : 0);
                for (int i = usedSlots; i < 5; i++) sb.Append('☆');

                // thêm giá trị số bên cạnh
                sb.AppendFormat(" ({0:N1})", r);

                return sb.ToString();
            }
        }


        public override string ToString()
        {
            // Trả về một dòng đã định dạng theo cột để khi Program in từng đối tượng sẽ có
            // các cột căn đều. Program có thể in header riêng trước khi in các ToString().
            string state = coKhach ? "Đang chạy" : "Rảnh";
            string name = tenTaiXe ?? "";
            if (name.Length > 25) name = name.Substring(0, 25);

            return string.Format("{0,-5} | {1,-25} | {2,-11} | {3,-8} | {4,-8} | {5,-10} | {6,-12} | {7,-12}",
                                  maTaiXe,
                                  name,
                                  StarRating,
                                  toaDoX,
                                  toaDoY,
                                  soChuyenDi,
                                  kinhNghiem,
                                  state);
        }

        // Tiêu đề cột khi in danh sách tài xế
        public static string Tieude_taixe()
        {
            return string.Format("{0,-5} | {1,-25} | {2,-11} | {3,-8} | {4,-8} | {5,-10} | {6,-12} | {7,-12}",
                                  "ID",
                                  "Tên tài xế",
                                  "Đánh giá",
                                  "X",
                                  "Y",
                                  "Số chuyến",
                                  "Kinh nghiệm",
                                  "Trạng thái");

        }
            public static string Separator_taixe()
        {
            return new string('-', 5) + "-+-" +
                   new string('-', 25) + "-+-" +
                   new string('-', 11) + "-+-" +
                   new string('-', 8) + "-+-" +
                   new string('-', 8) + "-+-" +
                   new string('-', 10) + "-+-" +
                   new string('-', 12) + "-+-" +
                   new string('-', 12);
        }

    }
}

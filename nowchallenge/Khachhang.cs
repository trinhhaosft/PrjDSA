using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nowchallenge
{
    public class KhachHang: IGhiFile
    {
        private int maKhachHang;
        private string tenKhachHang;
        private string quan;
        private double toaDoX;
        private double toaDoY;


        public KhachHang(int maKhachHang, string tenKhachHang, string quan, double toaDoX, double toaDoY)
        {
            this.MaKhachHang = maKhachHang;
            this.TenKhachHang = tenKhachHang;
            this.Quan = quan;
            this.ToaDoX = toaDoX;
            this.ToaDoY = toaDoY;
        }

        public int MaKhachHang
        {
            get { return maKhachHang; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Mã khách hàng phải lớn hơn 0");
                maKhachHang = value;
            }
        }

        public string TenKhachHang
        {
            get { return tenKhachHang; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên khách hàng không được để trống");

                string trimmed = value.Trim();

                // Kiểm tra từng ký tự
                bool allLetters = false;
                foreach (char c in trimmed)
                {
                    if (!char.IsWhiteSpace(c))
                    {
                        if (!char.IsLetter(c))
                        {
                            throw new ArgumentException("Tên khách hàng chỉ được chứa chữ và khoảng trắng");
                        }
                        allLetters = true;
                    }
                }

                if (!allLetters)
                    throw new ArgumentException("Tên khách hàng không được rỗng hoặc chỉ chứa khoảng trắng");

                tenKhachHang = trimmed;
            }
        }

        public string Quan
        {
            get { return quan; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên quận không được để trống");
                quan = value;
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

        public string ToFileString()
        {
            return $"{MaKhachHang} | {TenKhachHang} | {Quan} | {ToaDoX} | {ToaDoY}";
        }

       
        public override string ToString()
        {
            // Trả về một dòng đã định dạng theo cột để khi Program in từng KhachHang
            // sẽ căn đều; in header 1 lần trước vòng lặp.
            string name = TenKhachHang ?? "";
            if (name.Length > 25) name = name.Substring(0, 25);

            string district = Quan ?? "";
            if (district.Length > 15) district = district.Substring(0, 15);

            return string.Format("{0,-5} | {1,-25} | {2,-15} | {3,-8} | {4,-8}",
                                 MaKhachHang,
                                 name,
                                 district,
                                 ToaDoX,
                                 ToaDoY);
        }

        //Tiêu đề cột khi in danh sách khách hàng
        public static string Tieude_khachhang()
        {
            return string.Format("{0,-5} | {1,-25} | {2,-15} | {3,-8} | {4,-8}",
                                 "ID", "Tên khách hàng", "Quận", "X", "Y");
        }

        public static string Separator_khachhang()
        {
            return new string('-', 5) + "-+-" +
                   new string('-', 25) + "-+-" +
                   new string('-', 11) + "-+-" +
                   new string('-', 8) + "-+-" +
                   new string('-', 12);
        }
    }

}




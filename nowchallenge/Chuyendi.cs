using System;

namespace nowchallenge
{
    public class ChuyenDi : IGhiFile
    {
        private int maChuyenDi;
        private int maKhachHang;
        private int maTaiXe;
        private double quangDuong;
        private int giaTien;
        private DateTime thoiGianTaoChuyen;

        public ChuyenDi(int maChuyenDi, int maKhachHang, int maTaiXe, double quangDuong, DateTime thoiGianTaoChuyen)
        {
            if (maChuyenDi <= 0)
                throw new ArgumentException("Ma chuyen di phai lon hon 0");
            if (maKhachHang <= 0)
                throw new ArgumentException("Ma khach hang phai lon hon 0");
            if (maTaiXe <= 0)
                throw new ArgumentException("Ma tai xe phai lon hon 0");
            if (quangDuong <= 0)
                throw new ArgumentException("Quang duong phai lon hon 0");

            this.maChuyenDi = maChuyenDi;
            this.maKhachHang = maKhachHang;
            this.maTaiXe = maTaiXe;
            this.quangDuong = quangDuong;

            // Tính giá tiền theo quãng đường
            this.giaTien = (int)(quangDuong * 12000);

            // Nếu muốn dùng thời gian hiện tại thay vì truyền vào, có thể bỏ param thoiGianTaoChuyen
            this.thoiGianTaoChuyen = thoiGianTaoChuyen;
        }

        public int MaChuyenDi
        {
            get { return maChuyenDi; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Mã chuyến đi phải lớn hơn 0");
                maChuyenDi = value;
            }
        }

        public int MaKhachHang
        {
            get { return maKhachHang; }
            set
            {
                if (value <= 0)
                    throw new ArgumentNullException("ID không được bé hơn hoặc bằng 0");
                maKhachHang = value;
            }
        }

        public int MaTaiXe
        {
            get { return maTaiXe; }
            set
            {
                if (value <= 0)
                    throw new ArgumentNullException("ID không được bé hơn hoặc bằng không");
                maTaiXe = value;
            }
        }

        public double QuangDuong
        {
            get { return quangDuong; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Quãng đường phải lớn hơn 0");
                quangDuong = value;
                GiaTien = (int)(quangDuong * 12000); // tự động cập nhật giá tiền
            }
        }

        public int GiaTien
        {
            get { return giaTien; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Giá tiền không được âm");
                giaTien = value;
            }
        }

        public DateTime ThoiGianTaoChuyen
        {
            get { return thoiGianTaoChuyen; }
            set
            {
                if (value > DateTime.Now)
                    throw new ArgumentException("Thời gian tạo không hợp lệ");
                thoiGianTaoChuyen = value;
            }
        }

        public string ToFileString()
        {
            return $"{MaChuyenDi} | {MaKhachHang} | {MaTaiXe} | {QuangDuong} | {GiaTien} | {ThoiGianTaoChuyen}";
        }
        public override string ToString()
        {
            return "ChuyenDi | " +
                   "Ma=" + MaChuyenDi + " | " +
                   "KhachHang=" + MaKhachHang + " | " +
                   "TaiXe=" + MaTaiXe + " | " +
                   "QuangDuong=" + QuangDuong + " km | " +
                   "GiaTien=" + GiaTien + " VND | " +
                   "ThoiGian=" + ThoiGianTaoChuyen;
        }
    }
}
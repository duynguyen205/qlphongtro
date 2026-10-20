using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace QuanLyPhongTro
{
    // ========================================================
    // PHẦN 1: KẾT NỐI VÀ THỰC THI CƠ SỞ DỮ LIỆU
    // ========================================================
    public static class KetNoiCSDL
    {
        public static string ChuoiKetNoi = @"Server=.\SQLEXPRESS;Database=QuanLyPhongTro;Trusted_Connection=True;TrustServerCertificate=True;";

        public static DataTable LayDuLieu(string truyVan, SqlParameter[] thamSo = null)
        {
            using (SqlConnection ketNoi = new SqlConnection(ChuoiKetNoi))
            {
                using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                {
                    if (thamSo != null) lenh.Parameters.AddRange(thamSo);
                    using (SqlDataAdapter boThichUng = new SqlDataAdapter(lenh))
                    {
                        DataTable bang = new DataTable();
                        boThichUng.Fill(bang);
                        return bang;
                    }
                }
            }
        }

        public static int ThucThiLenh(string truyVan, SqlParameter[] thamSo = null)
        {
            using (SqlConnection ketNoi = new SqlConnection(ChuoiKetNoi))
            {
                using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                {
                    ketNoi.Open();
                    if (thamSo != null) lenh.Parameters.AddRange(thamSo);
                    return lenh.ExecuteNonQuery();
                }
            }
        }

        public static object LayGiaTriDon(string truyVan, SqlParameter[] thamSo = null)
        {
            using (SqlConnection ketNoi = new SqlConnection(ChuoiKetNoi))
            {
                using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                {
                    ketNoi.Open();
                    if (thamSo != null) lenh.Parameters.AddRange(thamSo);
                    return lenh.ExecuteScalar();
                }
            }
        }
    }

    // ========================================================
    // PHẦN 2: FORM ĐĂNG NHẬP
    // ========================================================
    public class FormDangNhap : Form
    {
        private TextBox txtTaiKhoan;
        private TextBox txtMatKhau;
        private Button btnDangNhap;
        private Button btnThoat;

        public FormDangNhap()
        {
            this.Text = "Đăng Nhập Hệ Thống";
            this.Size = new Size(390, 240);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label lblTieuDe = new Label() { Text = "HỆ THỐNG QUẢN LÝ PHÒNG TRỌ", Font = new Font("Arial", 11, FontStyle.Bold), ForeColor = Color.DarkBlue, AutoSize = true, Location = new Point(50, 20) };
            Label lblUser = new Label() { Text = "Tài khoản:", Location = new Point(35, 65), AutoSize = true };
            Label lblPass = new Label() { Text = "Mật khẩu:", Location = new Point(35, 105), AutoSize = true };

            txtTaiKhoan = new TextBox() { Location = new Point(125, 62), Width = 200, Text = "admin" };
            txtMatKhau = new TextBox() { Location = new Point(125, 102), Width = 200, PasswordChar = '*', Text = "123456" };

            btnDangNhap = new Button() { Text = "Đăng Nhập", Location = new Point(125, 145), Width = 95, Height = 32, BackColor = Color.Teal, ForeColor = Color.White };
            btnThoat = new Button() { Text = "Thoát", Location = new Point(230, 145), Width = 95, Height = 32 };

            btnDangNhap.Click += BtnDangNhap_Click;
            btnThoat.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { lblTieuDe, lblUser, lblPass, txtTaiKhoan, txtMatKhau, btnDangNhap, btnThoat });
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = "SELECT HoTen, VaiTro FROM TaiKhoan WHERE TenDangNhap = @tk AND MatKhau = @mk";
                SqlParameter[] p = {
                    new SqlParameter("@tk", txtTaiKhoan.Text.Trim()),
                    new SqlParameter("@mk", txtMatKhau.Text.Trim())
                };
                DataTable dt = KetNoiCSDL.LayDuLieu(sql, p);

                if (dt.Rows.Count > 0)
                {
                    string hoTen = dt.Rows[0]["HoTen"].ToString();
                    string vaiTro = dt.Rows[0]["VaiTro"].ToString();
                    MessageBox.Show($"Chào mừng {hoTen} ({vaiTro})!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Hide();
                    FormChinh f = new FormChinh(hoTen, vaiTro);
                    f.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ========================================================
    // PHẦN 3: FORM CHÍNH TOÀN DIỆN CỦA 5 MODULE (CÓ ĐỦ NÚT XÓA)
    // ========================================================
    public class FormChinh : Form
    {
        private string nguoiDungHienTai;
        private string vaiTroHienTai;
        private TabControl tabControl;

        // Module 1: Quản lý phòng
        private TabPage tabPhong;
        private DataGridView dgvPhong;
        private TextBox txtTenPhong, txtGiaThue, txtDienTich, txtMoTaPhong;
        private Button btnThemPhong, btnXoaPhong, btnLamMoiPhong;

        // Module 2: Khách thuê & Hợp đồng
        private TabPage tabKhach;
        private DataGridView dgvKhach, dgvHopDong;
        private TextBox txtTenKhach, txtSDT, txtCCCD, txtQueQuan, txtTienCoc;
        private ComboBox cboPhongHopDong, cboKhachHopDong;
        private Button btnThemKhach, btnXoaKhach, btnLapHopDong, btnXoaHopDong, btnLamMoiKhach;

        // Module 3: Điện nước & Dịch vụ
        private TabPage tabDienNuoc;
        private ComboBox cboPhongDienNuoc;
        private NumericUpDown numThangDN, numNamDN, numDienCu, numDienMoi, numNuocCu, numNuocMoi;
        private Button btnLuuChiSo, btnXoaChiSo, btnLamMoiDienNuoc;
        private DataGridView dgvChiSo;

        // Module 4: Hóa đơn & Thanh toán
        private TabPage tabHoaDon;
        private ComboBox cboPhongHoaDon;
        private NumericUpDown numThangHD, numNamHD;
        private Button btnTaoHoaDon, btnThanhToan, btnXoaHoaDon, btnLamMoiHoaDon;
        private DataGridView dgvHoaDon;

        // Module 5: Thống kê & Dashboard
        private TabPage tabThongKe;
        private Label lblThongKePhongTrong, lblThongKeDangThue, lblTongDoanhThu;
        private Button btnCapNhatThongKe;

        public FormChinh(string hoTen, string vaiTro)
        {
            this.nguoiDungHienTai = hoTen;
            this.vaiTroHienTai = vaiTro;
            this.Text = $"Quản Lý Phòng Trọ - Đang đăng nhập: {nguoiDungHienTai} [{vaiTroHienTai}]";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl() { Dock = DockStyle.Fill, Font = new Font("Arial", 9.5f) };

            KhoiTaoTabPhong();
            KhoiTaoTabKhachHopDong();
            KhoiTaoTabDienNuoc();
            KhoiTaoTabHoaDon();
            KhoiTaoTabThongKe();

            tabControl.TabPages.AddRange(new TabPage[] { tabPhong, tabKhach, tabDienNuoc, tabHoaDon, tabThongKe });
            this.Controls.Add(tabControl);

            tabControl.SelectedIndexChanged += (s, e) => {
                if (tabControl.SelectedTab == tabPhong) TaiPhong();
                else if (tabControl.SelectedTab == tabKhach) TaiKhachVaHopDong();
                else if (tabControl.SelectedTab == tabDienNuoc) TaiDienNuoc();
                else if (tabControl.SelectedTab == tabHoaDon) TaiHoaDon();
                else if (tabControl.SelectedTab == tabThongKe) CapNhatThongKe();
            };

            LamMoiTatCa();
        }

        // --- 1. TAB PHÒNG (CÓ NÚT XÓA PHÒNG) ---
        private void KhoiTaoTabPhong()
        {
            tabPhong = new TabPage("1. Quản lý phòng");
            Panel pnl = new Panel() { Dock = DockStyle.Top, Height = 120, BackColor = Color.WhiteSmoke };

            Label l1 = new Label() { Text = "Tên phòng:", Location = new Point(20, 20), AutoSize = true };
            txtTenPhong = new TextBox() { Location = new Point(100, 17), Width = 130 };
            Label l2 = new Label() { Text = "Giá thuê:", Location = new Point(250, 20), AutoSize = true };
            txtGiaThue = new TextBox() { Location = new Point(320, 17), Width = 120 };
            Label l3 = new Label() { Text = "Diện tích (m²):", Location = new Point(460, 20), AutoSize = true };
            txtDienTich = new TextBox() { Location = new Point(555, 17), Width = 80 };
            Label l4 = new Label() { Text = "Mô tả:", Location = new Point(20, 65), AutoSize = true };
            txtMoTaPhong = new TextBox() { Location = new Point(100, 62), Width = 340 };

            btnThemPhong = new Button() { Text = "Thêm Phòng", Location = new Point(460, 58), Width = 110, Height = 32, BackColor = Color.SeaGreen, ForeColor = Color.White };
            btnXoaPhong = new Button() { Text = "Xóa Phòng", Location = new Point(580, 58), Width = 100, Height = 32, BackColor = Color.Firebrick, ForeColor = Color.White };
            btnLamMoiPhong = new Button() { Text = "Làm Mới", Location = new Point(690, 58), Width = 90, Height = 32 };

            btnThemPhong.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtTenPhong.Text) || !decimal.TryParse(txtGiaThue.Text, out decimal gia))
                {
                    MessageBox.Show("Vui lòng nhập tên phòng và giá thuê hợp lệ!", "Cảnh báo"); return;
                }
                double.TryParse(txtDienTich.Text, out double dt);
                string sql = "INSERT INTO Phong (TenPhong, GiaThue, DienTich, TrangThai, MoTa) VALUES (@ten, @gia, @dt, N'Còn trống', @mt)";
                SqlParameter[] p = {
                    new SqlParameter("@ten", txtTenPhong.Text.Trim()),
                    new SqlParameter("@gia", gia),
                    new SqlParameter("@dt", dt),
                    new SqlParameter("@mt", txtMoTaPhong.Text.Trim())
                };
                KetNoiCSDL.ThucThiLenh(sql, p);
                LamMoiTatCa();
                txtTenPhong.Clear(); txtGiaThue.Clear(); txtDienTich.Clear(); txtMoTaPhong.Clear();
                MessageBox.Show("Thêm phòng trọ mới thành công!");
            };

            // NÚT XÓA PHÒNG: Kiểm tra nếu phòng đang cho thuê thì cảnh báo
            btnXoaPhong.Click += (s, e) => {
                if (dgvPhong.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một phòng trên danh sách để xóa!", "Thông báo");
                    return;
                }
                int maPhong = Convert.ToInt32(dgvPhong.CurrentRow.Cells["Mã"].Value);
                string tenPhong = dgvPhong.CurrentRow.Cells["Tên Phòng"].Value.ToString();

                DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa '{tenPhong}' không?\n(Toàn bộ lịch sử điện nước, hóa đơn, hợp đồng của phòng này sẽ bị xóa theo)", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        string sql = @"DELETE FROM HoaDon WHERE MaPhong = @id;
                                       DELETE FROM ChiSoDienNuoc WHERE MaPhong = @id;
                                       DELETE FROM HopDong WHERE MaPhong = @id;
                                       DELETE FROM Phong WHERE MaPhong = @id;";
                        KetNoiCSDL.ThucThiLenh(sql, new SqlParameter[] { new SqlParameter("@id", maPhong) });
                        LamMoiTatCa();
                        MessageBox.Show("Đã xóa phòng thành công!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa phòng: " + ex.Message);
                    }
                }
            };

            btnLamMoiPhong.Click += (s, e) => { LamMoiTatCa(); MessageBox.Show("Đã làm mới dữ liệu!"); };

            pnl.Controls.AddRange(new Control[] { l1, txtTenPhong, l2, txtGiaThue, l3, txtDienTich, l4, txtMoTaPhong, btnThemPhong, btnXoaPhong, btnLamMoiPhong });
            dgvPhong = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            tabPhong.Controls.AddRange(new Control[] { dgvPhong, pnl });
        }

        // --- 2. TAB KHÁCH & HỢP ĐỒNG (CÓ NÚT XÓA KHÁCH & XÓA HỢP ĐỒNG) ---
        private void KhoiTaoTabKhachHopDong()
        {
            tabKhach = new TabPage("2. Khách thuê & Hợp đồng");
            SplitContainer split = new SplitContainer() { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 290 };

            // Panel Khách
            Panel pnlKhach = new Panel() { Dock = DockStyle.Top, Height = 80, BackColor = Color.WhiteSmoke };
            Label l1 = new Label() { Text = "Họ tên:", Location = new Point(15, 15), AutoSize = true };
            txtTenKhach = new TextBox() { Location = new Point(70, 12), Width = 130 };
            Label l2 = new Label() { Text = "SĐT:", Location = new Point(215, 15), AutoSize = true };
            txtSDT = new TextBox() { Location = new Point(255, 12), Width = 110 };
            Label l3 = new Label() { Text = "CCCD:", Location = new Point(380, 15), AutoSize = true };
            txtCCCD = new TextBox() { Location = new Point(430, 12), Width = 120 };
            Label l4 = new Label() { Text = "Quê quán:", Location = new Point(15, 45), AutoSize = true };
            txtQueQuan = new TextBox() { Location = new Point(85, 42), Width = 280 };

            btnThemKhach = new Button() { Text = "Thêm Khách", Location = new Point(380, 40), Width = 110, Height = 28, BackColor = Color.Teal, ForeColor = Color.White };
            btnXoaKhach = new Button() { Text = "Xóa Khách", Location = new Point(500, 40), Width = 95, Height = 28, BackColor = Color.Firebrick, ForeColor = Color.White };
            btnLamMoiKhach = new Button() { Text = "Làm Mới", Location = new Point(605, 40), Width = 80, Height = 28 };

            btnThemKhach.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtTenKhach.Text)) return;
                string sql = "INSERT INTO KhachThue (HoTen, SoDienThoai, CCCD, QueQuan) VALUES (@t, @s, @c, @q)";
                SqlParameter[] p = {
                    new SqlParameter("@t", txtTenKhach.Text.Trim()),
                    new SqlParameter("@s", txtSDT.Text.Trim()),
                    new SqlParameter("@c", txtCCCD.Text.Trim()),
                    new SqlParameter("@q", txtQueQuan.Text.Trim())
                };
                KetNoiCSDL.ThucThiLenh(sql, p);
                txtTenKhach.Clear(); txtSDT.Clear(); txtCCCD.Clear(); txtQueQuan.Clear();
                TaiKhachVaHopDong();
                MessageBox.Show("Thêm khách thuê thành công!");
            };

            // NÚT XÓA KHÁCH THUÊ
            btnXoaKhach.Click += (s, e) => {
                if (dgvKhach.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn khách thuê cần xóa trên bảng!", "Thông báo");
                    return;
                }
                int maKhach = Convert.ToInt32(dgvKhach.CurrentRow.Cells["Mã Khách"].Value);
                string tenKhach = dgvKhach.CurrentRow.Cells["Họ Tên"].Value.ToString();

                DialogResult dr = MessageBox.Show($"Xác nhận xóa khách '{tenKhach}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        // Kiểm tra khách có hợp đồng không
                        object coHD = KetNoiCSDL.LayGiaTriDon("SELECT COUNT(*) FROM HopDong WHERE MaKhach = " + maKhach);
                        if (Convert.ToInt32(coHD) > 0)
                        {
                            MessageBox.Show("Khách này đang có hợp đồng thuê! Bạn cần xóa hợp đồng của khách trước.", "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            return;
                        }
                        KetNoiCSDL.ThucThiLenh("DELETE FROM KhachThue WHERE MaKhach = @id", new SqlParameter[] { new SqlParameter("@id", maKhach) });
                        TaiKhachVaHopDong();
                        MessageBox.Show("Đã xóa khách thuê!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            };

            btnLamMoiKhach.Click += (s, e) => TaiKhachVaHopDong();

            pnlKhach.Controls.AddRange(new Control[] { l1, txtTenKhach, l2, txtSDT, l3, txtCCCD, l4, txtQueQuan, btnThemKhach, btnXoaKhach, btnLamMoiKhach });
            dgvKhach = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            split.Panel1.Controls.AddRange(new Control[] { dgvKhach, pnlKhach });

            // Panel Hợp đồng
            Panel pnlHD = new Panel() { Dock = DockStyle.Top, Height = 75, BackColor = Color.WhiteSmoke };
            Label lP = new Label() { Text = "Phòng trống:", Location = new Point(15, 15), AutoSize = true };
            cboPhongHopDong = new ComboBox() { Location = new Point(105, 12), Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lK = new Label() { Text = "Khách thuê:", Location = new Point(250, 15), AutoSize = true };
            cboKhachHopDong = new ComboBox() { Location = new Point(330, 12), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lC = new Label() { Text = "Tiền cọc:", Location = new Point(495, 15), AutoSize = true };
            txtTienCoc = new TextBox() { Location = new Point(555, 12), Width = 90, Text = "1000000" };

            btnLapHopDong = new Button() { Text = "Lập Hợp Đồng Thuê", Location = new Point(660, 10), Width = 150, Height = 30, BackColor = Color.DarkSlateBlue, ForeColor = Color.White };
            btnXoaHopDong = new Button() { Text = "Hủy / Xóa HĐ", Location = new Point(820, 10), Width = 110, Height = 30, BackColor = Color.Firebrick, ForeColor = Color.White };

            btnLapHopDong.Click += (s, e) => {
                if (cboPhongHopDong.SelectedValue == null || cboKhachHopDong.SelectedValue == null)
                {
                    MessageBox.Show("Không có phòng trống hoặc khách để lập hợp đồng!", "Thông báo"); return;
                }
                decimal.TryParse(txtTienCoc.Text, out decimal coc);
                int pId = Convert.ToInt32(cboPhongHopDong.SelectedValue);
                int kId = Convert.ToInt32(cboKhachHopDong.SelectedValue);

                string sql = "INSERT INTO HopDong (MaPhong, MaKhach, NgayBatDau, TienCoc, TrangThai) VALUES (@p, @k, GETDATE(), @c, N'Hiệu lực');" +
                             "UPDATE Phong SET TrangThai = N'Đang thuê' WHERE MaPhong = @p;";
                SqlParameter[] p = { new SqlParameter("@p", pId), new SqlParameter("@k", kId), new SqlParameter("@c", coc) };
                KetNoiCSDL.ThucThiLenh(sql, p);
                LamMoiTatCa();
                MessageBox.Show("Lập hợp đồng thành công! Phòng đã chuyển sang trạng thái Đang thuê.");
            };

            // NÚT XÓA HỢP ĐỒNG: Tự động trả phòng về trạng thái 'Còn trống'
            btnXoaHopDong.Click += (s, e) => {
                if (dgvHopDong.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng hợp đồng cần xóa trên bảng!", "Thông báo");
                    return;
                }
                int maHD = Convert.ToInt32(dgvHopDong.CurrentRow.Cells["Mã HĐ"].Value);
                string tenPhong = dgvHopDong.CurrentRow.Cells["Phòng"].Value.ToString();

                DialogResult dr = MessageBox.Show($"Xóa hợp đồng này và chuyển '{tenPhong}' về trạng thái 'Còn trống'?", "Xác nhận xóa hợp đồng", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    string sql = @"DECLARE @pId INT;
                                   SELECT @pId = MaPhong FROM HopDong WHERE MaHopDong = @id;
                                   DELETE FROM HopDong WHERE MaHopDong = @id;
                                   UPDATE Phong SET TrangThai = N'Còn trống' WHERE MaPhong = @pId;";
                    KetNoiCSDL.ThucThiLenh(sql, new SqlParameter[] { new SqlParameter("@id", maHD) });
                    LamMoiTatCa();
                    MessageBox.Show("Đã xóa hợp đồng và phòng đã được trả về trạng thái Còn trống!");
                }
            };

            pnlHD.Controls.AddRange(new Control[] { lP, cboPhongHopDong, lK, cboKhachHopDong, lC, txtTienCoc, btnLapHopDong, btnXoaHopDong });
            dgvHopDong = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            split.Panel2.Controls.AddRange(new Control[] { dgvHopDong, pnlHD });

            tabKhach.Controls.Add(split);
        }

        // --- 3. TAB ĐIỆN NƯỚC (CÓ NÚT XÓA CHỈ SỐ) ---
        private void KhoiTaoTabDienNuoc()
        {
            tabDienNuoc = new TabPage("3. Điện nước & Dịch vụ");
            Panel pnl = new Panel() { Dock = DockStyle.Top, Height = 115, BackColor = Color.WhiteSmoke };

            Label l1 = new Label() { Text = "Phòng:", Location = new Point(20, 18), AutoSize = true };
            cboPhongDienNuoc = new ComboBox() { Location = new Point(75, 15), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l2 = new Label() { Text = "Tháng:", Location = new Point(210, 18), AutoSize = true };
            numThangDN = new NumericUpDown() { Location = new Point(260, 16), Width = 50, Minimum = 1, Maximum = 12, Value = DateTime.Now.Month };
            Label l3 = new Label() { Text = "Năm:", Location = new Point(325, 18), AutoSize = true };
            numNamDN = new NumericUpDown() { Location = new Point(365, 16), Width = 70, Minimum = 2000, Maximum = 2100, Value = DateTime.Now.Year };

            Label l4 = new Label() { Text = "Điện cũ:", Location = new Point(20, 60), AutoSize = true };
            numDienCu = new NumericUpDown() { Location = new Point(75, 58), Width = 65, Maximum = 999999 };
            Label l5 = new Label() { Text = "Điện mới:", Location = new Point(155, 60), AutoSize = true };
            numDienMoi = new NumericUpDown() { Location = new Point(220, 58), Width = 65, Maximum = 999999 };
            Label l6 = new Label() { Text = "Nước cũ:", Location = new Point(300, 60), AutoSize = true };
            numNuocCu = new NumericUpDown() { Location = new Point(360, 58), Width = 65, Maximum = 999999 };
            Label l7 = new Label() { Text = "Nước mới:", Location = new Point(440, 60), AutoSize = true };
            numNuocMoi = new NumericUpDown() { Location = new Point(510, 58), Width = 65, Maximum = 999999 };

            btnLuuChiSo = new Button() { Text = "Lưu Chỉ Số", Location = new Point(600, 48), Width = 100, Height = 36, BackColor = Color.OrangeRed, ForeColor = Color.White };
            btnXoaChiSo = new Button() { Text = "Xóa Bản Ghi", Location = new Point(710, 48), Width = 100, Height = 36, BackColor = Color.Firebrick, ForeColor = Color.White };
            btnLamMoiDienNuoc = new Button() { Text = "Làm Mới", Location = new Point(820, 48), Width = 80, Height = 36 };

            btnLuuChiSo.Click += (s, e) => {
                if (cboPhongDienNuoc.SelectedValue == null) return;
                string sql = @"IF EXISTS (SELECT 1 FROM ChiSoDienNuoc WHERE MaPhong=@p AND Thang=@t AND Nam=@n)
                                  UPDATE ChiSoDienNuoc SET DienCu=@dc, DienMoi=@dm, NuocCu=@nc, NuocMoi=@nm WHERE MaPhong=@p AND Thang=@t AND Nam=@n
                               ELSE
                                  INSERT INTO ChiSoDienNuoc (MaPhong, Thang, Nam, DienCu, DienMoi, NuocCu, NuocMoi) VALUES (@p, @t, @n, @dc, @dm, @nc, @nm)";
                SqlParameter[] p = {
                    new SqlParameter("@p", Convert.ToInt32(cboPhongDienNuoc.SelectedValue)),
                    new SqlParameter("@t", (int)numThangDN.Value),
                    new SqlParameter("@n", (int)numNamDN.Value),
                    new SqlParameter("@dc", (int)numDienCu.Value),
                    new SqlParameter("@dm", (int)numDienMoi.Value),
                    new SqlParameter("@nc", (int)numNuocCu.Value),
                    new SqlParameter("@nm", (int)numNuocMoi.Value)
                };
                KetNoiCSDL.ThucThiLenh(sql, p);
                TaiDienNuoc();
                MessageBox.Show("Ghi chỉ số điện nước thành công!");
            };

            // NÚT XÓA CHỈ SỐ ĐIỆN NƯỚC
            btnXoaChiSo.Click += (s, e) => {
                if (dgvChiSo.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn một dòng chỉ số điện nước cần xóa!", "Thông báo");
                    return;
                }
                int maCS = Convert.ToInt32(dgvChiSo.CurrentRow.Cells["Mã"].Value);
                DialogResult dr = MessageBox.Show("Xóa bản ghi chỉ số điện nước này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    KetNoiCSDL.ThucThiLenh("DELETE FROM ChiSoDienNuoc WHERE MaChiSo = @id", new SqlParameter[] { new SqlParameter("@id", maCS) });
                    TaiDienNuoc();
                    MessageBox.Show("Đã xóa chỉ số điện nước!");
                }
            };

            btnLamMoiDienNuoc.Click += (s, e) => TaiDienNuoc();

            pnl.Controls.AddRange(new Control[] { l1, cboPhongDienNuoc, l2, numThangDN, l3, numNamDN, l4, numDienCu, l5, numDienMoi, l6, numNuocCu, l7, numNuocMoi, btnLuuChiSo, btnXoaChiSo, btnLamMoiDienNuoc });
            dgvChiSo = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            tabDienNuoc.Controls.AddRange(new Control[] { dgvChiSo, pnl });
        }

        // --- 4. TAB HÓA ĐƠN (CÓ NÚT XÓA HÓA ĐƠN) ---
        private void KhoiTaoTabHoaDon()
        {
            tabHoaDon = new TabPage("4. Hóa đơn & Thanh toán");
            Panel pnl = new Panel() { Dock = DockStyle.Top, Height = 80, BackColor = Color.WhiteSmoke };

            Label l1 = new Label() { Text = "Phòng thuê:", Location = new Point(20, 25), AutoSize = true };
            cboPhongHoaDon = new ComboBox() { Location = new Point(95, 22), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            Label l2 = new Label() { Text = "Tháng:", Location = new Point(230, 25), AutoSize = true };
            numThangHD = new NumericUpDown() { Location = new Point(275, 23), Width = 50, Minimum = 1, Maximum = 12, Value = DateTime.Now.Month };
            Label l3 = new Label() { Text = "Năm:", Location = new Point(340, 25), AutoSize = true };
            numNamHD = new NumericUpDown() { Location = new Point(380, 23), Width = 70, Minimum = 2000, Maximum = 2100, Value = DateTime.Now.Year };

            btnTaoHoaDon = new Button() { Text = "Tạo Hóa Đơn", Location = new Point(475, 18), Width = 120, Height = 34, BackColor = Color.SteelBlue, ForeColor = Color.White };
            btnThanhToan = new Button() { Text = "Thu Tiền", Location = new Point(605, 18), Width = 100, Height = 34, BackColor = Color.ForestGreen, ForeColor = Color.White };
            btnXoaHoaDon = new Button() { Text = "Xóa Hóa Đơn", Location = new Point(715, 18), Width = 110, Height = 34, BackColor = Color.Firebrick, ForeColor = Color.White };
            btnLamMoiHoaDon = new Button() { Text = "Làm Mới", Location = new Point(835, 18), Width = 80, Height = 34 };

            btnTaoHoaDon.Click += (s, e) => {
                if (cboPhongHoaDon.SelectedValue == null)
                {
                    MessageBox.Show("Hiện không có phòng nào đang thuê!", "Thông báo"); return;
                }
                int pId = Convert.ToInt32(cboPhongHoaDon.SelectedValue);
                int t = (int)numThangHD.Value;
                int n = (int)numNamHD.Value;

                string q = @"SELECT p.GiaThue, cs.DienCu, cs.DienMoi, cs.NuocCu, cs.NuocMoi 
                             FROM Phong p 
                             LEFT JOIN ChiSoDienNuoc cs ON p.MaPhong = cs.MaPhong AND cs.Thang=@t AND cs.Nam=@n 
                             WHERE p.MaPhong=@p";
                DataTable dt = KetNoiCSDL.LayDuLieu(q, new SqlParameter[] { new SqlParameter("@p", pId), new SqlParameter("@t", t), new SqlParameter("@n", n) });
                if (dt.Rows.Count == 0) return;

                decimal phong = Convert.ToDecimal(dt.Rows[0]["GiaThue"]);
                int dCu = dt.Rows[0]["DienCu"] != DBNull.Value ? (int)dt.Rows[0]["DienCu"] : 0;
                int dMoi = dt.Rows[0]["DienMoi"] != DBNull.Value ? (int)dt.Rows[0]["DienMoi"] : 0;
                int nCu = dt.Rows[0]["NuocCu"] != DBNull.Value ? (int)dt.Rows[0]["NuocCu"] : 0;
                int nMoi = dt.Rows[0]["NuocMoi"] != DBNull.Value ? (int)dt.Rows[0]["NuocMoi"] : 0;

                string ins = "INSERT INTO HoaDon (MaPhong, Thang, Nam, TienPhong, TienDien, TienNuoc, TienDichVuKhac, TrangThai) VALUES (@p, @t, @n, @tp, @td, @tn, 100000, N'Chưa thanh toán')";
                SqlParameter[] pIns = {
                    new SqlParameter("@p", pId), new SqlParameter("@t", t), new SqlParameter("@n", n),
                    new SqlParameter("@tp", phong),
                    new SqlParameter("@td", Math.Max(0, dMoi - dCu) * 3500),
                    new SqlParameter("@tn", Math.Max(0, nMoi - nCu) * 25000)
                };
                KetNoiCSDL.ThucThiLenh(ins, pIns);
                TaiHoaDon();
                CapNhatThongKe();
                MessageBox.Show("Đã tạo hóa đơn tháng thành công!");
            };

            btnThanhToan.Click += (s, e) => {
                if (dgvHoaDon.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng nhấp chọn một hóa đơn trên danh sách!", "Thông báo"); return;
                }
                int maHD = Convert.ToInt32(dgvHoaDon.CurrentRow.Cells["Mã HĐ"].Value);
                KetNoiCSDL.ThucThiLenh("UPDATE HoaDon SET TrangThai = N'Đã thanh toán', NgayThanhToan = GETDATE() WHERE MaHoaDon = @id", new SqlParameter[] { new SqlParameter("@id", maHD) });
                TaiHoaDon();
                CapNhatThongKe();
                MessageBox.Show("Thanh toán tiền trọ thành công!");
            };

            // NÚT XÓA HÓA ĐƠN
            btnXoaHoaDon.Click += (s, e) => {
                if (dgvHoaDon.CurrentRow == null)
                {
                    MessageBox.Show("Vui lòng chọn dòng hóa đơn cần xóa!", "Thông báo");
                    return;
                }
                int maHD = Convert.ToInt32(dgvHoaDon.CurrentRow.Cells["Mã HĐ"].Value);
                DialogResult dr = MessageBox.Show("Xác nhận xóa hóa đơn này khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (dr == DialogResult.Yes)
                {
                    KetNoiCSDL.ThucThiLenh("DELETE FROM HoaDon WHERE MaHoaDon = @id", new SqlParameter[] { new SqlParameter("@id", maHD) });
                    TaiHoaDon();
                    CapNhatThongKe();
                    MessageBox.Show("Đã xóa hóa đơn!");
                }
            };

            btnLamMoiHoaDon.Click += (s, e) => TaiHoaDon();

            pnl.Controls.AddRange(new Control[] { l1, cboPhongHoaDon, l2, numThangHD, l3, numNamHD, btnTaoHoaDon, btnThanhToan, btnXoaHoaDon, btnLamMoiHoaDon });
            dgvHoaDon = new DataGridView() { Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            tabHoaDon.Controls.AddRange(new Control[] { dgvHoaDon, pnl });
        }

        // --- 5. TAB THỐNG KÊ ---
        private void KhoiTaoTabThongKe()
        {
            tabThongKe = new TabPage("5. Báo cáo & Thống kê");
            tabThongKe.BackColor = Color.White;

            Label title = new Label() { Text = "BẢNG ĐIỀU KHIỂN & BÁO CÁO DOANH THU", Location = new Point(40, 25), Font = new Font("Arial", 14, FontStyle.Bold), AutoSize = true, ForeColor = Color.DarkSlateBlue };

            GroupBox gb1 = new GroupBox() { Text = "Tình trạng phòng", Location = new Point(40, 75), Size = new Size(350, 160), Font = new Font("Arial", 10.5f) };
            lblThongKePhongTrong = new Label() { Text = "• Phòng trống: 0", Location = new Point(25, 45), AutoSize = true, ForeColor = Color.DarkGreen, Font = new Font("Arial", 12, FontStyle.Bold) };
            lblThongKeDangThue = new Label() { Text = "• Đang thuê: 0", Location = new Point(25, 95), AutoSize = true, ForeColor = Color.DarkBlue, Font = new Font("Arial", 12, FontStyle.Bold) };
            gb1.Controls.AddRange(new Control[] { lblThongKePhongTrong, lblThongKeDangThue });

            GroupBox gb2 = new GroupBox() { Text = "Doanh thu tháng hiện tại", Location = new Point(420, 75), Size = new Size(450, 160), Font = new Font("Arial", 10.5f) };
            lblTongDoanhThu = new Label() { Text = "Tổng thu: 0 VNĐ", Location = new Point(25, 60), AutoSize = true, ForeColor = Color.Crimson, Font = new Font("Arial", 14, FontStyle.Bold) };
            gb2.Controls.Add(lblTongDoanhThu);

            btnCapNhatThongKe = new Button() { Text = "Làm Mới Số Liệu", Location = new Point(40, 260), Width = 180, Height = 40, BackColor = Color.Teal, ForeColor = Color.White };
            btnCapNhatThongKe.Click += (s, e) => CapNhatThongKe();

            tabThongKe.Controls.AddRange(new Control[] { title, gb1, gb2, btnCapNhatThongKe });
        }

        // ========================================================
        // HÀM TẢI DỮ LIỆU & RESET BINDING
        // ========================================================
        public void TaiPhong()
        {
            dgvPhong.DataSource = null;
            dgvPhong.DataSource = KetNoiCSDL.LayDuLieu("SELECT MaPhong as [Mã], TenPhong as [Tên Phòng], GiaThue as [Giá Thuê], DienTich as [Diện Tích (m²)], TrangThai as [Trạng Thái], MoTa as [Mô Tả] FROM Phong");
        }

        public void TaiKhachVaHopDong()
        {
            dgvKhach.DataSource = null;
            dgvKhach.DataSource = KetNoiCSDL.LayDuLieu("SELECT MaKhach as [Mã Khách], HoTen as [Họ Tên], SoDienThoai as [SĐT], CCCD, QueQuan as [Quê Quán] FROM KhachThue");

            dgvHopDong.DataSource = null;
            dgvHopDong.DataSource = KetNoiCSDL.LayDuLieu(@"
                SELECT hd.MaHopDong as [Mã HĐ], p.TenPhong as [Phòng], k.HoTen as [Khách Thuê], 
                       hd.NgayBatDau as [Ngày Bắt Đầu], hd.TienCoc as [Tiền Cọc], hd.TrangThai as [Trạng Thái] 
                FROM HopDong hd 
                JOIN Phong p ON hd.MaPhong = p.MaPhong 
                JOIN KhachThue k ON hd.MaKhach = k.MaKhach");

            DataTable dtP = KetNoiCSDL.LayDuLieu("SELECT MaPhong, TenPhong FROM Phong WHERE TrangThai = N'Còn trống'");
            cboPhongHopDong.DataSource = null;
            cboPhongHopDong.DisplayMember = "TenPhong";
            cboPhongHopDong.ValueMember = "MaPhong";
            cboPhongHopDong.DataSource = dtP;

            DataTable dtK = KetNoiCSDL.LayDuLieu("SELECT MaKhach, HoTen FROM KhachThue");
            cboKhachHopDong.DataSource = null;
            cboKhachHopDong.DisplayMember = "HoTen";
            cboKhachHopDong.ValueMember = "MaKhach";
            cboKhachHopDong.DataSource = dtK;
        }

        public void TaiDienNuoc()
        {
            DataTable dtP = KetNoiCSDL.LayDuLieu("SELECT MaPhong, TenPhong FROM Phong");
            cboPhongDienNuoc.DataSource = null;
            cboPhongDienNuoc.DisplayMember = "TenPhong";
            cboPhongDienNuoc.ValueMember = "MaPhong";
            cboPhongDienNuoc.DataSource = dtP;

            dgvChiSo.DataSource = null;
            dgvChiSo.DataSource = KetNoiCSDL.LayDuLieu(@"
                SELECT cs.MaChiSo as [Mã], p.TenPhong as [Phòng], cs.Thang as [Tháng], cs.Nam as [Năm], 
                       cs.DienCu as [Điện Cũ], cs.DienMoi as [Điện Mới], (cs.DienMoi - cs.DienCu) as [Điện Dùng (kWh)], 
                       cs.NuocCu as [Nước Cũ], cs.NuocMoi as [Nước Mới], (cs.NuocMoi - cs.NuocCu) as [Nước Dùng (m³)] 
                FROM ChiSoDienNuoc cs 
                JOIN Phong p ON cs.MaPhong = p.MaPhong");
        }

        public void TaiHoaDon()
        {
            DataTable dtP = KetNoiCSDL.LayDuLieu("SELECT MaPhong, TenPhong FROM Phong WHERE TrangThai = N'Đang thuê'");
            cboPhongHoaDon.DataSource = null;
            cboPhongHoaDon.DisplayMember = "TenPhong";
            cboPhongHoaDon.ValueMember = "MaPhong";
            cboPhongHoaDon.DataSource = dtP;

            dgvHoaDon.DataSource = null;
            dgvHoaDon.DataSource = KetNoiCSDL.LayDuLieu(@"
                SELECT h.MaHoaDon as [Mã HĐ], p.TenPhong as [Phòng], h.Thang as [Tháng], h.Nam as [Năm], 
                       h.TienPhong as [Tiền Phòng], h.TienDien as [Tiền Điện], h.TienNuoc as [Tiền Nước], 
                       h.TienDichVuKhac as [Dịch Vụ], h.TongTien as [TỔNG TIỀN], h.TrangThai as [Trạng Thái] 
                FROM HoaDon h 
                JOIN Phong p ON h.MaPhong = p.MaPhong");
        }

        public void CapNhatThongKe()
        {
            int pTrong = Convert.ToInt32(KetNoiCSDL.LayGiaTriDon("SELECT COUNT(*) FROM Phong WHERE TrangThai = N'Còn trống'") ?? 0);
            int pThue = Convert.ToInt32(KetNoiCSDL.LayGiaTriDon("SELECT COUNT(*) FROM Phong WHERE TrangThai = N'Đang thuê'") ?? 0);

            int thang = DateTime.Now.Month;
            int nam = DateTime.Now.Year;
            object dtObj = KetNoiCSDL.LayGiaTriDon($"SELECT SUM(TongTien) FROM HoaDon WHERE Thang = {thang} AND Nam = {nam} AND TrangThai = N'Đã thanh toán'");
            decimal doanhThu = (dtObj != DBNull.Value && dtObj != null) ? Convert.ToDecimal(dtObj) : 0;

            lblThongKePhongTrong.Text = $"• Phòng trống: {pTrong} phòng";
            lblThongKeDangThue.Text = $"• Đang thuê: {pThue} phòng";
            lblTongDoanhThu.Text = $"Tổng thu (T{thang}/{nam}): {doanhThu:N0} VNĐ";
        }

        public void LamMoiTatCa()
        {
            try
            {
                TaiPhong();
                TaiKhachVaHopDong();
                TaiDienNuoc();
                TaiHoaDon();
                CapNhatThongKe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }
    }

    // ========================================================
    // PHẦN 4: KHỞI CHẠY CHƯƠNG TRÌNH
    // ========================================================
    static class ChuongTrinh
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDangNhap());
        }
    }
}
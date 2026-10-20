-- ========================================================
-- HỆ THỐNG QUẢN LÝ PHÒNG TRỌ (QuanLyPhongTro)
-- ========================================================
CREATE DATABASE QuanLyPhongTro;
GO
USE QuanLyPhongTro;
GO

-- Module 1: Tài khoản & Phân quyền
CREATE TABLE TaiKhoan (
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
    TenDangNhap NVARCHAR(50) NOT NULL UNIQUE,
    MatKhau NVARCHAR(100) NOT NULL,
    HoTen NVARCHAR(100) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL DEFAULT 'NhanVien' -- 'Admin', 'NhanVien'
);
GO

-- Module 1: Quản lý Phòng
CREATE TABLE Phong (
    MaPhong INT IDENTITY(1,1) PRIMARY KEY,
    TenPhong NVARCHAR(50) NOT NULL,
    GiaThue DECIMAL(18,2) NOT NULL,
    DienTich FLOAT NULL,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Còn trống', -- N'Còn trống', N'Đang thuê', N'Bảo trì'
    MoTa NVARCHAR(255) NULL
);
GO

-- Module 2: Khách thuê
CREATE TABLE KhachThue (
    MaKhach INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    SoDienThoai VARCHAR(15) NULL,
    CCCD VARCHAR(20) NULL UNIQUE,
    QueQuan NVARCHAR(150) NULL,
    NgaySinh DATE NULL
);
GO

-- Module 2: Hợp đồng thuê
CREATE TABLE HopDong (
    MaHopDong INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong INT NOT NULL FOREIGN KEY REFERENCES Phong(MaPhong),
    MaKhach INT NOT NULL FOREIGN KEY REFERENCES KhachThue(MaKhach),
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NULL,
    TienCoc DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Hiệu lực' -- N'Hiệu lực', N'Đã kết thúc'
);
GO

-- Module 3: Bảng giá dịch vụ
CREATE TABLE BangGiaDichVu (
    MaDichVu INT IDENTITY(1,1) PRIMARY KEY,
    TenDichVu NVARCHAR(50) NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    DonViTinh NVARCHAR(20) NOT NULL -- 'kWh', 'm3', 'Phòng', 'Người'
);
GO

-- Module 3: Chỉ số điện nước hàng tháng
CREATE TABLE ChiSoDienNuoc (
    MaChiSo INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong INT NOT NULL FOREIGN KEY REFERENCES Phong(MaPhong),
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    DienCu INT NOT NULL,
    DienMoi INT NOT NULL,
    NuocCu INT NOT NULL,
    NuocMoi INT NOT NULL,
    NgayGhi DATE DEFAULT GETDATE(),
    CONSTRAINT UQ_ChiSo_Phong_ThangNam UNIQUE (MaPhong, Thang, Nam)
);
GO

-- Module 4: Hóa đơn & Thanh toán
CREATE TABLE HoaDon (
    MaHoaDon INT IDENTITY(1,1) PRIMARY KEY,
    MaPhong INT NOT NULL FOREIGN KEY REFERENCES Phong(MaPhong),
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    TienPhong DECIMAL(18,2) NOT NULL,
    TienDien DECIMAL(18,2) NOT NULL DEFAULT 0,
    TienNuoc DECIMAL(18,2) NOT NULL DEFAULT 0,
    TienDichVuKhac DECIMAL(18,2) NOT NULL DEFAULT 0,
    TongTien AS (TienPhong + TienDien + TienNuoc + TienDichVuKhac) PERSISTED,
    TrangThai NVARCHAR(30) NOT NULL DEFAULT N'Chưa thanh toán', -- N'Đã thanh toán', N'Chưa thanh toán'
    NgayLap DATE DEFAULT GETDATE(),
    NgayThanhToan DATE NULL
);
GO

-- Dữ liệu mẫu ban đầu
INSERT INTO TaiKhoan (TenDangNhap, MatKhau, HoTen, VaiTro) VALUES 
('admin', '123456', N'Quản trị viên', 'Admin'),
('nhanvien', '123456', N'Nhân viên', 'NhanVien');

INSERT INTO Phong (TenPhong, GiaThue, DienTich, TrangThai, MoTa) VALUES
(N'Phòng 101', 2500000, 20, N'Đang thuê', N'Tầng 1'),
(N'Phòng 102', 2500000, 20, N'Còn trống', N'Tầng 1'),
(N'Phòng 201', 3200000, 25, N'Còn trống', N'Tầng 2 có ban công');

INSERT INTO BangGiaDichVu (TenDichVu, DonGia, DonViTinh) VALUES
(N'Điện', 3500, 'kWh'),
(N'Nước', 25000, 'm3'),
(N'Rác + Wifi', 100000, N'Phòng');
GO
# Quản Lý Vật Tư (QLVT)

Đồ án môn **Hệ quản trị cơ sở dữ liệu** - PTITHCM

## Giới thiệu

Ứng dụng Quản Lý Vật Tư (QLVT) là hệ thống desktop được phát triển bằng C# và DevExpress WinForms giúp quản lý kho vật tư, nhân viên, đơn đặt hàng, phiếu nhập/xuất hiệu quả.

## Tính năng chính

### Quản lý người dùng
- **Đăng nhập/Xác thực**: Hệ thống đăng nhập phân quyền (Admin, Nhân viên)
- **Tạo tài khoản**: Admin có thể tạo mới tài khoản cho nhân viên
- **Phân quyền**: 
  - **Admin**: Full quyền truy cập, tạo tài khoản, backup/restore
  - **Nhân viên**: Quản lý vật tư, đơn đặt hàng, phiếu nhập/xuất

### Quản lý dữ liệu
- **Nhân viên (NHANVIEN)**
  - Thêm, sửa, xóa nhân viên
  - Kiểm tra trùng mã nhân viên
  - Quản lý thông tin: Họ, Tên, CCCD, Ngày sinh, Địa chỉ, Lương, Ghi chú
  - Validate dữ liệu đầu vào (Regex, độ tuổi, lương tối thiểu)

- **Vật tư (VATTU)**
  - Quản lý danh sách vật tư
  - Theo dõi số lượng tồn kho

- **Đơn đặt hàng (DDH)**
  - Tạo và quản lý đơn đặt hàng
  - Chi tiết đơn đặt hàng (CTDDH)

- **Phiếu nhập (PHIEUNHAP)**
  - Lập phiếu nhập kho
  - Chi tiết phiếu nhập (CTPN)

- **Phiếu xuất (PHIEUXUAT)**
  - Lập phiếu xuất kho
  - Chi tiết phiếu xuất (CTPX)

### Báo cáo
- Báo cáo phiếu nhân viên lập trong năm theo loại (DevExpress Reports)

## Cấu trúc Database

| Bảng | Mô tả |
|------|-------|
| `NHANVIEN` | Thông tin nhân viên |
| `VATTU` | Danh sách vật tư |
| `DDH` | Đơn đặt hàng |
| `CTDDH` | Chi tiết đơn đặt hàng |
| `PHIEUNHAP` | Phiếu nhập kho |
| `CTPN` | Chi tiết phiếu nhập |
| `PHIEUXUAT` | Phiếu xuất kho |
| `CTPX` | Chi tiết phiếu xuất |

### Stored Procedures
- `sp_DangNhap`: Xác thực đăng nhập
- `sp_TaoTaiKhoan`: Tạo tài khoản mới
- `sp_XoaTaiKhoan`: Xóa tài khoản
- `sp_TraCuu_KiemTraMaNhanVien`: Kiểm tra trùng mã nhân viên

## Cấu trúc dự án

```
QLVT/
├── Program.cs                           # Entry point, Database connection
├── fmMain.cs                            # Main form (MDI container)
├── fmDangNhap.cs                        # Login form
├── fmTaoTK.cs                           # Create account form
├── fmNhanVien.cs                        # Employee management form
├── fmVatTu.cs                           # Material management form
├── fmDonDatHang.cs                      # Order management form
├── fmPhieuNhap.cs                       # Receipt form
├── fmPhieuXuat.cs                       # Issue form
├── XfrmPhieuNvLapTrongNamTheoLoai.cs    # Report filter form
├── Xrpt_PhieuNvLapTrongNamTheoLoai.cs   # DevExpress report
├── QLVTDataSet.xsd                      # Database schema
└── App.config                           # Connection strings
```

## Yêu cầu hệ thống

- **Framework**: .NET Framework 4.8
- **Database**: Microsoft SQL Server
- **IDE**: Visual Studio 2019/2022
- **DevExpress Components**: DevExpress WinForms v24.2.3

## Cấu hình Connection String

Mở `App.config` và cập nhật connection string:

```xml
<connectionStrings>
  <add name="QLVTConnectionString" 
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=QLVT;User ID=sa;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=True" />
</connectionStrings>
```

Hoặc cập nhật trong `Program.cs`:

```csharp
public static String connstrPublisher = "Data Source=YOUR_SERVER;Initial Catalog=QLVT;Integrated Security=true";
```

## Cài đặt và chạy

1. Mở project trong Visual Studio
2. Restore các package DevExpress (nếu cần)
3. Cập nhật connection string trong `App.config`
4. Nhấn F5 hoặc Build & Run
5. Đăng nhập với tài khoản Admin hoặc Nhân viên

## Quyền truy cập

| Tài khoản | Quyền |
|-----------|-------|
| Admin | Quản lý toàn bộ, tạo tài khoản, backup/restore |
| Nhân viên | Quản lý vật tư, đơn hàng, phiếu nhập/xuất |

## Tác giả

Đồ án môn **Hệ quản trị cơ sở dữ liệu** - PTITHCM

## Giấy phép

Dành cho mục đích học tập

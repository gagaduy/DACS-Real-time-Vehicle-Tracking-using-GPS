# 📡 Hệ thống Quản lý Giám sát Xe trực tuyến (GPS Tracking System)

Chào mừng bạn đến với dự án **GPS Management System**. Đây là hệ thống theo dõi và quản lý phương tiện vận tải thời gian thực, cho phép giám sát vị trí, tốc độ, lịch sử hành trình và quản lý cảnh báo vi phạm một cách hiệu quả và trực quan.

Dự án này được thiết kế theo kiến trúc **ASP.NET Core MVC** hiện đại, tích hợp bản đồ số và hệ thống thông báo đẩy (Push Notification) thời gian thực.

---

## 🛠 Công nghệ sử dụng (Tech Stack)

- **Backend & Frontend Core**: .NET 8.0 (ASP.NET Core MVC & Web API)
- **Cơ sở dữ liệu**: Microsoft SQL Server
- **ORM**: Entity Framework Core (EF Core)
- **Dữ liệu không gian (GIS)**: NetTopologySuite (Handling Spatial/Point data)
- **Thời gian thực**: SignalR (Live Map & Stats Update)
- **Giao diện**:
  - CSS Framework: AdminLTE 3 (Dựa trên Bootstrap 4/5)
  - Thư viện Bản đồ: Leaflet.js
  - Icons: FontAwesome 6
- **Giả lập (Simulator)**: Python 3 (Sử dụng thư viện `requests` để mô phỏng thiết bị GPS)

---

## 📋 Yêu cầu môi trường (Prerequisites)

Trước khi bắt đầu, hãy đảm bảo máy tính của bạn đã cài đặt các công cụ sau:

1. **.NET 8 SDK**: [Tải xuống tại đây](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **SQL Server**: Phiên bản 2019 trở lên hoặc **Docker Desktop** (khuyên dùng).
3. **Python 3.x**: Để chạy script giả lập GPS.
4. **Git**: Để quản lý mã nguồn.
5. **IDE**: Visual Studio 2022 (với gói ASP.NET và Web development) hoặc VS Code.

---

## 🚀 Các bước cài đặt chi tiết

### Bước 1: Clone mã nguồn
Mở Terminal/Command Prompt và chạy các lệnh sau để lấy mã nguồn từ nhánh `develop`:

```bash
git clone https://github.com/gagaduy/DACS-Real-time-Vehicle-Tracking-using-GPS.git
cd DACS-Real-time-Vehicle-Tracking-using-GPS
git checkout develop
```

### Bước 2: Thiết lập Database (Sử dụng Docker)
Nếu bạn đang dùng Mac/Linux (hoặc Windows muốn dùng Docker), hãy chạy lệnh sau để khởi tạo SQL Server:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrongPassword123" \
   -p 1433:1433 --name sql_server_gps \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

### Bước 3: Cập nhật Connection String
Mở file `appsettings.json` trong thư mục gốc và cập nhật chuỗi kết nối của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=GPSManagementDb;User Id=sa;Password=YourStrongPassword123;TrustServerCertificate=True;"
}
```

### Bước 4: Chạy Migration để tạo cấu trúc bảng
Hệ thống sử dụng EF Core. Chạy lệnh sau để tạo Database và các bảng cần thiết:

```bash
dotnet ef database update
```

### Bước 5: Chạy ứng dụng Web
Khởi động server ASP.NET Core:

```bash
dotnet run
```
Sau đó, truy cập ứng dụng tại: `http://localhost:5045` (hoặc port hiển thị trong terminal).

### Bước 6: Chạy script giả lập GPS
Để thấy xe di chuyển trên bản đồ, bạn cần chạy simulator (đảm bảo Web app vẫn đang chạy):

```bash
# Cài đặt thư viện requests nếu chưa có
pip install requests

# Chạy simulator
python gps_simulator.py
```

---

## 🔐 Tài khoản mặc định (Default Accounts)

Hệ thống đã được Seed sẵn các tài khoản sau để bạn kiểm thử phân quyền:

| Vai trò (Role) | Tài khoản (Username) | Mật khẩu (Password) | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Admin** | `admin` | `123` | Quản lý toàn diện (Thiết bị, Xe, Lịch sử, Cảnh báo) |
| **Fleet Manager** | `manager` | `123` | Theo dõi real-time, xem lịch sử và báo cáo cảnh báo |

---

## 👨‍💻 Thành viên phát triển
- **Lead Developer**: Duy HUTECH
- **Organization**: HUTECH University - Global Management Project

---
*Ghi chú: Luôn tạo nhánh mới từ develop khi phát triển tính năng mới: `git checkout -b feature/your-feature-name`*
# Hệ thống Quản lý Xe & Giám sát GPS thời gian thực

## Giới thiệu
Dự án quản lý cho thuê xe tích hợp định vị GPS, cảnh báo vùng an toàn (Geofencing) dành cho doanh nghiệp vận tải.

## Công nghệ sử dụng
- **Backend:** ASP.NET Core 8.0 MVC
- **Database:** Microsoft SQL Server 2022 (Docker)
- **ORM:** Entity Framework Core
- **Spatial Data:** NetTopologySuite
- **Real-time:** SignalR
- **Frontend:** Bootstrap 5, Leaflet.js (Map)

## Hướng dẫn thiết lập
1. Khởi động SQL Server qua Docker:
   `sudo docker start my_sql_server`
2. Cập nhật ConnectionString trong `appsettings.json`.
3. Chạy Migration để tạo database:
   `dotnet ef migrations add InitialCreate`
   `dotnet ef database update`

---

### 3. File `DATABASE.md` (Tài liệu nghiệp vụ cho Agent)

```markdown
# Cấu trúc Cơ sở dữ liệu

## Các bảng chính
1. **Vehicles**: Lưu thông tin xe (Biển số, dòng xe, trạng thái).
2. **GPSHistory**: Lưu lịch sử tọa độ (Dữ liệu Point, Vận tốc, Thời gian).
3. **Geofences**: Lưu vùng an toàn (Dữ liệu Polygon).
4. **Rentals**: Quản lý hợp đồng (Ngày bắt đầu, ngày kết thúc, khách hàng).
5. **Alerts**: Lưu lịch sử vi phạm (Xe nào, vi phạm gì, ở đâu).

## Logic nghiệp vụ
- Tọa độ xe được cập nhật mỗi 3 giây.
- Hệ thống tự động so sánh vị trí hiện tại của xe với vùng Geofence được gán trong hợp đồng thuê.
- Nếu `polygon.Contains(point) == false`, một bản ghi `Alert` sẽ được tạo.

---

### Bước tiếp theo Duy cần làm:
1. Tạo 2 file `.md` trên trong thư mục dự án.
2. Mở Antigravity, chọn Agent Gemini.
3. Dán cái **Master Prompt** ở trên vào.
4. Ngồi xem Agent "múa code" tạo ra các Model đầu tiên cho bạn.

Bạn thử dán vào Antigravity xem nó gen code có "nuột" không nhé! Có chỗ nào nó giải thích khó hiểu cứ hỏi tui.

Hệ thống slide và tài liệu dự án (car_rental_master_plan.html) của Duy đã sẵn sàng! Chúc bạn khởi đầu dự án thuận lợi!
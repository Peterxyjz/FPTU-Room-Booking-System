# Yêu cầu dự án FPTU Room Booking System

## 1. Tổng quan dự án

FPTU Room Booking System là một hệ thống đặt phòng trực tuyến phục vụ cho Đại học FPT, cho phép sinh viên, giảng viên và nhân viên đặt phòng học, phòng họp và các không gian khác trong khuôn viên trường.

## 2. Mục tiêu

- Tạo một nền tảng trực tuyến để quản lý việc đặt phòng hiệu quả
- Giảm thiểu xung đột lịch đặt phòng
- Tối ưu hóa việc sử dụng không gian trong khuôn viên trường
- Cung cấp giao diện quản trị cho admin
- Cung cấp giao diện thân thiện cho người dùng khác

## 3. Các chức năng chính

### 3.1. Chức năng chung

- Đăng nhập/Đăng xuất (tích hợp với hệ thống xác thực của trường)
- Xem thông tin phòng và tình trạng sẵn có
- Thông báo thời gian thực về các sự kiện liên quan (thông qua SignalR)

### 3.2. Chức năng cho Admin

- Quản lý người dùng (thêm, sửa, xóa, phân quyền)
- Quản lý phòng (thêm, sửa, xóa, cập nhật trạng thái)
- Quản lý lịch đặt phòng (xem, duyệt, từ chối, hủy)
- Xem các báo cáo và thống kê sử dụng phòng
- Cấu hình hệ thống (quy định đặt phòng, giờ hoạt động)

### 3.3. Chức năng cho Giảng viên

- Đặt phòng cho lớp học
- Xem lịch sử đặt phòng cá nhân
- Hủy hoặc chỉnh sửa đặt phòng
- Nhận thông báo về tình trạng đặt phòng

### 3.4. Chức năng cho Sinh viên

- Đặt phòng cho hoạt động nhóm/câu lạc bộ
- Xem lịch sử đặt phòng
- Hủy đặt phòng
- Nhận thông báo về tình trạng đặt phòng

## 4. Yêu cầu kỹ thuật

### 4.1. Kiến trúc

- Solution chứa 4 project:
  - **Repositories**: Triển khai mẫu Repository & Unit of Work, xử lý tương tác dữ liệu
  - **Services**: Xử lý business logic
  - **Admin**: Project MVC cho quản trị viên
  - **Client**: Project Razor Pages cho sinh viên và giảng viên

### 4.2. Công nghệ

- ASP.NET Core MVC & Razor Pages (.NET 8)
- Entity Framework Core 8.0
- SignalR để thông báo thời gian thực
- Blazor cho các component UI tương tác
- SQL Server cho cơ sở dữ liệu
- HTML, CSS, JavaScript, Bootstrap

### 4.3. Mô hình dữ liệu (Entities)

- User (Id, Username, Email, Role, etc.)
- Room (Id, Name, Capacity, Type, Status, etc.)
- Booking (Id, RoomId, UserId, StartTime, EndTime, Purpose, Status, etc.)
- Notification (Id, UserId, Message, IsRead, CreatedAt, etc.)
- Building (Id, Name, Location, etc.)

## 5. Yêu cầu phi chức năng

- **Bảo mật**: Xác thực, phân quyền, bảo vệ dữ liệu người dùng
- **Hiệu suất**: Thời gian phản hồi nhanh, xử lý đồng thời nhiều yêu cầu
- **Khả năng mở rộng**: Dễ dàng thêm tính năng mới, hỗ trợ tăng số lượng người dùng
- **Trải nghiệm người dùng**: Giao diện trực quan, dễ sử dụng, responsive
- **Độ tin cậy**: Sẵn sàng cao, sao lưu dữ liệu định kỳ

## 6. Yêu cầu triển khai

### 6.1. Repositories Project

- Sử dụng Generic Repository Pattern
- Triển khai Unit of Work
- Sử dụng EF Core để tương tác với cơ sở dữ liệu
- Xây dựng các query expression để truy vấn dữ liệu hiệu quả

### 6.2. Services Project

- Triển khai business logic
- Xử lý validation và business rules
- Phân tách rõ ràng các service theo chức năng

### 6.3. Admin Project (MVC)

- Xây dựng các controller cho quản lý người dùng, phòng, đặt phòng
- Tạo views cho giao diện quản trị
- Triển khai các tính năng báo cáo và thống kê
- Tích hợp Blazor Components cho các biểu đồ và bảng dữ liệu

### 6.4. Client Project (Razor Pages)

- Xây dựng các page cho việc đặt phòng, xem lịch
- Tạo giao diện thân thiện với người dùng
- Tích hợp Blazor Components cho các phần tương tác

### 6.5. SignalR

- Thông báo thời gian thực về trạng thái đặt phòng
- Cập nhật tình trạng phòng khi có thay đổi
- Thông báo cho admin khi có yêu cầu đặt phòng mới

### 6.6. Blazor Components

- Calendar component hiển thị lịch đặt phòng
- Room status dashboard
- Interactive reports và biểu đồ
- Form đặt phòng với validation thời gian thực

## 7. Testing

- Unit Testing cho Services và Repositories
- Integration Testing cho API endpoints
- UI Testing cho các chức năng chính
- Load Testing để đảm bảo hệ thống hoạt động tốt dưới tải cao

## 8. Documentation

- API Documentation
- Hướng dẫn sử dụng cho người dùng
- Technical documentation cho developers
- Database schema documentation

# Phân tích chi tiết các chức năng chính

## 1. Quản lý người dùng

### 1.1. Chức năng xác thực và phân quyền

- **Đăng nhập/Đăng xuất**: Sử dụng ASP.NET Core Identity để quản lý xác thực
- **Đăng ký tài khoản**: Cho phép người dùng tự tạo tài khoản với thông tin cá nhân, email trường, và mật khẩu. Tài khoản mới cần được xác nhận qua email trước khi kích hoạt.
- **Phân quyền**: Hỗ trợ 3 vai trò rõ ràng:
  - Admin: Quyền quản trị toàn hệ thống
  - Giảng viên: Quyền đặt phòng cho lớp học và các sự kiện giảng dạy
  - Sinh viên: Quyền đặt phòng cho học nhóm và các hoạt động sinh viên
- **Quên mật khẩu**: Khôi phục mật khẩu qua email
- **Khóa/Mở khóa tài khoản**: Admin có thể kích hoạt hoặc vô hiệu hóa tài khoản

### 1.2. Quản lý thông tin người dùng

- **Quản lý hồ sơ cá nhân**: Người dùng có thể cập nhật thông tin cá nhân
- **Lịch sử hoạt động**: Theo dõi lịch sử đặt phòng, hủy phòng
- **Thông báo cá nhân**: Nhận thông báo qua hệ thống khi có sự kiện liên quan

## 2. Quản lý phòng và cơ sở vật chất

### 2.1. Danh mục phòng

- **Thêm/Sửa/Xóa phòng**: Admin có thể quản lý danh sách phòng
- **Phân loại phòng**: Phòng học, phòng hội thảo, phòng lab, phòng làm việc nhóm
- **Thuộc tính phòng**: Sức chứa, tầng, tòa nhà, thiết bị có sẵn
- **Trạng thái phòng**: Sẵn sàng, đang bảo trì, đã đặt, không khả dụng

### 2.2. Quản lý thiết bị phòng

- **Danh sách thiết bị**: Máy chiếu, máy tính, bảng tương tác, microphone, etc.
- **Tình trạng thiết bị**: Theo dõi trạng thái hoạt động của thiết bị
- **Báo cáo sự cố**: Người dùng có thể báo cáo thiết bị hỏng hóc

### 2.3. Quản lý tòa nhà và vị trí

- **Sơ đồ khuôn viên**: Hiển thị các tòa nhà trong khuôn viên trường
- **Thông tin tòa nhà**: Số tầng, vị trí, giờ hoạt động
- **Tìm đường**: Hướng dẫn đường đi đến phòng cần đặt

## 3. Đặt phòng và lịch sử

### 3.1. Quy trình đặt phòng

- **Tìm kiếm phòng trống**: Tìm theo thời gian, sức chứa, loại phòng, thiết bị
- **Đặt phòng**: Chọn ngày, giờ bắt đầu, giờ kết thúc, mục đích sử dụng
- **Lặp lại đặt phòng**: Đặt theo lịch định kỳ (hàng tuần, hàng tháng)
- **Xét duyệt đặt phòng**: Admin duyệt yêu cầu đặt phòng
- **Thông báo kết quả**: Người đặt nhận thông báo về trạng thái đặt phòng

### 3.2. Quản lý đặt phòng

- **Sửa đổi đặt phòng**: Thay đổi thời gian, phòng, mục đích sử dụng
- **Hủy đặt phòng**: Hủy yêu cầu đã đặt với lý do
- **Gia hạn đặt phòng**: Kéo dài thời gian sử dụng
- **Xác nhận sử dụng**: Check-in/check-out khi sử dụng phòng
- **Phát hiện xung đột**: Cảnh báo khi có xung đột lịch đặt phòng

### 3.3. Lịch sử và thống kê

- **Lịch sử đặt phòng**: Xem lại các lần đặt phòng trước đây
- **Thống kê sử dụng**: Biểu đồ và báo cáo về tần suất sử dụng phòng
- **Đánh giá sau sử dụng**: Người dùng đánh giá về phòng và thiết bị sau khi sử dụng
- **Phân tích xu hướng**: Xác định thời điểm cao điểm và phòng được sử dụng nhiều

## 4. Lịch và hiển thị thông tin

### 4.1. Hiển thị lịch

- **Lịch theo ngày/tuần/tháng**: Xem lịch đặt phòng theo các khung thời gian
- **Lịch theo phòng**: Xem lịch sử dụng của một phòng cụ thể
- **Lịch theo tòa nhà**: Xem tình trạng đặt phòng của một tòa nhà
- **Lịch cá nhân**: Người dùng xem lịch đặt phòng cá nhân
- **Xuất lịch**: Xuất lịch ra file PDF, Excel hoặc tích hợp với Google Calendar

### 4.2. Tìm kiếm và lọc

- **Tìm kiếm nâng cao**: Tìm phòng theo nhiều tiêu chí
- **Lọc theo trạng thái**: Lọc phòng theo trạng thái sẵn có
- **Lọc theo thiết bị**: Tìm phòng có thiết bị cụ thể
- **Lọc theo thời gian**: Tìm phòng trống trong khung giờ cụ thể

## 5. Thông báo và giao tiếp thời gian thực

### 5.1. Hệ thống thông báo (SignalR)

- **Thông báo duyệt đặt phòng**: Thông báo khi yêu cầu được duyệt/từ chối
- **Thông báo thay đổi**: Cập nhật khi có thay đổi về lịch hoặc phòng
- **Nhắc nhở sự kiện**: Nhắc nhở trước khi sự kiện diễn ra
- **Thông báo hủy phòng**: Thông báo khi có hủy đặt phòng

### 5.2. Chat và hỗ trợ

- **Hỗ trợ trực tuyến**: Chat với admin để giải đáp thắc mắc
- **Thảo luận nhóm**: Chat nhóm cho người dùng cùng đặt phòng
- **Gửi thông báo**: Admin gửi thông báo chung đến người dùng

## 6. Báo cáo và phân tích

### 6.1. Báo cáo sử dụng

- **Báo cáo theo phòng**: Thống kê tần suất sử dụng từng phòng
- **Báo cáo theo thời gian**: Phân tích khung giờ cao điểm
- **Báo cáo theo đối tượng**: Thống kê theo sinh viên, giảng viên
- **Báo cáo hiệu suất**: Đánh giá hiệu quả sử dụng không gian

### 6.2. Phân tích dữ liệu

- **Dự báo nhu cầu**: Dự đoán nhu cầu sử dụng phòng trong tương lai
- **Phát hiện mẫu**: Nhận biết các mẫu sử dụng phòng
- **Dashboard thống kê**: Bảng điều khiển với các biểu đồ trực quan
- **Xuất báo cáo**: Xuất báo cáo dưới dạng PDF, Excel

## 7. Tích hợp Blazor

### 7.1. Components Blazor

- **Calendar Component**: Hiển thị lịch tương tác với các sự kiện
- **Room Booking Form**: Form đặt phòng với validation thời gian thực
- **Room Availability Viewer**: Hiển thị trạng thái phòng theo thời gian thực
- **Interactive Reports**: Biểu đồ và báo cáo tương tác
- **User Dashboard**: Bảng điều khiển cá nhân cho người dùng

### 7.2. Chức năng tương tác thời gian thực

- **Live Updates**: Cập nhật trạng thái phòng không cần tải lại trang
- **Drag and Drop Booking**: Kéo thả để đặt hoặc thay đổi lịch đặt phòng
- **Real-time Validation**: Kiểm tra tính khả dụng của phòng ngay lập tức
- **Interactive Maps**: Bản đồ tương tác hiển thị vị trí và trạng thái phòng

## 8. Quản lý hệ thống

### 8.1. Cấu hình hệ thống

- **Quy định đặt phòng**: Thời gian tối đa/tối thiểu, hạn chế theo vai trò
- **Thời gian hoạt động**: Cấu hình giờ mở cửa, đóng cửa
- **Quy tắc phê duyệt**: Thiết lập quy trình phê duyệt tự động/thủ công
- **Thông báo hệ thống**: Cấu hình nội dung và tần suất thông báo

### 8.2. Giám sát và bảo trì

- **Log hệ thống**: Ghi nhận các hoạt động và lỗi
- **Sao lưu dữ liệu**: Sao lưu tự động theo lịch
- **Theo dõi hiệu suất**: Giám sát tốc độ và tài nguyên hệ thống
- **Quản lý phiên bản**: Cập nhật và nâng cấp hệ thống

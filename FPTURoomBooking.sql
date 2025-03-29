-- Create Database
--DROP DATABASE FPTURoomBooking

CREATE DATABASE FPTURoomBooking;
GO

USE FPTURoomBooking;
GO

-- Create Role Table
CREATE TABLE [Role] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    Description NVARCHAR(255),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- Create User Table
CREATE TABLE [User] (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    PhoneNumber NVARCHAR(20),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    LastLogin DATETIME,
    CONSTRAINT FK_User_Role FOREIGN KEY (RoleId) REFERENCES [Role](Id)
);
GO

-- Create Building Table
CREATE TABLE Building (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Location NVARCHAR(255) NOT NULL,
    Floors INT NOT NULL,
    Description NVARCHAR(MAX),
    OpenTime TIME,
    CloseTime TIME,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Create Room Table
CREATE TABLE Room (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Capacity INT NOT NULL,
    Type NVARCHAR(50) NOT NULL, -- Phòng học/Phòng hội thảo/Phòng lab/Phòng nhóm
    Status NVARCHAR(50) NOT NULL DEFAULT N'Sẵn sàng', -- Sẵn sàng/Đang bảo trì/Đã đặt/Không khả dụng
    BuildingId INT NOT NULL,
    Floor INT NOT NULL,
    Description NVARCHAR(MAX),
    Image NVARCHAR(255),
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Room_Building FOREIGN KEY (BuildingId) REFERENCES Building(Id)
);
GO

-- Create Equipment Table
CREATE TABLE Equipment (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50) NOT NULL,
    Description NVARCHAR(MAX),
    IsActive BIT NOT NULL DEFAULT 1
);
GO

-- Create RoomEquipment Table (Junction table for Room and Equipment)
CREATE TABLE RoomEquipment (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomId INT NOT NULL,
    EquipmentId INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT N'Hoạt động', -- Hoạt động/Hỏng/Bảo trì
    LastMaintenance DATETIME,
    CONSTRAINT FK_RoomEquipment_Room FOREIGN KEY (RoomId) REFERENCES Room(Id),
    CONSTRAINT FK_RoomEquipment_Equipment FOREIGN KEY (EquipmentId) REFERENCES Equipment(Id),
    CONSTRAINT UQ_RoomEquipment UNIQUE (RoomId, EquipmentId)
);
GO

-- Create Booking Table
CREATE TABLE Booking (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoomId INT NOT NULL,
    UserId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    EndTime DATETIME NOT NULL,
    Purpose NVARCHAR(255) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT N'Chờ duyệt', -- Chờ duyệt/Đã duyệt/Từ chối/Hủy
    IsRecurring BIT NOT NULL DEFAULT 0,
    RecurringPattern NVARCHAR(50), -- Hàng ngày/Hàng tuần/Hàng tháng
    CheckInTime DATETIME,
    CheckOutTime DATETIME,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Booking_Room FOREIGN KEY (RoomId) REFERENCES Room(Id),
    CONSTRAINT FK_Booking_User FOREIGN KEY (UserId) REFERENCES [User](Id),
    CONSTRAINT CK_Booking_Times CHECK (EndTime > StartTime)
);
GO

-- Create Notification Table
CREATE TABLE Notification (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Type NVARCHAR(50) NOT NULL, -- Đặt phòng/Hủy phòng/Thay đổi/Nhắc nhở
    RelatedEntityId INT, -- ID của booking liên quan
    CONSTRAINT FK_Notification_User FOREIGN KEY (UserId) REFERENCES [User](Id)
);
GO

-- Create indexes for better performance
CREATE INDEX IX_User_RoleId ON [User](RoleId);
CREATE INDEX IX_Room_BuildingId ON Room(BuildingId);
CREATE INDEX IX_Room_Status ON Room(Status);
CREATE INDEX IX_RoomEquipment_RoomId ON RoomEquipment(RoomId);
CREATE INDEX IX_RoomEquipment_EquipmentId ON RoomEquipment(EquipmentId);
CREATE INDEX IX_Booking_RoomId ON Booking(RoomId);
CREATE INDEX IX_Booking_UserId ON Booking(UserId);
CREATE INDEX IX_Booking_Status ON Booking(Status);
CREATE INDEX IX_Booking_StartTime ON Booking(StartTime);
CREATE INDEX IX_Notification_UserId ON Notification(UserId);
CREATE INDEX IX_Notification_IsRead ON Notification(IsRead);
GO

-- Insert initial roles
INSERT INTO [Role] (Name, Description)
VALUES 
    (N'Admin', N'Quản trị viên có toàn quyền quản lý hệ thống'),
    (N'Giảng viên', N'Giảng viên có quyền đặt phòng cho lớp học'),
    (N'Sinh viên', N'Sinh viên có quyền đặt phòng cho hoạt động nhóm');
GO

-- Insert sample admin account (password: Admin@123) - This is just for demo purposes
-- In a real application, you would hash the password properly
INSERT INTO [User] (Username, Email, PasswordHash, RoleId, FullName, PhoneNumber)
VALUES ('admin', 'admin@fpt.edu.vn', 'AQAAAAEAACcQAAAAELU3SrNfB4HZ4kCHVkvf5yP8QKdlC8wcAbA2S8Vzf8ebGSVJYrH9L9EZHnbqE2+j6A==', 1, N'Administrator', '0123456789');
GO

-- Insert sample buildings
INSERT INTO Building (Name, Location, Floors, Description, OpenTime, CloseTime)
VALUES 
    (N'Alpha', N'Khu vực A, Khuôn viên FPT', 5, N'Tòa nhà chính khu vực giảng dạy', '07:00:00', '22:00:00'),
    (N'Beta', N'Khu vực B, Khuôn viên FPT', 4, N'Tòa nhà phòng lab và thực hành', '07:00:00', '22:00:00'),
    (N'Gamma', N'Khu vực C, Khuôn viên FPT', 3, N'Tòa nhà phòng hội thảo', '07:00:00', '21:00:00');
GO

-- Insert sample rooms
INSERT INTO Room (Name, Capacity, Type, Status, BuildingId, Floor, Description)
VALUES 
    (N'A101', 40, N'Phòng học', N'Sẵn sàng', 1, 1, N'Phòng học lý thuyết'),
    (N'A102', 40, N'Phòng học', N'Sẵn sàng', 1, 1, N'Phòng học lý thuyết'),
    (N'A201', 30, N'Phòng học', N'Sẵn sàng', 1, 2, N'Phòng học lý thuyết'),
    (N'B101', 25, N'Phòng lab', N'Sẵn sàng', 2, 1, N'Phòng máy tính'),
    (N'B102', 25, N'Phòng lab', N'Đang bảo trì', 2, 1, N'Phòng máy tính'),
    (N'C101', 100, N'Phòng hội thảo', N'Sẵn sàng', 3, 1, N'Phòng hội thảo lớn'),
    (N'A301', 10, N'Phòng nhóm', N'Sẵn sàng', 1, 3, N'Phòng học nhóm nhỏ');
GO

-- Insert sample equipment
INSERT INTO Equipment (Name, Type, Description)
VALUES 
    (N'Máy chiếu', N'Thiết bị nghe nhìn', N'Máy chiếu Epson EB-X41'),
    (N'Máy tính', N'Thiết bị điện tử', N'Máy tính để bàn HP ProDesk'),
    (N'Bảng tương tác', N'Thiết bị giảng dạy', N'Bảng tương tác thông minh'),
    (N'Micro không dây', N'Thiết bị âm thanh', N'Micro không dây Shure BLX24/SM58'),
    (N'Hệ thống âm thanh', N'Thiết bị âm thanh', N'Hệ thống loa và amply');
GO

-- Insert sample room equipment relationships
INSERT INTO RoomEquipment (RoomId, EquipmentId, Status)
VALUES 
    (1, 1, N'Hoạt động'), -- A101 có máy chiếu
    (1, 5, N'Hoạt động'), -- A101 có hệ thống âm thanh
    (2, 1, N'Hoạt động'), -- A102 có máy chiếu
    (2, 5, N'Hoạt động'), -- A102 có hệ thống âm thanh
    (3, 1, N'Hoạt động'), -- A201 có máy chiếu
    (4, 1, N'Hoạt động'), -- B101 có máy chiếu
    (4, 2, N'Hoạt động'), -- B101 có máy tính
    (5, 1, N'Hoạt động'), -- B102 có máy chiếu
    (5, 2, N'Hoạt động'), -- B102 có máy tính
    (6, 1, N'Hoạt động'), -- C101 có máy chiếu
    (6, 3, N'Hoạt động'), -- C101 có bảng tương tác
    (6, 4, N'Hoạt động'), -- C101 có micro không dây
    (6, 5, N'Hoạt động'); -- C101 có hệ thống âm thanh
GO

-- Create a check constraint to prevent booking conflicts
-- This trigger prevents double booking of the same room at overlapping times
CREATE TRIGGER TR_Booking_PreventConflict
ON Booking
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if there are any conflicting bookings
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN Booking b ON i.RoomId = b.RoomId
        WHERE 
            b.Id <> i.Id AND
            b.Status IN (N'Chờ duyệt', N'Đã duyệt') AND
            (
                (i.StartTime BETWEEN b.StartTime AND b.EndTime) OR
                (i.EndTime BETWEEN b.StartTime AND b.EndTime) OR
                (i.StartTime <= b.StartTime AND i.EndTime >= b.EndTime)
            )
    )
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50000, N'Phòng đã được đặt trong khoảng thời gian này.', 1;
    END
END;
GO

-- Create a trigger to update the room status when a booking is approved
CREATE TRIGGER TR_Booking_UpdateRoomStatus
ON Booking
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CurrentDate DATETIME = GETDATE();
    
    -- Update room status to 'Đã đặt' for rooms with active bookings at the current time
    UPDATE Room
    SET Status = N'Đã đặt', UpdatedAt = @CurrentDate
    FROM Room r
    JOIN Booking b ON r.Id = b.RoomId
    WHERE b.Status = N'Đã duyệt' 
        AND @CurrentDate BETWEEN b.StartTime AND b.EndTime;
    
    -- Update room status back to 'Sẵn sàng' for rooms without active bookings
    UPDATE Room
    SET Status = N'Sẵn sàng', UpdatedAt = @CurrentDate
    WHERE Id NOT IN (
        SELECT RoomId 
        FROM Booking 
        WHERE Status = N'Đã duyệt' 
            AND @CurrentDate BETWEEN StartTime AND EndTime
    )
    AND Status = N'Đã đặt'; -- Only update rooms that are currently marked as booked
END;
GO

-- Create a trigger to create notifications when booking status changes
CREATE TRIGGER TR_Booking_CreateNotification
ON Booking
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- For new bookings (insert)
    IF EXISTS (SELECT 1 FROM inserted i WHERE NOT EXISTS (SELECT 1 FROM deleted d WHERE d.Id = i.Id))
    BEGIN
        -- Notify user that booking request has been created
        INSERT INTO Notification (UserId, Message, Type, RelatedEntityId)
        SELECT 
            i.UserId,
            N'Yêu cầu đặt phòng ' + r.Name + N' đã được tạo và đang chờ duyệt.',
            N'Đặt phòng',
            i.Id
        FROM inserted i
        JOIN Room r ON i.RoomId = r.Id;
        
        -- Notify admins about new booking requests
        INSERT INTO Notification (UserId, Message, Type, RelatedEntityId)
        SELECT 
            u.Id,
            N'Có một yêu cầu đặt phòng ' + r.Name + N' mới từ ' + requester.FullName + N'.',
            N'Đặt phòng',
            i.Id
        FROM inserted i
        JOIN Room r ON i.RoomId = r.Id
        JOIN [User] requester ON i.UserId = requester.Id
        CROSS JOIN [User] u
        WHERE u.RoleId = 1; -- Admin role
    END
    
    -- For updated bookings
    IF EXISTS (SELECT 1 FROM inserted i JOIN deleted d ON i.Id = d.Id WHERE i.Status <> d.Status)
    BEGIN
        -- Notify user about booking status change
        INSERT INTO Notification (UserId, Message, Type, RelatedEntityId)
        SELECT 
            i.UserId,
            CASE i.Status
                WHEN N'Đã duyệt' THEN N'Yêu cầu đặt phòng ' + r.Name + N' đã được duyệt.'
                WHEN N'Từ chối' THEN N'Yêu cầu đặt phòng ' + r.Name + N' đã bị từ chối.'
                WHEN N'Hủy' THEN N'Yêu cầu đặt phòng ' + r.Name + N' đã bị hủy.'
                ELSE N'Trạng thái đặt phòng ' + r.Name + N' đã được cập nhật thành ' + i.Status + N'.'
            END,
            N'Thay đổi',
            i.Id
        FROM inserted i
        JOIN deleted d ON i.Id = d.Id
        JOIN Room r ON i.RoomId = r.Id
        WHERE i.Status <> d.Status;
    END
END;
GO

-- Create a stored procedure to get available rooms for a specific time period
CREATE PROCEDURE GetAvailableRooms
    @StartTime DATETIME,
    @EndTime DATETIME,
    @Capacity INT = NULL,
    @Type NVARCHAR(50) = NULL,
    @BuildingId INT = NULL,
    @EquipmentIds NVARCHAR(MAX) = NULL -- Comma-separated list of equipment IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create a temporary table to hold equipment IDs if provided
    DECLARE @TempEquipment TABLE (EquipmentId INT);
    
    -- Parse the comma-separated equipment IDs if provided
    IF @EquipmentIds IS NOT NULL AND LEN(@EquipmentIds) > 0
    BEGIN
        INSERT INTO @TempEquipment (EquipmentId)
        SELECT value FROM STRING_SPLIT(@EquipmentIds, ',');
    END
    
    -- Get the count of requested equipment if any
    DECLARE @EquipmentCount INT = (SELECT COUNT(*) FROM @TempEquipment);
    
    -- Get available rooms
    SELECT 
        r.*,
        b.Name AS BuildingName,
        b.Location AS BuildingLocation,
        (
            SELECT COUNT(*) 
            FROM RoomEquipment re 
            WHERE re.RoomId = r.Id AND re.Status = N'Hoạt động'
        ) AS EquipmentCount,
        (
            SELECT STRING_AGG(e.Name, ', ') 
            FROM RoomEquipment re 
            JOIN Equipment e ON re.EquipmentId = e.Id 
            WHERE re.RoomId = r.Id AND re.Status = N'Hoạt động'
        ) AS EquipmentList
    FROM Room r
    JOIN Building b ON r.BuildingId = b.Id
    WHERE r.Status IN (N'Sẵn sàng') -- Only rooms that are ready
        AND (@Capacity IS NULL OR r.Capacity >= @Capacity) -- Filter by capacity if provided
        AND (@Type IS NULL OR r.Type = @Type) -- Filter by room type if provided
        AND (@BuildingId IS NULL OR r.BuildingId = @BuildingId) -- Filter by building if provided
        AND r.Id NOT IN ( -- Exclude rooms that have approved bookings in the requested time period
            SELECT DISTINCT RoomId
            FROM Booking
            WHERE Status = N'Đã duyệt'
                AND (
                    (StartTime <= @StartTime AND EndTime > @StartTime) OR
                    (StartTime < @EndTime AND EndTime >= @EndTime) OR
                    (StartTime >= @StartTime AND EndTime <= @EndTime)
                )
        )
        -- If equipment is specified, make sure the room has all the requested equipment
        AND (@EquipmentCount = 0 OR (
            SELECT COUNT(DISTINCT EquipmentId)
            FROM RoomEquipment re
            WHERE re.RoomId = r.Id
                AND re.Status = N'Hoạt động'
                AND re.EquipmentId IN (SELECT EquipmentId FROM @TempEquipment)
        ) = @EquipmentCount)
    ORDER BY r.BuildingId, r.Floor, r.Name;
END;
GO

-- Create a stored procedure to get booking statistics
CREATE PROCEDURE GetBookingStatistics
    @StartDate DATETIME = NULL,
    @EndDate DATETIME = NULL,
    @BuildingId INT = NULL,
    @RoomType NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Set default dates if not provided
    SET @StartDate = ISNULL(@StartDate, DATEADD(MONTH, -1, GETDATE()));
    SET @EndDate = ISNULL(@EndDate, GETDATE());
    
    -- Get booking statistics per room
    SELECT 
        r.Id AS RoomId,
        r.Name AS RoomName,
        r.Type AS RoomType,
        b.Name AS BuildingName,
        r.Floor,
        COUNT(bk.Id) AS TotalBookings,
        SUM(CASE WHEN bk.Status = N'Đã duyệt' THEN 1 ELSE 0 END) AS ApprovedBookings,
        SUM(CASE WHEN bk.Status = N'Từ chối' THEN 1 ELSE 0 END) AS RejectedBookings,
        SUM(CASE WHEN bk.Status = N'Hủy' THEN 1 ELSE 0 END) AS CancelledBookings,
        SUM(CASE WHEN bk.Status = N'Chờ duyệt' THEN 1 ELSE 0 END) AS PendingBookings,
        SUM(DATEDIFF(MINUTE, bk.StartTime, bk.EndTime)) AS TotalBookingMinutes,
        AVG(DATEDIFF(MINUTE, bk.StartTime, bk.EndTime)) AS AvgBookingDurationMinutes
    FROM Room r
    LEFT JOIN Booking bk ON r.Id = bk.RoomId AND bk.StartTime BETWEEN @StartDate AND @EndDate
    JOIN Building b ON r.BuildingId = b.Id
    WHERE (@BuildingId IS NULL OR r.BuildingId = @BuildingId)
        AND (@RoomType IS NULL OR r.Type = @RoomType)
    GROUP BY r.Id, r.Name, r.Type, b.Name, r.Floor
    ORDER BY TotalBookings DESC;
    
    -- Get booking statistics per day
    SELECT 
        CAST(bk.StartTime AS DATE) AS BookingDate,
        COUNT(bk.Id) AS TotalBookings,
        COUNT(DISTINCT bk.RoomId) AS RoomsBooked,
        COUNT(DISTINCT bk.UserId) AS UniqueUsers,
        SUM(DATEDIFF(MINUTE, bk.StartTime, bk.EndTime)) AS TotalBookingMinutes
    FROM Booking bk
    JOIN Room r ON bk.RoomId = r.Id
    WHERE bk.StartTime BETWEEN @StartDate AND @EndDate
        AND (@BuildingId IS NULL OR r.BuildingId = @BuildingId)
        AND (@RoomType IS NULL OR r.Type = @RoomType)
    GROUP BY CAST(bk.StartTime AS DATE)
    ORDER BY BookingDate;
    
    -- Get most active users
    SELECT TOP 10
        u.Id AS UserId,
        u.FullName,
        u.Email,
        ro.Name AS UserRole,
        COUNT(bk.Id) AS TotalBookings,
        SUM(CASE WHEN bk.Status = N'Đã duyệt' THEN 1 ELSE 0 END) AS ApprovedBookings,
        SUM(DATEDIFF(MINUTE, bk.StartTime, bk.EndTime)) AS TotalBookingMinutes
    FROM Booking bk
    JOIN [User] u ON bk.UserId = u.Id
    JOIN [Role] ro ON u.RoleId = ro.Id
    JOIN Room r ON bk.RoomId = r.Id
    WHERE bk.StartTime BETWEEN @StartDate AND @EndDate
        AND (@BuildingId IS NULL OR r.BuildingId = @BuildingId)
        AND (@RoomType IS NULL OR r.Type = @RoomType)
    GROUP BY u.Id, u.FullName, u.Email, ro.Name
    ORDER BY TotalBookings DESC;
END;
GO
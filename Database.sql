USE MASTER
-- Xóa database hiện có nếu tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ShopEProduction')
BEGIN
    DROP DATABASE ShopEProduction;
END

-- Tạo mới database
CREATE DATABASE ShopEProduction;
GO

-- Chọn sử dụng database
USE ShopEProduction;
GO

-- Bảng Roles để lưu trữ chức năng của người dùng
CREATE TABLE ROLES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ROLE_NAME VARCHAR(50)
);
GO

-- Insert roles: Admin (ID = 1) and User (ID = 2)
INSERT INTO ROLES (ROLE_NAME)
VALUES ('Admin'), ('User');


-- Tạo bảng Users
CREATE TABLE USERS (
    ID INT IDENTITY(1,1) PRIMARY KEY,  -- Auto-incrementing field starting at 1
    USERNAME VARCHAR(50) NOT NULL UNIQUE,
    PASSWORD NVARCHAR(255) NOT NULL,
    FULLNAME VARCHAR(100),
    USER_IMAGE VARCHAR(255),
    EMAIL VARCHAR(100) NOT NULL UNIQUE,
    PHONENUMBER VARCHAR(20),
    USER_CREATE_AT DATETIME DEFAULT GETDATE(),
    USER_POINT INT,
    USER_ROLE_ID INT,
    USER_STATUS BIT,  -- BIT used for boolean
    FOREIGN KEY (USER_ROLE_ID) REFERENCES ROLES(ID)
);
GO

-- Kiểm tra dữ liệu bảng USERS
SELECT * FROM USERS;
GO

-- Insert users with role assignments
INSERT INTO USERS (USERNAME, PASSWORD, FULLNAME, USER_IMAGE, EMAIL, PHONENUMBER, USER_CREATE_AT, USER_POINT, USER_ROLE_ID, USER_STATUS)
VALUES
    ('quan', '123456', 'Nguyen Tat Quan', 'local_image_quan', 'tatquan1803@gmail.com', '0837931504', '2024-02-15 14:30:00', 10, 1, 1),  -- Admin
    ('quannt', '123456', 'Nguyen Tat Quan', 'local_image_quannt', 'quan2@hotmail.com', '0829505619', '2025-02-19 04:30:00', 2, 2, 0),  -- User
    ('tatquan', '123456', 'Nguyen Tat Quan', 'local_image_tatquan', 'quan1@outlook.com', '0837938956', GETDATE(), 2, 2, 1);  -- User


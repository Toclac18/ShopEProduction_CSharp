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


-- Create table Categories
CREATE TABLE CATEGORIES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CATEGORY_NAME VARCHAR(100) UNIQUE NOT NULL,  -- Ensures no duplicate category names
    DESCRIPTION TEXT,  -- Additional details about the category
    STATUS BIT DEFAULT 1 -- 1 = Active, 0 = Inactive
);

-- Create Products Table
CREATE TABLE PRODUCTS (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- Auto increment for better ID management
    NAME VARCHAR(100) NOT NULL,  -- Increased size for more flexibility
    PRODUCT_CATEGORY INT NOT NULL,  -- Renamed for clarity
    SOLD_NUMBER INT DEFAULT 0,  -- Fixed typo and added default value
    STATUS BIT DEFAULT 1, -- 1 = available, 0 = out of stock
    CONSTRAINT FK_PRODUCT_CATEGORY FOREIGN KEY (PRODUCT_CATEGORY) REFERENCES CATEGORIES(ID)
);

-- Create Product Details Table
CREATE TABLE PRODUCT_DETAILS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    PRODUCT_ID INT NOT NULL,
    DETAIL_DESC NVARCHAR(255),
    STATUS BIT DEFAULT 1, -- 1 = available, 0 = sold
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(ID) ON DELETE CASCADE
);

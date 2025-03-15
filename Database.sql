USE MASTER;

-- Disconnect all users from the database if they are connected
ALTER DATABASE ShopEProduction SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

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

DROP TABLE ROLES;
-- Bảng Roles để lưu trữ chức năng của người dùng
CREATE TABLE ROLES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ROLE_NAME VARCHAR(50)
);
GO

-- Insert roles: Admin (ID = 1) and User (ID = 2)
INSERT INTO ROLES (ROLE_NAME)
VALUES ('Admin'), ('User');

DROP TABLE USERS;
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

DROP TABLE CATEGORIES;
-- Create table Categories
CREATE TABLE CATEGORIES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CATEGORY_NAME VARCHAR(100) UNIQUE NOT NULL,  -- Ensures no duplicate category names
    DESCRIPTION TEXT,  -- Additional details about the category
    STATUS BIT DEFAULT 1 -- 1 = Active, 0 = Inactive
);

INSERT INTO CATEGORIES (CATEGORY_NAME, DESCRIPTION) VALUES
('Streaming Services', 'Netflix, YouTube Premium, Spotify, Disney+ accounts'),
('Mobile Cards', 'Top-up cards for mobile networks such as Verizon, AT&T, T-Mobile'),
('Gaming & Gift Cards', 'Google Play, App Store, Steam, PlayStation, Xbox gift cards'),
('Software & Licenses', 'Windows, Microsoft Office, Antivirus, VPN subscriptions'),
('E-books & Learning', 'Online courses, digital books, e-learning subscriptions');

SELECT * FROM CATEGORIES;

DROP TABLE PRODUCTS;
-- Create Products Table with condition for rented items
CREATE TABLE PRODUCTS (
    ID INT IDENTITY(1,1) PRIMARY KEY, -- Auto increment for better ID management
    NAME VARCHAR(100) NOT NULL,  -- Increased size for more flexibility
    CATEGORY_ID INT NOT NULL,  -- Product category
    SOLD_NUMBER INT NULL,  -- For sale items only; NULL for rent items
    CURRENT_AVAILABLE INT NOT NULL, -- Count from table PRODUCT_DETAILS
    TYPE INT NOT NULL, -- 0 for sell items, 1 for rent items
    STATUS BIT DEFAULT 1, -- 1 = available, 0 = out of stock
    FOREIGN KEY (CATEGORY_ID) REFERENCES CATEGORIES(ID)
);

DROP TABLE PRODUCT_DETAILS;
-- Create Product Details Table for rented and sold items
CREATE TABLE PRODUCT_DETAILS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    PRODUCT_ID INT NOT NULL,
    PRODUCT_TYPE INT NOT NULL,
    PRICE FLOAT NOT NULL,
    RELEASE_DATE DATETIME,
    EXPIRED_DATE DATETIME,
    DETAIL_DESC NVARCHAR(255) NOT NULL,
    DETAIL_PRIVATE_DESC NVARCHAR(255) NOT NULL,
    DURATION DATETIME,
    IS_RENTED_FLG BIT DEFAULT 0, -- 0 is not rented, 1 is rented, null if sell items
    IS_BOUGHT_FLG BIT DEFAULT 0, -- 0 is not yet purchased, 1 is purchased, null if rent items
    STATUS BIT DEFAULT 1, -- 1 is active, 0 is inactive
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(ID) ON DELETE CASCADE
);


INSERT INTO PRODUCTS (NAME, CATEGORY_ID, SOLD_NUMBER, CURRENT_AVAILABLE, TYPE, STATUS) VALUES
('Netflix Account for limited time Subscription', 1, 50, 100, 1, 1), -- Rent item
('YouTube Premium for rented demands', 1, 30, 80, 1, 1), -- Rent item
('Spotify Family Plan in short period', 1, 25, 50, 1, 1), -- Rent item
('Netflix Subscription For Owner (limit 1 user)', 1, 50, 100, 0, 1), -- Sell item
('Netflix Subscription For Owner (public account)', 1, 0, 100, 0, 1), -- Sell item
('YouTube Premium', 1, 30, 80, 0, 1), -- Sell item
('Spotify Family Plan', 1, 25, 50, 0, 1), -- Sell item
('Verizon $50 Mobile Card', 2, 10, 10, 0, 1), -- Sell item
('Verizon $100 Mobile Card', 2, 40, 18, 0, 1), -- Sell item
('Google Play $25 Gift Card', 3, 35, 20, 0, 1), -- Sell item
('Google Play $25 Gift Card', 3, 35, 20, 0, 1), -- Sell item
('Google Play $25 Gift Card', 3, 35, 20, 0, 0), -- Out of stock
('Windows 11 Pro License Key', 4, 20, 5, 0, 1), -- Sell item
('Windows 11 Pro License Key', 4, 20, 5, 0, 0), -- Out of stock
('Windows 11 Pro License Key', 4, 20, 5, 0, 1), -- Sell item
('Windows 11 Pro License Key', 4, 20, 5, 0, 0), -- Out of stock
('Microsoft Office 365 Annual Plan', 4, 15, 25, 0, 1), -- Sell item
('Microsoft Office 365 Annual Plan', 4, NULL, 25, 1, 1), -- Rent item (SOLD_NUMBER = NULL)
('Microsoft Office 365 Annual Plan', 4, NULL, 25, 1, 1), -- Rent item (SOLD_NUMBER = NULL)
('Microsoft Office 365 Annual Plan', 4, NULL, 25, 1, 1), -- Rent item (SOLD_NUMBER = NULL)
('Udemy Online Course - Digital Marketing', 5, 10, 60, 0, 1), -- Sell item
('Udemy Online Course - Digital Marketing', 5, 10, 60, 0, 1), -- Sell item
('Udemy Online Course - Digital Marketing For Rented User', 5, NULL, 60, 1, 1); -- Rent item (SOLD_NUMBER = NULL)


-- Insert for Products in Table PRODUCT_DETAILS
INSERT INTO PRODUCT_DETAILS (PRODUCT_ID, PRODUCT_TYPE, PRICE, RELEASE_DATE, EXPIRED_DATE, DETAIL_DESC, DETAIL_PRIVATE_DESC, DURATION, IS_RENTED_FLG, IS_BOUGHT_FLG, STATUS)
VALUES
-- For sell items (PRODUCT_TYPE = 0)
(1, 0, 10.00, NULL, NULL, 'Netflix Account for limited time Subscription. Limited-time Netflix subscription with access to premium content for sale.', 'Login details sent after purchase: Username and password.', NULL, NULL, 0, 1),
(2, 0, 15.00, NULL, NULL, 'YouTube Premium for rented demands. Get YouTube Premium subscription for a limited time rental.', 'Login details sent after purchase: Username and password for YouTube Premium.', NULL, NULL, 0, 1),
(3, 0, 20.00, NULL, NULL, 'Spotify Family Plan in short period. Spotify family plan with up to 6 accounts available for sale for a limited time.', 'Account details for Spotify Family Plan will be sent via email.', NULL, NULL, 0, 1),
(4, 0, 25.00, NULL, NULL, 'Netflix Subscription For Owner (limit 1 user). Personal Netflix account for sale, valid for a single user.', 'Login credentials will be sent after purchase.', NULL, NULL, 0, 1),
(5, 0, 30.00, NULL, NULL, 'Netflix Subscription For Owner (public account). Public Netflix account for sale with shared access.', 'Credentials will be sent via phone or email.', NULL, NULL, 0, 1),
(6, 0, 12.00, NULL, NULL, 'YouTube Premium. YouTube Premium account available for purchase.', 'Account login details sent after payment confirmation.', NULL, NULL, 0, 1),
(7, 0, 18.00, NULL, NULL, 'Spotify Family Plan. Spotify family plan subscription for sale, up to 6 accounts.', 'Login details will be sent to the buyer.', NULL, NULL, 0, 1),
(8, 0, 50.00, NULL, NULL, 'Verizon $50 Mobile Card. Verizon prepaid mobile card for sale.', 'Code for activation will be sent to your email after purchase.', NULL, NULL, 0, 1),
(9, 0, 100.00, NULL, NULL, 'Verizon $100 Mobile Card. Prepaid Verizon mobile card with a value of $100.', 'Activation code sent upon purchase via email.', NULL, NULL, 0, 1),
(10, 0, 25.00, NULL, NULL, 'Google Play $25 Gift Card. Google Play store gift card worth $25 for sale.', 'The gift card code will be emailed after purchase.', NULL, NULL, 0, 1),
(11, 0, 25.00, NULL, NULL, 'Google Play $25 Gift Card. Google Play gift card available for sale.', 'Gift card code will be emailed to you.', NULL, NULL, 0, 1),
(12, 0, 25.00, NULL, NULL, 'Google Play $25 Gift Card. Google Play gift card with a value of $25 for sale.', 'Gift card code emailed after purchase.', NULL, NULL, 0, 1),
(13, 0, 150.00, NULL, NULL, 'Windows 11 Pro License Key. Windows 11 Pro key for sale.', 'License key sent after purchase confirmation.', NULL, NULL, 0, 1),
(14, 0, 150.00, NULL, NULL, 'Windows 11 Pro License Key. Windows 11 Pro license key for sale.', 'License key sent via email after purchase.', NULL, NULL, 0, 1),
(15, 0, 150.00, NULL, NULL, 'Windows 11 Pro License Key. Windows 11 Pro license for sale.', 'Key sent to your email after payment confirmation.', NULL, NULL, 0, 1),
(16, 0, 150.00, NULL, NULL, 'Windows 11 Pro License Key. A new Windows 11 Pro key available for sale.', 'License key will be sent to your email.', NULL, NULL, 0, 1),
(17, 0, 100.00, NULL, NULL, 'Microsoft Office 365 Annual Plan. Annual subscription for Microsoft Office 365 for sale.', 'Login credentials for Office 365 will be sent after purchase.', NULL, NULL, 0, 1),
(18, 0, 100.00, NULL, NULL, 'Microsoft Office 365 Annual Plan. Office 365 annual plan available for sale.', 'Office 365 login details sent after payment.', NULL, NULL, 0, 1),
(19, 0, 100.00, NULL, NULL, 'Microsoft Office 365 Annual Plan. A complete Microsoft Office 365 annual subscription available for purchase.', 'Office 365 account details will be emailed.', NULL, NULL, 0, 1),
(20, 0, 100.00, NULL, NULL, 'Microsoft Office 365 Annual Plan. Annual Microsoft Office 365 plan for sale.', 'Credentials sent to your email after purchase confirmation.', NULL, NULL, 0, 1),
(21, 0, 50.00, NULL, NULL, 'Udemy Online Course - Digital Marketing. Online course in Digital Marketing available for sale.', 'Course access details will be emailed after purchase.', NULL, NULL, 0, 1),
(22, 0, 50.00, NULL, NULL, 'Udemy Online Course - Digital Marketing. Udemy Digital Marketing course available for purchase.', 'Course login details will be sent to you via email.', NULL, NULL, 0, 1),

-- For rented items (PRODUCT_TYPE = 1)
(1, 1, 10.00, NULL, NULL, 'Udemy Online Course - Digital Marketing For Rented User. Rent this Udemy Digital Marketing course for a limited period.', 'Renting access details will be sent after payment confirmation.', NULL, 0, NULL, 1),
(2, 1, 12.00, NULL, NULL, 'YouTube Premium for rented demands. Rent YouTube Premium subscription for a period.', 'Login details for rental will be sent after confirmation.', NULL, 0, NULL, 1),
(3, 1, 20.00, NULL, NULL, 'Netflix Account for limited time Subscription. Rent Netflix account for a limited time subscription.', 'Login details sent upon successful rental.', NULL, 0, NULL, 1);


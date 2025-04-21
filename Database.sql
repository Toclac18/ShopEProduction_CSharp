USE MASTER;

-- Disconnect all users from the database if they are connected
ALTER DATABASE ShopEProduction SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

-- Xóa database hiện có nếu tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'ShopEProduction')
BEGIN
    DROP DATABASE ShopEProduction;
END;

-- Tạo mới database
CREATE DATABASE ShopEProduction;
GO

-- Chọn sử dụng database
USE ShopEProduction;
GO

--DROP TABLE ROLES;
-- Bảng Roles để lưu trữ chức năng của người dùng
CREATE TABLE ROLES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    ROLE_NAME VARCHAR(50)
);
GO

-- Insert roles: Admin (ID = 1) and User (ID = 2)
INSERT INTO ROLES (ROLE_NAME)
VALUES ('Admin'), ('User');

--DROP TABLE USERS;
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

--DROP TABLE CATEGORIES;
-- Create table Categories
CREATE TABLE CATEGORIES (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CATEGORY_NAME VARCHAR(100) UNIQUE NOT NULL,  -- Ensures no duplicate category names
    DESCRIPTION TEXT,  -- Additional details about the category
    STATUS BIT DEFAULT 1 -- 1 = Active, 0 = Inactive
);

SELECT * FROM CATEGORIES;

--DROP TABLE PRODUCTS;
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

--DROP TABLE PRODUCT_DETAILS;
-- Create Product Details Table for rented and sold items
CREATE TABLE PRODUCT_DETAILS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    PRODUCT_ID INT NOT NULL,
    PRODUCT_TYPE INT NOT NULL,
    PRICE FLOAT NOT NULL,
    RELEASED_DATE DATETIME,
    DETAIL_DESC NVARCHAR(255) NOT NULL,
    DETAIL_PRIVATE_DESC NVARCHAR(255) NOT NULL,
    IS_RENTED_FLG BIT DEFAULT 0, -- 0 is not rented, 1 is rented, null if sell items
    IS_BOUGHT_FLG BIT DEFAULT 0, -- 0 is not yet purchased, 1 is purchased, null if rent items
    STATUS BIT DEFAULT 1, -- 1 is active, 0 is inactive
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(ID) ON DELETE CASCADE
);

--DROP TABLE CARTS
-- Create table CARTS
CREATE TABLE CARTS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT NOT NULL UNIQUE,  -- No UNIQUE constraint
    FOREIGN KEY (USER_ID) REFERENCES USERS(ID)
);

--DROP TABLE CART_ITEMS;
-- Create table CART_ITEMS
CREATE TABLE CART_ITEMS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CART_ID INT NOT NULL,
    PRODUCT_ID INT NOT NULL,
    PRODUCT_DETAIL_ID INT NOT NULL,
	PRODUCT_DETAIL_NAME NVARCHAR(255) NOT NULL,
    PRODUCT_DETAIL_PRICE DECIMAL(18,2) NOT NULL,  -- Change FLOAT to DECIMAL
    QUANTITY INT NOT NULL,
    FOREIGN KEY (CART_ID) REFERENCES CARTS(ID),
    FOREIGN KEY (PRODUCT_ID) REFERENCES PRODUCTS(ID),
    FOREIGN KEY (PRODUCT_DETAIL_ID) REFERENCES PRODUCT_DETAILS(ID)
);

-- create table DISCOUNTS
CREATE TABLE DISCOUNTS (
	ID INT IDENTITY(1,1) PRIMARY KEY,
	USER_ID INT NOT NULL,
	DISCOUNT_VALUE FLOAT NOT NULL,
	EXPIRED_DATE DATETIME NULL,
	STATUS BIT,
	CONSTRAINT FK_Discount_Users FOREIGN KEY (USER_ID)
        REFERENCES Users(ID) ON DELETE CASCADE,
);
--DROP TABLE PURCHASE_HISTORY;
-- PurchaseHistory
CREATE TABLE PURCHASE_HISTORY (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT NOT NULL,
    CART_ID INT NOT NULL,
    PURCHASE_DATE DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PurchaseHistory_Users FOREIGN KEY (USER_ID) REFERENCES Users(ID) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseHistory_Carts FOREIGN KEY (CART_ID) REFERENCES CARTS(ID) ON DELETE CASCADE
);


--DROP TABLE PURCHASE_HISTORY_DETAILS;
-- PurchaseHistoryDetails
CREATE TABLE PURCHASE_HISTORY_DETAILS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    HISTORY_ID INT NOT NULL,
    PRODUCT_DETAIL_ID INT NOT NULL,
    PRICE_AT_PURCHASE FLOAT NOT NULL,
    IS_RENTED_FLG BIT NULL,
    IS_BOUGHT_FLG BIT NULL,
	DISCOUNT_ID INT NULL,
    CONSTRAINT FK_PurchaseHistoryDetails_PurchaseHistory FOREIGN KEY (HISTORY_ID) 
        REFERENCES PURCHASE_HISTORY(ID) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseHistoryDetails_ProductDetails FOREIGN KEY (PRODUCT_DETAIL_ID) 
        REFERENCES PRODUCT_DETAILS(ID) ON DELETE NO ACTION,
	CONSTRAINT FK_PurchaseHistoryDetails_Discounts FOREIGN KEY (DISCOUNT_ID) 
        REFERENCES DISCOUNTS(ID) ON DELETE NO ACTION,
);

-- create table RENT_IN_PROCESS
CREATE TABLE RENT_IN_PROCESS (
	ID INT IDENTITY(1,1),
	USER_ID INT NOT NULL,
	PRODUCT_DETAIL_ID INT NOT NULL,
	RENTED_DATE DATETIME NOT NULL,
	EXPIRED_DATE DATETIME NOT NULL, -- RENTED_DATE + DURATION
	RENTED_TYPE BIT DEFAULT 0 NOT NULL, -- 0 is rent by days, 1 is rent by months
	DURATION DATETIME, -- convert to days all time. 1 months dedault set = 30 days
	IS_EXTENDED BIT DEFAULT 0, -- Mark as if product in rented process is extended for more or not. Only if this flag and IS_EXPIRED = 1 then record will be in history table
	IS_EXPIRED BIT DEFAULT 0, -- this flag is set to 1 if and only if flag IS_EXTENDED = 0.
	CONSTRAINT FK_RentInProcess_ProductDetails FOREIGN KEY (PRODUCT_DETAIL_ID) 
        REFERENCES PRODUCT_DETAILS(ID)
);

--DROP TABLE RENT_HISTORY;
-- RENT_HISTORY table
CREATE TABLE RENT_HISTORY (
	ID INT IDENTITY(1,1) PRIMARY KEY,
	USER_ID INT NOT NULL,
	CONSTRAINT FK_RentHistory_Users FOREIGN KEY (USER_ID) 
        REFERENCES USERS(ID) ON DELETE CASCADE
);

--DROP TABLE RENT_HISTORY_DETAILS
--RENT_HISTORY_DETAILS
CREATE  TABLE RENT_HISTORY_DETAILS (
	ID INT IDENTITY(1,1) PRIMARY KEY,
	HISTORY_ID INT NOT NULL,
	PRODUCT_DETAIL_ID INT NOT NULL,
	DURATION DATETIME NOT NULL,
	RENTED_DATE DATETIME NOT NULL,
	EXPIRED_DATE DATETIME NOT NULL,
	PRICE FLOAT NOT NULL,
	DISCOUNT_ID INT NULL,
	CONSTRAINT FK_RentHistoryDetails_RentHistory FOREIGN KEY (HISTORY_ID) 
        REFERENCES RENT_HISTORY(ID) ON DELETE CASCADE,
    CONSTRAINT FK_RentHistoryDetails_ProductDetails FOREIGN KEY (PRODUCT_DETAIL_ID) 
        REFERENCES PRODUCT_DETAILS(ID) ON DELETE NO ACTION,
	CONSTRAINT FK_RentHistoryDetails_Discounts FOREIGN KEY (DISCOUNT_ID) 
        REFERENCES DISCOUNTS(ID) ON DELETE NO ACTION,
);

--DROP TABLE WALLET_HISTORY;
-- Create the WALLET_HISTORY table
CREATE TABLE WALLET_HISTORY (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT NOT NULL,
    CURRENT_BALANCE DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_WalletHistory_Users FOREIGN KEY (USER_ID)
        REFERENCES Users(ID) ON DELETE CASCADE,
    CONSTRAINT UQ_WalletHistory_UserId UNIQUE (USER_ID) -- Ensures one wallet history per user
);

--DROP TABLE WALLET_HISTORY_DETAILS;
-- Create the WALLET_HISTORY_DETAILS table
CREATE TABLE WALLET_HISTORY_DETAILS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    HISTORY_ID INT NOT NULL,
    HISTORY_TYPE NVARCHAR(3) NOT NULL CHECK (HISTORY_TYPE IN ('IN', 'OUT')),
    TIME_EXECUTION DATETIME NOT NULL DEFAULT GETDATE(),
    PRE_VALUE DECIMAL(18,2) NOT NULL,
    CHANGE_AMOUNT DECIMAL(18,2) NOT NULL,
    POST_VALUE DECIMAL(18,2) NOT NULL,
    DESCRIPTION NVARCHAR(255) NULL,
    PURCHASE_DETAIL_ID INT NULL, -- New foreign key
	RENT_DETAIL_ID INT NULL,
    CONSTRAINT FK_WALLET_HISTORY FOREIGN KEY (HISTORY_ID) 
        REFERENCES WALLET_HISTORY(ID) ON DELETE CASCADE,
    CONSTRAINT FK_PURCHASE_HISTORY_DETAILS FOREIGN KEY (PURCHASE_DETAIL_ID)
        REFERENCES PURCHASE_HISTORY_DETAILS(ID) ON DELETE NO ACTION, -- Changed to NO ACTION
	CONSTRAINT FK_RENT_HISTORY_DETAILS FOREIGN KEY (RENT_DETAIL_ID)
        REFERENCES RENT_HISTORY_DETAILS(ID) ON DELETE NO ACTION, -- Changed to NO ACTION
);


--Insert data to table categories
INSERT INTO CATEGORIES (CATEGORY_NAME, DESCRIPTION) VALUES
('Streaming Services', 'Netflix, YouTube Premium, Spotify, Disney+ accounts'),
('Mobile Cards', 'Top-up cards for mobile networks such as Verizon, AT&T, T-Mobile'),
('Gaming & Gift Cards', 'Google Play, App Store, Steam, PlayStation, Xbox gift cards'),
('Software & Licenses', 'Windows, Microsoft Office, Antivirus, VPN subscriptions'),
('E-books & Learning', 'Online courses, digital books, e-learning subscriptions');

-- Inser  into Products
INSERT INTO PRODUCTS (NAME, CATEGORY_ID, SOLD_NUMBER, CURRENT_AVAILABLE, TYPE, STATUS)
VALUES
('Netflix Account for limited time Subscription', 1, NULL, 100, 1, 1),
('YouTube Premium for rented demands', 1, NULL, 80, 1, 1),
('Spotify Family Plan in short period', 1, NULL, 50, 1, 1),
('Netflix Subscription For Owner (limit 1 user)', 1, NULL, 100, 1, 1),
('Netflix Subscription For Owner (public account)', 1, 0, 100, 0, 1),
('YouTube Premium', 1, 30, 80, 0, 1),
('Spotify Family Plan', 1, 25, 50, 0, 1),
('Verizon $50 Mobile Card', 2, 10, 10, 0, 1),
('Verizon $100 Mobile Card', 2, 40, 18, 0, 1),
('Google Play $25 Gift Card', 3, 35, 20, 0, 1),
('Google Play $55 Gift Card', 3, 40, 5, 0, 1),
('Google Play $100 Gift Card', 3, 25, 0, 0, 0),
('Windows 10 Pro License Key', 4, 25, 10, 0, 1),
('Windows 10 Pro License Key', 4, 20, 0, 0, 0),
('Windows 11 Pro License Key', 4, 10, 15, 0, 1),
('Windows 11 Pro License Key', 4, 20, 0, 0, 0),
('Microsoft Office 365 Annual Plan', 4, 15, 25, 0, 1),
('Microsoft Office 365 Annual Plan', 4, 15, 30, 0, 1),
('Udemy Online Course Standard Account - Digital Marketing', 5, 10, 60, 0, 1),
('Udemy Online Course Premium Account - Digital Marketing', 5, 15, 20, 0, 1),
('Udemy Online Course - Digital Marketing For Rented User', 5, NULL, 60, 1, 1);

-- Insert into Product details
INSERT INTO PRODUCT_DETAILS (PRODUCT_ID, PRODUCT_TYPE, PRICE, RELEASED_DATE, DETAIL_DESC, DETAIL_PRIVATE_DESC, IS_RENTED_FLG, IS_BOUGHT_FLG, STATUS)
VALUES
(1, 1, 10, NULL, 'Netflix Account for limited time Subscription. Limited-time Netflix subscription with access to premium content for sale.', 'Login details sent after purchase: Username: user1@gmail.com | Password: abc1234', 0, NULL, 1),
(2, 1, 15, NULL, 'YouTube Premium for rented demands. Get YouTube Premium subscription for a limited time rental.', 'Login details sent after purchase: Username: user2@gmail.com | Password: 122333', 0, NULL, 1),
(3, 1, 20, NULL, 'Spotify Family Plan in short period. Spotify family plan with up to 6 accounts available for sale for a limited time.', 'Account details for Spotify Family Plan: Username: user3@gmail.com | Password: mnbv1234', 0, NULL, 1),
(4, 1, 25, NULL, 'Netflix Subscription For Owner (limit 1 user). Personal Netflix account for sale, valid for a single user.', 'Login credentials: Username: user4@hotmail.com | Password: abcdxyzm', 0, NULL, 1),
(5, 0, 30, NULL, 'Netflix Subscription For Owner (public account). Public Netflix account for sale with shared access.', 'Credentials: Username: user5@outlook.com | Password: aioklp123', NULL, 0, 1),
(6, 0, 12, NULL, 'YouTube Premium. YouTube Premium account available for purchase.', 'Account login details: Username: user6@hotmail.com | Password: alohaki@1234', NULL, 0, 1),
(7, 0, 18, NULL, 'Spotify Family Plan. Spotify family plan subscription for sale, up to 6 accounts.', 'Login details: Username: user7@hotmail.com | Password: anoyam/123@a', NULL, 0, 1),
(8, 0, 50, NULL, 'Verizon $50 Mobile Card. Verizon prepaid mobile card for sale.', 'Code for activation: user8@fpt.com.vn | Password: MqZSp!@/', NULL, 0, 1),
(9, 0, 100, NULL, 'Verizon $100 Mobile Card. Prepaid Verizon mobile card with a value of $100.', 'Activation code sent upon purchase via email. Username: user8@devops.com.vn | Password: 7XiLs88', NULL, 0, 1),
(10, 0, 25, NULL, 'Google Play $25 Gift Card. Google Play store gift card worth $25 for sale.', 'The gift card code: ao$D32', NULL, 0, 1),
(11, 0, 25, NULL, 'Google Play $25 Gift Card. Google Play gift card available for sale.', 'Gift card code: 2cm)9d', NULL, 0, 1),
(12, 0, 25, NULL, 'Google Play $25 Gift Card. Google Play gift card with a value of $25 for sale.', 'Gift card code: 123mMjKL', NULL, 1, 0),
(13, 0, 150, NULL, 'Windows 11 Pro License Key. Windows 11 Pro key for sale.', 'License key: aoik-htmk-qweo-mngh', NULL, 0, 1),
(14, 0, 150, NULL, 'Windows 11 Pro License Key. Windows 11 Pro license key for sale.', 'License key: 12km-45jk-asifo-98cb', NULL, 1, 0),
(15, 0, 150, NULL, 'Windows 11 Pro License Key. Windows 11 Pro license for sale.', 'Key: moik-akco-sssf-9mkw',Null, 0, 1),
(16, 0, 150, NULL, 'Windows 11 Pro License Key. A new Windows 11 Pro key available for sale.', 'License key: adkj-awem-984n-5j2b', NULL, 1, 0),
(17, 0, 100, NULL, 'Microsoft Office 365 Annual Plan. Annual subscription for Microsoft Office 365 for sale.', 'Login credentials for Office 365: Account: user9@hust.com.vn | Password: 1mkoca', NULL, 0, 1),
(18, 0, 100, NULL, 'Microsoft Office 365 Annual Plan. Office 365 annual plan available for sale.', 'Office 365 login details: Account: user10@fpt.edu.vn | Password: 36asdR4s', NULL, 0, 1),
(19, 0, 100, NULL, 'Microsoft Office 365 Annual Plan. A complete Microsoft Office 365 annual subscription available for purchase.', 'Office 365 account: Username: user11@dma.com | Password: 1Dgm10', NULL, 0, 1),
(20, 0, 100, NULL, 'Microsoft Office 365 Annual Plan. Annual Microsoft Office 365 plan for sale.', 'Credentials: Account: user12@fsoft.com | Password: ac(0D4G', NULL, 0, 1),
(21, 1, 50, NULL,  'Udemy Online Course - Digital Marketing. Online course in Digital Marketing available for sale.', 'Course access details: Account: m2FDm45#./ | Password: 1jKL3$#', 0, NULL, 1),
(1, 1, 10, NULL,  'Udemy Online Course - Digital Marketing For Rented User. Rent this Udemy Digital Marketing course for a limited period.', 'Renting access details: Account: user13@fhn.com.vn | Password: 1A4FK3nj', 0, NULL, 1),
(2, 1, 12, NULL,  'YouTube Premium for rented demands. Rent YouTube Premium subscription for a period.', 'Login details for rental: Account: user14@gmail.com | Password: 9Aklm@1', 0, NULL, 1),
(3, 1, 20, NULL,  'Netflix Account for limited time Subscription. Rent Netflix account for a limited time subscription.', 'Login details sent upon successful rental: Account: user15@hotmail.vn | Password: 98fd@m(*', 0, NULL, 1);

-- Verify
SELECT * FROM PURCHASE_HISTORY WHERE USER_ID = 1;
SELECT phd.*, pd.DETAIL_DESC 
FROM PURCHASE_HISTORY_DETAILS phd
JOIN PRODUCT_DETAILS pd ON phd.PRODUCT_DETAIL_ID = pd.ID
WHERE phd.HISTORY_ID = 1;

SELECT wh.CURRENT_BALANCE, whd.*, phd.PRICE_AT_PURCHASE 
FROM WALLET_HISTORY wh
JOIN WALLET_HISTORY_DETAILS whd ON wh.ID = whd.HISTORY_ID
LEFT JOIN PURCHASE_HISTORY_DETAILS phd ON whd.PURCHASE_DETAIL_ID = phd.ID
WHERE wh.USER_ID = 1;

SELECT * FROM CARTS 
WHERE USER_ID = 5

SELECT * FROM CART_ITEMS
WHERE CART_ID = 5

SELECT * FROM USERS
WHERE USERS.ID = 5;

SELECT * FROM WALLET_HISTORY
WHERE USER_ID = 5

SELECT * FROM PURCHASE_HISTORY
WHERE USER_ID = 5

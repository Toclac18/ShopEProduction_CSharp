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

DROP TABLE CARTS
-- Create table CARTS
CREATE TABLE CARTS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT NOT NULL UNIQUE,  -- No UNIQUE constraint
    FOREIGN KEY (USER_ID) REFERENCES USERS(ID)
);

DROP TABLE CART_ITEMS;
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

DROP TABLE PURCHASE_HISTORY;
-- PurchaseHistory
CREATE TABLE PURCHASE_HISTORY (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT NOT NULL,
    CART_ID INT NOT NULL,
    PURCHASE_DATE DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_PurchaseHistory_Users FOREIGN KEY (USER_ID) REFERENCES Users(ID) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseHistory_Carts FOREIGN KEY (CART_ID) REFERENCES CARTS(ID) ON DELETE CASCADE
);


DROP TABLE PURCHASE_HISTORY_DETAILS;
-- PurchaseHistoryDetails
CREATE TABLE PURCHASE_HISTORY_DETAILS (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    HISTORY_ID INT NOT NULL,
    PRODUCT_DETAIL_ID INT NOT NULL,
    PRICE_AT_PURCHASE FLOAT NOT NULL,
    IS_RENTED_FLG BIT NULL,
    IS_BOUGHT_FLG BIT NULL,
    CONSTRAINT FK_PurchaseHistoryDetails_PurchaseHistory FOREIGN KEY (HISTORY_ID) 
        REFERENCES PURCHASE_HISTORY(ID) ON DELETE CASCADE,
    CONSTRAINT FK_PurchaseHistoryDetails_ProductDetails FOREIGN KEY (PRODUCT_DETAIL_ID) 
        REFERENCES PRODUCT_DETAILS(ID) ON DELETE NO ACTION
);

DROP TABLE WALLET_HISTORY;
-- Create the WALLET_HISTORY table
CREATE TABLE WALLET_HISTORY (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    USER_ID INT NOT NULL,
    CURRENT_BALANCE DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    CONSTRAINT FK_WalletHistory_Users FOREIGN KEY (USER_ID)
        REFERENCES Users(ID) ON DELETE CASCADE,
    CONSTRAINT UQ_WalletHistory_UserId UNIQUE (USER_ID) -- Ensures one wallet history per user
);

DROP TABLE WALLET_HISTORY_DETAILS;
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
    CONSTRAINT FK_WALLET_HISTORY FOREIGN KEY (HISTORY_ID) 
        REFERENCES WALLET_HISTORY(ID) ON DELETE CASCADE,
    CONSTRAINT FK_PURCHASE_HISTORY_DETAILS FOREIGN KEY (PURCHASE_DETAIL_ID)
        REFERENCES PURCHASE_HISTORY_DETAILS(ID) ON DELETE NO ACTION -- Changed to NO ACTION
);

--Insert data to table categories
INSERT INTO CATEGORIES (CATEGORY_NAME, DESCRIPTION) VALUES
('Streaming Services', 'Netflix, YouTube Premium, Spotify, Disney+ accounts'),
('Mobile Cards', 'Top-up cards for mobile networks such as Verizon, AT&T, T-Mobile'),
('Gaming & Gift Cards', 'Google Play, App Store, Steam, PlayStation, Xbox gift cards'),
('Software & Licenses', 'Windows, Microsoft Office, Antivirus, VPN subscriptions'),
('E-books & Learning', 'Online courses, digital books, e-learning subscriptions');

-- Insert into WalletHistory
INSERT INTO WALLET_HISTORY (USER_ID, CURRENT_BALANCE) VALUES (1, 50.00);

-- Insert into PurchaseHistory
INSERT INTO PURCHASE_HISTORY(USER_ID, CART_ID, PURCHASE_DATE)
VALUES (1, 1, '2025-03-23 14:30:00');

-- Insert into PurchaseHistoryDetails
INSERT INTO PURCHASE_HISTORY_DETAILS(HISTORY_ID, PRODUCT_DETAIL_ID, PRICE_AT_PURCHASE, IS_RENTED_FLG, IS_BOUGHT_FLG)
VALUES (1, 1, 25.99, 1, NULL);

-- Insert into WalletHistoryDetails with link to PurchaseHistoryDetails
INSERT INTO WALLET_HISTORY_DETAILS (HISTORY_ID, HISTORY_TYPE, TIME_EXECUTION, PRE_VALUE, CHANGE_AMOUNT, POST_VALUE, DESCRIPTION, PURCHASE_DETAIL_ID)
VALUES (1, 'OUT', '2025-03-23 14:30:00', 75.99, -25.99, 50.00, 'Purchase of Product A', 1);

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
WHERE USER_ID = 1

SELECT * FROM CART_ITEMS
WHERE CART_ID = 1
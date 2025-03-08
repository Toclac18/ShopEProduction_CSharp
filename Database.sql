
-- Xóa database hiện có nếu tồn tại
DROP DATABASE IF EXISTS ShopEProduction;
CREATE DATABASE ShopEProduction;
USE ShopEProduction;

-- Bảng Roles để lưu trữ chức năng của người dùng
CREATE TABLE ROLES (
						ID INT AUTO_INCREMENT PRIMARY KEY,
                       `ROLE_NAME` VARCHAR(50)
);
-- inser data
INSERT INTO `shopeproduction`.`roles` (`ROLE_NAME`)	VALUES('ADMIN'),('USER');

-- Tạo bảng Users
CREATE TABLE USERS (
	ID VARCHAR(20) PRIMARY KEY,
    USERNAME VARCHAR(50) NOT NULL UNIQUE,
    PASSWORD VARCHAR(255) NOT NULL,
    FULLNAME VARCHAR(100),
    USER_IMAGE VARCHAR(255),
    EMAIL VARCHAR(100) NOT NULL UNIQUE,
    PHONENUMBER VARCHAR(20),
    USER_CREATE_AT DATETIME DEFAULT CURRENT_TIMESTAMP, -- Change from TIMESTAMP to DATETIME
    `USER_POINT` INT,
    USER_ROLE_ID INT,
    `USER_STATUS` BOOLEAN,
    FOREIGN KEY (USER_ROLE_ID) REFERENCES ROLES(ID)
);

select * from users;

-- insert data
-- INSERT INTO Users (USERNAME, PASSWORD, FULLNAME, USER_IMAGE, EMAIL, PHONENUMBER, USER_CREATE_AT, `USER_POINT`, USER_ROLE_ID, `USER_STATUS`)
-- VALUES
--     ('quan', '123456', 'Nguyen Tat Quan','local_image_quan', 'tatquan1803@gmail.com', '0837931504','2024-02-15 14:30:00', 10,1,1),
--     ('quannt', '123456', 'Nguyen Tat Quan','local_image_quannt', 'quannthe187203@fpt.edu.vn', '0829505619','2025-02-19 4:30:00', 2,2,1),
--     ('tatquan', '123456', 'Nguyen Tat Quan','local_image_tatquan', 'tatquan7203@outlook.com', '0837938956', NOW(), 2,1,1);
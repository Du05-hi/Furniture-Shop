CREATE DATABASE DuongChiDu_QLNoiThat;
GO

USE DuongChiDu_QLNoiThat;
GO

CREATE TABLE quanlynoithat (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    Description NVARCHAR(255),
    ImageUrl NVARCHAR(255),
    Category NVARCHAR(50)
);

-- Dữ liệu mẫu
INSERT INTO quanlynoithat (Name, Price, Description, ImageUrl, Category)
VALUES 
(N'Sofa gỗ', 5000000, N'Sofa gỗ cao cấp 3 chỗ ngồi', 'sofa.jpg', N'Sofa'),
(N'Bàn ăn 6 ghế', 7000000, N'Bàn ăn gỗ tự nhiên kèm 6 ghế', 'banan.jpg', N'Bàn'),
(N'Tủ quần áo 3 cánh', 4500000, N'Tủ quần áo gỗ công nghiệp', 'tuquanao.jpg', N'Tủ');

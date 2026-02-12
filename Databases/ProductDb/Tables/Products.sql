-- Create Products table
CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(256) NOT NULL,
    Description NVARCHAR(2000),
    Price DECIMAL(18,2) NOT NULL,
    StockQuantity INT NOT NULL,
    CategoryId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Create Categories table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(256) NOT NULL,
    Description NVARCHAR(2000),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create indexes
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_Price ON Products(Price);
CREATE INDEX IX_Products_IsActive ON Products(IsActive);
CREATE INDEX IX_Categories_IsActive ON Categories(IsActive);

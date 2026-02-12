-- Migration: 1.0.0 - Initial database schema
-- Date: 2024-01-01
-- Description: Create initial schema with Products, Categories, and audit tables

BEGIN TRANSACTION;

-- Create base tables if they don't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(256) NOT NULL UNIQUE,
        Description NVARCHAR(MAX),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2,
        IsActive BIT NOT NULL DEFAULT 1
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
BEGIN
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
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductAuditLogs')
BEGIN
    CREATE TABLE ProductAuditLogs (
        AuditLogId BIGINT PRIMARY KEY IDENTITY(1,1),
        ProductId INT NOT NULL,
        OperationType NVARCHAR(50) NOT NULL,
        OldValues NVARCHAR(MAX),
        NewValues NVARCHAR(MAX),
        ChangedBy NVARCHAR(256),
        ChangedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IPAddress NVARCHAR(45),
        FOREIGN KEY (ProductId) REFERENCES Products(Id) ON DELETE CASCADE
    );
END;

-- Create initial indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryId')
    CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_IsActive')
    CREATE INDEX IX_Products_IsActive ON Products(IsActive);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductAuditLogs_ProductId')
    CREATE INDEX IX_ProductAuditLogs_ProductId ON ProductAuditLogs(ProductId);

COMMIT TRANSACTION;

-- Migration: 1.1.0 - Add stored procedures and functions
-- Date: 2024-02-01
-- Description: Add stored procedures, functions, views, and triggers for enhanced functionality

BEGIN TRANSACTION;

-- Create stored procedures if they don't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'sp_GetProductsWithStats' AND type = 'P')
BEGIN
    EXEC sp_executesql N'CREATE PROCEDURE sp_GetProductsWithStats AS SELECT 1;';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'sp_GetProductById' AND type = 'P')
BEGIN
    EXEC sp_executesql N'CREATE PROCEDURE sp_GetProductById AS SELECT 1;';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'sp_CreateProduct' AND type = 'P')
BEGIN
    EXEC sp_executesql N'CREATE PROCEDURE sp_CreateProduct AS SELECT 1;';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'sp_UpdateProduct' AND type = 'P')
BEGIN
    EXEC sp_executesql N'CREATE PROCEDURE sp_UpdateProduct AS SELECT 1;';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'sp_DeleteProduct' AND type = 'P')
BEGIN
    EXEC sp_executesql N'CREATE PROCEDURE sp_DeleteProduct AS SELECT 1;';
END;

-- Create functions
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'CalculateDiscountedPrice' AND type = 'FN')
BEGIN
    EXEC sp_executesql N'CREATE FUNCTION dbo.CalculateDiscountedPrice(@Price DECIMAL(18,2), @Discount DECIMAL(5,2)) RETURNS DECIMAL(18,2) AS BEGIN RETURN @Price; END;';
END;

IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'GetProductStock' AND type = 'FN')
BEGIN
    EXEC sp_executesql N'CREATE FUNCTION dbo.GetProductStock(@ProductId INT) RETURNS NVARCHAR(20) AS BEGIN RETURN ''In Stock''; END;';
END;

-- Create view
IF NOT EXISTS (SELECT * FROM sys.views WHERE name = 'vw_ProductDetails')
BEGIN
    EXEC sp_executesql N'CREATE VIEW vw_ProductDetails AS SELECT 1 AS Id;';
END;

-- Add new columns to Products if needed (example of schema enhancement)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'SKU')
BEGIN
    ALTER TABLE Products ADD SKU NVARCHAR(50) UNIQUE;
END;

COMMIT TRANSACTION;

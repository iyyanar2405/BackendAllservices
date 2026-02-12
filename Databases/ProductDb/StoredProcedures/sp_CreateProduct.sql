-- Stored Procedure: Create product
CREATE PROCEDURE sp_CreateProduct
    @Name NVARCHAR(256),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(18,2),
    @StockQuantity INT,
    @CategoryId INT,
    @IsActive BIT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        INSERT INTO Products (Name, Description, Price, StockQuantity, CategoryId, CreatedAt, IsActive)
        VALUES (@Name, @Description, @Price, @StockQuantity, @CategoryId, GETUTCDATE(), @IsActive);
        
        DECLARE @ProductId INT = SCOPE_IDENTITY();
        
        -- Audit log
        INSERT INTO ProductAuditLogs (ProductId, OperationType, NewValues, ChangedBy, ChangedAt)
        VALUES (@ProductId, 'INSERT', 
            CONCAT('Name: ', @Name, ', Price: ', @Price, ', Stock: ', @StockQuantity),
            SYSTEM_USER, GETUTCDATE());
        
        COMMIT TRANSACTION;
        
        SELECT @ProductId AS ProductId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

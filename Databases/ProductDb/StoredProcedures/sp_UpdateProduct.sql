-- Stored Procedure: Update product
CREATE PROCEDURE sp_UpdateProduct
    @ProductId INT,
    @Name NVARCHAR(256),
    @Description NVARCHAR(MAX),
    @Price DECIMAL(18,2),
    @StockQuantity INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @OldValues NVARCHAR(MAX);
        
        -- Get old values for audit
        SELECT @OldValues = CONCAT('Name: ', Name, ', Price: ', Price, ', Stock: ', StockQuantity)
        FROM Products
        WHERE Id = @ProductId;
        
        UPDATE Products
        SET 
            Name = @Name,
            Description = @Description,
            Price = @Price,
            StockQuantity = @StockQuantity,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @ProductId;
        
        -- Audit log
        INSERT INTO ProductAuditLogs (ProductId, OperationType, OldValues, NewValues, ChangedBy, ChangedAt)
        VALUES (@ProductId, 'UPDATE', @OldValues,
            CONCAT('Name: ', @Name, ', Price: ', @Price, ', Stock: ', @StockQuantity),
            SYSTEM_USER, GETUTCDATE());
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

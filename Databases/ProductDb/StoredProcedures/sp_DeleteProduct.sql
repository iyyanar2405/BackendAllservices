-- Stored Procedure: Delete (soft delete) product
CREATE PROCEDURE sp_DeleteProduct
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        DECLARE @ProductName NVARCHAR(256);
        SELECT @ProductName = Name FROM Products WHERE Id = @ProductId;
        
        UPDATE Products
        SET 
            IsActive = 0,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @ProductId;
        
        -- Audit log
        INSERT INTO ProductAuditLogs (ProductId, OperationType, NewValues, ChangedBy, ChangedAt)
        VALUES (@ProductId, 'DELETE', CONCAT('Product: ', @ProductName, ' - Soft Deleted'),
            SYSTEM_USER, GETUTCDATE());
        
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;

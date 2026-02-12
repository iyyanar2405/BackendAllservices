-- Trigger: Audit Product Changes
CREATE TRIGGER tr_Products_Audit
ON Products
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (SELECT * FROM inserted)
    BEGIN
        IF EXISTS (SELECT * FROM deleted)
        BEGIN
            -- UPDATE operation
            INSERT INTO ProductAuditLogs (ProductId, OperationType, ChangedAt)
            SELECT i.Id, 'UPDATE', GETUTCDATE()
            FROM inserted i;
        END
        ELSE
        BEGIN
            -- INSERT operation
            INSERT INTO ProductAuditLogs (ProductId, OperationType, ChangedAt)
            SELECT i.Id, 'INSERT', GETUTCDATE()
            FROM inserted i;
        END
    END
    ELSE
    BEGIN
        -- DELETE operation
        INSERT INTO ProductAuditLogs (ProductId, OperationType, ChangedAt)
        SELECT d.Id, 'DELETE', GETUTCDATE()
        FROM deleted d;
    END
END;

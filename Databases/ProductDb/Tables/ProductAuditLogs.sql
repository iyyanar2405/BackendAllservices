-- Create ProductAuditLogs table for tracking changes
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

-- Create indexes for audit logs
CREATE INDEX IX_ProductAuditLogs_ProductId ON ProductAuditLogs(ProductId);
CREATE INDEX IX_ProductAuditLogs_ChangedAt ON ProductAuditLogs(ChangedAt DESC);
CREATE INDEX IX_ProductAuditLogs_OperationType ON ProductAuditLogs(OperationType);

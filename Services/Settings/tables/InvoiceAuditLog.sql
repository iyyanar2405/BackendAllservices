-- InvoiceAuditLog table for tracking invoice changes
CREATE TABLE [dbo].[InvoiceAuditLog]
(
    [InvoiceAuditLogId] INT IDENTITY(1,1) NOT NULL,
    [InvoiceId] INT NOT NULL,
    [InvoiceNumber] NVARCHAR(50) NOT NULL,
    [Action] NVARCHAR(50) NOT NULL, -- 'Created', 'Updated', 'Payment Planned', 'Paid', 'Cancelled'
    [OldValue] NVARCHAR(MAX) NULL, -- JSON of old values
    [NewValue] NVARCHAR(MAX) NULL, -- JSON of new values
    [ChangedFields] NVARCHAR(500) NULL, -- Comma-separated list of changed fields
    [Reason] NVARCHAR(500) NULL,
    [ActionDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ActionBy] INT NOT NULL,
    [IPAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,

    CONSTRAINT [PK_InvoiceAuditLog] PRIMARY KEY CLUSTERED ([InvoiceAuditLogId]),
    CONSTRAINT [FK_InvoiceAuditLog_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [Invoices]([InvoiceId]),
    CONSTRAINT [FK_InvoiceAuditLog_ActionBy] FOREIGN KEY ([ActionBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_InvoiceAuditLog_InvoiceId] ON [dbo].[InvoiceAuditLog] ([InvoiceId]);
CREATE NONCLUSTERED INDEX [IX_InvoiceAuditLog_ActionDate] ON [dbo].[InvoiceAuditLog] ([ActionDate]);
CREATE NONCLUSTERED INDEX [IX_InvoiceAuditLog_ActionBy] ON [dbo].[InvoiceAuditLog] ([ActionBy]);
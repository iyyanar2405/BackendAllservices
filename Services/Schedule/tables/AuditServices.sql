-- AuditServices table for many-to-many relationship between audits and services
CREATE TABLE [dbo].[AuditServices]
(
    [AuditServiceId] INT IDENTITY(1,1) NOT NULL,
    [AuditId] INT NOT NULL,
    [ServiceId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'active', -- 'active', 'completed', 'cancelled'
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [Notes] NVARCHAR(1000) NULL,

    CONSTRAINT [PK_AuditServices] PRIMARY KEY CLUSTERED ([AuditServiceId]),
    CONSTRAINT [UK_AuditServices_Audit_Service] UNIQUE ([AuditId], [ServiceId]),
    CONSTRAINT [FK_AuditServices_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits]([AuditId]),
    CONSTRAINT [FK_AuditServices_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_AuditServices_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditServices_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditServices_AuditId] ON [dbo].[AuditServices] ([AuditId]);
CREATE NONCLUSTERED INDEX [IX_AuditServices_ServiceId] ON [dbo].[AuditServices] ([ServiceId]);
CREATE NONCLUSTERED INDEX [IX_AuditServices_IsActive] ON [dbo].[AuditServices] ([IsActive]);
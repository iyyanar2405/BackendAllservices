-- AuditSiteServices table for services provided during audit site visits
CREATE TABLE [dbo].[AuditSiteServices]
(
    [AuditSiteServiceId] INT IDENTITY(1,1) NOT NULL,
    [AuditSiteAuditId] INT NOT NULL,
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
    [Cost] DECIMAL(10,2) NULL,
    [Currency] NVARCHAR(3) NULL DEFAULT 'USD',

    CONSTRAINT [PK_AuditSiteServices] PRIMARY KEY CLUSTERED ([AuditSiteServiceId]),
    CONSTRAINT [UK_AuditSiteServices_AuditSite_Service] UNIQUE ([AuditSiteAuditId], [ServiceId]),
    CONSTRAINT [FK_AuditSiteServices_AuditSiteAuditId] FOREIGN KEY ([AuditSiteAuditId]) REFERENCES [AuditSiteAudits]([AuditSiteAuditId]),
    CONSTRAINT [FK_AuditSiteServices_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_AuditSiteServices_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditSiteServices_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditSiteServices_AuditSiteAuditId] ON [dbo].[AuditSiteServices] ([AuditSiteAuditId]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteServices_ServiceId] ON [dbo].[AuditSiteServices] ([ServiceId]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteServices_IsActive] ON [dbo].[AuditSiteServices] ([IsActive]);
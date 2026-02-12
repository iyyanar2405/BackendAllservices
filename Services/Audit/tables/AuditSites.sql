-- AuditSites table for many-to-many relationship between audits and sites
CREATE TABLE [dbo].[AuditSites]
(
    [AuditSiteId] INT IDENTITY(1,1) NOT NULL,
    [AuditId] INT NOT NULL,
    [SiteId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'active', -- 'active', 'completed', 'cancelled'
    [ScheduledDate] DATETIME NULL,
    [CompletedDate] DATETIME NULL,
    [Notes] NVARCHAR(1000) NULL,

    CONSTRAINT [PK_AuditSites] PRIMARY KEY CLUSTERED ([AuditSiteId]),
    CONSTRAINT [UK_AuditSites_Audit_Site] UNIQUE ([AuditId], [SiteId]),
    CONSTRAINT [FK_AuditSites_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits]([AuditId]),
    CONSTRAINT [FK_AuditSites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_AuditSites_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditSites_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditSites_AuditId] ON [dbo].[AuditSites] ([AuditId]);
CREATE NONCLUSTERED INDEX [IX_AuditSites_SiteId] ON [dbo].[AuditSites] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_AuditSites_IsActive] ON [dbo].[AuditSites] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_AuditSites_ScheduledDate] ON [dbo].[AuditSites] ([ScheduledDate]);
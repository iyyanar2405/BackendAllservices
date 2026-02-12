-- AuditSiteAudits table for specific audit instances at sites
CREATE TABLE [dbo].[AuditSiteAudits]
(
    [AuditSiteAuditId] INT IDENTITY(1,1) NOT NULL,
    [AuditId] INT NOT NULL,
    [SiteId] INT NOT NULL,
    [AuditTypeId] INT NOT NULL,
    [AuditNumber] NVARCHAR(50) NOT NULL,
    [ScheduledDate] DATETIME NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [CompletedDate] DATETIME NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'scheduled', -- 'scheduled', 'in-progress', 'completed', 'cancelled'
    [LeadAuditorId] INT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Notes] NVARCHAR(1000) NULL,
    [ReportPath] NVARCHAR(500) NULL,
    [CertificateIssued] BIT NOT NULL DEFAULT 0,
    [CertificateNumber] NVARCHAR(100) NULL,

    CONSTRAINT [PK_AuditSiteAudits] PRIMARY KEY CLUSTERED ([AuditSiteAuditId]),
    CONSTRAINT [UK_AuditSiteAudits_AuditNumber] UNIQUE ([AuditNumber]),
    CONSTRAINT [FK_AuditSiteAudits_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits]([AuditId]),
    CONSTRAINT [FK_AuditSiteAudits_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_AuditSiteAudits_AuditTypeId] FOREIGN KEY ([AuditTypeId]) REFERENCES [AuditTypes]([AuditTypeId]),
    CONSTRAINT [FK_AuditSiteAudits_LeadAuditorId] FOREIGN KEY ([LeadAuditorId]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditSiteAudits_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditSiteAudits_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditSiteAudits_AuditId] ON [dbo].[AuditSiteAudits] ([AuditId]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteAudits_SiteId] ON [dbo].[AuditSiteAudits] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteAudits_Status] ON [dbo].[AuditSiteAudits] ([Status]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteAudits_ScheduledDate] ON [dbo].[AuditSiteAudits] ([ScheduledDate]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteAudits_LeadAuditorId] ON [dbo].[AuditSiteAudits] ([LeadAuditorId]);
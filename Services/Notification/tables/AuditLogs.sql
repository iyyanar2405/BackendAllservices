-- AuditLogs table for user action auditing
CREATE TABLE [dbo].[AuditLogs]
(
    [AuditLogId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NULL,
    [UserName] NVARCHAR(100) NULL, -- Stored for historical purposes even if user is deleted
    [Action] NVARCHAR(100) NOT NULL, -- 'Create', 'Read', 'Update', 'Delete', 'Login', 'Logout', 'Export', etc.
    [EntityType] NVARCHAR(100) NULL, -- 'User', 'Company', 'Site', 'Audit', 'Finding', 'Certificate', etc.
    [EntityId] INT NULL,
    [EntityName] NVARCHAR(200) NULL,
    [OldValues] NVARCHAR(MAX) NULL, -- JSON of old values for updates
    [NewValues] NVARCHAR(MAX) NULL, -- JSON of new values for creates/updates
    [ChangedFields] NVARCHAR(500) NULL, -- Comma-separated list of changed fields
    [ActionDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IPAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [SessionId] NVARCHAR(100) NULL,
    [RequestUrl] NVARCHAR(500) NULL,
    [RequestMethod] NVARCHAR(10) NULL, -- GET, POST, PUT, DELETE
    [Reason] NVARCHAR(500) NULL, -- Business reason for the action
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Success', -- 'Success', 'Failed', 'Unauthorized'
    [Duration] INT NULL, -- Action duration in milliseconds
    [ApplicationName] NVARCHAR(100) NULL DEFAULT 'CustomerPortal',
    [Environment] NVARCHAR(50) NULL DEFAULT 'Production',
    [CorrelationId] NVARCHAR(100) NULL, -- For tracking related actions
    [AdditionalData] NVARCHAR(MAX) NULL, -- JSON for additional context
    [CompanyId] INT NULL, -- Company context for the action
    [SiteId] INT NULL, -- Site context for the action

    CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED ([AuditLogId]),
    CONSTRAINT [FK_AuditLogs_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditLogs_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_AuditLogs_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [CK_AuditLogs_Status] CHECK ([Status] IN ('Success', 'Failed', 'Unauthorized'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditLogs_UserId] ON [dbo].[AuditLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_ActionDate] ON [dbo].[AuditLogs] ([ActionDate]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_Action] ON [dbo].[AuditLogs] ([Action]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityType] ON [dbo].[AuditLogs] ([EntityType]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_EntityId] ON [dbo].[AuditLogs] ([EntityId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_CompanyId] ON [dbo].[AuditLogs] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_SiteId] ON [dbo].[AuditLogs] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_Status] ON [dbo].[AuditLogs] ([Status]);
CREATE NONCLUSTERED INDEX [IX_AuditLogs_CorrelationId] ON [dbo].[AuditLogs] ([CorrelationId]);
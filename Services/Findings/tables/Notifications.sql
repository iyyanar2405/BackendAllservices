-- Notifications table for system notifications
CREATE TABLE [dbo].[Notifications]
(
    [NotificationId] INT IDENTITY(1,1) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [CategoryId] INT NOT NULL,
    [CompanyId] INT NULL,
    [SiteId] INT NULL,
    [ServiceId] INT NULL,
    [Priority] NVARCHAR(50) NOT NULL DEFAULT 'Medium', -- 'Low', 'Medium', 'High', 'Critical'
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- 'Active', 'Read', 'Archived', 'Dismissed'
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [ExpiryDate] DATETIME NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [ReadBy] NVARCHAR(MAX) NULL, -- JSON array of user IDs who have read this notification
    [TargetAudience] NVARCHAR(100) NULL, -- 'All', 'Company', 'Site', 'Role'
    [ActionRequired] BIT NOT NULL DEFAULT 0,
    [ActionUrl] NVARCHAR(500) NULL,
    [AttachmentPath] NVARCHAR(500) NULL,
    [RelatedEntityType] NVARCHAR(50) NULL, -- 'Audit', 'Finding', 'Certificate', 'Invoice'
    [RelatedEntityId] INT NULL,

    CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([NotificationId]),
    CONSTRAINT [FK_Notifications_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [NotificationCategories]([CategoryId]),
    CONSTRAINT [FK_Notifications_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_Notifications_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_Notifications_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_Notifications_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Notifications_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_Notifications_Priority] CHECK ([Priority] IN ('Low', 'Medium', 'High', 'Critical')),
    CONSTRAINT [CK_Notifications_Status] CHECK ([Status] IN ('Active', 'Read', 'Archived', 'Dismissed'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Notifications_CategoryId] ON [dbo].[Notifications] ([CategoryId]);
CREATE NONCLUSTERED INDEX [IX_Notifications_CompanyId] ON [dbo].[Notifications] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_Notifications_SiteId] ON [dbo].[Notifications] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Notifications_Priority] ON [dbo].[Notifications] ([Priority]);
CREATE NONCLUSTERED INDEX [IX_Notifications_Status] ON [dbo].[Notifications] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Notifications_CreatedDate] ON [dbo].[Notifications] ([CreatedDate]);
CREATE NONCLUSTERED INDEX [IX_Notifications_IsActive] ON [dbo].[Notifications] ([IsActive]);
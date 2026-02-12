-- UserSiteAccess table for user access control to sites
CREATE TABLE [dbo].[UserSiteAccess]
(
    [UserSiteAccessId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [SiteId] INT NOT NULL,
    [AccessLevel] NVARCHAR(50) NOT NULL DEFAULT 'read', -- 'read', 'write', 'admin'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [GrantedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [GrantedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserSiteAccess] PRIMARY KEY CLUSTERED ([UserSiteAccessId]),
    CONSTRAINT [UK_UserSiteAccess_User_Site] UNIQUE ([UserId], [SiteId]),
    CONSTRAINT [FK_UserSiteAccess_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserSiteAccess_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_UserSiteAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserSiteAccess_AccessLevel] CHECK ([AccessLevel] IN ('read', 'write', 'admin'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserSiteAccess_UserId] ON [dbo].[UserSiteAccess] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserSiteAccess_SiteId] ON [dbo].[UserSiteAccess] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_UserSiteAccess_IsActive] ON [dbo].[UserSiteAccess] ([IsActive]);
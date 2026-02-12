-- UserNotificationAccess table for user access control to notifications
CREATE TABLE [dbo].[UserNotificationAccess]
(
    [UserNotificationAccessId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [NotificationId] INT NOT NULL,
    [AccessLevel] NVARCHAR(50) NOT NULL DEFAULT 'read', -- 'read', 'write', 'admin'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [GrantedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [GrantedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserNotificationAccess] PRIMARY KEY CLUSTERED ([UserNotificationAccessId]),
    CONSTRAINT [UK_UserNotificationAccess_User_Notification] UNIQUE ([UserId], [NotificationId]),
    CONSTRAINT [FK_UserNotificationAccess_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserNotificationAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserNotificationAccess_AccessLevel] CHECK ([AccessLevel] IN ('read', 'write', 'admin'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserNotificationAccess_UserId] ON [dbo].[UserNotificationAccess] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserNotificationAccess_NotificationId] ON [dbo].[UserNotificationAccess] ([NotificationId]);
CREATE NONCLUSTERED INDEX [IX_UserNotificationAccess_IsActive] ON [dbo].[UserNotificationAccess] ([IsActive]);
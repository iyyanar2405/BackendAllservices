-- UserServiceAccess table for user access control to services
CREATE TABLE [dbo].[UserServiceAccess]
(
    [UserServiceAccessId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [ServiceId] INT NOT NULL,
    [AccessLevel] NVARCHAR(50) NOT NULL DEFAULT 'read', -- 'read', 'write', 'admin'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [GrantedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [GrantedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserServiceAccess] PRIMARY KEY CLUSTERED ([UserServiceAccessId]),
    CONSTRAINT [UK_UserServiceAccess_User_Service] UNIQUE ([UserId], [ServiceId]),
    CONSTRAINT [FK_UserServiceAccess_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserServiceAccess_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_UserServiceAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserServiceAccess_AccessLevel] CHECK ([AccessLevel] IN ('read', 'write', 'admin'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserServiceAccess_UserId] ON [dbo].[UserServiceAccess] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserServiceAccess_ServiceId] ON [dbo].[UserServiceAccess] ([ServiceId]);
CREATE NONCLUSTERED INDEX [IX_UserServiceAccess_IsActive] ON [dbo].[UserServiceAccess] ([IsActive]);
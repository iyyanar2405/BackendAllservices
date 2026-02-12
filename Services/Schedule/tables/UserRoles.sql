-- UserRoles table for many-to-many relationship between users and roles
CREATE TABLE [dbo].[UserRoles]
(
    [UserRoleId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [RoleId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [AssignedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserRoleId]),
    CONSTRAINT [UK_UserRoles_User_Role] UNIQUE ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([RoleId]),
    CONSTRAINT [FK_UserRoles_AssignedBy] FOREIGN KEY ([AssignedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserRoles_UserId] ON [dbo].[UserRoles] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);
CREATE NONCLUSTERED INDEX [IX_UserRoles_IsActive] ON [dbo].[UserRoles] ([IsActive]);
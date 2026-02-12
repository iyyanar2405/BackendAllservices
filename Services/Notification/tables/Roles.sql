-- Roles table for user roles and permissions
CREATE TABLE [dbo].[Roles]
(
    [RoleId] INT IDENTITY(1,1) NOT NULL,
    [RoleName] NVARCHAR(100) NOT NULL,
    [RoleCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [IsSystemRole] BIT NOT NULL DEFAULT 0, -- System roles cannot be deleted
    [Permissions] NVARCHAR(MAX) NULL, -- JSON string of permissions

    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([RoleId]),
    CONSTRAINT [UK_Roles_RoleName] UNIQUE ([RoleName]),
    CONSTRAINT [UK_Roles_RoleCode] UNIQUE ([RoleCode]),
    CONSTRAINT [FK_Roles_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Roles_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Roles_RoleName] ON [dbo].[Roles] ([RoleName]);
CREATE NONCLUSTERED INDEX [IX_Roles_IsActive] ON [dbo].[Roles] ([IsActive]);
-- UserCompanyAccess table for user access control to companies
CREATE TABLE [dbo].[UserCompanyAccess]
(
    [UserCompanyAccessId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [CompanyId] INT NOT NULL,
    [AccessLevel] NVARCHAR(50) NOT NULL DEFAULT 'READ', -- 'read', 'write', 'admin'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [GrantedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [GrantedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserCompanyAccess] PRIMARY KEY CLUSTERED ([UserCompanyAccessId]),
    CONSTRAINT [UK_UserCompanyAccess_User_Company] UNIQUE ([UserId], [CompanyId]),
    CONSTRAINT [FK_UserCompanyAccess_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserCompanyAccess_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_UserCompanyAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserCompanyAccess_AccessLevel] CHECK ([AccessLevel] IN ('read', 'write', 'admin'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserCompanyAccess_UserId] ON [dbo].[UserCompanyAccess] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserCompanyAccess_CompanyId] ON [dbo].[UserCompanyAccess] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_UserCompanyAccess_IsActive] ON [dbo].[UserCompanyAccess] ([IsActive]);
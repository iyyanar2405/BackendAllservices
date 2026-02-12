-- UserCountryAccess table for user access control to countries
CREATE TABLE [dbo].[UserCountryAccess]
(
    [UserCountryAccessId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [CountryId] INT NOT NULL,
    [AccessLevel] NVARCHAR(50) NOT NULL DEFAULT 'read', -- 'read', 'write', 'admin'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [GrantedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [GrantedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserCountryAccess] PRIMARY KEY CLUSTERED ([UserCountryAccessId]),
    CONSTRAINT [UK_UserCountryAccess_User_Country] UNIQUE ([UserId], [CountryId]),
    CONSTRAINT [FK_UserCountryAccess_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserCountryAccess_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Countries]([CountryId]),
    CONSTRAINT [FK_UserCountryAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserCountryAccess_AccessLevel] CHECK ([AccessLevel] IN ('read', 'write', 'admin'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserCountryAccess_UserId] ON [dbo].[UserCountryAccess] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserCountryAccess_CountryId] ON [dbo].[UserCountryAccess] ([CountryId]);
CREATE NONCLUSTERED INDEX [IX_UserCountryAccess_IsActive] ON [dbo].[UserCountryAccess] ([IsActive]);
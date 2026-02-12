-- UserCityAccess table for user access control to cities
CREATE TABLE [dbo].[UserCityAccess]
(
    [UserCityAccessId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [CityId] INT NOT NULL,
    [AccessLevel] NVARCHAR(50) NOT NULL DEFAULT 'read', -- 'read', 'write', 'admin'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [GrantedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiryDate] DATETIME NULL,
    [GrantedBy] INT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),

    CONSTRAINT [PK_UserCityAccess] PRIMARY KEY CLUSTERED ([UserCityAccessId]),
    CONSTRAINT [UK_UserCityAccess_User_City] UNIQUE ([UserId], [CityId]),
    CONSTRAINT [FK_UserCityAccess_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserCityAccess_CityId] FOREIGN KEY ([CityId]) REFERENCES [Cities]([CityId]),
    CONSTRAINT [FK_UserCityAccess_GrantedBy] FOREIGN KEY ([GrantedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserCityAccess_AccessLevel] CHECK ([AccessLevel] IN ('read', 'write', 'admin'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserCityAccess_UserId] ON [dbo].[UserCityAccess] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserCityAccess_CityId] ON [dbo].[UserCityAccess] ([CityId]);
CREATE NONCLUSTERED INDEX [IX_UserCityAccess_IsActive] ON [dbo].[UserCityAccess] ([IsActive]);
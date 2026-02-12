-- UserPreferences table for user preferences and settings
CREATE TABLE [dbo].[UserPreferences]
(
    [UserPreferenceId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [PreferenceKey] NVARCHAR(100) NOT NULL,
    [PreferenceValue] NVARCHAR(MAX) NULL,
    [PreferenceType] NVARCHAR(50) NOT NULL DEFAULT 'String', -- 'String', 'Number', 'Boolean', 'JSON'
    [Category] NVARCHAR(50) NULL, -- 'UI', 'Notification', 'Security', 'General'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,

    CONSTRAINT [PK_UserPreferences] PRIMARY KEY CLUSTERED ([UserPreferenceId]),
    CONSTRAINT [UK_UserPreferences_User_Key] UNIQUE ([UserId], [PreferenceKey]),
    CONSTRAINT [FK_UserPreferences_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserPreferences_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_UserPreferences_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserPreferences_UserId] ON [dbo].[UserPreferences] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserPreferences_PreferenceKey] ON [dbo].[UserPreferences] ([PreferenceKey]);
CREATE NONCLUSTERED INDEX [IX_UserPreferences_Category] ON [dbo].[UserPreferences] ([Category]);
CREATE NONCLUSTERED INDEX [IX_UserPreferences_IsActive] ON [dbo].[UserPreferences] ([IsActive]);
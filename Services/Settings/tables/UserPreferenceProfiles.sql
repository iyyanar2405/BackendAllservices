-- UserPreferenceProfiles table for storing user preference JSON profiles
CREATE TABLE [dbo].[UserPreferenceProfiles]
(
    [UserPreferenceProfileId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [PreferencesJson] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,

    CONSTRAINT [PK_UserPreferenceProfiles] PRIMARY KEY CLUSTERED ([UserPreferenceProfileId]),
    CONSTRAINT [FK_UserPreferenceProfiles_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserPreferenceProfiles_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([UserId])
);

CREATE NONCLUSTERED INDEX [IX_UserPreferenceProfiles_UserId] ON [dbo].[UserPreferenceProfiles] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserPreferenceProfiles_ModifiedDate] ON [dbo].[UserPreferenceProfiles] ([ModifiedDate]);

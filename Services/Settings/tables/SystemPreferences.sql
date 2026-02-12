-- SystemPreferences table for global system settings
CREATE TABLE [dbo].[SystemPreferences]
(
    [SystemPreferenceId] INT IDENTITY(1,1) NOT NULL,
    [PreferencesJson] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,

    CONSTRAINT [PK_SystemPreferences] PRIMARY KEY CLUSTERED ([SystemPreferenceId]),
    CONSTRAINT [FK_SystemPreferences_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([UserId])
);

CREATE NONCLUSTERED INDEX [IX_SystemPreferences_ModifiedDate] ON [dbo].[SystemPreferences] ([ModifiedDate]);

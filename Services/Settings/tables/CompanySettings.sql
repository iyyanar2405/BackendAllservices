-- CompanySettings table for extended company settings stored as JSON
CREATE TABLE [dbo].[CompanySettings]
(
    [CompanyId] INT NOT NULL,
    [SettingsJson] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedBy] INT NULL,

    CONSTRAINT [PK_CompanySettings] PRIMARY KEY CLUSTERED ([CompanyId]),
    CONSTRAINT [FK_CompanySettings_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanySettings_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [Users]([UserId])
);

CREATE NONCLUSTERED INDEX [IX_CompanySettings_ModifiedDate] ON [dbo].[CompanySettings] ([ModifiedDate]);

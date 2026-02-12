-- Settings schema updates for SettingsAPI.Gateway

-- Add Users.UserStatus if missing
IF COL_LENGTH('dbo.Users', 'UserStatus') IS NULL
BEGIN
    ALTER TABLE [dbo].[Users]
    ADD [UserStatus] NVARCHAR(50) NULL CONSTRAINT [DF_Users_UserStatus] DEFAULT 'Pending';
END

-- Add Companies settings columns if missing
IF COL_LENGTH('dbo.Companies', 'ParentCompanyId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Companies]
    ADD [ParentCompanyId] INT NULL;

    ALTER TABLE [dbo].[Companies]
    ADD CONSTRAINT [FK_Companies_ParentCompanyId]
        FOREIGN KEY ([ParentCompanyId]) REFERENCES [dbo].[Companies]([CompanyId]);
END

IF COL_LENGTH('dbo.Companies', 'AccountDNVId') IS NULL
BEGIN
    ALTER TABLE [dbo].[Companies]
    ADD [AccountDNVId] NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.Companies', 'ZipCode') IS NULL
BEGIN
    ALTER TABLE [dbo].[Companies]
    ADD [ZipCode] NVARCHAR(20) NULL;
END

IF COL_LENGTH('dbo.Companies', 'VATNumber') IS NULL
BEGIN
    ALTER TABLE [dbo].[Companies]
    ADD [VATNumber] NVARCHAR(50) NULL;
END

IF COL_LENGTH('dbo.Companies', 'PONumberRequired') IS NULL
BEGIN
    ALTER TABLE [dbo].[Companies]
    ADD [PONumberRequired] BIT NOT NULL CONSTRAINT [DF_Companies_PONumberRequired] DEFAULT 0;
END

IF COL_LENGTH('dbo.Companies', 'IsServiceRequestOpen') IS NULL
BEGIN
    ALTER TABLE [dbo].[Companies]
    ADD [IsServiceRequestOpen] BIT NOT NULL CONSTRAINT [DF_Companies_IsServiceRequestOpen] DEFAULT 0;
END

-- Add UserPreferences preference-detail fields if missing
IF COL_LENGTH('dbo.UserPreferences', 'ObjectType') IS NULL
BEGIN
    ALTER TABLE [dbo].[UserPreferences]
    ADD [ObjectType] NVARCHAR(50) NULL,
        [ObjectName] NVARCHAR(50) NULL,
        [PageName] NVARCHAR(50) NULL,
        [PreferenceDetail] NVARCHAR(MAX) NULL;
END

-- Create SystemPreferences table if missing
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SystemPreferences' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[SystemPreferences]
    (
        [SystemPreferenceId] INT IDENTITY(1,1) NOT NULL,
        [PreferencesJson] NVARCHAR(MAX) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,

        CONSTRAINT [PK_SystemPreferences] PRIMARY KEY CLUSTERED ([SystemPreferenceId]),
        CONSTRAINT [FK_SystemPreferences_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Users]([UserId])
    );

    CREATE NONCLUSTERED INDEX [IX_SystemPreferences_ModifiedDate]
        ON [dbo].[SystemPreferences] ([ModifiedDate]);
END

-- Create UserPreferenceProfiles table if missing
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserPreferenceProfiles' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[UserPreferenceProfiles]
    (
        [UserPreferenceProfileId] INT IDENTITY(1,1) NOT NULL,
        [UserId] INT NOT NULL,
        [PreferencesJson] NVARCHAR(MAX) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,

        CONSTRAINT [PK_UserPreferenceProfiles] PRIMARY KEY CLUSTERED ([UserPreferenceProfileId]),
        CONSTRAINT [FK_UserPreferenceProfiles_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserPreferenceProfiles_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Users]([UserId])
    );

    CREATE NONCLUSTERED INDEX [IX_UserPreferenceProfiles_UserId]
        ON [dbo].[UserPreferenceProfiles] ([UserId]);
    CREATE NONCLUSTERED INDEX [IX_UserPreferenceProfiles_ModifiedDate]
        ON [dbo].[UserPreferenceProfiles] ([ModifiedDate]);
END

-- Create NotificationTemplates table if missing
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'NotificationTemplates' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[NotificationTemplates]
    (
        [NotificationTemplateId] INT IDENTITY(1,1) NOT NULL,
        [TemplateName] NVARCHAR(200) NOT NULL,
        [TemplateType] NVARCHAR(50) NOT NULL,
        [Category] NVARCHAR(100) NULL,
        [Subject] NVARCHAR(300) NULL,
        [BodyHtml] NVARCHAR(MAX) NULL,
        [BodyText] NVARCHAR(MAX) NULL,
        [VariablesJson] NVARCHAR(MAX) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [CreatedBy] INT NULL,
        [ModifiedBy] INT NULL,

        CONSTRAINT [PK_NotificationTemplates] PRIMARY KEY CLUSTERED ([NotificationTemplateId]),
        CONSTRAINT [FK_NotificationTemplates_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([UserId]),
        CONSTRAINT [FK_NotificationTemplates_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[Users]([UserId])
    );

    CREATE NONCLUSTERED INDEX [IX_NotificationTemplates_TemplateName]
        ON [dbo].[NotificationTemplates] ([TemplateName]);
    CREATE NONCLUSTERED INDEX [IX_NotificationTemplates_IsActive]
        ON [dbo].[NotificationTemplates] ([IsActive]);
END

-- Create CompanySettings table if missing
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CompanySettings' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE [dbo].[CompanySettings]
    (
        [CompanyId] INT NOT NULL,
        [SettingsJson] NVARCHAR(MAX) NOT NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [UpdatedBy] INT NULL,

        CONSTRAINT [PK_CompanySettings] PRIMARY KEY CLUSTERED ([CompanyId]),
        CONSTRAINT [FK_CompanySettings_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[Companies]([CompanyId]) ON DELETE CASCADE,
        CONSTRAINT [FK_CompanySettings_UpdatedBy] FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Users]([UserId])
    );

    CREATE NONCLUSTERED INDEX [IX_CompanySettings_ModifiedDate]
        ON [dbo].[CompanySettings] ([ModifiedDate]);
END

-- NotificationTemplates table for email/SMS template storage
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
    CONSTRAINT [FK_NotificationTemplates_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_NotificationTemplates_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

CREATE NONCLUSTERED INDEX [IX_NotificationTemplates_TemplateName] ON [dbo].[NotificationTemplates] ([TemplateName]);
CREATE NONCLUSTERED INDEX [IX_NotificationTemplates_IsActive] ON [dbo].[NotificationTemplates] ([IsActive]);

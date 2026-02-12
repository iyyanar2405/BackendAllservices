-- AuditTypes table for different types of audits
CREATE TABLE [dbo].[AuditTypes]
(
    [AuditTypeId] INT IDENTITY(1,1) NOT NULL,
    [AuditTypeName] NVARCHAR(100) NOT NULL,
    [AuditTypeCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Duration] INT NULL, -- Duration in days
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Category] NVARCHAR(100) NULL,
    [RequiredCertifications] NVARCHAR(500) NULL,
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_AuditTypes] PRIMARY KEY CLUSTERED ([AuditTypeId]),
    CONSTRAINT [UK_AuditTypes_AuditTypeName] UNIQUE ([AuditTypeName]),
    CONSTRAINT [UK_AuditTypes_AuditTypeCode] UNIQUE ([AuditTypeCode]),
    CONSTRAINT [FK_AuditTypes_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditTypes_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditTypes_AuditTypeName] ON [dbo].[AuditTypes] ([AuditTypeName]);
CREATE NONCLUSTERED INDEX [IX_AuditTypes_IsActive] ON [dbo].[AuditTypes] ([IsActive]);
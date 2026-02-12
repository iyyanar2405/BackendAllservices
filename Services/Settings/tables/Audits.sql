CREATE TABLE [dbo].[Audits]
(
    [AuditId] INT NOT NULL,
    [Sites] NVARCHAR(MAX) NULL,
    [Services] NVARCHAR(MAX) NULL,
    [CompanyId] INT NULL,
    [Status] NVARCHAR(50) NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [LeadAuditor] NVARCHAR(100) NULL,
    [Type] NVARCHAR(50) NULL,

    CONSTRAINT [PK_Audits] PRIMARY KEY CLUSTERED ([AuditId])
);
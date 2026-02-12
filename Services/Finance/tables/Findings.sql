-- Findings table for audit findings
CREATE TABLE [dbo].[Findings]
(
    [FindingId] INT IDENTITY(1,1) NOT NULL,
    [FindingNumber] NVARCHAR(50) NOT NULL,
    [AuditId] INT NOT NULL,
    [SiteId] INT NULL,
    [Title] NVARCHAR(500) NOT NULL,
    [Description] NVARCHAR(MAX) NOT NULL,
    [FindingType] NVARCHAR(50) NOT NULL, -- 'NC' (Non-Conformity), 'OFI' (Opportunity for Improvement), 'Observation'
    [Severity] NVARCHAR(50) NULL, -- 'Critical', 'Major', 'Minor'
    [FindingStatusId] INT NOT NULL,
    [FindingCategoryId] INT NULL,
    [IdentifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [DueDate] DATETIME NULL,
    [ClosedDate] DATETIME NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [IdentifiedBy] INT NULL, -- Auditor who identified the finding
    [AssignedTo] INT NULL, -- Person responsible for resolution
    [Evidence] NVARCHAR(MAX) NULL,
    [RootCause] NVARCHAR(MAX) NULL,
    [CorrectiveAction] NVARCHAR(MAX) NULL,
    [PreventiveAction] NVARCHAR(MAX) NULL,
    [VerificationMethod] NVARCHAR(MAX) NULL,
    [CompletionDate] DATETIME NULL,
    [VerificationDate] DATETIME NULL,
    [VerifiedBy] INT NULL,

    CONSTRAINT [PK_Findings] PRIMARY KEY CLUSTERED ([FindingId]),
    CONSTRAINT [UK_Findings_FindingNumber] UNIQUE ([FindingNumber]),
    CONSTRAINT [FK_Findings_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits]([AuditId]),
    CONSTRAINT [FK_Findings_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_Findings_FindingStatusId] FOREIGN KEY ([FindingStatusId]) REFERENCES [FindingStatuses]([FindingStatusId]),
    CONSTRAINT [FK_Findings_FindingCategoryId] FOREIGN KEY ([FindingCategoryId]) REFERENCES [FindingCategories]([FindingCategoryId]),
    CONSTRAINT [FK_Findings_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Findings_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Findings_IdentifiedBy] FOREIGN KEY ([IdentifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Findings_AssignedTo] FOREIGN KEY ([AssignedTo]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Findings_VerifiedBy] FOREIGN KEY ([VerifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_Findings_FindingType] CHECK ([FindingType] IN ('NC', 'OFI', 'Observation')),
    CONSTRAINT [CK_Findings_Severity] CHECK ([Severity] IN ('Critical', 'Major', 'Minor'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Findings_AuditId] ON [dbo].[Findings] ([AuditId]);
CREATE NONCLUSTERED INDEX [IX_Findings_SiteId] ON [dbo].[Findings] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Findings_FindingStatusId] ON [dbo].[Findings] ([FindingStatusId]);
CREATE NONCLUSTERED INDEX [IX_Findings_FindingType] ON [dbo].[Findings] ([FindingType]);
CREATE NONCLUSTERED INDEX [IX_Findings_Severity] ON [dbo].[Findings] ([Severity]);
CREATE NONCLUSTERED INDEX [IX_Findings_DueDate] ON [dbo].[Findings] ([DueDate]);
CREATE NONCLUSTERED INDEX [IX_Findings_AssignedTo] ON [dbo].[Findings] ([AssignedTo]);
CREATE NONCLUSTERED INDEX [IX_Findings_IsActive] ON [dbo].[Findings] ([IsActive]);
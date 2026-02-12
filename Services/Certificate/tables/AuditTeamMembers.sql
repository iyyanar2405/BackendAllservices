-- AuditTeamMembers table for audit team composition
CREATE TABLE [dbo].[AuditTeamMembers]
(
    [AuditTeamMemberId] INT IDENTITY(1,1) NOT NULL,
    [AuditId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Role] NVARCHAR(100) NOT NULL, -- 'Lead Auditor', 'Auditor', 'Technical Expert', 'Observer'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [AssignedDate] DATETIME NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [Specialization] NVARCHAR(200) NULL,
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_AuditTeamMembers] PRIMARY KEY CLUSTERED ([AuditTeamMemberId]),
    CONSTRAINT [UK_AuditTeamMembers_Audit_User] UNIQUE ([AuditId], [UserId]),
    CONSTRAINT [FK_AuditTeamMembers_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits]([AuditId]),
    CONSTRAINT [FK_AuditTeamMembers_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditTeamMembers_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditTeamMembers_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditTeamMembers_AuditId] ON [dbo].[AuditTeamMembers] ([AuditId]);
CREATE NONCLUSTERED INDEX [IX_AuditTeamMembers_UserId] ON [dbo].[AuditTeamMembers] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AuditTeamMembers_Role] ON [dbo].[AuditTeamMembers] ([Role]);
CREATE NONCLUSTERED INDEX [IX_AuditTeamMembers_IsActive] ON [dbo].[AuditTeamMembers] ([IsActive]);
-- AuditSiteRepresentatives table for company representatives during audit site visits
CREATE TABLE [dbo].[AuditSiteRepresentatives]
(
    [AuditSiteRepresentativeId] INT IDENTITY(1,1) NOT NULL,
    [AuditSiteAuditId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [Role] NVARCHAR(100) NULL, -- 'Primary Contact', 'Technical Representative', 'Quality Manager', etc.
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [ContactPhone] NVARCHAR(20) NULL,
    [ContactEmail] NVARCHAR(255) NULL,
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_AuditSiteRepresentatives] PRIMARY KEY CLUSTERED ([AuditSiteRepresentativeId]),
    CONSTRAINT [UK_AuditSiteRepresentatives_AuditSite_User] UNIQUE ([AuditSiteAuditId], [UserId]),
    CONSTRAINT [FK_AuditSiteRepresentatives_AuditSiteAuditId] FOREIGN KEY ([AuditSiteAuditId]) REFERENCES [AuditSiteAudits]([AuditSiteAuditId]),
    CONSTRAINT [FK_AuditSiteRepresentatives_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditSiteRepresentatives_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_AuditSiteRepresentatives_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_AuditSiteRepresentatives_AuditSiteAuditId] ON [dbo].[AuditSiteRepresentatives] ([AuditSiteAuditId]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteRepresentatives_UserId] ON [dbo].[AuditSiteRepresentatives] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_AuditSiteRepresentatives_IsActive] ON [dbo].[AuditSiteRepresentatives] ([IsActive]);
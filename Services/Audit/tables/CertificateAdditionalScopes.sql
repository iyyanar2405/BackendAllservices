-- CertificateAdditionalScopes table for additional scopes associated with certificates
CREATE TABLE [dbo].[CertificateAdditionalScopes]
(
    [CertificateAdditionalScopeId] INT IDENTITY(1,1) NOT NULL,
    [CertificateId] INT NOT NULL,
    [ScopeDescription] NVARCHAR(1000) NOT NULL,
    [ScopeType] NVARCHAR(100) NULL, -- 'Product', 'Service', 'Process', 'Location'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [EffectiveDate] DATETIME NULL,
    [ExpiryDate] DATETIME NULL,
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_CertificateAdditionalScopes] PRIMARY KEY CLUSTERED ([CertificateAdditionalScopeId]),
    CONSTRAINT [FK_CertificateAdditionalScopes_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [Certificates]([CertificateId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CertificateAdditionalScopes_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_CertificateAdditionalScopes_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_CertificateAdditionalScopes_CertificateId] ON [dbo].[CertificateAdditionalScopes] ([CertificateId]);
CREATE NONCLUSTERED INDEX [IX_CertificateAdditionalScopes_ScopeType] ON [dbo].[CertificateAdditionalScopes] ([ScopeType]);
CREATE NONCLUSTERED INDEX [IX_CertificateAdditionalScopes_IsActive] ON [dbo].[CertificateAdditionalScopes] ([IsActive]);
-- Certificates table for issued certificates
CREATE TABLE [dbo].[Certificates]
(
    [CertificateId] INT IDENTITY(1,1) NOT NULL,
    [CertificateNumber] NVARCHAR(100) NOT NULL,
    [CertificateName] NVARCHAR(200) NOT NULL,
    [CompanyId] INT NOT NULL,
    [SiteId] INT NULL,
    [ServiceId] INT NOT NULL,
    [IssueDate] DATETIME NOT NULL,
    [ExpiryDate] DATETIME NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- 'Active', 'Expired', 'Suspended', 'Cancelled'
    [CertificateType] NVARCHAR(100) NULL, -- 'Initial', 'Renewal', 'Recertification'
    [Scope] NVARCHAR(MAX) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [IssuedBy] INT NULL,
    [RevisionNumber] INT NOT NULL DEFAULT 1,
    [PreviousCertificateId] INT NULL, -- Reference to previous certificate if this is a renewal
    [CertificatePath] NVARCHAR(500) NULL, -- Path to certificate file
    [AuditId] INT NULL, -- Audit that led to this certificate
    [Notes] NVARCHAR(1000) NULL,

    CONSTRAINT [PK_Certificates] PRIMARY KEY CLUSTERED ([CertificateId]),
    CONSTRAINT [UK_Certificates_CertificateNumber] UNIQUE ([CertificateNumber]),
    CONSTRAINT [FK_Certificates_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_Certificates_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_Certificates_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_Certificates_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Certificates_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Certificates_IssuedBy] FOREIGN KEY ([IssuedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Certificates_PreviousCertificateId] FOREIGN KEY ([PreviousCertificateId]) REFERENCES [Certificates]([CertificateId]),
    CONSTRAINT [FK_Certificates_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits]([AuditId]),
    CONSTRAINT [CK_Certificates_Status] CHECK ([Status] IN ('Active', 'Expired', 'Suspended', 'Cancelled')),
    CONSTRAINT [CK_Certificates_CertificateType] CHECK ([CertificateType] IN ('Initial', 'Renewal', 'Recertification'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Certificates_CompanyId] ON [dbo].[Certificates] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_Certificates_SiteId] ON [dbo].[Certificates] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_Certificates_ServiceId] ON [dbo].[Certificates] ([ServiceId]);
CREATE NONCLUSTERED INDEX [IX_Certificates_Status] ON [dbo].[Certificates] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Certificates_ExpiryDate] ON [dbo].[Certificates] ([ExpiryDate]);
CREATE NONCLUSTERED INDEX [IX_Certificates_IsActive] ON [dbo].[Certificates] ([IsActive]);
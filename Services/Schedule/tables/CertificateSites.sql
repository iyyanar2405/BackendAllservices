-- CertificateSites table for many-to-many relationship between certificates and sites
CREATE TABLE [dbo].[CertificateSites]
(
    [CertificateSiteId] INT IDENTITY(1,1) NOT NULL,
    [CertificateId] INT NOT NULL,
    [SiteId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Scope] NVARCHAR(1000) NULL, -- Specific scope for this site
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_CertificateSites] PRIMARY KEY CLUSTERED ([CertificateSiteId]),
    CONSTRAINT [UK_CertificateSites_Certificate_Site] UNIQUE ([CertificateId], [SiteId]),
    CONSTRAINT [FK_CertificateSites_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [Certificates]([CertificateId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CertificateSites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_CertificateSites_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_CertificateSites_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_CertificateSites_CertificateId] ON [dbo].[CertificateSites] ([CertificateId]);
CREATE NONCLUSTERED INDEX [IX_CertificateSites_SiteId] ON [dbo].[CertificateSites] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_CertificateSites_IsActive] ON [dbo].[CertificateSites] ([IsActive]);
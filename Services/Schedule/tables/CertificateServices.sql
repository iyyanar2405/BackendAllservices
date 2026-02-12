-- CertificateServices table for many-to-many relationship between certificates and services
CREATE TABLE [dbo].[CertificateServices]
(
    [CertificateServiceId] INT IDENTITY(1,1) NOT NULL,
    [CertificateId] INT NOT NULL,
    [ServiceId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Scope] NVARCHAR(1000) NULL, -- Specific scope for this service
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_CertificateServices] PRIMARY KEY CLUSTERED ([CertificateServiceId]),
    CONSTRAINT [UK_CertificateServices_Certificate_Service] UNIQUE ([CertificateId], [ServiceId]),
    CONSTRAINT [FK_CertificateServices_CertificateId] FOREIGN KEY ([CertificateId]) REFERENCES [Certificates]([CertificateId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CertificateServices_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_CertificateServices_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_CertificateServices_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_CertificateServices_CertificateId] ON [dbo].[CertificateServices] ([CertificateId]);
CREATE NONCLUSTERED INDEX [IX_CertificateServices_ServiceId] ON [dbo].[CertificateServices] ([ServiceId]);
CREATE NONCLUSTERED INDEX [IX_CertificateServices_IsActive] ON [dbo].[CertificateServices] ([IsActive]);
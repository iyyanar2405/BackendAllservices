-- Contracts table for service contracts
CREATE TABLE [dbo].[Contracts]
(
    [ContractId] INT IDENTITY(1,1) NOT NULL,
    [ContractNumber] NVARCHAR(100) NOT NULL,
    [ContractName] NVARCHAR(200) NOT NULL,
    [CompanyId] INT NOT NULL,
    [ContractType] NVARCHAR(100) NULL, -- 'Certification', 'Assessment', 'Training', 'Consulting'
    [StartDate] DATETIME NOT NULL,
    [EndDate] DATETIME NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- 'Draft', 'Active', 'Completed', 'Cancelled', 'Suspended'
    [TotalValue] DECIMAL(12,2) NULL,
    [Currency] NVARCHAR(3) NULL DEFAULT 'USD',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [SignedDate] DATETIME NULL,
    [SignedByClient] NVARCHAR(100) NULL,
    [SignedByDNV] NVARCHAR(100) NULL,
    [ContractPath] NVARCHAR(500) NULL, -- Path to contract document
    [Terms] NVARCHAR(MAX) NULL,
    [Notes] NVARCHAR(1000) NULL,
    [RenewalDate] DATETIME NULL,
    [AutoRenewal] BIT NOT NULL DEFAULT 0,

    CONSTRAINT [PK_Contracts] PRIMARY KEY CLUSTERED ([ContractId]),
    CONSTRAINT [UK_Contracts_ContractNumber] UNIQUE ([ContractNumber]),
    CONSTRAINT [FK_Contracts_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_Contracts_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Contracts_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_Contracts_Status] CHECK ([Status] IN ('Draft', 'Active', 'Completed', 'Cancelled', 'Suspended'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Contracts_CompanyId] ON [dbo].[Contracts] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_Contracts_Status] ON [dbo].[Contracts] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Contracts_StartDate] ON [dbo].[Contracts] ([StartDate]);
CREATE NONCLUSTERED INDEX [IX_Contracts_EndDate] ON [dbo].[Contracts] ([EndDate]);
CREATE NONCLUSTERED INDEX [IX_Contracts_IsActive] ON [dbo].[Contracts] ([IsActive]);
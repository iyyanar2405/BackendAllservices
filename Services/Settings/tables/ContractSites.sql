-- ContractSites table for many-to-many relationship between contracts and sites
CREATE TABLE [dbo].[ContractSites]
(
    [ContractSiteId] INT IDENTITY(1,1) NOT NULL,
    [ContractId] INT NOT NULL,
    [SiteId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active', -- 'Active', 'Completed', 'Cancelled'
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_ContractSites] PRIMARY KEY CLUSTERED ([ContractSiteId]),
    CONSTRAINT [UK_ContractSites_Contract_Site] UNIQUE ([ContractId], [SiteId]),
    CONSTRAINT [FK_ContractSites_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Contracts]([ContractId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ContractSites_SiteId] FOREIGN KEY ([SiteId]) REFERENCES [Sites]([SiteId]),
    CONSTRAINT [FK_ContractSites_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_ContractSites_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_ContractSites_ContractId] ON [dbo].[ContractSites] ([ContractId]);
CREATE NONCLUSTERED INDEX [IX_ContractSites_SiteId] ON [dbo].[ContractSites] ([SiteId]);
CREATE NONCLUSTERED INDEX [IX_ContractSites_IsActive] ON [dbo].[ContractSites] ([IsActive]);
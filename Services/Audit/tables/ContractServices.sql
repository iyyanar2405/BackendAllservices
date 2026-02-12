-- ContractServices table for many-to-many relationship between contracts and services
CREATE TABLE [dbo].[ContractServices]
(
    [ContractServiceId] INT IDENTITY(1,1) NOT NULL,
    [ContractId] INT NOT NULL,
    [ServiceId] INT NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 1,
    [UnitPrice] DECIMAL(10,2) NULL,
    [TotalPrice] DECIMAL(12,2) NULL,
    [Currency] NVARCHAR(3) NULL DEFAULT 'USD',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [StartDate] DATETIME NULL,
    [EndDate] DATETIME NULL,
    [Status] NVARCHAR(50) NULL DEFAULT 'Active', -- 'Active', 'Completed', 'Cancelled'
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_ContractServices] PRIMARY KEY CLUSTERED ([ContractServiceId]),
    CONSTRAINT [UK_ContractServices_Contract_Service] UNIQUE ([ContractId], [ServiceId]),
    CONSTRAINT [FK_ContractServices_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Contracts]([ContractId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ContractServices_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [Services]([ServiceId]),
    CONSTRAINT [FK_ContractServices_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_ContractServices_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_ContractServices_ContractId] ON [dbo].[ContractServices] ([ContractId]);
CREATE NONCLUSTERED INDEX [IX_ContractServices_ServiceId] ON [dbo].[ContractServices] ([ServiceId]);
CREATE NONCLUSTERED INDEX [IX_ContractServices_IsActive] ON [dbo].[ContractServices] ([IsActive]);
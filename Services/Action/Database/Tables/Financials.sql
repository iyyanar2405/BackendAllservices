-- Financials table for financial data tracking
CREATE TABLE [dbo].[Financials]
(
    [FinancialId] INT IDENTITY(1,1) NOT NULL,
    [CompanyId] INT NOT NULL,
    [Year] INT NOT NULL,
    [Quarter] INT NULL, -- 1-4 for quarterly data, NULL for annual
    [Month] INT NULL, -- 1-12 for monthly data, NULL for annual/quarterly
    [Revenue] DECIMAL(15,2) NULL,
    [Expenses] DECIMAL(15,2) NULL,
    [Profit] DECIMAL(15,2) NULL,
    [OutstandingAmount] DECIMAL(15,2) NULL,
    [PaidAmount] DECIMAL(15,2) NULL,
    [OverdueAmount] DECIMAL(15,2) NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Notes] NVARCHAR(1000) NULL,
    [DataSource] NVARCHAR(100) NULL, -- 'Manual', 'Automated', 'Import'

    CONSTRAINT [PK_Financials] PRIMARY KEY CLUSTERED ([FinancialId]),
    CONSTRAINT [UK_Financials_Company_Year_Quarter_Month] UNIQUE ([CompanyId], [Year], [Quarter], [Month]),
    CONSTRAINT [FK_Financials_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_Financials_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Financials_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_Financials_Quarter] CHECK ([Quarter] BETWEEN 1 AND 4 OR [Quarter] IS NULL),
    CONSTRAINT [CK_Financials_Month] CHECK ([Month] BETWEEN 1 AND 12 OR [Month] IS NULL)
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Financials_CompanyId] ON [dbo].[Financials] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_Financials_Year] ON [dbo].[Financials] ([Year]);
CREATE NONCLUSTERED INDEX [IX_Financials_IsActive] ON [dbo].[Financials] ([IsActive]);
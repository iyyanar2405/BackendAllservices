-- Invoices table for billing and payment tracking
CREATE TABLE [dbo].[Invoices]
(
    [InvoiceId] INT IDENTITY(1,1) NOT NULL,
    [InvoiceNumber] NVARCHAR(50) NOT NULL,
    [CompanyId] INT NOT NULL,
    [ContractId] INT NULL,
    [InvoiceDate] DATETIME NOT NULL,
    [DueDate] DATETIME NOT NULL,
    [PlannedPaymentDate] DATETIME NULL,
    [PaidDate] DATETIME NULL,
    [Amount] DECIMAL(12,2) NOT NULL,
    [TaxAmount] DECIMAL(12,2) NULL DEFAULT 0,
    [TotalAmount] DECIMAL(12,2) NOT NULL,
    [Currency] NVARCHAR(3) NOT NULL DEFAULT 'USD',
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- 'Draft', 'Pending', 'Paid', 'Overdue', 'Cancelled'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Description] NVARCHAR(500) NULL,
    [Terms] NVARCHAR(1000) NULL,
    [Notes] NVARCHAR(1000) NULL,
    [InvoicePath] NVARCHAR(500) NULL, -- Path to invoice document
    [PaymentMethod] NVARCHAR(50) NULL,
    [PaymentReference] NVARCHAR(100) NULL,
    [DiscountAmount] DECIMAL(10,2) NULL DEFAULT 0,
    [LateFee] DECIMAL(10,2) NULL DEFAULT 0,

    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([InvoiceId]),
    CONSTRAINT [UK_Invoices_InvoiceNumber] UNIQUE ([InvoiceNumber]),
    CONSTRAINT [FK_Invoices_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_Invoices_ContractId] FOREIGN KEY ([ContractId]) REFERENCES [Contracts]([ContractId]),
    CONSTRAINT [FK_Invoices_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Invoices_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_Invoices_Status] CHECK ([Status] IN ('Draft', 'Pending', 'Paid', 'Overdue', 'Cancelled'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Invoices_CompanyId] ON [dbo].[Invoices] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_Invoices_Status] ON [dbo].[Invoices] ([Status]);
CREATE NONCLUSTERED INDEX [IX_Invoices_DueDate] ON [dbo].[Invoices] ([DueDate]);
CREATE NONCLUSTERED INDEX [IX_Invoices_InvoiceDate] ON [dbo].[Invoices] ([InvoiceDate]);
CREATE NONCLUSTERED INDEX [IX_Invoices_IsActive] ON [dbo].[Invoices] ([IsActive]);
-- Services table for certification services offered
CREATE TABLE [dbo].[Services]
(
    [ServiceId] INT IDENTITY(1,1) NOT NULL,
    [ServiceName] NVARCHAR(200) NOT NULL,
    [ServiceCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [ServiceType] NVARCHAR(100) NULL, -- e.g., 'Certification', 'Assessment', 'Training'
    [Category] NVARCHAR(100) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Duration] INT NULL, -- Duration in days
    [Cost] DECIMAL(10,2) NULL,
    [Currency] NVARCHAR(3) NULL DEFAULT 'USD',
    [Prerequisites] NVARCHAR(1000) NULL,
    [ValidityPeriod] INT NULL, -- Validity in months
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_Services] PRIMARY KEY CLUSTERED ([ServiceId]),
    CONSTRAINT [UK_Services_ServiceCode] UNIQUE ([ServiceCode]),
    CONSTRAINT [UK_Services_ServiceName] UNIQUE ([ServiceName]),
    CONSTRAINT [FK_Services_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Services_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Services_ServiceName] ON [dbo].[Services] ([ServiceName]);
CREATE NONCLUSTERED INDEX [IX_Services_ServiceCode] ON [dbo].[Services] ([ServiceCode]);
CREATE NONCLUSTERED INDEX [IX_Services_IsActive] ON [dbo].[Services] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Services_ServiceType] ON [dbo].[Services] ([ServiceType]);
CREATE NONCLUSTERED INDEX [IX_Services_Category] ON [dbo].[Services] ([Category]);
-- Companies table for customer organizations
CREATE TABLE [dbo].[Companies]
(
    [CompanyId] INT IDENTITY(1,1) NOT NULL,
    [CompanyName] NVARCHAR(200) NOT NULL,
    [CompanyCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [Address] NVARCHAR(255) NULL,
    [CityId] INT NULL,
    [CountryId] INT NULL,
    [PostalCode] NVARCHAR(20) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Email] NVARCHAR(255) NULL,
    [Website] NVARCHAR(255) NULL,
    [ContactPerson] NVARCHAR(100) NULL,
    [ContactEmail] NVARCHAR(255) NULL,
    [ContactPhone] NVARCHAR(20) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Industry] NVARCHAR(100) NULL,
    [EmployeeCount] INT NULL,
    [TaxId] NVARCHAR(50) NULL,
    [RegistrationNumber] NVARCHAR(50) NULL,
    [LogoUrl] NVARCHAR(500) NULL,

    CONSTRAINT [PK_Companies] PRIMARY KEY CLUSTERED ([CompanyId]),
    CONSTRAINT [UK_Companies_CompanyCode] UNIQUE ([CompanyCode]),
    CONSTRAINT [UK_Companies_CompanyName] UNIQUE ([CompanyName]),
    CONSTRAINT [FK_Companies_CityId] FOREIGN KEY ([CityId]) REFERENCES [Cities]([CityId]),
    CONSTRAINT [FK_Companies_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Countries]([CountryId]),
    CONSTRAINT [FK_Companies_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Companies_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Companies_CompanyName] ON [dbo].[Companies] ([CompanyName]);
CREATE NONCLUSTERED INDEX [IX_Companies_CompanyCode] ON [dbo].[Companies] ([CompanyCode]);
CREATE NONCLUSTERED INDEX [IX_Companies_IsActive] ON [dbo].[Companies] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Companies_CountryId] ON [dbo].[Companies] ([CountryId]);
CREATE NONCLUSTERED INDEX [IX_Companies_CityId] ON [dbo].[Companies] ([CityId]);
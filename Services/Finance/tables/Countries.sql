-- Countries table for geographical organization
CREATE TABLE [dbo].[Countries]
(
    [CountryId] INT IDENTITY(1,1) NOT NULL,
    [CountryName] NVARCHAR(100) NOT NULL,
    [CountryCode] NVARCHAR(3) NOT NULL, -- ISO 3166-1 alpha-3
    [CountryCodeAlpha2] NVARCHAR(2) NOT NULL, -- ISO 3166-1 alpha-2
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Region] NVARCHAR(100) NULL,
    [Continent] NVARCHAR(50) NULL,
    [Currency] NVARCHAR(3) NULL, -- ISO 4217 currency code
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED ([CountryId]),
    CONSTRAINT [UK_Countries_CountryCode] UNIQUE ([CountryCode]),
    CONSTRAINT [UK_Countries_CountryCodeAlpha2] UNIQUE ([CountryCodeAlpha2]),
    CONSTRAINT [UK_Countries_CountryName] UNIQUE ([CountryName]),
    CONSTRAINT [FK_Countries_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Countries_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Countries_CountryName] ON [dbo].[Countries] ([CountryName]);
CREATE NONCLUSTERED INDEX [IX_Countries_IsActive] ON [dbo].[Countries] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Countries_Region] ON [dbo].[Countries] ([Region]);
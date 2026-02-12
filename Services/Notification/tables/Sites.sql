-- Sites table for company locations/facilities
CREATE TABLE [dbo].[Sites]
(
    [SiteId] INT IDENTITY(1,1) NOT NULL,
    [SiteName] NVARCHAR(200) NOT NULL,
    [SiteCode] NVARCHAR(50) NOT NULL,
    [CompanyId] INT NOT NULL,
    [Address] NVARCHAR(255) NULL,
    [CityId] INT NULL,
    [CountryId] INT NULL,
    [PostalCode] NVARCHAR(20) NULL,
    [Latitude] DECIMAL(10,8) NULL,
    [Longitude] DECIMAL(11,8) NULL,
    [Phone] NVARCHAR(20) NULL,
    [Email] NVARCHAR(255) NULL,
    [ContactPerson] NVARCHAR(100) NULL,
    [ContactEmail] NVARCHAR(255) NULL,
    [ContactPhone] NVARCHAR(20) NULL,
    [SiteType] NVARCHAR(100) NULL, -- e.g., 'Headquarters', 'Manufacturing', 'Office', 'Warehouse'
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [EmployeeCount] INT NULL,
    [Area] DECIMAL(10,2) NULL, -- Area in square meters
    [TimeZone] NVARCHAR(50) NULL,

    CONSTRAINT [PK_Sites] PRIMARY KEY CLUSTERED ([SiteId]),
    CONSTRAINT [UK_Sites_SiteCode] UNIQUE ([SiteCode]),
    CONSTRAINT [UK_Sites_SiteName_Company] UNIQUE ([SiteName], [CompanyId]),
    CONSTRAINT [FK_Sites_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies]([CompanyId]),
    CONSTRAINT [FK_Sites_CityId] FOREIGN KEY ([CityId]) REFERENCES [Cities]([CityId]),
    CONSTRAINT [FK_Sites_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Countries]([CountryId]),
    CONSTRAINT [FK_Sites_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Sites_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Sites_CompanyId] ON [dbo].[Sites] ([CompanyId]);
CREATE NONCLUSTERED INDEX [IX_Sites_SiteName] ON [dbo].[Sites] ([SiteName]);
CREATE NONCLUSTERED INDEX [IX_Sites_SiteCode] ON [dbo].[Sites] ([SiteCode]);
CREATE NONCLUSTERED INDEX [IX_Sites_IsActive] ON [dbo].[Sites] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Sites_CountryId] ON [dbo].[Sites] ([CountryId]);
CREATE NONCLUSTERED INDEX [IX_Sites_CityId] ON [dbo].[Sites] ([CityId]);
CREATE NONCLUSTERED INDEX [IX_Sites_SiteType] ON [dbo].[Sites] ([SiteType]);
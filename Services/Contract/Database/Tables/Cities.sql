-- Cities table for geographical organization under countries
CREATE TABLE [dbo].[Cities]
(
    [CityId] INT IDENTITY(1,1) NOT NULL,
    [CityName] NVARCHAR(100) NOT NULL,
    [CountryId] INT NOT NULL,
    [StateProvince] NVARCHAR(100) NULL,
    [PostalCode] NVARCHAR(20) NULL,
    [Latitude] DECIMAL(10,8) NULL,
    [Longitude] DECIMAL(11,8) NULL,
    [TimeZone] NVARCHAR(50) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_Cities] PRIMARY KEY CLUSTERED ([CityId]),
    CONSTRAINT [UK_Cities_Name_Country] UNIQUE ([CityName], [CountryId]),
    CONSTRAINT [FK_Cities_CountryId] FOREIGN KEY ([CountryId]) REFERENCES [Countries]([CountryId]),
    CONSTRAINT [FK_Cities_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Cities_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Cities_CountryId] ON [dbo].[Cities] ([CountryId]);
CREATE NONCLUSTERED INDEX [IX_Cities_CityName] ON [dbo].[Cities] ([CityName]);
CREATE NONCLUSTERED INDEX [IX_Cities_IsActive] ON [dbo].[Cities] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Cities_StateProvince] ON [dbo].[Cities] ([StateProvince]);
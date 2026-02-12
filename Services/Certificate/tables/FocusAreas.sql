-- FocusAreas table for audit focus areas
CREATE TABLE [dbo].[FocusAreas]
(
    [FocusAreaId] INT IDENTITY(1,1) NOT NULL,
    [FocusAreaName] NVARCHAR(200) NOT NULL,
    [FocusAreaCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Category] NVARCHAR(100) NULL,
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_FocusAreas] PRIMARY KEY CLUSTERED ([FocusAreaId]),
    CONSTRAINT [UK_FocusAreas_FocusAreaName] UNIQUE ([FocusAreaName]),
    CONSTRAINT [UK_FocusAreas_FocusAreaCode] UNIQUE ([FocusAreaCode]),
    CONSTRAINT [FK_FocusAreas_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FocusAreas_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_FocusAreas_FocusAreaName] ON [dbo].[FocusAreas] ([FocusAreaName]);
CREATE NONCLUSTERED INDEX [IX_FocusAreas_IsActive] ON [dbo].[FocusAreas] ([IsActive]);
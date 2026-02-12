-- FindingCategories table for categorizing findings
CREATE TABLE [dbo].[FindingCategories]
(
    [FindingCategoryId] INT IDENTITY(1,1) NOT NULL,
    [CategoryName] NVARCHAR(200) NOT NULL,
    [CategoryCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [ParentCategoryId] INT NULL, -- For hierarchical categories
    [Color] NVARCHAR(7) NULL, -- Hex color code for UI display
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_FindingCategories] PRIMARY KEY CLUSTERED ([FindingCategoryId]),
    CONSTRAINT [UK_FindingCategories_CategoryName] UNIQUE ([CategoryName]),
    CONSTRAINT [UK_FindingCategories_CategoryCode] UNIQUE ([CategoryCode]),
    CONSTRAINT [FK_FindingCategories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [FindingCategories]([FindingCategoryId]),
    CONSTRAINT [FK_FindingCategories_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingCategories_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_FindingCategories_CategoryName] ON [dbo].[FindingCategories] ([CategoryName]);
CREATE NONCLUSTERED INDEX [IX_FindingCategories_ParentCategoryId] ON [dbo].[FindingCategories] ([ParentCategoryId]);
CREATE NONCLUSTERED INDEX [IX_FindingCategories_IsActive] ON [dbo].[FindingCategories] ([IsActive]);
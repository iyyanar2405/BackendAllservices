-- NotificationCategories table for categorizing notifications
CREATE TABLE [dbo].[NotificationCategories]
(
    [CategoryId] INT IDENTITY(1,1) NOT NULL,
    [CategoryName] NVARCHAR(100) NOT NULL,
    [CategoryCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Color] NVARCHAR(7) NULL, -- Hex color code for UI display
    [Icon] NVARCHAR(50) NULL, -- Icon name for UI display
    [Priority] INT NULL DEFAULT 5, -- 1-10 priority level
    [DisplayOrder] INT NULL DEFAULT 999,

    CONSTRAINT [PK_NotificationCategories] PRIMARY KEY CLUSTERED ([CategoryId]),
    CONSTRAINT [UK_NotificationCategories_CategoryName] UNIQUE ([CategoryName]),
    CONSTRAINT [UK_NotificationCategories_CategoryCode] UNIQUE ([CategoryCode]),
    CONSTRAINT [FK_NotificationCategories_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_NotificationCategories_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_NotificationCategories_CategoryName] ON [dbo].[NotificationCategories] ([CategoryName]);
CREATE NONCLUSTERED INDEX [IX_NotificationCategories_IsActive] ON [dbo].[NotificationCategories] ([IsActive]);
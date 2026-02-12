-- Chapters table for standard chapters/sections
CREATE TABLE [dbo].[Chapters]
(
    [ChapterId] INT IDENTITY(1,1) NOT NULL,
    [ChapterNumber] NVARCHAR(20) NOT NULL,
    [ChapterTitle] NVARCHAR(500) NOT NULL,
    [Description] NVARCHAR(2000) NULL,
    [StandardId] INT NULL, -- Reference to specific standard (ISO 9001, etc.)
    [ParentChapterId] INT NULL, -- For hierarchical chapters
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [DisplayOrder] INT NULL DEFAULT 999,
    [Level] INT NULL DEFAULT 1, -- Chapter hierarchy level

    CONSTRAINT [PK_Chapters] PRIMARY KEY CLUSTERED ([ChapterId]),
    CONSTRAINT [UK_Chapters_ChapterNumber_Standard] UNIQUE ([ChapterNumber], [StandardId]),
    CONSTRAINT [FK_Chapters_ParentChapterId] FOREIGN KEY ([ParentChapterId]) REFERENCES [Chapters]([ChapterId]),
    CONSTRAINT [FK_Chapters_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Chapters_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Chapters_ChapterNumber] ON [dbo].[Chapters] ([ChapterNumber]);
CREATE NONCLUSTERED INDEX [IX_Chapters_StandardId] ON [dbo].[Chapters] ([StandardId]);
CREATE NONCLUSTERED INDEX [IX_Chapters_ParentChapterId] ON [dbo].[Chapters] ([ParentChapterId]);
CREATE NONCLUSTERED INDEX [IX_Chapters_IsActive] ON [dbo].[Chapters] ([IsActive]);
-- Clauses table for specific clauses within chapters
CREATE TABLE [dbo].[Clauses]
(
    [ClauseId] INT IDENTITY(1,1) NOT NULL,
    [ClauseNumber] NVARCHAR(20) NOT NULL,
    [ClauseTitle] NVARCHAR(500) NOT NULL,
    [ClauseText] NVARCHAR(MAX) NULL,
    [ChapterId] INT NOT NULL,
    [ParentClauseId] INT NULL, -- For sub-clauses
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [DisplayOrder] INT NULL DEFAULT 999,
    [Level] INT NULL DEFAULT 1, -- Clause hierarchy level
    [IsAuditable] BIT NOT NULL DEFAULT 1, -- Whether this clause can have findings

    CONSTRAINT [PK_Clauses] PRIMARY KEY CLUSTERED ([ClauseId]),
    CONSTRAINT [UK_Clauses_ClauseNumber_Chapter] UNIQUE ([ClauseNumber], [ChapterId]),
    CONSTRAINT [FK_Clauses_ChapterId] FOREIGN KEY ([ChapterId]) REFERENCES [Chapters]([ChapterId]),
    CONSTRAINT [FK_Clauses_ParentClauseId] FOREIGN KEY ([ParentClauseId]) REFERENCES [Clauses]([ClauseId]),
    CONSTRAINT [FK_Clauses_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Clauses_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Clauses_ClauseNumber] ON [dbo].[Clauses] ([ClauseNumber]);
CREATE NONCLUSTERED INDEX [IX_Clauses_ChapterId] ON [dbo].[Clauses] ([ChapterId]);
CREATE NONCLUSTERED INDEX [IX_Clauses_ParentClauseId] ON [dbo].[Clauses] ([ParentClauseId]);
CREATE NONCLUSTERED INDEX [IX_Clauses_IsActive] ON [dbo].[Clauses] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Clauses_IsAuditable] ON [dbo].[Clauses] ([IsAuditable]);
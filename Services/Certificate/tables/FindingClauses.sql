-- FindingClauses table for many-to-many relationship between findings and clauses
CREATE TABLE [dbo].[FindingClauses]
(
    [FindingClauseId] INT IDENTITY(1,1) NOT NULL,
    [FindingId] INT NOT NULL,
    [ClauseId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_FindingClauses] PRIMARY KEY CLUSTERED ([FindingClauseId]),
    CONSTRAINT [UK_FindingClauses_Finding_Clause] UNIQUE ([FindingId], [ClauseId]),
    CONSTRAINT [FK_FindingClauses_FindingId] FOREIGN KEY ([FindingId]) REFERENCES [Findings]([FindingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FindingClauses_ClauseId] FOREIGN KEY ([ClauseId]) REFERENCES [Clauses]([ClauseId]),
    CONSTRAINT [FK_FindingClauses_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingClauses_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_FindingClauses_FindingId] ON [dbo].[FindingClauses] ([FindingId]);
CREATE NONCLUSTERED INDEX [IX_FindingClauses_ClauseId] ON [dbo].[FindingClauses] ([ClauseId]);
CREATE NONCLUSTERED INDEX [IX_FindingClauses_IsActive] ON [dbo].[FindingClauses] ([IsActive]);
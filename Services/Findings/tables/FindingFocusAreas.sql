-- FindingFocusAreas table for many-to-many relationship between findings and focus areas
CREATE TABLE [dbo].[FindingFocusAreas]
(
    [FindingFocusAreaId] INT IDENTITY(1,1) NOT NULL,
    [FindingId] INT NOT NULL,
    [FocusAreaId] INT NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Notes] NVARCHAR(500) NULL,

    CONSTRAINT [PK_FindingFocusAreas] PRIMARY KEY CLUSTERED ([FindingFocusAreaId]),
    CONSTRAINT [UK_FindingFocusAreas_Finding_FocusArea] UNIQUE ([FindingId], [FocusAreaId]),
    CONSTRAINT [FK_FindingFocusAreas_FindingId] FOREIGN KEY ([FindingId]) REFERENCES [Findings]([FindingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FindingFocusAreas_FocusAreaId] FOREIGN KEY ([FocusAreaId]) REFERENCES [FocusAreas]([FocusAreaId]),
    CONSTRAINT [FK_FindingFocusAreas_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingFocusAreas_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_FindingFocusAreas_FindingId] ON [dbo].[FindingFocusAreas] ([FindingId]);
CREATE NONCLUSTERED INDEX [IX_FindingFocusAreas_FocusAreaId] ON [dbo].[FindingFocusAreas] ([FocusAreaId]);
CREATE NONCLUSTERED INDEX [IX_FindingFocusAreas_IsActive] ON [dbo].[FindingFocusAreas] ([IsActive]);
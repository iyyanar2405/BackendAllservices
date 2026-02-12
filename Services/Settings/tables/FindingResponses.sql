-- FindingResponses table for responses and updates to findings
CREATE TABLE [dbo].[FindingResponses]
(
    [FindingResponseId] INT IDENTITY(1,1) NOT NULL,
    [FindingId] INT NOT NULL,
    [ResponseText] NVARCHAR(MAX) NOT NULL,
    [ResponseType] NVARCHAR(50) NOT NULL, -- 'Initial Response', 'Update', 'Final Response', 'Verification'
    [ResponseDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [RespondedBy] INT NOT NULL,
    [IsSubmittedToDNV] BIT NOT NULL DEFAULT 0,
    [SubmissionDate] DATETIME NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [AttachmentPath] NVARCHAR(500) NULL,
    [Status] NVARCHAR(50) NULL, -- 'Draft', 'Submitted', 'Under Review', 'Accepted', 'Rejected'
    [ReviewComments] NVARCHAR(MAX) NULL,
    [ReviewedBy] INT NULL,
    [ReviewDate] DATETIME NULL,

    CONSTRAINT [PK_FindingResponses] PRIMARY KEY CLUSTERED ([FindingResponseId]),
    CONSTRAINT [FK_FindingResponses_FindingId] FOREIGN KEY ([FindingId]) REFERENCES [Findings]([FindingId]) ON DELETE CASCADE,
    CONSTRAINT [FK_FindingResponses_RespondedBy] FOREIGN KEY ([RespondedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingResponses_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingResponses_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingResponses_ReviewedBy] FOREIGN KEY ([ReviewedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_FindingResponses_ResponseType] CHECK ([ResponseType] IN ('Initial Response', 'Update', 'Final Response', 'Verification')),
    CONSTRAINT [CK_FindingResponses_Status] CHECK ([Status] IN ('Draft', 'Submitted', 'Under Review', 'Accepted', 'Rejected'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_FindingResponses_FindingId] ON [dbo].[FindingResponses] ([FindingId]);
CREATE NONCLUSTERED INDEX [IX_FindingResponses_RespondedBy] ON [dbo].[FindingResponses] ([RespondedBy]);
CREATE NONCLUSTERED INDEX [IX_FindingResponses_ResponseDate] ON [dbo].[FindingResponses] ([ResponseDate]);
CREATE NONCLUSTERED INDEX [IX_FindingResponses_IsSubmittedToDNV] ON [dbo].[FindingResponses] ([IsSubmittedToDNV]);
CREATE NONCLUSTERED INDEX [IX_FindingResponses_Status] ON [dbo].[FindingResponses] ([Status]);
CREATE NONCLUSTERED INDEX [IX_FindingResponses_IsActive] ON [dbo].[FindingResponses] ([IsActive]);
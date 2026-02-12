-- FindingStatuses table for finding status definitions
CREATE TABLE [dbo].[FindingStatuses]
(
    [FindingStatusId] INT IDENTITY(1,1) NOT NULL,
    [StatusName] NVARCHAR(100) NOT NULL,
    [StatusCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Color] NVARCHAR(7) NULL, -- Hex color code for UI display
    [DisplayOrder] INT NULL DEFAULT 999,
    [IsClosedStatus] BIT NOT NULL DEFAULT 0, -- Indicates if this status means the finding is closed

    CONSTRAINT [PK_FindingStatuses] PRIMARY KEY CLUSTERED ([FindingStatusId]),
    CONSTRAINT [UK_FindingStatuses_StatusName] UNIQUE ([StatusName]),
    CONSTRAINT [UK_FindingStatuses_StatusCode] UNIQUE ([StatusCode]),
    CONSTRAINT [FK_FindingStatuses_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_FindingStatuses_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_FindingStatuses_StatusName] ON [dbo].[FindingStatuses] ([StatusName]);
CREATE NONCLUSTERED INDEX [IX_FindingStatuses_IsActive] ON [dbo].[FindingStatuses] ([IsActive]);
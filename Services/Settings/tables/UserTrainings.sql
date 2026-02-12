-- UserTrainings table for tracking user training completion and progress
CREATE TABLE [dbo].[UserTrainings]
(
    [UserTrainingId] INT IDENTITY(1,1) NOT NULL,
    [UserId] INT NOT NULL,
    [TrainingId] INT NOT NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Not Started', -- 'Not Started', 'In Progress', 'Completed', 'Failed', 'Expired'
    [EnrollmentDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [StartDate] DATETIME NULL,
    [CompletionDate] DATETIME NULL,
    [DueDate] DATETIME NULL,
    [Score] INT NULL, -- Percentage score if assessment taken
    [AttemptCount] INT NOT NULL DEFAULT 0,
    [MaxAttempts] INT NULL DEFAULT 3,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Progress] INT NOT NULL DEFAULT 0, -- Percentage completion (0-100)
    [TimeSpent] INT NULL, -- Time spent in minutes
    [CertificateIssued] BIT NOT NULL DEFAULT 0,
    [CertificateNumber] NVARCHAR(100) NULL,
    [CertificateIssuedDate] DATETIME NULL,
    [CertificateExpiryDate] DATETIME NULL,
    [Notes] NVARCHAR(1000) NULL,

    CONSTRAINT [PK_UserTrainings] PRIMARY KEY CLUSTERED ([UserTrainingId]),
    CONSTRAINT [UK_UserTrainings_User_Training] UNIQUE ([UserId], [TrainingId]),
    CONSTRAINT [FK_UserTrainings_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserTrainings_TrainingId] FOREIGN KEY ([TrainingId]) REFERENCES [Trainings]([TrainingId]),
    CONSTRAINT [FK_UserTrainings_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_UserTrainings_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_UserTrainings_Status] CHECK ([Status] IN ('Not Started', 'In Progress', 'Completed', 'Failed', 'Expired')),
    CONSTRAINT [CK_UserTrainings_Progress] CHECK ([Progress] BETWEEN 0 AND 100),
    CONSTRAINT [CK_UserTrainings_Score] CHECK ([Score] BETWEEN 0 AND 100 OR [Score] IS NULL)
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_UserTrainings_UserId] ON [dbo].[UserTrainings] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_UserTrainings_TrainingId] ON [dbo].[UserTrainings] ([TrainingId]);
CREATE NONCLUSTERED INDEX [IX_UserTrainings_Status] ON [dbo].[UserTrainings] ([Status]);
CREATE NONCLUSTERED INDEX [IX_UserTrainings_DueDate] ON [dbo].[UserTrainings] ([DueDate]);
CREATE NONCLUSTERED INDEX [IX_UserTrainings_CompletionDate] ON [dbo].[UserTrainings] ([CompletionDate]);
CREATE NONCLUSTERED INDEX [IX_UserTrainings_IsActive] ON [dbo].[UserTrainings] ([IsActive]);
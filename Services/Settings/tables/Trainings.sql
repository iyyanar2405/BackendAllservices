-- Trainings table for training programs and courses
CREATE TABLE [dbo].[Trainings]
(
    [TrainingId] INT IDENTITY(1,1) NOT NULL,
    [TrainingName] NVARCHAR(200) NOT NULL,
    [TrainingCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [TrainingType] NVARCHAR(100) NULL, -- 'Online', 'Classroom', 'Hybrid', 'Self-Paced'
    [Category] NVARCHAR(100) NULL,
    [Duration] INT NULL, -- Duration in hours
    [DueDate] DATETIME NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Prerequisites] NVARCHAR(1000) NULL,
    [LearningObjectives] NVARCHAR(MAX) NULL,
    [Materials] NVARCHAR(MAX) NULL, -- JSON array of materials/resources
    [AssessmentRequired] BIT NOT NULL DEFAULT 0,
    [PassingScore] INT NULL, -- Percentage required to pass
    [CertificateIssued] BIT NOT NULL DEFAULT 0,
    [ValidityPeriod] INT NULL, -- Validity in months
    [Cost] DECIMAL(10,2) NULL,
    [Currency] NVARCHAR(3) NULL DEFAULT 'USD',

    CONSTRAINT [PK_Trainings] PRIMARY KEY CLUSTERED ([TrainingId]),
    CONSTRAINT [UK_Trainings_TrainingCode] UNIQUE ([TrainingCode]),
    CONSTRAINT [UK_Trainings_TrainingName] UNIQUE ([TrainingName]),
    CONSTRAINT [FK_Trainings_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Trainings_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Trainings_TrainingName] ON [dbo].[Trainings] ([TrainingName]);
CREATE NONCLUSTERED INDEX [IX_Trainings_TrainingType] ON [dbo].[Trainings] ([TrainingType]);
CREATE NONCLUSTERED INDEX [IX_Trainings_Category] ON [dbo].[Trainings] ([Category]);
CREATE NONCLUSTERED INDEX [IX_Trainings_DueDate] ON [dbo].[Trainings] ([DueDate]);
CREATE NONCLUSTERED INDEX [IX_Trainings_IsActive] ON [dbo].[Trainings] ([IsActive]);
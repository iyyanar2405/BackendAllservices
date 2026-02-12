-- ErrorLogs table for system error logging
CREATE TABLE [dbo].[ErrorLogs]
(
    [ErrorLogId] INT IDENTITY(1,1) NOT NULL,
    [ErrorMessage] NVARCHAR(MAX) NOT NULL,
    [ErrorType] NVARCHAR(100) NULL, -- 'Database', 'Application', 'Authentication', 'Validation', 'Network'
    [Severity] NVARCHAR(50) NOT NULL DEFAULT 'Error', -- 'Info', 'Warning', 'Error', 'Critical'
    [Source] NVARCHAR(200) NULL, -- Source component/module
    [StackTrace] NVARCHAR(MAX) NULL,
    [UserId] INT NULL,
    [SessionId] NVARCHAR(100) NULL,
    [IPAddress] NVARCHAR(45) NULL,
    [UserAgent] NVARCHAR(500) NULL,
    [RequestUrl] NVARCHAR(500) NULL,
    [RequestMethod] NVARCHAR(10) NULL, -- GET, POST, PUT, DELETE
    [RequestBody] NVARCHAR(MAX) NULL,
    [ErrorCode] NVARCHAR(50) NULL,
    [InnerException] NVARCHAR(MAX) NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [MachineName] NVARCHAR(100) NULL,
    [ProcessId] INT NULL,
    [ThreadId] INT NULL,
    [ApplicationName] NVARCHAR(100) NULL DEFAULT 'CustomerPortal',
    [Environment] NVARCHAR(50) NULL DEFAULT 'Production', -- 'Development', 'Testing', 'Staging', 'Production'
    [CorrelationId] NVARCHAR(100) NULL, -- For tracking related requests
    [AdditionalData] NVARCHAR(MAX) NULL, -- JSON for additional context

    CONSTRAINT [PK_ErrorLogs] PRIMARY KEY CLUSTERED ([ErrorLogId]),
    CONSTRAINT [FK_ErrorLogs_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users]([UserId]),
    CONSTRAINT [CK_ErrorLogs_Severity] CHECK ([Severity] IN ('Info', 'Warning', 'Error', 'Critical'))
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_ErrorLogs_CreatedDate] ON [dbo].[ErrorLogs] ([CreatedDate]);
CREATE NONCLUSTERED INDEX [IX_ErrorLogs_Severity] ON [dbo].[ErrorLogs] ([Severity]);
CREATE NONCLUSTERED INDEX [IX_ErrorLogs_ErrorType] ON [dbo].[ErrorLogs] ([ErrorType]);
CREATE NONCLUSTERED INDEX [IX_ErrorLogs_UserId] ON [dbo].[ErrorLogs] ([UserId]);
CREATE NONCLUSTERED INDEX [IX_ErrorLogs_Source] ON [dbo].[ErrorLogs] ([Source]);
CREATE NONCLUSTERED INDEX [IX_ErrorLogs_CorrelationId] ON [dbo].[ErrorLogs] ([CorrelationId]);
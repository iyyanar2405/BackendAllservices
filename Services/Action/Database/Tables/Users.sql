-- Users table for customer portal authentication and user management
CREATE TABLE [dbo].[Users]
(
    [UserId] INT IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastLoginDate] DATETIME NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ModifiedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [CreatedBy] INT NULL,
    [ModifiedBy] INT NULL,
    [Phone] NVARCHAR(20) NULL,
    [Position] NVARCHAR(100) NULL,
    [Department] NVARCHAR(100) NULL,
    [TimeZone] NVARCHAR(50) NULL DEFAULT 'UTC',
    [Language] NVARCHAR(10) NULL DEFAULT 'EN',
    [IsEmailVerified] BIT NOT NULL DEFAULT 0,
    [EmailVerificationToken] NVARCHAR(255) NULL,
    [PasswordResetToken] NVARCHAR(255) NULL,
    [PasswordResetExpiry] DATETIME NULL,
    [TwoFactorEnabled] BIT NOT NULL DEFAULT 0,
    [TwoFactorSecret] NVARCHAR(100) NULL,

    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId]),
    CONSTRAINT [UK_Users_Username] UNIQUE ([Username]),
    CONSTRAINT [UK_Users_Email] UNIQUE ([Email]),
    CONSTRAINT [FK_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([UserId]),
    CONSTRAINT [FK_Users_ModifiedBy] FOREIGN KEY ([ModifiedBy]) REFERENCES [Users]([UserId])
);

-- Indexes for performance
CREATE NONCLUSTERED INDEX [IX_Users_Email] ON [dbo].[Users] ([Email]);
CREATE NONCLUSTERED INDEX [IX_Users_Username] ON [dbo].[Users] ([Username]);
CREATE NONCLUSTERED INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive]);
CREATE NONCLUSTERED INDEX [IX_Users_LastLoginDate] ON [dbo].[Users] ([LastLoginDate]);
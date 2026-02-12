-- Create Categories table
CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(256) NOT NULL UNIQUE,
    Description NVARCHAR(MAX),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Create index on IsActive for filtering
CREATE INDEX IX_Categories_IsActive ON Categories(IsActive);

-- Create index on Name for searching
CREATE INDEX IX_Categories_Name ON Categories(Name);

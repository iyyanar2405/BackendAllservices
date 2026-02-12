CREATE TABLE Audits (
    auditId INT PRIMARY KEY,
    sites NVARCHAR(MAX),      -- Store as comma-separated values or use a related table for normalization
    services NVARCHAR(MAX),   -- Same as above
    companyId INT,
    status NVARCHAR(50),
    startDate DATETIME,
    endDate DATETIME,
    leadAuditor NVARCHAR(100),
    type NVARCHAR(50)
);
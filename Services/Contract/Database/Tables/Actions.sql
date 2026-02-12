CREATE TABLE Actions (
    id INT PRIMARY KEY,
    action NVARCHAR(255),
    dueDate DATETIME,
    highPriority BIT,
    message NVARCHAR(MAX),
    language NVARCHAR(50),
    service NVARCHAR(100),
    site NVARCHAR(100),
    entityType NVARCHAR(100),
    entityId INT,
    subject NVARCHAR(255),
    snowLink NVARCHAR(255)
);
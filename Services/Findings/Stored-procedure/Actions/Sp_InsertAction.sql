CREATE PROCEDURE Sp_InsertAction
    @id INT,
    @action NVARCHAR(255),
    @dueDate DATETIME,
    @highPriority BIT,
    @message NVARCHAR(MAX),
    @language NVARCHAR(50),
    @service NVARCHAR(100),
    @site NVARCHAR(100),
    @entityType NVARCHAR(100),
    @entityId INT,
    @subject NVARCHAR(255),
    @snowLink NVARCHAR(255)
AS
BEGIN
    INSERT INTO Actions (
        id, action, dueDate, highPriority, message, language, service, site, entityType, entityId, subject, snowLink
    )
    VALUES (
        @id, @action, @dueDate, @highPriority, @message, @language, @service, @site, @entityType, @entityId, @subject, @snowLink
    );
END
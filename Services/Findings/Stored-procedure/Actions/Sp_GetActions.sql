CREATE PROCEDURE Sp_GetActions
    @category NVARCHAR(MAX) = NULL,        -- JSON array of category IDs
    @company NVARCHAR(MAX) = NULL,         -- JSON array of company IDs
    @service NVARCHAR(MAX) = NULL,         -- JSON array of service IDs
    @site NVARCHAR(MAX) = NULL,            -- JSON array of site IDs
    @isHighPriority BIT = 0,               -- Filter for high priority actions
    @pageNumber INT = 1,                   -- Page number (1-based)
    @pageSize INT = 10                     -- Number of items per page
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate pagination parameters
    IF @pageNumber < 1 SET @pageNumber = 1;
    IF @pageSize < 1 SET @pageSize = 10;
    IF @pageSize > 100 SET @pageSize = 100; -- Limit max page size
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @category IS NOT NULL AND @category != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@category);
    END
    
    -- Create temp table for company filter if provided
    DECLARE @companyFilter TABLE (companyId INT);
    IF @company IS NOT NULL AND @company != '[]'
    BEGIN
        INSERT INTO @companyFilter (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@company);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceId INT);
    IF @service IS NOT NULL AND @service != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@service);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilter TABLE (siteId INT);
    IF @site IS NOT NULL AND @site != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@site);
    END
    
    -- Calculate pagination
    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;
    DECLARE @totalItems INT;
    
    -- Main query with filtering
    WITH FilteredActions AS (
        SELECT DISTINCT a.*
        FROM Actions a
        LEFT JOIN Audits au ON a.entityId = au.auditId
        LEFT JOIN AuditServices aser ON aser.auditId = au.auditId
        LEFT JOIN Services s ON s.ServiceId = aser.ServiceId
        LEFT JOIN AuditSites asit ON asit.auditId = au.auditId
        WHERE 1=1
            -- Filter by categories if provided (based on entityType)
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType IN ('Certificate', 'certificates'))
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType IN ('Finding', 'findings'))
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType IN ('Schedule', 'schedule'))
                )
            )
            -- Filter by companies if provided
            AND (
                (SELECT COUNT(*) FROM @companyFilter) = 0
                OR au.companyId IN (SELECT companyId FROM @companyFilter)
            )
            -- Filter by services if provided
            AND (
                (SELECT COUNT(*) FROM @serviceFilter) = 0
                OR s.ServiceId IN (SELECT serviceId FROM @serviceFilter)
            )
            -- Filter by sites if provided
            AND (
                (SELECT COUNT(*) FROM @siteFilter) = 0
                OR asit.siteId IN (SELECT siteId FROM @siteFilter)
            )
            -- Filter by high priority if specified
            AND (
                @isHighPriority = 0 
                OR (@isHighPriority = 1 AND a.highPriority = 1)
            )
    )
    
    -- Get total count for pagination
    SELECT @totalItems = COUNT(*) FROM FilteredActions;
    
    -- Calculate total pages
    DECLARE @totalPages INT = CEILING(CAST(@totalItems AS FLOAT) / @pageSize);
    
    -- Return paginated results as table
    SELECT 
        a.action,
        a.dueDate,
        a.highPriority,
        a.id,
        a.message,
        a.language,
        a.service,
        a.site,
        a.entityType,
        CAST(a.entityId AS NVARCHAR(50)) AS entityId,
        a.subject,
        a.snowLink,
        @pageNumber AS currentPage,
        @totalItems AS totalItems,
        @totalPages AS totalPages
    FROM FilteredActions a
    ORDER BY 
        CASE WHEN a.dueDate IS NULL THEN 1 ELSE 0 END,  -- NULL dates last
        a.dueDate ASC,                                  -- Earliest dates first
        a.id DESC                                       -- Most recent IDs first for ties
    OFFSET @offset ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END

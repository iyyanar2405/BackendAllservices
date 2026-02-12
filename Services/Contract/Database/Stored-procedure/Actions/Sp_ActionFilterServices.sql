CREATE PROCEDURE Sp_ActionFilterServices
    @companies NVARCHAR(MAX) = NULL,   -- JSON array of company IDs
    @categories NVARCHAR(MAX) = NULL,  -- JSON array of category IDs
    @sites NVARCHAR(MAX) = NULL        -- JSON array of site IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create temp table for company filter if provided
    DECLARE @companyFilter TABLE (companyId INT);
    IF @companies IS NOT NULL AND @companies != '[]'
    BEGIN
        INSERT INTO @companyFilter (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@companies);
    END
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @categories IS NOT NULL AND @categories != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@categories);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilter TABLE (siteId INT);
    IF @sites IS NOT NULL AND @sites != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@sites);
    END
    
    WITH ServicesData AS (
        SELECT ServiceId AS id, ServiceName AS label
        FROM Services
    ),
    
    -- Get filtered services based on actions that match the criteria
    FilteredServices AS (
        SELECT DISTINCT aser.ServiceId
        FROM Audits au
        LEFT JOIN Actions a ON a.entityId = au.auditId
        LEFT JOIN AuditServices aser ON aser.AuditId = au.AuditId
        WHERE 1=1
            -- Filter by companies if provided
            AND (
                (SELECT COUNT(*) FROM @companyFilter) = 0
                OR au.companyId IN (SELECT companyId FROM @companyFilter)
            )
            -- Filter by categories if provided (based on action entityType)
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType = 'Certificate')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType = 'Finding')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType = 'Schedule')
                )
            )
            -- Filter by sites if provided
            AND (
                (SELECT COUNT(*) FROM @siteFilter) = 0
                OR EXISTS (SELECT 1 FROM AuditSites asit 
                    WHERE asit.AuditId = au.AuditId 
                    AND asit.SiteId IN (SELECT siteId FROM @siteFilter))
            )
    )
    
    -- Return services that have actions matching the filter criteria
    SELECT 
        sd.id,
        sd.label
    FROM ServicesData sd
    WHERE EXISTS (SELECT 1 FROM FilteredServices fs WHERE fs.ServiceId = sd.id)
    OR (
        (SELECT COUNT(*) FROM @companyFilter) = 0 
        AND (SELECT COUNT(*) FROM @categoryFilter) = 0
        AND (SELECT COUNT(*) FROM @siteFilter) = 0
        AND EXISTS (SELECT 1 FROM AuditServices aser WHERE aser.ServiceId = sd.id)
    )
    ORDER BY sd.label;
END

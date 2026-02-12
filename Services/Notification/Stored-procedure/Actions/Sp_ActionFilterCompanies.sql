CREATE PROCEDURE Sp_ActionFilterCompanies
    @categories NVARCHAR(MAX) = NULL,  -- JSON array of category IDs
    @services NVARCHAR(MAX) = NULL,    -- JSON array of service IDs
    @sites NVARCHAR(MAX) = NULL        -- JSON array of site IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @categories IS NOT NULL AND @categories != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@categories);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceId INT);
    IF @services IS NOT NULL AND @services != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@services);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilter TABLE (siteId INT);
    IF @sites IS NOT NULL AND @sites != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@sites);
    END
    
    -- Companies data (this should ideally come from a Companies table)
    -- For now, using a CTE with sample data matching the response
    WITH CompaniesData AS (
        SELECT CompanyId AS id, CompanyName AS label
        FROM Companies
    ),
    
    -- Get filtered companies based on actions that match the criteria
    FilteredCompanies AS (
        SELECT DISTINCT au.companyId
        FROM Audits au
        INNER JOIN Actions a ON a.entityId = au.auditId
        LEFT JOIN AuditServices aser ON aser.AuditId = au.AuditId
        WHERE 1=1
            -- Filter by categories if provided (based on action entityType)
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType = 'Certificate')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType = 'Finding')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType = 'Schedule')
                )
            )
            -- Filter by services if provided
            AND (
                (SELECT COUNT(*) FROM @serviceFilter) = 0
                OR aser.ServiceId IN (SELECT serviceId FROM @serviceFilter)
            )
            -- Filter by sites if provided
            AND (
                (SELECT COUNT(*) FROM @siteFilter) = 0
                OR EXISTS (SELECT 1 FROM AuditSites asit 
                    WHERE asit.AuditId = au.AuditId 
                    AND asit.SiteId IN (SELECT siteId FROM @siteFilter))
            )
    )
    
    -- Return companies that have actions matching the filter criteria
    SELECT 
        cd.id,
        cd.label
    FROM CompaniesData cd
    WHERE EXISTS (SELECT 1 FROM FilteredCompanies fc WHERE fc.companyId = cd.id)
    OR (
        (SELECT COUNT(*) FROM @categoryFilter) = 0 
        AND (SELECT COUNT(*) FROM @serviceFilter) = 0
        AND (SELECT COUNT(*) FROM @siteFilter) = 0
        AND EXISTS (SELECT 1 FROM Audits WHERE companyId = cd.id)
    )
    ORDER BY cd.label;
END

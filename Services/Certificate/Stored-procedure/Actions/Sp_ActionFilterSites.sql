CREATE PROCEDURE Sp_ActionFilterSites
    @companies NVARCHAR(MAX) = NULL,   -- JSON array of company IDs
    @categories NVARCHAR(MAX) = NULL,  -- JSON array of category IDs
    @services NVARCHAR(MAX) = NULL     -- JSON array of service IDs
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
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceId INT);
    IF @services IS NOT NULL AND @services != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@services);
    END
    
    ;WITH FilteredAudits AS (
        SELECT DISTINCT au.AuditId
        FROM Audits au
        LEFT JOIN Actions a ON a.entityId = au.AuditId
        LEFT JOIN AuditServices aser ON aser.AuditId = au.AuditId
        WHERE 1=1
            AND (
                (SELECT COUNT(*) FROM @companyFilter) = 0
                OR au.companyId IN (SELECT companyId FROM @companyFilter)
            )
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType = 'Certificate')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType = 'Finding')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType = 'Schedule')
                )
            )
            AND (
                (SELECT COUNT(*) FROM @serviceFilter) = 0
                OR aser.ServiceId IN (SELECT serviceId FROM @serviceFilter)
            )
    )
    SELECT
        co.CountryId AS CountryId,
        co.CountryName AS CountryName,
        ci.CityId AS CityId,
        ci.CityName AS CityName,
        s.SiteId AS SiteId,
        s.SiteName AS SiteName
    FROM Sites s
    INNER JOIN Cities ci ON ci.CityId = s.CityId
    INNER JOIN Countries co ON co.CountryId = s.CountryId
    INNER JOIN AuditSites ast ON ast.SiteId = s.SiteId
    INNER JOIN FilteredAudits fa ON fa.AuditId = ast.AuditId
    WHERE
        (SELECT COUNT(*) FROM @companyFilter) = 0
        OR s.CompanyId IN (SELECT companyId FROM @companyFilter)
    ORDER BY co.CountryName, ci.CityName, s.SiteName;
END


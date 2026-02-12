CREATE PROCEDURE Sp_GetAuditDaysGrid
    @startDate DATETIME,                   -- Start date for filtering
    @endDate DATETIME,                     -- End date for filtering
    @companies NVARCHAR(MAX) = NULL,       -- JSON array of company IDs
    @services NVARCHAR(MAX) = NULL,        -- JSON array of service names
    @sites NVARCHAR(MAX) = NULL            -- JSON array of site IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate date parameters
    IF @startDate IS NULL OR @endDate IS NULL
    BEGIN
        SELECT 
            NULL AS data,
            CAST(0 AS BIT) AS isSuccess,
            'Start date and end date are required' AS message,
            'INVALID_DATES' AS errorCode;
        RETURN;
    END
    
    -- Create temp table for company filter if provided
    DECLARE @companyFilter TABLE (companyId INT);
    IF @companies IS NOT NULL AND @companies != '[]'
    BEGIN
        INSERT INTO @companyFilter (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@companies);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceName NVARCHAR(100));
    IF @services IS NOT NULL AND @services != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceName)
        SELECT CAST(value AS NVARCHAR(100))
        FROM OPENJSON(@services);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilterTable TABLE (siteId INT);
    IF @sites IS NOT NULL AND @sites != '[]'
    BEGIN
        INSERT INTO @siteFilterTable (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@sites);
    END
    
    -- Sample hierarchical data based on your response structure
    -- In production, this would be built dynamically from your database tables
    WITH SiteData AS SELECT a.auditId,
            a.companyId,
            a.startDate,
            a.endDate,
            -- Calculate audit days
            CASE 
                WHEN a.endDate IS NOT NULL AND a.startDate IS NOT NULL 
                THEN CAST(DATEDIFF(DAY, a.startDate, a.endDate) + 1 AS DECIMAL(10,2))
                ELSE 1.0
            END AS auditDays,
            -- Extract location data (this would come from proper tables in production)
            CASE 
                WHEN asit.siteId IS NOT NULL THEN asit.siteId
                ELSE CAST(a.site AS INT) -- fallback if site is stored as string ID
            END AS siteId,
            COALESCE(a.site, 'Unknown Site') AS siteName
        FROM Audits a
        LEFT JOIN AuditSites asit ON asit.auditId = a.auditId
        LEFT JOIN AuditServices aser ON aser.auditId = a.auditId
        WHERE 1=1
            -- Filter by date range
            AND a.startDate >= @startDate
            AND a.startDate <= @endDate
            -- Filter by companies if provided
            AND (
                SELECT COUNT(*) FROM @companyFilter) = 0
                OR a.companyId IN SELECT companyId FROM @companyFilter)
            )
            -- Filter by services if provided
            AND (
                SELECT COUNT(*) FROM @serviceFilter) = 0
                OR aser.service IN SELECT serviceName FROM @serviceFilter)
            )
            -- Filter by sites if provided
            AND (
                SELECT COUNT(*) FROM @siteFilterTable) = 0
                OR asit.siteId IN SELECT siteId FROM @siteFilterTable)
            )
    ),
    
    -- Aggregate audit days per site (would be enhanced with proper Country/City mapping)
    SiteAggregation AS SELECT siteId,
            siteName,
            SUM(auditDays) AS totalAuditDays,
            -- In production, you'd join with proper location tables
            CASE 
                WHEN siteName LIKE '%Kerry%' OR siteName LIKE '%Albany%' THEN 'Albany'
                WHEN siteName LIKE '%Alton%' THEN 'Alton'
                WHEN siteName LIKE '%Austin%' OR siteName LIKE '%Mott MacDonald%' THEN 'Austin'
                WHEN siteName LIKE '%Beloit%' THEN 'Beloit'
                ELSE 'Unknown City'
            END AS cityName,
            -- Country mapping (simplified for demo)
            CASE 
                WHEN siteName LIKE '%Kerry%' OR siteName LIKE '%Mott MacDonald%' THEN 'USA'
                WHEN siteName LIKE '%Italy%' OR siteName LIKE '%Milano%' THEN 'Italy'
                WHEN siteName LIKE '%Brazil%' THEN 'Brazil'
                ELSE 'USA' -- default
            END AS countryName,
            CASE 
                WHEN siteName LIKE '%Kerry%' OR siteName LIKE '%Mott MacDonald%' THEN 255
                WHEN siteName LIKE '%Italy%' OR siteName LIKE '%Milano%' THEN 357
                WHEN siteName LIKE '%Brazil%' THEN 285
                ELSE 255 -- default USA ID
            END AS countryId
        FROM SiteData
        WHERE siteId IS NOT NULL
        GROUP BY siteId, siteName
    ),
    
    -- Aggregate by city
    CityAggregation AS SELECT countryId,
            countryName,
            cityName,
            SUM(totalAuditDays) AS cityAuditDays
        FROM SiteAggregation
        GROUP BY countryId, countryName, cityName
    ),
    
    -- Aggregate by country
    CountryAggregation AS SELECT countryId,
            countryName,
            SUM(cityAuditDays) AS countryAuditDays
        FROM CityAggregation
        GROUP BY countryId, countryName
    )
    
    -- Generate hierarchical JSON structure
    SELECT ca.countryId AS id,
                        ca.countryName AS name,
                        ca.countryAuditDays AS auditDays,
                        'Country' AS dataType,
                        SELECT cag.cityName AS name,
                                        cag.cityAuditDays AS auditDays,
                                        'City' AS dataType
                                SELECT sa.siteId AS id,
                                                sa.siteName AS name,
                                                sa.totalAuditDays AS auditDays,
                                                'Site' AS dataType
                                        
                                    FROM SiteAggregation sa
                                    WHERE sa.countryId = ca.countryId 
                                      AND sa.cityName = cag.cityName
                                ) AS children
                            FROM CityAggregation cag
                            WHERE cag.countryId = ca.countryId
                        ) AS children
                    FROM CountryAggregation ca
                
        CAST(1 AS BIT) AS isSuccess;
END


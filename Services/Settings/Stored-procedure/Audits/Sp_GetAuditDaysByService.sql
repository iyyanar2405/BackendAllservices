CREATE PROCEDURE Sp_GetAuditDaysByService
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
            'INVALID_DATES' AS errorCode,
            CAST(0 AS BIT) AS isSuccess,
            'Start date and end date are required' AS message;
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
    DECLARE @siteFilter TABLE (siteId INT);
    IF @sites IS NOT NULL AND @sites != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@sites);
    END
    
    -- Main query to aggregate audit days by service
    WITH FilteredAudits AS SELECT a.auditId,
            a.companyId,
            a.startDate,
            a.endDate,
            -- Calculate audit days (difference between start and end date + 1)
            CASE 
                WHEN a.endDate IS NOT NULL AND a.startDate IS NOT NULL 
                THEN CAST(DATEDIFF(DAY, a.startDate, a.endDate) + 1 AS DECIMAL(10,2))
                ELSE 1.0 -- Default to 1 day if dates are missing
            END AS auditDays
        FROM Audits a
        WHERE 1=1
            -- Filter by date range
            AND a.startDate >= @startDate
            AND a.startDate <= @endDate
            -- Filter by companies if provided
            AND (
                SELECT COUNT(*) FROM @companyFilter) = 0
                OR a.companyId IN SELECT companyId FROM @companyFilter)
            )
            -- Filter by sites if provided
            AND (
                SELECT COUNT(*) FROM @siteFilter) = 0
                OR EXISTS SELECT 1 FROM AuditSites asit 
                    WHERE asit.auditId = a.auditId 
                    AND asit.siteId IN SELECT siteId FROM @siteFilter)
                )
            )
    ),
    
    AuditWithServices AS SELECT fa.auditId,
            fa.auditDays,
            COALESCE(aser.service, 'Unknown Service') AS serviceName
        FROM FilteredAudits fa
        LEFT JOIN AuditServices aser ON aser.auditId = fa.auditId
        WHERE 1=1
            -- Filter by services if provided
            AND (
                SELECT COUNT(*) FROM @serviceFilter) = 0
                OR aser.service IN SELECT serviceName FROM @serviceFilter)
                OR @services IS NULL OR @services = '[]'
            )
    ),
    
    ServiceAggregation AS SELECT serviceName,
            SUM(auditDays) AS totalAuditDays
        FROM AuditWithServices
        GROUP BY serviceName
    ),
    
    TotalCalculation AS SELECT SUM(totalAuditDays) AS grandTotal
        FROM ServiceAggregation
    )
    
    -- Return the aggregated data in the required JSON structure
    SELECT sa.totalAuditDays AS auditDays,
                        CASE 
                            WHEN tc.grandTotal > 0 
                            THEN CAST(ROUND((sa.totalAuditDays / tc.grandTotal) * 100, 0) AS INT)
                            ELSE 0 
                        END AS auditpercentage,
                        sa.serviceName
                    FROM ServiceAggregation sa
                    CROSS JOIN TotalCalculation tc
                    WHERE sa.totalAuditDays > 0  -- Only include services with audit days
                    ORDER BY sa.totalAuditDays DESC, sa.serviceName
                ) AS pieChartData,
                tc.grandTotal AS totalServiceAuditsDayCount
            FROM TotalCalculation tc
        '' AS errorCode,
        CAST(1 AS BIT) AS isSuccess;
END


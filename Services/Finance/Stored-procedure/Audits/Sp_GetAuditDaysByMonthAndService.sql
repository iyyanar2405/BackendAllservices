CREATE PROCEDURE Sp_GetAuditDaysByMonthAndService
    @startDate DATETIME,                   -- Start date for filtering
    @endDate DATETIME,                     -- End date for filtering
    @companyFilter NVARCHAR(MAX) = NULL,   -- JSON array of company IDs
    @serviceFilter NVARCHAR(MAX) = NULL,   -- JSON array of service IDs
    @siteFilter NVARCHAR(MAX) = NULL       -- JSON array of site IDs
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
    DECLARE @companyFilterTable TABLE (companyId INT);
    IF @companyFilter IS NOT NULL AND @companyFilter != '[]'
    BEGIN
        INSERT INTO @companyFilterTable (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@companyFilter);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilterTable TABLE (serviceName NVARCHAR(100));
    IF @serviceFilter IS NOT NULL AND @serviceFilter != '[]'
    BEGIN
        INSERT INTO @serviceFilterTable (serviceName)
        SELECT CAST(value AS NVARCHAR(100))
        FROM OPENJSON(@serviceFilter);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilterTable TABLE (siteId INT);
    IF @siteFilter IS NOT NULL AND @siteFilter != '[]'
    BEGIN
        INSERT INTO @siteFilterTable (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@siteFilter);
    END
    
    -- Main query to aggregate audit days by month and service
    WITH FilteredAudits AS SELECT a.auditId,
            a.companyId,
            a.startDate,
            a.endDate,
            a.status,
            -- Calculate audit days (difference between start and end date + 1)
            CASE 
                WHEN a.endDate IS NOT NULL AND a.startDate IS NOT NULL 
                THEN CAST(DATEDIFF(DAY, a.startDate, a.endDate) + 1 AS DECIMAL(10,2))
                ELSE 1.0 -- Default to 1 day if dates are missing
            END AS auditDays,
            DATENAME(MONTH, a.startDate) AS monthName,
            MONTH(a.startDate) AS monthNumber
        FROM Audits a
        WHERE 1=1
            -- Filter by date range
            AND a.startDate >= @startDate
            AND a.startDate <= @endDate
            -- Filter by companies if provided
            AND (
                SELECT COUNT(*) FROM @companyFilterTable) = 0
                OR a.companyId IN SELECT companyId FROM @companyFilterTable)
            )
            -- Filter by sites if provided
            AND (
                SELECT COUNT(*) FROM @siteFilterTable) = 0
                OR EXISTS SELECT 1 FROM AuditSites asit 
                    WHERE asit.auditId = a.auditId 
                    AND asit.siteId IN SELECT siteId FROM @siteFilterTable)
                )
            )
    ),
    
    AuditWithServices AS SELECT fa.auditId,
            fa.companyId,
            fa.startDate,
            fa.endDate,
            fa.auditDays,
            fa.monthName,
            fa.monthNumber,
            COALESCE(aser.service, 'Unknown Service') AS serviceName
        FROM FilteredAudits fa
        LEFT JOIN AuditServices aser ON aser.auditId = fa.auditId
        WHERE 1=1
            -- Filter by services if provided
            AND (
                SELECT COUNT(*) FROM @serviceFilterTable) = 0
                OR aser.service IN SELECT serviceName FROM @serviceFilterTable)
            )
    ),
    
    MonthServiceAggregation AS SELECT monthName,
            monthNumber,
            serviceName,
            SUM(auditDays) AS totalAuditDays
        FROM AuditWithServices
        GROUP BY monthName, monthNumber, serviceName
    ),
    
    MonthTotals AS SELECT monthName,
            monthNumber,
            SUM(totalAuditDays) AS monthTotal
        FROM MonthServiceAggregation
        GROUP BY monthName, monthNumber
    )
    
    -- Return the aggregated data in the required JSON structure
    SELECT mt.monthName AS month,
                        mt.monthTotal AS monthCount,
                        SELECT msa.totalAuditDays AS auditDays,
                                msa.serviceName
                            FROM MonthServiceAggregation msa
                            WHERE msa.monthNumber = mt.monthNumber
                            ORDER BY msa.totalAuditDays DESC, msa.serviceName
                        ) AS serviceData
                    FROM MonthTotals mt
                    ORDER BY mt.monthNumber
                ) AS chartData
        '' AS errorCode,
        CAST(1 AS BIT) AS isSuccess;
END


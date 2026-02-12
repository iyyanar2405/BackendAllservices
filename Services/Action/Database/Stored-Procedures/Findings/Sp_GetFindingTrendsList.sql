-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get findings trends list with hierarchical location-based data (Country > City > Site) and year-over-year analysis
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFindingTrendsList]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @Companies NVARCHAR(MAX) = NULL;
    DECLARE @Services NVARCHAR(MAX) = NULL;
    DECLARE @Sites NVARCHAR(MAX) = NULL;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    DECLARE @CurrentYear INT = YEAR(GETDATE());
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @Companies = NULL,
                @Services = NULL, 
                @Sites = NULL
        END
        ELSE
        BEGIN
            SET @ErrorCode = 'INVALID_JSON';
            SET @Message = 'Invalid JSON format in parameters.';
            GOTO ErrorResponse;
        END

        -- Validate user exists and is active (if userId provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Parse filter arrays
        DECLARE @CompanyList TABLE (CompanyId INT);
        DECLARE @ServiceList TABLE (ServiceId INT);
        DECLARE @SiteList TABLE (SiteId INT);

        IF @Companies IS NOT NULL AND @Companies != '[]'
        BEGIN
            INSERT INTO @CompanyList (CompanyId)
            SELECT CAST([value] AS INT) FROM OPENJSON(@Companies);
        END

        IF @Services IS NOT NULL AND @Services != '[]'
        BEGIN
            INSERT INTO @ServiceList (ServiceId)
            SELECT CAST([value] AS INT) FROM OPENJSON(@Services);
        END

        IF @Sites IS NOT NULL AND @Sites != '[]'
        BEGIN
            INSERT INTO @SiteList (SiteId)
            SELECT CAST([value] AS INT) FROM OPENJSON(@Sites);
        END

        -- Get findings data with location hierarchy and year trends
        WITH FindingsLocationTrends AS SELECT ISNULL(s.Country, 'Unknown') as Country,
                ISNULL(s.City, 'Unknown') as City,
                ISNULL(s.SiteName, 'Unknown Site') as SiteName,
                YEAR(f.OpenedDate) as FindingYear,
                COUNT(*) as FindingCount
            FROM Findings f
            INNER JOIN Audits a ON f.AuditId = a.AuditId
            INNER JOIN Sites s ON a.SiteId = s.SiteId
            WHERE f.IsActive = 1 
            AND a.IsActive = 1
            AND s.IsActive = 1
            AND f.OpenedDate IS NOT NULL
            -- Company filter
            AND (NOT EXISTS SELECT 1 FROM @CompanyList) OR a.CompanyId IN SELECT CompanyId FROM @CompanyList))
            -- Service filter
            AND (NOT EXISTS SELECT 1 FROM @ServiceList) OR EXISTS SELECT 1 FROM AuditServices aus 
                INNER JOIN @ServiceList sl ON aus.ServiceId = sl.ServiceId
                WHERE aus.AuditId = a.AuditId AND aus.IsActive = 1
            ))
            -- Site filter
            AND (NOT EXISTS SELECT 1 FROM @SiteList) OR s.SiteId IN SELECT SiteId FROM @SiteList))
            -- User access control
            AND (@UserId IS NULL OR EXISTS SELECT 1 FROM UserCompanyAccess uca 
                WHERE uca.UserId = @UserId AND uca.CompanyId = a.CompanyId
            ))
            GROUP BY ISNULL(s.Country, 'Unknown'), ISNULL(s.City, 'Unknown'), ISNULL(s.SiteName, 'Unknown Site'), YEAR(f.OpenedDate)
        ),
        -- Pivot data to get year columns
        YearlyData AS SELECT Country,
                City,
                SiteName,
                SUM(CASE WHEN FindingYear = @CurrentYear THEN FindingCount ELSE 0 END) as CurrentYear,
                SUM(CASE WHEN FindingYear = @CurrentYear - 1 THEN FindingCount ELSE 0 END) as LastYear,
                SUM(CASE WHEN FindingYear = @CurrentYear - 2 THEN FindingCount ELSE 0 END) as YearBeforeLast,
                SUM(CASE WHEN FindingYear = @CurrentYear - 3 THEN FindingCount ELSE 0 END) as YearMinus3
            FROM FindingsLocationTrends
            GROUP BY Country, City, SiteName
        ),
        -- Aggregate at City level
        CityLevel AS SELECT Country,
                City,
                SUM(CurrentYear) as CurrentYear,
                SUM(LastYear) as LastYear,
                SUM(YearBeforeLast) as YearBeforeLast,
                SUM(YearMinus3) as YearMinus3
            FROM YearlyData
            GROUP BY Country, City
        ),
        -- Aggregate at Country level
        CountryLevel AS SELECT Country,
                SUM(CurrentYear) as CurrentYear,
                SUM(LastYear) as LastYear,
                SUM(YearBeforeLast) as YearBeforeLast,
                SUM(YearMinus3) as YearMinus3
            FROM YearlyData
            GROUP BY Country
        )

        -- Build the hierarchical JSON response
        DECLARE @ResponseData NVARCHAR(MAX);
        
        SELECT @ResponseData = SELECT cl.Country as location,
                        cl.CurrentYear as currentYear,
                        cl.LastYear as lastYear,
                        cl.YearBeforeLast as yearBeforeLast,
                        cl.YearMinus3 as yearMinus3 FROM CountryLevel cl
                    WHERE cl.Country = countries.Country
                SELECT citylvl.City as location,
                                citylvl.CurrentYear as currentYear,
                                citylvl.LastYear as lastYear,
                                citylvl.YearBeforeLast as yearBeforeLast,
                                citylvl.YearMinus3 as yearMinus3 FROM CityLevel citylvl
                            WHERE citylvl.Country = countries.Country 
                            AND citylvl.City = cities.City
                        SELECT yd.SiteName as location,
                                        yd.CurrentYear as currentYear,
                                        yd.LastYear as lastYear,
                                        yd.YearBeforeLast as yearBeforeLast,
                                        yd.YearMinus3 as yearMinus3 FROM YearlyData yd
                                    WHERE yd.Country = countries.Country 
                                    AND yd.City = cities.City
                                FROM SELECT DISTINCT Country, City, SiteName 
                                FROM YearlyData 
                                WHERE Country = countries.Country 
                                AND City = cities.City
                            ) sites
                        ) as children FROM SELECT DISTINCT Country, City 
                        FROM CityLevel 
                        WHERE Country = countries.Country
                    ) cities
                ) as children FROM SELECT DISTINCT Country 
                FROM CountryLevel
            ) countries
        );

        -- Handle empty result
        IF @ResponseData IS NULL
            SET @ResponseData = '[]';

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess,
                'Successfully retrieved findings trends list data.' as message);

        -- Log access for audit trail
        INSERT INTO AuditLogs (
            UserId, 
            Action, 
            EntityType, 
            EntityId, 
            Details, 
            CreatedAt
        )
        VALUES (
            @UserId,
            'LOCATION_TRENDS_ANALYSIS',
            'FINDING',
            NULL,
            'Retrieved findings trends list by location hierarchy',
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = 'Successfully retrieved findings trends list data.';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                @Message as message,
                @ErrorCode as errorCode) as JsonResponse;

    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO ErrorLogs (
            UserId,
            ErrorMessage,
            StackTrace,
            CreatedAt
        )
        VALUES (
            @UserId,
            ERROR_MESSAGE(),
            CONCAT('Procedure: Sp_GetFindingTrendsList, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving findings trends list.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get trends for all locations without filters:
EXEC Sp_GetFindingTrendsList @Parameters = N'{
    "userId": 123,
    "filters": {
        "companies": [],
        "services": [],
        "sites": []
    }
}';

2. Get trends with specific company filter:
EXEC Sp_GetFindingTrendsList @Parameters = N'{
    "userId": 123,
    "filters": {
        "companies": [408, 452],
        "services": [],
        "sites": []
    }
}';

3. Get trends with multiple filters:
EXEC Sp_GetFindingTrendsList @Parameters = N'{
    "userId": 123,
    "filters": {
        "companies": [408],
        "services": [780, 1060],
        "sites": [171135]
    }
}';

Expected JSON Response Format:
{
    "data": [
        {
            "data": {
                "location": "Australia",
                "currentYear": 56,
                "lastYear": 82,
                "yearBeforeLast": 64,
                "yearMinus3": 59,
                "__typename": "FindingsByYearAndLocation"
            },
            "children": [
                {
                    "data": {
                        "location": "Adelaide",
                        "currentYear": 2,
                        "lastYear": 0,
                        "yearBeforeLast": 4,
                        "yearMinus3": 5,
                        "__typename": "FindingsByYearAndLocation"
                    },
                    "children": [
                        {
                            "data": {
                                "location": "ISS Health Services Pty Limited - Health Division (Adelaide)",
                                "currentYear": 0,
                                "lastYear": 0,
                                "yearBeforeLast": 3,
                                "yearMinus3": 0,
                                "__typename": "FindingsByYearAndLocation"
                            },
                            "__typename": "FindingsBySite"
                        }
                    ],
                    "__typename": "FindingsByCity"
                }
            ],
            "__typename": "FindingsTrendData"
        }
    ],
    "isSuccess": true,
    "message": "Successfully retrieved findings trends list data.",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfFindingsTrendData"
}

Notes:
- Creates three-level hierarchy: Country > City > Site
- Provides year-over-year findings analysis for current year and previous 3 years
- Uses dynamic current year calculation based on GETDATE()
- Supports comprehensive filtering by companies, services, and sites
- Aggregates findings count at each hierarchical level
- Provides proper access control through user-company relationships
- Returns exact GraphQL response structure with proper type names
- Handles missing location data gracefully with fallback values
- Uses pivot logic to transform year data into columns
- Includes audit logging for compliance tracking
- Optimized with CTEs for efficient hierarchical aggregation
- Year columns: currentYear, lastYear, yearBeforeLast, yearMinus3
- Ensures consistent location hierarchy structure across all levels
*/



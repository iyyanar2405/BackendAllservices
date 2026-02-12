-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get findings trends graph data by category with year-over-year analysis
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFindingsTrendsGraphData]
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

        -- Get findings data with year trends by category
        WITH FindingsTrends AS SELECT fc.CategoryName,
                YEAR(f.OpenedDate) as [Year],
                COUNT(*) as FindingCount
            FROM Findings f
            INNER JOIN Audits a ON f.AuditId = a.AuditId
            INNER JOIN FindingCategories fc ON f.CategoryId = fc.CategoryId
            WHERE f.IsActive = 1 
            AND a.IsActive = 1
            AND f.OpenedDate IS NOT NULL
            -- Company filter
            AND (NOT EXISTS SELECT 1 FROM @CompanyList) OR a.CompanyId IN SELECT CompanyId FROM @CompanyList))
            -- Service filter
            AND (NOT EXISTS SELECT 1 FROM @ServiceList) OR EXISTS SELECT 1 FROM AuditServices aus 
                INNER JOIN @ServiceList sl ON aus.ServiceId = sl.ServiceId
                WHERE aus.AuditId = a.AuditId AND aus.IsActive = 1
            ))
            -- Site filter
            AND (NOT EXISTS SELECT 1 FROM @SiteList) OR EXISTS SELECT 1 FROM AuditSites ausites 
                INNER JOIN @SiteList stl ON ausites.SiteId = stl.SiteId
                WHERE ausites.AuditId = a.AuditId AND ausites.IsActive = 1
            ))
            -- User access control
            AND (@UserId IS NULL OR EXISTS SELECT 1 FROM UserCompanyAccess uca 
                WHERE uca.UserId = @UserId AND uca.CompanyId = a.CompanyId
            ))
            GROUP BY fc.CategoryName, YEAR(f.OpenedDate)
        ),
        -- Ensure we have all years for each category (including zeros)
        AllYears AS SELECT DISTINCT [Year] FROM FindingsTrends
            WHERE [Year] IS NOT NULL
        ),
        AllCategories AS SELECT DISTINCT CategoryName FROM FindingsTrends
        ),
        CategoryYearMatrix AS SELECT ac.CategoryName,
                ay.[Year]
            FROM AllCategories ac
            CROSS JOIN AllYears ay
        ),
        CompleteData AS SELECT cym.CategoryName,
                cym.[Year],
                ISNULL(ft.FindingCount, 0) as FindingCount
            FROM CategoryYearMatrix cym
            LEFT JOIN FindingsTrends ft ON cym.CategoryName = ft.CategoryName 
                AND cym.[Year] = ft.[Year]
        )

        -- Build the response data
        DECLARE @ResponseData NVARCHAR(MAX);
        
        SELECT @ResponseData = SELECT cd.CategoryName as categoryName,
                SELECT cd2.FindingCount as [count],
                        cd2.[Year] as [year] FROM CompleteData cd2
                    WHERE cd2.CategoryName = cd.CategoryName
                    ORDER BY cd2.[Year]
                ) as findings FROM CompleteData cd
            GROUP BY cd.CategoryName
        );

        -- Handle empty result
        IF @ResponseData IS NULL
            SET @ResponseData = '[]';

        -- Build final response structure
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as categories,
                        
                CAST(1 AS BIT) as isSuccess,
                'Successfully fetched Findings By Category response' as message);

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
            'TRENDS_ANALYSIS',
            'FINDING',
            NULL,
            'Retrieved findings trends graph data by category',
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = 'Successfully fetched Findings By Category response';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response
        SELECT CAST('[]' AS NVARCHAR(MAX)) as categories,
                        
                CAST(0 AS BIT) as isSuccess,
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
            CONCAT('Procedure: Sp_GetFindingsTrendsGraphData, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT CAST('[]' AS NVARCHAR(MAX)) as categories,
                        
                CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving findings trends data.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get trends for all findings without filters:
EXEC Sp_GetFindingsTrendsGraphData @Parameters = N'{
    "userId": 123,
    "companies": [],
    "services": [],
    "sites": []
}';

2. Get trends with specific company filter:
EXEC Sp_GetFindingsTrendsGraphData @Parameters = N'{
    "userId": 123,
    "companies": [408, 452],
    "services": [],
    "sites": []
}';

3. Get trends with multiple filters:
EXEC Sp_GetFindingsTrendsGraphData @Parameters = N'{
    "userId": 123,
    "companies": [408],
    "services": [780, 1060],
    "sites": [171135]
}';

Expected JSON Response Format:
{
    "data": {
        "categories": [
            {
                "categoryName": "CAT1 (Major)",
                "findings": [
                    {
                        "count": 17,
                        "year": 2022,
                        "__typename": "FindingsCount"
                    },
                    {
                        "count": 23,
                        "year": 2023,
                        "__typename": "FindingsCount"
                    },
                    {
                        "count": 37,
                        "year": 2024,
                        "__typename": "FindingsCount"
                    },
                    {
                        "count": 29,
                        "year": 2025,
                        "__typename": "FindingsCount"
                    }
                ],
                "__typename": "CategoryData"
            },
            {
                "categoryName": "CAT2 (Minor)",
                "findings": [
                    {
                        "count": 833,
                        "year": 2022,
                        "__typename": "FindingsCount"
                    },
                    {
                        "count": 708,
                        "year": 2023,
                        "__typename": "FindingsCount"
                    },
                    {
                        "count": 680,
                        "year": 2024,
                        "__typename": "FindingsCount"
                    },
                    {
                        "count": 412,
                        "year": 2025,
                        "__typename": "FindingsCount"
                    }
                ],
                "__typename": "CategoryData"
            }
        ],
        "__typename": "FindingsByCategoryResponse"
    },
    "isSuccess": true,
    "message": "Successfully fetched Findings By Category response",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfFindingsByCategoryResponse"
}

Notes:
- Groups findings by category and extracts year from OpenedDate
- Creates year-over-year trend analysis for each finding category
- Includes zero counts for years where a category has no findings
- Supports filtering by companies, services, and sites
- Provides proper access control through user-company relationships
- Returns exact GraphQL response structure with proper type names
- Handles all finding categories: CAT1 (Major), CAT2 (Minor), Observation, 
  Opportunity for Improvement, Noteworthy Effort, Risk of non-conformance, 
  Time bound non-conformity
- Uses CROSS JOIN to ensure complete year matrix for consistent trend data
- Includes audit logging for compliance tracking
- Optimized for trend analysis with proper year ordering
*/



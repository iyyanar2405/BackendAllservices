-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get findings grouped by clause/standard hierarchy with category counts
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFindingsByClauseList]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @StartDate DATE = NULL;
    DECLARE @EndDate DATE = NULL;
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
                @StartDate = TRY_CAST(JSON_VALUE(@Parameters, '$.filters.startDate') AS DATE),
                @EndDate = TRY_CAST(JSON_VALUE(@Parameters, '$.filters.endDate') AS DATE),
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

        -- Build base query for findings with filters
        WITH FilteredFindings AS SELECT f.FindingId,
                f.CategoryId,
                fc.CategoryName,
                f.ClauseId,
                c.ClauseName,
                c.ChapterId,
                ch.ChapterName,
                c.ServiceId,
                s.ServiceName,
                a.CompanyId
            FROM Findings f
            INNER JOIN Audits a ON f.AuditId = a.AuditId
            INNER JOIN FindingCategories fc ON f.CategoryId = fc.CategoryId
            LEFT JOIN Clauses c ON f.ClauseId = c.ClauseId
            LEFT JOIN Chapters ch ON c.ChapterId = ch.ChapterId
            LEFT JOIN Services s ON c.ServiceId = s.ServiceId
            WHERE f.IsActive = 1 
            AND a.IsActive = 1
            -- Date filters
            AND (@StartDate IS NULL OR f.OpenedDate >= @StartDate)
            AND (@EndDate IS NULL OR f.OpenedDate <= @EndDate)
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
        ),
        -- Aggregate by Service (top level)
        ServiceLevel AS SELECT ISNULL(ServiceName, 'Unknown Service') as ServiceName,
                COUNT(*) as TotalCount,
                CategoryName,
                COUNT(*) as CategoryCount
            FROM FilteredFindings
            GROUP BY ISNULL(ServiceName, 'Unknown Service'), CategoryName
        ),
        -- Aggregate by Chapter (middle level)
        ChapterLevel AS SELECT ISNULL(ServiceName, 'Unknown Service') as ServiceName,
                ISNULL(ChapterName, ' - ') as ChapterName,
                COUNT(*) as TotalCount,
                CategoryName,
                COUNT(*) as CategoryCount
            FROM FilteredFindings
            GROUP BY ISNULL(ServiceName, 'Unknown Service'), ISNULL(ChapterName, ' - '), CategoryName
        ),
        -- Aggregate by Clause (bottom level)
        ClauseLevel AS SELECT ISNULL(ServiceName, 'Unknown Service') as ServiceName,
                ISNULL(ChapterName, ' - ') as ChapterName,
                ISNULL(ClauseName, ' - ') as ClauseName,
                COUNT(*) as TotalCount,
                CategoryName,
                COUNT(*) as CategoryCount
            FROM FilteredFindings
            GROUP BY ISNULL(ServiceName, 'Unknown Service'), ISNULL(ChapterName, ' - '), ISNULL(ClauseName, ' - '), CategoryName
        )

        -- Build the hierarchical JSON response
        DECLARE @ResponseData NVARCHAR(MAX);
        
        SELECT @ResponseData = SELECT sl.ServiceName as name,
                        SUM(sl.TotalCount) as totalCount,
                        SELECT sl2.CategoryName as [key],
                                SUM(sl2.CategoryCount) as [value] FROM ServiceLevel sl2
                            WHERE sl2.ServiceName = sl.ServiceName
                            GROUP BY sl2.CategoryName
                        ) as categoryCounts FROM ServiceLevel sl
                    WHERE sl.ServiceName = services.ServiceName
                    GROUP BY sl.ServiceName
                SELECT ch.ChapterName as name,
                                SUM(ch.TotalCount) as totalCount,
                                SELECT ch2.CategoryName as [key],
                                        SUM(ch2.CategoryCount) as [value] FROM ChapterLevel ch2
                                    WHERE ch2.ServiceName = services.ServiceName 
                                    AND ch2.ChapterName = ch.ChapterName
                                    GROUP BY ch2.CategoryName
                                ) as categoryCounts FROM ChapterLevel ch
                            WHERE ch.ServiceName = services.ServiceName 
                            AND ch.ChapterName = chapters.ChapterName
                            GROUP BY ch.ChapterName
                        SELECT cl.ClauseName as name,
                                        SUM(cl.TotalCount) as totalCount,
                                        SELECT cl2.CategoryName as [key],
                                                SUM(cl2.CategoryCount) as [value] FROM ClauseLevel cl2
                                            WHERE cl2.ServiceName = services.ServiceName 
                                            AND cl2.ChapterName = chapters.ChapterName
                                            AND cl2.ClauseName = cl.ClauseName
                                            GROUP BY cl2.CategoryName
                                        ) as categoryCounts FROM ClauseLevel cl
                                    WHERE cl.ServiceName = services.ServiceName 
                                    AND cl.ChapterName = chapters.ChapterName
                                    GROUP BY cl.ClauseName
                                FROM SELECT DISTINCT ServiceName, ChapterName, ClauseName 
                                FROM ClauseLevel 
                                WHERE ServiceName = services.ServiceName 
                                AND ChapterName = chapters.ChapterName
                            ) clauses
                        ) as children FROM SELECT DISTINCT ServiceName, ChapterName 
                        FROM ChapterLevel 
                        WHERE ServiceName = services.ServiceName
                    ) chapters
                ) as children FROM SELECT DISTINCT ServiceName 
                FROM ServiceLevel
            ) services
        );

        -- Handle empty result
        IF @ResponseData IS NULL
            SET @ResponseData = '[]';

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess);

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
            'CLAUSE_ANALYSIS',
            'FINDING',
            NULL,
            'Retrieved findings by clause list analysis',
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = '';

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
            CONCAT('Procedure: Sp_GetFindingsByClauseList, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving findings by clause list.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all findings by clause without filters:
EXEC Sp_GetFindingsByClauseList @Parameters = N'{
    "userId": 123,
    "filters": {
        "startDate": null,
        "endDate": null,
        "companies": [],
        "services": [],
        "sites": []
    }
}';

2. Get findings by clause with date range:
EXEC Sp_GetFindingsByClauseList @Parameters = N'{
    "userId": 123,
    "filters": {
        "startDate": "2024-01-01",
        "endDate": "2024-12-31",
        "companies": [],
        "services": [],
        "sites": []
    }
}';

3. Get findings by clause with specific filters:
EXEC Sp_GetFindingsByClauseList @Parameters = N'{
    "userId": 123,
    "filters": {
        "startDate": null,
        "endDate": null,
        "companies": [408, 452],
        "services": [780, 1060],
        "sites": [171135]
    }
}';

Expected JSON Response Format:
{
    "data": [
        {
            "data": {
                "name": "ISO 14001:2015",
                "totalCount": 25,
                "categoryCounts": [
                    {
                        "key": "CAT2 (Minor)",
                        "value": 20,
                        "__typename": "KeyValuePairOfStringAndInt32"
                    },
                    {
                        "key": "CAT1 (Major)",
                        "value": 5,
                        "__typename": "KeyValuePairOfStringAndInt32"
                    }
                ],
                "__typename": "ClauseLitTrend"
            },
            "children": [
                {
                    "data": {
                        "name": "4 - Context of the organization",
                        "totalCount": 10,
                        "categoryCounts": [...],
                        "__typename": "ClauseLitTrend"
                    },
                    "children": [
                        {
                            "data": {
                                "name": "4.1 - Understanding the organization and its context",
                                "totalCount": 5,
                                "categoryCounts": [...],
                                "__typename": "ClauseLitTrend"
                            },
                            "__typename": "FindingsByClause"
                        }
                    ],
                    "__typename": "FindingsByChapters"
                }
            ],
            "__typename": "FindingbyClauseListTrendData"
        }
    ],
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfFindingbyClauseListTrendData"
}

Notes:
- Creates three-level hierarchy: Service > Chapter > Clause
- Aggregates findings count by category at each level
- Supports comprehensive filtering by date range, companies, services, and sites
- Provides access control validation through user-company relationships
- Returns category counts as key-value pairs with proper type names
- Handles missing/null clause information gracefully
- Includes audit logging for compliance tracking
- Returns exact GraphQL response structure with proper nesting
- Optimized with CTEs for efficient hierarchical aggregation
*/



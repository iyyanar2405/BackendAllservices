-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get paginated list of findings with filtering and sorting capabilities
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFindingList]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @CompanyId INT = NULL;
    DECLARE @SiteId INT = NULL;
    DECLARE @Status NVARCHAR(50) = NULL;
    DECLARE @Category NVARCHAR(100) = NULL;
    DECLARE @ServiceIds NVARCHAR(MAX) = NULL;
    DECLARE @PageNumber INT = 1;
    DECLARE @PageSize INT = 100;
    DECLARE @SortBy NVARCHAR(50) = 'openDate';
    DECLARE @SortOrder NVARCHAR(10) = 'DESC';
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @CompanyId = JSON_VALUE(@Parameters, '$.companyId'),
                @SiteId = JSON_VALUE(@Parameters, '$.siteId'),
                @Status = JSON_VALUE(@Parameters, '$.status'),
                @Category = JSON_VALUE(@Parameters, '$.category'),
                @ServiceIds = NULL,
                @PageNumber = ISNULL(JSON_VALUE(@Parameters, '$.pageNumber'), 1),
                @PageSize = ISNULL(JSON_VALUE(@Parameters, '$.pageSize'), 100),
                @SortBy = ISNULL(JSON_VALUE(@Parameters, '$.sortBy'), 'openDate'),
                @SortOrder = ISNULL(JSON_VALUE(@Parameters, '$.sortOrder'), 'DESC')
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

        -- Build base query with access control
        DECLARE @AccessControlFilter NVARCHAR(MAX) = '';
        IF @UserId IS NOT NULL
        BEGIN
            SET @AccessControlFilter = '
                AND EXISTS SELECT 1 FROM UserCompanyAccess uca 
                    WHERE uca.UserId = @UserId AND uca.CompanyId = a.CompanyId
                )';
        END

        -- Parse service IDs filter
        DECLARE @ServiceIdsList TABLE (ServiceId INT);
        IF @ServiceIds IS NOT NULL AND @ServiceIds != '[]'
        BEGIN
            INSERT INTO @ServiceIdsList (ServiceId)
            SELECT CAST([value] AS INT) 
            FROM OPENJSON(@ServiceIds);
        END

        -- Build dynamic filters
        DECLARE @WhereClause NVARCHAR(MAX) = '1=1';
        
        IF @CompanyId IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND a.CompanyId = @CompanyId';
            
        IF @SiteId IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND EXISTS SELECT 1 FROM AuditSites aus WHERE aus.AuditId = f.AuditId AND aus.SiteId = @SiteId AND aus.IsActive = 1)';
            
        IF @Status IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND fs.StatusName = @Status';
            
        IF @Category IS NOT NULL
            SET @WhereClause = @WhereClause + ' AND fc.CategoryName = @Category';
            
        IF EXISTS SELECT 1 FROM @ServiceIdsList)
            SET @WhereClause = @WhereClause + ' AND EXISTS SELECT 1 FROM AuditServices aus2 INNER JOIN @ServiceIdsList sil ON aus2.ServiceId = sil.ServiceId WHERE aus2.AuditId = f.AuditId AND aus2.IsActive = 1)';

        -- Calculate offset for pagination
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        -- Build ORDER BY clause
        DECLARE @OrderByClause NVARCHAR(200) = 'f.OpenedDate DESC';
        IF @SortBy = 'findingNumber'
            SET @OrderByClause = 'f.FindingNumber ' + @SortOrder;
        ELSE IF @SortBy = 'title'
            SET @OrderByClause = 'f.Title ' + @SortOrder;
        ELSE IF @SortBy = 'status'
            SET @OrderByClause = 'fs.StatusName ' + @SortOrder;
        ELSE IF @SortBy = 'category'
            SET @OrderByClause = 'fc.CategoryName ' + @SortOrder;
        ELSE IF @SortBy = 'dueDate'
            SET @OrderByClause = 'f.DueDate ' + @SortOrder;
        ELSE IF @SortBy = 'openDate'
            SET @OrderByClause = 'f.OpenedDate ' + @SortOrder;

        -- Build the main query
        DECLARE @FindingsData NVARCHAR(MAX);

        SELECT @FindingsData = SELECT f.FindingId as findingsId,
                f.FindingNumber as findingNumber,
                f.Title as title,
                fs.StatusName as status,
                fc.CategoryName as category,
                CASE 
                    WHEN fr.ResponseStatus = 'Draft' THEN 'Draft'
                    WHEN fr.ResponseStatus = 'Submitted' THEN 'Send to DNV'
                    ELSE NULL 
                END as response,
                a.CompanyId as companyId,
                FORMAT(f.OpenedDate, 'yyyy-MM-dd') as openDate,
                CASE 
                    WHEN f.DueDate IS NOT NULL 
                    THEN FORMAT(f.DueDate, 'yyyy-MM-dd')
                    ELSE NULL 
                END as dueDate,
                CASE 
                    WHEN f.AcceptedDate IS NOT NULL 
                    THEN FORMAT(f.AcceptedDate, 'yyyy-MM-dd')
                    ELSE NULL 
                END as acceptedDate,
                CASE 
                    WHEN f.ClosedDate IS NOT NULL 
                    THEN FORMAT(f.ClosedDate, 'yyyy-MM-dd')
                    ELSE NULL 
                END as closedDate,
                SELECT TOP 1 aus.SiteId 
                    FROM AuditSites aus 
                    WHERE aus.AuditId = f.AuditId AND aus.IsActive = 1
                    ORDER BY aus.CreatedAt
                ) as siteId,
                SELECT aus.ServiceId
                    FROM AuditServices aus
                    WHERE aus.AuditId = f.AuditId AND aus.IsActive = 1
                    ORDER BY aus.ServiceId
                ) as services FROM Findings f
            INNER JOIN Audits a ON f.AuditId = a.AuditId
            INNER JOIN FindingStatuses fs ON f.StatusId = fs.StatusId
            INNER JOIN FindingCategories fc ON f.CategoryId = fc.CategoryId
            LEFT JOIN SELECT fr.FindingId,
                    fr.ResponseStatus,
                    ROW_NUMBER() OVER (PARTITION BY fr.FindingId ORDER BY fr.UpdatedOn DESC) as rn
                FROM FindingResponses fr 
                WHERE fr.IsActive = 1
            ) fr ON f.FindingId = fr.FindingId AND fr.rn = 1
            WHERE f.IsActive = 1
            AND a.IsActive = 1
            AND (' + @WhereClause + ')' + @AccessControlFilter + '
            ORDER BY ' + @OrderByClause + '
            OFFSET ' + CAST(@Offset AS NVARCHAR) + ' ROWS
            FETCH NEXT ' + CAST(@PageSize AS NVARCHAR) + ' ROWS ONLY
        );

        -- Handle empty result
        IF @FindingsData IS NULL
            SET @FindingsData = '[]';

        -- Build final response
        DECLARE @ResponseData NVARCHAR(MAX);
        SELECT @ResponseData = SELECT NULL as isSuccess);

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
            'LIST',
            'FINDING',
            NULL,
            CONCAT('Retrieved findings list - Page: ', @PageNumber, ', Size: ', @PageSize),
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = '';

        -- Return success response
        SELECT @ResponseData as JsonResponse;
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
            CONCAT('Procedure: Sp_GetFindingList, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving findings list.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all findings with user access control:
EXEC Sp_GetFindingList @Parameters = N'{
    "userId": 123
}';

2. Get findings with filtering:
EXEC Sp_GetFindingList @Parameters = N'{
    "userId": 123,
    "companyId": 408,
    "status": "Open",
    "category": "CAT2 (Minor)",
    "pageNumber": 1,
    "pageSize": 20
}';

3. Get findings for specific services:
EXEC Sp_GetFindingList @Parameters = N'{
    "userId": 123,
    "serviceIds": [780, 1060],
    "sortBy": "openDate",
    "sortOrder": "DESC"
}';

4. Get findings for specific site:
EXEC Sp_GetFindingList @Parameters = N'{
    "userId": 123,
    "siteId": 171135,
    "pageNumber": 1,
    "pageSize": 50
}';

Expected JSON Response Format:
{
    "data": [
        {
            "findingsId": 4263107,
            "findingNumber": "CLAY-0006-2887291",
            "title": "Operational control and infrastructure - portable hoses",
            "status": "Open",
            "category": "CAT2 (Minor)",
            "response": null,
            "companyId": 408,
            "openDate": "2025-07-02",
            "dueDate": null,
            "acceptedDate": null,
            "closedDate": null,
            "siteId": 171135,
            "services": [780, 1060],
            "__typename": "FindingsListResponse"
        }
    ],
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfFindingsListResponse"
}

Notes:
- Supports comprehensive filtering by company, site, status, category, and services
- Includes pagination with configurable page size
- Supports multiple sorting options (findingNumber, title, status, category, openDate, dueDate)
- Provides access control validation through user-company relationships
- Returns response status based on latest finding response
- Handles date formatting consistently (yyyy-MM-dd)
- Returns services as array of service IDs
- Includes audit logging for compliance tracking
- Returns exact GraphQL response structure with proper type names
- Handles empty results gracefully
*/



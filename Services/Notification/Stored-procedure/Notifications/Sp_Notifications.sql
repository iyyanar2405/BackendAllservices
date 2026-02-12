-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get paginated notifications list with filtering by category, company, service, and site
-- =============================================
CREATE PROCEDURE [dbo].[Sp_Notifications]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @Category NVARCHAR(MAX) = NULL;
    DECLARE @Company NVARCHAR(MAX) = NULL;
    DECLARE @Service NVARCHAR(MAX) = NULL;
    DECLARE @Site NVARCHAR(MAX) = NULL;
    DECLARE @PageNumber INT = 1;
    DECLARE @PageSize INT = 10;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @Category = NULL,
                @Company = NULL,
                @Service = NULL,
                @Site = NULL,
                @PageNumber = ISNULL(JSON_VALUE(@Parameters, '$.pageNumber'), 1),
                @PageSize = ISNULL(JSON_VALUE(@Parameters, '$.pageSize'), 10)
        END
        ELSE
        BEGIN
            SET @ErrorCode = 'INVALID_JSON';
            SET @Message = 'Invalid JSON format in parameters.';
            GOTO ErrorResponse;
        END

        -- Set defaults for empty arrays
        IF @Category IS NULL OR @Category = '' SET @Category = '[]';
        IF @Company IS NULL OR @Company = '' SET @Company = '[]';
        IF @Service IS NULL OR @Service = '' SET @Service = '[]';
        IF @Site IS NULL OR @Site = '' SET @Site = '[]';

        -- Validate pagination parameters
        IF @PageNumber < 1 SET @PageNumber = 1;
        IF @PageSize < 1 OR @PageSize > 100 SET @PageSize = 10;

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Calculate offset for pagination
        DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

        -- Get total count for pagination
        DECLARE @TotalItems INT;
        SELECT @TotalItems = COUNT(*)
        FROM Notifications n
        WHERE n.IsActive = 1
        AND (
            @UserId IS NULL 
            OR EXISTS SELECT 1 FROM UserNotificationAccess una
                WHERE una.UserId = @UserId 
                AND una.CategoryId = n.CategoryId
                AND una.IsActive = 1
            )
        )
        AND (
            JSON_LENGTH(@Category) = 0
            OR EXISTS SELECT 1 FROM OPENJSON(@Category) cat
                WHERE CAST(cat.value AS INT) = n.CategoryId
            )
        )
        AND (
            JSON_LENGTH(@Company) = 0
            OR EXISTS SELECT 1 FROM OPENJSON(@Company) comp
                WHERE CAST(comp.value AS INT) = n.CompanyId
            )
        )
        AND (
            JSON_LENGTH(@Service) = 0
            OR EXISTS SELECT 1 FROM OPENJSON(@Service) serv
                WHERE CAST(serv.value AS INT) = n.ServiceId
            )
        )
        AND (
            JSON_LENGTH(@Site) = 0
            OR EXISTS SELECT 1 FROM OPENJSON(@Site) site_filter
                WHERE CAST(site_filter.value AS INT) = n.SiteId
            )
        );

        -- Calculate total pages
        DECLARE @TotalPages INT = CEILING(CAST(@TotalItems AS FLOAT) / @PageSize);

        -- Get paginated notifications
        DECLARE @NotificationsData NVARCHAR(MAX);
        
        SELECT @NotificationsData = SELECT FORMAT(n.CreatedAt, 'yyyy-MM-ddTHH:mm:ss.fffZ') as createdTime,
                n.NotificationId as infoId,
                n.Message as message,
                ISNULL(n.Language, 'en') as language,
                nc.CategoryName as notificationCategory,
                CAST(ISNULL(n.IsRead, 0) AS BIT) as readStatus,
                n.Subject as subject,
                ISNULL(n.EntityType, 'generic') as entityType,
                ISNULL(CAST(n.EntityId AS NVARCHAR(50)), '0') as entityId,
                ISNULL(n.ExternalLink, '') as snowLink FROM Notifications n
            INNER JOIN NotificationCategories nc ON n.CategoryId = nc.CategoryId
            WHERE n.IsActive = 1
            AND nc.IsActive = 1
            AND (
                @UserId IS NULL 
                OR EXISTS SELECT 1 FROM UserNotificationAccess una
                    WHERE una.UserId = @UserId 
                    AND una.CategoryId = n.CategoryId
                    AND una.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@Category) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Category) cat
                    WHERE CAST(cat.value AS INT) = n.CategoryId
                )
            )
            AND (
                JSON_LENGTH(@Company) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Company) comp
                    WHERE CAST(comp.value AS INT) = n.CompanyId
                )
            )
            AND (
                JSON_LENGTH(@Service) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Service) serv
                    WHERE CAST(serv.value AS INT) = n.ServiceId
                )
            )
            AND (
                JSON_LENGTH(@Site) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Site) site_filter
                    WHERE CAST(site_filter.value AS INT) = n.SiteId
                )
            )
            ORDER BY n.CreatedAt DESC
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY
        );

        -- Handle empty result
        IF @NotificationsData IS NULL OR @NotificationsData = ''
            SET @NotificationsData = '[]';

        -- Build pagination data structure
        DECLARE @PaginationData NVARCHAR(MAX);
        SELECT @PaginationData = SELECT @PageNumber as currentPage,
                NULL as items,
                @TotalItems as totalItems,
                @TotalPages as totalPages);

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess,
                'Success' as message);

        -- Mark notifications as read for this user (if userId provided)
        IF @UserId IS NOT NULL
        BEGIN
            UPDATE Notifications 
            SET IsRead = 1, 
                ReadAt = GETDATE(), 
                ReadByUserId = @UserId
            WHERE NotificationId IN SELECT n.NotificationId
                FROM Notifications n
                WHERE n.IsActive = 1
                AND EXISTS SELECT 1 FROM UserNotificationAccess una
                    WHERE una.UserId = @UserId 
                    AND una.CategoryId = n.CategoryId
                    AND una.IsActive = 1
                )
                AND (
                    JSON_LENGTH(@Category) = 0
                    OR EXISTS SELECT 1 FROM OPENJSON(@Category) cat
                        WHERE CAST(cat.value AS INT) = n.CategoryId
                    )
                )
                AND (
                    JSON_LENGTH(@Company) = 0
                    OR EXISTS SELECT 1 FROM OPENJSON(@Company) comp
                        WHERE CAST(comp.value AS INT) = n.CompanyId
                    )
                )
                AND (
                    JSON_LENGTH(@Service) = 0
                    OR EXISTS SELECT 1 FROM OPENJSON(@Service) serv
                        WHERE CAST(serv.value AS INT) = n.ServiceId
                    )
                )
                AND (
                    JSON_LENGTH(@Site) = 0
                    OR EXISTS SELECT 1 FROM OPENJSON(@Site) site_filter
                        WHERE CAST(site_filter.value AS INT) = n.SiteId
                    )
                )
                ORDER BY n.CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY
            )
            AND IsRead = 0;

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
                'VIEW_NOTIFICATIONS',
                'NOTIFICATIONS',
                NULL,
                CONCAT('Retrieved notifications page ', @PageNumber, ' with ', @PageSize, ' items'),
                GETDATE()
            );
        END

        SET @IsSuccess = 1;
        SET @Message = 'Success';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response with empty pagination structure
        DECLARE @EmptyPaginationData NVARCHAR(MAX);
        SELECT @EmptyPaginationData = SELECT 1 as currentPage,
                CAST('[]' AS NVARCHAR(MAX)
                0 as totalItems,
                0 as totalPages);

        SELECT 
                NULL as isSuccess,
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
            CONCAT('Procedure: Sp_Notifications, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response with empty pagination structure
        DECLARE @ErrorPaginationData NVARCHAR(MAX);
        SELECT @ErrorPaginationData = SELECT 1 as currentPage,
                CAST('[]' AS NVARCHAR(MAX)
                0 as totalItems,
                0 as totalPages);

        SELECT 
                NULL as isSuccess,
                'An error occurred while retrieving notifications.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get first page of notifications (no filters):
EXEC Sp_Notifications @Parameters = N'{
    "userId": 123,
    "category": [],
    "company": [],
    "service": [],
    "site": [],
    "pageNumber": 1,
    "pageSize": 10
}';

2. Get notifications filtered by category and company:
EXEC Sp_Notifications @Parameters = N'{
    "userId": 123,
    "category": [2, 4],
    "company": [384, 444],
    "service": [],
    "site": [],
    "pageNumber": 1,
    "pageSize": 10
}';

Expected JSON Response Format:
{
    "data": {
        "currentPage": 1,
        "items": [
            {
                "createdTime": "2025-09-12T11:47:01.677Z",
                "infoId": 4253,
                "message": "<p>New request {CS0002713}created.<br><br>...</p>",
                "language": "en",
                "notificationCategory": "New Request created",
                "readStatus": true,
                "subject": "Support request created",
                "entityType": "generic",
                "entityId": "0",
                "snowLink": "4d18be332b7b2a101450ff65d891bf78",
                "__typename": "GetNotificationList"
            }
        ],
        "totalItems": 45,
        "totalPages": 5,
        "__typename": "NotificationsPaginatedResponse"
    },
    "isSuccess": true,
    "message": "Success",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfNotificationsPaginatedResponse"
}

Database Tables Referenced:
- Notifications: Main notification records
- NotificationCategories: Category definitions
- UserNotificationAccess: User access to notification categories
- Users: User validation
- AuditLogs: Access tracking

Notes:
- Returns paginated notifications with complete pagination metadata
- Supports filtering by category, company, service, and site (arrays)
- Automatically marks viewed notifications as read for the user
- User access control applies if userId provided
- Orders notifications by creation date (newest first)
- Includes full pagination support with currentPage, totalItems, totalPages
- Returns exact GraphQL response structure with proper type names
- Empty filter arrays mean no filtering on that dimension
- Page size is limited to maximum 100 items for performance
- Includes audit logging for notification access tracking
- Handles HTML content in notification messages
- Supports multi-language notifications
- Links to external systems via snowLink field
*/



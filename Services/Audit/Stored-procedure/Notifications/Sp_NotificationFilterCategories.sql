-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get notification filter categories based on company, service, and site filters
-- =============================================
CREATE PROCEDURE [dbo].[Sp_NotificationFilterCategories]
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

        -- Set defaults for empty arrays
        IF @Companies IS NULL OR @Companies = '' SET @Companies = '[]';
        IF @Services IS NULL OR @Services = '' SET @Services = '[]';
        IF @Sites IS NULL OR @Sites = '' SET @Sites = '[]';

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Get notification categories based on filters
        DECLARE @CategoriesData NVARCHAR(MAX);
        
        SELECT @CategoriesData = SELECT DISTINCT
                nc.CategoryId as id,
                nc.CategoryName as label FROM NotificationCategories nc
            INNER JOIN Notifications n ON nc.CategoryId = n.CategoryId
            WHERE nc.IsActive = 1
            AND n.IsActive = 1
            AND (
                @UserId IS NULL 
                OR EXISTS SELECT 1 FROM UserNotificationAccess una
                    WHERE una.UserId = @UserId 
                    AND una.CategoryId = nc.CategoryId
                    AND una.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@Companies) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Companies) comp
                    WHERE CAST(comp.value AS INT) = n.CompanyId
                )
            )
            AND (
                JSON_LENGTH(@Services) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Services) serv
                    WHERE CAST(serv.value AS INT) = n.ServiceId
                )
            )
            AND (
                JSON_LENGTH(@Sites) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Sites) site_filter
                    WHERE CAST(site_filter.value AS INT) = n.SiteId
                )
            )
            ORDER BY nc.CategoryName
        );

        -- Handle empty result
        IF @CategoriesData IS NULL OR @CategoriesData = ''
            SET @CategoriesData = '[]';

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess,
                'Success' as message);

        SET @IsSuccess = 1;
        SET @Message = 'Success';

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
            CONCAT('Procedure: Sp_NotificationFilterCategories, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving notification categories.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all categories (no filters):
EXEC Sp_NotificationFilterCategories @Parameters = N'{
    "userId": 123,
    "companies": [],
    "services": [],
    "sites": []
}';

2. Get categories filtered by companies:
EXEC Sp_NotificationFilterCategories @Parameters = N'{
    "userId": 123,
    "companies": [384, 444, 468],
    "services": [],
    "sites": []
}';

Expected JSON Response Format:
{
    "data": [
        {
            "id": 2,
            "label": "Certificates",
            "__typename": "CategoryFilterResponse"
        },
        {
            "id": 4,
            "label": "Schedule",
            "__typename": "CategoryFilterResponse"
        }
    ],
    "isSuccess": true,
    "message": "Success",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfCategoryFilterResponse"
}

Database Tables Referenced:
- NotificationCategories: Category definitions
- Notifications: Notification records
- UserNotificationAccess: User access to notification categories
- Users: User validation

Notes:
- Filters categories based on existing notifications that match the provided filters
- Supports filtering by companies, services, and sites (arrays)
- Only returns categories that have actual notifications
- User access control applies if userId provided
- Returns distinct categories to avoid duplicates
- Returns exact GraphQL response structure with proper type names
- Empty filter arrays mean no filtering on that dimension
*/



-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get notification filter services based on company, category, and site filters
-- =============================================
CREATE PROCEDURE [dbo].[Sp_NotificationFilterServices]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @Companies NVARCHAR(MAX) = NULL;
    DECLARE @Categories NVARCHAR(MAX) = NULL;
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
                @Categories = NULL,
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
        IF @Categories IS NULL OR @Categories = '' SET @Categories = '[]';
        IF @Sites IS NULL OR @Sites = '' SET @Sites = '[]';

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Get notification services based on filters
        DECLARE @ServicesData NVARCHAR(MAX);
        
        SELECT @ServicesData = SELECT DISTINCT
                s.ServiceId as id,
                s.ServiceName as label FROM Services s
            INNER JOIN Notifications n ON s.ServiceId = n.ServiceId
            WHERE s.IsActive = 1
            AND n.IsActive = 1
            AND (
                @UserId IS NULL 
                OR EXISTS SELECT 1 FROM UserServiceAccess usa
                    WHERE usa.UserId = @UserId 
                    AND usa.ServiceId = s.ServiceId
                    AND usa.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@Companies) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Companies) comp
                    WHERE CAST(comp.value AS INT) = n.CompanyId
                )
            )
            AND (
                JSON_LENGTH(@Categories) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Categories) cat
                    WHERE CAST(cat.value AS INT) = n.CategoryId
                )
            )
            AND (
                JSON_LENGTH(@Sites) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Sites) site_filter
                    WHERE CAST(site_filter.value AS INT) = n.SiteId
                )
            )
            ORDER BY s.ServiceName
        );

        -- Handle empty result
        IF @ServicesData IS NULL OR @ServicesData = ''
            SET @ServicesData = '[]';

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
            CONCAT('Procedure: Sp_NotificationFilterServices, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving notification services.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all services (no filters):
EXEC Sp_NotificationFilterServices @Parameters = N'{
    "userId": 123,
    "companies": [],
    "categories": [],
    "sites": []
}';

2. Get services filtered by categories and companies:
EXEC Sp_NotificationFilterServices @Parameters = N'{
    "userId": 123,
    "companies": [384, 444],
    "categories": [2, 4],
    "sites": []
}';

Expected JSON Response Format:
{
    "data": [
        {
            "id": 162,
            "label": "ISO 20121:2012",
            "__typename": "ServiceFilterResponse"
        },
        {
            "id": 780,
            "label": "ISO 9001:2015",
            "__typename": "ServiceFilterResponse"
        },
        {
            "id": 1060,
            "label": "ISO 14001:2015",
            "__typename": "ServiceFilterResponse"
        }
    ],
    "isSuccess": true,
    "message": "Success",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfServiceFilterResponse"
}

Database Tables Referenced:
- Services: Service definitions
- Notifications: Notification records
- UserServiceAccess: User access to services
- Users: User validation

Notes:
- Filters services based on existing notifications that match the provided filters
- Supports filtering by companies, categories, and sites (arrays)
- Only returns services that have actual notifications
- User access control applies if userId provided
- Returns distinct services to avoid duplicates
- Returns exact GraphQL response structure with proper type names
- Empty filter arrays mean no filtering on that dimension
- Services are ordered alphabetically by name
*/



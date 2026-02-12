-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get notification filter companies based on category, service, and site filters
-- =============================================
CREATE PROCEDURE [dbo].[Sp_NotificationFilterCompanies]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @Categories NVARCHAR(MAX) = NULL;
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
                @Categories = NULL,
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
        IF @Categories IS NULL OR @Categories = '' SET @Categories = '[]';
        IF @Services IS NULL OR @Services = '' SET @Services = '[]';
        IF @Sites IS NULL OR @Sites = '' SET @Sites = '[]';

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Get notification companies based on filters
        DECLARE @CompaniesData NVARCHAR(MAX);
        
        SELECT @CompaniesData = SELECT DISTINCT
                c.CompanyId as id,
                c.CompanyName as label FROM Companies c
            INNER JOIN Notifications n ON c.CompanyId = n.CompanyId
            WHERE c.IsActive = 1
            AND n.IsActive = 1
            AND (
                @UserId IS NULL 
                OR EXISTS SELECT 1 FROM UserCompanyAccess uca
                    WHERE uca.UserId = @UserId 
                    AND uca.CompanyId = c.CompanyId
                    AND uca.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@Categories) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Categories) cat
                    WHERE CAST(cat.value AS INT) = n.CategoryId
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
            ORDER BY c.CompanyName
        );

        -- Handle empty result
        IF @CompaniesData IS NULL OR @CompaniesData = ''
            SET @CompaniesData = '[]';

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
            CONCAT('Procedure: Sp_NotificationFilterCompanies, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving notification companies.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all companies (no filters):
EXEC Sp_NotificationFilterCompanies @Parameters = N'{
    "userId": 123,
    "categories": [],
    "services": [],
    "sites": []
}';

2. Get companies filtered by categories and services:
EXEC Sp_NotificationFilterCompanies @Parameters = N'{
    "userId": 123,
    "categories": [2, 4],
    "services": [780, 1060],
    "sites": []
}';

Expected JSON Response Format:
{
    "data": [
        {
            "id": 384,
            "label": "ZANINI PORTE SPA",
            "__typename": "CompaniesFilterResponse"
        },
        {
            "id": 444,
            "label": "BRUNI GLASS SPA",
            "__typename": "CompaniesFilterResponse"
        },
        {
            "id": 468,
            "label": "Asbestos Compliance Solutions Ltd",
            "__typename": "CompaniesFilterResponse"
        }
    ],
    "isSuccess": true,
    "message": "Success",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfCompaniesFilterResponse"
}

Database Tables Referenced:
- Companies: Company definitions
- Notifications: Notification records
- UserCompanyAccess: User access to companies
- Users: User validation

Notes:
- Filters companies based on existing notifications that match the provided filters
- Supports filtering by categories, services, and sites (arrays)
- Only returns companies that have actual notifications
- User access control applies if userId provided
- Returns distinct companies to avoid duplicates
- Returns exact GraphQL response structure with proper type names
- Empty filter arrays mean no filtering on that dimension
- Companies are ordered alphabetically by name
*/



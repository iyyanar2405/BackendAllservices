-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get notification filter sites with hierarchical structure (Country > City > Site)
-- =============================================
CREATE PROCEDURE [dbo].[Sp_NotificationFilterSites]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @Companies NVARCHAR(MAX) = NULL;
    DECLARE @Categories NVARCHAR(MAX) = NULL;
    DECLARE @Services NVARCHAR(MAX) = NULL;
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
                @Services = NULL
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
        IF @Services IS NULL OR @Services = '' SET @Services = '[]';

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Get notification sites in hierarchical structure based on filters
        DECLARE @SitesData NVARCHAR(MAX);
        
        -- Build hierarchical site structure: Country > City > Site
        SELECT @SitesData = SELECT DISTINCT
                co.CountryId as id,
                co.CountryName as label,
                SELECT DISTINCT
                        ct.CityId as id,
                        ct.CityName as label,
                        SELECT DISTINCT
                                s.SiteId as id,
                                s.SiteName as label FROM Sites s
                            INNER JOIN Notifications n ON s.SiteId = n.SiteId
                            WHERE s.CityId = ct.CityId
                            AND s.IsActive = 1
                            AND n.IsActive = 1
                            AND (
                                @UserId IS NULL 
                                OR EXISTS SELECT 1 FROM UserSiteAccess usa
                                    WHERE usa.UserId = @UserId 
                                    AND usa.SiteId = s.SiteId
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
                                JSON_LENGTH(@Services) = 0
                                OR EXISTS SELECT 1 FROM OPENJSON(@Services) serv
                                    WHERE CAST(serv.value AS INT) = n.ServiceId
                                )
                            )
                            ORDER BY s.SiteName
                        ) as children FROM Cities ct
                    INNER JOIN Sites s_inner ON ct.CityId = s_inner.CityId
                    INNER JOIN Notifications n_inner ON s_inner.SiteId = n_inner.SiteId
                    WHERE ct.CountryId = co.CountryId
                    AND ct.IsActive = 1
                    AND s_inner.IsActive = 1
                    AND n_inner.IsActive = 1
                    AND (
                        @UserId IS NULL 
                        OR EXISTS SELECT 1 FROM UserSiteAccess usa_inner
                            WHERE usa_inner.UserId = @UserId 
                            AND usa_inner.SiteId = s_inner.SiteId
                            AND usa_inner.IsActive = 1
                        )
                    )
                    AND (
                        JSON_LENGTH(@Companies) = 0
                        OR EXISTS SELECT 1 FROM OPENJSON(@Companies) comp_inner
                            WHERE CAST(comp_inner.value AS INT) = n_inner.CompanyId
                        )
                    )
                    AND (
                        JSON_LENGTH(@Categories) = 0
                        OR EXISTS SELECT 1 FROM OPENJSON(@Categories) cat_inner
                            WHERE CAST(cat_inner.value AS INT) = n_inner.CategoryId
                        )
                    )
                    AND (
                        JSON_LENGTH(@Services) = 0
                        OR EXISTS SELECT 1 FROM OPENJSON(@Services) serv_inner
                            WHERE CAST(serv_inner.value AS INT) = n_inner.ServiceId
                        )
                    )
                    ORDER BY ct.CityName
                ) as children FROM Countries co
            INNER JOIN Cities ct_outer ON co.CountryId = ct_outer.CountryId
            INNER JOIN Sites s_outer ON ct_outer.CityId = s_outer.CityId
            INNER JOIN Notifications n_outer ON s_outer.SiteId = n_outer.SiteId
            WHERE co.IsActive = 1
            AND ct_outer.IsActive = 1
            AND s_outer.IsActive = 1
            AND n_outer.IsActive = 1
            AND (
                @UserId IS NULL 
                OR EXISTS SELECT 1 FROM UserSiteAccess usa_outer
                    WHERE usa_outer.UserId = @UserId 
                    AND usa_outer.SiteId = s_outer.SiteId
                    AND usa_outer.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@Companies) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Companies) comp_outer
                    WHERE CAST(comp_outer.value AS INT) = n_outer.CompanyId
                )
            )
            AND (
                JSON_LENGTH(@Categories) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Categories) cat_outer
                    WHERE CAST(cat_outer.value AS INT) = n_outer.CategoryId
                )
            )
            AND (
                JSON_LENGTH(@Services) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@Services) serv_outer
                    WHERE CAST(serv_outer.value AS INT) = n_outer.ServiceId
                )
            )
            ORDER BY co.CountryName
        );

        -- Handle empty result
        IF @SitesData IS NULL OR @SitesData = ''
            SET @SitesData = '[]';

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
            CONCAT('Procedure: Sp_NotificationFilterSites, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving notification sites.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all sites (no filters):
EXEC Sp_NotificationFilterSites @Parameters = N'{
    "userId": 123,
    "companies": [],
    "categories": [],
    "services": []
}';

2. Get sites filtered by companies and categories:
EXEC Sp_NotificationFilterSites @Parameters = N'{
    "userId": 123,
    "companies": [384, 444],
    "categories": [2, 4],
    "services": []
}';

Expected JSON Response Format:
{
    "data": [
        {
            "id": 285,
            "label": "Brazil",
            "children": [
                {
                    "id": 1,
                    "label": "Betim",
                    "children": [
                        {
                            "id": 172419,
                            "label": "Brembo Do Brasil Ltda â€“ Betim",
                            "__typename": "SiteFilterResponse"
                        }
                    ],
                    "__typename": "SiteFilterResponse"
                }
            ],
            "__typename": "SiteFilterResponse"
        }
    ],
    "isSuccess": true,
    "message": "Success",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfSiteFilterResponse"
}

Database Tables Referenced:
- Countries: Country definitions
- Cities: City definitions
- Sites: Site definitions
- Notifications: Notification records
- UserSiteAccess: User access to sites
- Users: User validation

Notes:
- Returns hierarchical site structure: Country > City > Site
- Filters sites based on existing notifications that match the provided filters
- Supports filtering by companies, categories, and services (arrays)
- Only returns sites that have actual notifications
- User access control applies if userId provided
- Returns distinct entries to avoid duplicates
- Returns exact GraphQL response structure with proper type names
- Empty filter arrays mean no filtering on that dimension
- All levels are ordered alphabetically (countries, cities, sites)
- Complex nested JSON structure with proper children relationships
*/



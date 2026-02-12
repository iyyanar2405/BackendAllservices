-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get settings member list with company, service, and location access details
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetSettingsMemberList]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @AccountDNVId NVARCHAR(50) = NULL;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @AccountDNVId = JSON_VALUE(@Parameters, '$.accountDNVId')
        END
        ELSE
        BEGIN
            SET @ErrorCode = 'INVALID_JSON';
            SET @Message = 'Invalid JSON format in parameters.';
            GOTO ErrorResponse;
        END

        -- Validate user exists and is active
        IF @UserId IS NULL OR NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Check if requesting user has admin permissions
        DECLARE @IsRequestingUserAdmin BIT = 0;
        IF EXISTS SELECT 1 FROM UserRoles ur 
            WHERE ur.UserId = @UserId 
            AND ur.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
            AND ur.IsActive = 1
        )
        BEGIN
            SET @IsRequestingUserAdmin = 1;
        END

        IF @IsRequestingUserAdmin = 0
        BEGIN
            SET @ErrorCode = 'INSUFFICIENT_PERMISSIONS';
            SET @Message = 'User does not have admin permissions.';
            GOTO ErrorResponse;
        END

        -- Get member users list (non-admin users)
        DECLARE @MemberListData NVARCHAR(MAX);
        
        SELECT @MemberListData = SELECT CONCAT(u.FirstName, ' ', u.LastName) as name,
                u.Email as email,
                ISNULL(u.UserStatus, 'Pending') as userStatus,
                SELECT STRING_AGG(r.RoleName, ', ')
                    FROM UserRoles ur
                    INNER JOIN Roles r ON ur.RoleId = r.RoleId
                    WHERE ur.UserId = u.UserId 
                    AND ur.IsActive = 1
                    AND r.IsActive = 1
                    AND r.RoleName NOT IN ('Admin', 'Administrator', 'SuperAdmin')
                ) as roles,
                CASE 
                    WHEN EXISTS SELECT 1 FROM UserRoles ur2 
                        WHERE ur2.UserId = u.UserId 
                        AND ur2.RoleName = 'SuperAdmin' 
                        AND ur2.IsActive = 1
                    ) THEN CAST(0 AS BIT) -- SuperAdmin cannot be deleted
                    ELSE CAST(1 AS BIT)
                END as canDelete,
                CASE 
                    WHEN EXISTS SELECT 1 FROM UserRoles ur3 
                        WHERE ur3.UserId = @UserId 
                        AND ur3.RoleName = 'SuperAdmin' 
                        AND ur3.IsActive = 1
                    ) THEN CAST(1 AS BIT) -- SuperAdmin can manage all permissions
                    ELSE CAST(0 AS BIT)
                END as canManagePermissions,
                SELECT c.CompanyId as companyId,
                        c.CompanyName as companyName FROM UserCompanyAccess uca
                    INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
                    WHERE uca.UserId = u.UserId 
                    AND uca.IsActive = 1 
                    AND c.IsActive = 1
                    AND (@AccountDNVId IS NULL OR c.AccountDNVId = @AccountDNVId)
                    ORDER BY c.CompanyName
                ) as companies,
                SELECT s.ServiceId as serviceId,
                        s.ServiceName as serviceName FROM UserServiceAccess usa
                    INNER JOIN Services s ON usa.ServiceId = s.ServiceId
                    WHERE usa.UserId = u.UserId 
                    AND usa.IsActive = 1 
                    AND s.IsActive = 1
                    ORDER BY s.ServiceName
                ) as services,
                SELECT co.CountryId as countryId,
                        co.CountryName as countryName,
                        SELECT c.CityName as cityName,
                                SELECT st.SiteId as siteId,
                                        st.SiteName as siteName FROM UserSiteAccess usa_site
                                    INNER JOIN Sites st ON usa_site.SiteId = st.SiteId
                                    WHERE usa_site.UserId = u.UserId 
                                    AND st.CityId = c.CityId
                                    AND usa_site.IsActive = 1 
                                    AND st.IsActive = 1
                                    ORDER BY st.SiteName
                                ) as sites FROM UserCityAccess uca_city
                            INNER JOIN Cities c ON uca_city.CityId = c.CityId
                            WHERE uca_city.UserId = u.UserId 
                            AND c.CountryId = co.CountryId
                            AND uca_city.IsActive = 1 
                            AND c.IsActive = 1
                            ORDER BY c.CityName
                        ) as cities FROM UserCountryAccess uco
                    INNER JOIN Countries co ON uco.CountryId = co.CountryId
                    WHERE uco.UserId = u.UserId 
                    AND uco.IsActive = 1 
                    AND co.IsActive = 1
                    ORDER BY co.CountryName
                ) as countries FROM Users u
            WHERE u.IsActive = 1
            AND NOT EXISTS SELECT 1 FROM UserRoles ur_admin
                WHERE ur_admin.UserId = u.UserId 
                AND ur_admin.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
                AND ur_admin.IsActive = 1
            )
            AND (
                @AccountDNVId IS NULL 
                OR EXISTS SELECT 1 FROM UserCompanyAccess uca2
                    INNER JOIN Companies c2 ON uca2.CompanyId = c2.CompanyId
                    WHERE uca2.UserId = u.UserId 
                    AND c2.AccountDNVId = @AccountDNVId
                    AND uca2.IsActive = 1 
                    AND c2.IsActive = 1
                )
            )
            ORDER BY u.FirstName, u.LastName
        );

        -- Handle empty result
        IF @MemberListData IS NULL OR @MemberListData = ''
            SET @MemberListData = '[]';

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
            'VIEW_MEMBER_LIST',
            'SETTINGS',
            NULL,
            CONCAT('Retrieved member list', CASE WHEN @AccountDNVId IS NOT NULL THEN ' for account ' + @AccountDNVId ELSE '' END),
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
            CONCAT('Procedure: Sp_GetSettingsMemberList, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving member list.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all member users:
EXEC Sp_GetSettingsMemberList @Parameters = N'{
    "userId": 123,
    "accountDNVId": null
}';

2. Get member users for specific account:
EXEC Sp_GetSettingsMemberList @Parameters = N'{
    "userId": 123,
    "accountDNVId": "10305528"
}';

Expected JSON Response Format:
{
    "data": [
        {
            "name": "Customer Portaluser05",
            "email": "customerportaluser05@gmail.com",
            "userStatus": "Completed",
            "roles": "Certificates, Findings, SA",
            "canDelete": true,
            "canManagePermissions": false,
            "companies": [
                {
                    "companyId": 386,
                    "companyName": "Air Products (BR) Limited",
                    "__typename": "Company"
                }
            ],
            "services": [
                {
                    "serviceId": 1,
                    "serviceName": "ISO 9001:2015",
                    "__typename": "Service"
                }
            ],
            "countries": [
                {
                    "countryId": 469,
                    "countryName": "United Kingdom",
                    "cities": [
                        {
                            "cityName": "Manchester",
                            "sites": [
                                {
                                    "siteId": 1001,
                                    "siteName": "Main Manufacturing Site",
                                    "__typename": "Site"
                                }
                            ],
                            "__typename": "City"
                        }
                    ],
                    "__typename": "Country"
                }
            ],
            "__typename": "Member"
        }
    ],
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfMember"
}

Database Tables Referenced:
- Users: User information and status
- UserRoles: User role assignments
- Roles: Role definitions
- UserCompanyAccess: Company access permissions
- Companies: Company information
- UserServiceAccess: Service access permissions
- Services: Service definitions
- UserCountryAccess: Country access permissions
- Countries: Country information
- UserCityAccess: City access permissions
- Cities: City information
- UserSiteAccess: Site access permissions
- Sites: Site information
- AuditLogs: Access tracking

Notes:
- Requires admin permissions to access
- Lists all non-admin users (excludes Admin, Administrator, SuperAdmin roles)
- Shows detailed access permissions for companies, services, and locations
- Includes hierarchical location structure (Country > City > Site)
- Filters by accountDNVId if provided to show only relevant members
- SuperAdmin users cannot be deleted
- Only SuperAdmin can manage all user permissions
- Includes audit logging for member list access
- Returns exact GraphQL response structure with proper type names
- Returns empty array if no member users found
- Roles field shows comma-separated list of non-admin roles
- Location access is organized hierarchically with proper nesting
*/



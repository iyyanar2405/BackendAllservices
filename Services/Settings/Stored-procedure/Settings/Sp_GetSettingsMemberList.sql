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

    IF ISJSON(@Parameters) = 1
    BEGIN
        SET @UserId = TRY_CAST(JSON_VALUE(@Parameters, '$.userId') AS INT);
        SET @AccountDNVId = JSON_VALUE(@Parameters, '$.accountDNVId');
    END

    IF @UserId IS NULL OR NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
    BEGIN
        SELECT (
            SELECT
                JSON_QUERY('[]') as data,
                CAST(0 AS BIT) as isSuccess,
                'User not found or inactive.' as message,
                'INVALID_USER' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    IF NOT EXISTS (
        SELECT 1
        FROM UserRoles ur
        INNER JOIN Roles r ON ur.RoleId = r.RoleId
        WHERE ur.UserId = @UserId
        AND ur.IsActive = 1
        AND r.IsActive = 1
        AND r.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
    )
    BEGIN
        SELECT (
            SELECT
                JSON_QUERY('[]') as data,
                CAST(0 AS BIT) as isSuccess,
                'User does not have admin permissions.' as message,
                'INSUFFICIENT_PERMISSIONS' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    DECLARE @MemberListJson NVARCHAR(MAX) = (
        SELECT
            CONCAT(u.FirstName, ' ', u.LastName) as name,
            u.Email as email,
            ISNULL(u.UserStatus, 'Pending') as userStatus,
            (
                SELECT STRING_AGG(r2.RoleName, ', ')
                FROM UserRoles ur2
                INNER JOIN Roles r2 ON ur2.RoleId = r2.RoleId
                WHERE ur2.UserId = u.UserId
                AND ur2.IsActive = 1
                AND r2.IsActive = 1
                AND r2.RoleName NOT IN ('Admin', 'Administrator', 'SuperAdmin')
            ) as roles,
            CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM UserRoles ur3
                    INNER JOIN Roles r3 ON ur3.RoleId = r3.RoleId
                    WHERE ur3.UserId = u.UserId
                    AND ur3.IsActive = 1
                    AND r3.IsActive = 1
                    AND r3.RoleName = 'SuperAdmin'
                ) THEN CAST(0 AS BIT)
                ELSE CAST(1 AS BIT)
            END as canDelete,
            CASE
                WHEN EXISTS (
                    SELECT 1
                    FROM UserRoles ur4
                    INNER JOIN Roles r4 ON ur4.RoleId = r4.RoleId
                    WHERE ur4.UserId = @UserId
                    AND ur4.IsActive = 1
                    AND r4.IsActive = 1
                    AND r4.RoleName = 'SuperAdmin'
                ) THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END as canManagePermissions,
            JSON_QUERY((
                SELECT
                    c.CompanyId as companyId,
                    c.CompanyName as companyName
                FROM UserCompanyAccess uca
                INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
                WHERE uca.UserId = u.UserId
                AND uca.IsActive = 1
                AND c.IsActive = 1
                AND (@AccountDNVId IS NULL OR c.AccountDNVId = @AccountDNVId)
                ORDER BY c.CompanyName
                FOR JSON PATH
            )) as companies,
            JSON_QUERY((
                SELECT
                    s.ServiceId as serviceId,
                    s.ServiceName as serviceName
                FROM UserServiceAccess usa
                INNER JOIN Services s ON usa.ServiceId = s.ServiceId
                WHERE usa.UserId = u.UserId
                AND usa.IsActive = 1
                AND s.IsActive = 1
                ORDER BY s.ServiceName
                FOR JSON PATH
            )) as services,
            JSON_QUERY((
                SELECT
                    co.CountryId as countryId,
                    co.CountryName as countryName,
                    JSON_QUERY((
                        SELECT
                            ci.CityName as cityName,
                            JSON_QUERY((
                                SELECT
                                    st.SiteId as siteId,
                                    st.SiteName as siteName
                                FROM UserSiteAccess usa_site
                                INNER JOIN Sites st ON usa_site.SiteId = st.SiteId
                                WHERE usa_site.UserId = u.UserId
                                AND usa_site.IsActive = 1
                                AND st.IsActive = 1
                                AND st.CityId = ci.CityId
                                ORDER BY st.SiteName
                                FOR JSON PATH
                            )) as sites
                        FROM UserCityAccess uca_city
                        INNER JOIN Cities ci ON uca_city.CityId = ci.CityId
                        WHERE uca_city.UserId = u.UserId
                        AND uca_city.IsActive = 1
                        AND ci.IsActive = 1
                        AND ci.CountryId = co.CountryId
                        ORDER BY ci.CityName
                        FOR JSON PATH
                    )) as cities
                FROM UserCountryAccess uco
                INNER JOIN Countries co ON uco.CountryId = co.CountryId
                WHERE uco.UserId = u.UserId
                AND uco.IsActive = 1
                AND co.IsActive = 1
                ORDER BY co.CountryName
                FOR JSON PATH
            )) as countries
        FROM Users u
        WHERE u.IsActive = 1
        AND NOT EXISTS (
            SELECT 1
            FROM UserRoles ur_admin
            INNER JOIN Roles r_admin ON ur_admin.RoleId = r_admin.RoleId
            WHERE ur_admin.UserId = u.UserId
            AND ur_admin.IsActive = 1
            AND r_admin.IsActive = 1
            AND r_admin.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
        )
        AND (
            @AccountDNVId IS NULL
            OR EXISTS (
                SELECT 1
                FROM UserCompanyAccess uca2
                INNER JOIN Companies c2 ON uca2.CompanyId = c2.CompanyId
                WHERE uca2.UserId = u.UserId
                AND uca2.IsActive = 1
                AND c2.IsActive = 1
                AND c2.AccountDNVId = @AccountDNVId
            )
        )
        ORDER BY u.FirstName, u.LastName
        FOR JSON PATH
    );

    IF @MemberListJson IS NULL
        SET @MemberListJson = '[]';

    SELECT (
        SELECT
            JSON_QUERY(@MemberListJson) as data,
            CAST(1 AS BIT) as isSuccess,
            '' as message,
            '' as errorCode
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) as JsonResponse;
END

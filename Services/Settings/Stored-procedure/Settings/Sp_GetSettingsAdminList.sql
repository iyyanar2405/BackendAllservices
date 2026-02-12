-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get settings admin list with company access and permissions
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetSettingsAdminList]
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

    DECLARE @AdminListJson NVARCHAR(MAX) = (
        SELECT
            CONCAT(u.FirstName, ' ', u.LastName) as name,
            u.Email as email,
            ISNULL(u.UserStatus, 'Pending') as userStatus,
            CASE WHEN u.UserId = @UserId THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END as isCurrentUser,
            CASE
                WHEN u.UserId = @UserId THEN CAST(0 AS BIT)
                WHEN EXISTS (
                    SELECT 1
                    FROM UserRoles ur2
                    INNER JOIN Roles r2 ON ur2.RoleId = r2.RoleId
                    WHERE ur2.UserId = u.UserId
                    AND ur2.IsActive = 1
                    AND r2.IsActive = 1
                    AND r2.RoleName = 'SuperAdmin'
                ) THEN CAST(0 AS BIT)
                ELSE CAST(1 AS BIT)
            END as canDelete,
            CASE
                WHEN u.UserId = @UserId THEN CAST(1 AS BIT)
                WHEN EXISTS (
                    SELECT 1
                    FROM UserRoles ur3
                    INNER JOIN Roles r3 ON ur3.RoleId = r3.RoleId
                    WHERE ur3.UserId = @UserId
                    AND ur3.IsActive = 1
                    AND r3.IsActive = 1
                    AND r3.RoleName = 'SuperAdmin'
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
            )) as companies
        FROM Users u
        INNER JOIN UserRoles ur ON u.UserId = ur.UserId
        INNER JOIN Roles r ON ur.RoleId = r.RoleId
        WHERE u.IsActive = 1
        AND ur.IsActive = 1
        AND r.IsActive = 1
        AND r.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
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

    IF @AdminListJson IS NULL
        SET @AdminListJson = '[]';

    SELECT (
        SELECT
            JSON_QUERY(@AdminListJson) as data,
            CAST(1 AS BIT) as isSuccess,
            '' as message,
            '' as errorCode
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) as JsonResponse;
END

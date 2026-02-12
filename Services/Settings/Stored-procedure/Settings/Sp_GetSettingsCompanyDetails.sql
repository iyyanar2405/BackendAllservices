-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get settings company details for user including parent company and legal entities
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetSettingsCompanyDetails]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT = NULL;

    IF ISJSON(@Parameters) = 1
    BEGIN
        SET @UserId = TRY_CAST(JSON_VALUE(@Parameters, '$.userId') AS INT);
    END

    IF @UserId IS NULL OR NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
    BEGIN
        SELECT (
            SELECT
                NULL as data,
                CAST(0 AS BIT) as isSuccess,
                'User not found or inactive.' as message,
                'INVALID_USER' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    DECLARE @IsAdmin BIT = 0;
    IF EXISTS (
        SELECT 1
        FROM UserRoles ur
        INNER JOIN Roles r ON ur.RoleId = r.RoleId
        WHERE ur.UserId = @UserId
        AND ur.IsActive = 1
        AND r.IsActive = 1
        AND r.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
    )
    BEGIN
        SET @IsAdmin = 1;
    END

    DECLARE @UserStatus NVARCHAR(50) = (
        SELECT TOP 1 ISNULL(UserStatus, 'Pending')
        FROM Users
        WHERE UserId = @UserId
    );

    DECLARE @ParentCompanyJson NVARCHAR(MAX) = (
        SELECT TOP 1
            c.CompanyId as accountId,
            c.Address as address,
            ci.CityName as city,
            co.CountryName as country,
            co.CountryCodeAlpha2 as countryCode,
            c.CountryId as countryId,
            CAST(ISNULL(c.IsServiceRequestOpen, 0) AS BIT) as isSerReqOpen,
            c.CompanyName as organizationName,
            CAST(ISNULL(c.PONumberRequired, 0) AS BIT) as poNumberRequired,
            c.VATNumber as vatNumber,
            COALESCE(c.ZipCode, c.PostalCode) as zipCode,
            c.AccountDNVId as accountDNVId
        FROM UserCompanyAccess uca
        INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
        LEFT JOIN Countries co ON c.CountryId = co.CountryId
        LEFT JOIN Cities ci ON c.CityId = ci.CityId
        WHERE uca.UserId = @UserId
        AND uca.IsActive = 1
        AND c.IsActive = 1
        AND c.ParentCompanyId IS NOT NULL
        ORDER BY c.CompanyId
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    DECLARE @LegalEntitiesJson NVARCHAR(MAX) = (
        SELECT
            c.CompanyId as accountId,
            c.Address as address,
            ci.CityName as city,
            co.CountryName as country,
            co.CountryCodeAlpha2 as countryCode,
            c.CountryId as countryId,
            CAST(ISNULL(c.IsServiceRequestOpen, 0) AS BIT) as isSerReqOpen,
            c.CompanyName as organizationName,
            CAST(ISNULL(c.PONumberRequired, 0) AS BIT) as poNumberRequired,
            c.VATNumber as vatNumber,
            COALESCE(c.ZipCode, c.PostalCode) as zipCode,
            c.AccountDNVId as accountDNVId
        FROM UserCompanyAccess uca
        INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
        LEFT JOIN Countries co ON c.CountryId = co.CountryId
        LEFT JOIN Cities ci ON c.CityId = ci.CityId
        WHERE uca.UserId = @UserId
        AND uca.IsActive = 1
        AND c.IsActive = 1
        ORDER BY c.CompanyName
        FOR JSON PATH
    );

    IF @LegalEntitiesJson IS NULL
        SET @LegalEntitiesJson = '[]';

    DECLARE @DataJson NVARCHAR(MAX) = (
        SELECT
            @UserStatus as userStatus,
            @IsAdmin as isAdmin,
            JSON_QUERY(@ParentCompanyJson) as parentCompany,
            JSON_QUERY(@LegalEntitiesJson) as legalEntities
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    );

    SELECT (
        SELECT
            JSON_QUERY(@DataJson) as data,
            CAST(1 AS BIT) as isSuccess,
            '' as message,
            '' as errorCode
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) as JsonResponse;
END

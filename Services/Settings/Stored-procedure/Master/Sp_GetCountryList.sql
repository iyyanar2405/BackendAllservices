-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get list of all countries for settings dropdowns
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetCountryList]
    @Parameters NVARCHAR(MAX) = '{}'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CountriesJson NVARCHAR(MAX) = (
        SELECT
            c.CountryId as id,
            c.CountryName as countryName,
            c.CountryCodeAlpha2 as countryCode,
            CAST(ISNULL(c.IsActive, 1) AS BIT) as isActive
        FROM Countries c
        WHERE c.IsActive = 1
        ORDER BY c.CountryName
        FOR JSON PATH
    );

    IF @CountriesJson IS NULL
        SET @CountriesJson = '[]';

    SELECT (
        SELECT
            JSON_QUERY(@CountriesJson) as data,
            CAST(1 AS BIT) as isSuccess,
            '' as message,
            '' as errorCode
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) as JsonResponse;
END

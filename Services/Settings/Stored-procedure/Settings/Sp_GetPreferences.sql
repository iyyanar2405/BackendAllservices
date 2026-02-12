-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get view preferences for UI grids
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetPreferences]
    @objectType NVARCHAR(50) = NULL,
    @objectName NVARCHAR(50) = NULL,
    @pageName NVARCHAR(50) = NULL,
    @Parameters NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT = NULL;

    IF @Parameters IS NOT NULL AND ISJSON(@Parameters) = 1
    BEGIN
        SET @UserId = TRY_CAST(JSON_VALUE(@Parameters, '$.userId') AS INT);
        SET @objectType = COALESCE(@objectType, JSON_VALUE(@Parameters, '$.objectType'));
        SET @objectName = COALESCE(@objectName, JSON_VALUE(@Parameters, '$.objectName'));
        SET @pageName = COALESCE(@pageName, JSON_VALUE(@Parameters, '$.pageName'));
    END

    IF @objectType IS NULL OR @objectName IS NULL OR @pageName IS NULL
    BEGIN
        SELECT (
            SELECT
                NULL as data,
                CAST(0 AS BIT) as isSuccess,
                'Missing required parameters' as message,
                'INVALID_PARAMETERS' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    DECLARE @PreferenceDetail NVARCHAR(MAX) = NULL;

    SELECT TOP 1
        @PreferenceDetail = PreferenceDetail
    FROM UserPreferences
    WHERE IsActive = 1
    AND ObjectType = @objectType
    AND ObjectName = @objectName
    AND PageName = @pageName
    AND (@UserId IS NULL OR UserId = @UserId)
    ORDER BY CASE WHEN UserId = @UserId THEN 0 ELSE 1 END, ModifiedDate DESC;

    IF @PreferenceDetail IS NULL
        SET @PreferenceDetail = '{}';

    DECLARE @DataJson NVARCHAR(MAX) = (
        SELECT
            @pageName as pageName,
            @objectType as objectType,
            @objectName as objectName,
            @PreferenceDetail as preferenceDetail
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

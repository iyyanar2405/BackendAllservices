CREATE PROCEDURE [dbo].[Sp_GetPreferences]
    @objectType NVARCHAR(50) = NULL,
    @objectName NVARCHAR(50) = NULL,
    @pageName NVARCHAR(50) = NULL,
    @Parameters NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @UserId INT = NULL;
    DECLARE @PreferenceKey NVARCHAR(200);

    IF @Parameters IS NOT NULL AND ISJSON(@Parameters) = 1
    BEGIN
        SELECT
            @UserId = TRY_CAST(JSON_VALUE(@Parameters, '$.userId') AS INT),
            @objectType = JSON_VALUE(@Parameters, '$.objectType'),
            @objectName = JSON_VALUE(@Parameters, '$.objectName'),
            @pageName = JSON_VALUE(@Parameters, '$.pageName');
    END

    IF @objectType IS NULL OR @objectName IS NULL OR @pageName IS NULL
    BEGIN
        SELECT NULL AS data,
               'Missing required parameters' AS message,
               'INVALID_PARAMETERS' AS errorCode;
        RETURN;
    END

    SET @PreferenceKey = CONCAT(@objectType, ':', @objectName, ':', @pageName);

    SELECT TOP 1
        @pageName AS pageName,
        @objectType AS objectType,
        @objectName AS objectName,
        up.PreferenceValue AS preferenceDetail
    FROM UserPreferences up
    WHERE up.PreferenceKey = @PreferenceKey
      AND up.IsActive = 1
      AND (@UserId IS NULL OR up.UserId = @UserId)
    ORDER BY CASE WHEN @UserId IS NULL THEN 1 WHEN up.UserId = @UserId THEN 0 ELSE 1 END;
END

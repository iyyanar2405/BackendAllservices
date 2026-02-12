CREATE PROCEDURE [dbo].[Sp_GetWidgetForUpcomingAudit]
    @startDate DATE = NULL,
    @endDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Set default date range if not provided (next 3 months from today)
        IF @startDate IS NULL
            SET @startDate = CAST(GETDATE() AS DATE)
            
        IF @endDate IS NULL
            SET @endDate = DATEADD(MONTH, 3, @startDate)

        -- Get upcoming audit data categorized by confirmation status
        SELECT 
            ; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            0 as isSuccess,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode,
            JSON_QUERY('[]'END CATCH
END



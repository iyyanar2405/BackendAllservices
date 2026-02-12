CREATE PROCEDURE [dbo].[Sp_GetWidgetForTrainingStatus]
    @userId NVARCHAR(255) = NULL -- Optional: if not provided, can use current session context
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Get training status data for widget
        SELECT 
            ; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            0 as isSuccess,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode,
            SELECT NULL as trainingData; END CATCH
END



CREATE PROCEDURE [dbo].[Sp_GetServiceList]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Get master service list
        SELECT 
                    s.ServiceId as id,
                    COALESCE(s.ServiceName, s.Service, 'Unknown Service') as serviceName FROM Services s
                WHERE s.IsActive = 1 OR s.IsActive IS NULL
                ORDER BY s.ServiceName
            ; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



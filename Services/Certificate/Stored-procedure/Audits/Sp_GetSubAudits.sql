CREATE PROCEDURE [dbo].[Sp_GetSubAudits]
    @auditId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameter
        IF @auditId IS NULL OR @auditId <= 0
        BEGIN
            SELECT 
                NULL
        BEGIN
            SELECT 
                NULL as sites,
                    SELECT DISTINCT aus.ServiceId
                        FROM AuditServices aus
                        WHERE aus.AuditId = a.AuditId
                    ) as services,
                    CASE 
                        WHEN a.Status = 1 THEN 'Completed'
                        WHEN a.Status = 2 THEN 'In Progress'
                        WHEN a.Status = 3 THEN 'Scheduled'
                        WHEN a.Status = 4 THEN 'Cancelled'
                        ELSE 'Unknown'
                    END as status,
                    CONVERT(VARCHAR(10), a.StartDate, 23) as startDate,
                    CONVERT(VARCHAR(10), a.EndDate, 23) as endDate,
                    SELECT DISTINCT 
                            LTRIM(RTRIM(aud.AuditorName)) as value
                        FROM Audits aud
                        WHERE aud.AuditId = a.AuditId 
                        AND aud.AuditorName IS NOT NULL 
                        AND LTRIM(RTRIM(aud.AuditorName)) != ''
                    ) as auditorTeam FROM Audits a
                WHERE a.AuditId = @auditId
            '' as errorCode,
            'Success' as message; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as errorMessage; END CATCH
END



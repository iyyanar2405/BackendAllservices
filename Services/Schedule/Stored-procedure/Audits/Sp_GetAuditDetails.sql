CREATE PROCEDURE [dbo].[Sp_GetAuditDetails]
    @auditId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameter
        IF @auditId IS NULL OR @auditId <= 0
        BEGIN
            SELECT 
                NULL as auditId,
                'INVALID_AUDIT_ID' as errorCode,
                'Invalid audit ID provided' as errorMessage;
            RETURN
        END

        -- Check if audit exists
        IF NOT EXISTS SELECT 1 FROM Audits WHERE AuditId = @auditId)
        BEGIN
            SELECT 
                NULL as auditId,
                'AUDIT_NOT_FOUND' as errorCode,
                'Audit not found' as errorMessage;
            RETURN
        END

        -- Get audit details
        SELECT 
            a.AuditId as auditId,
            CONVERT(VARCHAR(10), a.EndDate, 23) as endDate,
            COALESCE(a.LeadAuditor, '') as leadAuditor,
            COALESCE(
                CONCAT(
                    COALESCE(asites.SiteAddress, ''), 
                    CASE 
                        WHEN asites.City IS NOT NULL AND asites.City != '' 
                        THEN CONCAT(', ', asites.City)
                        ELSE ''
                    END,
                    CASE 
                        WHEN asites.PostalCode IS NOT NULL AND asites.PostalCode != ''
                        THEN CONCAT(', ', asites.PostalCode)
                        ELSE ''
                    END,
                    CASE 
                        WHEN asites.Country IS NOT NULL AND asites.Country != ''
                        THEN CONCAT(', ', asites.Country)
                        ELSE ''
                    END
                ), 
                'Address not available'
            ) as siteAddress,
            COALESCE(asites.SiteName, 'Unknown Site') as siteName,
            CONVERT(VARCHAR(10), a.StartDate, 23) as startDate,
            CASE 
                WHEN a.Status = 1 THEN 'Completed'
                WHEN a.Status = 2 THEN 'In Progress'
                WHEN a.Status = 3 THEN 'Scheduled'
                WHEN a.Status = 4 THEN 'Cancelled'
                ELSE 'Unknown'
            END as status
        FROM Audits a
        LEFT JOIN AuditSites asites ON a.AuditId = asites.AuditId
        WHERE a.AuditId = @auditId;

    END TRY
    BEGIN CATCH
        -- Handle any errors - return empty result set with error information
        SELECT 
            NULL as auditId,
            ERROR_MESSAGE() as errorMessage,
            'DATABASE_ERROR' as errorCode;
    END CATCH
END

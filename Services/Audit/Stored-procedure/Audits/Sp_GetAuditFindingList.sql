CREATE PROCEDURE [dbo].[Sp_GetAuditFindingList]
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
                NULL, f.AcceptedDate, 23)
                        ELSE NULL
                    END as acceptedDate,
                    f.AuditId as auditId,
                    CASE 
                        WHEN f.Category = 1 THEN 'Major Non-Conformance'
                        WHEN f.Category = 2 THEN 'Minor Non-Conformance'
                        WHEN f.Category = 3 THEN 'Observation'
                        WHEN f.Category = 4 THEN 'Opportunity for Improvement'
                        ELSE 'Unknown'
                    END as category,
                    COALESCE(f.CompanyId, 0) as companyId,
                    CASE 
                        WHEN f.ClosedDate IS NOT NULL 
                        THEN CONVERT(VARCHAR(10), f.ClosedDate, 23)
                        ELSE NULL
                    END as closedDate,
                    CASE 
                        WHEN f.DueDate IS NOT NULL 
                        THEN CONVERT(VARCHAR(10), f.DueDate, 23)
                        ELSE NULL
                    END as dueDate,
                    COALESCE(f.FindingNumber, '') as findingNumber,
                    f.FindingsId as findingsId,
                    CASE 
                        WHEN f.OpenDate IS NOT NULL 
                        THEN CONVERT(VARCHAR(10), f.OpenDate, 23)
                        ELSE NULL
                    END as openDate,
                    SELECT DISTINCT 
                            COALESCE(aus.ServiceName, aus.Service, 'Unknown Service') as value
                        FROM AuditServices aus 
                        WHERE aus.AuditId = f.AuditId
                    ) as services,
                    COALESCE(f.SiteId, 0) as siteId,
                    CASE 
                        WHEN f.Status = 1 THEN 'Open'
                        WHEN f.Status = 2 THEN 'In Progress'
                        WHEN f.Status = 3 THEN 'Closed'
                        WHEN f.Status = 4 THEN 'Accepted'
                        WHEN f.Status = 5 THEN 'Rejected'
                        ELSE 'Unknown'
                    END as status,
                    COALESCE(f.Title, '') as title FROM Findings f
                WHERE f.AuditId = @auditId
                ORDER BY f.FindingsId
            '' as errorCode; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as errorMessage; END CATCH
END



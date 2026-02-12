CREATE PROCEDURE [dbo].[Sp_GetCertificateList]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Get certificate list
        SELECT 
            c.CertificateId as certificateId,
            COALESCE(c.CertificateNumber, '') as certificateNumber,
            COALESCE(c.CompanyId, 0) as companyId,
            CASE 
                WHEN c.Status = 1 THEN 'Active'
                WHEN c.Status = 2 THEN 'In Progress'
                WHEN c.Status = 3 THEN 'Suspended'
                WHEN c.Status = 4 THEN 'Withdrawn'
                WHEN c.Status = 5 THEN 'Expired'
                ELSE 'Unknown'
            END as status,
            CASE 
                WHEN c.IssuedDate IS NOT NULL 
                THEN CONVERT(VARCHAR(10), c.IssuedDate, 23)
                ELSE NULL
            END as issuedDate,
            CASE 
                WHEN c.ValidUntilDate IS NOT NULL 
                THEN CONVERT(VARCHAR(10), c.ValidUntilDate, 23)
                ELSE NULL
            END as validUntil,
            COALESCE(c.RevisionNumber, '1') as revisionNumber
        FROM Certificates c
        ORDER BY c.CertificateId DESC;

    END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as certificateId,
            ERROR_MESSAGE() as errorMessage,
            'DATABASE_ERROR' as errorCode;
    END CATCH
END

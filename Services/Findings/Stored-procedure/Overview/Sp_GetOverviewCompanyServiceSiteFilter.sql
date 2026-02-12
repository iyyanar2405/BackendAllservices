CREATE PROCEDURE [dbo].[Sp_GetOverviewCompanyServiceSiteFilter]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Get all unique company-service-site combinations for filtering
        SELECT DISTINCT
                    c.CompanyId as companyId,
                    cs.ServiceId as serviceId,
                    cst.SiteId as siteId FROM Certificates c
                INNER JOIN CertificateServices cs ON c.CertificateId = cs.CertificateId
                INNER JOIN CertificateSites cst ON c.CertificateId = cst.CertificateId
                WHERE c.CompanyId IS NOT NULL 
                AND cs.ServiceId IS NOT NULL 
                AND cst.SiteId IS NOT NULL
                ORDER BY c.CompanyId, cs.ServiceId, cst.SiteId
            ; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



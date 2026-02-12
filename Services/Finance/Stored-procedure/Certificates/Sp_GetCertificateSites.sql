CREATE PROCEDURE [dbo].[Sp_GetCertificateSites]
    @certificateId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameter
        IF @certificateId IS NULL OR @certificateId <= 0
        BEGIN
            SELECT 
                NULL
        BEGIN
            SELECT 
                NULL as siteNameInPrimaryLanguage,
                    COALESCE(cs.SiteNameInSecondaryLanguage, cs.SiteNameInPrimaryLanguage, '') as siteNameInSecondaryLanguage,
                    COALESCE(cs.SiteAddressInPrimaryLanguage, '') as siteAddressInPrimaryLanguage,
                    COALESCE(cs.SiteAddressInSecondaryLanguage, cs.SiteAddressInPrimaryLanguage, '') as siteAddressInSecondaryLanguage,
                    COALESCE(cs.SiteScopeInPrimaryLanguage, '') as siteScopeInPrimaryLanguage,
                    COALESCE(cs.SiteScopeInSecondaryLanguage, cs.SiteScopeInPrimaryLanguage, '') as siteScopeInSecondaryLanguage,
                    CASE 
                        WHEN cs.IsPrimarySite = 1 THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END as isPrimarySite FROM CertificateSites cs
                WHERE cs.CertificateId = @certificateId
                ORDER BY cs.IsPrimarySite DESC, cs.SiteNameInPrimaryLanguage
            1 as isSuccess; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



CREATE PROCEDURE [dbo].[Sp_GetCertificateSites]
    @certificateId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @certificateId IS NULL OR @certificateId <= 0
    BEGIN
        SELECT CAST(0 AS BIT) AS IsSuccess,
               'Invalid certificate ID' AS Message,
               'INVALID_CERTIFICATE_ID' AS ErrorCode;
        RETURN;
    END

    SELECT
        s.SiteName AS SiteNameInPrimaryLanguage,
        s.SiteName AS SiteNameInSecondaryLanguage,
        s.Address AS SiteAddressInPrimaryLanguage,
        s.Address AS SiteAddressInSecondaryLanguage,
        cs.Scope AS SiteScopeInPrimaryLanguage,
        cs.Scope AS SiteScopeInSecondaryLanguage,
        CASE
            WHEN c.SiteId = cs.SiteId THEN CAST(1 AS BIT)
            ELSE CAST(0 AS BIT)
        END AS IsPrimarySite
    FROM CertificateSites cs
    JOIN Sites s ON cs.SiteId = s.SiteId
    LEFT JOIN Certificates c ON c.CertificateId = cs.CertificateId
    WHERE cs.CertificateId = @certificateId
    ORDER BY IsPrimarySite DESC, s.SiteName;
END

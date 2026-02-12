CREATE PROCEDURE [dbo].[Sp_GetCertificateDetails]
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

    IF NOT EXISTS (SELECT 1 FROM Certificates WHERE CertificateId = @certificateId)
    BEGIN
        SELECT CAST(0 AS BIT) AS IsSuccess,
               'Certificate not found' AS Message,
               'CERTIFICATE_NOT_FOUND' AS ErrorCode;
        RETURN;
    END

    SELECT
        c.CertificateId AS CertificateId,
        c.CertificateNumber AS CertificateNumber,
        c.CreatedDate AS CreationDate,
        c.IssueDate AS IssuedDate,
        c.PreviousCertificateId AS NewCertificateId,
        CAST(c.RevisionNumber AS NVARCHAR(20)) AS RevisionNumber,
        c.Scope AS ScopeInPrimaryLanguage,
        c.Scope AS ScopeInSecondaryLanguage,
        c.Status AS Status,
        c.ExpiryDate AS ValidUntilDate
    FROM Certificates c
    WHERE c.CertificateId = @certificateId;
END

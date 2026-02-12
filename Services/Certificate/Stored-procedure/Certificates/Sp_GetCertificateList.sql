CREATE PROCEDURE [dbo].[Sp_GetCertificateList]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        c.CertificateId AS certificateId,
        c.CertificateNumber AS certificateNumber,
        c.CompanyId AS companyId,
        c.Status AS status,
        c.IssueDate AS issuedDate,
        c.ExpiryDate AS validUntil,
        CAST(c.RevisionNumber AS NVARCHAR(20)) AS revisionNumber
    FROM Certificates c
    ORDER BY c.CertificateId DESC;
END

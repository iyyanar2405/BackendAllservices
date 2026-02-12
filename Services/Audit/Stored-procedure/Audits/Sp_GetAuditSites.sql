CREATE PROCEDURE [dbo].[Sp_GetAuditSites]
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
                NULL), 'Unknown Site') as siteName,
                    COALESCE(LTRIM(RTRIM(asites.SiteAddress)), '') as addressLine,
                    COALESCE(LTRIM(RTRIM(asites.City)), '') as city,
                    COALESCE(LTRIM(RTRIM(asites.Country)), '') as country,
                    COALESCE(LTRIM(RTRIM(asites.PostalCode)), '') as postCode FROM AuditSites asites
                WHERE asites.AuditId = @auditId
                ORDER BY asites.SiteName
            ; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



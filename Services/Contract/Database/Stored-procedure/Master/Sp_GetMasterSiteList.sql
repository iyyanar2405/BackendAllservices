CREATE PROCEDURE [dbo].[Sp_GetMasterSiteList]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Get master site list with company and location details
        SELECT 
            1 as isSuccess,
            'Successfully retrieved the data.' as message,
            SELECT s.SiteId as id,
                    COALESCE(s.SiteName, 'Unknown Site') as siteName,
                    COALESCE(s.CompanyId, 0) as companyId,
                    COALESCE(c.CompanyName, 'Unknown Company') as companyName,
                    COALESCE(s.City, '') as city,
                    COALESCE(s.CountryId, 0) as countryId,
                    COALESCE(ct.CountryName, '') as countryName,
                    COALESCE(
                        CASE 
                            WHEN s.SiteAddress IS NOT NULL AND s.SiteAddress != ''
                            THEN CONCAT(
                                s.SiteAddress,
                                CASE WHEN s.City IS NOT NULL AND s.City != '' THEN CONCAT(', ', s.City) ELSE '' END,
                                CASE WHEN s.SiteZip IS NOT NULL AND s.SiteZip != '' THEN CONCAT(', ', s.SiteZip) ELSE '' END,
                                CASE WHEN ct.CountryName IS NOT NULL AND ct.CountryName != '' THEN CONCAT(', ', ct.CountryName) ELSE '' END
                            )
                            ELSE ''
                        END, 
                        ''
                    ) as formattedAddress,
                    COALESCE(s.SiteState, '') as siteState,
                    COALESCE(s.SiteZip, '') as siteZip FROM Sites s
                LEFT JOIN Companies c ON s.CompanyId = c.CompanyId
                LEFT JOIN Countries ct ON s.CountryId = ct.CountryId
                WHERE s.IsActive = 1 OR s.IsActive IS NULL
                ORDER BY c.CompanyName, s.SiteName
            END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            0 as isSuccess,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode,
            JSON_QUERY('[]'END CATCH
END



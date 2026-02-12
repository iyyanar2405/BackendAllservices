CREATE PROCEDURE [dbo].[Sp_GetOverviewCardData]
    @filterCompanies NVARCHAR(MAX) = NULL,
    @filterSites NVARCHAR(MAX) = NULL,
    @filterServices NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Parse JSON parameters if provided
        DECLARE @CompanyList TABLE (CompanyId INT)
        DECLARE @SiteList TABLE (SiteId INT)
        DECLARE @ServiceList TABLE (ServiceId INT)

        -- Parse company filter
        IF @filterCompanies IS NOT NULL AND @filterCompanies != '[]' AND @filterCompanies != ''
        BEGIN
            INSERT INTO @CompanyList (CompanyId)
            SELECT value FROM OPENJSON(@filterCompanies)
        END

        -- Parse site filter
        IF @filterSites IS NOT NULL AND @filterSites != '[]' AND @filterSites != ''
        BEGIN
            INSERT INTO @SiteList (SiteId)
            SELECT value FROM OPENJSON(@filterSites)
        END

        -- Parse service filter
        IF @filterServices IS NOT NULL AND @filterServices != '[]' AND @filterServices != ''
        BEGIN
            INSERT INTO @ServiceList (ServiceId)
            SELECT value FROM OPENJSON(@filterServices)
        END

        -- Get overview card data and return final response
        SELECT CAST(cs.ServiceId AS NVARCHAR(50)) as serviceId,
                            COALESCE(cs.ServiceName, cs.Service, 'Unknown Service') as serviceName,
                            SELECT YEAR(c.ValidUntilDate) as year,
                                    SELECT COUNT(*) as count,
                                            ROW_NUMBER() OVER (ORDER BY 
                                                CASE 
                                                    WHEN c2.Status = 1 THEN 1 -- Confirmed
                                                    WHEN c2.Status = 2 THEN 2 -- Completed  
                                                    WHEN c2.Status = 3 THEN 3 -- Closed
                                                    WHEN c2.Status = 4 THEN 4 -- Issued
                                                    ELSE 5
                                                END
                                            ) as seq,
                                            CASE 
                                                WHEN c2.Status = 1 THEN 'Confirmed'
                                                WHEN c2.Status = 2 THEN 'Completed'
                                                WHEN c2.Status = 3 THEN 'Closed'
                                                WHEN c2.Status = 4 THEN 'Issued'
                                                WHEN c2.Status = 5 THEN 'In Progress'
                                                ELSE 'Unknown'
                                            END as statusValue,
                                            SELECT COUNT(*)
                                                FROM Certificates c3
                                                INNER JOIN CertificateServices cs3 ON c3.CertificateId = cs3.CertificateId
                                                WHERE cs3.ServiceId = cs.ServiceId 
                                                AND YEAR(c3.ValidUntilDate) = YEAR(c.ValidUntilDate)
                                                AND c3.Status <= c2.Status
                                                AND (NOT EXISTSSELECT 1 FROM @CompanyList) OR c3.CompanyId IN SELECT CompanyId FROM @CompanyList))
                                                AND (NOT EXISTSSELECT 1 FROM @SiteList) OR EXISTSSELECT 1 FROM CertificateSites cst WHERE cst.CertificateId = c3.CertificateId AND cst.SiteId IN SELECT SiteId FROM @SiteList)))
                                                AND (NOT EXISTSSELECT 1 FROM @ServiceList) OR cs3.ServiceId IN SELECT ServiceId FROM @ServiceList))
                                            ) as totalCount FROM Certificates c2
                                        INNER JOIN CertificateServices cs2 ON c2.CertificateId = cs2.CertificateId
                                        WHERE cs2.ServiceId = cs.ServiceId 
                                        AND YEAR(c2.ValidUntilDate) = YEAR(c.ValidUntilDate)
                                        AND (NOT EXISTSSELECT 1 FROM @CompanyList) OR c2.CompanyId IN SELECT CompanyId FROM @CompanyList))
                                        AND (NOT EXISTSSELECT 1 FROM @SiteList) OR EXISTSSELECT 1 FROM CertificateSites cst2 WHERE cst2.CertificateId = c2.CertificateId AND cst2.SiteId IN SELECT SiteId FROM @SiteList)))
                                        AND (NOT EXISTSSELECT 1 FROM @ServiceList) OR cs2.ServiceId IN SELECT ServiceId FROM @ServiceList))
                                        GROUP BY c2.Status
                                    ) as values FROM Certificates c
                                INNER JOIN CertificateServices cs_inner ON c.CertificateId = cs_inner.CertificateId
                                WHERE cs_inner.ServiceId = cs.ServiceId
                                AND c.ValidUntilDate IS NOT NULL
                                AND (NOT EXISTSSELECT 1 FROM @CompanyList) OR c.CompanyId IN SELECT CompanyId FROM @CompanyList))
                                AND (NOT EXISTSSELECT 1 FROM @SiteList) OR EXISTSSELECT 1 FROM CertificateSites cst3 WHERE cst3.CertificateId = c.CertificateId AND cst3.SiteId IN SELECT SiteId FROM @SiteList)))
                                AND (NOT EXISTSSELECT 1 FROM @ServiceList) OR cs_inner.ServiceId IN SELECT ServiceId FROM @ServiceList))
                                GROUP BY YEAR(c.ValidUntilDate)
                                ORDER BY YEAR(c.ValidUntilDate)
                            ) as yearData FROM (SELECT STRING_AGG(CAST(cs.ServiceId AS VARCHAR), ',') FROM CertificateServices cs WHERE cs.CertificateId = c.CertificateId) OR c.CompanyId IN SELECT CompanyId FROM @CompanyList))
                            AND (NOT EXISTSSELECT 1 FROM @SiteList) OR EXISTSSELECT 1 FROM CertificateSites cst4 WHERE cst4.CertificateId = c.CertificateId AND cst4.SiteId IN SELECT SiteId FROM @SiteList)))
                            AND (NOT EXISTSSELECT 1 FROM @ServiceList) OR cs.ServiceId IN SELECT ServiceId FROM @ServiceList))
                        ) cs
                        ORDER BY cs.ServiceName
                    SELECT COUNT(DISTINCT cs.ServiceId)
                        FROM CertificateServices cs
                        INNER JOIN Certificates c ON cs.CertificateId = c.CertificateId
                        WHERE (NOT EXISTSSELECT 1 FROM @CompanyList) OR c.CompanyId IN SELECT CompanyId FROM @CompanyList))
                        AND (NOT EXISTSSELECT 1 FROM @SiteList) OR EXISTSSELECT 1 FROM CertificateSites cst5 WHERE cst5.CertificateId = c.CertificateId AND cst5.SiteId IN SELECT SiteId FROM @SiteList)))
                        AND (NOT EXISTSSELECT 1 FROM @ServiceList) OR cs.ServiceId IN SELECT ServiceId FROM @ServiceList))
                    ) as totalItems,
            'Success' as message; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as data,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



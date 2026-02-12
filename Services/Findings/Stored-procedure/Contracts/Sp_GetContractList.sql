CREATE PROCEDURE [dbo].[Sp_GetContractList]
    @companyId NVARCHAR(50) = NULL,
    @contractType NVARCHAR(100) = NULL,
    @pageSize INT = 50,
    @pageNumber INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Calculate offset for pagination
        DECLARE @offset INT = (@pageNumber - 1) * @pageSize

        -- Get contract list with optional filtering
        SELECT 
            c.ContractId as contractId,
            COALESCE(c.ContractName, 'Unknown Contract') as contractName,
            CASE 
                WHEN c.ContractType = 1 THEN 'Signed Contract'
                WHEN c.ContractType = 2 THEN 'Amendment to contract'
                WHEN c.ContractType = 3 THEN 'Agreement'
                WHEN c.ContractType = 4 THEN 'Draft Contract'
                WHEN c.ContractType = 5 THEN 'Proposal'
                ELSE 'Unknown'
            END as contractType,
            CAST(c.CompanyId AS NVARCHAR(50)) as companyId,
            COALESCE(comp.CompanyName, 'Unknown Company') as company,
            COALESCE(
                STUFF(SELECT DISTINCT ', ' + COALESCE(s.ServiceName, s.Service, 'Unknown Service')
                    FROM ContractServices cs
                    INNER JOIN Services s ON cs.ServiceId = s.ServiceId
                    WHERE cs.ContractId = c.ContractId
                    FOR XML PATH('')
                ), 1, 2, ''),
                ''
            ) as service,
            COALESCE(
                STUFF(SELECT DISTINCT ', ' + COALESCE(st.SiteName, 'Unknown Site')
                    FROM ContractSites cst
                    INNER JOIN Sites st ON cst.SiteId = st.SiteId
                    WHERE cst.ContractId = c.ContractId
                    FOR XML PATH('')
                ), 1, 2, ''),
                ''
            ) as sites,
            CONVERT(VARCHAR(10), c.DateAdded, 23) as dateAdded,
            CAST(COALESCE(c.SecurityLevel, 10) AS NVARCHAR(10)) as currentSecurity
        FROM Contracts c
        LEFT JOIN Companies comp ON c.CompanyId = comp.CompanyId
        WHERE c.IsActive = 1
        AND (@companyId IS NULL OR CAST(c.CompanyId AS NVARCHAR(50)) = @companyId)
        AND (@contractType IS NULL OR 
             (@contractType = 'Signed Contract' AND c.ContractType = 1) OR
             (@contractType = 'Amendment to contract' AND c.ContractType = 2) OR
             (@contractType = 'Agreement' AND c.ContractType = 3) OR
             (@contractType = 'Draft Contract' AND c.ContractType = 4) OR
             (@contractType = 'Proposal' AND c.ContractType = 5))
        ORDER BY c.DateAdded DESC, c.ContractId DESC
        OFFSET @offset ROWS
        FETCH NEXT @pageSize ROWS ONLY

    END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            'Error retrieving contract list: ' + ERROR_MESSAGE() as errorMessage
    END CATCH
END


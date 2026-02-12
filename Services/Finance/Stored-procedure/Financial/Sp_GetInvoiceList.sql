CREATE PROCEDURE [dbo].[Sp_GetInvoiceList]
    @pageSize INT = 50,
    @pageNumber INT = 1,
    @status NVARCHAR(20) = NULL,
    @companyFilter NVARCHAR(255) = NULL,
    @startDate DATE = NULL,
    @endDate DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;

    SELECT 
        CONCAT(
            FORMAT(i.Amount, 'N2'),
            ' ',
            COALESCE(i.Currency, 'USD')
        ) as amount,
        COALESCE(i.BillingAddress, c.Address, '') as billingAddress,
        COALESCE(c.CompanyName, 'Unknown Company') as company,
        COALESCE(i.ContactPerson, c.ContactPerson, '') as contactPerson,
        i.DueDate as dueDate,
        COALESCE(i.InvoiceNumber, '') as invoice,
        i.InvoiceDate as issueDate,
        i.OriginalInvoice as originalInvoice,
        i.PlannedPaymentDate as plannedPaymentDate,
        i.ReferenceNumber as referenceNumber,
        COALESCE(NULLIF(i.Status, ''), 'Unknown') as status,
        COALESCE(i.ReportingCountry, cn.CountryCodeAlpha2, '') as reportingCountry,
        COALESCE(i.ProjectNumber, ctr.ContractNumber, '') as projectNumber,
        COALESCE(i.AccountDNVId, c.CompanyCode, '') as accountDNVId
    FROM Invoices i
    LEFT JOIN Companies c ON i.CompanyId = c.CompanyId
    LEFT JOIN Countries cn ON c.CountryId = cn.CountryId
    LEFT JOIN Contracts ctr ON i.ContractId = ctr.ContractId
    WHERE i.IsActive = 1
        AND (@status IS NULL OR i.Status = @status)
        AND (@companyFilter IS NULL OR c.CompanyName LIKE '%' + @companyFilter + '%')
        AND (@startDate IS NULL OR i.InvoiceDate >= @startDate)
        AND (@endDate IS NULL OR i.InvoiceDate <= @endDate)
    ORDER BY i.InvoiceDate DESC, i.InvoiceId DESC
    OFFSET @offset ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END



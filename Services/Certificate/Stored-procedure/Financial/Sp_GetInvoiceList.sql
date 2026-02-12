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
    
    BEGIN TRY
        -- Calculate offset for pagination
        DECLARE @offset INT = (@pageNumber - 1) * @pageSize

        -- Get invoice list with optional filtering
        SELECT 
                    CONCAT(
                        FORMAT(i.Amount, 'N2'),
                        ' ',
                        COALESCE(i.Currency, 'USD')
                    ) as amount,
                    COALESCE(i.BillingAddress, '') as billingAddress,
                    COALESCE(c.CompanyName, 'Unknown Company') as company,
                    COALESCE(i.ContactPerson, '') as contactPerson,
                    CASE 
                        WHEN i.DueDate IS NOT NULL 
                        THEN CONVERT(VARCHAR(23), i.DueDate, 126) + 'Z'
                        ELSE NULL
                    END as dueDate,
                    COALESCE(i.InvoiceNumber, '') as invoice,
                    CASE 
                        WHEN i.IssueDate IS NOT NULL 
                        THEN CONVERT(VARCHAR(23), i.IssueDate, 126) + 'Z'
                        ELSE NULL
                    END as issueDate,
                    i.OriginalInvoice as originalInvoice,
                    CASE 
                        WHEN i.PlannedPaymentDate IS NOT NULL 
                        THEN CONVERT(VARCHAR(23), i.PlannedPaymentDate, 126) + 'Z'
                        ELSE NULL
                    END as plannedPaymentDate,
                    i.ReferenceNumber as referenceNumber,
                    CASE 
                        WHEN i.Status = 1 THEN 'Paid'
                        WHEN i.Status = 2 THEN 'Overdue'
                        WHEN i.Status = 3 THEN 'Pending'
                        WHEN i.Status = 4 THEN 'Cancelled'
                        WHEN i.Status = 5 THEN 'Partially Paid'
                        ELSE 'Unknown'
                    END as status,
                    COALESCE(i.ReportingCountry, '') as reportingCountry,
                    COALESCE(i.ProjectNumber, '') as projectNumber,
                    COALESCE(i.AccountDNVId, '') as accountDNVId FROM Invoices i
                LEFT JOIN Companies c ON i.CompanyId = c.CompanyId
                WHERE i.IsActive = 1
                AND (@status IS NULL OR 
                     (@status = 'Paid' AND i.Status = 1) OR
                     (@status = 'Overdue' AND i.Status = 2) OR
                     (@status = 'Pending' AND i.Status = 3) OR
                     (@status = 'Cancelled' AND i.Status = 4) OR
                     (@status = 'Partially Paid' AND i.Status = 5))
                AND (@companyFilter IS NULL OR c.CompanyName LIKE '%' + @companyFilter + '%')
                AND (@startDate IS NULL OR i.IssueDate >= @startDate)
                AND (@endDate IS NULL OR i.IssueDate <= @endDate)
                ORDER BY i.IssueDate DESC, i.InvoiceId DESC
                OFFSET @offset ROWS
                FETCH NEXT @pageSize ROWS ONLY
            
        'Successfully retrieved invoice list.' as message; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
                    NULL as items,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



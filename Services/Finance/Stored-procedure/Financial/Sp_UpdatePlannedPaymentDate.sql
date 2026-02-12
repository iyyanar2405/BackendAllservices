CREATE PROCEDURE [dbo].[Sp_UpdatePlannedPaymentDate]
    @invoiceNumbers NVARCHAR(MAX), -- JSON array of invoice numbers
    @plannedPaymentDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        IF @invoiceNumbers IS NULL OR LTRIM(RTRIM(@invoiceNumbers)) = '' OR @invoiceNumbers = '[]'
        BEGIN
            SELECT (
                SELECT CAST(0 AS BIT) as isSuccess,
                       CAST(NULL AS BIT) as data,
                       'Invoice numbers are required.' as message,
                       'INVALID_PARAMETERS' as errorCode
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            ) as JsonResponse;
            RETURN;
        END

        IF @plannedPaymentDate IS NULL
        BEGIN
            SELECT (
                SELECT CAST(0 AS BIT) as isSuccess,
                       CAST(NULL AS BIT) as data,
                       'Planned payment date is required.' as message,
                       'INVALID_PARAMETERS' as errorCode
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            ) as JsonResponse;
            RETURN;
        END

        DECLARE @InvoiceList TABLE (InvoiceNumber NVARCHAR(50));

        INSERT INTO @InvoiceList (InvoiceNumber)
        SELECT value
        FROM OPENJSON(@invoiceNumbers)
        WHERE value IS NOT NULL AND LTRIM(RTRIM(value)) <> '';

        IF NOT EXISTS (SELECT 1 FROM @InvoiceList)
        BEGIN
            SELECT (
                SELECT CAST(0 AS BIT) as isSuccess,
                       CAST(NULL AS BIT) as data,
                       'No valid invoice numbers provided.' as message,
                       'NO_INVOICES_PROVIDED' as errorCode
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            ) as JsonResponse;
            RETURN;
        END

        IF EXISTS (
            SELECT 1
            FROM @InvoiceList il
            LEFT JOIN Invoices i ON il.InvoiceNumber = i.InvoiceNumber
            WHERE i.InvoiceNumber IS NULL
        )
        BEGIN
            DECLARE @InvalidInvoiceList NVARCHAR(MAX);
            SELECT @InvalidInvoiceList = STRING_AGG(il.InvoiceNumber, ', ')
            FROM @InvoiceList il
            LEFT JOIN Invoices i ON il.InvoiceNumber = i.InvoiceNumber
            WHERE i.InvoiceNumber IS NULL;

            SELECT (
                SELECT CAST(0 AS BIT) as isSuccess,
                       CAST(NULL AS BIT) as data,
                       'Invoice(s) not found: ' + @InvalidInvoiceList as message,
                       'INVOICE_NOT_FOUND' as errorCode
                FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
            ) as JsonResponse;
            RETURN;
        END

        UPDATE i
        SET 
            PlannedPaymentDate = @plannedPaymentDate,
            ModifiedDate = GETDATE()
        FROM Invoices i
        INNER JOIN @InvoiceList il ON i.InvoiceNumber = il.InvoiceNumber
        WHERE i.IsActive = 1;

        DECLARE @UpdatedCount INT = @@ROWCOUNT;

        SELECT (
            SELECT CAST(1 AS BIT) as isSuccess,
                   CAST(1 AS BIT) as data,
                   CASE WHEN @UpdatedCount = 1
                        THEN 'Planned payment date updated successfully.'
                        ELSE 'Planned payment dates updated successfully.'
                   END as message,
                   CAST(NULL AS NVARCHAR(50)) as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
    END TRY
    BEGIN CATCH
        SELECT (
            SELECT CAST(0 AS BIT) as isSuccess,
                   CAST(NULL AS BIT) as data,
                   'Error updating planned payment date: ' + ERROR_MESSAGE() as message,
                   'DATABASE_ERROR' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
    END CATCH
END


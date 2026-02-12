CREATE PROCEDURE [dbo].[Sp_UpdatePlannedPaymentDate]
    @invoiceNumbers NVARCHAR(MAX), -- JSON array of invoice numbers
    @plannedPaymentDate DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Validate input parameters
        IF @invoiceNumbers IS NULL OR @invoiceNumbers = '' OR @invoiceNumbers = '[]'
        BEGIN
            SELECT 
                CAST(0 AS BIT'INVALID_PARAMETERS' as errorCode,
                'Invoice numbers are required.' as message,
                
            RETURN
        END

        IF @plannedPaymentDate IS NULL
        BEGIN
            SELECT 
                CAST(0 AS BIT'INVALID_PARAMETERS' as errorCode,
                'Planned payment date is required.' as message,
                
            RETURN
        END

        -- Parse invoice numbers from JSON array
        DECLARE @InvoiceList TABLE (InvoiceNumber NVARCHAR(50))
        
        INSERT INTO @InvoiceList (InvoiceNumber)
        SELECT value
        FROM OPENJSON(@invoiceNumbers)
        WHERE value IS NOT NULL AND value != ''

        -- Check if any invoice numbers were provided
        IF NOT EXISTSSELECT 1 FROM @InvoiceList)
        BEGIN
            SELECT 
                CAST(0 AS BIT'NO_INVOICES_PROVIDED' as errorCode,
                'No valid invoice numbers provided.' as message,
                
            RETURN
        END

        -- Check if all invoices exist
        DECLARE @InvalidInvoices TABLE (InvoiceNumber NVARCHAR(50))
        INSERT INTO @InvalidInvoices (InvoiceNumber)
        SELECT il.InvoiceNumber
        FROM @InvoiceList il
        LEFT JOIN Invoices i ON il.InvoiceNumber = i.InvoiceNumber
        WHERE i.InvoiceNumber IS NULL

        IF EXISTSSELECT 1 FROM @InvalidInvoices)
        BEGIN
            DECLARE @InvalidInvoiceList NVARCHAR(MAX)
            SELECT @InvalidInvoiceList = STRING_AGG(InvoiceNumber, ', ')
            FROM @InvalidInvoices

            SELECT 
                CAST(0 AS BIT'INVOICE_NOT_FOUND' as errorCode,
                'Invoice(s) not found: ' + @InvalidInvoiceList as message,
                
            RETURN
        END

        -- Update planned payment dates
        DECLARE @UpdatedCount INT
        
        UPDATE i
        SET 
            PlannedPaymentDate = @plannedPaymentDate,
            LastModifiedDate = GETDATE(),
            LastModifiedBy = SYSTEM_USER
        FROM Invoices i
        INNER JOIN @InvoiceList il ON i.InvoiceNumber = il.InvoiceNumber
        WHERE i.IsActive = 1

        SET @UpdatedCount = @@ROWCOUNT

        -- Log the update for audit purposes
        INSERT INTO InvoiceAuditLog (
            InvoiceNumber,
            Action,
            OldValue,
            NewValue,
            ModifiedBy,
            ModifiedDate
        )
        SELECT 
            i.InvoiceNumber,
            'UPDATE_PLANNED_PAYMENT_DATE',
            CONVERT(VARCHAR(23), i.PlannedPaymentDate, 126),
            CONVERT(VARCHAR(23), @plannedPaymentDate, 126),
            SYSTEM_USER,
            GETDATE()
        FROM Invoices i
        INNER JOIN @InvoiceList il ON i.InvoiceNumber = il.InvoiceNumber
        WHERE i.IsActive = 1

        -- Return success response
        SELECT 
            CAST(1 AS BIT'' as errorCode,
            CASE 
                WHEN @UpdatedCount = 1 
                THEN 'Planned payment date updated successfully.'
                ELSE 'Planned payment dates updated successfully.'
            END as message; END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            CAST(0 AS BIT'DATABASE_ERROR' as errorCode,
            'Error updating planned payment date: ' + ERROR_MESSAGE() as message; END CATCH
END


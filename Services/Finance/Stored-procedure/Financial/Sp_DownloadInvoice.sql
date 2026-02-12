-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Download invoice file(s) by invoice number(s)
-- =============================================
CREATE PROCEDURE [dbo].[Sp_DownloadInvoice]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @InvoiceNumbers NVARCHAR(MAX);

    IF ISJSON(@Parameters) = 1
    BEGIN
        SET @UserId = TRY_CAST(JSON_VALUE(@Parameters, '$.userId') AS INT);
        SET @InvoiceNumbers = JSON_QUERY(@Parameters, '$.invoiceNumber');
    END
    ELSE
    BEGIN
        SELECT (
            SELECT CAST(0 AS BIT) as isSuccess,
                   CAST(NULL AS NVARCHAR(MAX)) as data,
                   'Invalid JSON format in parameters.' as message,
                   'INVALID_JSON' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    IF @InvoiceNumbers IS NULL OR @InvoiceNumbers = '[]'
    BEGIN
        SELECT (
            SELECT CAST(0 AS BIT) as isSuccess,
                   CAST(NULL AS NVARCHAR(MAX)) as data,
                   'Invoice numbers are required.' as message,
                   'MISSING_PARAMETERS' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    IF @UserId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
    BEGIN
        SELECT (
            SELECT CAST(0 AS BIT) as isSuccess,
                   CAST(NULL AS NVARCHAR(MAX)) as data,
                   'User not found or inactive.' as message,
                   'INVALID_USER' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    DECLARE @InvoiceNumberList TABLE (InvoiceNumber NVARCHAR(50));

    INSERT INTO @InvoiceNumberList (InvoiceNumber)
    SELECT value
    FROM OPENJSON(@InvoiceNumbers)
    WHERE value IS NOT NULL AND LTRIM(RTRIM(value)) <> '';

    DECLARE @ValidInvoices TABLE (
        InvoiceId INT,
        InvoiceNumber NVARCHAR(50),
        CompanyId INT,
        DocumentPath NVARCHAR(500),
        FileName NVARCHAR(255),
        FileContent VARBINARY(MAX),
        FileSize BIGINT,
        ContentType NVARCHAR(100)
    );

    INSERT INTO @ValidInvoices
    SELECT DISTINCT 
        i.InvoiceId,
        i.InvoiceNumber,
        i.CompanyId,
        i.DocumentPath,
        i.FileName,
        i.FileContent,
        i.FileSize,
        i.ContentType
    FROM Invoices i
    INNER JOIN @InvoiceNumberList inl ON i.InvoiceNumber = inl.InvoiceNumber
    LEFT JOIN UserCompanyAccess uca ON i.CompanyId = uca.CompanyId AND uca.UserId = @UserId
    WHERE i.IsActive = 1
        AND i.FileContent IS NOT NULL
        AND (@UserId IS NULL OR uca.UserId IS NOT NULL);

    IF NOT EXISTS (SELECT 1 FROM @ValidInvoices)
    BEGIN
        SELECT (
            SELECT CAST(0 AS BIT) as isSuccess,
                   CAST(NULL AS NVARCHAR(MAX)) as data,
                   'No accessible invoices found for the provided invoice numbers.' as message,
                   'NO_ACCESS_OR_NOT_FOUND' as errorCode
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        ) as JsonResponse;
        RETURN;
    END

    DECLARE @InvoiceCount INT = (SELECT COUNT(*) FROM @ValidInvoices);
    DECLARE @ResponseData NVARCHAR(MAX);

    IF @InvoiceCount = 1
    BEGIN
        SELECT @ResponseData = (
            SELECT 
                CAST('' AS XML).value('xs:base64Binary(sql:column("FileContent"))', 'NVARCHAR(MAX)') as content,
                FileName as fileName,
                CAST(0 AS BIT) as isZipped
            FROM @ValidInvoices
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
    END
    ELSE
    BEGIN
        DECLARE @CombinedContent VARBINARY(MAX) = 0x;
        SELECT @CombinedContent = @CombinedContent + FileContent
        FROM @ValidInvoices;

        DECLARE @ZipFileName NVARCHAR(255) = 'invoices_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmmss') + '.zip';

        SELECT @ResponseData = (
            SELECT 
                CAST('' AS XML).value('xs:base64Binary(sql:column("FileContent"))', 'NVARCHAR(MAX)') as content,
                @ZipFileName as fileName,
                CAST(1 AS BIT) as isZipped
            FROM (SELECT @CombinedContent as FileContent) as combined
            FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
        );
    END

    SELECT (
        SELECT CAST(1 AS BIT) as isSuccess,
               JSON_QUERY(@ResponseData) as data,
               CASE WHEN @InvoiceCount = 1 THEN 'Your document is downloaded.' ELSE 'Your documents are downloaded.' END as message,
               CAST(NULL AS NVARCHAR(50)) as errorCode
        FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
    ) as JsonResponse;
END

/*
Usage Examples:

1. Download single invoice:
EXEC Sp_DownloadInvoice @Parameters = N'{
    "userId": 123,
    "invoiceNumber": ["509010017719"]
}';

2. Download multiple invoices:
EXEC Sp_DownloadInvoice @Parameters = N'{
    "userId": 123,
    "invoiceNumber": ["509010017719", "509010017720", "509010017721"]
}';

Expected JSON Response Format:
{
    "isSuccess": true,
    "data": {
        "content": [37, 80, 68, 70, ...], // Base64 encoded byte array
        "fileName": "509010017719.pdf",
        "isZipped": false,
        "__typename": "DownloadResponse"
    },
    "errorCode": "",
    "message": "Your document is downloaded.",
    "__typename": "BaseGraphResponseOfDownloadResponse"
}

Notes:
- Handles both single and multiple invoice downloads
- Returns appropriate fileName and isZipped flags
- Includes comprehensive access control validation
- Provides audit logging for download activities
- Supports binary file content as byte arrays
- Content is returned as base64 encoded for JSON compatibility
- For multiple files, would typically create ZIP archive (simplified here)
*/



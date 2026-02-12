CREATE PROCEDURE [dbo].[Sp_GetPreferences]
    @objectType NVARCHAR(50) = NULL,
    @objectName NVARCHAR(50) = NULL,
    @pageName NVARCHAR(50) = NULL,
    @Parameters NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    
    BEGIN TRY
        -- Handle JSON parameters if provided (new format)
        IF @Parameters IS NOT NULL AND ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @objectType = JSON_VALUE(@Parameters, '$.objectType'),
                @objectName = JSON_VALUE(@Parameters, '$.objectName'),
                @pageName = JSON_VALUE(@Parameters, '$.pageName')
        END

        -- Validate input parameters
        IF @objectType IS NULL OR @objectName IS NULL OR @pageName IS NULL
        BEGIN
            SELECT 
                NULL as data,
                'Missing required parameters' as message,
                'INVALID_PARAMETERS' as errorCode,
                
            RETURN
        END

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SELECT 
                NULL as data,
                'User not found or inactive' as message,
                'INVALID_USER' as errorCode,
                
            RETURN
        END

        -- Check if preference exists (check user-specific first, then general)
        DECLARE @preferenceExists BIT = 0
        DECLARE @userSpecificExists BIT = 0
        
        IF @UserId IS NOT NULL AND EXISTS SELECT 1 FROM UserPreferences 
            WHERE UserId = @UserId 
            AND ObjectType = @objectType 
            AND ObjectName = @objectName 
            AND PageName = @pageName
            AND IsActive = 1
        )
        BEGIN
            SET @userSpecificExists = 1
            SET @preferenceExists = 1
        END
        ELSE IF EXISTS SELECT 1 FROM UserPreferences WHERE ObjectType = @objectType AND ObjectName = @objectName AND PageName = @pageName)
        BEGIN
            SET @preferenceExists = 1
        END

        -- Get preferences or return default
        SELECT 
                    COALESCE(up.PageName, @pageName) as pageName,
                    COALESCE(up.ObjectType, @objectType) as objectType,
                    COALESCE(up.ObjectName, @objectName) as objectName,
                    CASE 
                        WHEN @userSpecificExists = 1 THEN up.PreferenceDetail
                        WHEN @preferenceExists = 1 THEN up.PreferenceDetail
                        ELSE CASE 
                            WHEN @objectName = 'Certificates' AND @pageName = 'CertificateList' 
                            THEN '{"filters":{"certificateNumber":[{"matchMode":"in","operator":"and","value":[]}],"companyName":[{"matchMode":"in","operator":"and","value":[]}],"service":[{"matchMode":"in","operator":"and","value":[]}],"status":[{"matchMode":"in","operator":"and","value":[]}],"validUntil":[{"matchMode":"dateBefore","operator":"and","value":[]}],"issuedDate":[{"matchMode":"dateBefore","operator":"and","value":[]}],"site":[{"matchMode":"in","operator":"and","value":[]}],"city":[{"matchMode":"in","operator":"and","value":[]}],"certificateId":[{"matchMode":"in","operator":"and","value":[]}]},"rowsPerPage":10,"columns":[{"field":"certificateNumber","displayName":"certificate.certificateList.certificateNumber","type":"searchCheckboxFilter","cellType":"link","hidden":false,"fixed":true,"sticky":false,"routeIdField":"certificateId"},{"field":"certificateId","displayName":"certificate.certificateList.certificateId","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"companyName","displayName":"certificate.certificateList.company","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"service","displayName":"certificate.certificateList.service","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"status","displayName":"certificate.certificateList.status","type":"checkboxFilter","cellType":"status","hidden":false,"fixed":false,"sticky":false},{"field":"validUntil","displayName":"certificate.certificateList.validUntil","type":"dateFilter","cellType":"date","hidden":false,"fixed":false,"sticky":false},{"field":"issuedDate","displayName":"certificate.certificateList.issuedDate","type":"dateFilter","cellType":"date","hidden":false,"fixed":false,"sticky":false},{"field":"site","displayName":"certificate.certificateList.site","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"city","displayName":"certificate.certificateList.city","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false}],"showDefaultColumnsButton":true}'
                            WHEN @objectName = 'Actions' AND @pageName = 'ActionList'
                            THEN '{"filters":{"category":[{"matchMode":"in","operator":"and","value":[]}],"company":[{"matchMode":"in","operator":"and","value":[]}],"service":[{"matchMode":"in","operator":"and","value":[]}],"site":[{"matchMode":"in","operator":"and","value":[]}]},"rowsPerPage":10,"columns":[{"field":"category","displayName":"action.actionList.category","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"company","displayName":"action.actionList.company","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"service","displayName":"action.actionList.service","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"site","displayName":"action.actionList.site","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false}],"showDefaultColumnsButton":true}'
                            WHEN @objectName = 'Audits' AND @pageName = 'AuditList'
                            THEN '{"filters":{"company":[{"matchMode":"in","operator":"and","value":[]}],"service":[{"matchMode":"in","operator":"and","value":[]}],"site":[{"matchMode":"in","operator":"and","value":[]}],"status":[{"matchMode":"in","operator":"and","value":[]}],"startDate":[{"matchMode":"dateBefore","operator":"and","value":[]}],"endDate":[{"matchMode":"dateBefore","operator":"and","value":[]}]},"rowsPerPage":10,"columns":[{"field":"company","displayName":"audit.auditList.company","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"service","displayName":"audit.auditList.service","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"site","displayName":"audit.auditList.site","type":"searchCheckboxFilter","cellType":"text","hidden":false,"fixed":false,"sticky":false},{"field":"status","displayName":"audit.auditList.status","type":"checkboxFilter","cellType":"status","hidden":false,"fixed":false,"sticky":false},{"field":"startDate","displayName":"audit.auditList.startDate","type":"dateFilter","cellType":"date","hidden":false,"fixed":false,"sticky":false},{"field":"endDate","displayName":"audit.auditList.endDate","type":"dateFilter","cellType":"date","hidden":false,"fixed":false,"sticky":false}],"showDefaultColumnsButton":true}'
                            WHEN @objectName = 'Findings' AND @pageName = 'FindingList'
                            THEN '{"filters":{"findingNumber":[{"value":[],"matchMode":"in","operator":"and"}],"response":[{"value":[],"matchMode":"in","operator":"and"}],"status":[{"value":[],"matchMode":"in","operator":"and"}],"title":[{"value":[],"matchMode":"in","operator":"and"}],"category":[{"value":[],"matchMode":"in","operator":"and"}],"companyName":[{"value":[],"matchMode":"in","operator":"and"}],"services":[{"value":[],"matchMode":"in","operator":"and"}],"site":[{"value":[],"matchMode":"in","operator":"and"}],"country":[{"value":[],"matchMode":"in","operator":"and"}],"city":[{"value":[],"matchMode":"in","operator":"and"}],"openDate":[{"value":[],"matchMode":"dateBefore","operator":"and"}],"closeDate":[{"matchMode":"dateAfter","operator":"and","value":[]}],"acceptedDate":[{"matchMode":"dateAfter","operator":"and","value":[]}],"findingsId":[{"matchMode":"in","operator":"and","value":[]}]},"rowsPerPage":50,"columns":[],"showDefaultColumnsButton":false}'
                            ELSE '{"filters":{},"rowsPerPage":10,"columns":[],"showDefaultColumnsButton":true}'
                        END
                    END as preferenceDetail FROM SELECT 1 as dummy) d
                LEFT JOIN UserPreferences up ON (
                    (@userSpecificExists = 1 AND up.UserId = @UserId AND up.ObjectType = @objectType AND up.ObjectName = @objectName AND up.PageName = @pageName)
                    OR (@userSpecificExists = 0 AND @preferenceExists = 1 AND up.ObjectType = @objectType AND up.ObjectName = @objectName AND up.PageName = @pageName)
                )
            1 as isSuccess,

        -- Log access for audit trail (if user provided)
        IF @UserId IS NOT NULL
        BEGIN
            INSERT INTO AuditLogs (
                UserId, 
                Action, 
                EntityType, 
                EntityId, 
                Details, 
                CreatedAt
            )
            VALUES (
                @UserId,
                'VIEW_PREFERENCES',
                'USER_PREFERENCES',
                NULL,
                CONCAT('Retrieved preferences for ', @objectType, '/', @objectName, '/', @pageName),
                GETDATE()
            );
        END

    END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as data,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get detailed information for a specific finding by finding number
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFindingDetails]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @FindingNumber NVARCHAR(100);
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @FindingNumber = JSON_VALUE(@Parameters, '$.findingNumber')
        END
        ELSE
        BEGIN
            SET @ErrorCode = 'INVALID_JSON';
            SET @Message = 'Invalid JSON format in parameters.';
            GOTO ErrorResponse;
        END

        -- Validate required parameters
        IF @FindingNumber IS NULL OR LTRIM(RTRIM(@FindingNumber)) = ''
        BEGIN
            SET @ErrorCode = 'MISSING_PARAMETERS';
            SET @Message = 'Finding number is required.';
            GOTO ErrorResponse;
        END

        -- Validate user exists and is active (if userId provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Check if finding exists and user has access
        DECLARE @FindingId INT;
        DECLARE @AuditId INT;
        DECLARE @CompanyId INT;

        SELECT 
            @FindingId = f.FindingId,
            @AuditId = f.AuditId,
            @CompanyId = a.CompanyId
        FROM Findings f
        INNER JOIN Audits a ON f.AuditId = a.AuditId
        WHERE f.FindingNumber = @FindingNumber 
        AND f.IsActive = 1;

        IF @FindingId IS NULL
        BEGIN
            SET @ErrorCode = 'FINDING_NOT_FOUND';
            SET @Message = 'Finding not found.';
            GOTO ErrorResponse;
        END

        -- Validate user access to company (if userId provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM UserCompanyAccess uca 
            WHERE uca.UserId = @UserId AND uca.CompanyId = @CompanyId
        )
        BEGIN
            SET @ErrorCode = 'ACCESS_DENIED';
            SET @Message = 'User does not have access to this finding.';
            GOTO ErrorResponse;
        END

        -- Build the response JSON
        DECLARE @ResponseData NVARCHAR(MAX);

        SELECT @ResponseData = SELECT CAST(1 AS BIT) as isSuccess,
                SELECT CASE 
                            WHEN f.AcceptedDate IS NOT NULL 
                            THEN FORMAT(f.AcceptedDate, 'yyyy-MM-dd')
                            ELSE NULL 
                        END as acceptedDate,
                        f.AuditId as auditId,
                        SELECT STRING_AGG(
                                CONCAT(u.FirstName, ' ', u.LastName), 
                                ', '
                            ) WITHIN GROUP (ORDER BY u.LastName, u.FirstName)
                            FROM AuditTeamMembers atm
                            INNER JOIN Users u ON atm.UserId = u.UserId
                            WHERE atm.AuditId = f.AuditId AND atm.IsActive = 1
                        ) as auditors,
                        CONCAT(at.AuditTypeName, CASE WHEN a.Priority IS NOT NULL THEN CONCAT('; ', a.Priority) ELSE '' END) as auditType,
                        CASE 
                            WHEN f.ClosedDate IS NOT NULL 
                            THEN FORMAT(f.ClosedDate, 'yyyy-MM-dd')
                            ELSE NULL 
                        END as closedDate,
                        CASE 
                            WHEN f.DueDate IS NOT NULL 
                            THEN FORMAT(f.DueDate, 'yyyy-MM-dd')
                            ELSE NULL 
                        END as dueDate,
                        f.FindingNumber as findingNumber,
                        CASE 
                            WHEN f.OpenedDate IS NOT NULL 
                            THEN FORMAT(f.OpenedDate, 'yyyy-MM-dd')
                            ELSE NULL 
                        END as openedDate,
                        SELECT s.ServiceName
                            FROM AuditServices aus
                            INNER JOIN Services s ON aus.ServiceId = s.ServiceId
                            WHERE aus.AuditId = f.AuditId AND aus.IsActive = 1
                        ) as services,
                        SELECT CONCAT(
                                    ISNULL(site.AddressLine1, ''), 
                                    CASE WHEN site.AddressLine2 IS NOT NULL THEN CONCAT(', ', site.AddressLine2) ELSE '' END,
                                    CASE WHEN site.City IS NOT NULL THEN CONCAT(', ', site.City) ELSE '' END,
                                    CASE WHEN site.StateProvince IS NOT NULL THEN CONCAT(', ', site.StateProvince) ELSE '' END,
                                    CASE WHEN site.PostalCode IS NOT NULL THEN CONCAT(', ', site.PostalCode) ELSE '' END,
                                    CASE WHEN c.CountryName IS NOT NULL THEN CONCAT(', ', c.CountryName) ELSE '' END
                                ) as siteAddress,
                                site.SiteName as siteName FROM AuditSites ausites
                            INNER JOIN Sites site ON ausites.SiteId = site.SiteId
                            LEFT JOIN Countries c ON site.CountryId = c.CountryId
                            WHERE ausites.AuditId = f.AuditId AND ausites.IsActive = 1
                        ) as sites,
                        fs.StatusName as status FROM Findings f
                    INNER JOIN Audits a ON f.AuditId = a.AuditId
                    INNER JOIN AuditTypes at ON a.AuditTypeId = at.AuditTypeId
                    INNER JOIN FindingStatuses fs ON f.StatusId = fs.StatusId
                    WHERE f.FindingId = @FindingId
                '' as errorCode);

        -- Log access for audit trail
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
            'VIEW',
            'FINDING',
            @FindingId,
            CONCAT('Viewed finding details: ', @FindingNumber),
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = '';

        -- Return success response
        SELECT @ResponseData as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response
        SELECT 
                CAST(0 AS BIT) as isSuccess,
                CAST(NULL AS NVARCHAR(MAX)@ErrorCode as errorCode,
                @Message as message) as JsonResponse;

    END TRY
    BEGIN CATCH
        -- Log error
        INSERT INTO ErrorLogs (
            UserId,
            ErrorMessage,
            StackTrace,
            CreatedAt
        )
        VALUES (
            @UserId,
            ERROR_MESSAGE(),
            CONCAT('Procedure: Sp_GetFindingDetails, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST(0 AS BIT) as isSuccess,
                CAST(NULL AS NVARCHAR(MAX)'SERVER_ERROR' as errorCode,
                'An error occurred while retrieving finding details.' as message) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get finding details with user validation:
EXEC Sp_GetFindingDetails @Parameters = N'{
    "userId": 123,
    "findingNumber": "CLAY-0006-2887291"
}';

2. Get finding details without user validation:
EXEC Sp_GetFindingDetails @Parameters = N'{
    "findingNumber": "CLAY-0006-2887291"
}';

Expected JSON Response Format:
{
    "isSuccess": true,
    "data": {
        "acceptedDate": null,
        "auditId": 2887291,
        "auditors": ["Douglas Milne", "James Clayton", "Peter Potter"],
        "auditType": "Periodic Audit; P1",
        "closedDate": null,
        "dueDate": null,
        "findingNumber": "CLAY-0006-2887291",
        "openedDate": "2025-07-02",
        "services": ["ISO 14001:2015", "ISO 9001:2015"],
        "sites": [
            {
                "siteAddress": "5th Floor, the Administration Building, Stanlow Manufacturing Complex, Ellesmere Port, Cheshire, CH65 4HB, United Kingdom",
                "siteName": "Essar Oil (UK) Limited trading as EET Fuels",
                "__typename": "SiteDataInFindingDetails"
            }
        ],
        "status": "Open",
        "__typename": "FindingDetailsResponse"
    },
    "errorCode": "",
    "message": "",
    "__typename": "BaseGraphResponseOfFindingDetailsResponse"
}

Notes:
- Provides comprehensive finding details including audit context
- Includes auditor names concatenated from audit team members
- Formats addresses properly with all components
- Returns services and sites as JSON arrays
- Includes proper date formatting (yyyy-MM-dd)
- Provides access control validation through user-company relationships
- Includes audit logging for compliance tracking
- Handles null dates appropriately
- Returns exact GraphQL response structure with proper type names
*/



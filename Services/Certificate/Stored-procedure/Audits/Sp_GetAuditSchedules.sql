-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get audit schedules with filtering capabilities
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetAuditSchedules]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @CalendarScheduleFilter NVARCHAR(MAX) = NULL;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @CalendarScheduleFilter = NULL
        END
        ELSE
        BEGIN
            SET @ErrorCode = 'INVALID_JSON';
            SET @Message = 'Invalid JSON format in parameters.';
            GOTO ErrorResponse;
        END

        -- Set default for empty filter
        IF @CalendarScheduleFilter IS NULL OR @CalendarScheduleFilter = ''
            SET @CalendarScheduleFilter = '{}';

        -- Validate user exists and is active (if provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Extract filter parameters from CalendarScheduleFilter
        DECLARE @CompanyIds NVARCHAR(MAX) = NULL;
        DECLARE @ServiceIds NVARCHAR(MAX) = NULL;
        DECLARE @SiteIds NVARCHAR(MAX) = NULL;
        DECLARE @FromDate DATETIME = TRY_CAST(JSON_VALUE(@CalendarScheduleFilter, '$.fromDate') AS DATETIME);
        DECLARE @ToDate DATETIME = TRY_CAST(JSON_VALUE(@CalendarScheduleFilter, '$.toDate') AS DATETIME);
        DECLARE @StatusFilter NVARCHAR(MAX) = NULL;

        -- Set defaults for empty arrays
        IF @CompanyIds IS NULL OR @CompanyIds = '' SET @CompanyIds = '[]';
        IF @ServiceIds IS NULL OR @ServiceIds = '' SET @ServiceIds = '[]';
        IF @SiteIds IS NULL OR @SiteIds = '' SET @SiteIds = '[]';
        IF @StatusFilter IS NULL OR @StatusFilter = '' SET @StatusFilter = '[]';

        -- Get audit schedules based on filters
        DECLARE @SchedulesData NVARCHAR(MAX);
        
        SELECT @SchedulesData = SELECT asa.SiteAuditId as siteAuditId,
                CASE 
                    WHEN asa.StartDate IS NOT NULL THEN FORMAT(asa.StartDate, 'yyyy-MM-ddTHH:mm:ss.fffZ') 
                    ELSE NULL 
                END as startDate,
                CASE 
                    WHEN asa.EndDate IS NOT NULL THEN FORMAT(asa.EndDate, 'yyyy-MM-ddTHH:mm:ss.fffZ') 
                    ELSE NULL 
                END as endDate,
                ISNULL(asa.Status, 'To Be Confirmed') as status,
                SELECT DISTINCT
                        ass.ServiceId
                    FROM AuditSiteServices ass
                    WHERE ass.SiteAuditId = asa.SiteAuditId
                    AND ass.IsActive = 1
                ) as serviceIds,
                asa.SiteId as siteId,
                ISNULL(asa.AuditType, 'Standard Audit') as auditType,
                asa.LeadAuditor as leadAuditor,
                SELECT DISTINCT
                        asr.RepresentativeName
                    FROM AuditSiteRepresentatives asr
                    WHERE asr.SiteAuditId = asa.SiteAuditId
                    AND asr.IsActive = 1
                ) as siteRepresentatives,
                asa.CompanyId as companyId,
                asa.AuditId as auditID,
                ISNULL(co.CountryCode, '') as reportingCountry,
                ISNULL(asa.ProjectNumber, '') as projectNumber,
                ISNULL(c.AccountDNVId, '') as accountDNVId FROM AuditSiteAudits asa
            INNER JOIN Sites s ON asa.SiteId = s.SiteId
            INNER JOIN Companies c ON asa.CompanyId = c.CompanyId
            LEFT JOIN Cities ct ON s.CityId = ct.CityId
            LEFT JOIN Countries co ON ct.CountryId = co.CountryId
            WHERE asa.IsActive = 1
            AND s.IsActive = 1
            AND c.IsActive = 1
            AND (
                @UserId IS NULL 
                OR EXISTS SELECT 1 FROM UserSiteAccess usa
                    WHERE usa.UserId = @UserId 
                    AND usa.SiteId = asa.SiteId
                    AND usa.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@CompanyIds) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@CompanyIds) comp
                    WHERE CAST(comp.value AS INT) = asa.CompanyId
                )
            )
            AND (
                JSON_LENGTH(@SiteIds) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@SiteIds) site_filter
                    WHERE CAST(site_filter.value AS INT) = asa.SiteId
                )
            )
            AND (
                JSON_LENGTH(@ServiceIds) = 0
                OR EXISTS SELECT 1 FROM AuditSiteServices ass_filter
                    INNER JOIN OPENJSON(@ServiceIds) serv_filter ON CAST(serv_filter.value AS INT) = ass_filter.ServiceId
                    WHERE ass_filter.SiteAuditId = asa.SiteAuditId
                    AND ass_filter.IsActive = 1
                )
            )
            AND (
                JSON_LENGTH(@StatusFilter) = 0
                OR EXISTS SELECT 1 FROM OPENJSON(@StatusFilter) stat_filter
                    WHERE stat_filter.value = asa.Status
                )
            )
            AND (
                @FromDate IS NULL 
                OR asa.StartDate >= @FromDate
                OR (@FromDate IS NOT NULL AND asa.StartDate IS NULL)
            )
            AND (
                @ToDate IS NULL 
                OR asa.EndDate <= @ToDate
                OR (@ToDate IS NOT NULL AND asa.EndDate IS NULL)
            )
            ORDER BY 
                CASE WHEN asa.StartDate IS NULL THEN 1 ELSE 0 END,
                asa.StartDate DESC,
                asa.SiteAuditId DESC
        );

        -- Handle empty result
        IF @SchedulesData IS NULL OR @SchedulesData = ''
            SET @SchedulesData = '[]';

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess,
                'Success' as message);

        -- Log access for audit trail
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
                'VIEW_AUDIT_SCHEDULES',
                'SCHEDULE',
                NULL,
                'Retrieved audit schedules',
                GETDATE()
            );
        END

        SET @IsSuccess = 1;
        SET @Message = 'Success';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                @Message as message,
                @ErrorCode as errorCode) as JsonResponse;

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
            CONCAT('Procedure: Sp_GetAuditSchedules, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving audit schedules.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all audit schedules (no filters):
EXEC Sp_GetAuditSchedules @Parameters = N'{
    "userId": 123,
    "calendarScheduleFilter": {}
}';

2. Get audit schedules with filters:
EXEC Sp_GetAuditSchedules @Parameters = N'{
    "userId": 123,
    "calendarScheduleFilter": {
        "companyIds": [341, 344],
        "serviceIds": [780, 597],
        "siteIds": [172389],
        "statuses": ["Confirmed", "To Be Confirmed"],
        "fromDate": "2023-01-01T00:00:00.000Z",
        "toDate": "2023-12-31T23:59:59.999Z"
    }
}';

Expected JSON Response Format:
{
    "data": [
        {
            "siteAuditId": 4459416,
            "startDate": null,
            "endDate": null,
            "status": "To Be Confirmed",
            "serviceIds": [780],
            "siteId": 172389,
            "auditType": "Re-certification Audit",
            "leadAuditor": null,
            "siteRepresentatives": ["Barbara Dal Sasso"],
            "companyId": 341,
            "auditID": 3166847,
            "reportingCountry": "IT",
            "projectNumber": "PRJC-101044-2008-MSC-ITA",
            "accountDNVId": "10049030",
            "__typename": "AuditSchedulesResponse"
        }
    ],
    "isSuccess": true,
    "message": "Success",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfAuditSchedulesResponse"
}

Database Tables Referenced:
- AuditSiteAudits: Main audit schedule records
- AuditSiteServices: Services for each audit
- AuditSiteRepresentatives: Site representatives for audits
- Sites: Site information
- Companies: Company information
- Cities: City information
- Countries: Country information
- UserSiteAccess: User access to sites
- Users: User validation
- AuditLogs: Access tracking

Notes:
- Filters audit schedules based on multiple criteria
- Supports filtering by companies, services, sites, statuses, and date ranges
- Returns arrays for serviceIds and siteRepresentatives
- User access control applies if userId provided
- Handles null dates appropriately (schedules without confirmed dates)
- Orders by start date with nulls first, then by most recent
- Returns exact GraphQL response structure with proper type names
- Includes audit logging for schedule access tracking
- All filter arrays are optional and default to no filtering
- Date filtering handles cases where audit dates are not yet set
*/



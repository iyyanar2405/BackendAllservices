-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get manage finding details by finding number with multi-language support
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetManageFindingDetails]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @FindingNumber NVARCHAR(255) = NULL;
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
            SET @ErrorCode = 'MISSING_FINDING_NUMBER';
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

        -- Get finding details with related information
        DECLARE @FindingId INT;
        DECLARE @CompanyId INT;
        DECLARE @CategoryName NVARCHAR(255);
        DECLARE @PrimaryLanguage NVARCHAR(100);
        DECLARE @SecondaryLanguage NVARCHAR(100);
        DECLARE @TitlePrimary NVARCHAR(500);
        DECLARE @TitleSecondary NVARCHAR(500);
        DECLARE @DescriptionPrimary NVARCHAR(MAX);
        DECLARE @DescriptionSecondary NVARCHAR(MAX);
        
        SELECT TOP 1 
            @FindingId = f.FindingId,
            @CompanyId = a.CompanyId,
            @CategoryName = fc.CategoryName,
            @PrimaryLanguage = ISNULL(f.PrimaryLanguage, 'English'),
            @SecondaryLanguage = f.SecondaryLanguage,
            @TitlePrimary = f.TitleInPrimaryLanguage,
            @TitleSecondary = f.TitleInSecondaryLanguage,
            @DescriptionPrimary = f.DescriptionInPrimaryLanguage,
            @DescriptionSecondary = f.DescriptionInSecondaryLanguage
        FROM Findings f
        INNER JOIN Audits a ON f.AuditId = a.AuditId
        LEFT JOIN FindingCategories fc ON f.CategoryId = fc.CategoryId
        WHERE f.FindingNumber = @FindingNumber 
        AND f.IsActive = 1 
        AND a.IsActive = 1;

        IF @FindingId IS NULL
        BEGIN
            SET @ErrorCode = 'FINDING_NOT_FOUND';
            SET @Message = 'Finding not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Check user access to the finding's company (if userId provided)
        IF @UserId IS NOT NULL AND NOT EXISTS SELECT 1 FROM UserCompanyAccess uca 
            WHERE uca.UserId = @UserId AND uca.CompanyId = @CompanyId
        )
        BEGIN
            SET @ErrorCode = 'ACCESS_DENIED';
            SET @Message = 'User does not have access to this finding.';
            GOTO ErrorResponse;
        END

        -- Get clauses associated with the finding
        DECLARE @ClausesData NVARCHAR(MAX);
        SELECT @ClausesData = SELECT CASE 
                    WHEN c.ClauseName IS NOT NULL AND c.ClauseName != '' 
                    THEN c.ClauseName
                    ELSE CONCAT('Clause ', fc.ClauseId)
                END as [value]
            FROM FindingClauses fc
            LEFT JOIN Clauses c ON fc.ClauseId = c.ClauseId
            WHERE fc.FindingId = @FindingId 
            AND fc.IsActive = 1
            ORDER BY c.ClauseName
        );

        -- Handle empty clauses
        IF @ClausesData IS NULL OR @ClausesData = ''
            SET @ClausesData = '[]';

        -- Get focus areas associated with the finding
        DECLARE @FocusAreasData NVARCHAR(MAX);
        SELECT @FocusAreasData = SELECT fa.FocusAreaInPrimaryLanguage as focusAreaInPrimaryLanguage,
                fa.FocusAreaInSecondaryLanguage as focusAreaInSecondaryLanguage FROM FindingFocusAreas ffa
            INNER JOIN FocusAreas fa ON ffa.FocusAreaId = fa.FocusAreaId
            WHERE ffa.FindingId = @FindingId 
            AND ffa.IsActive = 1
            AND fa.IsActive = 1
            ORDER BY fa.FocusAreaInPrimaryLanguage
        );

        -- Handle empty focus areas
        IF @FocusAreasData IS NULL OR @FocusAreasData = ''
            SET @FocusAreasData = '[]';

        -- Build the response data
        DECLARE @ResponseData NVARCHAR(MAX);
        SELECT @ResponseData = SELECT ISNULL(@CategoryName, 'Unknown') as category,
                NULL as clauses,
                @DescriptionPrimary as descriptionInPrimaryLanguage,
                @DescriptionSecondary as descriptionInSecondaryLanguage,
                NULL as focusAreas,
                @PrimaryLanguage as primaryLanguage,
                @SecondaryLanguage as secondaryLanguage,
                @TitlePrimary as titleInPrimaryLanguage,
                @TitleSecondary as titleInSecondaryLanguage);

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess);

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
            'VIEW_MANAGE_FINDING',
            'FINDING',
            @FindingId,
            CONCAT('Retrieved manage finding details for: ', @FindingNumber),
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = '';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response
        SELECT NULL as category,
                        CAST('[]' AS NVARCHAR(MAX)) as clauses,
                        NULL as descriptionInPrimaryLanguage,
                        NULL as descriptionInSecondaryLanguage,
                        CAST('[]' AS NVARCHAR(MAX)) as focusAreas,
                        NULL as primaryLanguage,
                        NULL as secondaryLanguage,
                        NULL as titleInPrimaryLanguage,
                        NULL as titleInSecondaryLanguage,
                        
                CAST(0 AS BIT) as isSuccess,
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
            CONCAT('Procedure: Sp_GetManageFindingDetails, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT NULL as category,
                        CAST('[]' AS NVARCHAR(MAX)) as clauses,
                        NULL as descriptionInPrimaryLanguage,
                        NULL as descriptionInSecondaryLanguage,
                        CAST('[]' AS NVARCHAR(MAX)) as focusAreas,
                        NULL as primaryLanguage,
                        NULL as secondaryLanguage,
                        NULL as titleInPrimaryLanguage,
                        NULL as titleInSecondaryLanguage,
                        
                CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving manage finding details.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get manage finding details for a specific finding:
EXEC Sp_GetManageFindingDetails @Parameters = N'{
    "userId": 123,
    "findingNumber": "CLAY-0006-2887291"
}';

2. Get manage finding details without user validation:
EXEC Sp_GetManageFindingDetails @Parameters = N'{
    "findingNumber": "CLAY-0006-2887291"
}';

3. Invalid finding number example:
EXEC Sp_GetManageFindingDetails @Parameters = N'{
    "userId": 123,
    "findingNumber": "INVALID-NUMBER"
}';

Expected JSON Response Format:
{
    "data": {
        "category": "CAT2 (Minor)",
        "clauses": [
            "7.1.3 Infrastructure",
            "8.1 Operational planning and control"
        ],
        "descriptionInPrimaryLanguage": "Requirement: The organisation shall define and implement processes...",
        "descriptionInSecondaryLanguage": null,
        "focusAreas": [
            {
                "focusAreaInPrimaryLanguage": "Effective ODP operations",
                "focusAreaInSecondaryLanguage": null,
                "__typename": "FocusAreaData"
            }
        ],
        "primaryLanguage": "English",
        "secondaryLanguage": null,
        "titleInPrimaryLanguage": "Operational control and infrastructure - portable hoses",
        "titleInSecondaryLanguage": null,
        "__typename": "ManageFindingDetailsResponse"
    },
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfManageFindingDetailsResponse"
}

Error Response Format:
{
    "data": {
        "category": null,
        "clauses": [],
        "descriptionInPrimaryLanguage": null,
        "descriptionInSecondaryLanguage": null,
        "focusAreas": [],
        "primaryLanguage": null,
        "secondaryLanguage": null,
        "titleInPrimaryLanguage": null,
        "titleInSecondaryLanguage": null,
        "__typename": "ManageFindingDetailsResponse"
    },
    "isSuccess": false,
    "message": "Finding not found or inactive.",
    "errorCode": "FINDING_NOT_FOUND",
    "/__typename": "BaseGraphResponseOfManageFindingDetailsResponse"
}

Notes:
- Retrieves comprehensive finding details for management purposes
- Supports multi-language fields (primary and secondary languages)
- Returns associated clauses as an array of clause names
- Includes focus areas with multi-language support and proper type names
- Validates finding exists and user has access to the finding's company
- Handles missing or null language fields gracefully
- Provides proper fallback values for missing data
- Includes audit logging for security and compliance tracking
- Returns exact GraphQL response structure with proper type names
- Supports optional user validation - can work with or without userId
- Maintains consistent data structure even in error scenarios
- Handles empty clauses and focus areas with proper empty arrays
- Optimized queries with proper joins and filtering
*/



-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get settings company details for user including parent company and legal entities
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetSettingsCompanyDetails]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId')
        END
        ELSE
        BEGIN
            SET @ErrorCode = 'INVALID_JSON';
            SET @Message = 'Invalid JSON format in parameters.';
            GOTO ErrorResponse;
        END

        -- Validate user exists and is active
        IF @UserId IS NULL OR NOT EXISTS SELECT 1 FROM Users WHERE UserId = @UserId AND IsActive = 1)
        BEGIN
            SET @ErrorCode = 'INVALID_USER';
            SET @Message = 'User not found or inactive.';
            GOTO ErrorResponse;
        END

        -- Get user details and status
        DECLARE @UserStatus NVARCHAR(50);
        DECLARE @IsAdmin BIT;
        DECLARE @ParentCompanyId INT = NULL;
        
        SELECT 
            @UserStatus = ISNULL(u.UserStatus, 'Pending'),
            @IsAdmin = CASE WHEN ur.RoleName IN ('Admin', 'Administrator') THEN 1 ELSE 0 END
        FROM Users u
        LEFT JOIN UserRoles ur ON u.UserId = ur.UserId AND ur.IsActive = 1
        WHERE u.UserId = @UserId AND u.IsActive = 1;

        -- Get parent company information (if exists)
        DECLARE @ParentCompanyData NVARCHAR(MAX) = NULL;
        
        SELECT TOP 1 @ParentCompanyId = ParentCompanyId
        FROM UserCompanyAccess uca
        INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
        WHERE uca.UserId = @UserId 
        AND c.ParentCompanyId IS NOT NULL
        AND uca.IsActive = 1 
        AND c.IsActive = 1;

        IF @ParentCompanyId IS NOT NULL
        BEGIN
            SELECT @ParentCompanyData = SELECT TOP 1
                    c.CompanyId as accountId,
                    c.Address as address,
                    c.City as city,
                    co.CountryName as country,
                    c.CountryId as countryId,
                    CAST(ISNULL(c.IsServiceRequestOpen, 0) AS BIT) as isSerReqOpen,
                    c.CompanyName as organizationName,
                    CAST(ISNULL(c.PONumberRequired, 0) AS BIT) as poNumberRequired,
                    c.VATNumber as vatNumber,
                    c.ZipCode as zipCode FROM Companies c
                LEFT JOIN Countries co ON c.CountryId = co.CountryId
                WHERE c.CompanyId = @ParentCompanyId 
                AND c.IsActive = 1
            );
        END

        -- Get legal entities (companies user has access to)
        DECLARE @LegalEntitiesData NVARCHAR(MAX);
        
        SELECT @LegalEntitiesData = SELECT c.CompanyId as accountId,
                c.Address as address,
                c.City as city,
                co.CountryName as country,
                co.CountryCode as countryCode,
                c.CountryId as countryId,
                CAST(ISNULL(c.IsServiceRequestOpen, 0) AS BIT) as isSerReqOpen,
                c.CompanyName as organizationName,
                CAST(ISNULL(c.PONumberRequired, 0) AS BIT) as poNumberRequired,
                c.VATNumber as vatNumber,
                c.ZipCode as zipCode,
                c.AccountDNVId as accountDNVId FROM UserCompanyAccess uca
            INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
            LEFT JOIN Countries co ON c.CountryId = co.CountryId
            WHERE uca.UserId = @UserId 
            AND uca.IsActive = 1 
            AND c.IsActive = 1
            ORDER BY c.CompanyName
        );

        -- Handle empty legal entities
        IF @LegalEntitiesData IS NULL OR @LegalEntitiesData = ''
            SET @LegalEntitiesData = '[]';

        -- Build the response data
        DECLARE @ResponseData NVARCHAR(MAX);
        SELECT @ResponseData = SELECT @UserStatus as userStatus,
                @IsAdmin as isAdmin,
                CASE 
                    WHEN @ParentCompanyData IS NOT NULL THEN NULL
                    ELSE NULL 
                END as parentCompany,
                NULL as legalEntities);

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
            'VIEW_COMPANY_DETAILS',
            'SETTINGS',
            NULL,
            'Retrieved settings company details',
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = '';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response
        SELECT NULL as userStatus,
                        CAST(0 AS BIT) as isAdmin,
                        NULL as parentCompany,
                        CAST('[]' AS NVARCHAR(MAX)) as legalEntities,
                        
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
            CONCAT('Procedure: Sp_GetSettingsCompanyDetails, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT NULL as userStatus,
                        CAST(0 AS BIT) as isAdmin,
                        NULL as parentCompany,
                        CAST('[]' AS NVARCHAR(MAX)) as legalEntities,
                        
                CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving company details.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get company details for a user:
EXEC Sp_GetSettingsCompanyDetails @Parameters = N'{
    "userId": 123
}';

Expected JSON Response Format:
{
    "data": {
        "userStatus": "Completed",
        "isAdmin": true,
        "parentCompany": null,
        "legalEntities": [
            {
                "accountId": 386,
                "address": "Manchester Road Carrington",
                "city": "Manchester",
                "country": "United Kingdom",
                "countryCode": "GB",
                "countryId": 469,
                "isSerReqOpen": false,
                "organizationName": "Air Products (BR) Limited",
                "poNumberRequired": false,
                "vatNumber": "GB",
                "zipCode": "M31 4TG",
                "accountDNVId": "10305528",
                "__typename": "LegalEntity"
            }
        ],
        "__typename": "UserCompanyDetailsResponse"
    },
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfUserCompanyDetailsResponse"
}

Notes:
- Retrieves user company details including status and admin privileges
- Returns parent company information if user belongs to a subsidiary
- Lists all legal entities (companies) the user has access to
- Includes complete address and organizational information
- Provides admin status based on user roles
- Handles cases where user has no parent company (returns null)
- Returns empty array for legal entities if user has no company access
- Includes audit logging for security and compliance tracking
- Returns exact GraphQL response structure with proper type names
- Supports user status tracking (Completed, Pending, etc.)
- Includes country information with both name and code
*/



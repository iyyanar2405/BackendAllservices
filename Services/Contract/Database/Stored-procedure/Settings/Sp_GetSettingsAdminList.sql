-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get settings admin list with company access and permissions
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetSettingsAdminList]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @AccountDNVId NVARCHAR(50) = NULL;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @AccountDNVId = JSON_VALUE(@Parameters, '$.accountDNVId')
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

        -- Check if requesting user has admin permissions
        DECLARE @IsRequestingUserAdmin BIT = 0;
        IF EXISTS SELECT 1 FROM UserRoles ur 
            WHERE ur.UserId = @UserId 
            AND ur.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
            AND ur.IsActive = 1
        )
        BEGIN
            SET @IsRequestingUserAdmin = 1;
        END

        IF @IsRequestingUserAdmin = 0
        BEGIN
            SET @ErrorCode = 'INSUFFICIENT_PERMISSIONS';
            SET @Message = 'User does not have admin permissions.';
            GOTO ErrorResponse;
        END

        -- Get admin users list
        DECLARE @AdminListData NVARCHAR(MAX);
        
        SELECT @AdminListData = SELECT CONCAT(u.FirstName, ' ', u.LastName) as name,
                u.Email as email,
                ISNULL(u.UserStatus, 'Pending') as userStatus,
                CASE WHEN u.UserId = @UserId THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END as isCurrentUser,
                CASE 
                    WHEN u.UserId = @UserId THEN CAST(0 AS BIT) -- Current user cannot delete themselves
                    WHEN EXISTS SELECT 1 FROM UserRoles ur2 
                        WHERE ur2.UserId = u.UserId 
                        AND ur2.RoleName = 'SuperAdmin' 
                        AND ur2.IsActive = 1
                    ) THEN CAST(0 AS BIT) -- SuperAdmin cannot be deleted
                    ELSE CAST(1 AS BIT)
                END as canDelete,
                CASE 
                    WHEN u.UserId = @UserId THEN CAST(1 AS BIT) -- Current user can manage their own permissions
                    WHEN EXISTS SELECT 1 FROM UserRoles ur3 
                        WHERE ur3.UserId = @UserId 
                        AND ur3.RoleName = 'SuperAdmin' 
                        AND ur3.IsActive = 1
                    ) THEN CAST(1 AS BIT) -- SuperAdmin can manage all permissions
                    ELSE CAST(0 AS BIT)
                END as canManagePermissions,
                SELECT c.CompanyId as companyId,
                        c.CompanyName as companyName FROM UserCompanyAccess uca
                    INNER JOIN Companies c ON uca.CompanyId = c.CompanyId
                    WHERE uca.UserId = u.UserId 
                    AND uca.IsActive = 1 
                    AND c.IsActive = 1
                    AND (@AccountDNVId IS NULL OR c.AccountDNVId = @AccountDNVId)
                    ORDER BY c.CompanyName
                ) as companies FROM Users u
            INNER JOIN UserRoles ur ON u.UserId = ur.UserId
            WHERE ur.RoleName IN ('Admin', 'Administrator', 'SuperAdmin')
            AND ur.IsActive = 1
            AND u.IsActive = 1
            AND (
                @AccountDNVId IS NULL 
                OR EXISTS SELECT 1 FROM UserCompanyAccess uca2
                    INNER JOIN Companies c2 ON uca2.CompanyId = c2.CompanyId
                    WHERE uca2.UserId = u.UserId 
                    AND c2.AccountDNVId = @AccountDNVId
                    AND uca2.IsActive = 1 
                    AND c2.IsActive = 1
                )
            )
            ORDER BY u.FirstName, u.LastName
        );

        -- Handle empty result
        IF @AdminListData IS NULL OR @AdminListData = ''
            SET @AdminListData = '[]';

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
            'VIEW_ADMIN_LIST',
            'SETTINGS',
            NULL,
            CONCAT('Retrieved admin list', CASE WHEN @AccountDNVId IS NOT NULL THEN ' for account ' + @AccountDNVId ELSE '' END),
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = '';

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
            CONCAT('Procedure: Sp_GetSettingsAdminList, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving admin list.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all admin users:
EXEC Sp_GetSettingsAdminList @Parameters = N'{
    "userId": 123,
    "accountDNVId": null
}';

2. Get admin users for specific account:
EXEC Sp_GetSettingsAdminList @Parameters = N'{
    "userId": 123,
    "accountDNVId": "10305528"
}';

Expected JSON Response Format:
{
    "data": [
        {
            "name": "John Smith",
            "email": "john.smith@company.com",
            "userStatus": "Completed",
            "isCurrentUser": false,
            "canDelete": true,
            "canManagePermissions": false,
            "companies": [
                {
                    "companyId": 386,
                    "companyName": "Air Products (BR) Limited",
                    "__typename": "Company"
                }
            ],
            "__typename": "AdminUser"
        }
    ],
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfAdminUser"
}

Notes:
- Requires admin permissions to access
- Lists all users with Admin, Administrator, or SuperAdmin roles
- Shows company access for each admin user
- Includes permission flags for delete and manage operations
- Filters by accountDNVId if provided
- Current user is marked as isCurrentUser = true
- SuperAdmin users cannot be deleted
- Current user cannot delete themselves
- Only SuperAdmin can manage all permissions
- Includes audit logging for admin list access
- Returns exact GraphQL response structure with proper type names
- Returns empty array if no admin users found
*/



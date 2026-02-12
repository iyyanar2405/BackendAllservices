-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get latest finding response details by finding number
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetLatestFindingResponse]
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

        -- Check if finding exists
        DECLARE @FindingId INT;
        DECLARE @CompanyId INT;
        
        SELECT TOP 1 
            @FindingId = f.FindingId,
            @CompanyId = a.CompanyId
        FROM Findings f
        INNER JOIN Audits a ON f.AuditId = a.AuditId
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

        -- Get the latest finding response
        DECLARE @ResponseData NVARCHAR(MAX);
        
        SELECT @ResponseData = SELECT TOP 1
                fr.RootCause as rootCause,
                fr.CorrectiveAction as correctiveAction,
                fr.Correction as correction,
                CAST(ISNULL(fr.IsSubmitToDnv, 0) AS BIT) as isSubmitToDnv,
                CASE 
                    WHEN fr.UpdatedDate IS NOT NULL THEN FORMAT(fr.UpdatedDate, 'yyyy-MM-ddTHH:mm:ss.fffZ')
                    ELSE NULL 
                END as updatedOn,
                CAST(ISNULL(fr.IsDraft, 0) AS BIT) as isDraft,
                fr.ResponseId as respondId FROM FindingResponses fr
            WHERE fr.FindingId = @FindingId 
            AND fr.IsActive = 1
            ORDER BY fr.CreatedDate DESC, fr.ResponseId DESC
        );

        -- Handle case where no response found
        IF @ResponseData IS NULL
        BEGIN
            SELECT @ResponseData = SELECT NULL as rootCause,
                    NULL as correctiveAction,
                    NULL as correction,
                    CAST(0 AS BIT) as isSubmitToDnv,
                    NULL as updatedOn,
                    CAST(0 AS BIT) as isDraft,
                    NULL as respondId);
        END

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess,
                'Latest finding response retrieved successfully.' as message);

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
            'VIEW_LATEST_RESPONSE',
            'FINDING',
            @FindingId,
            CONCAT('Retrieved latest response for finding: ', @FindingNumber),
            GETDATE()
        );

        SET @IsSuccess = 1;
        SET @Message = 'Latest finding response retrieved successfully.';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        -- Return error response with empty data structure
        DECLARE @EmptyData NVARCHAR(MAX);
        SELECT @EmptyData = SELECT NULL as rootCause,
                NULL as correctiveAction,
                NULL as correction,
                CAST(0 AS BIT) as isSubmitToDnv,
                NULL as updatedOn,
                CAST(0 AS BIT) as isDraft,
                NULL as respondId);

        SELECT 
                NULL as isSuccess,
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
            CONCAT('Procedure: Sp_GetLatestFindingResponse, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response with empty data structure
        DECLARE @EmptyErrorData NVARCHAR(MAX);
        SELECT @EmptyErrorData = SELECT NULL as rootCause,
                NULL as correctiveAction,
                NULL as correction,
                CAST(0 AS BIT) as isSubmitToDnv,
                NULL as updatedOn,
                CAST(0 AS BIT) as isDraft,
                NULL as respondId);

        SELECT 
                NULL as isSuccess,
                'An error occurred while retrieving the latest finding response.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get latest response for a specific finding:
EXEC Sp_GetLatestFindingResponse @Parameters = N'{
    "userId": 123,
    "findingNumber": "CLAY-0006-2887291"
}';

2. Get latest response without user validation:
EXEC Sp_GetLatestFindingResponse @Parameters = N'{
    "findingNumber": "CLAY-0006-2887291"
}';

3. Invalid finding number example:
EXEC Sp_GetLatestFindingResponse @Parameters = N'{
    "userId": 123,
    "findingNumber": "INVALID-NUMBER"
}';

Expected JSON Response Format:
{
    "data": {
        "rootCause": "Sample root cause text",
        "correctiveAction": "Sample corrective action text", 
        "correction": "Sample correction text",
        "isSubmitToDnv": false,
        "updatedOn": "2025-09-19T10:30:00.000Z",
        "isDraft": false,
        "respondId": 12345,
        "__typename": "FindingLatestRespondResponse"
    },
    "isSuccess": true,
    "message": "Latest finding response retrieved successfully.",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfFindingLatestRespondResponse"
}

Error Response Format:
{
    "data": {
        "rootCause": null,
        "correctiveAction": null,
        "correction": null,
        "isSubmitToDnv": false,
        "updatedOn": null,
        "isDraft": false,
        "respondId": null,
        "__typename": "FindingLatestRespondResponse"
    },
    "isSuccess": false,
    "message": "Finding not found or inactive.",
    "errorCode": "FINDING_NOT_FOUND",
    "__typename": "BaseGraphResponseOfFindingLatestRespondResponse"
}

Notes:
- Retrieves the most recent response for a specific finding by finding number
- Orders by CreatedDate DESC and ResponseId DESC to get the absolute latest response
- Validates finding exists and user has access to the finding's company
- Returns proper null values when no response exists (maintains data structure)
- Includes proper ISO 8601 datetime formatting for updatedOn field
- Provides comprehensive error handling with appropriate error codes
- Includes audit logging for security and compliance tracking
- Handles both cases: finding with responses and finding without responses
- Returns exact GraphQL response structure with proper type names
- Supports optional user validation - can work with or without userId
- Maintains consistent data structure even in error scenarios
*/



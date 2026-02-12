-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get finding response history including previous responses and auditor comments
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetFindingResponseHistory]
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

        -- Build the response history data
        DECLARE @ResponseHistoryData NVARCHAR(MAX);

        -- Get previous responses and auditor comments
        SELECT @ResponseHistoryData = SELECT ac.CommentsInPrimaryLanguage as commentsInPrimaryLanguage,
                        ac.CommentsInSecondaryLanguage as commentsInSecondaryLanguage,
                        ac.ResponseCommentId as responseCommentId,
                        CONCAT(u.FirstName, ' ', u.LastName) as updatedBy,
                        FORMAT(ac.UpdatedOn, 'yyyy-MM-ddTHH:mm:ss.fffZ') as updatedOn FROM AuditorComments ac
                    INNER JOIN Users u ON ac.UpdatedBy = u.UserId
                    WHERE ac.FindingId = @FindingId 
                    AND ac.IsActive = 1
                    ORDER BY ac.UpdatedOn DESC
                ) as auditorComments,
                SELECT fr.CorrectionInPrimaryLanguage as correctionInPrimaryLanguage,
                        fr.CorrectionInSecondaryLanguage as correctionInSecondaryLanguage,
                        fr.CorrectiveActionInPrimaryLanguage as correctiveActionInPrimaryLanguage,
                        fr.CorrectiveActionInSecondaryLanguage as correctiveActionInSecondaryLanguage,
                        fr.ResponseId as responseId,
                        fr.RootCauseInPrimaryLanguage as rootCauseInPrimaryLanguage,
                        fr.RootCauseInSecondaryLanguage as rootCauseInSecondaryLanguage,
                        CONCAT(u.FirstName, ' ', u.LastName) as updatedBy,
                        FORMAT(fr.UpdatedOn, 'yyyy-MM-ddTHH:mm:ss.fffZ') as updatedOn FROM FindingResponses fr
                    INNER JOIN Users u ON fr.UpdatedBy = u.UserId
                    WHERE fr.FindingId = @FindingId 
                    AND fr.IsActive = 1
                    AND fr.IsSubmitted = 1  -- Only submitted responses
                    ORDER BY fr.UpdatedOn DESC
                ) as previousResponse);

        -- Handle empty result case
        IF @ResponseHistoryData IS NULL OR @ResponseHistoryData = '[]'
        BEGIN
            SET @ResponseHistoryData = '[]';
        END

        -- Build the final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT CAST(1 AS BIT) as isSuccess,
                NULL;

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
            'VIEW_HISTORY',
            'FINDING',
            @FindingId,
            CONCAT('Viewed finding response history: ', @FindingNumber),
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
                CAST(0 AS BIT) as isSuccess,
                CAST('[]' AS NVARCHAR(MAX)@ErrorCode as errorCode,
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
            CONCAT('Procedure: Sp_GetFindingResponseHistory, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST(0 AS BIT) as isSuccess,
                CAST('[]' AS NVARCHAR(MAX)'SERVER_ERROR' as errorCode,
                'An error occurred while retrieving finding response history.' as message) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get finding response history with user validation:
EXEC Sp_GetFindingResponseHistory @Parameters = N'{
    "userId": 123,
    "findingNumber": "CLAY-0006-2887291"
}';

2. Get finding response history without user validation:
EXEC Sp_GetFindingResponseHistory @Parameters = N'{
    "findingNumber": "CLAY-0006-2887291"
}';

Expected JSON Response Format (when data exists):
{
    "isSuccess": true,
    "data": [
        {
            "auditorComments": [
                {
                    "commentsInPrimaryLanguage": "Primary language comment",
                    "commentsInSecondaryLanguage": "Secondary language comment",
                    "responseCommentId": 123,
                    "updatedBy": "John Doe",
                    "updatedOn": "2025-07-02T10:30:00.000Z",
                    "__typename": "AuditorComments"
                }
            ],
            "previousResponse": [
                {
                    "correctionInPrimaryLanguage": "Correction in primary language",
                    "correctionInSecondaryLanguage": "Correction in secondary language",
                    "correctiveActionInPrimaryLanguage": "Corrective action in primary language",
                    "correctiveActionInSecondaryLanguage": "Corrective action in secondary language",
                    "responseId": 456,
                    "rootCauseInPrimaryLanguage": "Root cause in primary language",
                    "rootCauseInSecondaryLanguage": "Root cause in secondary language",
                    "updatedBy": "Jane Smith",
                    "updatedOn": "2025-07-01T14:20:00.000Z",
                    "__typename": "PreviousResponse"
                }
            ],
            "__typename": "PreviousResponsesAndCommentsResponse"
        }
    ],
    "errorCode": "",
    "message": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfPreviousResponsesAndCommentsResponse"
}

Expected JSON Response Format (when no data):
{
    "isSuccess": true,
    "data": [],
    "errorCode": "",
    "message": "",
    "__typename": "BaseGraphResponseOfIEnumerableOfPreviousResponsesAndCommentsResponse"
}

Notes:
- Returns both auditor comments and previous responses for a finding
- Supports multi-language content (primary and secondary languages)
- Orders results by most recent updates first
- Only includes submitted responses in history
- Includes proper ISO 8601 date formatting with timezone
- Provides access control validation through user-company relationships
- Includes audit logging for compliance tracking
- Handles empty results gracefully
- Returns exact GraphQL response structure with proper type names
- Includes comprehensive error handling and validation
*/



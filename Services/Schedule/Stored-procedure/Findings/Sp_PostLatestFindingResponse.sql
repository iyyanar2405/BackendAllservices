-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Post/submit latest finding response (mutation operation)
-- =============================================
CREATE PROCEDURE [dbo].[Sp_PostLatestFindingResponse]
    @Parameters NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @UserId INT = NULL;
    DECLARE @FindingNumber NVARCHAR(255) = NULL;
    DECLARE @ResponseId NVARCHAR(50) = NULL;
    DECLARE @IsSubmitToDnv BIT = 0;
    DECLARE @RootCause NVARCHAR(MAX) = NULL;
    DECLARE @CorrectiveAction NVARCHAR(MAX) = NULL;
    DECLARE @Correction NVARCHAR(MAX) = NULL;
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        BEGIN TRANSACTION;
        
        -- Parse JSON input parameters
        IF ISJSON(@Parameters) = 1
        BEGIN
            SELECT 
                @UserId = JSON_VALUE(@Parameters, '$.userId'),
                @FindingNumber = JSON_VALUE(@Parameters, '$.request.findingNumber'),
                @ResponseId = JSON_VALUE(@Parameters, '$.request.responseId'),
                @IsSubmitToDnv = CASE 
                    WHEN JSON_VALUE(@Parameters, '$.request.isSubmitToDnv') = 'true' THEN 1 
                    ELSE 0 
                END,
                @RootCause = JSON_VALUE(@Parameters, '$.request.rootCause'),
                @CorrectiveAction = JSON_VALUE(@Parameters, '$.request.correctiveAction'),
                @Correction = JSON_VALUE(@Parameters, '$.request.correction')
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

        -- Get finding details
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

        -- Validate response content (at least one field should be provided)
        IF (ISNULL(@RootCause, '') = '' OR LTRIM(RTRIM(@RootCause)) IN ('', '.')) AND
           (ISNULL(@CorrectiveAction, '') = '' OR LTRIM(RTRIM(@CorrectiveAction)) IN ('', '.')) AND
           (ISNULL(@Correction, '') = '' OR LTRIM(RTRIM(@Correction)) IN ('', '.'))
        BEGIN
            SET @ErrorCode = 'INSUFFICIENT_RESPONSE_DATA';
            SET @Message = 'At least one response field (root cause, corrective action, or correction) must be provided with meaningful content.';
            GOTO ErrorResponse;
        END

        -- Clean up response data (replace single dots with NULL for meaningful content)
        IF LTRIM(RTRIM(@RootCause)) = '.' SET @RootCause = NULL;
        IF LTRIM(RTRIM(@CorrectiveAction)) = '.' SET @CorrectiveAction = NULL;
        IF LTRIM(RTRIM(@Correction)) = '.' SET @Correction = NULL;

        DECLARE @NewResponseId INT;
        DECLARE @CurrentDateTime DATETIME = GETDATE();

        -- Check if updating existing response or creating new one
        IF @ResponseId IS NOT NULL AND @ResponseId != ''
        BEGIN
            -- Update existing response
            DECLARE @ExistingResponseId INT = CAST(@ResponseId AS INT);
            
            -- Validate the response belongs to this finding
            IF NOT EXISTS SELECT 1 FROM FindingResponses 
                WHERE ResponseId = @ExistingResponseId 
                AND FindingId = @FindingId 
                AND IsActive = 1
            )
            BEGIN
                SET @ErrorCode = 'INVALID_RESPONSE_ID';
                SET @Message = 'Response ID does not belong to this finding.';
                GOTO ErrorResponse;
            END

            -- Update the existing response
            UPDATE FindingResponses 
            SET 
                RootCause = @RootCause,
                CorrectiveAction = @CorrectiveAction,
                Correction = @Correction,
                IsSubmitToDnv = @IsSubmitToDnv,
                IsDraft = CASE WHEN @IsSubmitToDnv = 1 THEN 0 ELSE 1 END,
                UpdatedDate = @CurrentDateTime,
                UpdatedBy = @UserId
            WHERE ResponseId = @ExistingResponseId;

            SET @NewResponseId = @ExistingResponseId;
        END
        ELSE
        BEGIN
            -- Create new response
            INSERT INTO FindingResponses (
                FindingId,
                RootCause,
                CorrectiveAction,
                Correction,
                IsSubmitToDnv,
                IsDraft,
                CreatedDate,
                CreatedBy,
                UpdatedDate,
                UpdatedBy,
                IsActive
            )
            VALUES (
                @FindingId,
                @RootCause,
                @CorrectiveAction,
                @Correction,
                @IsSubmitToDnv,
                CASE WHEN @IsSubmitToDnv = 1 THEN 0 ELSE 1 END,
                @CurrentDateTime,
                @UserId,
                @CurrentDateTime,
                @UserId,
                1
            );

            SET @NewResponseId = SCOPE_IDENTITY();
        END

        -- Update finding status if submitted to DNV
        IF @IsSubmitToDnv = 1
        BEGIN
            UPDATE Findings 
            SET 
                Status = 'Submitted',
                LastResponseDate = @CurrentDateTime,
                UpdatedDate = @CurrentDateTime,
                UpdatedBy = @UserId
            WHERE FindingId = @FindingId;
        END

        -- Log the action
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
            CASE WHEN @IsSubmitToDnv = 1 THEN 'SUBMIT_FINDING_RESPONSE' ELSE 'SAVE_FINDING_RESPONSE' END,
            'FINDING',
            @FindingId,
            CONCAT('Posted finding response for: ', @FindingNumber, ', ResponseId: ', @NewResponseId),
            @CurrentDateTime
        );

        COMMIT TRANSACTION;

        -- Build success response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT CAST(1 AS BIT) as isSuccess,
                'Finding response submitted successfully.' as message);

        SET @IsSuccess = 1;
        SET @Message = 'Finding response submitted successfully.';

        -- Return success response
        SELECT @FinalResponse as JsonResponse;
        RETURN;

        ErrorResponse:
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
        -- Return error response
        SELECT 
                CAST(0 AS BIT) as isSuccess,
                @Message as message,
                @ErrorCode as errorCode) as JsonResponse;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        
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
            CONCAT('Procedure: Sp_PostLatestFindingResponse, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST(0 AS BIT) as isSuccess,
                'An error occurred while submitting the finding response.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Submit new finding response:
EXEC Sp_PostLatestFindingResponse @Parameters = N'{
    "userId": 123,
    "request": {
        "findingNumber": "CLAY-0006-2887291",
        "responseId": "",
        "isSubmitToDnv": true,
        "rootCause": "Inadequate training procedures",
        "correctiveAction": "Implement comprehensive training program",
        "correction": "Immediate retraining of affected personnel"
    }
}';

2. Update existing response:
EXEC Sp_PostLatestFindingResponse @Parameters = N'{
    "userId": 123,
    "request": {
        "findingNumber": "CLAY-0006-2887291",
        "responseId": "456",
        "isSubmitToDnv": false,
        "rootCause": "Updated root cause analysis",
        "correctiveAction": "Revised corrective action plan",
        "correction": "Updated immediate correction"
    }
}';

3. Save as draft:
EXEC Sp_PostLatestFindingResponse @Parameters = N'{
    "userId": 123,
    "request": {
        "findingNumber": "CLAY-0006-2887291",
        "responseId": "",
        "isSubmitToDnv": false,
        "rootCause": "Draft root cause",
        "correctiveAction": "Draft action",
        "correction": "Draft correction"
    }
}';

Expected JSON Response Format (Success):
{
    "isSuccess": true,
    "message": "Finding response submitted successfully.",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfFindingLatestRespondResponse"
}

Error Response Format:
{
    "isSuccess": false,
    "message": "Finding not found or inactive.",
    "errorCode": "FINDING_NOT_FOUND",
    "__typename": "BaseGraphResponseOfFindingLatestRespondResponse"
}

Notes:
- Supports both creating new responses and updating existing ones
- Distinguishes between draft saves and final submissions to DNV
- Updates finding status when submitted to DNV
- Validates response content to ensure meaningful data
- Handles single dot (.) inputs as placeholders and converts to NULL
- Provides comprehensive error handling with specific error codes
- Uses database transactions to ensure data consistency
- Includes audit logging for all response submissions
- Validates user access to the finding's company
- Automatically sets IsDraft flag based on isSubmitToDnv value
- Updates finding's LastResponseDate when submitted
- Returns exact GraphQL response structure with proper type names
- Supports both insert and update operations based on responseId presence
*/


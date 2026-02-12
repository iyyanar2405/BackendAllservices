CREATE PROCEDURE [dbo].[Sp_GetUserValidation]
    @userId NVARCHAR(255) = NULL, -- Optional: if not provided, can use current session context
    @veracityId NVARCHAR(255) = NULL -- Optional: alternative user identifier
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Determine user to validate (could be from session context or parameters)
        DECLARE @currentUserId NVARCHAR(255)
        DECLARE @currentVeracityId NVARCHAR(255)
        
        -- If no specific user provided, use session context or default logic
        IF @userId IS NULL AND @veracityId IS NULL
        BEGIN
            -- In a real implementation, this would come from the session/token context
            -- For now, we'll handle the case where user context is provided
            SET @currentUserId = @userId
            SET @currentVeracityId = @veracityId
        END
        ELSE
        BEGIN
            SET @currentUserId = @userId
            SET @currentVeracityId = @veracityId
        END

        -- Validate and get user information
        SELECT 
                    CASE 
                        WHEN u.IsActive = 1 THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END as userIsActive,
                    COALESCE(u.TermsAcceptanceRedirectUrl, '') as termsAcceptanceRedirectUrl,
                    u.PolicySubCode as policySubCode,
                    CASE 
                        WHEN u.IsDnvUser = 1 THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END as isDnvUser,
                    COALESCE(u.Email, '') as userEmail,
                    COALESCE(u.VeracityId, '') as veracityId,
                    COALESCE(u.PortalLanguage, 'en') as portalLanguage,
                    CASE 
                        WHEN u.IsAdmin = 1 THEN CAST(1 AS BIT)
                        ELSE CAST(0 AS BIT)
                    END as isAdmin FROM Users u
                WHERE (
                    (@currentUserId IS NOT NULL AND u.UserId = @currentUserId) OR
                    (@currentVeracityId IS NOT NULL AND u.VeracityId = @currentVeracityId) OR
                    (@currentUserId IS NULL AND @currentVeracityId IS NULL AND u.IsActive = 1)
                )
                AND u.IsActive = 1
            '' as errorCode,
            'Status fetched successfully' as message,

        -- If no user found, return inactive response
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 
                        CAST(0 AS BIT) as userIsActive,
                        '' as termsAcceptanceRedirectUrl,
                        NULL as policySubCode,
                        CAST(0 AS BIT) as isDnvUser,
                        '' as userEmail,
                        '' as veracityId,
                        'en' as portalLanguage,
                        CAST(0 AS BIT) as isAdmin,
                        
                'USER_NOT_FOUND' as errorCode,
                'User not found or inactive' as message,
                
        END

    END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
                    CAST(0 AS BIT) as userIsActive,
                    '' as termsAcceptanceRedirectUrl,
                    NULL as policySubCode,
                    CAST(0 AS BIT) as isDnvUser,
                    '' as userEmail,
                    '' as veracityId,
                    'en' as portalLanguage,
                    CAST(0 AS BIT) as isAdmin,
                    
            'DATABASE_ERROR' as errorCode,
            ERROR_MESSAGE() as message; END CATCH
END



CREATE PROCEDURE [dbo].[Sp_GetUserProfile]
    @userId NVARCHAR(255) = NULL, -- Optional: if not provided, can use current session context
    @veracityId NVARCHAR(255) = NULL -- Optional: alternative user identifier
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Determine user to get profile for (could be from session context or parameters)
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

        -- Get user profile information
        SELECT 
                    COALESCE(u.FirstName, '') as firstName,
                    COALESCE(u.LastName, '') as lastName,
                    COALESCE(u.DisplayName, CONCAT(COALESCE(u.FirstName, ''), ' ', COALESCE(u.LastName, ''))) as displayName,
                    COALESCE(c.CountryName, '') as country,
                    COALESCE(c.CountryCode, '') as countryCode,
                    COALESCE(u.Region, '') as region,
                    COALESCE(u.Email, '') as email,
                    u.Phone as phone,
                    COALESCE(u.CommunicationLanguage, 'English') as communicationLanguage,
                    COALESCE(u.JobTitle, '') as jobTitle,
                    COALESCE(u.PortalLanguage, 'en') as portalLanguage,
                    COALESCE(u.VeracityId, '') as veracityId,
                    SELECT DISTINCT ur.RoleLevel
                                FROM UserRoles ur2
                                WHERE ur2.UserId = u.UserId 
                                AND ur2.RoleName = ur.RoleName
                                ORDER BY ur2.RoleLevel
                            ) as roleLevel,
                            LTRIM(RTRIM(ur.RoleName)) as roleName FROM UserRoles ur
                        WHERE ur.UserId = u.UserId
                        GROUP BY ur.RoleName
                        ORDER BY ur.RoleName
                    ) as accessLevel FROM Users u
                LEFT JOIN Countries c ON u.CountryId = c.CountryId
                WHERE (
                    (@currentUserId IS NOT NULL AND u.UserId = @currentUserId) OR
                    (@currentVeracityId IS NOT NULL AND u.VeracityId = @currentVeracityId) OR
                    (@currentUserId IS NULL AND @currentVeracityId IS NULL AND u.IsActive = 1)
                )
                AND u.IsActive = 1
            1 as isSuccess,
            'Profile retrieved successfully.' as message,

        -- If no user found, return error response
        IF @@ROWCOUNT = 0
        BEGIN
            SELECT 
                NULL as data,
                'User profile not found.' as message,
                'USER_NOT_FOUND' as errorCode,
                
        END

    END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            NULL as data,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode; END CATCH
END



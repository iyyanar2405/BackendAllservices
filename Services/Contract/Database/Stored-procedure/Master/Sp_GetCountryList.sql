-- =============================================
-- Author: Generated for Customer Portal Backend
-- Create date: 2025-09-19
-- Description: Get list of all countries for settings dropdowns
-- =============================================
CREATE PROCEDURE [dbo].[Sp_GetCountryList]
    @Parameters NVARCHAR(MAX) = '{}'
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @ErrorCode NVARCHAR(50) = '';
    DECLARE @Message NVARCHAR(500) = '';
    DECLARE @IsSuccess BIT = 0;
    
    BEGIN TRY
        -- Get all active countries
        DECLARE @CountriesData NVARCHAR(MAX);
        
        SELECT @CountriesData = SELECT c.CountryId as id,
                c.CountryName as countryName,
                c.CountryCode as countryCode,
                CAST(ISNULL(c.IsActive, 1) AS BIT) as isActive FROM Countries c
            WHERE c.IsActive = 1
            ORDER BY c.CountryName
        );

        -- Handle empty result
        IF @CountriesData IS NULL OR @CountriesData = ''
            SET @CountriesData = '[]';

        -- Build final response
        DECLARE @FinalResponse NVARCHAR(MAX);
        SELECT @FinalResponse = SELECT NULL as isSuccess);

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
            NULL,
            ERROR_MESSAGE(),
            CONCAT('Procedure: Sp_GetCountryList, Line: ', ERROR_LINE(), ', Error: ', ERROR_MESSAGE()),
            GETDATE()
        );

        -- Return error response
        SELECT 
                CAST('[]' AS NVARCHAR(MAX)CAST(0 AS BIT) as isSuccess,
                'An error occurred while retrieving countries.' as message,
                'SERVER_ERROR' as errorCode) as JsonResponse;
    END CATCH
END

/*
Usage Examples:

1. Get all countries:
EXEC Sp_GetCountryList @Parameters = N'{}';

Expected JSON Response Format:
{
    "data": [
        {
            "id": 1,
            "countryName": "Afghanistan",
            "countryCode": "AF",
            "isActive": true,
            "__typename": "Country"
        },
        {
            "id": 2,
            "countryName": "Albania",
            "countryCode": "AL",
            "isActive": true,
            "__typename": "Country"
        }
    ],
    "isSuccess": true,
    "message": "",
    "errorCode": "",
    "__typename": "BaseGraphResponseOfCountry"
}

Notes:
- Retrieves all active countries sorted alphabetically
- Returns empty array if no countries found
- No authentication required (public reference data)
- Includes both country name and ISO country code
- Simple lookup for dropdown/selection components
- Returns exact GraphQL response structure with proper type names
*/



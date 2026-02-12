/* ************************************************************* */
/* ***** Create the stored procedure that Valids Tokens ***** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[_TokenValid] (@tokenId bigint, @isValid int out)
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-08-15 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Return 1 if the passed [TOKEN_ID] is valid, else 0.         
	 * 2023-01-16:	Renamed to _TokenValid for BUG 22797
	 * ************************************************************* */
	SET NOCOUNT ON;

	SET @isValid = ISNULL((SELECT 1 
					       FROM [dbo].[Token_Request] 
						   WHERE [Token_ID] = @tokenId 
						     AND [Expires_DT] > GETDATE()), 0)
END
GO
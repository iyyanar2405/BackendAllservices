/* ************************************************************* */
/* ***** Create the stored procedure that generates Tokens ***** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[_AddTokenRequest] (@tokenId bigint out)
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-06-30 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Return the Identity ID created. This is used
	 *              by [dbo.TokenEncrypt] & [dbo.TokenDecrypt] via the
	 *              returned [TOKEN_ID]. This allows users authenticated
	 *              via Platform to access services hosted outside
	 *              Platforms Sessions and Authentication System.
	 *
	 *              The [TOKEN_ID] returned have dual temporal properties.
	 * 
	 *              First they have a TTL as defined by [Expires_DT]'s
	 *              default constraint, currently 24 hrs.
	 *
	 *              Second they have an active period that can be
	 *              temporally extended via [dbo.TouchToken].
	 * 2023-01-16:	Renamed to _AddTokenRequest for BUG 22797
	 * ************************************************************* */
	SET NOCOUNT ON;

	-- Be self cleaning, remove any Tokens that have expired.
	DELETE FROM [dbo].[Token_Request] WHERE [Expires_DT] < GETDATE()

	-- Create a new record and send back the request ID.
	INSERT INTO [dbo].[Token_Request] DEFAULT VALUES 

	SET @tokenId = SCOPE_IDENTITY()
END
GO
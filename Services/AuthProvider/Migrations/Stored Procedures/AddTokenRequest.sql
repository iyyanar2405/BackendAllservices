/* ************************************************************* */
/* ***** Create the stored procedure that generates Tokens ***** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[AddTokenRequest] (@tokenId bigint out)
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-06-30 
	 * Version: 1.0.1
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
	 * 2023-01-16:	Reworked to be a STUB to call either (local)
					or PRDSQL01 if server is PRDSQL03 for BUG 22797
	 * ************************************************************* */
	SET NOCOUNT ON;

	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_AddTokenRequest]
			@tokenId = @tokenId OUTPUT
	ELSE
		EXEC [AuthProvider].[dbo].[_AddTokenRequest]
			@tokenId = @tokenId OUTPUT

END
GO
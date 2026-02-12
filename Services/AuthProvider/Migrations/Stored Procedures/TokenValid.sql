/* ************************************************************* */
/* ***** Create the stored procedure that Valids Tokens ***** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[TokenValid] (@tokenId bigint, @isValid int out)
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-08-15 
	 * Version: 1.0.1
	 *
	 * Description: Return 1 if the passed [TOKEN_ID] is valid, else 0.
	 * 2023-01-16:	Reworked to be a STUB to call either (local)
	 *				or PRDSQL01 if server is PRDSQL03,
	 *				and to convert incoming VARCHAR(MAX) to VARCHAR(8000)
	 *				for BUG 22797
	 * ************************************************************* */
	SET NOCOUNT ON;

	/* Determine if we need to run this on the linked server or if we can run it locally. */
	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_TokenValid]
			@tokenId = @tokenId,
			@isValid = @isValid OUTPUT
	ELSE
		EXEC [AuthProvider].[dbo].[_TokenValid]
			@tokenId = @tokenId,
			@isValid = @isValid OUTPUT
END
GO
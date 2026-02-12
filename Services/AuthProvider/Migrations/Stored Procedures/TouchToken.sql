/* ************************************************************* */
/* * Create the stored procedure that Updates Tokens Temporality */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[TouchToken] (@tokenId bigint)
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-06-30 
	 * Version: 1.0.1
	 * Modified: 2023-01-17, Seth Buckley
	 *
	 * Description: Extends the [TEMPORAL_TT] time for a valid token.
	 * 2023-01-16:	Reworked to be a STUB to call either (local)
	 *				or PRDSQL01 if server is PRDSQL03,
	 *				and to convert incoming VARCHAR(MAX) to VARCHAR(8000)
	 *				for BUG 22797
	 * ************************************************************* */
	SET NOCOUNT ON;

	/* Determine if we need to run this on the linked server or if we can run it locally. */
	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_TouchToken]
			@tokenId = @tokenId
	ELSE
		EXEC [AuthProvider].[dbo].[_TouchToken]
			@tokenId = @tokenId
END
GO

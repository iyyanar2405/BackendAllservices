/* ************************************************************* */
/* * Create the stored procedure that Updates Tokens Temporality */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[_TouchToken] (@tokenId bigint)
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-06-30 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Extends the [TEMPORAL_TT] time for a valid token.         
	 * 2023-01-16:	Renamed to _TouchToken for BUG 22797
	 * ************************************************************* */
	SET NOCOUNT ON;

	UPDATE [dbo].[Token_Request]
	SET [Temporal_TT] = GETDATE()
	WHERE [Token_ID] = @tokenId 
	  AND [Expires_DT] > GETDATE()
END
GO

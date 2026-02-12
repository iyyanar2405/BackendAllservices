/* ************************************************************* */
/* *** Create the stored procedure that Decrypts using Tokens ** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[_TokenDecrypt]
	   @tokenId					bigint
	  ,@temporallyMinutesOld	int
      ,@isSecret				varchar(3000)
	  ,@clearText				varchar(3000) out
WITH ENCRYPTION
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-06-30 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Use the passed [TOKEN_ID] to determine the token 
	 *              to validate.
	 *
	 *              Verify [Expires_DT] has not expired.
	 *
	 *              When a token is intially created from [AddTokenRequest]
	 *              it's temporally not active until [TouchToken] has been 
	 *              called.
	 *
	 *              [temporallyMinutesOld] is used to create a time in
	 *              the past. We then Compare that past time against
	 *              what is currently stored in [TEMPORAL_TT] to 
	 *              validate the token has not expired.
	 *
	 *              If the [TOKEN_ID] is invalid or the time outs have
	 *              kicked off then return an empty string.
	 *              
	 *              Else take the contents of [IS_SECRET] and decrypt
	 *              into [CLEAR_TEXT]
	 * 2023-01-16:	Renamed to _TokenDecrypt for BUG 22797
	 * 2023-01-19:	Changing VARCHAR(8000) to VARCHAR(3000) for the
	 *				SQL encryption bug
	 * ************************************************************* */
	SET NOCOUNT ON;

	-- Initialize to an ERRORED state
	SET @clearText = ''

	DECLARE @TemporallyExpired DATETIME
	SET @TemporallyExpired = DATEADD(minute, -1 * @temporallyMinutesOld, GETDATE())

	-- Verify the passed token id has not expired.
	IF EXISTS (SELECT 1 
			   FROM [dbo].[Token_Request] 
			   WHERE [Token_ID] = @tokenId 
				 AND [Expires_DT] > GETDATE() -- The TTL must still be valid.
				 AND [Temporal_TT] > @TemporallyExpired) -- Verify that STL as defined by the caller is not expired.
		BEGIN
			-- this will connect to the STUB procedure and be redirected
			EXEC [dbo].[DataDecrypt] @isSecret, @clearText OUTPUT
		END
END
GO
/* ********************************************************************** */
/* *** Create the stored procedure that Encrypts only for valid Tokens ** */
/* ********************************************************************** */
CREATE PROCEDURE [dbo].[_TokenEncrypt]
	   @tokenId  		bigint
      ,@keepSecret		varchar(3000)
	  ,@madeSecret      varchar(8000) out
WITH ENCRYPTION
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-07-18 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Use the passed [TOKEN_ID] to determine if
	 *              temporally valid to Encrypt. If not valid then
	 *              return an empty string.
	 *              
	 *              Else take the contents of [KEEP_SECRET] and encrypt
	 *              into [MADE_SECRET]
	 * 2023-01-16:	Renamed to _TokenEncrypt for BUG 22797
	 * 2023-01-19:	Changing VARCHAR(8000) to VARCHAR(3000) for the
	 *				SQL encryption bug
	 * ************************************************************* */
	SET NOCOUNT ON;

	-- Initialize to an ERRORED state
	SET @madeSecret = ''

	-- Verify the passed token id has not expired.
	IF EXISTS (SELECT 1 
	           FROM [dbo].[Token_Request] 
			   WHERE [Token_ID] = @tokenId 
				 AND [Expires_DT] > GETDATE())
		BEGIN
			-- this will connect to the STUB procedure and be redirected
			EXEC [dbo].[DataEncrypt] @keepSecret, @madeSecret OUTPUT
		END
END
GO
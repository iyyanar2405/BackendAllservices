/* ************************************************************* */
/* *** Create the stored procedure that Decrypts using Tokens ** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[TokenDecrypt]
	   @tokenId					bigint
	  ,@temporallyMinutesOld	int
      ,@isSecret				varchar(max)
	  ,@clearText				varchar(max) out
AS
BEGIN TRY
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
	 * 2023-01-16:	Reworked to be a STUB to call either (local)
	 *				or PRDSQL01 if server is PRDSQL03,
	 *				and to convert incoming VARCHAR(MAX) to VARCHAR(8000)
	 *				for BUG 22797
	 * 2023-01-19:	Changing VARCHAR(MAX) to VARCHAR(3000) for the
	 *				remote call to PRDSQL01 due to SQL encryption bug
	 * ************************************************************* */
	SET NOCOUNT ON;

	IF LEN(@isSecret) > 3000
	BEGIN
		RAISERROR ('isSecret parameter too large.', 16, 1);
	END

	DECLARE @secret VARCHAR(3000) = CAST(@isSecret AS VARCHAR(3000))
	DECLARE @clear VARCHAR(3000)

	/* Determine if we need to run this on the linked server or if we can run it locally. */
	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_TokenDecrypt]
			@tokenId = @tokenId,
			@temporallyMinutesOld = @temporallyMinutesOld,
			@isSecret = @secret,
			@clearText = @clear OUTPUT
	ELSE
		EXEC [AuthProvider].[dbo].[_TokenDecrypt]
			@tokenId = @tokenId,
			@temporallyMinutesOld = @temporallyMinutesOld,
			@isSecret = @secret,
			@clearText = @clear OUTPUT

	/* If the outbound parameter value is too large, throw an exception. */
	IF LEN(@clear) > 3000
	BEGIN
		RAISERROR ('clearText parameter too large.', 16, 1);
	END

	/* Cast and return data. */
	SET @clearText = CAST(@clear AS varchar(3000));

END TRY
BEGIN CATCH
    DECLARE @ErrorMessage NVARCHAR(4000);
    DECLARE @ErrorSeverity INT;
    DECLARE @ErrorState INT;

    SELECT
        @ErrorMessage = ERROR_MESSAGE(),
        @ErrorSeverity = ERROR_SEVERITY(),
        @ErrorState = ERROR_STATE();

    -- Use RAISERROR inside the CATCH block to return error
    -- information about the original error that caused
    -- execution to jump to the CATCH block.
    RAISERROR (@ErrorMessage, -- Message text.
               @ErrorSeverity, -- Severity.
               @ErrorState -- State.
               );
END CATCH;
GO
/* ********************************************************************** */
/* *** Create the stored procedure that Encrypts only for valid Tokens ** */
/* ********************************************************************** */
CREATE PROCEDURE [dbo].[TokenEncrypt]
	   @tokenId  		bigint
      ,@keepSecret		varchar(max)
	  ,@madeSecret      varchar(max) out
AS
BEGIN TRY
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-07-18 
	 * Version: 1.0.1
	 * Modified: 2023-01-17, Seth Buckley
	 *
	 * Description: Use the passed [TOKEN_ID] to determine if
	 *              temporally valid to Encrypt. If not valid then
	 *              return an empty string.
	 *              
	 *              Else take the contents of [KEEP_SECRET] and encrypt
	 *              into [MADE_SECRET]
	 * 2023-01-17:	Reworked to be a STUB to call either (local)
	 *				or PRDSQL01 if server is PRDSQL03,
	 *				and to convert incoming VARCHAR(MAX) to VARCHAR(8000)
	 *				for BUG 22797
	 * 2023-01-19:	Changing internal @secret variable to 3000 and @made to 8000
	 * ************************************************************* */
	SET NOCOUNT ON;

	/* If the inbound parameter value is too large, throw an exception. */
	IF LEN(@keepSecret) > 3000
	BEGIN
		RAISERROR ('keepSecret parameter too large.', 16, 1);
	END

	/* Publicly, the procedure comes in as a MAX however the private needs a max of 8000 because of the linked server requirements. */
	DECLARE @secret varchar(3000) = CAST(@keepSecret AS varchar(3000));
	DECLARE @made	varchar(8000);

	-- Initialize to an ERRORED state
	SET @madeSecret = ''

	/* Determine if we need to run this on the linked server or if we can run it locally. */
	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_TokenEncrypt]
			@tokenId = @tokenId,
			@keepSecret = @secret,
			@madeSecret = @made OUTPUT
	ELSE
		EXEC [AuthProvider].[dbo].[_TokenEncrypt]
			@tokenId = @tokenId,
			@keepSecret = @secret,
			@madeSecret = @made OUTPUT

	/* If the outbound parameter value is too large, throw an exception. */
	IF LEN(@made) > 3000
	BEGIN
		RAISERROR ('[made] parameter too large.', 16, 1);
	END

	/* Cast and return data. */
	SET @madeSecret = CAST(@made AS VARCHAR(3000));

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
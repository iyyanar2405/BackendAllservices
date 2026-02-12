/* ************************************************************* */
/* ***     Create the stored procedure that Decrypts data     ** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[DataDecrypt]
       @isSecret				varchar(max)
	  ,@clearText				varchar(max) out
AS
BEGIN TRY
	/**************************************************************
	* Author: Iyyanar Kalivarathan
	* Date: 2022-07-18 
	* Version: 1.0.1
	*
	* Description:	Take the contents of [IS_SECRET] and decrypt
	*				into [CLEAR_TEXT]
	* 2023-01-16:	Reworked to be a STUB to call either (local)
	*				or PRDSQL01 if server is PRDSQL03,
	*				and to convert incoming VARCHAR(MAX) to VARCHAR(8000)
	*				for BUG 22797
	* 2023-01-19:	Changing VARCHAR(MAX) to VARCHAR(3000) for the
	*				remote call to PRDSQL01 due to SQL encryption bug
	***************************************************************/
	SET NOCOUNT ON;

	/* If the inbound parameter value is too large, throw an exception. */
	IF LEN(@isSecret) > 8000
	BEGIN
		RAISERROR ('isSecret parameter too large.', 16, 1);
	END

	/* Publicly, the procedure comes in as a MAX however the private needs a max of 8000 because of the linked server requirements. */
	DECLARE @secret varchar(8000) = CAST(@isSecret AS varchar(8000));
	DECLARE @clear	varchar(3000);
	
	/* Determine if we need to run this on the linked server or if we can run it locally. */
	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_DataDecrypt]
			@isSecret = @secret,
			@clearText = @clear OUTPUT
	ELSE
		EXEC [AuthProvider].[dbo].[_DataDecrypt]
			@isSecret = @secret,
			@clearText = @clear OUTPUT	

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
/* ************************************************************* */
/* *** Create the stored procedure that Encrypts data ** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[DataEncrypt]
       @keepSecret		varchar(max)
	  ,@madeSecret      varchar(max) out
AS
BEGIN TRY
    /* *************************************************************
	* Author: Iyyanar Kalivarathan
	* Date: 2022-07-18 
	* Version: 1.0.1
	*
	* Description:	Take the contents of [KEEP_SECRET] and encrypt
	*				into [MADE_SECRET]
	* 2023-01-16:	Reworked to be a STUB to call either (local)
	*				or PRDSQL01 if server is PRDSQL03,
	*				and to convert incoming VARCHAR(MAX) to VARCHAR(8000)
	*				for BUG 22797
	* 2023-01-19:	Changing internal @keep variable to 3000 and @made to 8000
	************************************************************** */
	SET NOCOUNT ON;

	IF LEN(@keepSecret) > 3000
	BEGIN
		RAISERROR ('keepSecret parameter is too large.', 16, 1);
	END
	DECLARE @keep VARCHAR(3000) = CAST(@keepSecret AS VARCHAR(3000));
	DECLARE @made VARCHAR(8000);

	IF @@SERVERNAME = 'PRDSQL03'
		EXEC [PRDSQL01].[AuthProvider].[dbo].[_DataEncrypt]
			@keepSecret = @keep,
			@madeSecret = @made OUTPUT
	ELSE
		EXEC [AuthProvider].[dbo].[_DataEncrypt]
			@keepSecret = @keep,
			@madeSecret = @made OUTPUT

	IF LEN(@made) > 8000
	BEGIN
		RAISERROR ('[made] output parameter too large.', 16, 1);
	END
	
	/* Cast and return data. */
	SET @madeSecret = CAST(@made AS VARCHAR(8000));

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
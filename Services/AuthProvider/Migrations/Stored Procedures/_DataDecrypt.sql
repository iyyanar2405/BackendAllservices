/* ************************************************************* */
/* ***     Create the stored procedure that Decrypts data     ** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[_DataDecrypt]
       @isSecret				VARCHAR(8000)
	  ,@clearText				VARCHAR(3000) out
WITH ENCRYPTION, EXECUTE AS OWNER
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-07-18 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Take the contents of [IS_SECRET] and decrypt
	 *              into [CLEAR_TEXT]
	 * 2023-01-16:	Renamed to _DataDecrypt for BUG 22797
	 * 2023-01-19:	@isSecret needs 8000, @clearText needs 3000
	 * ************************************************************* */
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY HadvidaKey  
		DECRYPTION BY CERTIFICATE HadvidaVault
		WITH PASSWORD = 'k$EN!L5324Dcbe@ZqAUrTV';

	DECLARE @asBinary VARBINARY(MAX)
	SET @asBinary = CONVERT(VARBINARY(MAX), @isSecret, 1)

	-- Now decrypt and convert to a string.
	-- TRY_CONVERT returns NULL if it fails, ISNULL will convert a NULL to empty string
	SET @clearText = ISNULL(TRY_CONVERT(VARCHAR(3000), DecryptByKey(@asBinary)), '')

	CLOSE SYMMETRIC KEY HadvidaKey
END
GO
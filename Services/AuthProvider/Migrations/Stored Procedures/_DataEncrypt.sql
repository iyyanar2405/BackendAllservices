/* ************************************************************* */
/* *** Create the stored procedure that Encrypts data ** */
/* ************************************************************* */
CREATE PROCEDURE [dbo].[_DataEncrypt]
       @keepSecret		varchar(3000)
	  ,@madeSecret      varchar(8000) out
WITH ENCRYPTION, EXECUTE AS OWNER
AS
BEGIN
    /* *************************************************************
	 * Author: Iyyanar Kalivarathan
	 * Date: 2022-07-18 
	 * Version: 1.0.1
	 * Modified: 2023-01-16, Seth Buckley
	 *
	 * Description: Take the contents of [KEEP_SECRET] and encrypt
	 *              into [MADE_SECRET]
	 * 2023-01-16:	Renamed to _DataEncrypt for BUG 22797
	 * 2023-01-19:	keepSecret 3000 needs 6122 to decrypt into madeSecret
	 * ************************************************************* */
	SET NOCOUNT ON;

	OPEN SYMMETRIC KEY HadvidaKey  
		DECRYPTION BY CERTIFICATE HadvidaVault
		WITH PASSWORD = 'k$EN!L5324Dcbe@ZqAUrTV';

	-- TRY_CONVERT returns NULL if it fails, ISNULL will convert a NULL to empty string
	SET @madeSecret = ISNULL(TRY_CONVERT(VARCHAR(8000), EncryptByKey(Key_GUID('HadvidaKey'), @keepSecret), 1), '')


	CLOSE SYMMETRIC KEY HadvidaKey
END
GO
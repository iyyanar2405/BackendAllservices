# Introduction 
The AuthProvider handles user and pdf security.

# PDF Security Flow
The flow from the report starts in the Stored Procedure.

- token_id = AuthProvider.dbo.Token_Request 
- Then for each patient data row you'd call: AuthProvider.dbo.TokenEncrypt(token_id, {UserID}|{DemographicID})
- This will generate the encrypted version of the user id and demographic id to append to the URL.
- The URL parameters are: {Site}/{ReportID}/{TokenID}/{EncryptedData}

The flow from the PDF side.
- AuthProvider.dbo.TouchToken(TokenID) -> This activated the token.
- AuthProvider.dbo.TokenDecrypt(TokenID, TemporalTokenMinutes, {EncryptedData})
  `TemporalTokenMinutes` := This allows us to define how long a token is valid for. 
If we pass 15 then the tokens temporal side must have a Date/Time within the last 15 minutes. 
This allows us to define different temporal times based on the use.
//"AuthProviderConn": "Server=DESKTOP-6A76F24\\MSSQLSERVER1;Database=AuthProvider;MultipleActiveResultSets=true;Integrated Security=true;User Id=sa;Password=Smart@#123;Trusted_Connection=True;TrustServerCertificate=True;"


# Token Testing
### Encrypt String

```sh
DECLARE	@madeSecretA varchar(max)

EXEC	[dbo].[DataEncrypt]
		@keepSecret = N'Hello World',
		@madeSecret = @madeSecretA OUTPUT

SELECT	@madeSecretA as N'@madeSecret'
GO
```

### Decrypt String
```sh
DECLARE	@clearTextA varchar(max)

EXEC	[dbo].[DataDecrypt]
		@isSecret = N'0x00F670D8A3F7E14EB654ABEDD3AD9F4301000000C72060A8E884DF8E6AD939C6B2338C027D612EEDE2EBD202BC2EACBD17593DBA05D5FFD711B58BDB2287170812308B49',
		@clearText = @clearTextA OUTPUT

SELECT	@clearTextA as N'@clearText'
```

### Create a Token
```sh
DECLARE	@tokenId bigint
EXEC	[dbo].[AddTokenRequest]
		@tokenId = @tokenId OUTPUT
SELECT	@tokenId as N'@tokenId'
GO
```

### Extend a Temporal Token
```sh
EXEC	[dbo].[TouchToken]
		@tokenId = 5
```

### Encrypt only If Temporal Token is valid
```sh
DECLARE	@madeSecret varchar(max)

EXEC	[dbo].[TokenEncrypt]
		@tokenId = 5,
		@keepSecret = 'This is my very long test that has lots of words and numbers and other things 1234567890 `~!@#$%^&*()-_=+ ,./;[]\<>?:"',
		@madeSecret = @madeSecret OUTPUT

SELECT	@madeSecret as 'encryptedHexString'
```

### Decrypt a string if the Temporal Token is valid
```sh
DECLARE	@clearText varchar(max)

EXEC	[dbo].[TokenDecrypt]
		@tokenId = 5,
		@temporallyMinutesOld = 15,
		@isSecret = N'0x00F670D8A3F7E14EB654ABEDD3AD9F430100000095B00C5EE8790AF87798A265022DAF4FE5B6C1E2670BB787ED91F7310068D852DA38852C6B5CC6D1F687B9A479E63A8B496BB32CCF5AF530E8A074438231B8BFDECB6F062030B25195EDA7A523BF566E9CD64AF2EA01C1554E055D061F3B01FAA1A569EC97EB7A372CA18F3DE21D7AC65CD6593094BAFA4F011387866896836CD8F8096F947074231AE7BBDE53F6F678',
		@clearText = @clearText OUTPUT

SELECT	@clearText as 'decodedString'
```


# Scaffold Identity in ASP.NET Core
https://learn.microsoft.com/en-us/aspnet/core/security/authentication/scaffold-identity?view=aspnetcore-6.0&tabs=visual-studio

# Swagger Open Api Interface
Accessible from https://localhost:44344/swagger
- Swagger Doc needs to be manually kept up to date when endpoint documentation changes.
	Generating swagger docs can be done by running the following:
	Example Dev Powershell Run: dotnet swagger tofile --output swagger/swagger.json bin/debug/net6.0/AuthProvider.dll v1

# AuthProvider Database Deployment
1) Deploy initial AuthProvider DB using Script location at Repo Path: System_Administration\Uploader\2-CreateAuthProviderDBSimple.sql
2) Warning!! <strong>MUST UPDATE APPSETTING FLAG IsDbMigration to true</strong>; If no AuthProvider Db Signature has been created. 
3) Verify AuthProviderConn has the correct connectionstring in appsettings.json
3) In Package Manager Console run update-database --context=AuthProviderContext -verbose 
4) all Identity Tables should be added to the AuthProvider Db now.

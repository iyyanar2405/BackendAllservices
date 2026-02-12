-- Insert sample data for SystemPreferences table
IF NOT EXISTS (SELECT 1 FROM [dbo].[SystemPreferences])
BEGIN
	INSERT INTO [dbo].[SystemPreferences] ([PreferencesJson], [CreatedDate], [ModifiedDate], [UpdatedBy])
	VALUES
	(N'{"generalSettings":{"systemName":"Customer Portal","systemVersion":"2.1.0","maintenanceMode":false,"maxFileUploadSize":10485760,"sessionTimeout":3600}}', GETDATE(), GETDATE(), NULL);
END

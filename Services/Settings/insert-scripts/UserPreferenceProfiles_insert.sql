-- Insert sample data for UserPreferenceProfiles table
IF NOT EXISTS (SELECT 1 FROM [dbo].[UserPreferenceProfiles] WHERE [UserId] = 1)
BEGIN
	INSERT INTO [dbo].[UserPreferenceProfiles]
	([UserId], [PreferencesJson], [CreatedDate], [ModifiedDate], [UpdatedBy])
	VALUES
	(1, N'{"language":"en-US","timeZone":"America/New_York","dateFormat":"MM/dd/yyyy","timeFormat":"12h","currency":"USD","notifications":{"email":true,"browser":true,"mobile":false,"auditReminders":true,"certificateExpiry":true,"findingUpdates":true,"invoiceAlerts":true},"dashboard":{"defaultView":"overview","widgets":["pending-audits","certificate-status"],"refreshInterval":300},"display":{"theme":"light","compactMode":false,"showHelpTips":true,"itemsPerPage":25}}', GETDATE(), GETDATE(), 1);
END

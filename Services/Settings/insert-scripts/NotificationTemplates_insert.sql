-- Insert sample data for NotificationTemplates table
IF NOT EXISTS (SELECT 1 FROM [dbo].[NotificationTemplates] WHERE [NotificationTemplateId] IN (1, 2))
BEGIN
	SET IDENTITY_INSERT [dbo].[NotificationTemplates] ON;

	INSERT INTO [dbo].[NotificationTemplates]
	([NotificationTemplateId], [TemplateName], [TemplateType], [Category], [Subject], [BodyHtml], [BodyText], [VariablesJson], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
	VALUES
	(1, 'Audit Reminder', 'Email', 'Audit', 'Upcoming Audit Reminder - {{CompanyName}}',
	 '<p>Dear {{ContactName}},</p><p>This is a reminder that your audit is scheduled for {{AuditDate}}.</p>',
	 'Dear {{ContactName}}, This is a reminder that your audit is scheduled for {{AuditDate}}.',
	 N'["CompanyName","ContactName","AuditDate","AuditType"]', 1, GETDATE(), GETDATE(), NULL, NULL),
	(2, 'Certificate Expiry Warning', 'Email', 'Certificate', 'Certificate Expiry Notice - {{CertificateNumber}}',
	 '<p>Your certificate {{CertificateNumber}} will expire on {{ExpiryDate}}.</p>',
	 'Your certificate {{CertificateNumber}} will expire on {{ExpiryDate}}.',
	 N'["CertificateNumber","ExpiryDate","CompanyName"]', 1, GETDATE(), GETDATE(), NULL, NULL);

	SET IDENTITY_INSERT [dbo].[NotificationTemplates] OFF;
END

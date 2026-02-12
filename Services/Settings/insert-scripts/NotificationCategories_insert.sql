-- Insert sample data for NotificationCategories table
-- This script creates categories for different types of notifications

SET IDENTITY_INSERT [dbo].[NotificationCategories] ON;

INSERT INTO [dbo].[NotificationCategories] 
([CategoryId], [CategoryName], [CategoryCode], [Description], [Icon], [Color], [Priority], [DisplayOrder], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Audit Reminder', 'audit_reminder', 'Reminders about upcoming audits and deadlines', 'calendar', '#2196F3', 3, 1, 1, GETDATE(), GETDATE(), 1, 1),
(2, 'Certificate Expiry', 'certificate_expiry', 'Notifications about certificate expiration dates', 'certificate', '#FF9800', 2, 2, 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Finding Response', 'finding_response', 'Notifications about finding responses and status updates', 'exclamation-circle', '#F44336', 2, 3, 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Invoice Payment', 'invoice_payment', 'Payment reminders and invoice status updates', 'credit-card', '#4CAF50', 3, 4, 1, GETDATE(), GETDATE(), 1, 1),
(5, 'System Alert', 'system_alert', 'System maintenance and important announcements', 'bell', '#9C27B0', 1, 5, 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Document Update', 'document_update', 'Notifications about document and procedure updates', 'file-text', '#607D8B', 4, 6, 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Training Reminder', 'training_reminder', 'Training requirements and completion reminders', 'graduation-cap', '#00BCD4', 3, 7, 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Schedule Change', 'schedule_change', 'Audit schedule changes and updates', 'clock', '#FF5722', 2, 8, 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Approval Request', 'approval_request', 'Requests for approvals and authorizations', 'check-circle', '#8BC34A', 3, 9, 1, GETDATE(), GETDATE(), 1, 1),
(10, 'Compliance Alert', 'compliance_alert', 'Regulatory compliance and deadline alerts', 'shield', '#E91E63', 2, 10, 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[NotificationCategories] OFF;

-- Verify the insert
SELECT * FROM [dbo].[NotificationCategories] ORDER BY Priority DESC, CategoryName;
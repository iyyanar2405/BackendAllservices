-- Insert sample data for NotificationCategories table
-- This script creates categories for different types of notifications

SET IDENTITY_INSERT [dbo].[NotificationCategories] ON;

INSERT INTO [dbo].[NotificationCategories] 
([CategoryId], [CategoryName], [CategoryDescription], [Icon], [Color], [Priority], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Audit Reminder', 'Reminders about upcoming audits and deadlines', 'calendar', '#2196F3', 'Medium', 1, GETDATE(), GETDATE(), 1, 1),
(2, 'Certificate Expiry', 'Notifications about certificate expiration dates', 'certificate', '#FF9800', 'High', 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Finding Response', 'Notifications about finding responses and status updates', 'exclamation-circle', '#F44336', 'High', 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Invoice Payment', 'Payment reminders and invoice status updates', 'credit-card', '#4CAF50', 'Medium', 1, GETDATE(), GETDATE(), 1, 1),
(5, 'System Alert', 'System maintenance and important announcements', 'bell', '#9C27B0', 'Critical', 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Document Update', 'Notifications about document and procedure updates', 'file-text', '#607D8B', 'Low', 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Training Reminder', 'Training requirements and completion reminders', 'graduation-cap', '#00BCD4', 'Medium', 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Schedule Change', 'Audit schedule changes and updates', 'clock', '#FF5722', 'High', 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Approval Request', 'Requests for approvals and authorizations', 'check-circle', '#8BC34A', 'Medium', 1, GETDATE(), GETDATE(), 1, 1),
(10, 'Compliance Alert', 'Regulatory compliance and deadline alerts', 'shield', '#E91E63', 'High', 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[NotificationCategories] OFF;

-- Verify the insert
SELECT * FROM [dbo].[NotificationCategories] ORDER BY Priority DESC, CategoryName;
-- Insert sample data for AuditLogs table
-- This script creates lightweight audit trail entries

INSERT INTO [dbo].[AuditLogs]
([UserId], [UserName], [Action], [EntityType], [EntityId], [EntityName], [OldValues], [NewValues], [ChangedFields], [ActionDate], [IPAddress], [UserAgent], [SessionId], [RequestUrl], [RequestMethod], [Reason], [Status], [Duration], [ApplicationName], [Environment], [CorrelationId], [AdditionalData], [CompanyId], [SiteId])
VALUES
(1, 'System Administrator', 'Login', 'User', 1, 'admin', NULL, '{"LastLoginDate":"2025-01-02T08:30:00Z"}', 'LastLoginDate', '2025-01-02 08:30:00', '192.168.1.10', 'Mozilla/5.0', 'sess_001_20250102', '/api/auth/login', 'POST', 'Successful admin login', 'Success', 245, 'CustomerPortal', 'Production', 'corr_001', NULL, NULL, NULL),
(4, 'Michael Brown', 'Create', 'Finding', 1, 'ACM-2024-001', NULL, '{"Status":"Open"}', 'Status', '2024-03-16 10:15:00', '192.168.1.11', 'Mozilla/5.0', 'sess_004_20240316', '/api/findings', 'POST', 'Finding raised during audit', 'Success', 310, 'CustomerPortal', 'Production', 'corr_002', NULL, 1, 1),
(21, 'John Doe', 'Update', 'Notification', 5, 'Invoice Payment Reminder', '{"IsRead":false}', '{"IsRead":true}', 'IsRead', '2025-06-20 14:00:00', '203.0.113.25', 'Mozilla/5.0', 'sess_021_20250620', '/api/notifications/5', 'PUT', 'Marked as read', 'Success', 120, 'CustomerPortal', 'Production', 'corr_003', NULL, 1, 1),
(18, 'Robert Lee', 'Update', 'Invoice', 4, 'DNV-INV-2025-002', '{"Status":"Pending"}', '{"Status":"Paid"}', 'Status', '2025-06-10 12:30:00', '192.168.1.30', 'Mozilla/5.0', 'sess_018_20250610', '/api/invoices/4', 'PUT', 'Payment received', 'Success', 420, 'CustomerPortal', 'Production', 'corr_004', NULL, 1, 1),
(12, 'Kevin Lee', 'Export', 'Audit', 7, 'TFL-2024-001', NULL, '{"Format":"PDF"}', 'Format', '2025-01-10 09:30:00', '203.0.113.25', 'Mozilla/5.0', 'sess_012_20250110', '/api/audits/7/export', 'GET', 'Audit report exported', 'Success', 860, 'CustomerPortal', 'Production', 'corr_005', NULL, 2, 4);

-- Verify the insert
SELECT COUNT(*) as TotalAuditLogs FROM [dbo].[AuditLogs];
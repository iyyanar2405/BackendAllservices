-- Insert sample data for AuditLogs table
-- This script creates comprehensive audit trail records for system activities

SET IDENTITY_INSERT [dbo].[AuditLogs] ON;

INSERT INTO [dbo].[AuditLogs] 
([AuditLogId], [TableName], [Operation], [RecordId], [OldValues], [NewValues], [ChangedFields], [UserId], [UserName], [IPAddress], [UserAgent], [SessionId], [Timestamp], [ReasonCode], [Comments])
VALUES
-- User Login/Logout Activities
(1, 'Users', 'LOGIN', 1, NULL, '{"LastLoginDate": "2025-01-02T08:30:00Z", "LoginCount": 156}', 'LastLoginDate,LoginCount', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250102', '2025-01-02 08:30:00', 'USER_LOGIN', 'Successful admin login'),

(2, 'Users', 'LOGIN', 21, NULL, '{"LastLoginDate": "2025-01-02T09:15:00Z", "LoginCount": 45}', 'LastLoginDate,LoginCount', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20250102', '2025-01-02 09:15:00', 'USER_LOGIN', 'Customer portal access - Acme Corporation'),

(3, 'Users', 'LOGIN', 5, NULL, '{"LastLoginDate": "2025-01-02T07:45:00Z", "LoginCount": 89}', 'LastLoginDate,LoginCount', 5, 'Lars Andersen', '192.168.1.15', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_005_20250102', '2025-01-02 07:45:00', 'USER_LOGIN', 'Lead auditor mobile access'),

-- Certificate Management Activities
(4, 'Certificates', 'INSERT', 1, NULL, '{"CertificateNumber": "DNV-QMS-001-2024", "CompanyId": 2, "ServiceId": 1, "Status": "Active"}', 'CertificateNumber,CompanyId,ServiceId,Status', 17, 'Anna Petersen', '192.168.1.20', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_017_20240320', '2024-03-20 14:30:00', 'CERT_ISSUED', 'ISO 9001:2015 certificate issued to Acme Corporation'),

(5, 'Certificates', 'UPDATE', 1, '{"Status": "Pending", "ApprovalDate": null}', '{"Status": "Active", "ApprovalDate": "2024-03-20T14:30:00Z"}', 'Status,ApprovalDate', 2, 'Erik Johansen', '192.168.1.12', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_002_20240320', '2024-03-20 14:35:00', 'CERT_APPROVED', 'Certificate approved by Regional Manager'),

(6, 'Certificates', 'UPDATE', 15, '{"Status": "Active"}', '{"Status": "Suspended"}', 'Status', 2, 'Erik Johansen', '192.168.1.12', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_002_20250102', '2025-01-02 10:20:00', 'CERT_SUSPENDED', 'Certificate suspended due to major nonconformity'),

-- Finding Management Activities
(7, 'Findings', 'INSERT', 15, NULL, '{"FindingNumber": "ACM-2025-001", "AuditId": 3, "CategoryId": 2, "Status": "Open"}', 'FindingNumber,AuditId,CategoryId,Status', 7, 'James Wilson', '192.168.1.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_007_20250615', '2025-06-15 11:30:00', 'FINDING_RAISED', 'Management review frequency finding raised'),

(8, 'Findings', 'UPDATE', 15, '{"Status": "Open", "ResponseDate": null}', '{"Status": "Responded", "ResponseDate": "2025-06-20T14:00:00Z"}', 'Status,ResponseDate', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20250620', '2025-06-20 14:00:00', 'FINDING_RESPONDED', 'Customer submitted corrective action response'),

(9, 'Findings', 'UPDATE', 10, '{"Status": "Responded"}', '{"Status": "Verified"}', 'Status', 9, 'Maria Eriksson', '192.168.1.18', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_009_20250425', '2025-04-25 16:15:00', 'FINDING_VERIFIED', 'Finding verification completed successfully'),

-- Invoice and Payment Activities
(10, 'Invoices', 'INSERT', 4, NULL, '{"InvoiceNumber": "DNV-INV-2025-002", "CompanyId": 2, "Amount": 9775.00, "Status": "Sent"}', 'InvoiceNumber,CompanyId,Amount,Status', 18, 'Robert Lee', '192.168.1.30', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_018_20250518', '2025-05-18 10:00:00', 'INVOICE_CREATED', 'Surveillance audit invoice generated'),

(11, 'Invoices', 'UPDATE', 4, '{"Status": "Sent", "PaymentDate": null}', '{"Status": "Paid", "PaymentDate": "2025-06-10T12:30:00Z"}', 'Status,PaymentDate', 18, 'Robert Lee', '192.168.1.30', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_018_20250610', '2025-06-10 12:30:00', 'PAYMENT_RECEIVED', 'Payment received from Acme Corporation'),

(12, 'Invoices', 'UPDATE', 14, '{"Status": "Sent", "DueDate": "2025-07-01"}', '{"Status": "Overdue", "DueDate": "2025-07-01"}', 'Status', 18, 'Robert Lee', '192.168.1.30', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_018_20250706', '2025-07-06 00:01:00', 'INVOICE_OVERDUE', 'Automatic status update - invoice overdue'),

-- Audit Scheduling Activities
(13, 'AuditSiteAudits', 'INSERT', 4, NULL, '{"AuditId": 4, "SiteId": 2, "ScheduledStartDate": "2025-05-15", "ScheduledEndDate": "2025-05-17"}', 'AuditId,SiteId,ScheduledStartDate,ScheduledEndDate', 16, 'David Kim', '192.168.1.35', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_016_20250415', '2025-04-15 14:20:00', 'AUDIT_SCHEDULED', 'Surveillance audit scheduled for Acme Corp'),

(14, 'AuditSiteAudits', 'UPDATE', 13, '{"ScheduledStartDate": "2025-06-02", "ScheduledEndDate": "2025-06-04"}', '{"ScheduledStartDate": "2025-06-09", "ScheduledEndDate": "2025-06-11"}', 'ScheduledStartDate,ScheduledEndDate', 16, 'David Kim', '192.168.1.35', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_016_20250528', '2025-05-28 09:30:00', 'AUDIT_RESCHEDULED', 'Audit rescheduled due to weather conditions'),

-- User Management Activities
(15, 'Users', 'INSERT', 30, NULL, '{"FirstName": "Hiroshi", "LastName": "Tanaka", "Email": "h.tanaka@autoinnovation.jp", "CompanyId": 11}', 'FirstName,LastName,Email,CompanyId', 2, 'Erik Johansen', '192.168.1.12', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_002_20240305', '2024-03-05 11:15:00', 'USER_CREATED', 'New user account created for Auto Innovation Inc'),

(16, 'Users', 'UPDATE', 30, '{"IsActive": true}', '{"IsActive": false}', 'IsActive', 2, 'Erik Johansen', '192.168.1.12', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_002_20250102', '2025-01-02 16:45:00', 'USER_DEACTIVATED', 'User account temporarily deactivated'),

-- Security and Access Activities
(17, 'UserRoles', 'INSERT', 45, NULL, '{"UserId": 15, "RoleId": 3}', 'UserId,RoleId', 2, 'Erik Johansen', '192.168.1.12', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_002_20240305', '2024-03-05 14:30:00', 'ROLE_ASSIGNED', 'Junior Auditor role assigned to new employee'),

(18, 'UserPreferences', 'UPDATE', 42, '{"EmailFrequency": "immediate"}', '{"EmailFrequency": "daily"}', 'EmailFrequency', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20240815', '2024-08-15 10:20:00', 'PREF_CHANGED', 'Email notification frequency updated'),

-- Failed Login Attempts
(19, 'Users', 'LOGIN_FAILED', 23, NULL, '{"FailedAttempts": 3, "LastFailedLogin": "2025-01-02T09:45:00Z"}', 'FailedAttempts,LastFailedLogin', NULL, 'sarah.thompson@techflow.co.uk', '198.51.100.42', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', NULL, '2025-01-02 09:45:00', 'LOGIN_FAILED', 'Failed login attempt - incorrect password'),

(20, 'Users', 'ACCOUNT_LOCKED', 23, '{"IsLocked": false}', '{"IsLocked": true, "LockDate": "2025-01-02T09:50:00Z"}', 'IsLocked,LockDate', NULL, 'sarah.thompson@techflow.co.uk', '198.51.100.42', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', NULL, '2025-01-02 09:50:00', 'ACCOUNT_LOCKED', 'Account locked after multiple failed login attempts'),

-- Notification Activities
(21, 'Notifications', 'INSERT', 25, NULL, '{"UserId": 4, "CategoryId": 1, "Title": "Multi-Site Audit Coordination", "IsRead": false}', 'UserId,CategoryId,Title,IsRead', 16, 'David Kim', '192.168.1.35', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_016_20250420', '2025-04-20 13:45:00', 'NOTIFICATION_SENT', 'Multi-site audit coordination notification'),

(22, 'Notifications', 'UPDATE', 8, '{"IsRead": false, "ReadDate": null}', '{"IsRead": true, "ReadDate": "2025-09-03T14:30:00Z"}', 'IsRead,ReadDate', 23, 'Sarah Thompson', '198.51.100.35', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_023_20250903', '2025-09-03 14:30:00', 'NOTIFICATION_READ', 'Finding response notification marked as read'),

-- Training Activities
(23, 'UserTrainings', 'INSERT', 35, NULL, '{"UserId": 21, "TrainingId": 28, "Status": "In Progress", "EnrollmentDate": "2024-09-01"}', 'UserId,TrainingId,Status,EnrollmentDate', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20240901', '2024-09-01 08:00:00', 'TRAINING_ENROLLED', 'Enrolled in annual quality system updates'),

(24, 'UserTrainings', 'UPDATE', 35, '{"Status": "In Progress", "CompletionDate": null}', '{"Status": "Completed", "CompletionDate": "2024-09-15T16:00:00Z", "Score": 87}', 'Status,CompletionDate,Score', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20240915', '2024-09-15 16:00:00', 'TRAINING_COMPLETED', 'Training completed successfully'),

-- Data Export Activities
(25, 'AuditReports', 'EXPORT', NULL, NULL, '{"ReportType": "AuditSummary", "Format": "PDF", "AuditId": 3}', 'ReportType,Format,AuditId', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20250620', '2025-06-20 15:30:00', 'REPORT_EXPORTED', 'Audit summary report exported to PDF'),

(26, 'CertificateData', 'EXPORT', NULL, NULL, '{"ReportType": "CertificateList", "Format": "Excel", "CompanyId": 2}', 'ReportType,Format,CompanyId', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20250102', '2025-01-02 11:45:00', 'DATA_EXPORTED', 'Certificate data exported for compliance review'),

-- System Configuration Changes
(27, 'SystemSettings', 'UPDATE', 1, '{"MaintenanceMode": false}', '{"MaintenanceMode": true, "MaintenanceStart": "2025-09-25T02:00:00Z"}', 'MaintenanceMode,MaintenanceStart', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250918', '2025-09-18 16:00:00', 'SYSTEM_CONFIG', 'Maintenance mode scheduled for system updates'),

(28, 'SystemSettings', 'UPDATE', 1, '{"MaintenanceMode": true}', '{"MaintenanceMode": false, "MaintenanceEnd": "2025-09-25T06:00:00Z"}', 'MaintenanceMode,MaintenanceEnd', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250925', '2025-09-25 06:00:00', 'SYSTEM_CONFIG', 'Maintenance completed - system back online'),

-- Bulk Operations
(29, 'Notifications', 'BULK_INSERT', NULL, NULL, '{"NotificationCount": 50, "CategoryId": 5, "Recipients": "All Users"}', 'NotificationCount,CategoryId,Recipients', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250918', '2025-09-18 17:00:00', 'BULK_NOTIFICATION', 'System maintenance notifications sent to all users'),

(30, 'UserPreferences', 'BULK_UPDATE', NULL, '{"DefaultLanguage": "en-US"}', '{"DefaultLanguage": "en-GB"}', 'DefaultLanguage', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250310', '2025-03-10 09:00:00', 'BULK_UPDATE', 'Language preferences updated for UK users'),

-- API Access Logs
(31, 'APIAccess', 'GET', NULL, NULL, '{"Endpoint": "/api/certificates", "ResponseCode": 200, "ResponseTime": 245}', 'Endpoint,ResponseCode,ResponseTime', 21, 'Michael Johnson', '203.0.113.25', 'DNV-Mobile-App/2.1.0', 'api_sess_021_20250102', '2025-01-02 09:30:00', 'API_ACCESS', 'Mobile app certificate data retrieval'),

(32, 'APIAccess', 'POST', NULL, NULL, '{"Endpoint": "/api/findings/response", "ResponseCode": 201, "DataSize": 1024}', 'Endpoint,ResponseCode,DataSize', 23, 'Sarah Thompson', '198.51.100.35', 'DNV-Portal-Integration/1.5.0', 'api_sess_023_20250903', '2025-09-03 14:00:00', 'API_ACCESS', 'Finding response submitted via API'),

-- Document Management
(33, 'Documents', 'UPLOAD', 156, NULL, '{"FileName": "QualityManual_v3.2.pdf", "FileSize": 2048576, "DocumentType": "Quality Manual"}', 'FileName,FileSize,DocumentType', 22, 'Lisa Chen', '203.0.113.28', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_022_20250910', '2025-09-10 14:15:00', 'DOCUMENT_UPLOAD', 'Updated quality manual uploaded'),

(34, 'Documents', 'DOWNLOAD', 156, NULL, '{"FileName": "QualityManual_v3.2.pdf", "DownloadSize": 2048576}', 'FileName,DownloadSize', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20250912', '2025-09-12 10:30:00', 'DOCUMENT_DOWNLOAD', 'Quality manual downloaded for review'),

-- Compliance and Regulatory Activities
(35, 'ComplianceChecks', 'RUN', NULL, NULL, '{"CheckType": "GDPR", "RecordsChecked": 250, "ViolationsFound": 0}', 'CheckType,RecordsChecked,ViolationsFound', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250915', '2025-09-15 02:00:00', 'COMPLIANCE_CHECK', 'Automated GDPR compliance check completed'),

-- Session Management
(36, 'UserSessions', 'LOGOUT', NULL, '{"SessionDuration": 480, "PagesVisited": 15}', '{"SessionEnd": "2025-01-02T17:30:00Z"}', 'SessionEnd', 21, 'Michael Johnson', '203.0.113.25', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_021_20250102', '2025-01-02 17:30:00', 'USER_LOGOUT', 'User logged out normally'),

(37, 'UserSessions', 'TIMEOUT', NULL, '{"SessionDuration": 480}', '{"SessionEnd": "2025-01-02T16:45:00Z", "TimeoutReason": "Inactivity"}', 'SessionEnd,TimeoutReason', 24, 'James Smith', '198.51.100.40', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_024_20250102', '2025-01-02 16:45:00', 'SESSION_TIMEOUT', 'Session timed out due to inactivity'),

-- Data Integrity Checks
(38, 'DataIntegrity', 'CHECK', NULL, NULL, '{"TablesChecked": 25, "RecordsValidated": 15000, "ErrorsFound": 0}', 'TablesChecked,RecordsValidated,ErrorsFound', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250101', '2025-01-01 03:00:00', 'DATA_INTEGRITY', 'Daily data integrity check completed successfully'),

-- Performance Monitoring
(39, 'Performance', 'MONITOR', NULL, NULL, '{"AvgResponseTime": 150, "PeakUsers": 45, "SystemLoad": 65}', 'AvgResponseTime,PeakUsers,SystemLoad', 1, 'John Anderson', '192.168.1.10', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_001_20250102', '2025-01-02 12:00:00', 'PERFORMANCE_LOG', 'Hourly system performance monitoring'),

-- Emergency Access
(40, 'Users', 'EMERGENCY_ACCESS', 21, NULL, '{"EmergencyReason": "Critical finding response required", "AuthorizedBy": "Erik Johansen"}', 'EmergencyReason,AuthorizedBy', 2, 'Erik Johansen', '192.168.1.12', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36', 'sess_002_20250620', '2025-06-20 18:30:00', 'EMERGENCY_ACCESS', 'Emergency access granted for critical finding response');

SET IDENTITY_INSERT [dbo].[AuditLogs] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalAuditLogs FROM [dbo].[AuditLogs];

-- Show audit log statistics by operation
SELECT 
    Operation,
    COUNT(*) as LogCount,
    COUNT(DISTINCT UserId) as UniqueUsers,
    MIN(Timestamp) as FirstOccurrence,
    MAX(Timestamp) as LastOccurrence
FROM [dbo].[AuditLogs] 
GROUP BY Operation
ORDER BY LogCount DESC;

-- Show audit log statistics by table
SELECT 
    TableName,
    COUNT(*) as LogCount,
    COUNT(DISTINCT Operation) as UniqueOperations,
    COUNT(DISTINCT UserId) as UniqueUsers
FROM [dbo].[AuditLogs] 
WHERE TableName IS NOT NULL
GROUP BY TableName
ORDER BY LogCount DESC;

-- Show recent activity (last 30 days)
SELECT 
    CAST(Timestamp as DATE) as Date,
    COUNT(*) as ActivityCount,
    COUNT(DISTINCT UserId) as ActiveUsers,
    COUNT(DISTINCT Operation) as UniqueOperations
FROM [dbo].[AuditLogs] 
WHERE Timestamp >= DATEADD(day, -30, GETDATE())
GROUP BY CAST(Timestamp as DATE)
ORDER BY Date DESC;

-- Show user activity summary
SELECT 
    UserName,
    COUNT(*) as TotalActions,
    COUNT(DISTINCT Operation) as UniqueOperations,
    COUNT(DISTINCT TableName) as TablesAccessed,
    MIN(Timestamp) as FirstActivity,
    MAX(Timestamp) as LastActivity
FROM [dbo].[AuditLogs] 
WHERE UserId IS NOT NULL
GROUP BY UserId, UserName
ORDER BY TotalActions DESC;

-- Show security-related events
SELECT 
    Operation,
    UserName,
    IPAddress,
    Timestamp,
    Comments,
    CASE 
        WHEN Operation LIKE '%FAILED%' THEN 'Failed Action'
        WHEN Operation LIKE '%LOCKED%' THEN 'Security Lock'
        WHEN Operation LIKE 'EMERGENCY%' THEN 'Emergency Access'
        ELSE 'Security Event'
    END as SecurityEventType
FROM [dbo].[AuditLogs] 
WHERE Operation IN ('LOGIN_FAILED', 'ACCOUNT_LOCKED', 'EMERGENCY_ACCESS', 'UNAUTHORIZED_ACCESS')
   OR ReasonCode IN ('SECURITY_VIOLATION', 'EMERGENCY_ACCESS', 'LOGIN_FAILED')
ORDER BY Timestamp DESC;

-- Show data modification summary
SELECT 
    TableName,
    Operation,
    COUNT(*) as ModificationCount,
    COUNT(DISTINCT UserId) as UniqueModifiers,
    MAX(Timestamp) as LastModified
FROM [dbo].[AuditLogs] 
WHERE Operation IN ('INSERT', 'UPDATE', 'DELETE')
GROUP BY TableName, Operation
ORDER BY TableName, ModificationCount DESC;
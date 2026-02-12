-- Insert sample data for Notifications table
-- This script creates various types of notifications for users

SET IDENTITY_INSERT [dbo].[Notifications] ON;

INSERT INTO [dbo].[Notifications] 
([NotificationId], [UserId], [CategoryId], [Title], [Message], [ActionUrl], [IsRead], [Priority], [ExpiryDate], [CreatedDate], [ReadDate], [CreatedBy])
VALUES
-- Certificate Expiry Notifications
(1, 21, 2, 'Certificate Expiring Soon', 'Your ISO 9001:2015 certificate (DNV-QMS-001-2024) will expire on March 20, 2027. Please schedule your recertification audit.', '/certificates/1', 0, 'High', '2027-03-20', '2025-09-15', NULL, 17),
(2, 23, 2, 'Certificate Renewal Required', 'ISO 27001:2013 certificate (DNV-ISMS-001-2024) expires in 18 months. Contact your account manager to schedule recertification.', '/certificates/4', 1, 'High', '2027-01-20', '2025-09-10', '2025-09-12', 17),
(3, 25, 2, 'Wind Turbine Certificate Expiry', 'Wind Turbine Type Certificate (DNV-WTC-001-2024) expires on June 8, 2029. Annual surveillance due.', '/certificates/6', 0, 'Medium', '2029-06-08', '2025-09-14', NULL, 17),

-- Audit Reminder Notifications
(4, 21, 1, 'Upcoming Surveillance Audit', 'ISO 14001:2015 surveillance audit scheduled for May 15-17, 2025. Please prepare required documentation.', '/audits/4', 0, 'Medium', '2025-05-17', '2025-04-15', NULL, 7),
(5, 23, 1, 'Audit Preparation Required', 'ISO 9001:2015 surveillance audit scheduled for September 1-3, 2025. Site preparation checklist attached.', '/audits/8', 1, 'Medium', '2025-09-03', '2025-08-15', '2025-08-16', 12),
(6, 25, 1, 'Pre-Assessment Scheduled', 'ISO 50001:2018 pre-assessment scheduled for October 14-15, 2025. Technical documentation review required.', '/audits/15', 0, 'Medium', '2025-10-15', '2025-09-14', NULL, 8),

-- Finding Response Notifications
(7, 21, 3, 'Finding Response Overdue', 'Response to finding ACM-2025-001 (Management Review Frequency) is overdue. Please submit corrective action plan.', '/findings/15', 0, 'High', NULL, '2025-06-17', NULL, 7),
(8, 23, 3, 'New Finding Raised', 'New observation TFL-2025-001 raised during surveillance audit. Response due October 2, 2025.', '/findings/16', 1, 'Medium', '2025-10-02', '2025-09-02', '2025-09-03', 12),
(9, 27, 3, 'Critical Finding Alert', 'CRITICAL: Finding MAR-2025-001 requires immediate attention. Life saving equipment inspection overdue.', '/findings/19', 0, 'Critical', '2025-04-15', '2025-04-08', NULL, 14),

-- Invoice Payment Notifications
(10, 21, 4, 'Invoice Payment Reminder', 'Invoice DNV-INV-2025-002 (€9,775.00) is due on June 17, 2025. Please arrange payment to avoid late fees.', '/invoices/4', 0, 'Medium', '2025-06-17', '2025-06-03', NULL, 18),
(11, 25, 4, 'Payment Overdue', 'Invoice DNV-INV-2025-006 (€17,850.00) is overdue. Please contact our finance team immediately.', '/invoices/14', 0, 'High', NULL, '2025-07-06', NULL, 18),
(12, 17, 4, 'Payment Confirmation', 'Payment received for invoice DNV-INV-2024-016 (Global Manufacturing - $92,000.00). Thank you for your prompt payment.', '/invoices/29', 1, 'Low', NULL, '2024-09-15', '2024-09-16', 18),

-- System Alert Notifications
(13, 1, 5, 'System Maintenance Scheduled', 'Customer portal will be unavailable on September 25, 2025 from 02:00-06:00 UTC for scheduled maintenance.', '/maintenance', 1, 'Critical', '2025-09-25', '2025-09-18', '2025-09-18', 1),
(14, 2, 5, 'New Feature Release', 'New dashboard analytics features are now available. View enhanced reporting capabilities in your dashboard.', '/dashboard', 0, 'Low', NULL, '2025-09-15', NULL, 1),

-- Document Update Notifications
(15, 21, 6, 'Quality Manual Updated', 'Quality manual v3.2 has been approved and is now available. Please review the updated sections.', '/documents/quality-manual', 0, 'Low', NULL, '2025-09-10', NULL, 22),
(16, 23, 6, 'GDPR Policy Revision', 'Data protection policy has been updated to reflect new regulatory requirements. Review required by all staff.', '/documents/gdpr-policy', 1, 'Medium', '2025-10-01', '2025-09-05', '2025-09-07', 24),

-- Training Reminder Notifications
(17, 22, 7, 'ISO 45001 Training Due', 'Annual refresher training for ISO 45001:2018 requirements is due by October 15, 2025.', '/training/iso45001', 0, 'Medium', '2025-10-15', '2025-09-15', NULL, 20),
(18, 24, 7, 'Cybersecurity Training Required', 'Mandatory cybersecurity awareness training must be completed by September 30, 2025.', '/training/cybersecurity', 1, 'High', '2025-09-30', '2025-09-01', '2025-09-02', 20),

-- Schedule Change Notifications
(19, 25, 8, 'Audit Rescheduled', 'Your wind turbine surveillance audit has been rescheduled from June 2-4 to June 9-11, 2025 due to weather conditions.', '/audits/13', 0, 'High', NULL, '2025-05-28', NULL, 16),
(20, 27, 8, 'Auditor Assignment Change', 'Lead auditor for your DNV Ships surveillance has been changed to Maria Eriksson due to scheduling conflict.', '/audits/18', 1, 'Medium', NULL, '2025-03-20', '2025-03-21', 16),

-- Approval Request Notifications
(21, 2, 9, 'Extension Request Approval', 'Client has requested deadline extension for finding AUTO-2024-002. Approval required from Regional Manager.', '/approvals/finding-extension/17', 0, 'Medium', '2025-01-10', '2025-01-05', NULL, 5),
(22, 17, 9, 'Certificate Suspension Review', 'Certificate suspension request for Acme Corporation ISO 45001 requires certification manager approval.', '/approvals/certificate-suspension/23', 1, 'High', NULL, '2025-01-16', '2025-01-17', 8),

-- Compliance Alert Notifications
(23, 25, 10, 'Environmental Permit Renewal', 'Environmental operating permit for Wind Farm 1 expires December 31, 2025. Renewal application required by November 1.', '/compliance/permits', 0, 'High', '2025-12-31', '2025-09-15', NULL, 26),
(24, 29, 10, 'Food Safety Regulation Update', 'New EU food safety regulations effective January 1, 2026. Impact assessment required for BRC certification.', '/compliance/food-safety', 0, 'Medium', '2026-01-01', '2025-09-12', NULL, 30),

-- Additional Recent Notifications
(25, 4, 1, 'Multi-Site Audit Coordination', 'Global Manufacturing multi-site surveillance requires coordination between US, UK, and German teams.', '/audits/30', 0, 'Medium', '2025-05-09', '2025-04-20', NULL, 16),
(26, 9, 3, 'Finding Verification Complete', 'Finding FEC-2024-001 (Temperature Monitoring) has been verified and closed. Excellent corrective action implementation.', '/findings/10', 1, 'Low', NULL, '2025-04-25', '2025-04-26', 9),
(27, 12, 8, 'Weather Impact on Audit', 'Small Tech Solutions audit may be delayed due to severe weather forecast. Will confirm by Monday morning.', '/audits/31', 0, 'Medium', NULL, '2025-09-19', NULL, 16),

-- Overdue Finding Notifications for Different Users
(28, 5, 3, 'Finding Response Required', 'Response to finding AUTO-2024-002 (Supplier Quality Agreement) is past due. Immediate action required.', '/findings/17', 0, 'Critical', NULL, '2025-01-04', NULL, 5),
(29, 25, 3, 'Finding Under Dispute', 'Finding GES-2025-001 (Noise Level Monitoring) status changed to "Disputed". Awaiting technical review.', '/findings/18', 0, 'Medium', NULL, '2025-06-04', NULL, 13),

-- Invoice and Payment Notifications
(30, 19, 4, 'Payment Plan Available', 'Payment plan options are available for invoice DNV-INV-2024-014 (€39,040.00). Contact finance to discuss terms.', '/invoices/27', 0, 'Low', NULL, '2025-01-15', NULL, 18),
(31, 28, 4, 'Early Payment Discount', 'Pay invoice DNV-INV-2025-008 early and receive 2% discount. Offer valid until April 25, 2025.', '/invoices/19', 1, 'Low', '2025-04-25', '2025-04-15', '2025-04-16', 18),

-- Training and Competency Notifications
(32, 15, 7, 'Junior Auditor Development', 'Your junior auditor development program includes ISO 14001 training scheduled for October 2025.', '/training/junior-auditor', 0, 'Low', NULL, '2025-09-18', NULL, 2),
(33, 11, 7, 'Lead Auditor Qualification', 'Congratulations! You are now qualified as Lead Auditor for ISO 9001:2015. New assignments available.', '/training/lead-auditor', 1, 'Low', NULL, '2025-08-15', '2025-08-16', 2),

-- Customer Service and Support
(34, 21, 6, 'Customer Satisfaction Survey', 'Please complete our audit satisfaction survey for your recent ISO 9001:2015 surveillance audit.', '/surveys/audit-satisfaction', 0, 'Low', '2025-04-12', '2025-03-20', NULL, 19),
(35, 23, 6, 'Account Manager Assignment', 'Sarah Thompson has been assigned as your new account manager. She will contact you within 2 business days.', '/contacts/account-manager', 1, 'Low', NULL, '2025-09-01', '2025-09-02', 19);

SET IDENTITY_INSERT [dbo].[Notifications] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalNotifications FROM [dbo].[Notifications];

-- Show notification statistics by category
SELECT 
    nc.CategoryName,
    COUNT(n.NotificationId) as TotalNotifications,
    COUNT(CASE WHEN n.IsRead = 0 THEN 1 END) as UnreadCount,
    COUNT(CASE WHEN n.IsRead = 1 THEN 1 END) as ReadCount
FROM [dbo].[NotificationCategories] nc
LEFT JOIN [dbo].[Notifications] n ON nc.CategoryId = n.CategoryId
GROUP BY nc.CategoryId, nc.CategoryName
ORDER BY TotalNotifications DESC;

-- Show notifications by priority
SELECT 
    Priority,
    COUNT(*) as Count,
    COUNT(CASE WHEN IsRead = 0 THEN 1 END) as UnreadCount
FROM [dbo].[Notifications] 
GROUP BY Priority
ORDER BY 
    CASE Priority 
        WHEN 'Critical' THEN 1 
        WHEN 'High' THEN 2 
        WHEN 'Medium' THEN 3 
        WHEN 'Low' THEN 4 
    END;

-- Show recent unread notifications by user
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    c.CompanyName,
    COUNT(n.NotificationId) as UnreadNotifications,
    COUNT(CASE WHEN n.Priority IN ('Critical', 'High') THEN 1 END) as HighPriorityUnread
FROM [dbo].[Users] u
LEFT JOIN [dbo].[Companies] comp ON u.CompanyId = comp.CompanyId
LEFT JOIN [dbo].[Notifications] n ON u.UserId = n.UserId AND n.IsRead = 0
GROUP BY u.UserId, u.FirstName, u.LastName, c.CompanyName
HAVING COUNT(n.NotificationId) > 0
ORDER BY HighPriorityUnread DESC, UnreadNotifications DESC;

-- Show overdue notifications (past expiry date)
SELECT 
    n.Title,
    u.FirstName + ' ' + u.LastName as UserName,
    nc.CategoryName,
    n.Priority,
    n.ExpiryDate,
    DATEDIFF(day, n.ExpiryDate, GETDATE()) as DaysOverdue
FROM [dbo].[Notifications] n
INNER JOIN [dbo].[Users] u ON n.UserId = u.UserId
INNER JOIN [dbo].[NotificationCategories] nc ON n.CategoryId = nc.CategoryId
WHERE n.ExpiryDate < GETDATE() AND n.IsRead = 0
ORDER BY DaysOverdue DESC;

-- Show notifications by creation date (recent activity)
SELECT 
    CAST(CreatedDate as DATE) as Date,
    COUNT(*) as NotificationCount,
    COUNT(CASE WHEN IsRead = 1 THEN 1 END) as ReadCount
FROM [dbo].[Notifications] 
WHERE CreatedDate >= DATEADD(day, -30, GETDATE())
GROUP BY CAST(CreatedDate as DATE)
ORDER BY Date DESC;
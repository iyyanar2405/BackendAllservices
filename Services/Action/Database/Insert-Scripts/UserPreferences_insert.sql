-- Insert sample data for UserPreferences table
-- This script creates user notification preferences and settings

SET IDENTITY_INSERT [dbo].[UserPreferences] ON;

INSERT INTO [dbo].[UserPreferences] 
([PreferenceId], [UserId], [PreferenceName], [PreferenceValue], [Category], [DataType], [IsEditable], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy])
VALUES
-- Email Notification Preferences for DNV Staff
(1, 1, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 1),
(2, 1, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 1),
(3, 1, 'CertificateExpiryEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 1),
(4, 1, 'SystemAlertEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 1),
(5, 1, 'EmailFrequency', 'immediate', 'Notifications', 'string', 1, '2024-01-15', '2024-01-15', 1, 1),
(6, 1, 'Language', 'en-US', 'General', 'string', 1, '2024-01-15', '2024-03-10', 1, 1),
(7, 1, 'TimeZone', 'Europe/Oslo', 'General', 'string', 1, '2024-01-15', '2024-01-15', 1, 1),
(8, 1, 'DateFormat', 'DD/MM/YYYY', 'General', 'string', 1, '2024-01-15', '2024-01-15', 1, 1),

-- Preferences for Erik Johansen (Regional Manager)
(9, 2, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 2),
(10, 2, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 2),
(11, 2, 'CertificateExpiryEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 2),
(12, 2, 'SystemAlertEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 2),
(13, 2, 'ApprovalRequestEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 2),
(14, 2, 'EmailFrequency', 'immediate', 'Notifications', 'string', 1, '2024-01-15', '2024-01-15', 1, 2),
(15, 2, 'Language', 'en-US', 'General', 'string', 1, '2024-01-15', '2024-01-15', 1, 2),
(16, 2, 'TimeZone', 'Europe/Oslo', 'General', 'string', 1, '2024-01-15', '2024-01-15', 1, 2),
(17, 2, 'DashboardView', 'manager', 'Display', 'string', 1, '2024-01-15', '2024-05-20', 1, 2),

-- Preferences for Lead Auditors (Lars Andersen, Maria Eriksson, Jean Dubois)
(18, 5, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-01-20', '2024-01-20', 1, 5),
(19, 5, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-20', '2024-01-20', 1, 5),
(20, 5, 'FindingResponseEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-20', '2024-01-20', 1, 5),
(21, 5, 'ScheduleChangeEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-20', '2024-01-20', 1, 5),
(22, 5, 'EmailFrequency', 'immediate', 'Notifications', 'string', 1, '2024-01-20', '2024-01-20', 1, 5),
(23, 5, 'Language', 'en-US', 'General', 'string', 1, '2024-01-20', '2024-01-20', 1, 5),
(24, 5, 'TimeZone', 'Europe/Copenhagen', 'General', 'string', 1, '2024-01-20', '2024-01-20', 1, 5),
(25, 5, 'MobileNotifications', 'true', 'Notifications', 'boolean', 1, '2024-01-20', '2024-01-20', 1, 5),

(26, 9, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-01-25', '2024-01-25', 1, 9),
(27, 9, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-25', '2024-01-25', 1, 9),
(28, 9, 'FindingResponseEmail', 'true', 'Notifications', 'boolean', 1, '2024-01-25', '2024-01-25', 1, 9),
(29, 9, 'Language', 'sv-SE', 'General', 'string', 1, '2024-01-25', '2024-01-25', 1, 9),
(30, 9, 'TimeZone', 'Europe/Stockholm', 'General', 'string', 1, '2024-01-25', '2024-01-25', 1, 9),
(31, 9, 'DateFormat', 'YYYY-MM-DD', 'General', 'string', 1, '2024-01-25', '2024-01-25', 1, 9),

(32, 12, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-01', '2024-02-01', 1, 12),
(33, 12, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-01', '2024-02-01', 1, 12),
(34, 12, 'Language', 'fr-FR', 'General', 'string', 1, '2024-02-01', '2024-02-01', 1, 12),
(35, 12, 'TimeZone', 'Europe/Paris', 'General', 'string', 1, '2024-02-01', '2024-02-01', 1, 12),
(36, 12, 'DateFormat', 'DD/MM/YYYY', 'General', 'string', 1, '2024-02-01', '2024-02-01', 1, 12),

-- Customer Preferences - Acme Corporation
(37, 21, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 21),
(38, 21, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 21),
(39, 21, 'CertificateExpiryEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 21),
(40, 21, 'FindingResponseEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 21),
(41, 21, 'InvoiceReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 21),
(42, 21, 'EmailFrequency', 'daily', 'Notifications', 'string', 1, '2024-02-10', '2024-08-15', 1, 21),
(43, 21, 'Language', 'en-US', 'General', 'string', 1, '2024-02-10', '2024-02-10', 1, 21),
(44, 21, 'TimeZone', 'America/New_York', 'General', 'string', 1, '2024-02-10', '2024-02-10', 1, 21),
(45, 21, 'DashboardView', 'client', 'Display', 'string', 1, '2024-02-10', '2024-02-10', 1, 21),

(46, 22, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 22),
(47, 22, 'AuditReminderEmail', 'false', 'Notifications', 'boolean', 1, '2024-02-10', '2024-06-01', 1, 22),
(48, 22, 'TrainingReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 22),
(49, 22, 'EmailFrequency', 'weekly', 'Notifications', 'string', 1, '2024-02-10', '2024-02-10', 1, 22),
(50, 22, 'Language', 'en-US', 'General', 'string', 1, '2024-02-10', '2024-02-10', 1, 22),
(51, 22, 'TimeZone', 'America/New_York', 'General', 'string', 1, '2024-02-10', '2024-02-10', 1, 22),

-- TechFlow Solutions Ltd Preferences
(52, 23, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-15', '2024-02-15', 1, 23),
(53, 23, 'CertificateExpiryEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-15', '2024-02-15', 1, 23),
(54, 23, 'SystemAlertEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-15', '2024-02-15', 1, 23),
(55, 23, 'DocumentUpdateEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-15', '2024-02-15', 1, 23),
(56, 23, 'EmailFrequency', 'immediate', 'Notifications', 'string', 1, '2024-02-15', '2024-02-15', 1, 23),
(57, 23, 'Language', 'en-GB', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 23),
(58, 23, 'TimeZone', 'Europe/London', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 23),
(59, 23, 'DateFormat', 'DD/MM/YYYY', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 23),

(60, 24, 'EmailNotifications', 'false', 'Notifications', 'boolean', 1, '2024-02-15', '2024-07-10', 1, 24),
(61, 24, 'TrainingReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-15', '2024-02-15', 1, 24),
(62, 24, 'Language', 'en-GB', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 24),
(63, 24, 'TimeZone', 'Europe/London', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 24),

-- Green Energy Solutions Preferences
(64, 25, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 25),
(65, 25, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 25),
(66, 25, 'CertificateExpiryEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 25),
(67, 25, 'ComplianceAlertEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 25),
(68, 25, 'EmailFrequency', 'immediate', 'Notifications', 'string', 1, '2024-02-20', '2024-02-20', 1, 25),
(69, 25, 'Language', 'de-DE', 'General', 'string', 1, '2024-02-20', '2024-02-20', 1, 25),
(70, 25, 'TimeZone', 'Europe/Berlin', 'General', 'string', 1, '2024-02-20', '2024-02-20', 1, 25),
(71, 25, 'DateFormat', 'DD.MM.YYYY', 'General', 'string', 1, '2024-02-20', '2024-02-20', 1, 25),

(72, 26, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 26),
(73, 26, 'ComplianceAlertEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 26),
(74, 26, 'Language', 'de-DE', 'General', 'string', 1, '2024-02-20', '2024-02-20', 1, 26),
(75, 26, 'TimeZone', 'Europe/Berlin', 'General', 'string', 1, '2024-02-20', '2024-02-20', 1, 26),

-- Maritime Excellence Ltd Preferences
(76, 27, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-25', '2024-02-25', 1, 27),
(77, 27, 'AuditReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-25', '2024-02-25', 1, 27),
(78, 27, 'FindingResponseEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-25', '2024-02-25', 1, 27),
(79, 27, 'ScheduleChangeEmail', 'true', 'Notifications', 'boolean', 1, '2024-02-25', '2024-02-25', 1, 27),
(80, 27, 'EmailFrequency', 'immediate', 'Notifications', 'string', 1, '2024-02-25', '2024-02-25', 1, 27),
(81, 27, 'Language', 'en-GB', 'General', 'string', 1, '2024-02-25', '2024-02-25', 1, 27),
(82, 27, 'TimeZone', 'Europe/London', 'General', 'string', 1, '2024-02-25', '2024-02-25', 1, 27),
(83, 27, 'MobileNotifications', 'true', 'Notifications', 'boolean', 1, '2024-02-25', '2024-02-25', 1, 27),

-- Food Excellence Corporation Preferences
(84, 28, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-03-01', '2024-03-01', 1, 28),
(85, 28, 'ComplianceAlertEmail', 'true', 'Notifications', 'boolean', 1, '2024-03-01', '2024-03-01', 1, 28),
(86, 28, 'InvoiceReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-03-01', '2024-03-01', 1, 28),
(87, 28, 'EmailFrequency', 'daily', 'Notifications', 'string', 1, '2024-03-01', '2024-03-01', 1, 28),
(88, 28, 'Language', 'it-IT', 'General', 'string', 1, '2024-03-01', '2024-03-01', 1, 28),
(89, 28, 'TimeZone', 'Europe/Rome', 'General', 'string', 1, '2024-03-01', '2024-03-01', 1, 28),
(90, 28, 'DateFormat', 'DD/MM/YYYY', 'General', 'string', 1, '2024-03-01', '2024-03-01', 1, 28),

(91, 29, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-03-01', '2024-03-01', 1, 29),
(92, 29, 'TrainingReminderEmail', 'true', 'Notifications', 'boolean', 1, '2024-03-01', '2024-03-01', 1, 29),
(93, 29, 'Language', 'it-IT', 'General', 'string', 1, '2024-03-01', '2024-03-01', 1, 29),
(94, 29, 'TimeZone', 'Europe/Rome', 'General', 'string', 1, '2024-03-01', '2024-03-01', 1, 29),

-- Auto Innovation Inc Preferences
(95, 30, 'EmailNotifications', 'true', 'Notifications', 'boolean', 1, '2024-03-05', '2024-03-05', 1, 30),
(96, 30, 'FindingResponseEmail', 'true', 'Notifications', 'boolean', 1, '2024-03-05', '2024-03-05', 1, 30),
(97, 30, 'ScheduleChangeEmail', 'false', 'Notifications', 'boolean', 1, '2024-03-05', '2024-09-01', 1, 30),
(98, 30, 'EmailFrequency', 'weekly', 'Notifications', 'string', 1, '2024-03-05', '2024-03-05', 1, 30),
(99, 30, 'Language', 'ja-JP', 'General', 'string', 1, '2024-03-05', '2024-03-05', 1, 30),
(100, 30, 'TimeZone', 'Asia/Tokyo', 'General', 'string', 1, '2024-03-05', '2024-03-05', 1, 30),

-- Additional Display and System Preferences
(101, 1, 'DashboardRefreshRate', '30', 'Display', 'integer', 1, '2024-01-15', '2024-01-15', 1, 1),
(102, 1, 'DefaultPageSize', '25', 'Display', 'integer', 1, '2024-01-15', '2024-01-15', 1, 1),
(103, 1, 'ShowQuickActions', 'true', 'Display', 'boolean', 1, '2024-01-15', '2024-01-15', 1, 1),

(104, 2, 'AutoSaveInterval', '300', 'System', 'integer', 1, '2024-01-15', '2024-01-15', 1, 2),
(105, 2, 'SessionTimeout', '480', 'System', 'integer', 1, '2024-01-15', '2024-01-15', 1, 2),

(106, 5, 'CalendarView', 'month', 'Display', 'string', 1, '2024-01-20', '2024-01-20', 1, 5),
(107, 5, 'WorkingHours', '08:00-17:00', 'General', 'string', 1, '2024-01-20', '2024-01-20', 1, 5),

(108, 21, 'NotificationSound', 'true', 'Notifications', 'boolean', 1, '2024-02-10', '2024-02-10', 1, 21),
(109, 21, 'PopupNotifications', 'false', 'Notifications', 'boolean', 1, '2024-02-10', '2024-06-15', 1, 21),

(110, 23, 'ExportFormat', 'PDF', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 23),
(111, 23, 'ReportDelivery', 'email', 'General', 'string', 1, '2024-02-15', '2024-02-15', 1, 23),

(112, 25, 'MobileSync', 'true', 'System', 'boolean', 1, '2024-02-20', '2024-02-20', 1, 25),
(113, 25, 'OfflineMode', 'enabled', 'System', 'string', 1, '2024-02-20', '2024-02-20', 1, 25),

-- Privacy and Security Preferences
(114, 21, 'DataRetentionPeriod', '7', 'Privacy', 'integer', 1, '2024-02-10', '2024-02-10', 1, 21),
(115, 23, 'ShareAnalytics', 'false', 'Privacy', 'boolean', 1, '2024-02-15', '2024-02-15', 1, 23),
(116, 25, 'CookieConsent', 'essential', 'Privacy', 'string', 1, '2024-02-20', '2024-02-20', 1, 25),

-- Two-Factor Authentication Preferences
(117, 1, 'TwoFactorAuth', 'enabled', 'Security', 'string', 0, '2024-01-15', '2024-01-15', 1, 1),
(118, 2, 'TwoFactorAuth', 'enabled', 'Security', 'string', 0, '2024-01-15', '2024-01-15', 1, 2),
(119, 5, 'TwoFactorAuth', 'enabled', 'Security', 'string', 0, '2024-01-20', '2024-01-20', 1, 5),
(120, 21, 'TwoFactorAuth', 'disabled', 'Security', 'string', 0, '2024-02-10', '2024-08-01', 1, 21);

SET IDENTITY_INSERT [dbo].[UserPreferences] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalPreferences FROM [dbo].[UserPreferences];

-- Show preferences by category
SELECT 
    Category,
    COUNT(*) as PreferenceCount,
    COUNT(DISTINCT UserId) as UsersWithPreferences
FROM [dbo].[UserPreferences] 
GROUP BY Category
ORDER BY PreferenceCount DESC;

-- Show notification preferences summary
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    c.CompanyName,
    COUNT(up.PreferenceId) as TotalPreferences,
    COUNT(CASE WHEN up.Category = 'Notifications' THEN 1 END) as NotificationPreferences,
    MAX(CASE WHEN up.PreferenceName = 'EmailNotifications' THEN up.PreferenceValue END) as EmailEnabled,
    MAX(CASE WHEN up.PreferenceName = 'EmailFrequency' THEN up.PreferenceValue END) as EmailFrequency
FROM [dbo].[Users] u
LEFT JOIN [dbo].[Companies] c ON u.CompanyId = c.CompanyId
LEFT JOIN [dbo].[UserPreferences] up ON u.UserId = up.UserId
GROUP BY u.UserId, u.FirstName, u.LastName, c.CompanyName
HAVING COUNT(up.PreferenceId) > 0
ORDER BY TotalPreferences DESC;

-- Show language and timezone distribution
SELECT 
    MAX(CASE WHEN up.PreferenceName = 'Language' THEN up.PreferenceValue END) as Language,
    MAX(CASE WHEN up.PreferenceName = 'TimeZone' THEN up.PreferenceValue END) as TimeZone,
    COUNT(DISTINCT up.UserId) as UserCount
FROM [dbo].[UserPreferences] up
WHERE up.PreferenceName IN ('Language', 'TimeZone')
GROUP BY up.UserId
HAVING MAX(CASE WHEN up.PreferenceName = 'Language' THEN up.PreferenceValue END) IS NOT NULL
ORDER BY Language, TimeZone;

-- Show users with customized email frequencies
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    c.CompanyName,
    up.PreferenceValue as EmailFrequency,
    up.UpdatedDate
FROM [dbo].[Users] u
INNER JOIN [dbo].[Companies] c ON u.CompanyId = c.CompanyId
INNER JOIN [dbo].[UserPreferences] up ON u.UserId = up.UserId
WHERE up.PreferenceName = 'EmailFrequency'
ORDER BY up.PreferenceValue, u.FirstName;
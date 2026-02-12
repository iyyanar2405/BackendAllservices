-- Insert sample data for AuditTeamMembers table
-- This script assigns lead auditors to audits

INSERT INTO [dbo].[AuditTeamMembers]
([AuditId], [UserId], [Role], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [AssignedDate], [StartDate], [EndDate], [Specialization], [Notes])
VALUES
(1, 4, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-02-15', '2024-03-15', '2024-03-19', 'Quality', NULL),
(2, 7, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-02-10', '2025-03-10', '2025-03-12', 'Quality', NULL),
(3, 4, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-04-20', '2024-05-20', '2024-05-24', 'Environment', NULL),
(4, 7, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-04-15', '2025-05-15', NULL, 'Environment', NULL),
(5, 8, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-06-10', '2025-07-10', NULL, 'Safety', NULL),
(6, 5, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2023-08-05', '2023-09-05', '2023-09-09', 'Quality', NULL),
(7, 12, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-08-02', '2024-09-02', '2024-09-04', 'Quality', NULL),
(8, 12, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-08-01', '2025-09-01', NULL, 'Quality', NULL),
(9, 10, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2023-12-15', '2024-01-15', '2024-01-19', 'Security', NULL),
(10, 10, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-12-13', '2025-01-13', '2025-01-15', 'Security', NULL),
(11, 6, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-01-12', '2024-02-12', '2024-02-16', 'Environment', NULL),
(12, 6, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-05-03', '2024-06-03', '2024-06-07', 'Energy', NULL),
(13, 13, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-05-02', '2025-06-02', NULL, 'Energy', NULL),
(14, 13, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-07-19', '2024-08-19', '2024-08-23', 'Energy', NULL),
(15, 8, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-09-14', '2025-10-14', NULL, 'Energy', NULL),
(16, 6, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2023-10-06', '2023-11-06', '2023-11-10', 'Maritime', NULL),
(17, 6, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-03-08', '2024-04-08', '2024-04-12', 'Maritime', NULL),
(18, 14, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-03-07', '2025-04-07', NULL, 'Maritime', NULL),
(19, 8, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-08-16', '2024-09-16', '2024-09-20', 'Safety', NULL),
(20, 9, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2023-12-22', '2024-01-22', '2024-01-26', 'Food Safety', NULL),
(21, 9, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-02-11', '2024-03-11', '2024-03-15', 'Food Safety', NULL),
(22, 9, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-06-01', '2024-07-01', '2024-07-05', 'Food Safety', NULL),
(23, 9, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-12-20', '2025-01-20', '2025-01-22', 'Food Safety', NULL),
(24, 5, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2023-05-12', '2023-06-12', '2023-06-16', 'Automotive', NULL),
(25, 5, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2023-09-23', '2023-10-23', '2023-10-27', 'Automotive', NULL),
(26, 11, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-09-21', '2024-10-21', '2024-10-23', 'Automotive', NULL),
(27, 5, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-11-02', '2024-12-02', NULL, 'Safety', NULL),
(28, 4, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-04-06', '2024-05-06', '2024-05-17', 'Quality', NULL),
(29, 6, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-07-12', '2024-08-12', '2024-08-23', 'Environment', NULL),
(30, 7, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-04-05', '2025-05-05', NULL, 'Quality', NULL),
(31, 12, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2024-10-18', '2024-11-18', '2024-11-22', 'Security', NULL),
(32, 9, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-01-17', '2025-02-17', '2025-02-21', 'Food Safety', NULL),
(34, 10, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-09-10', '2025-09-10', NULL, 'IT Service', NULL),
(35, 8, 'Lead Auditor', 1, GETDATE(), GETDATE(), 1, 1, '2025-10-01', '2025-10-01', NULL, 'Sustainability', NULL);

-- Verify the insert
SELECT COUNT(*) as TotalAuditTeamAssignments FROM [dbo].[AuditTeamMembers];
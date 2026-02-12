-- Insert sample data for Audits table
-- This script creates audit headers referenced by other audit seed scripts

INSERT INTO [dbo].[Audits]
([AuditId], [Sites], [Services], [CompanyId], [Status], [StartDate], [EndDate], [LeadAuditor], [Type])
VALUES
-- Acme Corporation audits
(1, '1,2,3', '1', 1, 'Completed', '2024-03-15', '2024-03-19', 'Michael Brown', 'Initial'),
(2, '1,2', '1', 1, 'Completed', '2025-03-10', '2025-03-12', 'Robert Anderson', 'Surveillance'),
(3, '1,2', '2', 1, 'Completed', '2024-05-20', '2024-05-24', 'Michael Brown', 'Initial'),
(4, '1,2', '2', 1, 'Scheduled', '2025-05-15', NULL, 'Robert Anderson', 'Surveillance'),
(5, '1', '3', 1, 'Scheduled', '2025-07-10', NULL, 'Elena Martinez', 'Pre-Assessment'),

-- TechFlow Industries audits
(6, '4,5,6', '1', 2, 'Completed', '2023-09-05', '2023-09-09', 'Sarah Wilson', 'Initial'),
(7, '4,5', '1', 2, 'Completed', '2024-09-02', '2024-09-04', 'Kevin Lee', 'Surveillance'),
(8, '4,5', '1', 2, 'Scheduled', '2025-09-01', NULL, 'Kevin Lee', 'Surveillance'),
(9, '4,5', '4', 2, 'Completed', '2024-01-15', '2024-01-19', 'Kevin Lee', 'Initial'),
(10, '4,5', '4', 2, 'Completed', '2025-01-13', '2025-01-15', 'Kevin Lee', 'Surveillance'),

-- Green Energy Solutions audits
(11, '7,8,9', '2', 3, 'Completed', '2024-02-12', '2024-02-16', 'Luis Garcia', 'Initial'),
(12, '8', '23', 3, 'Completed', '2024-06-03', '2024-06-07', 'Luis Garcia', 'Initial'),
(13, '8', '23', 3, 'Scheduled', '2025-06-02', NULL, 'Ingrid Schmidt', 'Surveillance'),
(14, '9', '24', 3, 'Completed', '2024-08-19', '2024-08-23', 'Ingrid Schmidt', 'Initial'),
(15, '7', '7', 3, 'Planned', '2025-10-14', NULL, 'Elena Martinez', 'Pre-Assessment'),

-- Maritime Solutions audits
(16, '10,11,12', '1', 4, 'Completed', '2023-11-06', '2023-11-10', 'Luis Garcia', 'Initial'),
(17, '11,12', '22', 4, 'Completed', '2024-04-08', '2024-04-12', 'Luis Garcia', 'Initial'),
(18, '11,12', '22', 4, 'Scheduled', '2025-04-07', NULL, 'Hiroshi Tanaka', 'Surveillance'),
(19, '10,11', '3', 4, 'Completed', '2024-09-16', '2024-09-20', 'Elena Martinez', 'Initial'),

-- Food Excellence Corp audits
(20, '13,14,15', '5', 5, 'Completed', '2024-01-22', '2024-01-26', 'Sarah Wilson', 'Initial'),
(21, '14,15', '28', 5, 'Completed', '2024-03-11', '2024-03-15', 'Sarah Wilson', 'Initial'),
(22, '14', '29', 5, 'Completed', '2024-07-01', '2024-07-05', 'Sarah Wilson', 'Initial'),
(23, '13,14', '5', 5, 'Completed', '2025-01-20', '2025-01-22', 'Sarah Wilson', 'Surveillance'),

-- AutoTech Manufacturing audits
(24, '16,17,18', '1', 6, 'Completed', '2023-06-12', '2023-06-16', 'Sarah Wilson', 'Initial'),
(25, '16,17,18', '8', 6, 'Completed', '2023-10-23', '2023-10-27', 'Sarah Wilson', 'Initial'),
(26, '16,17', '8', 6, 'Completed', '2024-10-21', '2024-10-23', 'Lisa Taylor', 'Surveillance'),
(27, '17,18', '11', 6, 'In Progress', '2024-12-02', NULL, 'Sarah Wilson', 'Special'),

-- Global Manufacturing Inc audits
(28, '19,20,21,22', '1', 17, 'Completed', '2024-05-06', '2024-05-17', 'Michael Brown', 'Integrated'),
(29, '19,20,21,22', '2', 17, 'Completed', '2024-08-12', '2024-08-23', 'Luis Garcia', 'Integrated'),
(30, '19,20,21', '1', 17, 'Scheduled', '2025-05-05', NULL, 'John Smith', 'Surveillance'),

-- Small company audits
(31, '23', '4', 19, 'Completed', '2024-11-18', '2024-11-22', 'Kevin Lee', 'Initial'),
(32, '24', '5', 20, 'Completed', '2025-02-17', '2025-02-21', 'Sarah Wilson', 'Initial'),

-- Future planned audits referenced in team assignments
(34, '4,5', '10', 2, 'Planned', '2025-09-10', NULL, 'Kevin Lee', 'Initial'),
(35, '7,8,9', '26', 3, 'Planned', '2025-10-01', NULL, 'Elena Martinez', 'Initial');

-- Verify the insert
SELECT COUNT(*) as TotalAudits FROM [dbo].[Audits];
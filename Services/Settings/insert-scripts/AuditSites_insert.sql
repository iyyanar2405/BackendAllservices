-- Insert sample data for AuditSites table
-- This script maps audits to primary sites

INSERT INTO [dbo].[AuditSites]
([AuditId], [SiteId], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Status], [ScheduledDate], [CompletedDate], [Notes])
VALUES
(1, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-03-15', '2024-03-19', 'Primary site for audit 1'),
(2, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-03-10', '2025-03-12', 'Primary site for audit 2'),
(3, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-05-20', '2024-05-24', 'Primary site for audit 3'),
(4, 1, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-05-15', NULL, 'Primary site for audit 4'),
(5, 1, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-07-10', NULL, 'Primary site for audit 5'),
(6, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-09-05', '2023-09-09', 'Primary site for audit 6'),
(7, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-09-02', '2024-09-04', 'Primary site for audit 7'),
(8, 4, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-09-01', NULL, 'Primary site for audit 8'),
(9, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-01-15', '2024-01-19', 'Primary site for audit 9'),
(10, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-01-13', '2025-01-15', 'Primary site for audit 10'),
(11, 7, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-02-12', '2024-02-16', 'Primary site for audit 11'),
(12, 8, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-06-03', '2024-06-07', 'Primary site for audit 12'),
(13, 8, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-06-02', NULL, 'Primary site for audit 13'),
(14, 9, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-08-19', '2024-08-23', 'Primary site for audit 14'),
(15, 7, 1, GETDATE(), GETDATE(), 1, 1, 'planned', '2025-10-14', NULL, 'Primary site for audit 15'),
(16, 10, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-11-06', '2023-11-10', 'Primary site for audit 16'),
(17, 11, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-04-08', '2024-04-12', 'Primary site for audit 17'),
(18, 11, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-04-07', NULL, 'Primary site for audit 18'),
(19, 10, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-09-16', '2024-09-20', 'Primary site for audit 19'),
(20, 13, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-01-22', '2024-01-26', 'Primary site for audit 20'),
(21, 14, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-03-11', '2024-03-15', 'Primary site for audit 21'),
(22, 14, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-07-01', '2024-07-05', 'Primary site for audit 22'),
(23, 13, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-01-20', '2025-01-22', 'Primary site for audit 23'),
(24, 16, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-06-12', '2023-06-16', 'Primary site for audit 24'),
(25, 16, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-10-23', '2023-10-27', 'Primary site for audit 25'),
(26, 16, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-10-21', '2024-10-23', 'Primary site for audit 26'),
(27, 17, 1, GETDATE(), GETDATE(), 1, 1, 'in-progress', '2024-12-02', NULL, 'Primary site for audit 27'),
(28, 19, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-05-06', '2024-05-17', 'Primary site for audit 28'),
(29, 19, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-08-12', '2024-08-23', 'Primary site for audit 29'),
(30, 19, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-05-05', NULL, 'Primary site for audit 30'),
(31, 23, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-11-18', '2024-11-22', 'Primary site for audit 31'),
(32, 24, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-02-17', '2025-02-21', 'Primary site for audit 32'),
(34, 4, 1, GETDATE(), GETDATE(), 1, 1, 'planned', '2025-09-10', NULL, 'Primary site for audit 34'),
(35, 7, 1, GETDATE(), GETDATE(), 1, 1, 'planned', '2025-10-01', NULL, 'Primary site for audit 35');

-- Verify the insert
SELECT COUNT(*) as TotalAuditSiteAssignments FROM [dbo].[AuditSites];
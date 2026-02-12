-- Insert sample data for AuditServices table
-- This script links audits to services

INSERT INTO [dbo].[AuditServices]
([AuditId], [ServiceId], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Status], [StartDate], [EndDate], [Notes])
VALUES
(1, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-03-15', '2024-03-19', NULL),
(2, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-03-10', '2025-03-12', NULL),
(3, 2, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-05-20', '2024-05-24', NULL),
(4, 2, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-05-15', NULL, NULL),
(5, 3, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-07-10', NULL, NULL),
(6, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-09-05', '2023-09-09', NULL),
(7, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-09-02', '2024-09-04', NULL),
(8, 1, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-09-01', NULL, NULL),
(9, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-01-15', '2024-01-19', NULL),
(10, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-01-13', '2025-01-15', NULL),
(11, 2, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-02-12', '2024-02-16', NULL),
(12, 23, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-06-03', '2024-06-07', NULL),
(13, 23, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-06-02', NULL, NULL),
(14, 24, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-08-19', '2024-08-23', NULL),
(15, 7, 1, GETDATE(), GETDATE(), 1, 1, 'planned', '2025-10-14', NULL, NULL),
(16, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-11-06', '2023-11-10', NULL),
(17, 22, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-04-08', '2024-04-12', NULL),
(18, 22, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-04-07', NULL, NULL),
(19, 3, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-09-16', '2024-09-20', NULL),
(20, 5, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-01-22', '2024-01-26', NULL),
(21, 28, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-03-11', '2024-03-15', NULL),
(22, 29, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-07-01', '2024-07-05', NULL),
(23, 5, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-01-20', '2025-01-22', NULL),
(24, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-06-12', '2023-06-16', NULL),
(25, 8, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2023-10-23', '2023-10-27', NULL),
(26, 8, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-10-21', '2024-10-23', NULL),
(27, 11, 1, GETDATE(), GETDATE(), 1, 1, 'in-progress', '2024-12-02', NULL, NULL),
(28, 1, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-05-06', '2024-05-17', NULL),
(29, 2, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-08-12', '2024-08-23', NULL),
(30, 1, 1, GETDATE(), GETDATE(), 1, 1, 'scheduled', '2025-05-05', NULL, NULL),
(31, 4, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2024-11-18', '2024-11-22', NULL),
(32, 5, 1, GETDATE(), GETDATE(), 1, 1, 'completed', '2025-02-17', '2025-02-21', NULL),
(34, 10, 1, GETDATE(), GETDATE(), 1, 1, 'planned', '2025-09-10', NULL, NULL),
(35, 26, 1, GETDATE(), GETDATE(), 1, 1, 'planned', '2025-10-01', NULL, NULL);

-- Verify the insert
SELECT COUNT(*) as TotalAuditServices FROM [dbo].[AuditServices];
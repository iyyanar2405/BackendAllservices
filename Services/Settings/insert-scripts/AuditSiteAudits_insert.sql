-- Insert sample data for AuditSiteAudits table
-- This script creates audit instances per site

INSERT INTO [dbo].[AuditSiteAudits]
([AuditId], [SiteId], [AuditTypeId], [AuditNumber], [ScheduledDate], [StartDate], [EndDate], [CompletedDate], [Status], [LeadAuditorId], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [Notes], [ReportPath], [CertificateIssued], [CertificateNumber])
VALUES
(1, 1, 1, 'AUD-0001', '2024-03-15', '2024-03-15', '2024-03-19', '2024-03-19', 'completed', 4, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0001'),
(2, 1, 2, 'AUD-0002', '2025-03-10', '2025-03-10', '2025-03-12', '2025-03-12', 'completed', 7, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0002'),
(3, 1, 1, 'AUD-0003', '2024-05-20', '2024-05-20', '2024-05-24', '2024-05-24', 'completed', 4, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0003'),
(4, 1, 2, 'AUD-0004', '2025-05-15', '2025-05-15', NULL, NULL, 'scheduled', 7, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(5, 1, 6, 'AUD-0005', '2025-07-10', '2025-07-10', NULL, NULL, 'scheduled', 8, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(6, 4, 1, 'AUD-0006', '2023-09-05', '2023-09-05', '2023-09-09', '2023-09-09', 'completed', 5, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0006'),
(7, 4, 2, 'AUD-0007', '2024-09-02', '2024-09-02', '2024-09-04', '2024-09-04', 'completed', 12, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0007'),
(8, 4, 2, 'AUD-0008', '2025-09-01', '2025-09-01', NULL, NULL, 'scheduled', 12, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(9, 4, 1, 'AUD-0009', '2024-01-15', '2024-01-15', '2024-01-19', '2024-01-19', 'completed', 10, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0009'),
(10, 4, 2, 'AUD-0010', '2025-01-13', '2025-01-13', '2025-01-15', '2025-01-15', 'completed', 10, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0010'),
(11, 7, 1, 'AUD-0011', '2024-02-12', '2024-02-12', '2024-02-16', '2024-02-16', 'completed', 6, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0011'),
(12, 8, 1, 'AUD-0012', '2024-06-03', '2024-06-03', '2024-06-07', '2024-06-07', 'completed', 6, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0012'),
(13, 8, 2, 'AUD-0013', '2025-06-02', '2025-06-02', NULL, NULL, 'scheduled', 13, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(14, 9, 1, 'AUD-0014', '2024-08-19', '2024-08-19', '2024-08-23', '2024-08-23', 'completed', 13, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0014'),
(15, 7, 6, 'AUD-0015', '2025-10-14', '2025-10-14', NULL, NULL, 'scheduled', 8, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(16, 10, 1, 'AUD-0016', '2023-11-06', '2023-11-06', '2023-11-10', '2023-11-10', 'completed', 6, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0016'),
(17, 11, 1, 'AUD-0017', '2024-04-08', '2024-04-08', '2024-04-12', '2024-04-12', 'completed', 6, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0017'),
(18, 11, 2, 'AUD-0018', '2025-04-07', '2025-04-07', NULL, NULL, 'scheduled', 14, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(19, 10, 1, 'AUD-0019', '2024-09-16', '2024-09-16', '2024-09-20', '2024-09-20', 'completed', 8, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0019'),
(20, 13, 1, 'AUD-0020', '2024-01-22', '2024-01-22', '2024-01-26', '2024-01-26', 'completed', 9, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0020'),
(21, 14, 1, 'AUD-0021', '2024-03-11', '2024-03-11', '2024-03-15', '2024-03-15', 'completed', 9, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0021'),
(22, 14, 1, 'AUD-0022', '2024-07-01', '2024-07-01', '2024-07-05', '2024-07-05', 'completed', 9, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0022'),
(23, 13, 2, 'AUD-0023', '2025-01-20', '2025-01-20', '2025-01-22', '2025-01-22', 'completed', 9, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0023'),
(24, 16, 1, 'AUD-0024', '2023-06-12', '2023-06-12', '2023-06-16', '2023-06-16', 'completed', 5, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0024'),
(25, 16, 1, 'AUD-0025', '2023-10-23', '2023-10-23', '2023-10-27', '2023-10-27', 'completed', 5, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0025'),
(26, 16, 2, 'AUD-0026', '2024-10-21', '2024-10-21', '2024-10-23', '2024-10-23', 'completed', 11, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0026'),
(27, 17, 4, 'AUD-0027', '2024-12-02', '2024-12-02', NULL, NULL, 'in-progress', 5, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(28, 19, 5, 'AUD-0028', '2024-05-06', '2024-05-06', '2024-05-17', '2024-05-17', 'completed', 4, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0028'),
(29, 19, 5, 'AUD-0029', '2024-08-12', '2024-08-12', '2024-08-23', '2024-08-23', 'completed', 6, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0029'),
(30, 19, 2, 'AUD-0030', '2025-05-05', '2025-05-05', NULL, NULL, 'scheduled', 7, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(31, 23, 1, 'AUD-0031', '2024-11-18', '2024-11-18', '2024-11-22', '2024-11-22', 'completed', 12, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0031'),
(32, 24, 1, 'AUD-0032', '2025-02-17', '2025-02-21', '2025-02-21', '2025-02-21', 'completed', 9, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 1, 'CERT-AUD-0032'),
(34, 4, 1, 'AUD-0034', '2025-09-10', '2025-09-10', NULL, NULL, 'scheduled', 10, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL),
(35, 7, 1, 'AUD-0035', '2025-10-01', '2025-10-01', NULL, NULL, 'scheduled', 8, 1, GETDATE(), GETDATE(), 1, 1, NULL, NULL, 0, NULL);

-- Verify the insert
SELECT COUNT(*) as TotalAuditSiteAudits FROM [dbo].[AuditSiteAudits];
-- Insert sample data for AuditSiteAudits table
-- This script links audits to specific sites

INSERT INTO [dbo].[AuditSiteAudits] 
([AuditSiteAuditId], [AuditId], [SiteId], [AuditDate], [Status], [AuditorId], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation Audit Site Assignments
(NEWID(), 1, 1, '2024-03-15', 'Completed', 4, '2024-03-15', '2024-03-19', 4, 4), -- ISO 9001 Initial - HQ (Day 1)
(NEWID(), 1, 2, '2024-03-17', 'Completed', 4, '2024-03-17', '2024-03-19', 4, 4), -- ISO 9001 Initial - Manufacturing (Day 3)
(NEWID(), 1, 3, '2024-03-18', 'Completed', 7, '2024-03-18', '2024-03-19', 4, 7), -- ISO 9001 Initial - Warehouse (Day 4)

(NEWID(), 2, 1, '2025-03-10', 'Completed', 7, '2025-03-10', '2025-03-12', 7, 7), -- ISO 9001 Surveillance - HQ
(NEWID(), 2, 2, '2025-03-11', 'Completed', 7, '2025-03-11', '2025-03-12', 7, 7), -- ISO 9001 Surveillance - Manufacturing

(NEWID(), 3, 1, '2024-05-20', 'Completed', 4, '2024-05-20', '2024-05-24', 4, 4), -- ISO 14001 Initial - HQ
(NEWID(), 3, 2, '2024-05-22', 'Completed', 8, '2024-05-22', '2024-05-24', 4, 8), -- ISO 14001 Initial - Manufacturing

(NEWID(), 4, 1, '2025-05-15', 'Scheduled', 7, '2025-05-15', NULL, 7, NULL), -- ISO 14001 Surveillance - HQ
(NEWID(), 4, 2, '2025-05-16', 'Scheduled', 7, '2025-05-16', NULL, 7, NULL), -- ISO 14001 Surveillance - Manufacturing

(NEWID(), 5, 1, '2025-07-10', 'Scheduled', 8, '2025-07-10', NULL, 8, NULL), -- ISO 45001 Pre-Assessment - HQ

-- TechFlow Industries Audit Site Assignments
(NEWID(), 6, 4, '2023-09-05', 'Completed', 5, '2023-09-05', '2023-09-09', 5, 5), -- ISO 9001 Initial - Main Office
(NEWID(), 6, 5, '2023-09-07', 'Completed', 12, '2023-09-07', '2023-09-09', 5, 12), -- ISO 9001 Initial - Data Center
(NEWID(), 6, 6, '2023-09-08', 'Completed', 5, '2023-09-08', '2023-09-09', 5, 5), -- ISO 9001 Initial - Dev Lab

(NEWID(), 7, 4, '2024-09-02', 'Completed', 12, '2024-09-02', '2024-09-04', 12, 12), -- ISO 9001 Surveillance - Main Office
(NEWID(), 7, 5, '2024-09-03', 'Completed', 12, '2024-09-03', '2024-09-04', 12, 12), -- ISO 9001 Surveillance - Data Center

(NEWID(), 8, 4, '2025-09-01', 'Scheduled', 12, '2025-09-01', NULL, 12, NULL), -- ISO 9001 Surveillance Y2 - Main Office
(NEWID(), 8, 5, '2025-09-02', 'Scheduled', 12, '2025-09-02', NULL, 12, NULL), -- ISO 9001 Surveillance Y2 - Data Center

(NEWID(), 9, 4, '2024-01-15', 'Completed', 10, '2024-01-15', '2024-01-19', 10, 10), -- ISO 27001 Initial - Main Office
(NEWID(), 9, 5, '2024-01-17', 'Completed', 10, '2024-01-17', '2024-01-19', 10, 10), -- ISO 27001 Initial - Data Center

(NEWID(), 10, 4, '2025-01-13', 'Completed', 10, '2025-01-13', '2025-01-15', 10, 10), -- ISO 27001 Surveillance - Main Office
(NEWID(), 10, 5, '2025-01-14', 'Completed', 10, '2025-01-14', '2025-01-15', 10, 10), -- ISO 27001 Surveillance - Data Center

-- Green Energy Solutions Audit Site Assignments
(NEWID(), 11, 7, '2024-02-12', 'Completed', 6, '2024-02-12', '2024-02-16', 6, 6), -- ISO 14001 Initial - HQ
(NEWID(), 11, 8, '2024-02-14', 'Completed', 13, '2024-02-14', '2024-02-16', 6, 13), -- ISO 14001 Initial - Wind Farm
(NEWID(), 11, 9, '2024-02-15', 'Completed', 6, '2024-02-15', '2024-02-16', 6, 6), -- ISO 14001 Initial - Solar Installation

(NEWID(), 12, 8, '2024-06-03', 'Completed', 6, '2024-06-03', '2024-06-07', 6, 6), -- Wind Turbine Cert - Wind Farm
(NEWID(), 13, 8, '2025-06-02', 'Scheduled', 13, '2025-06-02', NULL, 13, NULL), -- Wind Turbine Surveillance

(NEWID(), 14, 9, '2024-08-19', 'Completed', 13, '2024-08-19', '2024-08-23', 13, 13), -- Solar Panel Cert - Solar Installation

(NEWID(), 15, 7, '2025-10-14', 'Planned', 8, '2025-10-14', NULL, 8, NULL), -- ISO 50001 Pre-Assessment - HQ

-- Maritime Solutions Audit Site Assignments
(NEWID(), 16, 10, '2023-11-06', 'Completed', 6, '2023-11-06', '2023-11-10', 6, 6), -- ISO 9001 Initial - Oslo Office
(NEWID(), 16, 11, '2023-11-08', 'Completed', 14, '2023-11-08', '2023-11-10', 6, 14), -- ISO 9001 Initial - Shipyard
(NEWID(), 16, 12, '2023-11-09', 'Completed', 6, '2023-11-09', '2023-11-10', 6, 6), -- ISO 9001 Initial - Port

(NEWID(), 17, 11, '2024-04-08', 'Completed', 6, '2024-04-08', '2024-04-12', 6, 6), -- DNV Ships - Shipyard
(NEWID(), 17, 12, '2024-04-10', 'Completed', 14, '2024-04-10', '2024-04-12', 6, 14), -- DNV Ships - Port

(NEWID(), 18, 11, '2025-04-07', 'Scheduled', 14, '2025-04-07', NULL, 14, NULL), -- DNV Ships Surveillance - Shipyard
(NEWID(), 18, 12, '2025-04-08', 'Scheduled', 14, '2025-04-08', NULL, 14, NULL), -- DNV Ships Surveillance - Port

(NEWID(), 19, 10, '2024-09-16', 'Completed', 8, '2024-09-16', '2024-09-20', 8, 8), -- ISO 45001 Initial - Oslo Office
(NEWID(), 19, 11, '2024-09-18', 'Completed', 8, '2024-09-18', '2024-09-20', 8, 8), -- ISO 45001 Initial - Shipyard

-- Food Excellence Corp Audit Site Assignments
(NEWID(), 20, 13, '2024-01-22', 'Completed', 9, '2024-01-22', '2024-01-26', 9, 9), -- ISO 22000 Initial - HQ
(NEWID(), 20, 14, '2024-01-24', 'Completed', 9, '2024-01-24', '2024-01-26', 9, 9), -- ISO 22000 Initial - Production
(NEWID(), 20, 15, '2024-01-25', 'Completed', 9, '2024-01-25', '2024-01-26', 9, 9), -- ISO 22000 Initial - Distribution

(NEWID(), 21, 14, '2024-03-11', 'Completed', 9, '2024-03-11', '2024-03-15', 9, 9), -- HACCP Initial - Production
(NEWID(), 21, 15, '2024-03-13', 'Completed', 9, '2024-03-13', '2024-03-15', 9, 9), -- HACCP Initial - Distribution

(NEWID(), 22, 14, '2024-07-01', 'Completed', 9, '2024-07-01', '2024-07-05', 9, 9), -- BRC Initial - Production

(NEWID(), 23, 13, '2025-01-20', 'Completed', 9, '2025-01-20', '2025-01-22', 9, 9), -- ISO 22000 Surveillance - HQ
(NEWID(), 23, 14, '2025-01-21', 'Completed', 9, '2025-01-21', '2025-01-22', 9, 9), -- ISO 22000 Surveillance - Production

-- AutoTech Manufacturing Audit Site Assignments
(NEWID(), 24, 16, '2023-06-12', 'Completed', 5, '2023-06-12', '2023-06-16', 5, 5), -- ISO 9001 Initial - Rome HQ
(NEWID(), 24, 17, '2023-06-14', 'Completed', 11, '2023-06-14', '2023-06-16', 5, 11), -- ISO 9001 Initial - Milan Factory
(NEWID(), 24, 18, '2023-06-15', 'Completed', 5, '2023-06-15', '2023-06-16', 5, 5), -- ISO 9001 Initial - Turin R&D

(NEWID(), 25, 16, '2023-10-23', 'Completed', 5, '2023-10-23', '2023-10-27', 5, 5), -- IATF Initial - Rome HQ
(NEWID(), 25, 17, '2023-10-25', 'Completed', 11, '2023-10-25', '2023-10-27', 5, 11), -- IATF Initial - Milan Factory
(NEWID(), 25, 18, '2023-10-26', 'Completed', 5, '2023-10-26', '2023-10-27', 5, 5), -- IATF Initial - Turin R&D

(NEWID(), 26, 16, '2024-10-21', 'Completed', 11, '2024-10-21', '2024-10-23', 11, 11), -- IATF Surveillance - Rome HQ
(NEWID(), 26, 17, '2024-10-22', 'Completed', 11, '2024-10-22', '2024-10-23', 11, 11), -- IATF Surveillance - Milan Factory

(NEWID(), 27, 17, '2024-12-02', 'In Progress', 5, '2024-12-02', NULL, 5, NULL), -- ISO 26262 - Milan Factory
(NEWID(), 27, 18, '2024-12-04', 'In Progress', 5, '2024-12-04', NULL, 5, NULL), -- ISO 26262 - Turin R&D

-- Global Manufacturing Inc Multi-Site Audits
(NEWID(), 28, 19, '2024-05-06', 'Completed', 4, '2024-05-06', '2024-05-17', 4, 4), -- ISO 9001 Multi-Site - US HQ
(NEWID(), 28, 20, '2024-05-09', 'Completed', 7, '2024-05-09', '2024-05-17', 4, 7), -- ISO 9001 Multi-Site - UK Ops
(NEWID(), 28, 21, '2024-05-13', 'Completed', 11, '2024-05-13', '2024-05-17', 4, 11), -- ISO 9001 Multi-Site - DE Plant
(NEWID(), 28, 22, '2024-05-15', 'Completed', 4, '2024-05-15', '2024-05-17', 4, 4), -- ISO 9001 Multi-Site - AP Hub

(NEWID(), 29, 19, '2024-08-12', 'Completed', 6, '2024-08-12', '2024-08-23', 6, 6), -- ISO 14001 Multi-Site - US HQ
(NEWID(), 29, 20, '2024-08-15', 'Completed', 8, '2024-08-15', '2024-08-23', 6, 8), -- ISO 14001 Multi-Site - UK Ops
(NEWID(), 29, 21, '2024-08-19', 'Completed', 6, '2024-08-19', '2024-08-23', 6, 6), -- ISO 14001 Multi-Site - DE Plant
(NEWID(), 29, 22, '2024-08-21', 'Completed', 8, '2024-08-21', '2024-08-23', 6, 8), -- ISO 14001 Multi-Site - AP Hub

(NEWID(), 30, 19, '2025-05-05', 'Scheduled', 7, '2025-05-05', NULL, 7, NULL), -- ISO 9001 Surveillance - US HQ
(NEWID(), 30, 20, '2025-05-07', 'Scheduled', 7, '2025-05-07', NULL, 7, NULL), -- ISO 9001 Surveillance - UK Ops
(NEWID(), 30, 21, '2025-05-08', 'Scheduled', 11, '2025-05-08', NULL, 7, NULL), -- ISO 9001 Surveillance - DE Plant

-- Small Company Audits
(NEWID(), 31, 23, '2024-11-18', 'Completed', 12, '2024-11-18', '2024-11-22', 12, 12), -- Small Tech ISO 27001

(NEWID(), 32, 24, '2025-02-17', 'Completed', 9, '2025-02-17', '2025-02-21', 9, 9); -- Family Food ISO 22000

-- Verify the insert
SELECT COUNT(*) as TotalAuditSiteAssignments FROM [dbo].[AuditSiteAudits];

-- Show audit site assignments by status
SELECT 
    Status,
    COUNT(*) as Count
FROM [dbo].[AuditSiteAudits] 
GROUP BY Status
ORDER BY Count DESC;

-- Show sites per audit
SELECT 
    a.AuditTitle,
    COUNT(asa.SiteId) as SitesAudited,
    STRING_AGG(s.SiteName, ', ') as Sites
FROM [dbo].[AuditSiteAudits] asa
INNER JOIN [dbo].[Audits] a ON asa.AuditId = a.AuditId
INNER JOIN [dbo].[Sites] s ON asa.SiteId = s.SiteId
GROUP BY a.AuditId, a.AuditTitle
ORDER BY SitesAudited DESC;

-- Show auditor workload
SELECT 
    u.FirstName + ' ' + u.LastName as Auditor,
    COUNT(asa.AuditSiteAuditId) as SiteAudits,
    COUNT(DISTINCT asa.AuditId) as TotalAudits
FROM [dbo].[AuditSiteAudits] asa
INNER JOIN [dbo].[Users] u ON asa.AuditorId = u.UserId
GROUP BY asa.AuditorId, u.FirstName, u.LastName
ORDER BY SiteAudits DESC;
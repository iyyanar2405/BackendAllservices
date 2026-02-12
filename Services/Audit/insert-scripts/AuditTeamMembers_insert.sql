-- Insert sample data for AuditTeamMembers table
-- This script assigns team members to audits

INSERT INTO [dbo].[AuditTeamMembers] 
([AuditTeamMemberId], [AuditId], [UserId], [Role], [AssignedDate], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation Audits Team Assignments
(NEWID(), 1, 4, 'Lead Auditor', '2024-02-15', 1, '2024-02-15', '2024-03-19', 4, 4), -- ISO 9001 Initial
(NEWID(), 1, 7, 'Auditor', '2024-02-15', 1, '2024-02-15', '2024-03-19', 4, 7),
(NEWID(), 1, 16, 'Audit Coordinator', '2024-02-15', 1, '2024-02-15', '2024-03-19', 4, 16),

(NEWID(), 2, 7, 'Lead Auditor', '2025-02-10', 1, '2025-02-10', '2025-03-12', 7, 7), -- ISO 9001 Surveillance
(NEWID(), 2, 11, 'Auditor', '2025-02-10', 1, '2025-02-10', '2025-03-12', 7, 11),

(NEWID(), 3, 4, 'Lead Auditor', '2024-04-20', 1, '2024-04-20', '2024-05-24', 4, 4), -- ISO 14001 Initial
(NEWID(), 3, 8, 'Technical Specialist', '2024-04-20', 1, '2024-04-20', '2024-05-24', 4, 8),

(NEWID(), 4, 7, 'Lead Auditor', '2025-04-15', 1, '2025-04-15', NULL, 7, NULL), -- ISO 14001 Surveillance
(NEWID(), 4, 8, 'Technical Specialist', '2025-04-15', 1, '2025-04-15', NULL, 7, NULL),

(NEWID(), 5, 8, 'Lead Auditor', '2025-06-10', 1, '2025-06-10', NULL, 8, NULL), -- ISO 45001 Pre-Assessment

-- TechFlow Industries Audits Team Assignments
(NEWID(), 6, 5, 'Lead Auditor', '2023-08-05', 1, '2023-08-05', '2023-09-09', 5, 5), -- ISO 9001 Initial
(NEWID(), 6, 12, 'Auditor', '2023-08-05', 1, '2023-08-05', '2023-09-09', 5, 12),
(NEWID(), 6, 16, 'Audit Coordinator', '2023-08-05', 1, '2023-08-05', '2023-09-09', 5, 16),

(NEWID(), 7, 12, 'Lead Auditor', '2024-08-02', 1, '2024-08-02', '2024-09-04', 12, 12), -- ISO 9001 Surveillance Y1

(NEWID(), 8, 12, 'Lead Auditor', '2025-08-01', 1, '2025-08-01', NULL, 12, NULL), -- ISO 9001 Surveillance Y2

(NEWID(), 9, 10, 'Lead Auditor', '2023-12-15', 1, '2023-12-15', '2024-01-19', 10, 10), -- ISO 27001 Initial
(NEWID(), 9, 14, 'IT Security Specialist', '2023-12-15', 1, '2023-12-15', '2024-01-19', 10, 14),

(NEWID(), 10, 10, 'Lead Auditor', '2024-12-13', 1, '2024-12-13', '2025-01-15', 10, 10), -- ISO 27001 Surveillance

-- Green Energy Solutions Audits Team Assignments
(NEWID(), 11, 6, 'Lead Auditor', '2024-01-12', 1, '2024-01-12', '2024-02-16', 6, 6), -- ISO 14001 Initial
(NEWID(), 11, 13, 'Environmental Specialist', '2024-01-12', 1, '2024-01-12', '2024-02-16', 6, 13),

(NEWID(), 12, 6, 'Lead Auditor', '2024-05-03', 1, '2024-05-03', '2024-06-07', 6, 6), -- Wind Turbine Cert
(NEWID(), 12, 13, 'Wind Energy Specialist', '2024-05-03', 1, '2024-05-03', '2024-06-07', 6, 13),
(NEWID(), 12, 14, 'Technical Auditor', '2024-05-03', 1, '2024-05-03', '2024-06-07', 6, 14),

(NEWID(), 13, 13, 'Lead Auditor', '2025-05-02', 1, '2025-05-02', NULL, 13, NULL), -- Wind Turbine Surveillance
(NEWID(), 13, 6, 'Technical Review', '2025-05-02', 1, '2025-05-02', NULL, 13, NULL),

(NEWID(), 14, 13, 'Lead Auditor', '2024-07-19', 1, '2024-07-19', '2024-08-23', 13, 13), -- Solar Panel Cert
(NEWID(), 14, 8, 'Solar Energy Specialist', '2024-07-19', 1, '2024-07-19', '2024-08-23', 13, 8),

(NEWID(), 15, 8, 'Lead Auditor', '2025-09-14', 1, '2025-09-14', NULL, 8, NULL), -- ISO 50001 Pre-Assessment

-- Maritime Solutions Audits Team Assignments
(NEWID(), 16, 6, 'Lead Auditor', '2023-10-06', 1, '2023-10-06', '2023-11-10', 6, 6), -- ISO 9001 Initial
(NEWID(), 16, 14, 'Maritime Specialist', '2023-10-06', 1, '2023-10-06', '2023-11-10', 6, 14),
(NEWID(), 16, 11, 'Quality Auditor', '2023-10-06', 1, '2023-10-06', '2023-11-10', 6, 11),

(NEWID(), 17, 6, 'Lead Auditor', '2024-03-08', 1, '2024-03-08', '2024-04-12', 6, 6), -- DNV Ships Cert
(NEWID(), 17, 14, 'Ship Classification Specialist', '2024-03-08', 1, '2024-03-08', '2024-04-12', 6, 14),
(NEWID(), 17, 12, 'Marine Engineering Auditor', '2024-03-08', 1, '2024-03-08', '2024-04-12', 6, 12),

(NEWID(), 18, 14, 'Lead Auditor', '2025-03-07', 1, '2025-03-07', NULL, 14, NULL), -- DNV Ships Surveillance
(NEWID(), 18, 6, 'Technical Review', '2025-03-07', 1, '2025-03-07', NULL, 14, NULL),

(NEWID(), 19, 8, 'Lead Auditor', '2024-08-16', 1, '2024-08-16', '2024-09-20', 8, 8), -- ISO 45001 Initial
(NEWID(), 19, 14, 'Safety Specialist', '2024-08-16', 1, '2024-08-16', '2024-09-20', 8, 14),

-- Food Excellence Corp Audits Team Assignments
(NEWID(), 20, 9, 'Lead Auditor', '2023-12-22', 1, '2023-12-22', '2024-01-26', 9, 9), -- ISO 22000 Initial
(NEWID(), 20, 15, 'Food Safety Specialist', '2023-12-22', 1, '2023-12-22', '2024-01-26', 9, 15),

(NEWID(), 21, 9, 'Lead Auditor', '2024-02-11', 1, '2024-02-11', '2024-03-15', 9, 9), -- HACCP Cert
(NEWID(), 21, 15, 'HACCP Specialist', '2024-02-11', 1, '2024-02-11', '2024-03-15', 9, 15),

(NEWID(), 22, 9, 'Lead Auditor', '2024-06-01', 1, '2024-06-01', '2024-07-05', 9, 9), -- BRC Food Safety
(NEWID(), 22, 15, 'BRC Specialist', '2024-06-01', 1, '2024-06-01', '2024-07-05', 9, 15),

(NEWID(), 23, 9, 'Lead Auditor', '2024-12-20', 1, '2024-12-20', '2025-01-22', 9, 9), -- ISO 22000 Surveillance

-- AutoTech Manufacturing Audits Team Assignments
(NEWID(), 24, 5, 'Lead Auditor', '2023-05-12', 1, '2023-05-12', '2023-06-16', 5, 5), -- ISO 9001 Initial
(NEWID(), 24, 11, 'Quality Auditor', '2023-05-12', 1, '2023-05-12', '2023-06-16', 5, 11),

(NEWID(), 25, 5, 'Lead Auditor', '2023-09-23', 1, '2023-09-23', '2023-10-27', 5, 5), -- IATF Initial
(NEWID(), 25, 11, 'Automotive Specialist', '2023-09-23', 1, '2023-09-23', '2023-10-27', 5, 11),
(NEWID(), 25, 7, 'IATF Specialist', '2023-09-23', 1, '2023-09-23', '2023-10-27', 5, 7),

(NEWID(), 26, 11, 'Lead Auditor', '2024-09-21', 1, '2024-09-21', '2024-10-23', 11, 11), -- IATF Surveillance
(NEWID(), 26, 7, 'Automotive Auditor', '2024-09-21', 1, '2024-09-21', '2024-10-23', 11, 7),

(NEWID(), 27, 5, 'Lead Auditor', '2024-11-02', 1, '2024-11-02', NULL, 5, NULL), -- ISO 26262 
(NEWID(), 27, 11, 'Functional Safety Specialist', '2024-11-02', 1, '2024-11-02', NULL, 5, NULL),

-- Global Manufacturing Inc Multi-Site Audits Team Assignments
(NEWID(), 28, 4, 'Lead Auditor', '2024-04-06', 1, '2024-04-06', '2024-05-17', 4, 4), -- ISO 9001 Multi-Site
(NEWID(), 28, 7, 'Regional Auditor - Europe', '2024-04-06', 1, '2024-04-06', '2024-05-17', 4, 7),
(NEWID(), 28, 11, 'Regional Auditor - Europe', '2024-04-06', 1, '2024-04-06', '2024-05-17', 4, 11),
(NEWID(), 28, 12, 'Regional Auditor - Asia', '2024-04-06', 1, '2024-04-06', '2024-05-17', 4, 12),

(NEWID(), 29, 6, 'Lead Auditor', '2024-07-12', 1, '2024-07-12', '2024-08-23', 6, 6), -- ISO 14001 Multi-Site
(NEWID(), 29, 8, 'Environmental Specialist - Europe', '2024-07-12', 1, '2024-07-12', '2024-08-23', 6, 8),
(NEWID(), 29, 13, 'Environmental Specialist - Asia', '2024-07-12', 1, '2024-07-12', '2024-08-23', 6, 13),

(NEWID(), 30, 7, 'Lead Auditor', '2025-04-05', 1, '2025-04-05', NULL, 7, NULL), -- ISO 9001 Multi-Site Surveillance
(NEWID(), 30, 11, 'Regional Auditor - Europe', '2025-04-05', 1, '2025-04-05', NULL, 7, NULL),
(NEWID(), 30, 12, 'Regional Auditor - Asia', '2025-04-05', 1, '2025-04-05', NULL, 7, NULL),

-- Small Company Audits Team Assignments
(NEWID(), 31, 12, 'Lead Auditor', '2024-10-18', 1, '2024-10-18', '2024-11-22', 12, 12), -- Small Tech ISO 27001
(NEWID(), 31, 10, 'IT Security Specialist', '2024-10-18', 1, '2024-10-18', '2024-11-22', 12, 10),

(NEWID(), 32, 9, 'Lead Auditor', '2025-01-17', 1, '2025-01-17', '2025-02-21', 9, 9), -- Family Food ISO 22000

-- Future Planned Audits Team Assignments
(NEWID(), 34, 10, 'Lead Auditor', '2025-09-10', 1, '2025-09-10', NULL, 10, NULL), -- TechFlow ISO 20000-1
(NEWID(), 34, 12, 'IT Service Management Specialist', '2025-09-10', 1, '2025-09-10', NULL, 10, NULL),

(NEWID(), 35, 8, 'Lead Auditor', '2025-10-01', 1, '2025-10-01', NULL, 8, NULL), -- Green Energy Carbon Footprint
(NEWID(), 35, 6, 'Carbon Assessment Specialist', '2025-10-01', 1, '2025-10-01', NULL, 8, NULL);

-- Verify the insert
SELECT COUNT(*) as TotalAuditTeamAssignments FROM [dbo].[AuditTeamMembers];

-- Show team composition by audit
SELECT 
    a.AuditTitle,
    c.CompanyName,
    COUNT(atm.UserId) as TeamSize,
    STRING_AGG(CONCAT(u.FirstName, ' ', u.LastName, ' (', atm.Role, ')'), ', ') as TeamMembers
FROM [dbo].[AuditTeamMembers] atm
INNER JOIN [dbo].[Audits] a ON atm.AuditId = a.AuditId
INNER JOIN [dbo].[Companies] c ON a.CompanyId = c.CompanyId
INNER JOIN [dbo].[Users] u ON atm.UserId = u.UserId
WHERE atm.IsActive = 1
GROUP BY a.AuditId, a.AuditTitle, c.CompanyName
ORDER BY TeamSize DESC;

-- Show auditor workload
SELECT 
    u.FirstName + ' ' + u.LastName as Auditor,
    COUNT(atm.AuditId) as TotalAudits,
    COUNT(CASE WHEN atm.Role = 'Lead Auditor' THEN 1 END) as LeadAuditorRoles,
    COUNT(CASE WHEN a.Status IN ('Scheduled', 'In Progress', 'Planned') THEN 1 END) as UpcomingAudits
FROM [dbo].[AuditTeamMembers] atm
INNER JOIN [dbo].[Users] u ON atm.UserId = u.UserId
INNER JOIN [dbo].[Audits] a ON atm.AuditId = a.AuditId
WHERE atm.IsActive = 1
GROUP BY atm.UserId, u.FirstName, u.LastName
ORDER BY TotalAudits DESC;

-- Show team roles distribution
SELECT 
    Role,
    COUNT(*) as Count
FROM [dbo].[AuditTeamMembers] 
WHERE IsActive = 1
GROUP BY Role
ORDER BY Count DESC;
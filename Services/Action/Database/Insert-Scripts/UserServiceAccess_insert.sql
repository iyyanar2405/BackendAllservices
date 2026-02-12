-- Insert sample data for UserServiceAccess table
-- This script defines which users can access which services

INSERT INTO [dbo].[UserServiceAccess] 
([UserId], [ServiceId], [AccessLevel], [GrantedDate], [IsActive], [GrantedBy])
VALUES
-- DNV Super Admin - Access to ALL services
(1, 1, 'Full', GETDATE(), 1, 1),   -- ISO 9001:2015
(1, 2, 'Full', GETDATE(), 1, 1),   -- ISO 14001:2015
(1, 3, 'Full', GETDATE(), 1, 1),   -- ISO 45001:2018
(1, 4, 'Full', GETDATE(), 1, 1),   -- ISO 27001:2013
(1, 5, 'Full', GETDATE(), 1, 1),   -- ISO 22000:2018
(1, 6, 'Full', GETDATE(), 1, 1),   -- ISO 13485:2016
(1, 7, 'Full', GETDATE(), 1, 1),   -- ISO 50001:2018
(1, 8, 'Full', GETDATE(), 1, 1),   -- IATF 16949:2016
(1, 9, 'Full', GETDATE(), 1, 1),   -- AS9100:2016
(1, 10, 'Full', GETDATE(), 1, 1),  -- ISO 20000-1:2018
(1, 11, 'Full', GETDATE(), 1, 1),  -- ISO 26262
(1, 12, 'Full', GETDATE(), 1, 1),  -- EN 1090
(1, 13, 'Full', GETDATE(), 1, 1),  -- CE Marking
(1, 14, 'Full', GETDATE(), 1, 1),  -- GDPR Compliance
(1, 15, 'Full', GETDATE(), 1, 1),  -- OHSAS 18001
(1, 16, 'Full', GETDATE(), 1, 1),  -- ISO 17025:2017
(1, 17, 'Full', GETDATE(), 1, 1),  -- ISO 31000:2018
(1, 18, 'Full', GETDATE(), 1, 1),  -- IEC 61508
(1, 19, 'Full', GETDATE(), 1, 1),  -- ISO 15189:2012
(1, 20, 'Full', GETDATE(), 1, 1),  -- GMP Certification
(1, 21, 'Full', GETDATE(), 1, 1),  -- API Standards
(1, 22, 'Full', GETDATE(), 1, 1),  -- DNV Rules for Ships
(1, 23, 'Full', GETDATE(), 1, 1),  -- Wind Turbine Certification
(1, 24, 'Full', GETDATE(), 1, 1),  -- Solar Panel Certification
(1, 25, 'Full', GETDATE(), 1, 1),  -- Cyber Security Assessment
(1, 26, 'Full', GETDATE(), 1, 1),  -- Carbon Footprint Verification
(1, 27, 'Full', GETDATE(), 1, 1),  -- Supply Chain Security
(1, 28, 'Full', GETDATE(), 1, 1),  -- HACCP Certification
(1, 29, 'Full', GETDATE(), 1, 1),  -- BRC Food Safety
(1, 30, 'Full', GETDATE(), 1, 1),  -- Sustainable Business

-- DNV Regional Managers - Broad service access by region/specialty
-- European Regional Manager (UserId 2) - European focused services
(2, 1, 'Full', GETDATE(), 1, 1),   -- ISO 9001 (universal)
(2, 2, 'Full', GETDATE(), 1, 1),   -- ISO 14001 (environmental - EU focus)
(2, 3, 'Full', GETDATE(), 1, 1),   -- ISO 45001 (safety)
(2, 4, 'Full', GETDATE(), 1, 1),   -- ISO 27001 (IT security)
(2, 7, 'Full', GETDATE(), 1, 1),   -- ISO 50001 (energy - EU important)
(2, 8, 'Full', GETDATE(), 1, 1),   -- IATF 16949 (automotive - Europe strong)
(2, 12, 'Full', GETDATE(), 1, 1),  -- EN 1090 (European standard)
(2, 13, 'Full', GETDATE(), 1, 1),  -- CE Marking (European)
(2, 14, 'Full', GETDATE(), 1, 1),  -- GDPR (European)
(2, 23, 'Full', GETDATE(), 1, 1),  -- Wind Turbine (Europe renewable)
(2, 24, 'Full', GETDATE(), 1, 1),  -- Solar Panel
(2, 26, 'Full', GETDATE(), 1, 1),  -- Carbon Footprint (EU regulations)
(2, 30, 'Full', GETDATE(), 1, 1),  -- Sustainable Business

-- Americas Regional Manager (UserId 3) - Americas focused services
(3, 1, 'Full', GETDATE(), 1, 1),   -- ISO 9001 (universal)
(3, 2, 'Full', GETDATE(), 1, 1),   -- ISO 14001
(3, 3, 'Full', GETDATE(), 1, 1),   -- ISO 45001
(3, 8, 'Full', GETDATE(), 1, 1),   -- IATF 16949 (automotive)
(3, 9, 'Full', GETDATE(), 1, 1),   -- AS9100 (aerospace - strong in Americas)
(3, 15, 'Full', GETDATE(), 1, 1),  -- OHSAS 18001
(3, 21, 'Full', GETDATE(), 1, 1),  -- API Standards (oil & gas - Americas)
(3, 25, 'Full', GETDATE(), 1, 1),  -- Cyber Security
(3, 27, 'Full', GETDATE(), 1, 1),  -- Supply Chain Security

-- Lead Auditors - Specialization-based access
-- Lead Auditor 1 (UserId 4) - Manufacturing & Quality focus
(4, 1, 'Audit', GETDATE(), 1, 1),  -- ISO 9001
(4, 8, 'Audit', GETDATE(), 1, 1),  -- IATF 16949
(4, 9, 'Audit', GETDATE(), 1, 1),  -- AS9100
(4, 11, 'Audit', GETDATE(), 1, 1), -- ISO 26262
(4, 16, 'Audit', GETDATE(), 1, 1), -- ISO 17025

-- Lead Auditor 2 (UserId 5) - Manufacturing & Technical focus
(5, 1, 'Audit', GETDATE(), 1, 1),  -- ISO 9001
(5, 8, 'Audit', GETDATE(), 1, 1),  -- IATF 16949
(5, 11, 'Audit', GETDATE(), 1, 1), -- ISO 26262
(5, 12, 'Audit', GETDATE(), 1, 1), -- EN 1090
(5, 13, 'Audit', GETDATE(), 1, 1), -- CE Marking
(5, 18, 'Audit', GETDATE(), 1, 1), -- IEC 61508

-- Lead Auditor 3 (UserId 6) - Energy & Maritime focus
(6, 2, 'Audit', GETDATE(), 1, 1),  -- ISO 14001
(6, 3, 'Audit', GETDATE(), 1, 1),  -- ISO 45001
(6, 7, 'Audit', GETDATE(), 1, 1),  -- ISO 50001
(6, 21, 'Audit', GETDATE(), 1, 1), -- API Standards
(6, 22, 'Audit', GETDATE(), 1, 1), -- DNV Rules for Ships
(6, 23, 'Audit', GETDATE(), 1, 1), -- Wind Turbine Certification
(6, 24, 'Audit', GETDATE(), 1, 1), -- Solar Panel Certification
(6, 26, 'Audit', GETDATE(), 1, 1), -- Carbon Footprint

-- Senior Auditors - Specific area expertise
-- Senior Auditor 1 (UserId 7) - Quality & Manufacturing
(7, 1, 'Audit', GETDATE(), 1, 1),  -- ISO 9001
(7, 8, 'Audit', GETDATE(), 1, 1),  -- IATF 16949
(7, 16, 'Audit', GETDATE(), 1, 1), -- ISO 17025

-- Senior Auditor 2 (UserId 8) - Environmental & Energy
(8, 2, 'Audit', GETDATE(), 1, 1),  -- ISO 14001
(8, 7, 'Audit', GETDATE(), 1, 1),  -- ISO 50001
(8, 26, 'Audit', GETDATE(), 1, 1), -- Carbon Footprint
(8, 30, 'Audit', GETDATE(), 1, 1), -- Sustainable Business

-- Senior Auditor 3 (UserId 9) - Food & Health
(9, 5, 'Audit', GETDATE(), 1, 1),  -- ISO 22000
(9, 6, 'Audit', GETDATE(), 1, 1),  -- ISO 13485
(9, 19, 'Audit', GETDATE(), 1, 1), -- ISO 15189
(9, 20, 'Audit', GETDATE(), 1, 1), -- GMP Certification
(9, 28, 'Audit', GETDATE(), 1, 1), -- HACCP Certification
(9, 29, 'Audit', GETDATE(), 1, 1), -- BRC Food Safety

-- Senior Auditor 4 (UserId 10) - IT & Security
(10, 4, 'Audit', GETDATE(), 1, 1), -- ISO 27001
(10, 10, 'Audit', GETDATE(), 1, 1), -- ISO 20000-1
(10, 14, 'Audit', GETDATE(), 1, 1), -- GDPR Compliance
(10, 25, 'Audit', GETDATE(), 1, 1), -- Cyber Security Assessment
(10, 27, 'Audit', GETDATE(), 1, 1), -- Supply Chain Security

-- Regular Auditors - Limited service access
(11, 1, 'Audit', GETDATE(), 1, 1), -- ISO 9001
(11, 8, 'Audit', GETDATE(), 1, 1), -- IATF 16949
(12, 4, 'Audit', GETDATE(), 1, 1), -- ISO 27001
(12, 10, 'Audit', GETDATE(), 1, 1), -- ISO 20000-1
(13, 2, 'Audit', GETDATE(), 1, 1), -- ISO 14001
(13, 23, 'Audit', GETDATE(), 1, 1), -- Wind Turbine
(14, 22, 'Audit', GETDATE(), 1, 1), -- DNV Rules for Ships
(14, 21, 'Audit', GETDATE(), 1, 1), -- API Standards

-- Junior Auditor - Read access only
(15, 1, 'Read', GETDATE(), 1, 1),  -- ISO 9001
(15, 2, 'Read', GETDATE(), 1, 1),  -- ISO 14001

-- DNV Support Staff
-- Audit Coordinator (UserId 16) - Scheduling access to common services
(16, 1, 'Schedule', GETDATE(), 1, 1),
(16, 2, 'Schedule', GETDATE(), 1, 1),
(16, 3, 'Schedule', GETDATE(), 1, 1),
(16, 4, 'Schedule', GETDATE(), 1, 1),
(16, 5, 'Schedule', GETDATE(), 1, 1),

-- Certification Officer (UserId 17) - Certificate management access
(17, 1, 'Certificate', GETDATE(), 1, 1),
(17, 2, 'Certificate', GETDATE(), 1, 1),
(17, 3, 'Certificate', GETDATE(), 1, 1),
(17, 4, 'Certificate', GETDATE(), 1, 1),
(17, 5, 'Certificate', GETDATE(), 1, 1),
(17, 8, 'Certificate', GETDATE(), 1, 1),
(17, 9, 'Certificate', GETDATE(), 1, 1),

-- Customer Company Users - Access to services they use/need
-- Acme Corporation users (Manufacturing focus)
(21, 1, 'Owner', GETDATE(), 1, 1),  -- ISO 9001
(21, 2, 'Owner', GETDATE(), 1, 1),  -- ISO 14001
(21, 3, 'Owner', GETDATE(), 1, 1),  -- ISO 45001
(21, 8, 'Read', GETDATE(), 1, 1),   -- IATF 16949 (considering)
(22, 1, 'Full', GETDATE(), 1, 21),  -- ISO 9001
(22, 3, 'Full', GETDATE(), 1, 21),  -- ISO 45001

-- TechFlow Industries users (IT/Technology focus)
(23, 1, 'Owner', GETDATE(), 1, 1),  -- ISO 9001
(23, 4, 'Owner', GETDATE(), 1, 1),  -- ISO 27001
(23, 10, 'Owner', GETDATE(), 1, 1), -- ISO 20000-1
(23, 14, 'Owner', GETDATE(), 1, 1), -- GDPR Compliance
(23, 25, 'Read', GETDATE(), 1, 1),  -- Cyber Security (considering)
(24, 4, 'Full', GETDATE(), 1, 23),  -- ISO 27001
(24, 14, 'Full', GETDATE(), 1, 23), -- GDPR Compliance

-- Green Energy Solutions users (Energy/Environmental focus)
(25, 2, 'Owner', GETDATE(), 1, 1),  -- ISO 14001
(25, 3, 'Owner', GETDATE(), 1, 1),  -- ISO 45001
(25, 7, 'Owner', GETDATE(), 1, 1),  -- ISO 50001
(25, 23, 'Owner', GETDATE(), 1, 1), -- Wind Turbine Certification
(25, 24, 'Owner', GETDATE(), 1, 1), -- Solar Panel Certification
(25, 26, 'Owner', GETDATE(), 1, 1), -- Carbon Footprint
(26, 23, 'Full', GETDATE(), 1, 25), -- Wind Turbine (site manager)
(26, 24, 'Read', GETDATE(), 1, 25), -- Solar Panel

-- Maritime Solutions users (Maritime focus)
(27, 1, 'Owner', GETDATE(), 1, 1),  -- ISO 9001
(27, 3, 'Owner', GETDATE(), 1, 1),  -- ISO 45001
(27, 22, 'Owner', GETDATE(), 1, 1), -- DNV Rules for Ships
(27, 21, 'Read', GETDATE(), 1, 1),  -- API Standards (related)
(28, 1, 'Full', GETDATE(), 1, 27),  -- ISO 9001
(28, 22, 'Full', GETDATE(), 1, 27), -- DNV Rules for Ships

-- Food Excellence Corp users (Food safety focus)
(29, 1, 'Owner', GETDATE(), 1, 1),  -- ISO 9001
(29, 5, 'Owner', GETDATE(), 1, 1),  -- ISO 22000
(29, 28, 'Owner', GETDATE(), 1, 1), -- HACCP Certification
(29, 29, 'Owner', GETDATE(), 1, 1), -- BRC Food Safety
(30, 5, 'Full', GETDATE(), 1, 29),  -- ISO 22000
(30, 28, 'Full', GETDATE(), 1, 29); -- HACCP

-- Verify the insert
SELECT COUNT(*) as TotalServiceAccessRecords FROM [dbo].[UserServiceAccess];

-- Show service access summary
SELECT 
    s.ServiceName,
    COUNT(usa.UserId) as UsersWithAccess,
    STRING_AGG(usa.AccessLevel, ', ') as AccessLevels
FROM [dbo].[Services] s
LEFT JOIN [dbo].[UserServiceAccess] usa ON s.ServiceId = usa.ServiceId AND usa.IsActive = 1
GROUP BY s.ServiceId, s.ServiceName
ORDER BY UsersWithAccess DESC;

-- Show user service access by access level
SELECT 
    AccessLevel,
    COUNT(*) as Count
FROM [dbo].[UserServiceAccess] 
WHERE IsActive = 1
GROUP BY AccessLevel
ORDER BY Count DESC;

-- Show services by category with user access
SELECT 
    CASE 
        WHEN s.ServiceName LIKE '%ISO 9001%' OR s.ServiceName LIKE '%Quality%' THEN 'Quality Management'
        WHEN s.ServiceName LIKE '%ISO 14001%' OR s.ServiceName LIKE '%Environmental%' OR s.ServiceName LIKE '%Carbon%' THEN 'Environmental'
        WHEN s.ServiceName LIKE '%ISO 45001%' OR s.ServiceName LIKE '%OHSAS%' OR s.ServiceName LIKE '%Safety%' THEN 'Health & Safety'
        WHEN s.ServiceName LIKE '%ISO 27001%' OR s.ServiceName LIKE '%Cyber%' OR s.ServiceName LIKE '%GDPR%' THEN 'Information Security'
        WHEN s.ServiceName LIKE '%Food%' OR s.ServiceName LIKE '%HACCP%' OR s.ServiceName LIKE '%BRC%' OR s.ServiceName LIKE '%ISO 22000%' THEN 'Food Safety'
        WHEN s.ServiceName LIKE '%Wind%' OR s.ServiceName LIKE '%Solar%' OR s.ServiceName LIKE '%Energy%' THEN 'Renewable Energy'
        WHEN s.ServiceName LIKE '%Ship%' OR s.ServiceName LIKE '%Maritime%' OR s.ServiceName LIKE '%API%' THEN 'Maritime & Energy'
        WHEN s.ServiceName LIKE '%IATF%' OR s.ServiceName LIKE '%AS9100%' OR s.ServiceName LIKE '%ISO 26262%' THEN 'Automotive & Aerospace'
        ELSE 'Other Specialized'
    END as ServiceCategory,
    COUNT(DISTINCT s.ServiceId) as ServicesCount,
    COUNT(usa.UserId) as TotalUserAccess
FROM [dbo].[Services] s
LEFT JOIN [dbo].[UserServiceAccess] usa ON s.ServiceId = usa.ServiceId AND usa.IsActive = 1
GROUP BY 
    CASE 
        WHEN s.ServiceName LIKE '%ISO 9001%' OR s.ServiceName LIKE '%Quality%' THEN 'Quality Management'
        WHEN s.ServiceName LIKE '%ISO 14001%' OR s.ServiceName LIKE '%Environmental%' OR s.ServiceName LIKE '%Carbon%' THEN 'Environmental'
        WHEN s.ServiceName LIKE '%ISO 45001%' OR s.ServiceName LIKE '%OHSAS%' OR s.ServiceName LIKE '%Safety%' THEN 'Health & Safety'
        WHEN s.ServiceName LIKE '%ISO 27001%' OR s.ServiceName LIKE '%Cyber%' OR s.ServiceName LIKE '%GDPR%' THEN 'Information Security'
        WHEN s.ServiceName LIKE '%Food%' OR s.ServiceName LIKE '%HACCP%' OR s.ServiceName LIKE '%BRC%' OR s.ServiceName LIKE '%ISO 22000%' THEN 'Food Safety'
        WHEN s.ServiceName LIKE '%Wind%' OR s.ServiceName LIKE '%Solar%' OR s.ServiceName LIKE '%Energy%' THEN 'Renewable Energy'
        WHEN s.ServiceName LIKE '%Ship%' OR s.ServiceName LIKE '%Maritime%' OR s.ServiceName LIKE '%API%' THEN 'Maritime & Energy'
        WHEN s.ServiceName LIKE '%IATF%' OR s.ServiceName LIKE '%AS9100%' OR s.ServiceName LIKE '%ISO 26262%' THEN 'Automotive & Aerospace'
        ELSE 'Other Specialized'
    END
ORDER BY TotalUserAccess DESC;
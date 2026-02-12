-- Insert sample data for UserSiteAccess table
-- This script defines which users can access which sites

INSERT INTO [dbo].[UserSiteAccess] 
([UserId], [SiteId], [AccessLevel], [GrantedDate], [IsActive], [GrantedBy])
VALUES
-- DNV Super Admin - Access to ALL sites
(1, 1, 'Full', GETDATE(), 1, 1),   -- All Acme sites
(1, 2, 'Full', GETDATE(), 1, 1),
(1, 3, 'Full', GETDATE(), 1, 1),
(1, 4, 'Full', GETDATE(), 1, 1),   -- All TechFlow sites
(1, 5, 'Full', GETDATE(), 1, 1),
(1, 6, 'Full', GETDATE(), 1, 1),
(1, 7, 'Full', GETDATE(), 1, 1),   -- All Green Energy sites
(1, 8, 'Full', GETDATE(), 1, 1),
(1, 9, 'Full', GETDATE(), 1, 1),
(1, 10, 'Full', GETDATE(), 1, 1),  -- All Maritime sites
(1, 11, 'Full', GETDATE(), 1, 1),
(1, 12, 'Full', GETDATE(), 1, 1),
(1, 13, 'Full', GETDATE(), 1, 1),  -- All Food Excellence sites
(1, 14, 'Full', GETDATE(), 1, 1),
(1, 15, 'Full', GETDATE(), 1, 1),
(1, 16, 'Full', GETDATE(), 1, 1),  -- All AutoTech sites
(1, 17, 'Full', GETDATE(), 1, 1),
(1, 18, 'Full', GETDATE(), 1, 1),
(1, 19, 'Full', GETDATE(), 1, 1),  -- All Global Manufacturing sites
(1, 20, 'Full', GETDATE(), 1, 1),
(1, 21, 'Full', GETDATE(), 1, 1),
(1, 22, 'Full', GETDATE(), 1, 1),
(1, 23, 'Full', GETDATE(), 1, 1),  -- Small companies
(1, 24, 'Full', GETDATE(), 1, 1),
(1, 25, 'Full', GETDATE(), 1, 1),  -- Additional sites
(1, 26, 'Full', GETDATE(), 1, 1),
(1, 27, 'Full', GETDATE(), 1, 1),
(1, 28, 'Full', GETDATE(), 1, 1),
(1, 29, 'Full', GETDATE(), 1, 1),
(1, 30, 'Full', GETDATE(), 1, 1),

-- DNV Regional Managers - Regional site access
-- European Regional Manager (UserId 2) - European sites
(2, 4, 'Full', GETDATE(), 1, 1),   -- TechFlow sites (UK)
(2, 5, 'Full', GETDATE(), 1, 1),
(2, 6, 'Full', GETDATE(), 1, 1),
(2, 7, 'Full', GETDATE(), 1, 1),   -- Green Energy sites (Germany)
(2, 8, 'Full', GETDATE(), 1, 1),
(2, 9, 'Full', GETDATE(), 1, 1),
(2, 10, 'Full', GETDATE(), 1, 1),  -- Maritime sites (Norway)
(2, 11, 'Full', GETDATE(), 1, 1),
(2, 12, 'Full', GETDATE(), 1, 1),
(2, 13, 'Full', GETDATE(), 1, 1),  -- Food Excellence sites (France)
(2, 14, 'Full', GETDATE(), 1, 1),
(2, 15, 'Full', GETDATE(), 1, 1),
(2, 16, 'Full', GETDATE(), 1, 1),  -- AutoTech sites (Italy)
(2, 17, 'Full', GETDATE(), 1, 1),
(2, 18, 'Full', GETDATE(), 1, 1),
(2, 20, 'Full', GETDATE(), 1, 1),  -- Global Manufacturing UK
(2, 21, 'Full', GETDATE(), 1, 1),  -- Global Manufacturing Germany
(2, 23, 'Full', GETDATE(), 1, 1),  -- Small Tech (UK)
(2, 24, 'Full', GETDATE(), 1, 1),  -- Family Food (France)
(2, 25, 'Full', GETDATE(), 1, 1),  -- Dutch Innovation
(2, 26, 'Full', GETDATE(), 1, 1),  -- EuroLogistics
(2, 27, 'Full', GETDATE(), 1, 1),  -- Alpine Precision
(2, 28, 'Full', GETDATE(), 1, 1),  -- Nordic Forest
(2, 29, 'Full', GETDATE(), 1, 1),  -- Danish Wind

-- Americas Regional Manager (UserId 3) - Americas sites
(3, 1, 'Full', GETDATE(), 1, 1),   -- Acme sites (USA)
(3, 2, 'Full', GETDATE(), 1, 1),
(3, 3, 'Full', GETDATE(), 1, 1),
(3, 19, 'Full', GETDATE(), 1, 1),  -- Global Manufacturing US
(3, 22, 'Full', GETDATE(), 1, 1),  -- Global Manufacturing Asia Pacific

-- Lead Auditors - Specific site assignments based on their company access
-- Lead Auditor 1 (UserId 4) - Acme, Green Energy, Food Excellence, Health Plus
(4, 1, 'Audit', GETDATE(), 1, 1),  -- Acme HQ
(4, 2, 'Audit', GETDATE(), 1, 1),  -- Acme Manufacturing
(4, 7, 'Audit', GETDATE(), 1, 1),  -- Green Energy HQ
(4, 8, 'Audit', GETDATE(), 1, 1),  -- Green Energy Wind Farm
(4, 13, 'Audit', GETDATE(), 1, 1), -- Food Excellence HQ
(4, 14, 'Audit', GETDATE(), 1, 1), -- Food Excellence Production

-- Lead Auditor 2 (UserId 5) - Manufacturing sites
(5, 16, 'Audit', GETDATE(), 1, 1), -- AutoTech HQ
(5, 17, 'Audit', GETDATE(), 1, 1), -- AutoTech Factory
(5, 18, 'Audit', GETDATE(), 1, 1), -- AutoTech R&D
(5, 27, 'Audit', GETDATE(), 1, 1), -- Alpine Precision
(5, 30, 'Audit', GETDATE(), 1, 1), -- Tokyo Precision
(5, 19, 'Audit', GETDATE(), 1, 1), -- Global Manufacturing US
(5, 20, 'Audit', GETDATE(), 1, 1), -- Global Manufacturing UK
(5, 21, 'Audit', GETDATE(), 1, 1), -- Global Manufacturing Germany

-- Lead Auditor 3 (UserId 6) - Energy and maritime
(6, 10, 'Audit', GETDATE(), 1, 1), -- Maritime HQ
(6, 11, 'Audit', GETDATE(), 1, 1), -- Maritime Shipyard
(6, 12, 'Audit', GETDATE(), 1, 1), -- Maritime Port
(6, 28, 'Audit', GETDATE(), 1, 1), -- Nordic Forest
(6, 29, 'Audit', GETDATE(), 1, 1), -- Danish Wind

-- Senior Auditors - Limited site access
(7, 1, 'Audit', GETDATE(), 1, 1),  -- Acme HQ
(7, 4, 'Audit', GETDATE(), 1, 1),  -- TechFlow HQ
(8, 7, 'Audit', GETDATE(), 1, 1),  -- Green Energy HQ
(8, 10, 'Audit', GETDATE(), 1, 1), -- Maritime HQ
(9, 13, 'Audit', GETDATE(), 1, 1), -- Food Excellence HQ
(9, 16, 'Audit', GETDATE(), 1, 1), -- AutoTech HQ
(10, 25, 'Audit', GETDATE(), 1, 1), -- Dutch Innovation
(10, 26, 'Audit', GETDATE(), 1, 1), -- EuroLogistics

-- Regular Auditors - Specific site assignments
(11, 1, 'Audit', GETDATE(), 1, 1), -- Acme HQ
(11, 2, 'Audit', GETDATE(), 1, 1), -- Acme Manufacturing
(12, 4, 'Audit', GETDATE(), 1, 1), -- TechFlow HQ
(12, 23, 'Audit', GETDATE(), 1, 1), -- Small Tech
(13, 7, 'Audit', GETDATE(), 1, 1), -- Green Energy HQ
(13, 29, 'Audit', GETDATE(), 1, 1), -- Danish Wind
(14, 10, 'Audit', GETDATE(), 1, 1), -- Maritime HQ
(14, 28, 'Audit', GETDATE(), 1, 1), -- Nordic Forest

-- Junior Auditor - Read access only
(15, 1, 'Read', GETDATE(), 1, 1),  -- Acme HQ
(15, 4, 'Read', GETDATE(), 1, 1),  -- TechFlow HQ

-- DNV Support Staff - Function-based access
-- Audit Coordinator (UserId 16) - Scheduling access
(16, 1, 'Schedule', GETDATE(), 1, 1),
(16, 4, 'Schedule', GETDATE(), 1, 1),
(16, 7, 'Schedule', GETDATE(), 1, 1),
(16, 10, 'Schedule', GETDATE(), 1, 1),
(16, 13, 'Schedule', GETDATE(), 1, 1),

-- Certification Officer (UserId 17) - Certificate sites
(17, 1, 'Certificate', GETDATE(), 1, 1),
(17, 4, 'Certificate', GETDATE(), 1, 1),
(17, 7, 'Certificate', GETDATE(), 1, 1),
(17, 10, 'Certificate', GETDATE(), 1, 1),
(17, 13, 'Certificate', GETDATE(), 1, 1),
(17, 16, 'Certificate', GETDATE(), 1, 1),

-- Customer Company Users - Access to their OWN sites only
-- Acme Corporation users
(21, 1, 'Owner', GETDATE(), 1, 1),  -- Acme Admin -> All Acme sites
(21, 2, 'Owner', GETDATE(), 1, 1),
(21, 3, 'Owner', GETDATE(), 1, 1),
(22, 1, 'Full', GETDATE(), 1, 21), -- Acme Site Manager -> Acme HQ (primary)
(22, 2, 'Read', GETDATE(), 1, 21), -- Acme Site Manager -> Manufacturing (read access)

-- TechFlow Industries users
(23, 4, 'Owner', GETDATE(), 1, 1),  -- TechFlow Admin -> All TechFlow sites
(23, 5, 'Owner', GETDATE(), 1, 1),
(23, 6, 'Owner', GETDATE(), 1, 1),
(24, 4, 'Full', GETDATE(), 1, 23), -- TechFlow HSE -> Main Office (primary)
(24, 5, 'Full', GETDATE(), 1, 23), -- TechFlow HSE -> Data Center (HSE responsibility)

-- Green Energy Solutions users
(25, 7, 'Owner', GETDATE(), 1, 1),  -- Green Energy Rep -> All sites
(25, 8, 'Owner', GETDATE(), 1, 1),
(25, 9, 'Owner', GETDATE(), 1, 1),
(26, 8, 'Full', GETDATE(), 1, 25), -- Green Energy Site Manager -> Wind Farm (primary)
(26, 9, 'Read', GETDATE(), 1, 25), -- Green Energy Site Manager -> Solar (read access)

-- Maritime Solutions users
(27, 10, 'Owner', GETDATE(), 1, 1), -- Maritime Admin -> All sites
(27, 11, 'Owner', GETDATE(), 1, 1),
(27, 12, 'Owner', GETDATE(), 1, 1),
(28, 11, 'Full', GETDATE(), 1, 27), -- Maritime Quality -> Shipyard (primary)
(28, 12, 'Full', GETDATE(), 1, 27), -- Maritime Quality -> Port (quality oversight)

-- Food Excellence Corp users
(29, 13, 'Owner', GETDATE(), 1, 1), -- Food Excellence Rep -> All sites
(29, 14, 'Owner', GETDATE(), 1, 1),
(29, 15, 'Owner', GETDATE(), 1, 1),
(30, 14, 'Full', GETDATE(), 1, 29), -- Food Excellence Site Manager -> Production (primary)
(30, 15, 'Read', GETDATE(), 1, 29); -- Food Excellence Site Manager -> Distribution (read access)

-- Verify the insert
SELECT COUNT(*) as TotalSiteAccessRecords FROM [dbo].[UserSiteAccess];

-- Show site access summary
SELECT 
    s.SiteName,
    c.CompanyName,
    COUNT(usa.UserId) as UsersWithAccess,
    STRING_AGG(usa.AccessLevel, ', ') as AccessLevels
FROM [dbo].[Sites] s
INNER JOIN [dbo].[Companies] c ON s.CompanyId = c.CompanyId
LEFT JOIN [dbo].[UserSiteAccess] usa ON s.SiteId = usa.SiteId AND usa.IsActive = 1
GROUP BY s.SiteId, s.SiteName, c.CompanyName
ORDER BY c.CompanyName, s.SiteName;

-- Show access levels by site type
SELECT 
    s.SiteType,
    usa.AccessLevel,
    COUNT(*) as Count
FROM [dbo].[UserSiteAccess] usa
INNER JOIN [dbo].[Sites] s ON usa.SiteId = s.SiteId
WHERE usa.IsActive = 1
GROUP BY s.SiteType, usa.AccessLevel
ORDER BY s.SiteType, usa.AccessLevel;
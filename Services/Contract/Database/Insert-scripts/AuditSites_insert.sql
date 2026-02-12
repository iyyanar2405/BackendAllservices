-- Insert sample data for AuditSites table
-- This script maps which sites participate in audits

INSERT INTO [dbo].[AuditSites] 
([AuditSiteId], [SiteId], [ServiceId], [IsActive], [IsMainSite], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation audit sites
(NEWID(), 1, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Acme HQ - ISO 9001 (main site)
(NEWID(), 2, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Acme Manufacturing - ISO 9001
(NEWID(), 3, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Acme Warehouse - ISO 9001
(NEWID(), 1, 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Acme HQ - ISO 14001 (main site)
(NEWID(), 2, 2, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Acme Manufacturing - ISO 14001
(NEWID(), 1, 3, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Acme HQ - ISO 45001 (main site)
(NEWID(), 2, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Acme Manufacturing - ISO 45001
(NEWID(), 3, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Acme Warehouse - ISO 45001

-- TechFlow Industries audit sites
(NEWID(), 4, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- TechFlow Main Office - ISO 9001 (main site)
(NEWID(), 5, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- TechFlow Data Center - ISO 9001
(NEWID(), 6, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- TechFlow Dev Lab - ISO 9001
(NEWID(), 4, 4, 1, 1, GETDATE(), GETDATE(), 1, 1), -- TechFlow Main Office - ISO 27001 (main site)
(NEWID(), 5, 4, 1, 0, GETDATE(), GETDATE(), 1, 1), -- TechFlow Data Center - ISO 27001
(NEWID(), 4, 10, 1, 1, GETDATE(), GETDATE(), 1, 1), -- TechFlow Main Office - ISO 20000-1 (main site)
(NEWID(), 5, 10, 1, 0, GETDATE(), GETDATE(), 1, 1), -- TechFlow Data Center - ISO 20000-1
(NEWID(), 4, 14, 1, 1, GETDATE(), GETDATE(), 1, 1), -- TechFlow Main Office - GDPR (main site)

-- Green Energy Solutions audit sites
(NEWID(), 7, 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Green Energy HQ - ISO 14001 (main site)
(NEWID(), 8, 2, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Wind Farm - ISO 14001
(NEWID(), 9, 2, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Solar Installation - ISO 14001
(NEWID(), 7, 3, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Green Energy HQ - ISO 45001 (main site)
(NEWID(), 8, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Wind Farm - ISO 45001
(NEWID(), 7, 7, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Green Energy HQ - ISO 50001 (main site)
(NEWID(), 8, 7, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Wind Farm - ISO 50001
(NEWID(), 9, 7, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Solar Installation - ISO 50001
(NEWID(), 8, 23, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Wind Farm - Wind Turbine Certification (main site)
(NEWID(), 9, 24, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Solar Installation - Solar Panel Certification (main site)
(NEWID(), 7, 26, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Green Energy HQ - Carbon Footprint (main site)
(NEWID(), 8, 26, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Wind Farm - Carbon Footprint
(NEWID(), 9, 26, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Solar Installation - Carbon Footprint

-- Maritime Solutions audit sites
(NEWID(), 10, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Maritime Oslo Office - ISO 9001 (main site)
(NEWID(), 11, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Maritime Shipyard - ISO 9001
(NEWID(), 12, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Maritime Port - ISO 9001
(NEWID(), 10, 3, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Maritime Oslo Office - ISO 45001 (main site)
(NEWID(), 11, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Maritime Shipyard - ISO 45001
(NEWID(), 12, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Maritime Port - ISO 45001
(NEWID(), 11, 22, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Maritime Shipyard - DNV Rules for Ships (main site)
(NEWID(), 12, 22, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Maritime Port - DNV Rules for Ships

-- Food Excellence Corp audit sites
(NEWID(), 13, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Food Paris HQ - ISO 9001 (main site)
(NEWID(), 14, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Food Production Lyon - ISO 9001
(NEWID(), 15, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Food Distribution - ISO 9001
(NEWID(), 13, 5, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Food Paris HQ - ISO 22000 (main site)
(NEWID(), 14, 5, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Food Production Lyon - ISO 22000
(NEWID(), 15, 5, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Food Distribution - ISO 22000
(NEWID(), 14, 28, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Food Production Lyon - HACCP (main site)
(NEWID(), 15, 28, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Food Distribution - HACCP
(NEWID(), 14, 29, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Food Production Lyon - BRC Food Safety (main site)

-- AutoTech Manufacturing audit sites
(NEWID(), 16, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- AutoTech Rome HQ - ISO 9001 (main site)
(NEWID(), 17, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- AutoTech Milan Factory - ISO 9001
(NEWID(), 18, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- AutoTech Turin R&D - ISO 9001
(NEWID(), 16, 8, 1, 1, GETDATE(), GETDATE(), 1, 1), -- AutoTech Rome HQ - IATF 16949 (main site)
(NEWID(), 17, 8, 1, 0, GETDATE(), GETDATE(), 1, 1), -- AutoTech Milan Factory - IATF 16949
(NEWID(), 18, 8, 1, 0, GETDATE(), GETDATE(), 1, 1), -- AutoTech Turin R&D - IATF 16949
(NEWID(), 17, 11, 1, 1, GETDATE(), GETDATE(), 1, 1), -- AutoTech Milan Factory - ISO 26262 (main site)
(NEWID(), 18, 11, 1, 0, GETDATE(), GETDATE(), 1, 1), -- AutoTech Turin R&D - ISO 26262

-- Global Manufacturing Inc audit sites (multi-national)
(NEWID(), 19, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Global US HQ - ISO 9001 (main site)
(NEWID(), 20, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global UK Ops - ISO 9001
(NEWID(), 21, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global DE Plant - ISO 9001
(NEWID(), 22, 1, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global AP Hub - ISO 9001
(NEWID(), 19, 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Global US HQ - ISO 14001 (main site)
(NEWID(), 20, 2, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global UK Ops - ISO 14001
(NEWID(), 21, 2, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global DE Plant - ISO 14001
(NEWID(), 19, 3, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Global US HQ - ISO 45001 (main site)
(NEWID(), 20, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global UK Ops - ISO 45001
(NEWID(), 21, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global DE Plant - ISO 45001
(NEWID(), 22, 3, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Global AP Hub - ISO 45001

-- Additional specialized sites
-- Small Tech Solutions
(NEWID(), 23, 4, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Small Tech Office - ISO 27001 (main site)

-- Family Food Co
(NEWID(), 24, 5, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Family Food Kitchen - ISO 22000 (main site)
(NEWID(), 24, 28, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Family Food Kitchen - HACCP (main site)

-- Dutch Innovation Lab
(NEWID(), 25, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Dutch Innovation Lab - ISO 9001 (main site)
(NEWID(), 25, 4, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Dutch Innovation Lab - ISO 27001 (main site)

-- EuroLogistics Main Hub
(NEWID(), 26, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- EuroLogistics Hub - ISO 9001 (main site)
(NEWID(), 26, 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- EuroLogistics Hub - ISO 14001 (main site)

-- Alpine Precision Factory
(NEWID(), 27, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Alpine Precision - ISO 9001 (main site)
(NEWID(), 27, 16, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Alpine Precision - ISO 17025 (main site)

-- Nordic Forest Mill
(NEWID(), 28, 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Nordic Forest - ISO 14001 (main site)
(NEWID(), 28, 3, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Nordic Forest - ISO 45001 (main site)

-- Danish Wind Offshore Base
(NEWID(), 29, 23, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Danish Wind - Wind Turbine Certification (main site)
(NEWID(), 29, 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Danish Wind - ISO 14001 (main site)

-- Tokyo Precision Main Plant
(NEWID(), 30, 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Tokyo Precision - ISO 9001 (main site)
(NEWID(), 30, 8, 1, 1, GETDATE(), GETDATE(), 1, 1); -- Tokyo Precision - IATF 16949 (main site)

-- Verify the insert
SELECT COUNT(*) as TotalAuditSiteAssignments FROM [dbo].[AuditSites];

-- Show audit sites by service
SELECT 
    s.ServiceName,
    COUNT(aus.SiteId) as SiteCount,
    COUNT(CASE WHEN aus.IsMainSite = 1 THEN 1 END) as MainSites
FROM [dbo].[Services] s
LEFT JOIN [dbo].[AuditSites] aus ON s.ServiceId = aus.ServiceId AND aus.IsActive = 1
GROUP BY s.ServiceId, s.ServiceName
HAVING COUNT(aus.SiteId) > 0
ORDER BY SiteCount DESC;

-- Show sites with their services
SELECT 
    c.CompanyName,
    st.SiteName,
    COUNT(aus.ServiceId) as ServiceCount,
    STRING_AGG(s.ServiceName, ', ') as Services
FROM [dbo].[AuditSites] aus
INNER JOIN [dbo].[Sites] st ON aus.SiteId = st.SiteId
INNER JOIN [dbo].[Companies] c ON st.CompanyId = c.CompanyId
INNER JOIN [dbo].[Services] s ON aus.ServiceId = s.ServiceId
WHERE aus.IsActive = 1
GROUP BY c.CompanyId, c.CompanyName, st.SiteId, st.SiteName
ORDER BY c.CompanyName, st.SiteName;

-- Show main sites vs additional sites
SELECT 
    'Main Sites' as SiteType,
    COUNT(*) as Count
FROM [dbo].[AuditSites] 
WHERE IsActive = 1 AND IsMainSite = 1
UNION ALL
SELECT 
    'Additional Sites' as SiteType,
    COUNT(*) as Count
FROM [dbo].[AuditSites] 
WHERE IsActive = 1 AND IsMainSite = 0;
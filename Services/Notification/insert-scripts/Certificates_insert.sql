-- Insert sample data for Certificates table
-- This script creates certificate records for completed audits

SET IDENTITY_INSERT [dbo].[Certificates] ON;

INSERT INTO [dbo].[Certificates] 
([CertificateId], [CompanyId], [ServiceId], [CertificateNumber], [CertificateTitle], [IssueDate], [ExpiryDate], [Status], [Scope], [Sites], [IssuedBy], [ValidatedBy], [AuditId], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation Certificates
(1, 1, 1, 'DNV-QMS-001-2024', 'ISO 9001:2015 Quality Management System Certificate', '2024-03-20', '2027-03-20', 'Valid', 'Design, manufacture and supply of industrial equipment and components', 'Acme Headquarters, Acme Manufacturing Plant 1, Acme Warehouse Chicago', 17, 4, 1, 1, '2024-03-20', '2024-03-20', 17, 17),
(2, 1, 2, 'DNV-EMS-001-2024', 'ISO 14001:2015 Environmental Management System Certificate', '2024-05-25', '2027-05-25', 'Valid', 'Environmental management for industrial manufacturing operations', 'Acme Headquarters, Acme Manufacturing Plant 1', 17, 4, 3, 1, '2024-05-25', '2024-05-25', 17, 17),

-- TechFlow Industries Certificates
(3, 2, 1, 'DNV-QMS-002-2023', 'ISO 9001:2015 Quality Management System Certificate', '2023-09-10', '2026-09-10', 'Valid', 'Software development and IT service delivery', 'TechFlow Main Office, TechFlow Data Center, TechFlow Development Lab', 17, 5, 6, 1, '2023-09-10', '2023-09-10', 17, 17),
(4, 2, 4, 'DNV-ISMS-001-2024', 'ISO 27001:2013 Information Security Management Certificate', '2024-01-20', '2027-01-20', 'Valid', 'Information security management for software development and data center operations', 'TechFlow Main Office, TechFlow Data Center', 17, 10, 9, 1, '2024-01-20', '2024-01-20', 17, 17),

-- Green Energy Solutions Certificates
(5, 3, 2, 'DNV-EMS-002-2024', 'ISO 14001:2015 Environmental Management System Certificate', '2024-02-17', '2027-02-17', 'Valid', 'Environmental management for renewable energy operations', 'Green Energy Berlin HQ, Green Energy Wind Farm 1, Green Energy Solar Installation', 17, 6, 11, 1, '2024-02-17', '2024-02-17', 17, 17),
(6, 3, 23, 'DNV-WTC-001-2024', 'Wind Turbine Type Certificate', '2024-06-08', '2029-06-08', 'Valid', 'Type certification for offshore wind turbine model GES-5000', 'Green Energy Wind Farm 1', 17, 6, 12, 1, '2024-06-08', '2024-06-08', 17, 17),
(7, 3, 24, 'DNV-SPC-001-2024', 'Solar Panel Installation Certificate', '2024-08-24', '2027-08-24', 'Valid', 'Solar panel installation certification for utility-scale facility', 'Green Energy Solar Installation', 17, 13, 14, 1, '2024-08-24', '2024-08-24', 17, 17),

-- Maritime Solutions Certificates
(8, 4, 1, 'DNV-QMS-003-2023', 'ISO 9001:2015 Quality Management System Certificate', '2023-11-11', '2026-11-11', 'Valid', 'Shipbuilding and marine engineering services', 'Maritime Solutions Oslo Office, Maritime Shipyard Bergen, Maritime Port Facility', 17, 6, 16, 1, '2023-11-11', '2023-11-11', 17, 17),
(9, 4, 22, 'DNV-SHIP-001-2024', 'DNV Rules for Ships Classification Certificate', '2024-04-13', '2029-04-13', 'Valid', 'Ship classification and maritime safety certification', 'Maritime Shipyard Bergen, Maritime Port Facility', 17, 6, 17, 1, '2024-04-13', '2024-04-13', 17, 17),
(10, 4, 3, 'DNV-OHSMS-001-2024', 'ISO 45001:2018 Occupational Health and Safety Certificate', '2024-09-21', '2027-09-21', 'Valid', 'Occupational health and safety management for maritime operations', 'Maritime Solutions Oslo Office, Maritime Shipyard Bergen', 17, 8, 19, 1, '2024-09-21', '2024-09-21', 17, 17),

-- Food Excellence Corp Certificates
(11, 5, 5, 'DNV-FSMS-001-2024', 'ISO 22000:2018 Food Safety Management System Certificate', '2024-01-27', '2027-01-27', 'Valid', 'Food safety management for food production and distribution', 'Food Excellence Paris HQ, Food Production Facility Lyon, Food Distribution Center', 17, 9, 20, 1, '2024-01-27', '2024-01-27', 17, 17),
(12, 5, 28, 'DNV-HACCP-001-2024', 'HACCP Certification', '2024-03-16', '2027-03-16', 'Valid', 'Hazard Analysis and Critical Control Points system', 'Food Production Facility Lyon, Food Distribution Center', 17, 9, 21, 1, '2024-03-16', '2024-03-16', 17, 17),
(13, 5, 29, 'DNV-BRC-001-2024', 'BRC Global Standard for Food Safety Certificate', '2024-07-06', '2025-07-06', 'Valid', 'BRC Global Standard for Food Safety Grade A', 'Food Production Facility Lyon', 17, 9, 22, 1, '2024-07-06', '2024-07-06', 17, 17),

-- AutoTech Manufacturing Certificates
(14, 6, 1, 'DNV-QMS-004-2023', 'ISO 9001:2015 Quality Management System Certificate', '2023-06-17', '2026-06-17', 'Valid', 'Design and manufacture of automotive components and systems', 'AutoTech Rome Headquarters, AutoTech Milan Factory, AutoTech Turin R&D Center', 17, 5, 24, 1, '2023-06-17', '2023-06-17', 17, 17),
(15, 6, 8, 'DNV-IATF-001-2023', 'IATF 16949:2016 Automotive Quality Management Certificate', '2023-10-28', '2026-10-28', 'Valid', 'Automotive quality management system for component manufacturing', 'AutoTech Rome Headquarters, AutoTech Milan Factory, AutoTech Turin R&D Center', 17, 5, 25, 1, '2023-10-28', '2023-10-28', 17, 17),

-- Global Manufacturing Inc Certificates (Multi-site/Multi-national)
(16, 17, 1, 'DNV-QMS-005-2024', 'ISO 9001:2015 Multi-Site Quality Management System Certificate', '2024-05-18', '2027-05-18', 'Valid', 'Global manufacturing operations for industrial equipment across multiple sites', 'Global Mfg US Headquarters, Global Mfg UK Operations, Global Mfg Germany Plant, Global Mfg Asia Pacific', 17, 4, 28, 1, '2024-05-18', '2024-05-18', 17, 17),
(17, 17, 2, 'DNV-EMS-003-2024', 'ISO 14001:2015 Multi-Site Environmental Management Certificate', '2024-08-24', '2027-08-24', 'Valid', 'Global environmental management system for manufacturing operations', 'Global Mfg US Headquarters, Global Mfg UK Operations, Global Mfg Germany Plant, Global Mfg Asia Pacific', 17, 6, 29, 1, '2024-08-24', '2024-08-24', 17, 17),

-- Small Company Certificates
(18, 19, 4, 'DNV-ISMS-002-2024', 'ISO 27001:2013 Information Security Management Certificate', '2024-11-23', '2027-11-23', 'Valid', 'Information security management for software development services', 'Small Tech Office', 17, 12, 31, 1, '2024-11-23', '2024-11-23', 17, 17),
(19, 20, 5, 'DNV-FSMS-002-2025', 'ISO 22000:2018 Food Safety Management System Certificate', '2025-02-22', '2028-02-22', 'Valid', 'Food safety management for artisanal food production', 'Family Food Kitchen', 17, 9, 32, 1, '2025-02-22', '2025-02-22', 17, 17),

-- Certificates with different statuses for variety
(20, 2, 10, 'DNV-ITSM-001-2025', 'ISO 20000-1:2018 IT Service Management Certificate', '2025-11-15', '2028-11-15', 'Pending', 'IT service management and service delivery', 'TechFlow Main Office, TechFlow Data Center', NULL, NULL, NULL, 1, '2025-09-15', '2025-09-15', 17, 17),
(21, 3, 26, 'DNV-CARBON-001-2025', 'Carbon Footprint Verification Certificate', '2025-12-06', '2026-12-06', 'Pending', 'Carbon footprint verification for renewable energy operations', 'Green Energy Berlin HQ, Green Energy Wind Farm 1, Green Energy Solar Installation', NULL, NULL, NULL, 1, '2025-10-06', '2025-10-06', 17, 17),

-- Expired certificate example
(22, 13, 1, 'DNV-QMS-006-2020', 'ISO 9001:2015 Quality Management System Certificate', '2020-05-15', '2023-05-15', 'Expired', 'Precision manufacturing and engineering services', 'Tokyo Precision Main Plant', 17, 5, NULL, 0, '2020-05-15', '2023-05-15', 17, 17),

-- Suspended certificate example
(23, 1, 3, 'DNV-OHSMS-002-2024', 'ISO 45001:2018 Occupational Health and Safety Certificate', '2024-08-01', '2027-08-01', 'Suspended', 'Occupational health and safety management system', 'Acme Headquarters, Acme Manufacturing Plant 1', 17, 8, NULL, 1, '2024-08-01', '2025-01-15', 17, 17);

SET IDENTITY_INSERT [dbo].[Certificates] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalCertificates FROM [dbo].[Certificates];

-- Show certificate status distribution
SELECT 
    Status,
    COUNT(*) as Count
FROM [dbo].[Certificates] 
GROUP BY Status
ORDER BY Count DESC;

-- Show certificates by company
SELECT 
    c.CompanyName,
    COUNT(cert.CertificateId) as CertificateCount,
    COUNT(CASE WHEN cert.Status = 'Valid' THEN 1 END) as ValidCertificates,
    COUNT(CASE WHEN cert.ExpiryDate < GETDATE() THEN 1 END) as ExpiredCertificates
FROM [dbo].[Companies] c
LEFT JOIN [dbo].[Certificates] cert ON c.CompanyId = cert.CompanyId
GROUP BY c.CompanyId, c.CompanyName
HAVING COUNT(cert.CertificateId) > 0
ORDER BY CertificateCount DESC;

-- Show certificates expiring soon (next 6 months)
SELECT 
    cert.CertificateNumber,
    cert.CertificateTitle,
    c.CompanyName,
    s.ServiceName,
    cert.ExpiryDate,
    DATEDIFF(day, GETDATE(), cert.ExpiryDate) as DaysUntilExpiry
FROM [dbo].[Certificates] cert
INNER JOIN [dbo].[Companies] c ON cert.CompanyId = c.CompanyId
INNER JOIN [dbo].[Services] s ON cert.ServiceId = s.ServiceId
WHERE cert.Status = 'Valid' 
AND cert.ExpiryDate BETWEEN GETDATE() AND DATEADD(month, 6, GETDATE())
ORDER BY cert.ExpiryDate;

-- Show certificates by service type
SELECT 
    s.ServiceName,
    COUNT(cert.CertificateId) as CertificateCount,
    COUNT(CASE WHEN cert.Status = 'Valid' THEN 1 END) as ValidCertificates
FROM [dbo].[Services] s
LEFT JOIN [dbo].[Certificates] cert ON s.ServiceId = cert.ServiceId
GROUP BY s.ServiceId, s.ServiceName
HAVING COUNT(cert.CertificateId) > 0
ORDER BY CertificateCount DESC;
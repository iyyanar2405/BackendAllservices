-- Insert sample data for AuditServices table
-- This script links audit types with services

INSERT INTO [dbo].[AuditServices] 
([AuditServiceId], [AuditTypeId], [ServiceId], [IsDefault], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- ISO 9001 audit services
(NEWID(), 1, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 9001
(NEWID(), 2, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 9001
(NEWID(), 3, 1, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 9001
(NEWID(), 4, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Special Audit - ISO 9001
(NEWID(), 6, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Pre-Assessment - ISO 9001
(NEWID(), 7, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Transfer Audit - ISO 9001
(NEWID(), 8, 1, 0, GETDATE(), GETDATE(), 1, 1), -- Extension Audit - ISO 9001

-- ISO 14001 audit services
(NEWID(), 1, 2, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 14001
(NEWID(), 2, 2, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 14001
(NEWID(), 3, 2, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 14001
(NEWID(), 4, 2, 0, GETDATE(), GETDATE(), 1, 1), -- Special Audit - ISO 14001
(NEWID(), 5, 2, 1, GETDATE(), GETDATE(), 1, 1), -- Integrated Audit - ISO 14001 (common with ISO 9001)
(NEWID(), 6, 2, 0, GETDATE(), GETDATE(), 1, 1), -- Pre-Assessment - ISO 14001

-- ISO 45001 audit services
(NEWID(), 1, 3, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 45001
(NEWID(), 2, 3, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 45001
(NEWID(), 3, 3, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 45001
(NEWID(), 5, 3, 1, GETDATE(), GETDATE(), 1, 1), -- Integrated Audit - ISO 45001
(NEWID(), 9, 3, 1, GETDATE(), GETDATE(), 1, 1), -- Extraordinary Audit - ISO 45001 (safety incidents)

-- ISO 27001 audit services
(NEWID(), 1, 4, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 27001
(NEWID(), 2, 4, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 27001
(NEWID(), 3, 4, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 27001
(NEWID(), 4, 4, 1, GETDATE(), GETDATE(), 1, 1), -- Special Audit - ISO 27001 (security incidents)
(NEWID(), 9, 4, 1, GETDATE(), GETDATE(), 1, 1), -- Extraordinary Audit - ISO 27001

-- ISO 22000 audit services (Food Safety)
(NEWID(), 1, 5, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 22000
(NEWID(), 2, 5, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 22000
(NEWID(), 3, 5, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 22000
(NEWID(), 9, 5, 1, GETDATE(), GETDATE(), 1, 1), -- Extraordinary Audit - ISO 22000 (food safety incidents)

-- ISO 13485 audit services (Medical Devices)
(NEWID(), 1, 6, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 13485
(NEWID(), 2, 6, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 13485
(NEWID(), 3, 6, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 13485
(NEWID(), 10, 6, 1, GETDATE(), GETDATE(), 1, 1), -- Witness Audit - ISO 13485

-- ISO 50001 audit services (Energy Management)
(NEWID(), 1, 7, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 50001
(NEWID(), 2, 7, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 50001
(NEWID(), 3, 7, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 50001

-- IATF 16949 audit services (Automotive)
(NEWID(), 1, 8, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - IATF 16949
(NEWID(), 2, 8, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - IATF 16949
(NEWID(), 3, 8, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - IATF 16949
(NEWID(), 4, 8, 1, GETDATE(), GETDATE(), 1, 1), -- Special Audit - IATF 16949

-- AS9100 audit services (Aerospace)
(NEWID(), 1, 9, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - AS9100
(NEWID(), 2, 9, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - AS9100
(NEWID(), 3, 9, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - AS9100

-- ISO 20000-1 audit services (IT Service Management)
(NEWID(), 1, 10, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - ISO 20000-1
(NEWID(), 2, 10, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - ISO 20000-1
(NEWID(), 3, 10, 1, GETDATE(), GETDATE(), 1, 1), -- Recertification Audit - ISO 20000-1

-- Specialized services (selected audit types only)
-- Wind Turbine Certification
(NEWID(), 1, 23, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - Wind Turbine
(NEWID(), 2, 23, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - Wind Turbine
(NEWID(), 10, 23, 1, GETDATE(), GETDATE(), 1, 1), -- Witness Audit - Wind Turbine

-- Solar Panel Certification
(NEWID(), 1, 24, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - Solar Panel
(NEWID(), 2, 24, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - Solar Panel

-- DNV Rules for Ships
(NEWID(), 1, 22, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - Ships
(NEWID(), 2, 22, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - Ships
(NEWID(), 10, 22, 1, GETDATE(), GETDATE(), 1, 1), -- Witness Audit - Ships

-- API Standards
(NEWID(), 1, 21, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - API
(NEWID(), 2, 21, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - API

-- HACCP Certification
(NEWID(), 1, 28, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - HACCP
(NEWID(), 2, 28, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - HACCP
(NEWID(), 9, 28, 1, GETDATE(), GETDATE(), 1, 1), -- Extraordinary Audit - HACCP

-- BRC Food Safety
(NEWID(), 1, 29, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - BRC
(NEWID(), 2, 29, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - BRC

-- Cyber Security Assessment
(NEWID(), 1, 25, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - Cyber Security
(NEWID(), 4, 25, 1, GETDATE(), GETDATE(), 1, 1), -- Special Audit - Cyber Security
(NEWID(), 9, 25, 1, GETDATE(), GETDATE(), 1, 1), -- Extraordinary Audit - Cyber Security

-- Carbon Footprint Verification
(NEWID(), 1, 26, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - Carbon Footprint
(NEWID(), 2, 26, 1, GETDATE(), GETDATE(), 1, 1), -- Surveillance Audit - Carbon Footprint

-- GDPR Compliance
(NEWID(), 1, 14, 1, GETDATE(), GETDATE(), 1, 1), -- Initial Certification Audit - GDPR
(NEWID(), 4, 14, 1, GETDATE(), GETDATE(), 1, 1), -- Special Audit - GDPR
(NEWID(), 9, 14, 1, GETDATE(), GETDATE(), 1, 1); -- Extraordinary Audit - GDPR

-- Verify the insert
SELECT COUNT(*) as TotalAuditServiceMappings FROM [dbo].[AuditServices];

-- Show audit services by type
SELECT 
    at.TypeName,
    COUNT(aus.ServiceId) as ServiceCount,
    COUNT(CASE WHEN aus.IsDefault = 1 THEN 1 END) as DefaultServices
FROM [dbo].[AuditTypes] at
LEFT JOIN [dbo].[AuditServices] aus ON at.AuditTypeId = aus.AuditTypeId
GROUP BY at.AuditTypeId, at.TypeName
ORDER BY ServiceCount DESC;

-- Show services by audit type
SELECT 
    s.ServiceName,
    COUNT(aus.AuditTypeId) as AuditTypeCount,
    STRING_AGG(at.TypeName, ', ') as AvailableAuditTypes
FROM [dbo].[Services] s
LEFT JOIN [dbo].[AuditServices] aus ON s.ServiceId = aus.ServiceId
LEFT JOIN [dbo].[AuditTypes] at ON aus.AuditTypeId = at.AuditTypeId
GROUP BY s.ServiceId, s.ServiceName
HAVING COUNT(aus.AuditTypeId) > 0
ORDER BY AuditTypeCount DESC;
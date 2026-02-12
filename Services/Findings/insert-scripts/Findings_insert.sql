-- Insert sample data for Findings table
-- This script creates audit findings for various audits

SET IDENTITY_INSERT [dbo].[Findings] ON;

INSERT INTO [dbo].[Findings] 
([FindingId], [AuditId], [SiteId], [FindingNumber], [FindingTitle], [FindingDescription], [CategoryId], [StatusId], [Severity], [ClauseReference], [RaisedDate], [DueDate], [RaisedBy], [AssignedTo], [RootCause], [CorrectiveAction], [PreventiveAction], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation Findings (ISO 9001 Initial Certification)
(1, 1, 1, 'ACM-2024-001', 'Inadequate Document Control Procedure', 'The document control procedure does not clearly define the process for reviewing and approving quality manual changes', 2, 5, 'Minor', 'ISO 9001:2015 - 7.5.3', '2024-03-16', '2024-04-15', 4, 21, 'Procedure was written without clear approval workflow', 'Updated document control procedure with clear approval matrix and workflow', 'Implemented quarterly review of all procedures', 1, '2024-03-16', '2024-04-20', 4, 21),
(2, 1, 2, 'ACM-2024-002', 'Missing Calibration Records', 'Calibration records for measuring equipment in production line 2 are not available for the last 6 months', 1, 5, 'Major', 'ISO 9001:2015 - 7.1.5', '2024-03-17', '2024-04-17', 4, 22, 'Calibration tracking system was not properly maintained', 'Established new calibration tracking system and performed emergency calibrations', 'Implemented automated calibration reminders and monthly verification', 1, '2024-03-17', '2024-05-01', 4, 22),
(3, 1, 1, 'ACM-2024-003', 'Customer Complaint Analysis', 'Analysis of customer complaints lacks depth in root cause identification', 4, 5, 'Low', 'ISO 9001:2015 - 9.1.2', '2024-03-18', '2024-05-18', 7, 21, 'Staff not trained in root cause analysis techniques', 'Provided training on 5-Why analysis and fishbone diagrams', 'Scheduled annual refresher training on problem-solving techniques', 1, '2024-03-18', '2024-05-25', 7, 21),

-- TechFlow Industries Findings (ISO 27001 Initial)
(4, 9, 4, 'TFL-2024-001', 'Incomplete Risk Assessment', 'Information security risk assessment does not cover all identified assets', 1, 5, 'Major', 'ISO 27001:2013 - A.12.6', '2024-01-16', '2024-02-16', 10, 23, 'Asset inventory was incomplete when risk assessment was conducted', 'Completed comprehensive asset inventory and updated risk assessment', 'Implemented quarterly asset inventory reviews', 1, '2024-01-16', '2024-02-20', 10, 23),
(5, 9, 5, 'TFL-2024-002', 'Access Control Documentation', 'User access rights documentation is not consistently maintained', 2, 5, 'Minor', 'ISO 27001:2013 - A.9.2', '2024-01-17', '2024-03-17', 10, 24, 'Manual process prone to human error', 'Implemented automated access control documentation system', 'Established monthly access rights reviews', 1, '2024-01-17', '2024-03-15', 10, 24),

-- Green Energy Solutions Findings (Wind Turbine Certification)
(6, 12, 8, 'GES-2024-001', 'Safety System Testing Incomplete', 'Emergency shutdown system testing records are incomplete for turbine units 5-8', 1, 5, 'Major', 'IEC 61400-1 - 8.3', '2024-06-04', '2024-07-04', 6, 25, 'Testing schedule was not properly coordinated with maintenance', 'Completed all missing safety system tests and updated records', 'Implemented integrated testing and maintenance scheduling system', 1, '2024-06-04', '2024-07-10', 6, 25),
(7, 12, 8, 'GES-2024-002', 'Environmental Monitoring Gaps', 'Bird migration monitoring data has gaps during key migration periods', 2, 5, 'Minor', 'Environmental Impact Assessment', '2024-06-05', '2024-08-05', 13, 26, 'Monitoring equipment malfunction during migration season', 'Installed redundant monitoring equipment and filled data gaps', 'Established backup monitoring protocols and quarterly equipment checks', 1, '2024-06-05', '2024-08-15', 13, 26),

-- Maritime Solutions Findings (DNV Rules for Ships)
(8, 17, 11, 'MAR-2024-001', 'Welding Procedure Qualification', 'Welding procedure qualification records for hull section assembly are outdated', 1, 5, 'Major', 'DNV Rules Part 2 Ch.3', '2024-04-09', '2024-05-09', 6, 27, 'Procedures not updated when new welding materials were introduced', 'Updated and re-qualified all welding procedures with new materials', 'Implemented procedure review process for any material changes', 1, '2024-04-09', '2024-05-15', 6, 27),
(9, 17, 12, 'MAR-2024-002', 'Load Testing Documentation', 'Load testing documentation for crane operations is incomplete', 2, 5, 'Minor', 'DNV Rules Part 4 Ch.5', '2024-04-10', '2024-06-10', 14, 28, 'Testing was performed but documentation not properly filed', 'Completed missing documentation and established filing protocol', 'Implemented digital documentation system with automatic filing', 1, '2024-04-10', '2024-06-05', 14, 28),

-- Food Excellence Corp Findings (HACCP)
(10, 21, 14, 'FEC-2024-001', 'Temperature Monitoring Gaps', 'Cold storage temperature monitoring shows gaps in data logging', 1, 5, 'Major', 'HACCP Principle 4 - Monitoring', '2024-03-12', '2024-04-12', 9, 29, 'Data logger battery failure not detected promptly', 'Replaced data loggers and implemented battery monitoring alerts', 'Established daily verification of all monitoring systems', 1, '2024-03-12', '2024-04-18', 9, 29),
(11, 21, 15, 'FEC-2024-002', 'Allergen Control Procedure', 'Allergen control procedure for distribution center needs clarification', 2, 5, 'Minor', 'HACCP Principle 2 - CCP Identification', '2024-03-13', '2024-05-13', 9, 30, 'Procedure written ambiguously causing confusion', 'Rewrote procedure with clear step-by-step instructions and visual aids', 'Scheduled annual procedure review with staff feedback', 1, '2024-03-13', '2024-05-10', 9, 30),

-- AutoTech Manufacturing Findings (IATF 16949 Surveillance)
(12, 26, 17, 'AUTO-2024-001', 'Statistical Process Control', 'SPC charts for critical dimensions are not being maintained consistently', 1, 5, 'Major', 'IATF 16949:2016 - 9.1.1', '2024-10-22', '2024-11-22', 11, NULL, 'Operators not trained on SPC chart maintenance', 'Provided comprehensive SPC training and updated work instructions', 'Implemented daily SPC chart reviews and monthly operator assessments', 1, '2024-10-22', '2024-11-28', 11, NULL),

-- Global Manufacturing Inc Findings (Multi-site ISO 14001)
(13, 29, 19, 'GLOB-2024-001', 'Waste Segregation Procedures', 'Hazardous waste segregation procedures vary between sites', 6, 5, 'Major', 'ISO 14001:2015 - 8.1', '2024-08-13', '2024-09-13', 6, NULL, 'Each site developed procedures independently without coordination', 'Developed standardized global waste segregation procedures', 'Established annual multi-site procedure harmonization reviews', 1, '2024-08-13', '2024-09-20', 6, NULL),
(14, 29, 21, 'GLOB-2024-002', 'Environmental Objectives Tracking', 'Environmental objectives tracking system needs enhancement', 4, 5, 'Low', 'ISO 14001:2015 - 6.2', '2024-08-19', '2024-10-19', 8, NULL, 'Tracking system did not provide adequate visibility', 'Implemented dashboard system for real-time objectives tracking', 'Scheduled quarterly objectives review meetings', 1, '2024-08-19', '2024-10-25', 8, NULL),

-- Recent findings still in progress
(15, 4, 1, 'ACM-2025-001', 'Management Review Frequency', 'Management review meetings scheduled quarterly but only held twice in past year', 2, 2, 'Minor', 'ISO 14001:2015 - 9.3', '2025-05-16', '2025-06-16', 7, 21, NULL, NULL, NULL, 1, '2025-05-16', '2025-05-16', 7, 7),
(16, 8, 4, 'TFL-2025-001', 'Change Management Process', 'IT change management process documentation needs updating', 3, 1, 'Low', 'ISO 9001:2015 - 8.5.6', '2025-09-02', '2025-10-02', 12, 23, NULL, NULL, NULL, 1, '2025-09-02', '2025-09-02', 12, 12),

-- Overdue finding example
(17, 27, 17, 'AUTO-2024-002', 'Supplier Quality Agreement', 'Quality agreements with tier 1 suppliers missing required clauses', 1, 6, 'Major', 'IATF 16949:2016 - 8.4.2', '2024-12-03', '2025-01-03', 5, NULL, NULL, NULL, NULL, 1, '2024-12-03', '2024-12-03', 5, 5),

-- Disputed finding example
(18, 13, 8, 'GES-2025-001', 'Noise Level Monitoring', 'Noise level monitoring frequency should be increased during peak wind periods', 4, 8, 'Low', 'Environmental Permit Conditions', '2025-06-03', '2025-07-03', 13, 25, NULL, NULL, NULL, 1, '2025-06-03', '2025-06-03', 13, 13),

-- Critical finding example
(19, 18, 11, 'MAR-2025-001', 'Safety Equipment Inspection', 'Life saving equipment inspection records show overdue inspections', 5, 1, 'Critical', 'SOLAS Convention Ch.III', '2025-04-08', '2025-04-15', 14, 27, NULL, NULL, NULL, 1, '2025-04-08', '2025-04-08', 14, 14),

-- Extended deadline finding
(20, 32, 24, 'FAM-2025-001', 'HACCP Plan Review', 'HACCP plan annual review is 3 months overdue', 2, 7, 'Minor', 'HACCP Principle 7 - Verification', '2025-02-18', '2025-03-18', 9, NULL, NULL, NULL, NULL, 1, '2025-02-18', '2025-04-01', 9, 9);

SET IDENTITY_INSERT [dbo].[Findings] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalFindings FROM [dbo].[Findings];

-- Show findings by status
SELECT 
    fs.StatusName,
    COUNT(f.FindingId) as Count
FROM [dbo].[FindingStatuses] fs
LEFT JOIN [dbo].[Findings] f ON fs.StatusId = f.StatusId
GROUP BY fs.StatusId, fs.StatusName, fs.SortOrder
ORDER BY fs.SortOrder;

-- Show findings by severity
SELECT 
    Severity,
    COUNT(*) as Count
FROM [dbo].[Findings] 
GROUP BY Severity
ORDER BY 
    CASE Severity 
        WHEN 'Critical' THEN 1 
        WHEN 'Major' THEN 2 
        WHEN 'Minor' THEN 3 
        WHEN 'Medium' THEN 4 
        WHEN 'Low' THEN 5 
    END;

-- Show findings by company
SELECT 
    c.CompanyName,
    COUNT(f.FindingId) as TotalFindings,
    COUNT(CASE WHEN f.StatusId = 5 THEN 1 END) as ClosedFindings,
    COUNT(CASE WHEN f.StatusId IN (1,2,3) THEN 1 END) as OpenFindings,
    COUNT(CASE WHEN f.StatusId = 6 THEN 1 END) as OverdueFindings
FROM [dbo].[Companies] c
INNER JOIN [dbo].[Audits] a ON c.CompanyId = a.CompanyId
LEFT JOIN [dbo].[Findings] f ON a.AuditId = f.AuditId
GROUP BY c.CompanyId, c.CompanyName
HAVING COUNT(f.FindingId) > 0
ORDER BY TotalFindings DESC;

-- Show overdue findings
SELECT 
    f.FindingNumber,
    f.FindingTitle,
    c.CompanyName,
    s.SiteName,
    f.Severity,
    f.DueDate,
    DATEDIFF(day, f.DueDate, GETDATE()) as DaysOverdue
FROM [dbo].[Findings] f
INNER JOIN [dbo].[Audits] a ON f.AuditId = a.AuditId
INNER JOIN [dbo].[Companies] c ON a.CompanyId = c.CompanyId
INNER JOIN [dbo].[Sites] s ON f.SiteId = s.SiteId
WHERE f.StatusId = 6 OR (f.DueDate < GETDATE() AND f.StatusId NOT IN (5))
ORDER BY DaysOverdue DESC;

-- Show findings by category
SELECT 
    fc.CategoryName,
    fc.Severity as CategorySeverity,
    COUNT(f.FindingId) as Count
FROM [dbo].[FindingCategories] fc
LEFT JOIN [dbo].[Findings] f ON fc.CategoryId = f.CategoryId
GROUP BY fc.CategoryId, fc.CategoryName, fc.Severity
ORDER BY Count DESC;
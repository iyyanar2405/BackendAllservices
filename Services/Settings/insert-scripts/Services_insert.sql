-- Insert sample data for Services table
-- This script provides common certification and assessment services

SET IDENTITY_INSERT [dbo].[Services] ON;

INSERT INTO [dbo].[Services] 
([ServiceId], [ServiceName], [ServiceCode], [Description], [ServiceType], [Category], [Duration], [Cost], [Currency], [Prerequisites], [ValidityPeriod], [DisplayOrder], [IsActive], [CreatedDate], [ModifiedDate])
VALUES
-- ISO Management System Certifications
(1, 'ISO 9001 Quality Management System', 'ISO9001', 'Certification for Quality Management Systems according to ISO 9001 standard', 'Certification', 'Quality Management', 5, 15000.00, 'USD', 'Implemented QMS for at least 3 months', 36, 1, 1, GETDATE(), GETDATE()),
(2, 'ISO 14001 Environmental Management System', 'ISO14001', 'Certification for Environmental Management Systems according to ISO 14001 standard', 'Certification', 'Environmental Management', 5, 12000.00, 'USD', 'Implemented EMS for at least 3 months', 36, 2, 1, GETDATE(), GETDATE()),
(3, 'ISO 45001 Occupational Health and Safety', 'ISO45001', 'Certification for Occupational Health and Safety Management Systems', 'Certification', 'Health & Safety', 5, 13000.00, 'USD', 'Implemented OHSMS for at least 3 months', 36, 3, 1, GETDATE(), GETDATE()),
(4, 'ISO 27001 Information Security Management', 'ISO27001', 'Certification for Information Security Management Systems', 'Certification', 'Information Security', 7, 18000.00, 'USD', 'Implemented ISMS for at least 3 months', 36, 4, 1, GETDATE(), GETDATE()),
(5, 'ISO 50001 Energy Management System', 'ISO50001', 'Certification for Energy Management Systems according to ISO 50001', 'Certification', 'Energy Management', 4, 10000.00, 'USD', 'Energy baseline established', 36, 5, 1, GETDATE(), GETDATE()),

-- Food Safety and HACCP
(6, 'ISO 22000 Food Safety Management', 'ISO22000', 'Food Safety Management System certification', 'Certification', 'Food Safety', 5, 14000.00, 'USD', 'HACCP implementation', 36, 6, 1, GETDATE(), GETDATE()),
(7, 'HACCP Hazard Analysis', 'HACCP', 'Hazard Analysis and Critical Control Points certification', 'Certification', 'Food Safety', 3, 8000.00, 'USD', 'Food safety team established', 36, 7, 1, GETDATE(), GETDATE()),
(8, 'BRC Global Standard for Food Safety', 'BRC', 'British Retail Consortium Global Standard for Food Safety', 'Certification', 'Food Safety', 4, 12000.00, 'USD', 'HACCP system in place', 12, 8, 1, GETDATE(), GETDATE()),
(9, 'FSSC 22000 Food Safety System', 'FSSC22000', 'Food Safety System Certification based on ISO 22000', 'Certification', 'Food Safety', 5, 16000.00, 'USD', 'ISO 22000 prerequisite', 36, 9, 1, GETDATE(), GETDATE()),

-- Automotive and Manufacturing
(10, 'ISO/TS 16949 Automotive Quality', 'IATF16949', 'Automotive Quality Management System certification', 'Certification', 'Automotive', 6, 20000.00, 'USD', 'ISO 9001 certification required', 36, 10, 1, GETDATE(), GETDATE()),
(11, 'AS9100 Aerospace Quality Management', 'AS9100', 'Aerospace Quality Management System certification', 'Certification', 'Aerospace', 6, 22000.00, 'USD', 'ISO 9001 certification required', 36, 11, 1, GETDATE(), GETDATE()),
(12, 'ISO 13485 Medical Devices Quality', 'ISO13485', 'Medical Devices Quality Management System certification', 'Certification', 'Medical Devices', 6, 18000.00, 'USD', 'Medical device manufacturing', 36, 12, 1, GETDATE(), GETDATE()),

-- Maritime and Oil & Gas
(13, 'ISO 29001 Oil and Gas Quality', 'ISO29001', 'Quality Management Systems for Oil and Gas Industry', 'Certification', 'Oil & Gas', 6, 25000.00, 'USD', 'Oil & Gas sector experience', 36, 13, 1, GETDATE(), GETDATE()),
(14, 'MLC Maritime Labour Convention', 'MLC', 'Maritime Labour Convention certification for shipping', 'Certification', 'Maritime', 3, 15000.00, 'USD', 'Valid ship registration', 60, 14, 1, GETDATE(), GETDATE()),
(15, 'ISM International Safety Management', 'ISM', 'International Safety Management Code certification', 'Certification', 'Maritime', 4, 12000.00, 'USD', 'Safety management system', 60, 15, 1, GETDATE(), GETDATE()),

-- Business Continuity and Risk Management
(16, 'ISO 22301 Business Continuity Management', 'ISO22301', 'Business Continuity Management System certification', 'Certification', 'Business Continuity', 4, 14000.00, 'USD', 'Business impact analysis completed', 36, 16, 1, GETDATE(), GETDATE()),
(17, 'ISO 31000 Risk Management', 'ISO31000', 'Risk Management principles and guidelines assessment', 'Assessment', 'Risk Management', 3, 8000.00, 'USD', 'Risk register established', 24, 17, 1, GETDATE(), GETDATE()),

-- Sustainability and Social Responsibility
(18, 'ISO 26000 Social Responsibility', 'ISO26000', 'Social Responsibility guidance and assessment', 'Assessment', 'Social Responsibility', 3, 10000.00, 'USD', 'Stakeholder engagement process', 24, 18, 1, GETDATE(), GETDATE()),
(19, 'OHSAS 18001 Health and Safety (Legacy)', 'OHSAS18001', 'Legacy Occupational Health and Safety Assessment Series', 'Certification', 'Health & Safety', 4, 11000.00, 'USD', 'H&S management system', 36, 19, 0, GETDATE(), GETDATE()),

-- Training Services
(20, 'ISO 9001 Lead Auditor Training', 'LA9001', 'Lead Auditor training course for ISO 9001', 'Training', 'Quality Management', 5, 2500.00, 'USD', 'Basic QMS knowledge', 36, 20, 1, GETDATE(), GETDATE()),
(21, 'ISO 14001 Lead Auditor Training', 'LA14001', 'Lead Auditor training course for ISO 14001', 'Training', 'Environmental Management', 5, 2500.00, 'USD', 'Basic EMS knowledge', 36, 21, 1, GETDATE(), GETDATE()),
(22, 'ISO 45001 Lead Auditor Training', 'LA45001', 'Lead Auditor training course for ISO 45001', 'Training', 'Health & Safety', 5, 2500.00, 'USD', 'Basic OHSMS knowledge', 36, 22, 1, GETDATE(), GETDATE()),
(23, 'Internal Auditor Training - QMS', 'IA-QMS', 'Internal Auditor training for Quality Management Systems', 'Training', 'Quality Management', 2, 1200.00, 'USD', 'QMS awareness', 24, 23, 1, GETDATE(), GETDATE()),
(24, 'Root Cause Analysis Training', 'RCA-TRAIN', 'Training on Root Cause Analysis methodologies', 'Training', 'Quality Management', 2, 1500.00, 'USD', 'Basic problem-solving knowledge', 24, 24, 1, GETDATE(), GETDATE()),

-- Consulting Services
(25, 'ISO Implementation Consulting', 'ISO-CONSULT', 'Consulting services for ISO standard implementation', 'Consulting', 'Management Systems', 30, 50000.00, 'USD', 'Management commitment', 12, 25, 1, GETDATE(), GETDATE()),
(26, 'Gap Analysis Assessment', 'GAP-ASSESS', 'Gap analysis against specific standards', 'Assessment', 'Management Systems', 2, 5000.00, 'USD', 'Current system documentation', 12, 26, 1, GETDATE(), GETDATE()),
(27, 'Pre-Assessment Audit', 'PRE-AUDIT', 'Pre-assessment audit before certification', 'Assessment', 'Management Systems', 2, 7000.00, 'USD', 'System implementation completed', 6, 27, 1, GETDATE(), GETDATE()),
(28, 'Surveillance Audit', 'SURVEILLANCE', 'Annual surveillance audit for certified organizations', 'Assessment', 'Management Systems', 2, 6000.00, 'USD', 'Valid certification', 12, 28, 1, GETDATE(), GETDATE()),
(29, 'Recertification Audit', 'RECERT', 'Recertification audit for certificate renewal', 'Assessment', 'Management Systems', 4, 12000.00, 'USD', 'Expiring certification', 36, 29, 1, GETDATE(), GETDATE()),
(30, 'Management System Review', 'MSR', 'Comprehensive management system review service', 'Assessment', 'Management Systems', 3, 8000.00, 'USD', 'Implemented management system', 12, 30, 1, GETDATE(), GETDATE());

SET IDENTITY_INSERT [dbo].[Services] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalServices FROM [dbo].[Services];
SELECT ServiceType, COUNT(*) as Count FROM [dbo].[Services] WHERE IsActive = 1 GROUP BY ServiceType ORDER BY Count DESC;
SELECT TOP 10 ServiceName, ServiceCode, Category, Cost FROM [dbo].[Services] ORDER BY DisplayOrder;
-- Insert sample data for AuditTypes table
-- This script creates different types of audits

SET IDENTITY_INSERT [dbo].[AuditTypes] ON;

INSERT INTO [dbo].[AuditTypes] 
([AuditTypeId], [TypeName], [TypeCode], [Description], [StandardDuration], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Initial Certification Audit', 'ICA', 'First-time certification audit for new clients', 5, 1, GETDATE(), GETDATE(), 1, 1),
(2, 'Surveillance Audit', 'SA', 'Annual surveillance audit to maintain certification', 2, 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Recertification Audit', 'RCA', 'Three-year recertification audit to renew certificate', 4, 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Special Audit', 'SPA', 'Special audit due to significant changes or issues', 3, 1, GETDATE(), GETDATE(), 1, 1),
(5, 'Integrated Audit', 'IA', 'Combined audit for multiple management systems', 6, 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Pre-Assessment', 'PA', 'Optional preliminary assessment before certification', 2, 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Transfer Audit', 'TA', 'Audit for transferring certification from another body', 3, 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Extension Audit', 'EA', 'Audit for extending scope of existing certification', 2, 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Extraordinary Audit', 'EXA', 'Unplanned audit due to complaints or incidents', 2, 1, GETDATE(), GETDATE(), 1, 1),
(10, 'Witness Audit', 'WA', 'Audit to witness client processes and procedures', 1, 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[AuditTypes] OFF;

-- Verify the insert
SELECT * FROM [dbo].[AuditTypes] ORDER BY AuditTypeId;
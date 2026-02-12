-- Insert sample data for Roles table
-- This script creates role hierarchy for access control

SET IDENTITY_INSERT [dbo].[Roles] ON;

INSERT INTO [dbo].[Roles] 
([RoleId], [RoleName], [RoleDescription], [Permissions], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- DNV Admin Roles
(1, 'DNV Super Admin', 'Global DNV administrator with full system access', 'ALL', 1, GETDATE(), GETDATE(), 1, 1),
(2, 'DNV Admin', 'DNV administrator with broad system access', 'CREATE,READ,UPDATE,DELETE,APPROVE,AUDIT', 1, GETDATE(), GETDATE(), 1, 1),
(3, 'DNV Regional Manager', 'DNV regional operations manager', 'READ,UPDATE,APPROVE,AUDIT,SCHEDULE', 1, GETDATE(), GETDATE(), 1, 1),

-- DNV Operational Roles
(4, 'Lead Auditor', 'Senior auditor who leads audit teams', 'READ,UPDATE,AUDIT,SCHEDULE,APPROVE', 1, GETDATE(), GETDATE(), 1, 1),
(5, 'Senior Auditor', 'Experienced auditor with advanced permissions', 'READ,UPDATE,AUDIT,SCHEDULE', 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Auditor', 'Standard auditor role for conducting audits', 'READ,UPDATE,AUDIT', 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Junior Auditor', 'Entry-level auditor with basic permissions', 'READ,AUDIT', 1, GETDATE(), GETDATE(), 1, 1),

-- DNV Support Roles
(8, 'Audit Coordinator', 'Coordinates audit scheduling and logistics', 'READ,UPDATE,SCHEDULE', 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Certification Officer', 'Manages certificate issuance and maintenance', 'READ,UPDATE,CERTIFICATE', 1, GETDATE(), GETDATE(), 1, 1),
(10, 'Finance Officer', 'Handles financial operations and invoicing', 'READ,UPDATE,FINANCE', 1, GETDATE(), GETDATE(), 1, 1),
(11, 'Customer Service Rep', 'Provides customer support and assistance', 'READ,SUPPORT', 1, GETDATE(), GETDATE(), 1, 1),

-- Customer Company Roles
(12, 'Company Admin', 'Customer company administrator', 'READ,UPDATE,MANAGE_USERS', 1, GETDATE(), GETDATE(), 1, 1),
(13, 'Site Manager', 'Manages a specific customer site', 'READ,UPDATE,RESPOND', 1, GETDATE(), GETDATE(), 1, 1),
(14, 'Quality Manager', 'Quality management system administrator', 'READ,UPDATE,RESPOND,QUALITY', 1, GETDATE(), GETDATE(), 1, 1),
(15, 'HSE Manager', 'Health, Safety, and Environment manager', 'READ,UPDATE,RESPOND,HSE', 1, GETDATE(), GETDATE(), 1, 1),
(16, 'Technical Manager', 'Technical operations manager', 'READ,UPDATE,RESPOND,TECHNICAL', 1, GETDATE(), GETDATE(), 1, 1),

-- Customer Operational Roles  
(17, 'Site Representative', 'On-site representative during audits', 'READ,RESPOND', 1, GETDATE(), GETDATE(), 1, 1),
(18, 'Document Controller', 'Manages documentation and records', 'READ,UPDATE,DOCUMENT', 1, GETDATE(), GETDATE(), 1, 1),
(19, 'Management Representative', 'Company representative for management system', 'READ,UPDATE,RESPOND,APPROVE', 1, GETDATE(), GETDATE(), 1, 1),
(20, 'Training Coordinator', 'Manages training and competency records', 'READ,UPDATE,TRAINING', 1, GETDATE(), GETDATE(), 1, 1),

-- General User Roles
(21, 'Read Only User', 'Can only view assigned information', 'READ', 1, GETDATE(), GETDATE(), 1, 1),
(22, 'Standard User', 'Basic user with read and limited update access', 'READ,UPDATE', 1, GETDATE(), GETDATE(), 1, 1),

-- System Roles
(23, 'System Administrator', 'Technical system administrator', 'SYSTEM_ADMIN', 1, GETDATE(), GETDATE(), 1, 1),
(24, 'Report Viewer', 'Can access reports and dashboards', 'READ,REPORT', 1, GETDATE(), GETDATE(), 1, 1),
(25, 'Guest User', 'Limited guest access for temporary users', 'READ_LIMITED', 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[Roles] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalRoles FROM [dbo].[Roles];
SELECT RoleName, RoleDescription, Permissions FROM [dbo].[Roles] ORDER BY RoleId;

-- Show role hierarchy by permissions
SELECT 
    CASE 
        WHEN Permissions = 'ALL' THEN 'Administrative'
        WHEN Permissions LIKE '%AUDIT%' THEN 'Audit Operations'
        WHEN Permissions LIKE '%RESPOND%' THEN 'Customer Operations'
        WHEN Permissions LIKE '%SYSTEM%' THEN 'System'
        ELSE 'Support/Specialized'
    END as RoleCategory,
    COUNT(*) as RoleCount
FROM [dbo].[Roles] 
GROUP BY 
    CASE 
        WHEN Permissions = 'ALL' THEN 'Administrative'
        WHEN Permissions LIKE '%AUDIT%' THEN 'Audit Operations'
        WHEN Permissions LIKE '%RESPOND%' THEN 'Customer Operations'
        WHEN Permissions LIKE '%SYSTEM%' THEN 'System'
        ELSE 'Support/Specialized'
    END
ORDER BY RoleCount DESC;
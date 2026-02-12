-- Insert sample data for Roles table
-- This script creates role hierarchy for access control

SET IDENTITY_INSERT [dbo].[Roles] ON;

INSERT INTO [dbo].[Roles] 
([RoleId], [RoleName], [RoleCode], [Description], [Permissions], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy], [IsSystemRole])
VALUES
-- DNV Admin Roles
(1, 'DNV Super Admin', 'DNV_SUPER_ADMIN', 'Global DNV administrator with full system access', 'ALL', 1, GETDATE(), GETDATE(), 1, 1, 1),
(2, 'DNV Admin', 'DNV_ADMIN', 'DNV administrator with broad system access', 'CREATE,READ,UPDATE,DELETE,APPROVE,AUDIT', 1, GETDATE(), GETDATE(), 1, 1, 1),
(3, 'DNV Regional Manager', 'DNV_REGIONAL_MANAGER', 'DNV regional operations manager', 'READ,UPDATE,APPROVE,AUDIT,SCHEDULE', 1, GETDATE(), GETDATE(), 1, 1, 1),

-- DNV Operational Roles
(4, 'Lead Auditor', 'LEAD_AUDITOR', 'Senior auditor who leads audit teams', 'READ,UPDATE,AUDIT,SCHEDULE,APPROVE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(5, 'Senior Auditor', 'SENIOR_AUDITOR', 'Experienced auditor with advanced permissions', 'READ,UPDATE,AUDIT,SCHEDULE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(6, 'Auditor', 'AUDITOR', 'Standard auditor role for conducting audits', 'READ,UPDATE,AUDIT', 1, GETDATE(), GETDATE(), 1, 1, 0),
(7, 'Junior Auditor', 'JUNIOR_AUDITOR', 'Entry-level auditor with basic permissions', 'READ,AUDIT', 1, GETDATE(), GETDATE(), 1, 1, 0),

-- DNV Support Roles
(8, 'Audit Coordinator', 'AUDIT_COORDINATOR', 'Coordinates audit scheduling and logistics', 'READ,UPDATE,SCHEDULE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(9, 'Certification Officer', 'CERTIFICATION_OFFICER', 'Manages certificate issuance and maintenance', 'READ,UPDATE,CERTIFICATE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(10, 'Finance Officer', 'FINANCE_OFFICER', 'Handles financial operations and invoicing', 'READ,UPDATE,FINANCE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(11, 'Customer Service Rep', 'CUSTOMER_SERVICE_REP', 'Provides customer support and assistance', 'READ,SUPPORT', 1, GETDATE(), GETDATE(), 1, 1, 0),

-- Customer Company Roles
(12, 'Company Admin', 'COMPANY_ADMIN', 'Customer company administrator', 'READ,UPDATE,MANAGE_USERS', 1, GETDATE(), GETDATE(), 1, 1, 0),
(13, 'Site Manager', 'SITE_MANAGER', 'Manages a specific customer site', 'READ,UPDATE,RESPOND', 1, GETDATE(), GETDATE(), 1, 1, 0),
(14, 'Quality Manager', 'QUALITY_MANAGER', 'Quality management system administrator', 'READ,UPDATE,RESPOND,QUALITY', 1, GETDATE(), GETDATE(), 1, 1, 0),
(15, 'HSE Manager', 'HSE_MANAGER', 'Health, Safety, and Environment manager', 'READ,UPDATE,RESPOND,HSE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(16, 'Technical Manager', 'TECHNICAL_MANAGER', 'Technical operations manager', 'READ,UPDATE,RESPOND,TECHNICAL', 1, GETDATE(), GETDATE(), 1, 1, 0),

-- Customer Operational Roles  
(17, 'Site Representative', 'SITE_REPRESENTATIVE', 'On-site representative during audits', 'READ,RESPOND', 1, GETDATE(), GETDATE(), 1, 1, 0),
(18, 'Document Controller', 'DOCUMENT_CONTROLLER', 'Manages documentation and records', 'READ,UPDATE,DOCUMENT', 1, GETDATE(), GETDATE(), 1, 1, 0),
(19, 'Management Representative', 'MANAGEMENT_REPRESENTATIVE', 'Company representative for management system', 'READ,UPDATE,RESPOND,APPROVE', 1, GETDATE(), GETDATE(), 1, 1, 0),
(20, 'Training Coordinator', 'TRAINING_COORDINATOR', 'Manages training and competency records', 'READ,UPDATE,TRAINING', 1, GETDATE(), GETDATE(), 1, 1, 0),

-- General User Roles
(21, 'Read Only User', 'READ_ONLY_USER', 'Can only view assigned information', 'READ', 1, GETDATE(), GETDATE(), 1, 1, 0),
(22, 'Standard User', 'STANDARD_USER', 'Basic user with read and limited update access', 'READ,UPDATE', 1, GETDATE(), GETDATE(), 1, 1, 0),

-- System Roles
(23, 'System Administrator', 'SYSTEM_ADMIN', 'Technical system administrator', 'SYSTEM_ADMIN', 1, GETDATE(), GETDATE(), 1, 1, 1),
(24, 'Report Viewer', 'REPORT_VIEWER', 'Can access reports and dashboards', 'READ,REPORT', 1, GETDATE(), GETDATE(), 1, 1, 0),
(25, 'Guest User', 'GUEST_USER', 'Limited guest access for temporary users', 'READ_LIMITED', 1, GETDATE(), GETDATE(), 1, 1, 0);

SET IDENTITY_INSERT [dbo].[Roles] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalRoles FROM [dbo].[Roles];
SELECT RoleName, [Description], Permissions FROM [dbo].[Roles] ORDER BY RoleId;

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
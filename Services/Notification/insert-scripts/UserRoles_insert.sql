-- Insert sample data for UserRoles table
-- This script assigns roles to users based on their functions

INSERT INTO [dbo].[UserRoles] 
([UserId], [RoleId], [AssignedDate], [IsActive], [AssignedBy])
VALUES
-- DNV Staff Role Assignments
-- DNV Admin (UserId 1) - Super Admin
(1, 1, GETDATE(), 1, 1), -- DNV Super Admin

-- DNV Regional Managers (UserIds 2,3) - Regional Manager + Lead Auditor
(2, 3, GETDATE(), 1, 1), -- DNV Regional Manager
(2, 4, GETDATE(), 1, 1), -- Lead Auditor
(3, 3, GETDATE(), 1, 1), -- DNV Regional Manager
(3, 4, GETDATE(), 1, 1), -- Lead Auditor

-- Lead Auditors (UserIds 4,5,6) - Lead Auditor
(4, 4, GETDATE(), 1, 1), -- Lead Auditor
(5, 4, GETDATE(), 1, 1), -- Lead Auditor
(6, 4, GETDATE(), 1, 1), -- Lead Auditor

-- Senior Auditors (UserIds 7,8,9,10) - Senior Auditor
(7, 5, GETDATE(), 1, 1), -- Senior Auditor
(8, 5, GETDATE(), 1, 1), -- Senior Auditor
(9, 5, GETDATE(), 1, 1), -- Senior Auditor
(10, 5, GETDATE(), 1, 1), -- Senior Auditor

-- Regular Auditors (UserIds 11,12,13,14) - Auditor
(11, 6, GETDATE(), 1, 1), -- Auditor
(12, 6, GETDATE(), 1, 1), -- Auditor
(13, 6, GETDATE(), 1, 1), -- Auditor
(14, 6, GETDATE(), 1, 1), -- Auditor

-- Junior Auditor (UserId 15) - Junior Auditor
(15, 7, GETDATE(), 1, 1), -- Junior Auditor

-- DNV Support Staff
(16, 8, GETDATE(), 1, 1), -- Audit Coordinator
(17, 9, GETDATE(), 1, 1), -- Certification Officer
(18, 10, GETDATE(), 1, 1), -- Finance Officer
(19, 11, GETDATE(), 1, 1), -- Customer Service Rep
(20, 11, GETDATE(), 1, 1), -- Customer Service Rep

-- Customer Company Users Role Assignments

-- Acme Corporation (UserIds 21,22)
(21, 12, GETDATE(), 1, 1), -- Company Admin
(21, 19, GETDATE(), 1, 1), -- Management Representative
(22, 13, GETDATE(), 1, 1), -- Site Manager
(22, 17, GETDATE(), 1, 1), -- Site Representative

-- TechFlow Industries (UserIds 23,24)
(23, 12, GETDATE(), 1, 1), -- Company Admin
(23, 14, GETDATE(), 1, 1), -- Quality Manager
(24, 15, GETDATE(), 1, 1), -- HSE Manager
(24, 17, GETDATE(), 1, 1), -- Site Representative

-- Green Energy Solutions (UserIds 25,26)
(25, 19, GETDATE(), 1, 1), -- Management Representative
(25, 16, GETDATE(), 1, 1), -- Technical Manager
(26, 13, GETDATE(), 1, 1), -- Site Manager
(26, 18, GETDATE(), 1, 1), -- Document Controller

-- Maritime Solutions (UserIds 27,28)
(27, 12, GETDATE(), 1, 1), -- Company Admin
(27, 15, GETDATE(), 1, 1), -- HSE Manager
(28, 14, GETDATE(), 1, 1), -- Quality Manager
(28, 20, GETDATE(), 1, 1), -- Training Coordinator

-- Food Excellence Corp (UserIds 29,30)
(29, 19, GETDATE(), 1, 1), -- Management Representative
(29, 14, GETDATE(), 1, 1), -- Quality Manager
(30, 13, GETDATE(), 1, 1), -- Site Manager
(30, 18, GETDATE(), 1, 1), -- Document Controller

-- Additional role assignments for flexibility
-- Give some DNV staff additional support roles
(4, 8, GETDATE(), 1, 1), -- Lead Auditor + Audit Coordinator
(5, 9, GETDATE(), 1, 1), -- Lead Auditor + Certification Officer

-- Give some customer users additional roles
(21, 20, GETDATE(), 1, 1), -- Acme Admin + Training Coordinator
(23, 16, GETDATE(), 1, 1), -- TechFlow Admin + Technical Manager
(27, 20, GETDATE(), 1, 1), -- Maritime Admin + Training Coordinator

-- Some users get read-only access to additional areas
(22, 24, GETDATE(), 1, 1), -- Site Manager + Report Viewer
(24, 24, GETDATE(), 1, 1), -- HSE Manager + Report Viewer
(26, 24, GETDATE(), 1, 1), -- Site Manager + Report Viewer
(28, 24, GETDATE(), 1, 1), -- Quality Manager + Report Viewer
(30, 24, GETDATE(), 1, 1); -- Site Manager + Report Viewer

-- Verify the insert
SELECT COUNT(*) as TotalUserRoleAssignments FROM [dbo].[UserRoles];

-- Show user roles summary
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    u.CompanyId,
    STRING_AGG(r.RoleName, ', ') as AssignedRoles,
    COUNT(ur.RoleId) as RoleCount
FROM [dbo].[UserRoles] ur
INNER JOIN [dbo].[Users] u ON ur.UserId = u.UserId
INNER JOIN [dbo].[Roles] r ON ur.RoleId = r.RoleId
WHERE ur.IsActive = 1
GROUP BY u.UserId, u.FirstName, u.LastName, u.CompanyId
ORDER BY u.CompanyId, u.FirstName, u.LastName;

-- Show role distribution
SELECT 
    r.RoleName,
    COUNT(ur.UserId) as UserCount
FROM [dbo].[Roles] r
LEFT JOIN [dbo].[UserRoles] ur ON r.RoleId = ur.RoleId AND ur.IsActive = 1
GROUP BY r.RoleId, r.RoleName
ORDER BY UserCount DESC, r.RoleName;
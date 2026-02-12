-- Insert sample data for UserCompanyAccess table
-- This script defines which users can access which companies

INSERT INTO [dbo].[UserCompanyAccess] 
([UserId], [CompanyId], [AccessLevel], [GrantedDate], [IsActive], [GrantedBy])
VALUES
-- DNV Super Admin and Admin - Access to ALL companies
(1, 1, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> Acme Corporation
(1, 2, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> TechFlow Industries
(1, 3, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> Green Energy Solutions
(1, 4, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> Maritime Solutions
(1, 5, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> Food Excellence Corp
(1, 6, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> AutoTech Manufacturing
(1, 7, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> Health Plus Clinic
(1, 8, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> Dutch Innovation
(1, 9, 'Full', GETDATE(), 1, 1),  -- DNV Admin -> EuroLogistics
(1, 10, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Alpine Precision
(1, 11, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Nordic Forest
(1, 12, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Danish Wind Energy
(1, 13, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Tokyo Precision
(1, 14, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Australian Mining
(1, 15, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Canadian Energy
(1, 16, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Brazilian Construction
(1, 17, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Global Manufacturing Inc
(1, 18, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Indian Textiles
(1, 19, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Small Tech Solutions
(1, 20, 'Full', GETDATE(), 1, 1), -- DNV Admin -> Family Food Co

-- DNV Regional Managers - Regional company access
-- European Regional Manager (UserId 2) - European companies
(2, 2, 'Full', GETDATE(), 1, 1),  -- TechFlow Industries (UK)
(2, 3, 'Full', GETDATE(), 1, 1),  -- Green Energy Solutions (Germany)
(2, 4, 'Full', GETDATE(), 1, 1),  -- Maritime Solutions (Norway)
(2, 5, 'Full', GETDATE(), 1, 1),  -- Food Excellence Corp (France)
(2, 6, 'Full', GETDATE(), 1, 1),  -- AutoTech Manufacturing (Italy)
(2, 8, 'Full', GETDATE(), 1, 1),  -- Dutch Innovation (Netherlands)
(2, 9, 'Full', GETDATE(), 1, 1),  -- EuroLogistics (Belgium)
(2, 10, 'Full', GETDATE(), 1, 1), -- Alpine Precision (Switzerland)
(2, 11, 'Full', GETDATE(), 1, 1), -- Nordic Forest (Sweden)
(2, 12, 'Full', GETDATE(), 1, 1), -- Danish Wind Energy (Denmark)
(2, 17, 'Full', GETDATE(), 1, 1), -- Global Manufacturing Inc (European operations)
(2, 19, 'Full', GETDATE(), 1, 1), -- Small Tech Solutions (UK)
(2, 20, 'Full', GETDATE(), 1, 1), -- Family Food Co (France)

-- Americas Regional Manager (UserId 3) - Americas companies
(3, 1, 'Full', GETDATE(), 1, 1),  -- Acme Corporation (USA)
(3, 15, 'Full', GETDATE(), 1, 1), -- Canadian Energy (Canada)
(3, 16, 'Full', GETDATE(), 1, 1), -- Brazilian Construction (Brazil)
(3, 17, 'Full', GETDATE(), 1, 1), -- Global Manufacturing Inc (US operations)

-- Lead Auditors - Access to their assigned companies
-- Lead Auditor 1 (UserId 4) - Mixed portfolio
(4, 1, 'Full', GETDATE(), 1, 1),  -- Acme Corporation
(4, 3, 'Full', GETDATE(), 1, 1),  -- Green Energy Solutions
(4, 5, 'Full', GETDATE(), 1, 1),  -- Food Excellence Corp
(4, 7, 'Full', GETDATE(), 1, 1),  -- Health Plus Clinic

-- Lead Auditor 2 (UserId 5) - Manufacturing focus
(5, 6, 'Full', GETDATE(), 1, 1),  -- AutoTech Manufacturing
(5, 10, 'Full', GETDATE(), 1, 1), -- Alpine Precision
(5, 13, 'Full', GETDATE(), 1, 1), -- Tokyo Precision
(5, 17, 'Full', GETDATE(), 1, 1), -- Global Manufacturing Inc

-- Lead Auditor 3 (UserId 6) - Energy and maritime focus
(6, 4, 'Full', GETDATE(), 1, 1),  -- Maritime Solutions
(6, 11, 'Full', GETDATE(), 1, 1), -- Nordic Forest
(6, 12, 'Full', GETDATE(), 1, 1), -- Danish Wind Energy
(6, 15, 'Full', GETDATE(), 1, 1), -- Canadian Energy

-- Senior Auditors - Limited company access
(7, 1, 'Audit', GETDATE(), 1, 1),  -- Senior Auditor 1 -> Acme Corporation
(7, 2, 'Audit', GETDATE(), 1, 1),  -- Senior Auditor 1 -> TechFlow Industries
(8, 3, 'Audit', GETDATE(), 1, 1),  -- Senior Auditor 2 -> Green Energy Solutions
(8, 4, 'Audit', GETDATE(), 1, 1),  -- Senior Auditor 2 -> Maritime Solutions
(9, 5, 'Audit', GETDATE(), 1, 1),  -- Senior Auditor 3 -> Food Excellence Corp
(9, 6, 'Audit', GETDATE(), 1, 1),  -- Senior Auditor 3 -> AutoTech Manufacturing
(10, 8, 'Audit', GETDATE(), 1, 1), -- Senior Auditor 4 -> Dutch Innovation
(10, 9, 'Audit', GETDATE(), 1, 1), -- Senior Auditor 4 -> EuroLogistics

-- Regular Auditors - Specific company assignments
(11, 1, 'Audit', GETDATE(), 1, 1), -- Auditor 1 -> Acme Corporation
(11, 17, 'Audit', GETDATE(), 1, 1), -- Auditor 1 -> Global Manufacturing Inc
(12, 2, 'Audit', GETDATE(), 1, 1), -- Auditor 2 -> TechFlow Industries
(12, 19, 'Audit', GETDATE(), 1, 1), -- Auditor 2 -> Small Tech Solutions
(13, 3, 'Audit', GETDATE(), 1, 1), -- Auditor 3 -> Green Energy Solutions
(13, 12, 'Audit', GETDATE(), 1, 1), -- Auditor 3 -> Danish Wind Energy
(14, 4, 'Audit', GETDATE(), 1, 1), -- Auditor 4 -> Maritime Solutions
(14, 11, 'Audit', GETDATE(), 1, 1), -- Auditor 4 -> Nordic Forest

-- Junior Auditor - Limited access
(15, 1, 'Read', GETDATE(), 1, 1),  -- Junior Auditor -> Acme Corporation
(15, 2, 'Read', GETDATE(), 1, 1),  -- Junior Auditor -> TechFlow Industries

-- DNV Support Staff - Access based on function
-- Audit Coordinator - broad audit access
(16, 1, 'Schedule', GETDATE(), 1, 1),
(16, 2, 'Schedule', GETDATE(), 1, 1),
(16, 3, 'Schedule', GETDATE(), 1, 1),
(16, 4, 'Schedule', GETDATE(), 1, 1),
(16, 5, 'Schedule', GETDATE(), 1, 1),

-- Certification Officer - certificate management access
(17, 1, 'Certificate', GETDATE(), 1, 1),
(17, 2, 'Certificate', GETDATE(), 1, 1),
(17, 3, 'Certificate', GETDATE(), 1, 1),
(17, 4, 'Certificate', GETDATE(), 1, 1),
(17, 5, 'Certificate', GETDATE(), 1, 1),
(17, 6, 'Certificate', GETDATE(), 1, 1),

-- Finance Officer - financial access to all active companies
(18, 1, 'Finance', GETDATE(), 1, 1),
(18, 2, 'Finance', GETDATE(), 1, 1),
(18, 3, 'Finance', GETDATE(), 1, 1),
(18, 4, 'Finance', GETDATE(), 1, 1),
(18, 5, 'Finance', GETDATE(), 1, 1),
(18, 6, 'Finance', GETDATE(), 1, 1),
(18, 17, 'Finance', GETDATE(), 1, 1),

-- Customer Service Reps - read access to customer companies
(19, 1, 'Support', GETDATE(), 1, 1),
(19, 2, 'Support', GETDATE(), 1, 1),
(19, 3, 'Support', GETDATE(), 1, 1),
(20, 4, 'Support', GETDATE(), 1, 1),
(20, 5, 'Support', GETDATE(), 1, 1),
(20, 6, 'Support', GETDATE(), 1, 1),

-- Customer Company Users - Access to their OWN companies only
(21, 1, 'Owner', GETDATE(), 1, 1),  -- Acme Admin -> Acme Corporation
(22, 1, 'Full', GETDATE(), 1, 21),  -- Acme Site Manager -> Acme Corporation (granted by company admin)

(23, 2, 'Owner', GETDATE(), 1, 1),  -- TechFlow Admin -> TechFlow Industries
(24, 2, 'Full', GETDATE(), 1, 23),  -- TechFlow HSE Manager -> TechFlow Industries

(25, 3, 'Owner', GETDATE(), 1, 1),  -- Green Energy Rep -> Green Energy Solutions
(26, 3, 'Full', GETDATE(), 1, 25),  -- Green Energy Site Manager -> Green Energy Solutions

(27, 4, 'Owner', GETDATE(), 1, 1),  -- Maritime Admin -> Maritime Solutions
(28, 4, 'Full', GETDATE(), 1, 27),  -- Maritime Quality Manager -> Maritime Solutions

(29, 5, 'Owner', GETDATE(), 1, 1),  -- Food Excellence Rep -> Food Excellence Corp
(30, 5, 'Full', GETDATE(), 1, 29);  -- Food Excellence Site Manager -> Food Excellence Corp

-- Verify the insert
SELECT COUNT(*) as TotalCompanyAccessRecords FROM [dbo].[UserCompanyAccess];

-- Show access summary by company
SELECT 
    c.CompanyName,
    COUNT(uca.UserId) as UsersWithAccess,
    STRING_AGG(uca.AccessLevel, ', ') as AccessLevels
FROM [dbo].[Companies] c
LEFT JOIN [dbo].[UserCompanyAccess] uca ON c.CompanyId = uca.CompanyId AND uca.IsActive = 1
GROUP BY c.CompanyId, c.CompanyName
ORDER BY UsersWithAccess DESC;

-- Show access levels distribution
SELECT 
    AccessLevel,
    COUNT(*) as Count
FROM [dbo].[UserCompanyAccess] 
WHERE IsActive = 1
GROUP BY AccessLevel
ORDER BY Count DESC;
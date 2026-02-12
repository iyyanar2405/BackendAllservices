-- Insert sample data for UserTrainings table
-- This script creates training records for users showing completed and scheduled training

SET IDENTITY_INSERT [dbo].[UserTrainings] ON;

INSERT INTO [dbo].[UserTrainings] 
([UserTrainingId], [UserId], [TrainingId], [EnrollmentDate], [StartDate], [CompletionDate], [Status], [Score], [CertificationNumber], [ExpiryDate], [Comments], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy])
VALUES
-- DNV Staff Training Records - Admin and Management
(1, 1, 19, '2024-01-20', '2024-02-01', '2024-02-15', 'Completed', 88, 'DNV-MGT-2024-001', '2027-02-15', 'Excellent understanding of integrated management systems', '2024-01-20', '2024-02-15', 2, 2),
(2, 1, 20, '2024-03-01', '2024-03-15', '2024-03-22', 'Completed', 92, 'DNV-CHG-2024-001', '2027-03-22', 'Strong change management skills demonstrated', '2024-03-01', '2024-03-22', 2, 2),
(3, 1, 28, '2024-09-01', '2024-09-15', NULL, 'In Progress', NULL, NULL, NULL, 'Annual refresher training in progress', '2024-09-01', '2024-09-15', 2, 2),

(4, 2, 19, '2024-01-15', '2024-02-01', '2024-02-15', 'Completed', 94, 'DNV-MGT-2024-002', '2027-02-15', 'Exceptional management systems integration knowledge', '2024-01-15', '2024-02-15', 2, 2),
(5, 2, 6, '2024-02-01', '2024-02-20', '2024-03-05', 'Completed', 96, 'DNV-LA9001-2024-001', '2027-03-05', 'Outstanding lead auditor performance', '2024-02-01', '2024-03-05', 2, 2),
(6, 2, 29, '2024-08-01', '2024-08-15', '2024-08-30', 'Completed', 90, 'DNV-LAR-2024-001', '2027-08-30', 'Certification renewed successfully', '2024-08-01', '2024-08-30', 2, 2),

-- Lead Auditors Training Records
(7, 5, 1, '2024-01-10', '2024-01-25', '2024-02-20', 'Completed', 89, 'DNV-QMS-2024-001', '2027-02-20', 'Strong QMS knowledge and audit skills', '2024-01-10', '2024-02-20', 2, 2),
(8, 5, 6, '2024-02-15', '2024-03-01', '2024-03-15', 'Completed', 91, 'DNV-LA9001-2024-002', '2027-03-15', 'Excellent leadership during practical exercises', '2024-02-15', '2024-03-15', 2, 2),
(9, 5, 8, '2024-04-01', '2024-04-10', '2024-04-17', 'Completed', 87, 'DNV-INT-2024-001', '2027-04-17', 'Good interview techniques development', '2024-04-01', '2024-04-17', 2, 2),
(10, 5, 11, '2024-05-01', '2024-05-15', '2024-06-10', 'Completed', 93, 'DNV-IATF-2024-001', '2027-06-10', 'Excellent automotive industry knowledge', '2024-05-01', '2024-06-10', 2, 2),
(11, 5, 29, '2024-09-01', '2024-09-20', NULL, 'Scheduled', NULL, NULL, NULL, 'Annual lead auditor refresher scheduled', '2024-08-15', '2024-08-15', 2, 2),

(12, 9, 2, '2024-01-15', '2024-02-01', '2024-02-25', 'Completed', 85, 'DNV-EMS-2024-001', '2027-02-25', 'Good environmental management understanding', '2024-01-15', '2024-02-25', 2, 2),
(13, 9, 6, '2024-03-01', '2024-03-20', '2024-04-05', 'Completed', 88, 'DNV-LA9001-2024-003', '2027-04-05', 'Competent lead auditor skills', '2024-03-01', '2024-04-05', 2, 2),
(14, 9, 17, '2024-06-01', '2024-06-15', '2024-07-20', 'Completed', 92, 'DNV-WIND-2024-001', '2027-07-20', 'Excellent wind energy systems knowledge', '2024-06-01', '2024-07-20', 2, 2),
(15, 9, 29, '2024-09-10', NULL, NULL, 'Enrolled', NULL, NULL, NULL, 'Enrolled for refresher training', '2024-09-10', '2024-09-10', 2, 2),

(16, 12, 1, '2024-01-20', '2024-02-10', '2024-03-10', 'Completed', 90, 'DNV-QMS-2024-002', '2027-03-10', 'Solid QMS implementation skills', '2024-01-20', '2024-03-10', 2, 2),
(17, 12, 6, '2024-03-15', '2024-04-01', '2024-04-20', 'Completed', 89, 'DNV-LA9001-2024-004', '2027-04-20', 'Good audit leadership capabilities', '2024-03-15', '2024-04-20', 2, 2),
(18, 12, 12, '2024-05-01', '2024-05-20', '2024-06-15', 'Completed', 94, 'DNV-FSSC-2024-001', '2027-06-15', 'Outstanding food safety expertise', '2024-05-01', '2024-06-15', 2, 2),

-- Technical Specialists Training
(19, 7, 4, '2024-02-01', '2024-02-20', '2024-03-25', 'Completed', 91, 'DNV-ISMS-2024-001', '2027-03-25', 'Strong cybersecurity knowledge', '2024-02-01', '2024-03-25', 2, 2),
(20, 7, 14, '2024-04-01', '2024-04-15', '2024-04-22', 'Completed', 88, 'DNV-CYBER-2024-001', '2027-04-22', 'Good cybersecurity awareness', '2024-04-01', '2024-04-22', 2, 2),
(21, 7, 15, '2024-05-01', '2024-05-10', '2024-05-17', 'Completed', 89, 'DNV-GDPR-2024-001', '2027-05-17', 'Comprehensive GDPR understanding', '2024-05-01', '2024-05-17', 2, 2),
(22, 7, 16, '2024-07-01', '2024-07-15', '2024-08-10', 'Completed', 92, 'DNV-CLOUD-2024-001', '2027-08-10', 'Excellent cloud security knowledge', '2024-07-01', '2024-08-10', 2, 2),

(23, 8, 2, '2024-01-25', '2024-02-15', '2024-03-15', 'Completed', 87, 'DNV-EMS-2024-002', '2027-03-15', 'Good environmental management skills', '2024-01-25', '2024-03-15', 2, 2),
(24, 8, 21, '2024-04-01', '2024-04-20', '2024-05-15', 'Completed', 89, 'DNV-ENVC-2024-001', '2027-05-15', 'Strong regulatory compliance knowledge', '2024-04-01', '2024-05-15', 2, 2),
(25, 8, 5, '2024-06-01', '2024-06-20', '2024-07-15', 'Completed', 86, 'DNV-EnMS-2024-001', '2027-07-15', 'Good energy management understanding', '2024-06-01', '2024-07-15', 2, 2),

-- Junior Auditors Training
(26, 15, 1, '2024-03-01', '2024-03-20', '2024-04-25', 'Completed', 83, 'DNV-QMS-2024-003', '2027-04-25', 'Good foundational QMS knowledge', '2024-03-01', '2024-04-25', 2, 2),
(27, 15, 7, '2024-05-01', '2024-05-15', '2024-06-05', 'Completed', 85, 'DNV-IA-2024-001', '2027-06-05', 'Developing internal audit skills', '2024-05-01', '2024-06-05', 2, 2),
(28, 15, 8, '2024-07-01', '2024-07-10', '2024-07-17', 'Completed', 82, 'DNV-INT-2024-002', '2027-07-17', 'Good progress in interview techniques', '2024-07-01', '2024-07-17', 2, 2),
(29, 15, 6, '2024-09-01', '2024-09-20', NULL, 'In Progress', NULL, NULL, NULL, 'Working towards lead auditor qualification', '2024-08-15', '2024-09-20', 2, 2),

(30, 11, 10, '2024-02-01', '2024-02-20', '2024-03-20', 'Completed', 88, 'DNV-MAR-2024-001', '2027-03-20', 'Strong maritime safety knowledge', '2024-02-01', '2024-03-20', 2, 2),
(31, 11, 1, '2024-04-01', '2024-04-15', '2024-05-15', 'Completed', 84, 'DNV-QMS-2024-004', '2027-05-15', 'Solid quality management foundation', '2024-04-01', '2024-05-15', 2, 2),
(32, 11, 7, '2024-06-01', '2024-06-15', '2024-07-05', 'Completed', 86, 'DNV-IA-2024-002', '2027-07-05', 'Improving audit capabilities', '2024-06-01', '2024-07-05', 2, 2),

-- Customer Training Records - Acme Corporation
(33, 21, 1, '2024-02-15', '2024-03-01', '2024-04-05', 'Completed', 87, 'DNV-QMS-2024-005', '2027-04-05', 'Quality Manager - excellent QMS understanding', '2024-02-15', '2024-04-05', 2, 21),
(34, 21, 7, '2024-05-01', '2024-05-20', '2024-06-10', 'Completed', 85, 'DNV-IA-2024-003', '2027-06-10', 'Internal audit skills for quality system', '2024-05-01', '2024-06-10', 2, 21),
(35, 21, 28, '2024-09-01', '2024-09-15', NULL, 'In Progress', NULL, NULL, NULL, 'Annual quality system updates', '2024-09-01', '2024-09-15', 2, 21),

(36, 22, 3, '2024-03-01', '2024-03-20', '2024-04-15', 'Completed', 84, 'DNV-OHSMS-2024-001', '2027-04-15', 'Safety Officer - good OH&S knowledge', '2024-03-01', '2024-04-15', 2, 22),
(37, 22, 22, '2024-06-01', '2024-06-10', '2024-06-17', 'Completed', 86, 'DNV-OHSR-2024-001', '2027-06-17', 'Regulatory updates completed', '2024-06-01', '2024-06-17', 2, 22),
(38, 22, 17, '2024-09-15', NULL, NULL, 'Scheduled', NULL, NULL, NULL, 'Required for wind safety operations', '2024-09-01', '2024-09-01', 2, 22),

-- TechFlow Solutions Training
(39, 23, 4, '2024-02-20', '2024-03-10', '2024-04-15', 'Completed', 90, 'DNV-ISMS-2024-002', '2027-04-15', 'CTO - excellent cybersecurity leadership', '2024-02-20', '2024-04-15', 2, 23),
(40, 23, 14, '2024-05-01', '2024-05-15', '2024-05-22', 'Completed', 88, 'DNV-CYBER-2024-002', '2027-05-22', 'Strong cybersecurity awareness', '2024-05-01', '2024-05-22', 2, 23),
(41, 23, 15, '2024-06-01', '2024-06-15', '2024-06-22', 'Completed', 89, 'DNV-GDPR-2024-002', '2027-06-22', 'GDPR compliance well understood', '2024-06-01', '2024-06-22', 2, 23),
(42, 23, 27, '2024-08-01', '2024-08-20', '2024-09-05', 'Completed', 93, 'DNV-AI-2024-001', '2027-09-05', 'Excellent AI ethics and governance knowledge', '2024-08-01', '2024-09-05', 2, 23),

(43, 24, 14, '2024-07-01', '2024-07-15', '2024-07-22', 'Completed', 85, 'DNV-CYBER-2024-003', '2027-07-22', 'Cybersecurity Developer - good awareness', '2024-07-01', '2024-07-22', 2, 24),
(44, 24, 15, '2024-08-01', '2024-08-15', '2024-08-22', 'Completed', 87, 'DNV-GDPR-2024-003', '2027-08-22', 'Privacy compliance understood', '2024-08-01', '2024-08-22', 2, 24),
(45, 24, 18, '2024-09-15', NULL, NULL, 'Enrolled', NULL, NULL, NULL, 'Solar systems training for new project', '2024-09-10', '2024-09-10', 2, 24),

-- Green Energy Solutions Training
(46, 25, 5, '2024-03-01', '2024-03-20', '2024-04-10', 'Completed', 91, 'DNV-EnMS-2024-002', '2027-04-10', 'Energy Manager - excellent energy systems knowledge', '2024-03-01', '2024-04-10', 2, 25),
(47, 25, 17, '2024-04-01', '2024-04-20', '2024-05-25', 'Completed', 93, 'DNV-WIND-2024-002', '2027-05-25', 'Outstanding wind energy expertise', '2024-04-01', '2024-05-25', 2, 25),
(48, 25, 21, '2024-06-01', '2024-06-20', '2024-07-15', 'Completed', 88, 'DNV-ENVC-2024-002', '2027-07-15', 'Environmental compliance well understood', '2024-06-01', '2024-07-15', 2, 25),
(49, 25, 18, '2024-08-01', '2024-08-20', '2024-09-15', 'Completed', 90, 'DNV-SOLAR-2024-001', '2027-09-15', 'Solar systems certification achieved', '2024-08-01', '2024-09-15', 2, 25),

(50, 26, 21, '2024-04-01', '2024-04-20', '2024-05-10', 'Completed', 86, 'DNV-ENVC-2024-003', '2027-05-10', 'Environmental Specialist - good compliance knowledge', '2024-04-01', '2024-05-10', 2, 26),
(51, 26, 2, '2024-06-01', '2024-06-20', '2024-07-20', 'Completed', 87, 'DNV-EMS-2024-003', '2027-07-20', 'EMS implementation skills developed', '2024-06-01', '2024-07-20', 2, 26),

-- Maritime Excellence Training
(52, 27, 10, '2024-03-15', '2024-04-01', '2024-05-05', 'Completed', 89, 'DNV-MAR-2024-002', '2027-05-05', 'Operations Manager - strong maritime knowledge', '2024-03-15', '2024-05-05', 2, 27),
(53, 27, 3, '2024-05-01', '2024-05-20', '2024-06-15', 'Completed', 87, 'DNV-OHSMS-2024-002', '2027-06-15', 'Maritime safety management understood', '2024-05-01', '2024-06-15', 2, 27),
(54, 27, 23, '2024-07-01', '2024-07-20', '2024-08-15', 'Completed', 91, 'DNV-BCM-2024-001', '2027-08-15', 'Excellent business continuity planning', '2024-07-01', '2024-08-15', 2, 27),

-- Food Excellence Corporation Training
(55, 28, 12, '2024-03-01', '2024-03-20', '2024-04-20', 'Completed', 92, 'DNV-FSSC-2024-002', '2027-04-20', 'Quality Director - outstanding food safety knowledge', '2024-03-01', '2024-04-20', 2, 28),
(56, 28, 1, '2024-05-01', '2024-05-20', '2024-06-20', 'Completed', 88, 'DNV-QMS-2024-006', '2027-06-20', 'Quality management skills enhanced', '2024-05-01', '2024-06-20', 2, 28),
(57, 28, 24, '2024-07-01', '2024-07-15', '2024-08-05', 'Completed', 89, 'DNV-SCR-2024-001', '2027-08-05', 'Supply chain risk management completed', '2024-07-01', '2024-08-05', 2, 28),

(58, 29, 12, '2024-04-01', '2024-04-20', '2024-05-20', 'Completed', 85, 'DNV-FSSC-2024-003', '2027-05-20', 'Food Safety Manager - good foundation', '2024-04-01', '2024-05-20', 2, 29),
(59, 29, 22, '2024-06-01', '2024-06-10', '2024-06-17', 'Completed', 84, 'DNV-OHSR-2024-002', '2027-06-17', 'Health and safety updates completed', '2024-06-01', '2024-06-17', 2, 29),

-- Auto Innovation Inc Training
(60, 30, 11, '2024-04-01', '2024-04-20', '2024-05-25', 'Completed', 94, 'DNV-IATF-2024-002', '2027-05-25', 'Quality Manager - exceptional automotive knowledge', '2024-04-01', '2024-05-25', 2, 30),
(61, 30, 1, '2024-06-01', '2024-06-20', '2024-07-20', 'Completed', 90, 'DNV-QMS-2024-007', '2027-07-20', 'Strong quality management foundation', '2024-06-01', '2024-07-20', 2, 30),
(62, 30, 24, '2024-08-01', '2024-08-15', '2024-09-05', 'Completed', 87, 'DNV-SCR-2024-002', '2027-09-05', 'Automotive supply chain expertise', '2024-08-01', '2024-09-05', 2, 30);

SET IDENTITY_INSERT [dbo].[UserTrainings] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalUserTrainings FROM [dbo].[UserTrainings];

-- Show training completion statistics
SELECT 
    Status,
    COUNT(*) as Count,
    ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM [dbo].[UserTrainings]), 2) as Percentage
FROM [dbo].[UserTrainings] 
GROUP BY Status
ORDER BY Count DESC;

-- Show training completion by user type (DNV staff vs customers)
SELECT 
    CASE 
        WHEN u.CompanyId = 1 THEN 'DNV Staff'
        ELSE 'Customer'
    END as UserType,
    COUNT(ut.UserTrainingId) as TotalTrainings,
    COUNT(CASE WHEN ut.Status = 'Completed' THEN 1 END) as CompletedTrainings,
    COUNT(CASE WHEN ut.Status = 'In Progress' THEN 1 END) as InProgressTrainings,
    COUNT(CASE WHEN ut.Status = 'Scheduled' THEN 1 END) as ScheduledTrainings,
    ROUND(AVG(CASE WHEN ut.Score IS NOT NULL THEN ut.Score END), 1) as AverageScore
FROM [dbo].[Users] u
INNER JOIN [dbo].[UserTrainings] ut ON u.UserId = ut.UserId
GROUP BY CASE WHEN u.CompanyId = 1 THEN 'DNV Staff' ELSE 'Customer' END
ORDER BY TotalTrainings DESC;

-- Show top performing users by training scores
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    c.CompanyName,
    COUNT(ut.UserTrainingId) as CompletedTrainings,
    ROUND(AVG(ut.Score), 1) as AverageScore,
    MAX(ut.Score) as HighestScore,
    COUNT(CASE WHEN ut.Score >= 90 THEN 1 END) as ExcellentScores
FROM [dbo].[Users] u
INNER JOIN [dbo].[Companies] c ON u.CompanyId = c.CompanyId
INNER JOIN [dbo].[UserTrainings] ut ON u.UserId = ut.UserId
WHERE ut.Status = 'Completed' AND ut.Score IS NOT NULL
GROUP BY u.UserId, u.FirstName, u.LastName, c.CompanyName
HAVING COUNT(ut.UserTrainingId) >= 3
ORDER BY AverageScore DESC, CompletedTrainings DESC;

-- Show most popular training courses
SELECT 
    t.TrainingName,
    t.Category,
    COUNT(ut.UserTrainingId) as Enrollments,
    COUNT(CASE WHEN ut.Status = 'Completed' THEN 1 END) as Completions,
    ROUND(COUNT(CASE WHEN ut.Status = 'Completed' THEN 1 END) * 100.0 / COUNT(ut.UserTrainingId), 1) as CompletionRate,
    ROUND(AVG(CASE WHEN ut.Score IS NOT NULL THEN ut.Score END), 1) as AverageScore
FROM [dbo].[Trainings] t
INNER JOIN [dbo].[UserTrainings] ut ON t.TrainingId = ut.TrainingId
GROUP BY t.TrainingId, t.TrainingName, t.Category
ORDER BY Enrollments DESC, CompletionRate DESC;

-- Show upcoming training schedule
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    c.CompanyName,
    t.TrainingName,
    ut.StartDate,
    ut.Status,
    ut.Comments
FROM [dbo].[Users] u
INNER JOIN [dbo].[Companies] c ON u.CompanyId = c.CompanyId
INNER JOIN [dbo].[UserTrainings] ut ON u.UserId = ut.UserId
INNER JOIN [dbo].[Trainings] t ON ut.TrainingId = t.TrainingId
WHERE ut.Status IN ('Scheduled', 'In Progress', 'Enrolled')
ORDER BY ut.StartDate ASC, u.FirstName;

-- Show certification expiry report
SELECT 
    u.FirstName + ' ' + u.LastName as UserName,
    c.CompanyName,
    t.TrainingName,
    ut.CertificationNumber,
    ut.ExpiryDate,
    DATEDIFF(day, GETDATE(), ut.ExpiryDate) as DaysUntilExpiry,
    CASE 
        WHEN ut.ExpiryDate < GETDATE() THEN 'Expired'
        WHEN DATEDIFF(day, GETDATE(), ut.ExpiryDate) <= 90 THEN 'Expiring Soon'
        WHEN DATEDIFF(day, GETDATE(), ut.ExpiryDate) <= 180 THEN 'Renewal Due'
        ELSE 'Current'
    END as ExpiryStatus
FROM [dbo].[Users] u
INNER JOIN [dbo].[Companies] c ON u.CompanyId = c.CompanyId
INNER JOIN [dbo].[UserTrainings] ut ON u.UserId = ut.UserId
INNER JOIN [dbo].[Trainings] t ON ut.TrainingId = t.TrainingId
WHERE ut.Status = 'Completed' AND ut.ExpiryDate IS NOT NULL
ORDER BY ut.ExpiryDate ASC;
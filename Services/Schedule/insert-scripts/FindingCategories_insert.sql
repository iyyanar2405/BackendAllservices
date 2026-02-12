-- Insert sample data for FindingCategories table
-- This script creates categories for audit findings

SET IDENTITY_INSERT [dbo].[FindingCategories] ON;

INSERT INTO [dbo].[FindingCategories] 
([CategoryId], [CategoryName], [CategoryDescription], [Severity], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Non-Conformity', 'Non-fulfillment of a requirement', 'Major', 1, GETDATE(), GETDATE(), 1, 1),
(2, 'Minor Non-Conformity', 'Non-conformity which is not likely to result in failure of the management system', 'Minor', 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Observation', 'Statement of fact made during an audit and substantiated by objective evidence', 'Low', 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Opportunity for Improvement', 'Suggestion for enhancement of the management system', 'Low', 1, GETDATE(), GETDATE(), 1, 1),
(5, 'Critical Non-Conformity', 'Serious failure that could result in imminent danger', 'Critical', 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Systemic Non-Conformity', 'Non-conformity that affects the entire management system', 'Major', 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Documentation Issue', 'Issues related to documentation and records', 'Minor', 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Training Deficiency', 'Lack of adequate training or competence', 'Medium', 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Process Non-Conformity', 'Failure in process implementation or control', 'Major', 1, GETDATE(), GETDATE(), 1, 1),
(10, 'Communication Issue', 'Problems with internal or external communication', 'Medium', 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[FindingCategories] OFF;

-- Verify the insert
SELECT * FROM [dbo].[FindingCategories] ORDER BY CategoryId;
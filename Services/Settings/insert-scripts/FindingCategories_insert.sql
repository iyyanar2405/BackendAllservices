-- Insert sample data for FindingCategories table
-- This script creates categories for audit findings

SET IDENTITY_INSERT [dbo].[FindingCategories] ON;

INSERT INTO [dbo].[FindingCategories] 
([FindingCategoryId], [CategoryName], [CategoryCode], [Description], [ParentCategoryId], [Color], [DisplayOrder], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Non-Conformity', 'NON_CONFORMITY', 'Non-fulfillment of a requirement', NULL, '#F44336', 1, 1, GETDATE(), GETDATE(), 1, 1),
(2, 'Minor Non-Conformity', 'MINOR_NON_CONFORMITY', 'Non-conformity which is not likely to result in failure of the management system', NULL, '#FF9800', 2, 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Observation', 'OBSERVATION', 'Statement of fact made during an audit and substantiated by objective evidence', NULL, '#2196F3', 3, 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Opportunity for Improvement', 'OFI', 'Suggestion for enhancement of the management system', NULL, '#00BCD4', 4, 1, GETDATE(), GETDATE(), 1, 1),
(5, 'Critical Non-Conformity', 'CRITICAL_NON_CONFORMITY', 'Serious failure that could result in imminent danger', NULL, '#B71C1C', 5, 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Systemic Non-Conformity', 'SYSTEMIC_NON_CONFORMITY', 'Non-conformity that affects the entire management system', NULL, '#E91E63', 6, 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Documentation Issue', 'DOCUMENTATION_ISSUE', 'Issues related to documentation and records', NULL, '#607D8B', 7, 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Training Deficiency', 'TRAINING_DEFICIENCY', 'Lack of adequate training or competence', NULL, '#9C27B0', 8, 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Process Non-Conformity', 'PROCESS_NON_CONFORMITY', 'Failure in process implementation or control', NULL, '#795548', 9, 1, GETDATE(), GETDATE(), 1, 1),
(10, 'Communication Issue', 'COMMUNICATION_ISSUE', 'Problems with internal or external communication', NULL, '#3F51B5', 10, 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[FindingCategories] OFF;

-- Verify the insert
SELECT * FROM [dbo].[FindingCategories] ORDER BY DisplayOrder;
-- Insert sample data for FindingStatuses table
-- This script creates status values for audit findings

SET IDENTITY_INSERT [dbo].[FindingStatuses] ON;

INSERT INTO [dbo].[FindingStatuses] 
([StatusId], [StatusName], [StatusDescription], [Color], [SortOrder], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Open', 'Finding is newly identified and requires action', '#FF4444', 1, 1, GETDATE(), GETDATE(), 1, 1),
(2, 'In Progress', 'Corrective action is being implemented', '#FFA500', 2, 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Pending Verification', 'Action completed, awaiting auditor verification', '#FFD700', 3, 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Verified', 'Action verified as effective by auditor', '#32CD32', 4, 1, GETDATE(), GETDATE(), 1, 1),
(5, 'Closed', 'Finding fully resolved and closed', '#008000', 5, 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Overdue', 'Finding response is past due date', '#DC143C', 6, 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Extended', 'Deadline extended with approval', '#9370DB', 7, 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Disputed', 'Finding is being disputed by client', '#FF69B4', 8, 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[FindingStatuses] OFF;

-- Verify the insert
SELECT * FROM [dbo].[FindingStatuses] ORDER BY SortOrder;
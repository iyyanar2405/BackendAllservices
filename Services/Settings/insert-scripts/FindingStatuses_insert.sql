-- Insert sample data for FindingStatuses table
-- This script creates status values for audit findings

SET IDENTITY_INSERT [dbo].[FindingStatuses] ON;

INSERT INTO [dbo].[FindingStatuses] 
([FindingStatusId], [StatusName], [StatusCode], [Description], [Color], [DisplayOrder], [IsClosedStatus], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
(1, 'Open', 'OPEN', 'Finding is newly identified and requires action', '#FF4444', 1, 0, 1, GETDATE(), GETDATE(), 1, 1),
(2, 'In Progress', 'IN_PROGRESS', 'Corrective action is being implemented', '#FFA500', 2, 0, 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Pending Verification', 'PENDING_VERIFICATION', 'Action completed, awaiting auditor verification', '#FFD700', 3, 0, 1, GETDATE(), GETDATE(), 1, 1),
(4, 'Verified', 'VERIFIED', 'Action verified as effective by auditor', '#32CD32', 4, 1, 1, GETDATE(), GETDATE(), 1, 1),
(5, 'Closed', 'CLOSED', 'Finding fully resolved and closed', '#008000', 5, 1, 1, GETDATE(), GETDATE(), 1, 1),
(6, 'Overdue', 'OVERDUE', 'Finding response is past due date', '#DC143C', 6, 0, 1, GETDATE(), GETDATE(), 1, 1),
(7, 'Extended', 'EXTENDED', 'Deadline extended with approval', '#9370DB', 7, 0, 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Disputed', 'DISPUTED', 'Finding is being disputed by client', '#FF69B4', 8, 0, 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[FindingStatuses] OFF;

-- Verify the insert
SELECT * FROM [dbo].[FindingStatuses] ORDER BY DisplayOrder;
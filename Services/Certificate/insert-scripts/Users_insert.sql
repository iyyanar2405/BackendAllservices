-- Insert sample data for Users table
-- This script creates sample users for the customer portal system

SET IDENTITY_INSERT [dbo].[Users] ON;

INSERT INTO [dbo].[Users] 
([UserId], [Username], [Email], [FirstName], [LastName], [PasswordHash], [IsActive], [Phone], [Position], [Department], [TimeZone], [Language], [IsEmailVerified], [TwoFactorEnabled], [CreatedDate], [ModifiedDate])
VALUES
-- System Administrator
(1, 'admin', 'admin@dnv.com', 'System', 'Administrator', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+47-67-57-9900', 'System Administrator', 'IT', 'Europe/Oslo', 'EN', 1, 1, GETDATE(), GETDATE()),

-- DNV Staff - Lead Auditors
(2, 'j.smith', 'john.smith@dnv.com', 'John', 'Smith', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+44-20-7357-6080', 'Lead Auditor', 'Certification', 'Europe/London', 'EN', 1, 0, GETDATE(), GETDATE()),
(3, 'a.johnson', 'anna.johnson@dnv.com', 'Anna', 'Johnson', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+49-40-361-49-0', 'Senior Auditor', 'Certification', 'Europe/Berlin', 'EN', 1, 0, GETDATE(), GETDATE()),
(4, 'm.brown', 'michael.brown@dnv.com', 'Michael', 'Brown', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+1-281-721-6100', 'Lead Auditor', 'Certification', 'America/Chicago', 'EN', 1, 0, GETDATE(), GETDATE()),
(5, 's.wilson', 'sarah.wilson@dnv.com', 'Sarah', 'Wilson', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+33-1-55-24-7000', 'Principal Auditor', 'Certification', 'Europe/Paris', 'EN', 1, 0, GETDATE(), GETDATE()),
(6, 'l.garcia', 'luis.garcia@dnv.com', 'Luis', 'Garcia', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+34-91-781-9480', 'Senior Auditor', 'Certification', 'Europe/Madrid', 'ES', 1, 0, GETDATE(), GETDATE()),

-- DNV Technical Experts
(7, 'r.anderson', 'robert.anderson@dnv.com', 'Robert', 'Anderson', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+47-67-57-9900', 'Technical Expert', 'Maritime', 'Europe/Oslo', 'EN', 1, 0, GETDATE(), GETDATE()),
(8, 'e.martinez', 'elena.martinez@dnv.com', 'Elena', 'Martinez', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+31-10-592-3500', 'Technical Expert', 'Oil & Gas', 'Europe/Amsterdam', 'EN', 1, 0, GETDATE(), GETDATE()),
(9, 'h.tanaka', 'hiroshi.tanaka@dnv.com', 'Hiroshi', 'Tanaka', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+81-3-5421-0101', 'Technical Expert', 'Energy', 'Asia/Tokyo', 'JA', 1, 0, GETDATE(), GETDATE()),
(10, 'k.lee', 'kevin.lee@dnv.com', 'Kevin', 'Lee', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+65-6508-2888', 'Senior Auditor', 'Certification', 'Asia/Singapore', 'EN', 1, 0, GETDATE(), GETDATE()),

-- Customer Company Users - Acme Corporation
(11, 'j.doe', 'john.doe@acmecorp.com', 'John', 'Doe', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+1-555-123-4567', 'Quality Manager', 'Quality Assurance', 'America/New_York', 'EN', 1, 0, GETDATE(), GETDATE()),
(12, 'm.davis', 'mary.davis@acmecorp.com', 'Mary', 'Davis', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+1-555-123-4568', 'Environmental Coordinator', 'Environment', 'America/New_York', 'EN', 1, 0, GETDATE(), GETDATE()),
(13, 'r.miller', 'richard.miller@acmecorp.com', 'Richard', 'Miller', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+1-555-123-4569', 'Safety Officer', 'Health & Safety', 'America/New_York', 'EN', 1, 0, GETDATE(), GETDATE()),
(14, 'l.taylor', 'lisa.taylor@acmecorp.com', 'Lisa', 'Taylor', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+1-555-123-4570', 'Management Representative', 'Management', 'America/New_York', 'EN', 1, 0, GETDATE(), GETDATE()),

-- Customer Company Users - TechFlow Industries
(15, 'p.white', 'peter.white@techflow.com', 'Peter', 'White', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+44-20-1234-5678', 'Quality Director', 'Quality', 'Europe/London', 'EN', 1, 0, GETDATE(), GETDATE()),
(16, 's.clark', 'susan.clark@techflow.com', 'Susan', 'Clark', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+44-20-1234-5679', 'Information Security Manager', 'IT Security', 'Europe/London', 'EN', 1, 0, GETDATE(), GETDATE()),
(17, 'd.lewis', 'david.lewis@techflow.com', 'David', 'Lewis', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+44-20-1234-5680', 'Operations Manager', 'Operations', 'Europe/London', 'EN', 1, 0, GETDATE(), GETDATE()),

-- Customer Company Users - Green Energy Solutions
(18, 'a.mueller', 'andreas.mueller@greenenergy.de', 'Andreas', 'Mueller', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+49-30-1234-5678', 'Compliance Manager', 'Compliance', 'Europe/Berlin', 'DE', 1, 0, GETDATE(), GETDATE()),
(19, 'i.schmidt', 'ingrid.schmidt@greenenergy.de', 'Ingrid', 'Schmidt', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+49-30-1234-5679', 'Environmental Manager', 'Environment', 'Europe/Berlin', 'DE', 1, 0, GETDATE(), GETDATE()),
(20, 'm.weber', 'michael.weber@greenenergy.de', 'Michael', 'Weber', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+49-30-1234-5680', 'Energy Manager', 'Energy', 'Europe/Berlin', 'DE', 1, 0, GETDATE(), GETDATE()),

-- Customer Company Users - Maritime Solutions Ltd
(21, 't.hansen', 'tor.hansen@maritime-solutions.no', 'Tor', 'Hansen', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+47-22-123456', 'Fleet Manager', 'Operations', 'Europe/Oslo', 'NO', 1, 0, GETDATE(), GETDATE()),
(22, 'k.olsen', 'kari.olsen@maritime-solutions.no', 'Kari', 'Olsen', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+47-22-123457', 'Safety Manager', 'Safety', 'Europe/Oslo', 'NO', 1, 0, GETDATE(), GETDATE()),
(23, 'o.berg', 'ola.berg@maritime-solutions.no', 'Ola', 'Berg', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+47-22-123458', 'Technical Manager', 'Engineering', 'Europe/Oslo', 'NO', 1, 0, GETDATE(), GETDATE()),

-- Customer Company Users - Food Excellence Corp
(24, 'c.dubois', 'catherine.dubois@foodexcellence.fr', 'Catherine', 'Dubois', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+33-1-4567-8901', 'Quality Assurance Manager', 'Quality', 'Europe/Paris', 'FR', 1, 0, GETDATE(), GETDATE()),
(25, 'p.martin', 'pierre.martin@foodexcellence.fr', 'Pierre', 'Martin', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+33-1-4567-8902', 'Food Safety Manager', 'Food Safety', 'Europe/Paris', 'FR', 1, 0, GETDATE(), GETDATE()),
(26, 'n.bernard', 'nicole.bernard@foodexcellence.fr', 'Nicole', 'Bernard', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+33-1-4567-8903', 'Production Manager', 'Production', 'Europe/Paris', 'FR', 1, 0, GETDATE(), GETDATE()),

-- Customer Company Users - AutoTech Manufacturing
(27, 'g.rossi', 'giuseppe.rossi@autotech.it', 'Giuseppe', 'Rossi', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+39-02-1234-5678', 'Quality Director', 'Quality', 'Europe/Rome', 'IT', 1, 0, GETDATE(), GETDATE()),
(28, 'f.ferrari', 'francesca.ferrari@autotech.it', 'Francesca', 'Ferrari', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+39-02-1234-5679', 'Environmental Specialist', 'Environment', 'Europe/Rome', 'IT', 1, 0, GETDATE(), GETDATE()),
(29, 'l.bianchi', 'lorenzo.bianchi@autotech.it', 'Lorenzo', 'Bianchi', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+39-02-1234-5680', 'Manufacturing Manager', 'Manufacturing', 'Europe/Rome', 'IT', 1, 0, GETDATE(), GETDATE()),
(30, 'a.conti', 'alessandro.conti@autotech.it', 'Alessandro', 'Conti', '$2a$10$N9qo8uLOickgx2ZMRZoMye1kW7.QN6j1h9g3qBL.WJuJ8a4KqAQuO', 1, '+39-02-1234-5681', 'Health & Safety Coordinator', 'Safety', 'Europe/Rome', 'IT', 1, 0, GETDATE(), GETDATE());

SET IDENTITY_INSERT [dbo].[Users] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalUsers FROM [dbo].[Users];
SELECT Position, COUNT(*) as Count FROM [dbo].[Users] GROUP BY Position ORDER BY Count DESC;
SELECT TOP 10 Username, FirstName, LastName, Email, Position, Department FROM [dbo].[Users] ORDER BY UserId;
-- Insert sample data for Sites table
-- This script creates sample company sites/facilities

SET IDENTITY_INSERT [dbo].[Sites] ON;

INSERT INTO [dbo].[Sites] 
([SiteId], [SiteName], [SiteCode], [CompanyId], [Address], [CityId], [CountryId], [PostalCode], [Latitude], [Longitude], [Phone], [Email], [ContactPerson], [ContactEmail], [ContactPhone], [SiteType], [EmployeeCount], [Area], [TimeZone], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation Sites
(1, 'Acme Headquarters', 'ACME-HQ-001', 1, '123 Industrial Blvd', 1, 1, '10001', 40.7128, -74.0060, '+1-555-123-4567', 'hq@acmecorp.com', 'John Doe', 'john.doe@acmecorp.com', '+1-555-123-4567', 'Headquarters', 150, 5000.00, 'America/New_York', 1, GETDATE(), GETDATE(), 1, 1),
(2, 'Acme Manufacturing Plant 1', 'ACME-MFG-001', 1, '456 Factory Road', 2, 1, '90210', 34.0522, -118.2437, '+1-555-123-4568', 'plant1@acmecorp.com', 'Mary Davis', 'mary.davis@acmecorp.com', '+1-555-123-4568', 'Manufacturing', 350, 15000.00, 'America/Los_Angeles', 1, GETDATE(), GETDATE(), 1, 1),
(3, 'Acme Warehouse Chicago', 'ACME-WH-001', 1, '789 Storage Ave', 3, 1, '60601', 41.8781, -87.6298, '+1-555-123-4569', 'warehouse@acmecorp.com', 'Richard Miller', 'richard.miller@acmecorp.com', '+1-555-123-4569', 'Warehouse', 80, 8000.00, 'America/Chicago', 1, GETDATE(), GETDATE(), 1, 1),

-- TechFlow Industries Sites
(4, 'TechFlow Main Office', 'TECH-HQ-001', 2, '45 Technology Park', 6, 2, 'SW1A 1AA', 51.5074, -0.1278, '+44-20-1234-5678', 'mainoffice@techflow.com', 'Peter White', 'peter.white@techflow.com', '+44-20-1234-5678', 'Headquarters', 400, 3500.00, 'Europe/London', 1, GETDATE(), GETDATE(), 1, 1),
(5, 'TechFlow Data Center', 'TECH-DC-001', 2, '67 Server Street', 7, 2, 'M1 2AA', 53.4808, -2.2426, '+44-20-1234-5679', 'datacenter@techflow.com', 'Susan Clark', 'susan.clark@techflow.com', '+44-20-1234-5679', 'Data Center', 45, 2000.00, 'Europe/London', 1, GETDATE(), GETDATE(), 1, 1),
(6, 'TechFlow Development Lab', 'TECH-LAB-001', 2, '89 Innovation Road', 8, 2, 'B2 3BB', 52.4862, -1.8904, '+44-20-1234-5680', 'devlab@techflow.com', 'David Lewis', 'david.lewis@techflow.com', '+44-20-1234-5680', 'R&D', 120, 1800.00, 'Europe/London', 1, GETDATE(), GETDATE(), 1, 1),

-- Green Energy Solutions Sites
(7, 'Green Energy Berlin HQ', 'GES-HQ-001', 3, 'Umweltstraße 15', 11, 3, '10115', 52.5200, 13.4050, '+49-30-1234-5678', 'berlin@greenenergy.de', 'Andreas Mueller', 'andreas.mueller@greenenergy.de', '+49-30-1234-5678', 'Headquarters', 200, 4000.00, 'Europe/Berlin', 1, GETDATE(), GETDATE(), 1, 1),
(8, 'Green Energy Wind Farm 1', 'GES-WF-001', 3, 'Windpark Straße 23', 12, 3, '80331', 48.1351, 11.5820, '+49-30-1234-5679', 'windfarm1@greenenergy.de', 'Ingrid Schmidt', 'ingrid.schmidt@greenenergy.de', '+49-30-1234-5679', 'Wind Farm', 25, 50000.00, 'Europe/Berlin', 1, GETDATE(), GETDATE(), 1, 1),
(9, 'Green Energy Solar Installation', 'GES-SOL-001', 3, 'Solarfeld 45', 13, 3, '20095', 53.5511, 9.9937, '+49-30-1234-5680', 'solar@greenenergy.de', 'Michael Weber', 'michael.weber@greenenergy.de', '+49-30-1234-5680', 'Solar Farm', 15, 30000.00, 'Europe/Berlin', 1, GETDATE(), GETDATE(), 1, 1),

-- Maritime Solutions Sites
(10, 'Maritime Solutions Oslo Office', 'MAR-HQ-001', 4, 'Strandveien 22', 36, 11, '0001', 59.9139, 10.7522, '+47-22-123456', 'oslo@maritime-solutions.no', 'Tor Hansen', 'tor.hansen@maritime-solutions.no', '+47-22-123456', 'Headquarters', 180, 2800.00, 'Europe/Oslo', 1, GETDATE(), GETDATE(), 1, 1),
(11, 'Maritime Shipyard Bergen', 'MAR-SY-001', 4, 'Verftsveien 78', 37, 11, '5001', 60.3913, 5.3221, '+47-22-123457', 'shipyard@maritime-solutions.no', 'Kari Olsen', 'kari.olsen@maritime-solutions.no', '+47-22-123457', 'Shipyard', 120, 12000.00, 'Europe/Oslo', 1, GETDATE(), GETDATE(), 1, 1),
(12, 'Maritime Port Facility', 'MAR-PORT-001', 4, 'Havneveien 34', 38, 11, '7001', 63.4305, 10.3951, '+47-22-123458', 'port@maritime-solutions.no', 'Ola Berg', 'ola.berg@maritime-solutions.no', '+47-22-123458', 'Port', 20, 5000.00, 'Europe/Oslo', 1, GETDATE(), GETDATE(), 1, 1),

-- Food Excellence Corp Sites
(13, 'Food Excellence Paris HQ', 'FOOD-HQ-001', 5, '78 Avenue de la Gastronomie', 16, 4, '75001', 48.8566, 2.3522, '+33-1-4567-8901', 'paris@foodexcellence.fr', 'Catherine Dubois', 'catherine.dubois@foodexcellence.fr', '+33-1-4567-8901', 'Headquarters', 120, 2500.00, 'Europe/Paris', 1, GETDATE(), GETDATE(), 1, 1),
(14, 'Food Production Facility Lyon', 'FOOD-PROD-001', 5, '234 Production Street', 18, 4, '69001', 45.7640, 4.8357, '+33-1-4567-8902', 'production@foodexcellence.fr', 'Pierre Martin', 'pierre.martin@foodexcellence.fr', '+33-1-4567-8902', 'Manufacturing', 280, 8500.00, 'Europe/Paris', 1, GETDATE(), GETDATE(), 1, 1),
(15, 'Food Distribution Center', 'FOOD-DIST-001', 5, '567 Distribution Way', 17, 4, '13001', 43.2965, 5.3698, '+33-1-4567-8903', 'distribution@foodexcellence.fr', 'Nicole Bernard', 'nicole.bernard@foodexcellence.fr', '+33-1-4567-8903', 'Distribution', 60, 6000.00, 'Europe/Paris', 1, GETDATE(), GETDATE(), 1, 1),

-- AutoTech Manufacturing Sites
(16, 'AutoTech Rome Headquarters', 'AUTO-HQ-001', 6, 'Via dell''Industria 42', 21, 5, '00118', 41.9028, 12.4964, '+39-02-1234-5678', 'roma@autotech.it', 'Giuseppe Rossi', 'giuseppe.rossi@autotech.it', '+39-02-1234-5678', 'Headquarters', 200, 3500.00, 'Europe/Rome', 1, GETDATE(), GETDATE(), 1, 1),
(17, 'AutoTech Milan Factory', 'AUTO-MFG-001', 6, '123 Automotive Boulevard', 22, 5, '20121', 45.4642, 9.1900, '+39-02-1234-5679', 'milano@autotech.it', 'Francesca Ferrari', 'francesca.ferrari@autotech.it', '+39-02-1234-5679', 'Manufacturing', 650, 18000.00, 'Europe/Rome', 1, GETDATE(), GETDATE(), 1, 1),
(18, 'AutoTech Turin R&D Center', 'AUTO-RD-001', 6, '456 Research Park', 24, 5, '10121', 45.0703, 7.6869, '+39-02-1234-5680', 'torino@autotech.it', 'Lorenzo Bianchi', 'lorenzo.bianchi@autotech.it', '+39-02-1234-5680', 'R&D', 180, 4500.00, 'Europe/Rome', 1, GETDATE(), GETDATE(), 1, 1),

-- Global Manufacturing Inc Sites (Multi-national)
(19, 'Global Mfg US Headquarters', 'GLOB-US-HQ', 17, '789 Global Plaza', 1, 1, '10002', 40.7128, -74.0060, '+1-212-555-0001', 'us-hq@globalmanufacturing.com', 'Robert Global', 'robert.global@globalmanufacturing.com', '+1-212-555-0001', 'Headquarters', 500, 8000.00, 'America/New_York', 1, GETDATE(), GETDATE(), 1, 1),
(20, 'Global Mfg UK Operations', 'GLOB-UK-OPS', 17, '101 Global Street', 6, 2, 'EC2A 2BB', 51.5074, -0.1278, '+44-20-5555-0001', 'uk-ops@globalmanufacturing.com', 'Sarah Global-UK', 'sarah.uk@globalmanufacturing.com', '+44-20-5555-0001', 'Manufacturing', 800, 12000.00, 'Europe/London', 1, GETDATE(), GETDATE(), 1, 1),
(21, 'Global Mfg Germany Plant', 'GLOB-DE-PLT', 17, '202 Global Straße', 11, 3, '10116', 52.5200, 13.4050, '+49-30-5555-0001', 'de-plant@globalmanufacturing.com', 'Hans Global-DE', 'hans.de@globalmanufacturing.com', '+49-30-5555-0001', 'Manufacturing', 950, 15000.00, 'Europe/Berlin', 1, GETDATE(), GETDATE(), 1, 1),
(22, 'Global Mfg Asia Pacific', 'GLOB-AP-HUB', 17, '303 Global Avenue', 44, 18, '018984', 1.3521, 103.8198, '+65-6555-0001', 'ap-hub@globalmanufacturing.com', 'Li Global-AP', 'li.ap@globalmanufacturing.com', '+65-6555-0001', 'Regional Hub', 420, 6500.00, 'Asia/Singapore', 1, GETDATE(), GETDATE(), 1, 1),

-- Small Company Sites
(23, 'Small Tech Office', 'SMALL-OFF-001', 19, '12 Startup Lane', 6, 2, 'SW1V 1AA', 51.5074, -0.1278, '+44-20-9999-1234', 'office@smalltech.co.uk', 'Sarah Startup', 'sarah@smalltech.co.uk', '+44-20-9999-1234', 'Office', 15, 200.00, 'Europe/London', 1, GETDATE(), GETDATE(), 1, 1),
(24, 'Family Food Kitchen', 'FAM-KITCHEN-001', 20, '5 Family Street', 16, 4, '75002', 48.8566, 2.3522, '+33-1-9999-8888', 'kitchen@familyfood.fr', 'Jean Family', 'jean@familyfood.fr', '+33-1-9999-8888', 'Production', 25, 500.00, 'Europe/Paris', 1, GETDATE(), GETDATE(), 1, 1),

-- Additional sites for variety
(25, 'Dutch Innovation Lab', 'DUTCH-LAB-001', 8, '156 Innovatielaan', 31, 7, '1011', 52.3676, 4.9041, '+31-20-8765-4321', 'lab@dutchinnovation.nl', 'Pieter van der Berg', 'pieter.vandenberg@dutchinnovation.nl', '+31-20-8765-4321', 'R&D', 80, 1200.00, 'Europe/Amsterdam', 1, GETDATE(), GETDATE(), 1, 1),
(26, 'EuroLogistics Main Hub', 'EURO-HUB-001', 9, '67 Logistiekstraat', 32, 8, '2000', 51.2194, 4.4025, '+32-3-567-8901', 'hub@eurologistics.be', 'Marie Janssen', 'marie.janssen@eurologistics.be', '+32-3-567-8901', 'Logistics Hub', 200, 25000.00, 'Europe/Brussels', 1, GETDATE(), GETDATE(), 1, 1),
(27, 'Alpine Precision Factory', 'ALP-FAC-001', 10, '34 Präzisionsweg', 9, 9, '8001', 47.3769, 8.5417, '+41-44-123-4567', 'factory@alpineprecision.ch', 'Hans Mueller', 'hans.mueller@alpineprecision.ch', '+41-44-123-4567', 'Manufacturing', 150, 3500.00, 'Europe/Zurich', 1, GETDATE(), GETDATE(), 1, 1),
(28, 'Nordic Forest Mill', 'NOR-MILL-001', 11, '12 Skogsvägen', 12, 12, '111 22', 59.3293, 18.0686, '+46-8-123-4567', 'mill@nordicforest.se', 'Erik Andersson', 'erik.andersson@nordicforest.se', '+46-8-123-4567', 'Manufacturing', 180, 12000.00, 'Europe/Stockholm', 1, GETDATE(), GETDATE(), 1, 1),
(29, 'Danish Wind Offshore Base', 'DAN-OFF-001', 12, '88 Vindenergivej', 13, 13, '2100', 55.6761, 12.5683, '+45-33-12-34-56', 'offshore@danishwind.dk', 'Morten Larsen', 'morten.larsen@danishwind.dk', '+45-33-12-34-56', 'Offshore Base', 80, 2000.00, 'Europe/Copenhagen', 1, GETDATE(), GETDATE(), 1, 1),
(30, 'Tokyo Precision Main Plant', 'TOK-MAIN-001', 13, '3-4-5 Shibuya, Shibuya-ku', 41, 15, '150-0002', 35.6762, 139.6503, '+81-3-1234-5678', 'main@tokyoprecision.co.jp', 'Hiroshi Tanaka', 'hiroshi.tanaka@tokyoprecision.co.jp', '+81-3-1234-5678', 'Manufacturing', 600, 10000.00, 'Asia/Tokyo', 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[Sites] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalSites FROM [dbo].[Sites];
SELECT SiteType, COUNT(*) as Count FROM [dbo].[Sites] GROUP BY SiteType ORDER BY Count DESC;
SELECT s.SiteName, c.CompanyName, co.CountryName, s.SiteType, s.EmployeeCount 
FROM [dbo].[Sites] s 
INNER JOIN [dbo].[Companies] c ON s.CompanyId = c.CompanyId 
LEFT JOIN [dbo].[Countries] co ON s.CountryId = co.CountryId 
ORDER BY c.CompanyName, s.SiteName;
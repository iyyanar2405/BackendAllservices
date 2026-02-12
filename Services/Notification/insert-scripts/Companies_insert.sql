-- Insert sample data for Companies table
-- This script creates sample customer companies for the portal

SET IDENTITY_INSERT [dbo].[Companies] ON;

INSERT INTO [dbo].[Companies] 
([CompanyId], [CompanyName], [CompanyCode], [Description], [Address], [CityId], [CountryId], [PostalCode], [Phone], [Email], [Website], [ContactPerson], [ContactEmail], [ContactPhone], [Industry], [EmployeeCount], [TaxId], [RegistrationNumber], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- US Companies
(1, 'Acme Corporation', 'ACME-001', 'Leading manufacturing company specializing in industrial equipment and machinery', '123 Industrial Blvd', 1, 1, '10001', '+1-555-123-4567', 'info@acmecorp.com', 'https://www.acmecorp.com', 'John Doe', 'john.doe@acmecorp.com', '+1-555-123-4567', 'Manufacturing', 2500, '12-3456789', 'REG-NY-001', 1, GETDATE(), GETDATE(), 1, 1),

-- UK Companies
(2, 'TechFlow Industries Ltd', 'TECHFLOW-UK', 'Technology solutions provider with focus on digital transformation and cloud services', '45 Technology Park', 6, 2, 'SW1A 1AA', '+44-20-1234-5678', 'info@techflow.com', 'https://www.techflow.com', 'Peter White', 'peter.white@techflow.com', '+44-20-1234-5678', 'Technology', 850, 'GB123456789', 'REG-UK-002', 1, GETDATE(), GETDATE(), 1, 1),

-- German Companies
(3, 'Green Energy Solutions GmbH', 'GES-DE', 'Renewable energy company specializing in wind and solar power solutions', 'Umweltstraße 15', 11, 3, '10115', '+49-30-1234-5678', 'info@greenenergy.de', 'https://www.greenenergy.de', 'Andreas Mueller', 'andreas.mueller@greenenergy.de', '+49-30-1234-5678', 'Renewable Energy', 450, 'DE123456789', 'REG-DE-003', 1, GETDATE(), GETDATE(), 1, 1),

-- Norwegian Companies
(4, 'Maritime Solutions AS', 'MARISOL-NO', 'Maritime services company providing shipping and offshore solutions', 'Strandveien 22', 36, 11, '0001', '+47-22-123456', 'info@maritime-solutions.no', 'https://www.maritime-solutions.no', 'Tor Hansen', 'tor.hansen@maritime-solutions.no', '+47-22-123456', 'Maritime', 320, 'NO123456789', 'REG-NO-004', 1, GETDATE(), GETDATE(), 1, 1),

-- French Companies
(5, 'Food Excellence Corp', 'FOODEX-FR', 'Premium food manufacturing company with international distribution network', '78 Avenue de la Gastronomie', 16, 4, '75001', '+33-1-4567-8901', 'info@foodexcellence.fr', 'https://www.foodexcellence.fr', 'Catherine Dubois', 'catherine.dubois@foodexcellence.fr', '+33-1-4567-8901', 'Food Manufacturing', 680, 'FR123456789', 'REG-FR-005', 1, GETDATE(), GETDATE(), 1, 1),

-- Italian Companies
(6, 'AutoTech Manufacturing SpA', 'AUTOTECH-IT', 'Automotive component manufacturer specializing in precision engineering', 'Via dell\'Industria 42', 21, 5, '00118', '+39-02-1234-5678', 'info@autotech.it', 'https://www.autotech.it', 'Giuseppe Rossi', 'giuseppe.rossi@autotech.it', '+39-02-1234-5678', 'Automotive', 1200, 'IT123456789', 'REG-IT-006', 1, GETDATE(), GETDATE(), 1, 1),

-- Spanish Companies
(7, 'Iberica Construcciones SA', 'IBERICA-ES', 'Construction and infrastructure development company', 'Calle de la Construcción 89', 26, 6, '28001', '+34-91-9876-5432', 'info@iberica.es', 'https://www.iberica.es', 'Carlos Mendez', 'carlos.mendez@iberica.es', '+34-91-9876-5432', 'Construction', 950, 'ES123456789', 'REG-ES-007', 1, GETDATE(), GETDATE(), 1, 1),

-- Dutch Companies
(8, 'Dutch Innovation BV', 'DUTCHINNOV-NL', 'Innovation-driven technology company focused on sustainable solutions', 'Innovatielaan 156', 31, 7, '1011', '+31-20-8765-4321', 'info@dutchinnovation.nl', 'https://www.dutchinnovation.nl', 'Pieter van der Berg', 'pieter.vandenberg@dutchinnovation.nl', '+31-20-8765-4321', 'Technology', 380, 'NL123456789', 'REG-NL-008', 1, GETDATE(), GETDATE(), 1, 1),

-- Belgian Companies
(9, 'EuroLogistics BVBA', 'EUROLOG-BE', 'European logistics and supply chain management company', 'Logistiekstraat 67', 32, 8, '2000', '+32-3-567-8901', 'info@eurologistics.be', 'https://www.eurologistics.be', 'Marie Janssen', 'marie.janssen@eurologistics.be', '+32-3-567-8901', 'Logistics', 520, 'BE123456789', 'REG-BE-009', 1, GETDATE(), GETDATE(), 1, 1),

-- Swiss Companies
(10, 'Alpine Precision AG', 'ALPINE-CH', 'High-precision manufacturing company serving aerospace and medical device industries', 'Präzisionsweg 34', 9, 9, '8001', '+41-44-123-4567', 'info@alpineprecision.ch', 'https://www.alpineprecision.ch', 'Hans Mueller', 'hans.mueller@alpineprecision.ch', '+41-44-123-4567', 'Precision Manufacturing', 280, 'CH123456789', 'REG-CH-010', 1, GETDATE(), GETDATE(), 1, 1),

-- Swedish Companies
(11, 'Nordic Forest Products AB', 'NORFOR-SE', 'Sustainable forestry and wood products company', 'Skogsvägen 12', 12, 12, '111 22', '+46-8-123-4567', 'info@nordicforest.se', 'https://www.nordicforest.se', 'Erik Andersson', 'erik.andersson@nordicforest.se', '+46-8-123-4567', 'Forestry', 650, 'SE123456789', 'REG-SE-011', 1, GETDATE(), GETDATE(), 1, 1),

-- Danish Companies
(12, 'Danish Wind Energy A/S', 'DANWIND-DK', 'Wind energy solutions and offshore wind farm development', 'Vindenergivej 88', 13, 13, '2100', '+45-33-12-34-56', 'info@danishwind.dk', 'https://www.danishwind.dk', 'Morten Larsen', 'morten.larsen@danishwind.dk', '+45-33-12-34-56', 'Renewable Energy', 420, 'DK123456789', 'REG-DK-012', 1, GETDATE(), GETDATE(), 1, 1),

-- Japanese Companies
(13, 'Tokyo Precision Industries KK', 'TOKPREC-JP', 'Advanced manufacturing company specializing in electronic components', '3-4-5 Shibuya, Shibuya-ku', 41, 15, '150-0002', '+81-3-1234-5678', 'info@tokyoprecision.co.jp', 'https://www.tokyoprecision.co.jp', 'Hiroshi Tanaka', 'hiroshi.tanaka@tokyoprecision.co.jp', '+81-3-1234-5678', 'Electronics', 1800, 'JP123456789', 'REG-JP-013', 1, GETDATE(), GETDATE(), 1, 1),

-- Singapore Companies
(14, 'Southeast Asia Marine Pte Ltd', 'SEAMARINE-SG', 'Marine services and offshore operations company', '10 Marina Boulevard #15-01', 44, 18, '018983', '+65-6234-5678', 'info@seamarine.com.sg', 'https://www.seamarine.com.sg', 'Kevin Lee', 'kevin.lee@seamarine.com.sg', '+65-6234-5678', 'Marine Services', 290, 'SG123456789', 'REG-SG-014', 1, GETDATE(), GETDATE(), 1, 1),

-- Australian Companies
(15, 'Aussie Mining Solutions Pty Ltd', 'AUSMIN-AU', 'Mining equipment and services company serving the Australian market', '123 Mining Street', 45, 19, '2000', '+61-2-9876-5432', 'info@aussiemining.com.au', 'https://www.aussiemining.com.au', 'James Wilson', 'james.wilson@aussiemining.com.au', '+61-2-9876-5432', 'Mining', 750, 'AU123456789', 'REG-AU-015', 1, GETDATE(), GETDATE(), 1, 1),

-- Canadian Companies
(16, 'Canadian Energy Systems Inc', 'CANENERGY-CA', 'Energy infrastructure and renewable energy solutions provider', '456 Energy Drive', 46, 21, 'M5H 2N2', '+1-416-123-4567', 'info@canadianenergy.ca', 'https://www.canadianenergy.ca', 'Michael Thompson', 'michael.thompson@canadianenergy.ca', '+1-416-123-4567', 'Energy', 580, 'CA123456789', 'REG-CA-016', 1, GETDATE(), GETDATE(), 1, 1),

-- Multi-site Companies
(17, 'Global Manufacturing Inc', 'GLOBMAN-MULTI', 'International manufacturing conglomerate with operations in multiple countries', '789 Global Plaza', 1, 1, '10002', '+1-212-555-0001', 'info@globalmanufacturing.com', 'https://www.globalmanufacturing.com', 'Robert Global', 'robert.global@globalmanufacturing.com', '+1-212-555-0001', 'Manufacturing', 15000, 'US987654321', 'REG-GLOBAL-017', 1, GETDATE(), GETDATE(), 1, 1),

(18, 'European Technical Services Ltd', 'EUROTECH-MULTI', 'Pan-European technical services and consulting company', '321 European Way', 6, 2, 'EC1A 1BB', '+44-20-7123-4567', 'info@eurotech.eu', 'https://www.eurotech.eu', 'Alexandra European', 'alexandra.european@eurotech.eu', '+44-20-7123-4567', 'Technical Services', 3200, 'GB987654321', 'REG-EURO-018', 1, GETDATE(), GETDATE(), 1, 1),

-- Small Companies for Testing
(19, 'Small Tech Startup', 'SMALLTECH-001', 'Innovative technology startup developing AI solutions', '12 Startup Lane', 6, 2, 'SW1V 1AA', '+44-20-9999-1234', 'hello@smalltech.co.uk', 'https://www.smalltech.co.uk', 'Sarah Startup', 'sarah@smalltech.co.uk', '+44-20-9999-1234', 'Technology', 15, 'GB999888777', 'REG-SMALL-019', 1, GETDATE(), GETDATE(), 1, 1),

(20, 'Family Food Business', 'FAMFOOD-001', 'Traditional family-owned food production business', '5 Family Street', 16, 4, '75002', '+33-1-9999-8888', 'contact@familyfood.fr', 'https://www.familyfood.fr', 'Jean Family', 'jean@familyfood.fr', '+33-1-9999-8888', 'Food Production', 25, 'FR999888777', 'REG-FAMILY-020', 1, GETDATE(), GETDATE(), 1, 1);

SET IDENTITY_INSERT [dbo].[Companies] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalCompanies FROM [dbo].[Companies];
SELECT Industry, COUNT(*) as Count FROM [dbo].[Companies] GROUP BY Industry ORDER BY Count DESC;
SELECT c.CompanyName, co.CountryName, ci.CityName, c.EmployeeCount 
FROM [dbo].[Companies] c 
LEFT JOIN [dbo].[Countries] co ON c.CountryId = co.CountryId 
LEFT JOIN [dbo].[Cities] ci ON c.CityId = ci.CityId 
ORDER BY c.EmployeeCount DESC;
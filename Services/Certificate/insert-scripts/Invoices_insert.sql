-- Insert sample data for Invoices table
-- This script creates invoice records for audit services

SET IDENTITY_INSERT [dbo].[Invoices] ON;

INSERT INTO [dbo].[Invoices] 
([InvoiceId], [InvoiceNumber], [CompanyId], [AuditId], [ServiceId], [InvoiceDate], [DueDate], [Amount], [Currency], [TaxAmount], [TotalAmount], [Status], [PaymentDate], [PaymentMethod], [Description], [BillingAddress], [IsActive], [CreatedDate], [ModifiedDate], [CreatedBy], [ModifiedBy])
VALUES
-- Acme Corporation Invoices
(1, 'DNV-INV-2024-001', 1, 1, 1, '2024-03-20', '2024-04-19', 15000.00, 'USD', 2250.00, 17250.00, 'Paid', '2024-04-15', 'Bank Transfer', 'ISO 9001:2015 Initial Certification Audit', '123 Industrial Blvd, New York, NY 10001, USA', 1, '2024-03-20', '2024-04-15', 18, 18),
(2, 'DNV-INV-2024-002', 1, 3, 2, '2024-05-25', '2024-06-24', 16000.00, 'USD', 2400.00, 18400.00, 'Paid', '2024-06-20', 'Bank Transfer', 'ISO 14001:2015 Initial Certification Audit', '123 Industrial Blvd, New York, NY 10001, USA', 1, '2024-05-25', '2024-06-20', 18, 18),
(3, 'DNV-INV-2025-001', 1, 2, 1, '2025-03-13', '2025-04-12', 8000.00, 'USD', 1200.00, 9200.00, 'Paid', '2025-04-08', 'Credit Card', 'ISO 9001:2015 Surveillance Year 1', '123 Industrial Blvd, New York, NY 10001, USA', 1, '2025-03-13', '2025-04-08', 18, 18),
(4, 'DNV-INV-2025-002', 1, 4, 2, '2025-05-18', '2025-06-17', 8500.00, 'USD', 1275.00, 9775.00, 'Outstanding', NULL, NULL, 'ISO 14001:2015 Surveillance Year 1', '123 Industrial Blvd, New York, NY 10001, USA', 1, '2025-05-18', '2025-05-18', 18, 18),
(5, 'DNV-INV-2025-003', 1, 5, 3, '2025-07-12', '2025-08-11', 7000.00, 'USD', 1050.00, 8050.00, 'Pending', NULL, NULL, 'ISO 45001:2018 Pre-Assessment', '123 Industrial Blvd, New York, NY 10001, USA', 1, '2025-07-12', '2025-07-12', 18, 18),

-- TechFlow Industries Invoices
(6, 'DNV-INV-2023-001', 2, 6, 1, '2023-09-10', '2023-10-10', 18000.00, 'GBP', 3600.00, 21600.00, 'Paid', '2023-10-05', 'Bank Transfer', 'ISO 9001:2015 Initial Certification Audit', '45 Technology Park, London, SW1A 1AA, UK', 1, '2023-09-10', '2023-10-05', 18, 18),
(7, 'DNV-INV-2024-003', 2, 9, 4, '2024-01-20', '2024-02-19', 20000.00, 'GBP', 4000.00, 24000.00, 'Paid', '2024-02-15', 'Bank Transfer', 'ISO 27001:2013 Initial Certification Audit', '45 Technology Park, London, SW1A 1AA, UK', 1, '2024-01-20', '2024-02-15', 18, 18),
(8, 'DNV-INV-2024-004', 2, 7, 1, '2024-09-05', '2024-10-05', 9000.00, 'GBP', 1800.00, 10800.00, 'Paid', '2024-09-28', 'Credit Card', 'ISO 9001:2015 Surveillance Year 1', '45 Technology Park, London, SW1A 1AA, UK', 1, '2024-09-05', '2024-09-28', 18, 18),
(9, 'DNV-INV-2025-004', 2, 10, 4, '2025-01-16', '2025-02-15', 10000.00, 'GBP', 2000.00, 12000.00, 'Paid', '2025-02-10', 'Bank Transfer', 'ISO 27001:2013 Surveillance Year 1', '45 Technology Park, London, SW1A 1AA, UK', 1, '2025-01-16', '2025-02-10', 18, 18),
(10, 'DNV-INV-2025-005', 2, 8, 1, '2025-09-04', '2025-10-04', 9500.00, 'GBP', 1900.00, 11400.00, 'Pending', NULL, NULL, 'ISO 9001:2015 Surveillance Year 2', '45 Technology Park, London, SW1A 1AA, UK', 1, '2025-09-04', '2025-09-04', 18, 18),

-- Green Energy Solutions Invoices
(11, 'DNV-INV-2024-005', 3, 11, 2, '2024-02-17', '2024-03-19', 17000.00, 'EUR', 3230.00, 20230.00, 'Paid', '2024-03-15', 'Bank Transfer', 'ISO 14001:2015 Initial Certification Audit', 'Umweltstraße 15, 10115 Berlin, Germany', 1, '2024-02-17', '2024-03-15', 18, 18),
(12, 'DNV-INV-2024-006', 3, 12, 23, '2024-06-08', '2024-07-08', 35000.00, 'EUR', 6650.00, 41650.00, 'Paid', '2024-07-02', 'Bank Transfer', 'Wind Turbine Type Certification', 'Umweltstraße 15, 10115 Berlin, Germany', 1, '2024-06-08', '2024-07-02', 18, 18),
(13, 'DNV-INV-2024-007', 3, 14, 24, '2024-08-24', '2024-09-23', 25000.00, 'EUR', 4750.00, 29750.00, 'Paid', '2024-09-18', 'Bank Transfer', 'Solar Panel Installation Certification', 'Umweltstraße 15, 10115 Berlin, Germany', 1, '2024-08-24', '2024-09-18', 18, 18),
(14, 'DNV-INV-2025-006', 3, 13, 23, '2025-06-05', '2025-07-05', 15000.00, 'EUR', 2850.00, 17850.00, 'Outstanding', NULL, NULL, 'Wind Turbine Surveillance Audit', 'Umweltstraße 15, 10115 Berlin, Germany', 1, '2025-06-05', '2025-06-05', 18, 18),
(15, 'DNV-INV-2025-007', 3, 15, 7, '2025-10-16', '2025-11-15', 8000.00, 'EUR', 1520.00, 9520.00, 'Pending', NULL, NULL, 'ISO 50001:2018 Pre-Assessment', 'Umweltstraße 15, 10115 Berlin, Germany', 1, '2025-10-16', '2025-10-16', 18, 18),

-- Maritime Solutions Invoices
(16, 'DNV-INV-2023-002', 4, 16, 1, '2023-11-11', '2023-12-11', 22000.00, 'NOK', 5500.00, 27500.00, 'Paid', '2023-12-05', 'Bank Transfer', 'ISO 9001:2015 Initial Certification Audit', 'Strandveien 22, 0001 Oslo, Norway', 1, '2023-11-11', '2023-12-05', 18, 18),
(17, 'DNV-INV-2024-008', 4, 17, 22, '2024-04-13', '2024-05-13', 45000.00, 'NOK', 11250.00, 56250.00, 'Paid', '2024-05-08', 'Bank Transfer', 'DNV Rules for Ships Classification Certificate', 'Strandveien 22, 0001 Oslo, Norway', 1, '2024-04-13', '2024-05-08', 18, 18),
(18, 'DNV-INV-2024-009', 4, 19, 3, '2024-09-21', '2024-10-21', 19000.00, 'NOK', 4750.00, 23750.00, 'Paid', '2024-10-15', 'Credit Card', 'ISO 45001:2018 Initial Certification Audit', 'Strandveien 22, 0001 Oslo, Norway', 1, '2024-09-21', '2024-10-15', 18, 18),
(19, 'DNV-INV-2025-008', 4, 18, 22, '2025-04-10', '2025-05-10', 18000.00, 'NOK', 4500.00, 22500.00, 'Outstanding', NULL, NULL, 'DNV Rules for Ships Surveillance Audit', 'Strandveien 22, 0001 Oslo, Norway', 1, '2025-04-10', '2025-04-10', 18, 18),

-- Food Excellence Corp Invoices
(20, 'DNV-INV-2024-010', 5, 20, 5, '2024-01-27', '2024-02-26', 21000.00, 'EUR', 4200.00, 25200.00, 'Paid', '2024-02-20', 'Bank Transfer', 'ISO 22000:2018 Initial Certification Audit', '78 Avenue de la Gastronomie, 75001 Paris, France', 1, '2024-01-27', '2024-02-20', 18, 18),
(21, 'DNV-INV-2024-011', 5, 21, 28, '2024-03-16', '2024-04-15', 18000.00, 'EUR', 3600.00, 21600.00, 'Paid', '2024-04-10', 'Bank Transfer', 'HACCP Certification Audit', '78 Avenue de la Gastronomie, 75001 Paris, France', 1, '2024-03-16', '2024-04-10', 18, 18),
(22, 'DNV-INV-2024-012', 5, 22, 29, '2024-07-06', '2024-08-05', 23000.00, 'EUR', 4600.00, 27600.00, 'Paid', '2024-07-30', 'Credit Card', 'BRC Food Safety Initial Certification', '78 Avenue de la Gastronomie, 75001 Paris, France', 1, '2024-07-06', '2024-07-30', 18, 18),
(23, 'DNV-INV-2025-009', 5, 23, 5, '2025-01-23', '2025-02-22', 9000.00, 'EUR', 1800.00, 10800.00, 'Paid', '2025-02-18', 'Bank Transfer', 'ISO 22000:2018 Surveillance Year 1', '78 Avenue de la Gastronomie, 75001 Paris, France', 1, '2025-01-23', '2025-02-18', 18, 18),

-- AutoTech Manufacturing Invoices
(24, 'DNV-INV-2023-003', 6, 24, 1, '2023-06-17', '2023-07-17', 19000.00, 'EUR', 4180.00, 23180.00, 'Paid', '2023-07-12', 'Bank Transfer', 'ISO 9001:2015 Initial Certification Audit', 'Via dell''Industria 42, 00118 Rome, Italy', 1, '2023-06-17', '2023-07-12', 18, 18),
(25, 'DNV-INV-2023-004', 6, 25, 8, '2023-10-28', '2023-11-27', 28000.00, 'EUR', 6160.00, 34160.00, 'Paid', '2023-11-20', 'Bank Transfer', 'IATF 16949:2016 Initial Certification Audit', 'Via dell''Industria 42, 00118 Rome, Italy', 1, '2023-10-28', '2023-11-20', 18, 18),
(26, 'DNV-INV-2024-013', 6, 26, 8, '2024-10-24', '2024-11-23', 12000.00, 'EUR', 2640.00, 14640.00, 'Paid', '2024-11-18', 'Credit Card', 'IATF 16949:2016 Surveillance Year 1', 'Via dell''Industria 42, 00118 Rome, Italy', 1, '2024-10-24', '2024-11-18', 18, 18),
(27, 'DNV-INV-2024-014', 6, 27, 11, '2024-12-07', '2025-01-06', 32000.00, 'EUR', 7040.00, 39040.00, 'Outstanding', NULL, NULL, 'ISO 26262 Functional Safety Certification', 'Via dell''Industria 42, 00118 Rome, Italy', 1, '2024-12-07', '2024-12-07', 18, 18),

-- Global Manufacturing Inc Invoices (Large Multi-site)
(28, 'DNV-INV-2024-015', 17, 28, 1, '2024-05-18', '2024-06-17', 75000.00, 'USD', 11250.00, 86250.00, 'Paid', '2024-06-10', 'Wire Transfer', 'ISO 9001:2015 Multi-Site Initial Certification', '789 Global Plaza, New York, NY 10002, USA', 1, '2024-05-18', '2024-06-10', 18, 18),
(29, 'DNV-INV-2024-016', 17, 29, 2, '2024-08-24', '2024-09-23', 80000.00, 'USD', 12000.00, 92000.00, 'Paid', '2024-09-15', 'Wire Transfer', 'ISO 14001:2015 Multi-Site Initial Certification', '789 Global Plaza, New York, NY 10002, USA', 1, '2024-08-24', '2024-09-15', 18, 18),
(30, 'DNV-INV-2025-010', 17, 30, 1, '2025-05-10', '2025-06-09', 35000.00, 'USD', 5250.00, 40250.00, 'Outstanding', NULL, NULL, 'ISO 9001:2015 Multi-Site Surveillance', '789 Global Plaza, New York, NY 10002, USA', 1, '2025-05-10', '2025-05-10', 18, 18),

-- Small Company Invoices
(31, 'DNV-INV-2024-017', 19, 31, 4, '2024-11-23', '2024-12-23', 16000.00, 'GBP', 3200.00, 19200.00, 'Paid', '2024-12-18', 'Credit Card', 'ISO 27001:2013 Initial Certification Audit', '12 Startup Lane, London, SW1V 1AA, UK', 1, '2024-11-23', '2024-12-18', 18, 18),
(32, 'DNV-INV-2025-011', 20, 32, 5, '2025-02-22', '2025-03-24', 14000.00, 'EUR', 2800.00, 16800.00, 'Paid', '2025-03-20', 'Bank Transfer', 'ISO 22000:2018 Initial Certification Audit', '5 Family Street, 75002 Paris, France', 1, '2025-02-22', '2025-03-20', 18, 18),

-- Planned future invoices
(33, 'DNV-INV-2025-012', 2, 34, 10, '2025-11-15', '2025-12-15', 22000.00, 'GBP', 4400.00, 26400.00, 'Pending', NULL, NULL, 'ISO 20000-1:2018 Initial Certification Audit', '45 Technology Park, London, SW1A 1AA, UK', 1, '2025-11-15', '2025-11-15', 18, 18),
(34, 'DNV-INV-2025-013', 3, 35, 26, '2025-12-06', '2026-01-05', 28000.00, 'EUR', 5320.00, 33320.00, 'Pending', NULL, NULL, 'Carbon Footprint Verification Certification', 'Umweltstraße 15, 10115 Berlin, Germany', 1, '2025-12-06', '2025-12-06', 18, 18),

-- Overdue invoice example
(35, 'DNV-INV-2024-018', 8, NULL, 1, '2024-06-15', '2024-07-15', 15000.00, 'EUR', 2850.00, 17850.00, 'Overdue', NULL, NULL, 'Consultation Services - Quality Management', '156 Innovatielaan, 1011 Amsterdam, Netherlands', 1, '2024-06-15', '2024-08-15', 18, 18);

SET IDENTITY_INSERT [dbo].[Invoices] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalInvoices FROM [dbo].[Invoices];

-- Show invoice status distribution
SELECT 
    Status,
    COUNT(*) as Count,
    SUM(TotalAmount) as TotalAmount,
    Currency
FROM [dbo].[Invoices] 
GROUP BY Status, Currency
ORDER BY Status, Currency;

-- Show invoices by company
SELECT 
    c.CompanyName,
    COUNT(inv.InvoiceId) as InvoiceCount,
    SUM(inv.TotalAmount) as TotalBilled,
    inv.Currency,
    SUM(CASE WHEN inv.Status = 'Paid' THEN inv.TotalAmount ELSE 0 END) as TotalPaid,
    SUM(CASE WHEN inv.Status IN ('Outstanding', 'Overdue') THEN inv.TotalAmount ELSE 0 END) as TotalOutstanding
FROM [dbo].[Companies] c
INNER JOIN [dbo].[Invoices] inv ON c.CompanyId = inv.CompanyId
GROUP BY c.CompanyId, c.CompanyName, inv.Currency
ORDER BY TotalBilled DESC;

-- Show overdue invoices
SELECT 
    inv.InvoiceNumber,
    c.CompanyName,
    inv.InvoiceDate,
    inv.DueDate,
    inv.TotalAmount,
    inv.Currency,
    DATEDIFF(day, inv.DueDate, GETDATE()) as DaysOverdue
FROM [dbo].[Invoices] inv
INNER JOIN [dbo].[Companies] c ON inv.CompanyId = c.CompanyId
WHERE inv.Status IN ('Outstanding', 'Overdue') 
AND inv.DueDate < GETDATE()
ORDER BY DaysOverdue DESC;

-- Show revenue by month
SELECT 
    YEAR(inv.InvoiceDate) as Year,
    MONTH(inv.InvoiceDate) as Month,
    inv.Currency,
    COUNT(inv.InvoiceId) as InvoiceCount,
    SUM(inv.TotalAmount) as MonthlyRevenue
FROM [dbo].[Invoices] inv
WHERE inv.Status = 'Paid'
GROUP BY YEAR(inv.InvoiceDate), MONTH(inv.InvoiceDate), inv.Currency
ORDER BY Year DESC, Month DESC, Currency;

-- Show payment method distribution
SELECT 
    PaymentMethod,
    COUNT(*) as Count,
    SUM(TotalAmount) as TotalAmount
FROM [dbo].[Invoices] 
WHERE Status = 'Paid'
GROUP BY PaymentMethod
ORDER BY TotalAmount DESC;
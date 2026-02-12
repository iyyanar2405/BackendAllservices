-- Insert sample data for Countries table
-- This script provides a comprehensive list of countries for the customer portal

SET IDENTITY_INSERT [dbo].[Countries] ON;

INSERT INTO [dbo].[Countries] 
([CountryId], [CountryName], [CountryCode], [CountryCodeAlpha2], [Region], [Continent], [Currency], [DisplayOrder], [IsActive], [CreatedDate], [ModifiedDate])
VALUES
-- Major markets
(1, 'United States', 'USA', 'US', 'North America', 'North America', 'USD', 1, 1, GETDATE(), GETDATE()),
(2, 'United Kingdom', 'GBR', 'GB', 'Western Europe', 'Europe', 'GBP', 2, 1, GETDATE(), GETDATE()),
(3, 'Germany', 'DEU', 'DE', 'Western Europe', 'Europe', 'EUR', 3, 1, GETDATE(), GETDATE()),
(4, 'France', 'FRA', 'FR', 'Western Europe', 'Europe', 'EUR', 4, 1, GETDATE(), GETDATE()),
(5, 'Italy', 'ITA', 'IT', 'Southern Europe', 'Europe', 'EUR', 5, 1, GETDATE(), GETDATE()),
(6, 'Spain', 'ESP', 'ES', 'Southern Europe', 'Europe', 'EUR', 6, 1, GETDATE(), GETDATE()),
(7, 'Netherlands', 'NLD', 'NL', 'Western Europe', 'Europe', 'EUR', 7, 1, GETDATE(), GETDATE()),
(8, 'Belgium', 'BEL', 'BE', 'Western Europe', 'Europe', 'EUR', 8, 1, GETDATE(), GETDATE()),
(9, 'Switzerland', 'CHE', 'CH', 'Western Europe', 'Europe', 'CHF', 9, 1, GETDATE(), GETDATE()),
(10, 'Austria', 'AUT', 'AT', 'Western Europe', 'Europe', 'EUR', 10, 1, GETDATE(), GETDATE()),

-- Nordic countries
(11, 'Norway', 'NOR', 'NO', 'Northern Europe', 'Europe', 'NOK', 11, 1, GETDATE(), GETDATE()),
(12, 'Sweden', 'SWE', 'SE', 'Northern Europe', 'Europe', 'SEK', 12, 1, GETDATE(), GETDATE()),
(13, 'Denmark', 'DNK', 'DK', 'Northern Europe', 'Europe', 'DKK', 13, 1, GETDATE(), GETDATE()),
(14, 'Finland', 'FIN', 'FI', 'Northern Europe', 'Europe', 'EUR', 14, 1, GETDATE(), GETDATE()),

-- Asia Pacific
(15, 'Japan', 'JPN', 'JP', 'East Asia', 'Asia', 'JPY', 15, 1, GETDATE(), GETDATE()),
(16, 'China', 'CHN', 'CN', 'East Asia', 'Asia', 'CNY', 16, 1, GETDATE(), GETDATE()),
(17, 'South Korea', 'KOR', 'KR', 'East Asia', 'Asia', 'KRW', 17, 1, GETDATE(), GETDATE()),
(18, 'Singapore', 'SGP', 'SG', 'Southeast Asia', 'Asia', 'SGD', 18, 1, GETDATE(), GETDATE()),
(19, 'Australia', 'AUS', 'AU', 'Oceania', 'Oceania', 'AUD', 19, 1, GETDATE(), GETDATE()),
(20, 'New Zealand', 'NZL', 'NZ', 'Oceania', 'Oceania', 'NZD', 20, 1, GETDATE(), GETDATE()),

-- Other major markets
(21, 'Canada', 'CAN', 'CA', 'North America', 'North America', 'CAD', 21, 1, GETDATE(), GETDATE()),
(22, 'Brazil', 'BRA', 'BR', 'South America', 'South America', 'BRL', 22, 1, GETDATE(), GETDATE()),
(23, 'Mexico', 'MEX', 'MX', 'North America', 'North America', 'MXN', 23, 1, GETDATE(), GETDATE()),
(24, 'India', 'IND', 'IN', 'South Asia', 'Asia', 'INR', 24, 1, GETDATE(), GETDATE()),
(25, 'South Africa', 'ZAF', 'ZA', 'Southern Africa', 'Africa', 'ZAR', 25, 1, GETDATE(), GETDATE()),

-- Additional European countries
(26, 'Poland', 'POL', 'PL', 'Eastern Europe', 'Europe', 'PLN', 26, 1, GETDATE(), GETDATE()),
(27, 'Czech Republic', 'CZE', 'CZ', 'Eastern Europe', 'Europe', 'CZK', 27, 1, GETDATE(), GETDATE()),
(28, 'Hungary', 'HUN', 'HU', 'Eastern Europe', 'Europe', 'HUF', 28, 1, GETDATE(), GETDATE()),
(29, 'Portugal', 'PRT', 'PT', 'Southern Europe', 'Europe', 'EUR', 29, 1, GETDATE(), GETDATE()),
(30, 'Ireland', 'IRL', 'IE', 'Western Europe', 'Europe', 'EUR', 30, 1, GETDATE(), GETDATE());

SET IDENTITY_INSERT [dbo].[Countries] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalCountries FROM [dbo].[Countries];
SELECT TOP 5 * FROM [dbo].[Countries] ORDER BY DisplayOrder;
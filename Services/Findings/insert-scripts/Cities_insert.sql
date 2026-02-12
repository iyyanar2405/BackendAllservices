-- Insert sample data for Cities table
-- This script provides major cities for the countries in our system

SET IDENTITY_INSERT [dbo].[Cities] ON;

INSERT INTO [dbo].[Cities] 
([CityId], [CityName], [CountryId], [StateProvince], [PostalCode], [Latitude], [Longitude], [TimeZone], [DisplayOrder], [IsActive], [CreatedDate], [ModifiedDate])
VALUES
-- United States cities
(1, 'New York', 1, 'New York', '10001', 40.7128, -74.0060, 'America/New_York', 1, 1, GETDATE(), GETDATE()),
(2, 'Los Angeles', 1, 'California', '90001', 34.0522, -118.2437, 'America/Los_Angeles', 2, 1, GETDATE(), GETDATE()),
(3, 'Chicago', 1, 'Illinois', '60601', 41.8781, -87.6298, 'America/Chicago', 3, 1, GETDATE(), GETDATE()),
(4, 'Houston', 1, 'Texas', '77001', 29.7604, -95.3698, 'America/Chicago', 4, 1, GETDATE(), GETDATE()),
(5, 'Philadelphia', 1, 'Pennsylvania', '19101', 39.9526, -75.1652, 'America/New_York', 5, 1, GETDATE(), GETDATE()),

-- United Kingdom cities
(6, 'London', 2, 'England', 'SW1A 1AA', 51.5074, -0.1278, 'Europe/London', 6, 1, GETDATE(), GETDATE()),
(7, 'Manchester', 2, 'England', 'M1 1AA', 53.4808, -2.2426, 'Europe/London', 7, 1, GETDATE(), GETDATE()),
(8, 'Birmingham', 2, 'England', 'B1 1AA', 52.4862, -1.8904, 'Europe/London', 8, 1, GETDATE(), GETDATE()),
(9, 'Glasgow', 2, 'Scotland', 'G1 1AA', 55.8642, -4.2518, 'Europe/London', 9, 1, GETDATE(), GETDATE()),
(10, 'Edinburgh', 2, 'Scotland', 'EH1 1AA', 55.9533, -3.1883, 'Europe/London', 10, 1, GETDATE(), GETDATE()),

-- Germany cities
(11, 'Berlin', 3, 'Berlin', '10115', 52.5200, 13.4050, 'Europe/Berlin', 11, 1, GETDATE(), GETDATE()),
(12, 'Munich', 3, 'Bavaria', '80331', 48.1351, 11.5820, 'Europe/Berlin', 12, 1, GETDATE(), GETDATE()),
(13, 'Hamburg', 3, 'Hamburg', '20095', 53.5511, 9.9937, 'Europe/Berlin', 13, 1, GETDATE(), GETDATE()),
(14, 'Frankfurt', 3, 'Hesse', '60311', 50.1109, 8.6821, 'Europe/Berlin', 14, 1, GETDATE(), GETDATE()),
(15, 'Cologne', 3, 'North Rhine-Westphalia', '50667', 50.9375, 6.9603, 'Europe/Berlin', 15, 1, GETDATE(), GETDATE()),

-- France cities
(16, 'Paris', 4, 'Île-de-France', '75001', 48.8566, 2.3522, 'Europe/Paris', 16, 1, GETDATE(), GETDATE()),
(17, 'Marseille', 4, 'Provence-Alpes-Côte d''Azur', '13001', 43.2965, 5.3698, 'Europe/Paris', 17, 1, GETDATE(), GETDATE()),
(18, 'Lyon', 4, 'Auvergne-Rhône-Alpes', '69001', 45.7640, 4.8357, 'Europe/Paris', 18, 1, GETDATE(), GETDATE()),
(19, 'Toulouse', 4, 'Occitanie', '31000', 43.6047, 1.4442, 'Europe/Paris', 19, 1, GETDATE(), GETDATE()),
(20, 'Nice', 4, 'Provence-Alpes-Côte d''Azur', '06000', 43.7102, 7.2620, 'Europe/Paris', 20, 1, GETDATE(), GETDATE()),

-- Italy cities
(21, 'Rome', 5, 'Lazio', '00118', 41.9028, 12.4964, 'Europe/Rome', 21, 1, GETDATE(), GETDATE()),
(22, 'Milan', 5, 'Lombardy', '20121', 45.4642, 9.1900, 'Europe/Rome', 22, 1, GETDATE(), GETDATE()),
(23, 'Naples', 5, 'Campania', '80121', 40.8518, 14.2681, 'Europe/Rome', 23, 1, GETDATE(), GETDATE()),
(24, 'Turin', 5, 'Piedmont', '10121', 45.0703, 7.6869, 'Europe/Rome', 24, 1, GETDATE(), GETDATE()),
(25, 'Florence', 5, 'Tuscany', '50121', 43.7696, 11.2558, 'Europe/Rome', 25, 1, GETDATE(), GETDATE()),

-- Spain cities
(26, 'Madrid', 6, 'Community of Madrid', '28001', 40.4168, -3.7038, 'Europe/Madrid', 26, 1, GETDATE(), GETDATE()),
(27, 'Barcelona', 6, 'Catalonia', '08001', 41.3851, 2.1734, 'Europe/Madrid', 27, 1, GETDATE(), GETDATE()),
(28, 'Valencia', 6, 'Valencian Community', '46001', 39.4699, -0.3763, 'Europe/Madrid', 28, 1, GETDATE(), GETDATE()),
(29, 'Seville', 6, 'Andalusia', '41001', 37.3886, -5.9823, 'Europe/Madrid', 29, 1, GETDATE(), GETDATE()),
(30, 'Bilbao', 6, 'Basque Country', '48001', 43.2627, -2.9253, 'Europe/Madrid', 30, 1, GETDATE(), GETDATE()),

-- Netherlands cities
(31, 'Amsterdam', 7, 'North Holland', '1011', 52.3676, 4.9041, 'Europe/Amsterdam', 31, 1, GETDATE(), GETDATE()),
(32, 'Rotterdam', 7, 'South Holland', '3011', 51.9244, 4.4777, 'Europe/Amsterdam', 32, 1, GETDATE(), GETDATE()),
(33, 'The Hague', 7, 'South Holland', '2511', 52.0705, 4.3007, 'Europe/Amsterdam', 33, 1, GETDATE(), GETDATE()),
(34, 'Utrecht', 7, 'Utrecht', '3511', 52.0907, 5.1214, 'Europe/Amsterdam', 34, 1, GETDATE(), GETDATE()),
(35, 'Eindhoven', 7, 'North Brabant', '5611', 51.4416, 5.4697, 'Europe/Amsterdam', 35, 1, GETDATE(), GETDATE()),

-- Norway cities
(36, 'Oslo', 11, 'Oslo', '0001', 59.9139, 10.7522, 'Europe/Oslo', 36, 1, GETDATE(), GETDATE()),
(37, 'Bergen', 11, 'Vestland', '5001', 60.3913, 5.3221, 'Europe/Oslo', 37, 1, GETDATE(), GETDATE()),
(38, 'Trondheim', 11, 'Trøndelag', '7001', 63.4305, 10.3951, 'Europe/Oslo', 38, 1, GETDATE(), GETDATE()),
(39, 'Stavanger', 11, 'Rogaland', '4001', 58.9700, 5.7331, 'Europe/Oslo', 39, 1, GETDATE(), GETDATE()),
(40, 'Drammen', 11, 'Viken', '3001', 59.7440, 10.2045, 'Europe/Oslo', 40, 1, GETDATE(), GETDATE()),

-- Asia Pacific cities
(41, 'Tokyo', 15, 'Tokyo', '100-0001', 35.6762, 139.6503, 'Asia/Tokyo', 41, 1, GETDATE(), GETDATE()),
(42, 'Shanghai', 16, 'Shanghai', '200001', 31.2304, 121.4737, 'Asia/Shanghai', 42, 1, GETDATE(), GETDATE()),
(43, 'Seoul', 17, 'Seoul', '04518', 37.5665, 126.9780, 'Asia/Seoul', 43, 1, GETDATE(), GETDATE()),
(44, 'Singapore', 18, NULL, '018956', 1.3521, 103.8198, 'Asia/Singapore', 44, 1, GETDATE(), GETDATE()),
(45, 'Sydney', 19, 'New South Wales', '2000', -33.8688, 151.2093, 'Australia/Sydney', 45, 1, GETDATE(), GETDATE()),

-- Canada cities
(46, 'Toronto', 21, 'Ontario', 'M5H 2N2', 43.6532, -79.3832, 'America/Toronto', 46, 1, GETDATE(), GETDATE()),
(47, 'Vancouver', 21, 'British Columbia', 'V6B 1A1', 49.2827, -123.1207, 'America/Vancouver', 47, 1, GETDATE(), GETDATE()),
(48, 'Montreal', 21, 'Quebec', 'H2Y 1A6', 45.5017, -73.5673, 'America/Toronto', 48, 1, GETDATE(), GETDATE()),
(49, 'Calgary', 21, 'Alberta', 'T2G 0P6', 51.0447, -114.0719, 'America/Edmonton', 49, 1, GETDATE(), GETDATE()),
(50, 'Ottawa', 21, 'Ontario', 'K1A 0A6', 45.4215, -75.6972, 'America/Toronto', 50, 1, GETDATE(), GETDATE());

SET IDENTITY_INSERT [dbo].[Cities] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalCities FROM [dbo].[Cities];
SELECT c.CityName, co.CountryName, c.StateProvince 
FROM [dbo].[Cities] c 
INNER JOIN [dbo].[Countries] co ON c.CountryId = co.CountryId 
ORDER BY co.CountryName, c.CityName;
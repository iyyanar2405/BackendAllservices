-- Seed data
INSERT INTO Categories (Name, Description, IsActive) VALUES
('Electronics', 'Electronic devices and accessories', 1),
('Books', 'Books and publications', 1),
('Clothing', 'Apparel and fashion items', 1);

INSERT INTO Products (Name, Description, Price, StockQuantity, CategoryId, IsActive) VALUES
('Laptop', 'High-performance laptop', 999.99, 50, 1, 1),
('C# Programming', 'Learn C# programming', 49.99, 100, 2, 1),
('T-Shirt', 'Cotton T-Shirt', 19.99, 200, 3, 1);

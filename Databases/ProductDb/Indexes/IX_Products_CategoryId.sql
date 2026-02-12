-- Create indexes for better query performance
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);

CREATE INDEX IX_Products_Price ON Products(Price);

CREATE INDEX IX_Products_IsActive ON Products(IsActive);

CREATE INDEX IX_Products_CreatedAt ON Products(CreatedAt DESC);

-- Composite indexes for common queries
CREATE INDEX IX_Products_CategoryId_IsActive ON Products(CategoryId, IsActive) INCLUDE (Name, Price);

CREATE INDEX IX_Products_Price_IsActive ON Products(Price, IsActive);

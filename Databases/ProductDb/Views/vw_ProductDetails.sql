-- View: Product Details with enhanced information
CREATE VIEW vw_ProductDetails
AS
SELECT 
    p.Id,
    p.Name,
    p.Description,
    p.Price,
    p.StockQuantity,
    dbo.GetProductStock(p.Id) AS StockStatus,
    c.Id AS CategoryId,
    c.Name AS CategoryName,
    p.CreatedAt,
    p.UpdatedAt,
    p.IsActive,
    (SELECT COUNT(*) FROM ProductAuditLogs WHERE ProductId = p.Id) AS AuditLogCount,
    (SELECT MAX(ChangedAt) FROM ProductAuditLogs WHERE ProductId = p.Id) AS LastModifiedFromAudit
FROM Products p
LEFT JOIN Categories c ON p.CategoryId = c.Id;

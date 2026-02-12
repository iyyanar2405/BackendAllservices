-- Stored Procedure: Get product by ID with category details
CREATE PROCEDURE sp_GetProductById
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        p.Id,
        p.Name,
        p.Description,
        p.Price,
        p.StockQuantity,
        p.CategoryId,
        c.Name AS CategoryName,
        c.Description AS CategoryDescription,
        p.CreatedAt,
        p.UpdatedAt,
        p.IsActive
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.Id
    WHERE p.Id = @ProductId AND p.IsActive = 1;
END;

-- Function: Get product stock status
CREATE FUNCTION dbo.GetProductStock
(
    @ProductId INT
)
RETURNS NVARCHAR(20)
AS
BEGIN
    DECLARE @StockQuantity INT;
    SELECT @StockQuantity = StockQuantity FROM Products WHERE Id = @ProductId;
    
    RETURN CASE
        WHEN @StockQuantity IS NULL THEN 'Not Found'
        WHEN @StockQuantity <= 0 THEN 'Out of Stock'
        WHEN @StockQuantity <= 10 THEN 'Low Stock'
        WHEN @StockQuantity <= 50 THEN 'Medium Stock'
        ELSE 'In Stock'
    END;
END;

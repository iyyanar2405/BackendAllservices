-- Stored Procedure: Get products with statistics
CREATE PROCEDURE sp_GetProductsWithStats
    @CategoryId INT = NULL,
    @MinPrice DECIMAL(18,2) = NULL,
    @MaxPrice DECIMAL(18,2) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        p.Id,
        p.Name,
        p.Description,
        p.Price,
        p.StockQuantity,
        c.Name AS CategoryName,
        p.CreatedAt,
        p.UpdatedAt,
        p.IsActive,
        COUNT(*) OVER () AS TotalCount,
        AVG(p.Price) OVER (PARTITION BY p.CategoryId) AS AvgPriceByCategory,
        SUM(p.StockQuantity) OVER (PARTITION BY p.CategoryId) AS TotalStockByCategory
    FROM Products p
    LEFT JOIN Categories c ON p.CategoryId = c.Id
    WHERE 
        (p.IsActive = 1)
        AND (@CategoryId IS NULL OR p.CategoryId = @CategoryId)
        AND (@MinPrice IS NULL OR p.Price >= @MinPrice)
        AND (@MaxPrice IS NULL OR p.Price <= @MaxPrice)
    ORDER BY p.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;

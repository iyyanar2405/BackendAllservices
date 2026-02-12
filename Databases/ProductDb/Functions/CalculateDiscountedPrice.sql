-- Function: Calculate discounted price
CREATE FUNCTION dbo.CalculateDiscountedPrice
(
    @BasePrice DECIMAL(18,2),
    @DiscountPercentage DECIMAL(5,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    IF @DiscountPercentage < 0 OR @DiscountPercentage > 100
        RETURN @BasePrice;
    
    RETURN @BasePrice * (1 - (@DiscountPercentage / 100));
END;

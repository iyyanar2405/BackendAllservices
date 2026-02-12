CREATE PROCEDURE [dbo].[Sp_GetOverviewFinancialStatus]
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        -- Calculate financial status statistics
        WITH FinancialStatusData AS SELECT CASE 
                    WHEN f.PaymentStatus = 1 THEN 'Not Paid'
                    WHEN f.PaymentStatus = 2 THEN 'Overdue'
                    WHEN f.PaymentStatus = 3 THEN 'Paid'
                    WHEN f.PaymentStatus = 4 THEN 'Partially Paid'
                    ELSE 'Unknown'
                END as FinancialStatus,
                COUNT(*) as FinancialCount
            FROM Financials f
            WHERE f.PaymentStatus IS NOT NULL
            GROUP BY f.PaymentStatus
        ),
        TotalCount AS SELECT CAST(SUM(FinancialCount) AS FLOAT) as Total
            FROM FinancialStatusData
        )
        SELECT 
            1 as isSuccess,
            'Successfully retrieved the data.' as message,
            SELECT COALESCE(fsd.FinancialStatus, status_template.StatusName) as financialStatus,
                    COALESCE(fsd.FinancialCount, 0) as financialCount,
                    CASE 
                        WHEN tc.Total > 0 AND fsd.FinancialCount IS NOT NULL 
                        THEN ROUND((CAST(fsd.FinancialCount AS FLOAT) / tc.Total) * 100, 1)
                        ELSE 0.0
                    END as financialpercentage FROM SELECT 'Not Paid' as StatusName, 1 as SortOrder
                    UNION ALL
                    SELECT 'Overdue' as StatusName, 2 as SortOrder
                    UNION ALL
                    SELECT 'Paid' as StatusName, 3 as SortOrder
                    UNION ALL
                    SELECT 'Partially Paid' as StatusName, 4 as SortOrder
                ) status_template
                CROSS JOIN TotalCount tc
                LEFT JOIN FinancialStatusData fsd ON fsd.FinancialStatus = status_template.StatusName
                ORDER BY status_template.SortOrder
            END TRY
    BEGIN CATCH
        -- Handle any errors
        SELECT 
            0 as isSuccess,
            ERROR_MESSAGE() as message,
            'DATABASE_ERROR' as errorCode,
            JSON_QUERY('[]'END CATCH
END



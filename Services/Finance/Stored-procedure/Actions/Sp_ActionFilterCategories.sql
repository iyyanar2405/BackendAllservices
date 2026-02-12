CREATE PROCEDURE Sp_ActionFilterCategories
    @companies NVARCHAR(MAX) = NULL,  -- JSON array of company IDs
    @services NVARCHAR(MAX) = NULL,   -- JSON array of service IDs  
    @sites NVARCHAR(MAX) = NULL       -- JSON array of site IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Return action categories
    -- Categories: 2=Certificates, 3=Findings, 4=Schedule
    SELECT 
        id,
        label
    FROM (
        VALUES 
            (2, 'Certificates'),
            (3, 'Findings'), 
            (4, 'Schedule')
    ) AS Categories(id, label)
    ORDER BY id;
END

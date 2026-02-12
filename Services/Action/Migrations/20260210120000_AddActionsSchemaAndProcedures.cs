using ActionService.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace ActionService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260210120000_AddActionsSchemaAndProcedures")]
    public partial class AddActionsSchemaAndProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    action = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    dueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    highPriority = table.Column<bool>(type: "bit", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    language = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    service = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    site = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    entityType = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    entityId = table.Column<int>(type: "int", nullable: true),
                    subject = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    snowLink = table.Column<string>(type: "nvarchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.id);
                });

            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_ActionFilterCategories
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
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_ActionFilterCompanies
    @categories NVARCHAR(MAX) = NULL,  -- JSON array of category IDs
    @services NVARCHAR(MAX) = NULL,    -- JSON array of service IDs
    @sites NVARCHAR(MAX) = NULL        -- JSON array of site IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @categories IS NOT NULL AND @categories != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@categories);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceId INT);
    IF @services IS NOT NULL AND @services != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@services);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilter TABLE (siteId INT);
    IF @sites IS NOT NULL AND @sites != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@sites);
    END
    
    -- Companies data (this should ideally come from a Companies table)
    -- For now, using a CTE with sample data matching the response
    WITH CompaniesData AS (
        SELECT CompanyId AS id, CompanyName AS label
        FROM Companies
    ),
    
    -- Get filtered companies based on actions that match the criteria
    FilteredCompanies AS (
        SELECT DISTINCT au.companyId
        FROM Audits au
        INNER JOIN Actions a ON a.entityId = au.auditId
        LEFT JOIN AuditServices aser ON aser.AuditId = au.AuditId
        WHERE 1=1
            -- Filter by categories if provided (based on action entityType)
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType = 'Certificate')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType = 'Finding')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType = 'Schedule')
                )
            )
            -- Filter by services if provided
            AND (
                (SELECT COUNT(*) FROM @serviceFilter) = 0
                OR aser.ServiceId IN (SELECT serviceId FROM @serviceFilter)
            )
            -- Filter by sites if provided
            AND (
                (SELECT COUNT(*) FROM @siteFilter) = 0
                OR EXISTS (SELECT 1 FROM AuditSites asit 
                    WHERE asit.AuditId = au.AuditId 
                    AND asit.SiteId IN (SELECT siteId FROM @siteFilter))
            )
    )
    
    -- Return companies that have actions matching the filter criteria
    SELECT 
        cd.id,
        cd.label
    FROM CompaniesData cd
    WHERE EXISTS (SELECT 1 FROM FilteredCompanies fc WHERE fc.companyId = cd.id)
    OR (
        (SELECT COUNT(*) FROM @categoryFilter) = 0 
        AND (SELECT COUNT(*) FROM @serviceFilter) = 0
        AND (SELECT COUNT(*) FROM @siteFilter) = 0
        AND EXISTS (SELECT 1 FROM Audits WHERE companyId = cd.id)
    )
    ORDER BY cd.label;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_ActionFilterServices
    @companies NVARCHAR(MAX) = NULL,   -- JSON array of company IDs
    @categories NVARCHAR(MAX) = NULL,  -- JSON array of category IDs
    @sites NVARCHAR(MAX) = NULL        -- JSON array of site IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create temp table for company filter if provided
    DECLARE @companyFilter TABLE (companyId INT);
    IF @companies IS NOT NULL AND @companies != '[]'
    BEGIN
        INSERT INTO @companyFilter (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@companies);
    END
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @categories IS NOT NULL AND @categories != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@categories);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilter TABLE (siteId INT);
    IF @sites IS NOT NULL AND @sites != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@sites);
    END
    
    WITH ServicesData AS (
        SELECT ServiceId AS id, ServiceName AS label
        FROM Services
    ),
    
    -- Get filtered services based on actions that match the criteria
    FilteredServices AS (
        SELECT DISTINCT aser.ServiceId
        FROM Audits au
        LEFT JOIN Actions a ON a.entityId = au.auditId
        LEFT JOIN AuditServices aser ON aser.AuditId = au.AuditId
        WHERE 1=1
            -- Filter by companies if provided
            AND (
                (SELECT COUNT(*) FROM @companyFilter) = 0
                OR au.companyId IN (SELECT companyId FROM @companyFilter)
            )
            -- Filter by categories if provided (based on action entityType)
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType = 'Certificate')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType = 'Finding')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType = 'Schedule')
                )
            )
            -- Filter by sites if provided
            AND (
                (SELECT COUNT(*) FROM @siteFilter) = 0
                OR EXISTS (SELECT 1 FROM AuditSites asit 
                    WHERE asit.AuditId = au.AuditId 
                    AND asit.SiteId IN (SELECT siteId FROM @siteFilter))
            )
    )
    
    -- Return services that have actions matching the filter criteria
    SELECT 
        sd.id,
        sd.label
    FROM ServicesData sd
    WHERE EXISTS (SELECT 1 FROM FilteredServices fs WHERE fs.ServiceId = sd.id)
    OR (
        (SELECT COUNT(*) FROM @companyFilter) = 0 
        AND (SELECT COUNT(*) FROM @categoryFilter) = 0
        AND (SELECT COUNT(*) FROM @siteFilter) = 0
        AND EXISTS (SELECT 1 FROM AuditServices aser WHERE aser.ServiceId = sd.id)
    )
    ORDER BY sd.label;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_ActionFilterSites
    @companies NVARCHAR(MAX) = NULL,   -- JSON array of company IDs
    @categories NVARCHAR(MAX) = NULL,  -- JSON array of category IDs
    @services NVARCHAR(MAX) = NULL     -- JSON array of service IDs
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Create temp table for company filter if provided
    DECLARE @companyFilter TABLE (companyId INT);
    IF @companies IS NOT NULL AND @companies != '[]'
    BEGIN
        INSERT INTO @companyFilter (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@companies);
    END
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @categories IS NOT NULL AND @categories != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@categories);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceId INT);
    IF @services IS NOT NULL AND @services != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@services);
    END
    
    ;WITH FilteredAudits AS (
        SELECT DISTINCT au.AuditId
        FROM Audits au
        LEFT JOIN Actions a ON a.entityId = au.AuditId
        LEFT JOIN AuditServices aser ON aser.AuditId = au.AuditId
        WHERE 1=1
            AND (
                (SELECT COUNT(*) FROM @companyFilter) = 0
                OR au.companyId IN (SELECT companyId FROM @companyFilter)
            )
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType = 'Certificate')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType = 'Finding')
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType = 'Schedule')
                )
            )
            AND (
                (SELECT COUNT(*) FROM @serviceFilter) = 0
                OR aser.ServiceId IN (SELECT serviceId FROM @serviceFilter)
            )
    )
    SELECT
        co.CountryId AS CountryId,
        co.CountryName AS CountryName,
        ci.CityId AS CityId,
        ci.CityName AS CityName,
        s.SiteId AS SiteId,
        s.SiteName AS SiteName
    FROM Sites s
    INNER JOIN Cities ci ON ci.CityId = s.CityId
    INNER JOIN Countries co ON co.CountryId = s.CountryId
    INNER JOIN AuditSites ast ON ast.SiteId = s.SiteId
    INNER JOIN FilteredAudits fa ON fa.AuditId = ast.AuditId
    WHERE
        (SELECT COUNT(*) FROM @companyFilter) = 0
        OR s.CompanyId IN (SELECT companyId FROM @companyFilter)
    ORDER BY co.CountryName, ci.CityName, s.SiteName;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_GetActions
    @category NVARCHAR(MAX) = NULL,        -- JSON array of category IDs
    @company NVARCHAR(MAX) = NULL,         -- JSON array of company IDs
    @service NVARCHAR(MAX) = NULL,         -- JSON array of service IDs
    @site NVARCHAR(MAX) = NULL,            -- JSON array of site IDs
    @isHighPriority BIT = 0,               -- Filter for high priority actions
    @pageNumber INT = 1,                   -- Page number (1-based)
    @pageSize INT = 10                     -- Number of items per page
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Validate pagination parameters
    IF @pageNumber < 1 SET @pageNumber = 1;
    IF @pageSize < 1 SET @pageSize = 10;
    IF @pageSize > 100 SET @pageSize = 100; -- Limit max page size
    
    -- Create temp table for category filter if provided
    DECLARE @categoryFilter TABLE (categoryId INT);
    IF @category IS NOT NULL AND @category != '[]'
    BEGIN
        INSERT INTO @categoryFilter (categoryId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@category);
    END
    
    -- Create temp table for company filter if provided
    DECLARE @companyFilter TABLE (companyId INT);
    IF @company IS NOT NULL AND @company != '[]'
    BEGIN
        INSERT INTO @companyFilter (companyId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@company);
    END
    
    -- Create temp table for service filter if provided
    DECLARE @serviceFilter TABLE (serviceId INT);
    IF @service IS NOT NULL AND @service != '[]'
    BEGIN
        INSERT INTO @serviceFilter (serviceId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@service);
    END
    
    -- Create temp table for site filter if provided
    DECLARE @siteFilter TABLE (siteId INT);
    IF @site IS NOT NULL AND @site != '[]'
    BEGIN
        INSERT INTO @siteFilter (siteId)
        SELECT CAST(value AS INT)
        FROM OPENJSON(@site);
    END
    
    -- Calculate pagination
    DECLARE @offset INT = (@pageNumber - 1) * @pageSize;
    DECLARE @totalItems INT;
    
    -- Main query with filtering
    WITH FilteredActions AS (
        SELECT DISTINCT a.*
        FROM Actions a
        LEFT JOIN Audits au ON a.entityId = au.auditId
        LEFT JOIN AuditServices aser ON aser.auditId = au.auditId
        LEFT JOIN Services s ON s.ServiceId = aser.ServiceId
        LEFT JOIN AuditSites asit ON asit.auditId = au.auditId
        WHERE 1=1
            -- Filter by categories if provided (based on entityType)
            AND (
                (SELECT COUNT(*) FROM @categoryFilter) = 0
                OR (
                    EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 2 AND a.entityType IN ('Certificate', 'certificates'))
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 3 AND a.entityType IN ('Finding', 'findings'))
                    OR EXISTS (SELECT 1 FROM @categoryFilter WHERE categoryId = 4 AND a.entityType IN ('Schedule', 'schedule'))
                )
            )
            -- Filter by companies if provided
            AND (
                (SELECT COUNT(*) FROM @companyFilter) = 0
                OR au.companyId IN (SELECT companyId FROM @companyFilter)
            )
            -- Filter by services if provided
            AND (
                (SELECT COUNT(*) FROM @serviceFilter) = 0
                OR s.ServiceId IN (SELECT serviceId FROM @serviceFilter)
            )
            -- Filter by sites if provided
            AND (
                (SELECT COUNT(*) FROM @siteFilter) = 0
                OR asit.siteId IN (SELECT siteId FROM @siteFilter)
            )
            -- Filter by high priority if specified
            AND (
                @isHighPriority = 0 
                OR (@isHighPriority = 1 AND a.highPriority = 1)
            )
    )
    
    -- Get total count for pagination
    SELECT @totalItems = COUNT(*) FROM FilteredActions;
    
    -- Calculate total pages
    DECLARE @totalPages INT = CEILING(CAST(@totalItems AS FLOAT) / @pageSize);
    
    -- Return paginated results as table
    SELECT 
        a.action,
        a.dueDate,
        a.highPriority,
        a.id,
        a.message,
        a.language,
        a.service,
        a.site,
        a.entityType,
        CAST(a.entityId AS NVARCHAR(50)) AS entityId,
        a.subject,
        a.snowLink,
        @pageNumber AS currentPage,
        @totalItems AS totalItems,
        @totalPages AS totalPages
    FROM FilteredActions a
    ORDER BY 
        CASE WHEN a.dueDate IS NULL THEN 1 ELSE 0 END,  -- NULL dates last
        a.dueDate ASC,                                  -- Earliest dates first
        a.id DESC                                       -- Most recent IDs first for ties
    OFFSET @offset ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END");

            migrationBuilder.Sql(@"CREATE PROCEDURE Sp_InsertAction
    @id INT,
    @action NVARCHAR(255),
    @dueDate DATETIME,
    @highPriority BIT,
    @message NVARCHAR(MAX),
    @language NVARCHAR(50),
    @service NVARCHAR(100),
    @site NVARCHAR(100),
    @entityType NVARCHAR(100),
    @entityId INT,
    @subject NVARCHAR(255),
    @snowLink NVARCHAR(255)
AS
BEGIN
    INSERT INTO Actions (
        id, action, dueDate, highPriority, message, language, service, site, entityType, entityId, subject, snowLink
    )
    VALUES (
        @id, @action, @dueDate, @highPriority, @message, @language, @service, @site, @entityType, @entityId, @subject, @snowLink
    );
END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"IF OBJECT_ID('Sp_InsertAction', 'P') IS NOT NULL DROP PROCEDURE Sp_InsertAction");
            migrationBuilder.Sql(@"IF OBJECT_ID('Sp_GetActions', 'P') IS NOT NULL DROP PROCEDURE Sp_GetActions");
            migrationBuilder.Sql(@"IF OBJECT_ID('Sp_ActionFilterSites', 'P') IS NOT NULL DROP PROCEDURE Sp_ActionFilterSites");
            migrationBuilder.Sql(@"IF OBJECT_ID('Sp_ActionFilterServices', 'P') IS NOT NULL DROP PROCEDURE Sp_ActionFilterServices");
            migrationBuilder.Sql(@"IF OBJECT_ID('Sp_ActionFilterCompanies', 'P') IS NOT NULL DROP PROCEDURE Sp_ActionFilterCompanies");
            migrationBuilder.Sql(@"IF OBJECT_ID('Sp_ActionFilterCategories', 'P') IS NOT NULL DROP PROCEDURE Sp_ActionFilterCategories");

            migrationBuilder.DropTable(
                name: "Actions");
        }
    }
}

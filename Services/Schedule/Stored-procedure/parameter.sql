-- Get all categories
EXEC Sp_ActionFilterCategories

-- Filter by companies
EXEC Sp_ActionFilterCategories @companies = '[1,2,3]'

-- Filter by multiple parameters
EXEC Sp_ActionFilterCategories @companies = '[1]', 
@services = '["Service1"]', @sites = '[100]'


-- Get all companies (no filters)
EXEC Sp_ActionFilterCompanies

-- Filter by categories
EXEC Sp_ActionFilterCompanies @categories = '[2,3]'  -- Certificates and Findings

-- Filter by services
EXEC Sp_ActionFilterCompanies @services = '["Service1","Service2"]'

-- Filter by sites
EXEC Sp_ActionFilterCompanies @sites = '[100,101,102]'

-- Combined filters
EXEC Sp_ActionFilterCompanies @categories = '[3]', @services = '["Audit"]', @sites = '[100]'

-- Get all services (no filters)
EXEC Sp_ActionFilterServices

-- Filter by companies
EXEC Sp_ActionFilterServices @companies = '[341,353,366]'

-- Filter by categories
EXEC Sp_ActionFilterServices @categories = '[2,3]'  -- Certificates and Findings

-- Filter by sites
EXEC Sp_ActionFilterServices @sites = '[100,101,102]'

-- Combined filters
EXEC Sp_ActionFilterServices @companies = '[341]', @categories = '[2]', @sites = '[100]'


-- Get all sites (no filters)
EXEC Sp_ActionFilterSites

-- Filter by companies
EXEC Sp_ActionFilterSites @companies = '[341,353,366]'

-- Filter by categories
EXEC Sp_ActionFilterSites @categories = '[2,3]'  -- Certificates and Findings

-- Filter by services
EXEC Sp_ActionFilterSites @services = '["ISO 9001:2015","ISO 14001:2015"]'

-- Combined filters
EXEC Sp_ActionFilterSites @companies = '[341]', @categories = '[2]', @services = '["ISO 9001:2015"]'

-- Get first page of all actions (10 items)
EXEC Sp_GetActions

-- Get actions with pagination
EXEC Sp_GetActions @pageNumber = 2, @pageSize = 20

-- Filter by high priority actions only
EXEC Sp_GetActions @isHighPriority = 1

-- Filter by categories (Certificates and Findings)
EXEC Sp_GetActions @category = '[2,3]'

-- Filter by companies
EXEC Sp_GetActions @company = '[341,353,366]'

-- Filter by services
EXEC Sp_GetActions @service = '["ISO 9001:2015","SQF Food Safety Code: Food Manufacturing, Edition 9"]'

-- Combined filters with pagination
EXEC Sp_GetActions 
    @category = '[4]',              -- Schedule only
    @company = '[341]',             -- Specific company
    @isHighPriority = 0,            -- All priorities
    @pageNumber = 1,
    @pageSize = 15


    -- Get audit days for 2025 with company filter
EXEC Sp_GetAuditDaysByMonthAndService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companyFilter = '[386]'

-- Filter by specific services
EXEC Sp_GetAuditDaysByMonthAndService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companyFilter = '[386]',
    @serviceFilter = '["FSSC 22000 V6.0 - ISO/TS 22002-1:2009 (Food)", "ISO 14001:2015"]'

-- Filter by sites as well
EXEC Sp_GetAuditDaysByMonthAndService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companyFilter = '[386]',
    @serviceFilter = '[]',
    @siteFilter = '[100, 101]'

-- Get all audit days for a quarter
EXEC Sp_GetAuditDaysByMonthAndService 
    @startDate = '2025-07-01',
    @endDate = '2025-09-30'


-- Get audit days by service for 2025 (no filters)
EXEC Sp_GetAuditDaysByService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31'

-- Filter by specific companies
EXEC Sp_GetAuditDaysByService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companies = '[386, 341, 353]'

-- Filter by specific services
EXEC Sp_GetAuditDaysByService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companies = '[]',
    @services = '["ISO 14001:2015", "ISO 45001:2018", "ISO 9001:2015"]'

-- Filter by sites as well
EXEC Sp_GetAuditDaysByService 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companies = '[386]',
    @services = '[]',
    @sites = '[100, 101, 102]'

-- Filter by date range only (quarterly report)
EXEC Sp_GetAuditDaysByService 
    @startDate = '2025-07-01',
    @endDate = '2025-09-30'


-- Get audit days grid for 2025 (all data)
EXEC Sp_GetAuditDaysGrid 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31'

-- Filter by specific companies
EXEC Sp_GetAuditDaysGrid 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companies = '[386, 341, 353]'

-- Filter by specific services
EXEC Sp_GetAuditDaysGrid 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companies = '[]',
    @services = '["ISO 14001:2015", "ISO 9001:2015"]'

-- Filter by specific sites
EXEC Sp_GetAuditDaysGrid 
    @startDate = '2025-01-01',
    @endDate = '2025-12-31',
    @companies = '[]',
    @services = '[]',
    @sites = '[186183, 184940]'

-- Quarterly report
EXEC Sp_GetAuditDaysGrid 
    @startDate = '2025-07-01',
    @endDate = '2025-09-30'

    EXEC Sp_GetAuditDetails @auditId = 1392092


EXEC Sp_GetAuditFindingList @auditId = 1392092

EXEC Sp_GetAuditSites @auditId = 1392092

EXEC Sp_GetSubAudits @auditId = 1392092

EXEC Sp_GetCertificateDetails @certificateId = 678603

EXEC Sp_GetCertificateList

EXEC Sp_GetCertificateSites @certificateId = 678603

EXEC Sp_GetPreferences 
    @objectType = 'Grid',
    @objectName = 'Certificates', 
    @pageName = 'CertificateList'


    -- Get all overview data (no filters)
EXEC Sp_GetOverviewCardData

-- Filter by specific companies
EXEC Sp_GetOverviewCardData @filterCompanies = '[478, 444]'

-- Filter by sites and services
EXEC Sp_GetOverviewCardData 
    @filterSites = '[172208, 172212]',
    @filterServices = '[1184, 2055]'

-- Multiple filters
EXEC Sp_GetOverviewCardData 
    @filterCompanies = '[478]',
    @filterSites = '[172208]',
    @filterServices = '[1184]'


    EXEC Sp_GetOverviewFinancialStatus

    EXEC Sp_GetPreferences 
    @objectType = 'Grid',
    @objectName = 'Findings', 
    @pageName = 'FindingList'


    EXEC Sp_GetServiceList


    -- Validate user by Veracity ID
EXEC Sp_GetUserValidation @veracityId = 'ff145f9e-96d7-41ca-ad28-25cd3d2acb97'

-- Validate user by internal user ID
EXEC Sp_GetUserValidation @userId = 'USER123'

-- Validate current user (no parameters - uses session context)
EXEC Sp_GetUserValidation


-- Get upcoming audits for next 3 months (default)
EXEC Sp_GetWidgetForUpcomingAudit

-- Get upcoming audits for specific date range
EXEC Sp_GetWidgetForUpcomingAudit 
    @startDate = '2025-09-01',
    @endDate = '2025-09-30'

-- Get upcoming audits for next month only
EXEC Sp_GetWidgetForUpcomingAudit 
    @startDate = '2025-09-19',
    @endDate = '2025-10-19'


EXEC Sp_GetMasterSiteList

EXEC Sp_GetOverviewCompanyServiceSiteFilter

-- Get profile by Veracity ID
EXEC Sp_GetUserProfile @veracityId = 'ff145f9e-96d7-41ca-ad28-25cd3d2acb97'

-- Get profile by internal user ID
EXEC Sp_GetUserProfile @userId = 'USER123'

-- Get current user profile (no parameters - uses session context)
EXEC Sp_GetUserProfile


-- Get training status for all public trainings
EXEC Sp_GetWidgetForTrainingStatus

-- Get training status for specific user
EXEC Sp_GetWidgetForTrainingStatus @userId = 'USER123'

-- Get all contracts (first 50)
EXEC Sp_GetContractList

-- Get contracts for specific company
EXEC Sp_GetContractList @companyId = '10635467'

-- Get signed contracts only
EXEC Sp_GetContractList @contractType = 'Signed Contract'

-- Get contracts with pagination
EXEC Sp_GetContractList @pageSize = 25, @pageNumber = 2

-- Combined filtering
EXEC Sp_GetContractList 
    @companyId = '10635467',
    @contractType = 'Signed Contract',
    @pageSize = 10


-- Get all invoices (first 50)
EXEC Sp_GetInvoiceList

-- Get overdue invoices only
EXEC Sp_GetInvoiceList @status = 'Overdue'

-- Get invoices for specific company
EXEC Sp_GetInvoiceList @companyFilter = 'Britvic'

-- Get invoices with date range
EXEC Sp_GetInvoiceList 
    @startDate = '2018-01-01',
    @endDate = '2019-12-31'

-- Get paid invoices with pagination
EXEC Sp_GetInvoiceList 
    @status = 'Paid',
    @pageSize = 25,
    @pageNumber = 2

-- Combined filtering
EXEC Sp_GetInvoiceList 
    @status = 'Paid',
    @companyFilter = 'Kerry',
    @pageSize = 10


-- Update single invoice
EXEC Sp_UpdatePlannedPaymentDate 
    @invoiceNumbers = '["509010017719"]',
    @plannedPaymentDate = '2025-09-18T00:00:00.000Z'

-- Update multiple invoices
EXEC Sp_UpdatePlannedPaymentDate 
    @invoiceNumbers = '["509010017719", "462010001745", "462010000301"]',
    @plannedPaymentDate = '2025-10-01T00:00:00.000Z'

-- Update with different date
EXEC Sp_UpdatePlannedPaymentDate 
    @invoiceNumbers = '["509010017719"]',
    @plannedPaymentDate = '2025-12-15T00:00:00.000Z'
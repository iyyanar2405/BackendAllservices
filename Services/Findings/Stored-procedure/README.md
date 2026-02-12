# Stored Procedures Module Organization

This document describes the organized folder structure of stored procedures in the customer portal backend.

## Overview

All stored procedures have been organized into **12 logical modules** based on functionality and business domain. Each module contains related stored procedures for easier maintenance and development.

## Module Structure

### 1. **Actions/** (6 stored procedures)
Handles action-related operations including filtering and management.
- `Sp_ActionFilterCategories.sql` - Filter actions by categories
- `Sp_ActionFilterCompanies.sql` - Filter actions by companies
- `Sp_ActionFilterServices.sql` - Filter actions by services
- `Sp_ActionFilterSites.sql` - Filter actions by sites
- `Sp_GetActions.sql` - Retrieve action lists with pagination
- `Sp_InsertAction.sql` - Create new actions

### 2. **Audits/** (9 stored procedures)
Comprehensive audit management and scheduling operations.
- `Sp_GetAuditDaysByMonthAndService.sql` - Audit calendar view by month/service
- `Sp_GetAuditDaysByService.sql` - Audit days grouped by service
- `Sp_GetAuditDaysGrid.sql` - Audit calendar grid layout
- `Sp_GetAuditDetails.sql` - Detailed audit information
- `Sp_GetAuditFindingList.sql` - Findings associated with audits
- `Sp_GetAuditSchedules.sql` - Audit scheduling information
- `Sp_GetAuditSites.sql` - Sites involved in audits
- `Sp_GetScheduleCalendarInvite.sql` - Calendar integration for audit scheduling
- `Sp_GetSubAudits.sql` - Sub-audit management

### 3. **Certificates/** (3 stored procedures)
Certificate lifecycle management and tracking.
- `Sp_GetCertificateDetails.sql` - Detailed certificate information
- `Sp_GetCertificateList.sql` - Certificate listing and filtering
- `Sp_GetCertificateSites.sql` - Sites associated with certificates

### 4. **Contracts/** (1 stored procedure)
Contract management operations.
- `Sp_GetContractList.sql` - Contract listing and management

### 5. **Financial/** (4 stored procedures)
Financial operations including invoicing and payments.
- `Sp_DownloadInvoice.sql` - Invoice document generation
- `Sp_GetInvoiceList.sql` - Invoice listing and filtering
- `Sp_GetOverviewFinancialStatus.sql` - Financial status overview
- `Sp_UpdatePlannedPaymentDate.sql` - Payment schedule management

### 6. **Findings/** (9 stored procedures)
Non-conformity and finding management system.
- `Sp_GetFindingDetails.sql` - Detailed finding information
- `Sp_GetFindingList.sql` - Finding listing and filtering
- `Sp_GetFindingResponseHistory.sql` - Finding response tracking
- `Sp_GetFindingsByClauseList.sql` - Findings grouped by clauses
- `Sp_GetFindingsTrendsGraphData.sql` - Finding trend analytics
- `Sp_GetFindingTrendsList.sql` - Finding trend listings
- `Sp_GetLatestFindingResponse.sql` - Most recent finding responses
- `Sp_GetManageFindingDetails.sql` - Finding management operations
- `Sp_PostLatestFindingResponse.sql` - Submit finding responses

### 7. **Master/** (3 stored procedures)
Master data and reference information.
- `Sp_GetCountryList.sql` - Country reference data
- `Sp_GetMasterSiteList.sql` - Master site listings
- `Sp_GetServiceList.sql` - Service reference data

### 8. **Notifications/** (5 stored procedures)
Notification system and filtering operations.
- `Sp_NotificationFilterCategories.sql` - Filter notifications by categories
- `Sp_NotificationFilterCompanies.sql` - Filter notifications by companies
- `Sp_NotificationFilterServices.sql` - Filter notifications by services
- `Sp_NotificationFilterSites.sql` - Filter notifications by sites
- `Sp_Notifications.sql` - Notification management

### 9. **Overview/** (2 stored procedures)
Dashboard and overview operations.
- `Sp_GetOverviewCardData.sql` - Dashboard card information
- `Sp_GetOverviewCompanyServiceSiteFilter.sql` - Overview filtering options

### 10. **Settings/** (4 stored procedures)
System settings and configuration management.
- `Sp_GetPreferences.sql` - User preference management
- `Sp_GetSettingsAdminList.sql` - Admin settings management
- `Sp_GetSettingsCompanyDetails.sql` - Company configuration
- `Sp_GetSettingsMemberList.sql` - Member settings management

### 11. **Users/** (2 stored procedures)
User management and authentication.
- `Sp_GetUserProfile.sql` - User profile information
- `Sp_GetUserValidation.sql` - User authentication and validation

### 12. **Widgets/** (2 stored procedures)
Dashboard widget operations.
- `Sp_GetWidgetForTrainingStatus.sql` - Training status widget
- `Sp_GetWidgetForUpcomingAudit.sql` - Upcoming audit widget

## Benefits of This Organization

### 🎯 **Improved Maintainability**
- Related stored procedures are grouped together
- Easier to locate specific functionality
- Logical separation of concerns

### 🔧 **Development Efficiency**
- Module-based development approach
- Clear separation of business domains
- Easier team collaboration

### 📈 **Scalability**
- Easy to add new stored procedures to appropriate modules
- Clear structure for future enhancements
- Supports microservices architecture

### 🔍 **Better Code Management**
- Reduced complexity in main folder
- Easier code reviews and deployments
- Clear module boundaries

## Usage Guidelines

### For Developers:
1. **New Stored Procedures**: Add to the appropriate module folder
2. **Cross-Module Dependencies**: Document clearly and minimize
3. **Naming Conventions**: Follow existing `Sp_` prefix patterns

### For Database Administrators:
1. **Deployment**: Deploy by module for phased rollouts
2. **Backup**: Module-based backup strategies possible
3. **Monitoring**: Monitor performance by business domain

### For Project Managers:
1. **Feature Planning**: Align features with module structure
2. **Resource Allocation**: Assign teams to specific modules
3. **Progress Tracking**: Track development by module completion

## File Location
```
C:\POC\customer-portal-BE\Stored-procedure\
├── Actions/           (6 files)
├── Audits/            (9 files)
├── Certificates/      (3 files)
├── Contracts/         (1 file)
├── Financial/         (4 files)
├── Findings/          (9 files)
├── Master/            (3 files)
├── Notifications/     (5 files)
├── Overview/          (2 files)
├── Settings/          (4 files)
├── Users/             (2 files)
├── Widgets/           (2 files)
└── parameter.sql      (shared parameters)
```

**Total: 50 Stored Procedures organized into 12 modules**

---
*Generated on September 19, 2025*
*Customer Portal Backend - Database Layer*
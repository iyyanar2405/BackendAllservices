# GraphQL API Documentation - Hadvida Services

## Overview

All microservices are secured with JWT Bearer token authentication from the AuthProvider running at `http://localhost:5136`.

---

## 1. ACTION SERVICE (Port 5001)

### Endpoint
- **GraphQL**: `http://localhost:5001/graphql`
- **API Gateway**: `http://api-gateway/action/graphql`

### Queries

#### Get All Actions (with filters and pagination)
```graphql
query {
  actions(
    category: [1, 2]
    company: [101]
    service: [5]
    site: [10]
    isHighPriority: true
    pageNumber: 1
    pageSize: 20
  ) {
    success
    message
    data {
      items {
        id
        name
        description
        category
        priority
        status
        createdAt
      }
      totalCount
      pageNumber
      pageSize
    }
  }
}
```

#### Get Action Categories Filter
```graphql
query {
  actionCategoriesFilter(
    companies: [101]
    services: [5]
    sites: [10]
  ) {
    success
    message
    data {
      id
      name
      count
    }
  }
}
```

#### Get Action Companies Filter
```graphql
query {
  actionCompaniesFilter(
    categories: [1]
    services: [5]
    sites: [10]
  ) {
    success
    message
    data {
      id
      name
      count
    }
  }
}
```

#### Get Action Services Filter
```graphql
query {
  actionServicesFilter(
    companies: [101]
    categories: [1]
    sites: [10]
  ) {
    success
    message
    data {
      id
      name
      count
    }
  }
}
```

#### Get Action Sites Filter
```graphql
query {
  actionSitesFilter(
    companies: [101]
    categories: [1]
    services: [5]
  ) {
    success
    message
    data {
      siteId
      siteName
      companyId
      companyName
      actionCount
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "actions": {
      "success": true,
      "message": "Actions retrieved successfully",
      "data": {
        "items": [
          {
            "id": 1,
            "name": "Safety Audit",
            "description": "Complete facility safety check",
            "category": "Safety",
            "priority": "High",
            "status": "Active",
            "createdAt": "2026-02-12T10:30:00Z"
          }
        ],
        "totalCount": 50,
        "pageNumber": 1,
        "pageSize": 20
      }
    }
  }
}
```

---

## 2. AUDIT SERVICE (Port 5002)

### Endpoint
- **GraphQL**: `http://localhost:5002/graphql`
- **API Gateway**: `http://api-gateway/audit/graphql`

### Queries

#### View All Audits
```graphql
query {
  viewAudits {
    success
    message
    data {
      auditId
      auditName
      auditDate
      companyId
      companyName
      status
      auditDays
      findingCount
    }
  }
}
```

#### Get Audit Details
```graphql
query {
  auditDetails(auditId: 1) {
    success
    message
    data {
      auditId
      auditName
      auditDate
      startDate
      endDate
      companyId
      companyName
      auditDays
      scope
      status
      createdBy
    }
  }
}
```

#### View Findings for Audit
```graphql
query {
  viewFindings(auditId: 1) {
    success
    message
    data {
      findingId
      description
      severity
      status
      finding_type
      riskRating
    }
  }
}
```

#### View Sites for Audit
```graphql
query {
  viewSitesForAudit(auditId: 1) {
    success
    message
    data {
      siteId
      siteName
      auditDays
      findings
      status
    }
  }
}
```

#### View Sub-Audits
```graphql
query {
  viewSubAudits(auditId: 1) {
    success
    message
    data {
      subAuditId
      auditName
      status
      parentAuditId
    }
  }
}
```

#### Get Audit Days Per Site (Grid View)
```graphql
query {
  getAuditDaysPerSite(
    startDate: "2026-01-01"
    endDate: "2026-02-12"
    companies: [101]
    services: [5]
    sites: [10]
  ) {
    success
    message
    data {
      rows {
        siteId
        siteName
        auditDays
        pendingFindings
      }
    }
  }
}
```

#### Audit Days by Service (Pie Chart)
```graphql
query {
  auditDaysbyServicePieChart(filters: {
    startDate: "2026-01-01"
    endDate: "2026-02-12"
    companyIds: [101]
    serviceIds: [5]
  }) {
    success
    message
    data {
      series {
        serviceName
        auditDays
        percentage
      }
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "viewAudits": {
      "success": true,
      "message": "Audits retrieved successfully",
      "data": [
        {
          "auditId": 1,
          "auditName": "ISO 9001 Audit 2026",
          "auditDate": "2026-02-01",
          "companyId": 101,
          "companyName": "Hadvida Inc",
          "status": "Completed",
          "auditDays": 5,
          "findingCount": 12
        }
      ]
    }
  }
}
```

---

## 3. CERTIFICATE SERVICE (Port 5003)

### Endpoint
- **GraphQL**: `http://localhost:5003/graphql`
- **API Gateway**: `http://api-gateway/certificate/graphql`

### Queries

#### Get All Certificates
```graphql
query {
  certificates(
    pageNumber: 1
    pageSize: 20
  ) {
    success
    message
    data {
      certificates {
        certificateId
        certificateName
        certificateType
        issueDate
        expiryDate
        issuer
        status
        companyId
      }
      totalCount
    }
  }
}
```

#### Get Certificate by ID
```graphql
query {
  certificate(certificateId: 1) {
    success
    message
    data {
      certificateId
      certificateName
      certificateType
      issueDate
      expiryDate
      issuer
      description
      documentUrl
      status
    }
  }
}
```

#### Search Certificates
```graphql
query {
  searchCertificates(
    searchTerm: "ISO"
    certificateType: "International"
  ) {
    success
    message
    data {
      certificateId
      certificateName
      issuer
      expiryDate
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "certificates": {
      "success": true,
      "message": "Certificates retrieved successfully",
      "data": {
        "certificates": [
          {
            "certificateId": 1,
            "certificateName": "ISO 9001:2015",
            "certificateType": "Quality Management",
            "issueDate": "2023-01-15",
            "expiryDate": "2026-01-15",
            "issuer": "SGS",
            "status": "Active",
            "companyId": 101
          }
        ],
        "totalCount": 25
      }
    }
  }
}
```

---

## 4. CONTRACT SERVICE (Port 5004)

### Endpoint
- **GraphQL**: `http://localhost:5004/graphql`
- **API Gateway**: `http://api-gateway/contract/graphql`

### Queries

#### Get All Contracts
```graphql
query {
  contracts(
    status: "Active"
    pageNumber: 1
    pageSize: 20
  ) {
    success
    message
    data {
      contracts {
        contractId
        contractName
        contractType
        startDate
        endDate
        vendor
        value
        status
        companyId
      }
      totalCount
    }
  }
}
```

#### Get Contract Details
```graphql
query {
  contractDetails(contractId: 1) {
    success
    message
    data {
      contractId
      contractName
      contractType
      startDate
      endDate
      vendor
      value
      scope
      terms
      status
      documentUrl
    }
  }
}
```

#### Get Expiring Contracts
```graphql
query {
  expiringContracts(
    daysThreshold: 90
  ) {
    success
    message
    data {
      contractId
      contractName
      endDate
      daysRemaining
      vendor
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "contracts": {
      "success": true,
      "message": "Contracts retrieved successfully",
      "data": {
        "contracts": [
          {
            "contractId": 1,
            "contractName": "Maintenance Agreement",
            "contractType": "Service",
            "startDate": "2025-01-01",
            "endDate": "2026-12-31",
            "vendor": "ABC Services Ltd",
            "value": 50000,
            "status": "Active",
            "companyId": 101
          }
        ],
        "totalCount": 15
      }
    }
  }
}
```

---

## 5. FINANCE SERVICE (Port 5005)

### Endpoint
- **GraphQL**: `http://localhost:5005/graphql`
- **API Gateway**: `http://api-gateway/finance/graphql`

### Queries

#### Get Financial Summary
```graphql
query {
  financialSummary(
    startDate: "2026-01-01"
    endDate: "2026-02-12"
  ) {
    success
    message
    data {
      totalRevenue
      totalExpenses
      netIncome
      currency
      period
    }
  }
}
```

#### Get Invoices
```graphql
query {
  invoices(
    status: "Paid"
    pageNumber: 1
    pageSize: 20
  ) {
    success
    message
    data {
      invoices {
        invoiceId
        invoiceNumber
        date
        amount
        status
        vendor
        description
      }
      totalCount
      totalAmount
    }
  }
}
```

#### Get Budget Analysis
```graphql
query {
  budgetAnalysis(
    year: 2026
    department: "Operations"
  ) {
    success
    message
    data {
      department
      budgeted
      spent
      remaining
      percentage
    }
  }
}
```

### Mutations

#### Create Invoice
```graphql
mutation {
  createInvoice(input: {
    invoiceNumber: "INV-2026-001"
    vendor: "ABC Supplies"
    amount: 5000
    date: "2026-02-12"
    description: "Office supplies"
    status: "Pending"
  }) {
    success
    message
    data {
      invoiceId
      invoiceNumber
      status
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "financialSummary": {
      "success": true,
      "message": "Financial summary retrieved",
      "data": {
        "totalRevenue": 1500000,
        "totalExpenses": 950000,
        "netIncome": 550000,
        "currency": "USD",
        "period": "Jan-Feb 2026"
      }
    }
  }
}
```

---

## 6. FINDINGS SERVICE (Port 5000)

### Endpoint
- **GraphQL**: `http://localhost:5000/graphql`
- **API Gateway**: `http://api-gateway/findings/graphql`

### Queries

#### Get All Findings
```graphql
query {
  getFindings(
    companyId: 101
    status: "Open"
    category: "Safety"
  ) {
    id
    description
    severity
    status
    category
    companyId
    siteId
    createdAt
    updatedAt
    company {
      id
      name
    }
    site {
      id
      name
    }
  }
}
```

#### Get Finding by ID
```graphql
query {
  getFinding(id: 1) {
    id
    description
    severity
    status
    category
    riskRating
    remediationDeadline
    assignedTo
    company {
      id
      name
    }
    site {
      id
      name
    }
  }
}
```

#### Get Finding Statistics
```graphql
query {
  findingStatistics(
    companyId: 101
    startDate: "2026-01-01"
    endDate: "2026-02-12"
  ) {
    totalFindings
    openFindings
    closedFindings
    byCriticality {
      critical
      high
      medium
      low
    }
  }
}
```

### Mutations

#### Create Finding
```graphql
mutation {
  createFinding(input: {
    description: "Fire exit blocked"
    severity: "High"
    category: "Safety"
    companyId: 101
    siteId: 10
    riskRating: 8
  }) {
    success
    message
    data {
      id
      description
      status
    }
  }
}
```

#### Update Finding Status
```graphql
mutation {
  updateFindingStatus(
    id: 1
    status: "Resolved"
    comments: "Issue has been corrected"
  ) {
    success
    message
    data {
      id
      status
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "getFindings": [
      {
        "id": 1,
        "description": "Fire exit blocked by equipment",
        "severity": "High",
        "status": "Open",
        "category": "Safety",
        "companyId": 101,
        "siteId": 10,
        "createdAt": "2026-01-15T08:30:00Z",
        "company": {
          "id": 101,
          "name": "Hadvida Inc"
        },
        "site": {
          "id": 10,
          "name": "Main Facility"
        }
      }
    ]
  }
}
```

---

## 7. NOTIFICATION SERVICE (Port 5007)

### Endpoint
- **GraphQL**: `http://localhost:5007/graphql`
- **API Gateway**: `http://api-gateway/notification/graphql`

### Queries

#### Get All Notifications
```graphql
query {
  notifications(
    isRead: false
    pageNumber: 1
    pageSize: 20
  ) {
    success
    message
    data {
      notifications {
        notificationId
        title
        message
        type
        isRead
        createdAt
        recipientId
      }
      totalCount
    }
  }
}
```

#### Get Notification Details
```graphql
query {
  notificationDetails(notificationId: 1) {
    success
    message
    data {
      notificationId
      title
      message
      type
      content
      isRead
      createdAt
    }
  }
}
```

### Mutations

#### Send Notification
```graphql
mutation {
  sendNotification(input: {
    recipientId: "user-123"
    title: "Audit Completed"
    message: "The audit for Site ABC has been completed"
    type: "Audit"
    content: {
      auditId: 1
      siteId: 10
    }
  }) {
    success
    message
    data {
      notificationId
      status
    }
  }
}
```

#### Mark as Read
```graphql
mutation {
  markNotificationAsRead(notificationId: 1) {
    success
    message
    data {
      notificationId
      isRead
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "notifications": {
      "success": true,
      "message": "Notifications retrieved successfully",
      "data": {
        "notifications": [
          {
            "notificationId": 1,
            "title": "New Finding Assigned",
            "message": "A high priority finding has been assigned to you",
            "type": "Finding",
            "isRead": false,
            "createdAt": "2026-02-12T14:30:00Z"
          }
        ],
        "totalCount": 5
      }
    }
  }
}
```

---

## 8. SCHEDULE SERVICE (Port 5009)

### Endpoint
- **GraphQL**: `http://localhost:5009/graphql`
- **API Gateway**: `http://api-gateway/schedule/graphql`

### Queries

#### Get All Schedules
```graphql
query {
  schedules(
    status: "Active"
    pageNumber: 1
    pageSize: 20
  ) {
    success
    message
    data {
      schedules {
        scheduleId
        name
        description
        startDate
        endDate
        frequency
        status
        companyId
      }
      totalCount
    }
  }
}
```

#### Get Schedule Details
```graphql
query {
  scheduleDetails(scheduleId: 1) {
    success
    message
    data {
      scheduleId
      name
      description
      startDate
      endDate
      frequency
      recurrenceRule
      assignedTo
      status
    }
  }
}
```

#### Get Upcoming Schedules
```graphql
query {
  upcomingSchedules(
    daysAhead: 30
  ) {
    success
    message
    data {
      scheduleId
      name
      startDate
      endDate
      status
    }
  }
}
```

### Mutations

#### Create Schedule
```graphql
mutation {
  createSchedule(input: {
    name: "Monthly Audit"
    description: "Monthly facility audit"
    startDate: "2026-03-01"
    frequency: "Monthly"
    companyId: 101
  }) {
    success
    message
    data {
      scheduleId
      name
      status
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "schedules": {
      "success": true,
      "message": "Schedules retrieved successfully",
      "data": {
        "schedules": [
          {
            "scheduleId": 1,
            "name": "Weekly Safety Check",
            "description": "Routine safety inspection",
            "startDate": "2026-02-01",
            "endDate": null,
            "frequency": "Weekly",
            "status": "Active",
            "companyId": 101
          }
        ],
        "totalCount": 10
      }
    }
  }
}
```

---

## 9. SETTINGS SERVICE (Port 5010)

### Endpoint
- **GraphQL**: `http://localhost:5010/graphql`
- **API Gateway**: `http://api-gateway/settings/graphql`

### Queries

#### Get Application Settings
```graphql
query {
  applicationSettings(companyId: 101) {
    success
    message
    data {
      settingKey
      settingValue
      category
      description
      isActive
    }
  }
}
```

#### Get Setting by Key
```graphql
query {
  getSetting(key: "audit_frequency") {
    success
    message
    data {
      settingKey
      settingValue
      type
      category
    }
  }
}
```

#### Get Company Settings
```graphql
query {
  companySettings(companyId: 101) {
    success
    message
    data {
      companyId
      companyName
      settings {
        theme
        language
        timezone
        notificationPreferences
      }
    }
  }
}
```

### Mutations

#### Update Setting
```graphql
mutation {
  updateSetting(input: {
    settingKey: "audit_frequency"
    settingValue: "Monthly"
    companyId: 101
  }) {
    success
    message
    data {
      settingKey
      settingValue
    }
  }
}
```

#### Update Company Settings
```graphql
mutation {
  updateCompanySettings(input: {
    companyId: 101
    theme: "dark"
    language: "en"
    timezone: "UTC"
  }) {
    success
    message
    data {
      companyId
      settings {
        theme
        language
        timezone
      }
    }
  }
}
```

### Sample Response
```json
{
  "data": {
    "applicationSettings": {
      "success": true,
      "message": "Settings retrieved successfully",
      "data": [
        {
          "settingKey": "audit_frequency",
          "settingValue": "Monthly",
          "category": "Audit",
          "description": "Frequency of audits",
          "isActive": true
        }
      ]
    }
  }
}
```

---

## Authentication & Authorization

### Get JWT Token from AuthProvider
```powershell
$loginPayload = @{
  email = "user@hadvida.com"
  password = "YourPassword123"
} | ConvertTo-Json

$response = Invoke-RestMethod `
  -Uri "http://localhost:5136/api/auth/login" `
  -Method Post `
  -ContentType "application/json" `
  -Body $loginPayload

$token = $response.token
```

### Use JWT Token with GraphQL Queries
```powershell
$graphqlQuery = @{
  query = @"
    query {
      getFindings(companyId: 101, status: "Open") {
        id
        description
        severity
      }
    }
"@
} | ConvertTo-Json

$headers = @{
  Authorization = "Bearer $token"
  "Content-Type" = "application/json"
}

Invoke-RestMethod `
  -Uri "http://localhost:5000/graphql" `
  -Method Post `
  -Headers $headers `
  -Body $graphqlQuery
```

---

## Service Architecture Summary

| Service | Port | Database | Auth | REST | GraphQL |
|---------|------|----------|------|------|---------|
| Action | 5001 | SQL Server | JWT | ✅ | ✅ |
| Audit | 5002 | SQL Server | JWT | ✅ | ✅ |
| Certificate | 5003 | SQL Server | JWT | ✅ | ✅ |
| Contract | 5004 | SQL Server | JWT | ✅ | ✅ |
| Finance | 5005 | SQL Server | JWT | ✅ | ✅ |
| Findings | 5000 | SQL Server | JWT | ✅ | ✅ |
| Notification | 5007 | SQL Server | JWT | ✅ | ✅ |
| Schedule | 5009 | SQL Server | JWT | ✅ | ✅ |
| Settings | 5010 | SQL Server | JWT | ✅ | ✅ |
| AuthProvider | 5136 | SQL Server | - | ✅ | - |

---

## Error Handling

All services return standardized error responses:

```json
{
  "success": false,
  "message": "Error description",
  "data": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "message": "Field validation failed",
      "field": "fieldName"
    }
  ]
}
```

---

## Rate Limiting

- Global limit: 1000 requests per minute
- GraphQL limit: 200 requests per minute
- Custom limits per client based on Client-Id header

---

## CORS Configuration

All services accept requests from:
- `http://localhost:3000`
- `http://localhost:8080`
- `https://findings-ui.azurewebsites.net`
- Cross-origin credentials supported

---

*Last Updated: February 12, 2026*


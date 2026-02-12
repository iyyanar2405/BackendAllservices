INSERT INTO Audits (
    auditId,
    sites,
    services,
    companyId,
    status,
    startDate,
    endDate,
    leadAuditor,
    type
)
VALUES
(1392092, '171912', 'Service1', 12345, 'Completed', '2025-09-01', '2025-09-10', 'John Doe', 'Internal'),
(1392093, '171913,171914', 'Service2,Service3', 12346, 'InProgress', '2025-09-05', '2025-09-15', 'Jane Smith', 'External'),
(1392094, '171915', 'Service4', 12347, 'Scheduled', '2025-09-10', '2025-09-20', 'Mike Brown', 'Internal');
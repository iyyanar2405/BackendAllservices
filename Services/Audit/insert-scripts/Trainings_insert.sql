-- Insert sample data for Trainings table
-- This script creates various training programs and courses

SET IDENTITY_INSERT [dbo].[Trainings] ON;

INSERT INTO [dbo].[Trainings] 
([TrainingId], [TrainingName], [Description], [Category], [Duration], [DeliveryMethod], [Prerequisites], [LearningObjectives], [IsActive], [CreatedDate], [UpdatedDate], [CreatedBy], [UpdatedBy])
VALUES
-- ISO Standard Training Courses
(1, 'ISO 9001:2015 Quality Management Systems', 'Comprehensive training on ISO 9001:2015 requirements, implementation, and audit principles for quality management systems.', 'ISO Standards', 40, 'Blended', 'Basic quality management knowledge', 'Understand QMS requirements, implement QMS processes, conduct internal audits, prepare for certification', 1, '2024-01-15', '2024-01-15', 2, 2),

(2, 'ISO 14001:2015 Environmental Management Systems', 'In-depth training covering environmental management system requirements, environmental aspects identification, and compliance obligations.', 'ISO Standards', 32, 'Classroom', 'Environmental awareness, basic management systems knowledge', 'Implement EMS, identify environmental aspects, ensure regulatory compliance, conduct EMS audits', 1, '2024-01-15', '2024-01-15', 2, 2),

(3, 'ISO 45001:2018 Occupational Health and Safety', 'Training on occupational health and safety management systems, hazard identification, risk assessment, and incident investigation.', 'ISO Standards', 32, 'Online', 'Basic OH&S knowledge, workplace safety awareness', 'Implement OH&S management systems, conduct risk assessments, investigate incidents, ensure worker consultation', 1, '2024-01-15', '2024-01-15', 2, 2),

(4, 'ISO 27001:2013 Information Security Management', 'Comprehensive course on information security management systems, risk management, and cybersecurity controls.', 'ISO Standards', 40, 'Blended', 'IT fundamentals, basic security awareness', 'Implement ISMS, conduct risk assessments, manage security controls, ensure information confidentiality', 1, '2024-01-15', '2024-01-15', 2, 2),

(5, 'ISO 50001:2018 Energy Management Systems', 'Training on energy management systems, energy performance improvement, and energy efficiency optimization.', 'ISO Standards', 24, 'Online', 'Basic energy awareness, management systems knowledge', 'Implement EnMS, improve energy performance, conduct energy reviews, achieve energy objectives', 1, '2024-01-15', '2024-01-15', 2, 2),

-- Auditing Skills Training
(6, 'Lead Auditor Training - ISO 9001', 'Certified lead auditor course for ISO 9001:2015 quality management systems with practical audit exercises.', 'Auditing', 40, 'Classroom', 'QMS knowledge, previous audit experience preferred', 'Plan and conduct audits, lead audit teams, write effective audit reports, manage nonconformities', 1, '2024-01-20', '2024-01-20', 2, 2),

(7, 'Internal Auditor Development Program', 'Comprehensive program to develop internal auditing skills across multiple ISO standards and management systems.', 'Auditing', 24, 'Blended', 'Basic management systems knowledge', 'Conduct internal audits, interview techniques, evidence collection, audit reporting', 1, '2024-01-20', '2024-01-20', 2, 2),

(8, 'Audit Interview Techniques', 'Specialized training on effective interviewing skills, communication techniques, and evidence gathering during audits.', 'Auditing', 8, 'Classroom', 'Basic audit knowledge', 'Master interview techniques, build rapport with auditees, gather objective evidence, handle difficult situations', 1, '2024-01-20', '2024-01-20', 2, 2),

(9, 'Risk-Based Auditing Methodology', 'Advanced training on risk-based audit approaches, risk assessment techniques, and audit planning based on risk analysis.', 'Auditing', 16, 'Online', 'Audit experience, risk management basics', 'Apply risk-based thinking, prioritize audit activities, assess process risks, optimize audit effectiveness', 1, '2024-01-20', '2024-01-20', 2, 2),

-- Industry-Specific Training
(10, 'Maritime Safety and Compliance', 'Specialized training for maritime industry covering safety regulations, environmental compliance, and ship management systems.', 'Maritime', 32, 'Classroom', 'Maritime industry experience', 'Understand maritime regulations, implement safety management systems, ensure regulatory compliance, manage vessel operations', 1, '2024-01-25', '2024-01-25', 2, 2),

(11, 'Automotive Quality Systems - IATF 16949', 'Training on automotive quality management system requirements, core tools, and supplier quality management.', 'Automotive', 32, 'Blended', 'ISO 9001 knowledge, automotive industry experience', 'Implement IATF 16949, apply automotive core tools, manage supplier quality, ensure customer satisfaction', 1, '2024-01-25', '2024-01-25', 2, 2),

(12, 'Food Safety Management - FSSC 22000', 'Comprehensive training on food safety management systems, HACCP principles, and food industry compliance.', 'Food Safety', 24, 'Classroom', 'Food industry knowledge, HACCP awareness', 'Implement food safety systems, apply HACCP principles, ensure regulatory compliance, manage food safety risks', 1, '2024-01-25', '2024-01-25', 2, 2),

(13, 'Medical Device Quality Systems - ISO 13485', 'Training on medical device quality management systems, regulatory requirements, and risk management for medical devices.', 'Medical Devices', 32, 'Blended', 'Quality management experience, medical device knowledge', 'Implement ISO 13485, manage medical device risks, ensure regulatory compliance, design controls', 1, '2024-01-25', '2024-01-25', 2, 2),

-- Cybersecurity and Technology Training
(14, 'Cybersecurity Awareness and Best Practices', 'Essential cybersecurity training covering threat awareness, security controls, and best practices for information protection.', 'Cybersecurity', 8, 'Online', 'Basic computer literacy', 'Recognize security threats, implement security controls, follow security policies, protect sensitive information', 1, '2024-02-01', '2024-02-01', 2, 2),

(15, 'GDPR and Data Protection Compliance', 'Training on General Data Protection Regulation requirements, privacy principles, and data protection implementation.', 'Compliance', 16, 'Online', 'Basic privacy awareness', 'Understand GDPR requirements, implement privacy controls, conduct privacy impact assessments, handle data breaches', 1, '2024-02-01', '2024-02-01', 2, 2),

(16, 'Cloud Security and Compliance', 'Advanced training on cloud security frameworks, compliance requirements, and secure cloud implementation strategies.', 'Technology', 24, 'Blended', 'IT security knowledge, cloud basics', 'Implement cloud security, ensure cloud compliance, manage cloud risks, secure cloud architectures', 1, '2024-02-01', '2024-02-01', 2, 2),

-- Renewable Energy Training
(17, 'Wind Energy Systems and Safety', 'Specialized training for wind energy sector covering turbine systems, maintenance procedures, and safety protocols.', 'Renewable Energy', 40, 'Classroom', 'Engineering background, safety training', 'Understand wind systems, implement safety procedures, conduct inspections, manage maintenance programs', 1, '2024-02-05', '2024-02-05', 2, 2),

(18, 'Solar Energy Systems Certification', 'Comprehensive training on solar energy systems, installation standards, performance monitoring, and safety requirements.', 'Renewable Energy', 32, 'Blended', 'Electrical knowledge, renewable energy basics', 'Design solar systems, ensure installation quality, monitor performance, maintain safety standards', 1, '2024-02-05', '2024-02-05', 2, 2),

-- Management and Leadership Training
(19, 'Management Systems Integration', 'Advanced training on integrating multiple management systems (ISO 9001, 14001, 45001) for improved efficiency and effectiveness.', 'Management', 24, 'Classroom', 'Multiple ISO standard knowledge', 'Integrate management systems, optimize processes, reduce documentation, improve system effectiveness', 1, '2024-02-10', '2024-02-10', 2, 2),

(20, 'Change Management and Continuous Improvement', 'Training on change management principles, continuous improvement methodologies, and organizational development.', 'Management', 16, 'Online', 'Management experience', 'Lead organizational change, implement improvement processes, engage stakeholders, measure improvement', 1, '2024-02-10', '2024-02-10', 2, 2),

-- Compliance and Regulatory Training
(21, 'Environmental Regulatory Compliance', 'Training on environmental regulations, compliance monitoring, reporting requirements, and environmental impact assessment.', 'Compliance', 24, 'Blended', 'Environmental knowledge', 'Ensure regulatory compliance, conduct compliance audits, manage environmental permits, report environmental data', 1, '2024-02-15', '2024-02-15', 2, 2),

(22, 'Health and Safety Regulatory Updates', 'Regular updates on occupational health and safety regulations, legal requirements, and compliance obligations.', 'Compliance', 8, 'Online', 'OH&S knowledge', 'Understand regulatory changes, update safety procedures, ensure legal compliance, manage safety obligations', 1, '2024-02-15', '2024-02-15', 2, 2),

-- Specialized Assessment Training
(23, 'Business Continuity Management', 'Training on business continuity planning, crisis management, disaster recovery, and organizational resilience.', 'Risk Management', 24, 'Classroom', 'Risk management basics', 'Develop continuity plans, manage business risks, implement recovery procedures, test continuity systems', 1, '2024-02-20', '2024-02-20', 2, 2),

(24, 'Supply Chain Risk Management', 'Training on supply chain risk assessment, supplier evaluation, supply chain resilience, and vendor management.', 'Risk Management', 16, 'Online', 'Supply chain knowledge', 'Assess supply chain risks, evaluate suppliers, implement risk controls, ensure supply chain continuity', 1, '2024-02-20', '2024-02-20', 2, 2),

-- Communication and Soft Skills
(25, 'Effective Communication for Auditors', 'Training on communication skills, presentation techniques, conflict resolution, and stakeholder engagement for audit professionals.', 'Soft Skills', 16, 'Classroom', 'Audit experience', 'Improve communication skills, manage conflicts, engage stakeholders, present findings effectively', 1, '2024-02-25', '2024-02-25', 2, 2),

-- New and Emerging Standards
(26, 'ISO 56002:2019 Innovation Management', 'Training on innovation management systems, innovation processes, and organizational innovation capabilities.', 'ISO Standards', 16, 'Online', 'Management experience', 'Implement innovation systems, manage innovation processes, foster innovation culture, measure innovation performance', 1, '2024-03-01', '2024-03-01', 2, 2),

(27, 'Artificial Intelligence Ethics and Governance', 'Training on AI ethics, governance frameworks, responsible AI implementation, and AI risk management.', 'Technology', 16, 'Blended', 'Basic AI knowledge', 'Understand AI ethics, implement AI governance, manage AI risks, ensure responsible AI use', 1, '2024-03-01', '2024-03-01', 2, 2),

-- Refresher and Updates
(28, 'Annual Quality System Updates', 'Annual refresher training covering updates to quality management standards, best practices, and regulatory changes.', 'Refresher', 8, 'Online', 'QMS knowledge', 'Stay current with standards, implement updates, maintain competency, apply best practices', 1, '2024-03-05', '2024-03-05', 2, 2),

(29, 'Lead Auditor Refresher Course', 'Mandatory refresher training for certified lead auditors to maintain certification and update skills.', 'Refresher', 16, 'Classroom', 'Lead auditor certification', 'Maintain certification, update audit skills, learn new techniques, network with peers', 1, '2024-03-05', '2024-03-05', 2, 2),

-- Digital Transformation Training
(30, 'Digital Transformation in Quality Management', 'Training on digital tools, automation technologies, and digital quality management transformation.', 'Technology', 24, 'Blended', 'Quality management experience', 'Implement digital tools, automate processes, leverage technology, improve digital capabilities', 1, '2024-03-10', '2024-03-10', 2, 2);

SET IDENTITY_INSERT [dbo].[Trainings] OFF;

-- Verify the insert
SELECT COUNT(*) as TotalTrainings FROM [dbo].[Trainings];

-- Show training statistics by category
SELECT 
    Category,
    COUNT(*) as TrainingCount,
    AVG(Duration) as AverageDuration,
    COUNT(CASE WHEN DeliveryMethod = 'Online' THEN 1 END) as OnlineTrainings,
    COUNT(CASE WHEN DeliveryMethod = 'Classroom' THEN 1 END) as ClassroomTrainings,
    COUNT(CASE WHEN DeliveryMethod = 'Blended' THEN 1 END) as BlendedTrainings
FROM [dbo].[Trainings] 
WHERE IsActive = 1
GROUP BY Category
ORDER BY TrainingCount DESC;

-- Show training by delivery method
SELECT 
    DeliveryMethod,
    COUNT(*) as Count,
    AVG(Duration) as AverageDuration,
    MIN(Duration) as MinDuration,
    MAX(Duration) as MaxDuration
FROM [dbo].[Trainings] 
WHERE IsActive = 1
GROUP BY DeliveryMethod
ORDER BY Count DESC;

-- Show longest and shortest training courses
SELECT 
    TrainingName,
    Category,
    Duration,
    DeliveryMethod,
    CASE 
        WHEN Duration >= 32 THEN 'Long Course (32+ hours)'
        WHEN Duration >= 16 THEN 'Medium Course (16-31 hours)'
        WHEN Duration >= 8 THEN 'Short Course (8-15 hours)'
        ELSE 'Quick Course (<8 hours)'
    END as CourseLength
FROM [dbo].[Trainings] 
WHERE IsActive = 1
ORDER BY Duration DESC, TrainingName;

-- Show training creation timeline
SELECT 
    YEAR(CreatedDate) as Year,
    MONTH(CreatedDate) as Month,
    COUNT(*) as TrainingsCreated
FROM [dbo].[Trainings] 
GROUP BY YEAR(CreatedDate), MONTH(CreatedDate)
ORDER BY Year, Month;
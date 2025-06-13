CREATE TABLE "Absences" (
    "Id" uuid NOT NULL,
    "SpecializationId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "Type" text NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "DurationInDays" integer NOT NULL,
    "Description" character varying(1000),
    "DocumentPath" character varying(500),
    "IsApproved" boolean NOT NULL DEFAULT FALSE,
    "ApprovedAt" timestamp with time zone,
    "ApprovedBy" integer,
    "SyncStatus" text NOT NULL DEFAULT 'NotSynced',
    "AdditionalFields" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Absences" PRIMARY KEY ("Id")
);


CREATE TABLE "Courses" (
    "Id" integer NOT NULL,
    "SpecializationId" integer NOT NULL,
    "ModuleId" integer,
    "CourseType" integer NOT NULL,
    "CourseName" character varying(300) NOT NULL,
    "CourseNumber" character varying(100),
    "InstitutionName" character varying(200) NOT NULL,
    "CompletionDate" timestamp with time zone NOT NULL,
    "HasCertificate" boolean NOT NULL,
    "CertificateNumber" character varying(100),
    "IsApproved" boolean NOT NULL,
    "ApprovalDate" timestamp with time zone,
    "ApproverName" character varying(200),
    "SyncStatus" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Courses" PRIMARY KEY ("Id")
);


CREATE TABLE "Internships" (
    "Id" integer NOT NULL,
    "SpecializationId" integer NOT NULL,
    "ModuleId" integer,
    "InstitutionName" character varying(200) NOT NULL,
    "DepartmentName" character varying(200) NOT NULL,
    "SupervisorName" character varying(200),
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "DaysCount" integer NOT NULL,
    "IsCompleted" boolean NOT NULL,
    "IsApproved" boolean NOT NULL,
    "ApprovalDate" timestamp with time zone,
    "ApproverName" character varying(200),
    "SyncStatus" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Internships" PRIMARY KEY ("Id")
);


CREATE TABLE "Publications" (
    "Id" uuid NOT NULL,
    "SpecializationId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "Type" text NOT NULL,
    "Title" character varying(1000) NOT NULL,
    "Authors" character varying(2000),
    "Journal" character varying(500),
    "Publisher" character varying(500),
    "PublicationDate" timestamp with time zone NOT NULL,
    "Volume" character varying(50),
    "Issue" character varying(50),
    "Pages" character varying(50),
    "DOI" character varying(255),
    "PMID" character varying(50),
    "ISBN" character varying(50),
    "URL" character varying(1000),
    "Abstract" character varying(5000),
    "Keywords" character varying(1000),
    "FilePath" character varying(500),
    "IsFirstAuthor" boolean NOT NULL DEFAULT FALSE,
    "IsCorrespondingAuthor" boolean NOT NULL DEFAULT FALSE,
    "IsPeerReviewed" boolean NOT NULL DEFAULT FALSE,
    "SyncStatus" text NOT NULL DEFAULT 'NotSynced',
    "AdditionalFields" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Publications" PRIMARY KEY ("Id")
);


CREATE TABLE "Recognitions" (
    "Id" uuid NOT NULL,
    "SpecializationId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "Type" text NOT NULL,
    "Title" character varying(500) NOT NULL,
    "Description" character varying(2000),
    "Institution" character varying(500),
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "DaysReduction" integer NOT NULL,
    "IsApproved" boolean NOT NULL DEFAULT FALSE,
    "ApprovedAt" timestamp with time zone,
    "ApprovedBy" integer,
    "DocumentPath" character varying(500),
    "SyncStatus" text NOT NULL DEFAULT 'NotSynced',
    "AdditionalFields" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Recognitions" PRIMARY KEY ("Id")
);


CREATE TABLE "SelfEducations" (
    "Id" uuid NOT NULL,
    "SpecializationId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "Type" text NOT NULL,
    "Year" integer NOT NULL,
    "Title" character varying(500) NOT NULL,
    "Description" character varying(2000),
    "Provider" character varying(500),
    "Publisher" character varying(500),
    "StartDate" timestamp with time zone,
    "EndDate" timestamp with time zone,
    "DurationHours" integer,
    "IsCompleted" boolean NOT NULL DEFAULT FALSE,
    "CompletedAt" timestamp with time zone,
    "CertificatePath" character varying(500),
    "URL" character varying(1000),
    "ISBN" character varying(50),
    "DOI" character varying(255),
    "CreditHours" integer NOT NULL,
    "SyncStatus" text NOT NULL DEFAULT 'NotSynced',
    "AdditionalFields" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_SelfEducations" PRIMARY KEY ("Id")
);


CREATE TABLE "Specializations" (
    "Id" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "ProgramCode" character varying(20) NOT NULL,
    "SmkVersion" integer NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "PlannedEndDate" timestamp with time zone NOT NULL,
    "CalculatedEndDate" timestamp with time zone NOT NULL,
    "ProgramStructure" text NOT NULL,
    "CurrentModuleId" integer,
    "DurationYears" integer NOT NULL,
    CONSTRAINT "PK_Specializations" PRIMARY KEY ("Id")
);


CREATE TABLE "Users" (
    "Id" integer NOT NULL,
    "Email" character varying(100) NOT NULL,
    "Username" character varying(50) NOT NULL,
    "Password" text NOT NULL,
    "FullName" character varying(200) NOT NULL,
    "SmkVersion" integer NOT NULL,
    "SpecializationId" integer NOT NULL,
    "RegistrationDate" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);


CREATE TABLE "MedicalShifts" (
    "Id" integer NOT NULL,
    "InternshipId" integer NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "Hours" integer NOT NULL,
    "Minutes" integer NOT NULL,
    "Location" character varying(100) NOT NULL,
    "Year" integer NOT NULL,
    "SyncStatus" integer NOT NULL,
    "AdditionalFields" text,
    "ApprovalDate" timestamp with time zone,
    "ApproverName" character varying(100),
    "ApproverRole" character varying(100),
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_MedicalShifts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_MedicalShifts_Internships_InternshipId" FOREIGN KEY ("InternshipId") REFERENCES "Internships" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Procedures" (
    "Id" integer NOT NULL,
    "InternshipId" integer NOT NULL,
    "Date" timestamp with time zone NOT NULL,
    "Year" integer NOT NULL,
    "Code" character varying(20) NOT NULL,
    "OperatorCode" character varying(10),
    "PerformingPerson" character varying(100),
    "Location" character varying(100) NOT NULL,
    "PatientInitials" character varying(10),
    "PatientGender" character(1),
    "AssistantData" text,
    "ProcedureGroup" text,
    "Status" integer NOT NULL,
    "SyncStatus" integer NOT NULL,
    "AdditionalFields" text,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    "SmkVersion" integer NOT NULL,
    CONSTRAINT "PK_Procedures" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Procedures_Internships_InternshipId" FOREIGN KEY ("InternshipId") REFERENCES "Internships" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Modules" (
    "Id" integer NOT NULL,
    "SpecializationId" integer NOT NULL,
    "Type" integer NOT NULL,
    "SmkVersion" integer NOT NULL,
    "Version" text NOT NULL,
    "Name" character varying(100) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "Structure" text NOT NULL,
    "CompletedInternships" integer NOT NULL,
    "TotalInternships" integer NOT NULL,
    "CompletedCourses" integer NOT NULL,
    "TotalCourses" integer NOT NULL,
    "CompletedProceduresA" integer NOT NULL,
    "TotalProceduresA" integer NOT NULL,
    "CompletedProceduresB" integer NOT NULL,
    "TotalProceduresB" integer NOT NULL,
    "CompletedShiftHours" integer NOT NULL,
    "RequiredShiftHours" integer NOT NULL,
    "WeeklyShiftHours" double precision NOT NULL,
    "CompletedSelfEducationDays" integer NOT NULL,
    "TotalSelfEducationDays" integer NOT NULL,
    CONSTRAINT "PK_Modules" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Modules_Specializations_SpecializationId" FOREIGN KEY ("SpecializationId") REFERENCES "Specializations" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_Absences_SpecializationId" ON "Absences" ("SpecializationId");


CREATE INDEX "IX_Absences_Type" ON "Absences" ("Type");


CREATE INDEX "IX_Absences_UserId" ON "Absences" ("UserId");


CREATE INDEX "IX_Absences_UserId_StartDate_EndDate" ON "Absences" ("UserId", "StartDate", "EndDate");


CREATE INDEX "IX_Courses_CompletionDate" ON "Courses" ("CompletionDate");


CREATE INDEX "IX_Courses_CourseType" ON "Courses" ("CourseType");


CREATE INDEX "IX_Courses_ModuleId" ON "Courses" ("ModuleId");


CREATE INDEX "IX_Courses_SpecializationId" ON "Courses" ("SpecializationId");


CREATE INDEX "IX_Internships_ModuleId" ON "Internships" ("ModuleId");


CREATE INDEX "IX_Internships_SpecializationId" ON "Internships" ("SpecializationId");


CREATE INDEX "IX_Internships_StartDate_EndDate" ON "Internships" ("StartDate", "EndDate");


CREATE INDEX "IX_MedicalShifts_Date" ON "MedicalShifts" ("Date");


CREATE INDEX "IX_MedicalShifts_InternshipId" ON "MedicalShifts" ("InternshipId");


CREATE INDEX "IX_Modules_SpecializationId" ON "Modules" ("SpecializationId");


CREATE INDEX "IX_Procedures_Code" ON "Procedures" ("Code");


CREATE INDEX "IX_Procedures_Date" ON "Procedures" ("Date");


CREATE INDEX "IX_Procedures_InternshipId" ON "Procedures" ("InternshipId");


CREATE INDEX "IX_Publications_IsFirstAuthor" ON "Publications" ("IsFirstAuthor");


CREATE INDEX "IX_Publications_IsPeerReviewed" ON "Publications" ("IsPeerReviewed");


CREATE INDEX "IX_Publications_PublicationDate" ON "Publications" ("PublicationDate");


CREATE INDEX "IX_Publications_SpecializationId" ON "Publications" ("SpecializationId");


CREATE INDEX "IX_Publications_Type" ON "Publications" ("Type");


CREATE INDEX "IX_Publications_UserId" ON "Publications" ("UserId");


CREATE INDEX "IX_Publications_UserId_SpecializationId" ON "Publications" ("UserId", "SpecializationId");


CREATE INDEX "IX_Recognitions_IsApproved" ON "Recognitions" ("IsApproved");


CREATE INDEX "IX_Recognitions_SpecializationId" ON "Recognitions" ("SpecializationId");


CREATE INDEX "IX_Recognitions_Type" ON "Recognitions" ("Type");


CREATE INDEX "IX_Recognitions_UserId" ON "Recognitions" ("UserId");


CREATE INDEX "IX_Recognitions_UserId_SpecializationId" ON "Recognitions" ("UserId", "SpecializationId");


CREATE INDEX "IX_SelfEducations_IsCompleted" ON "SelfEducations" ("IsCompleted");


CREATE INDEX "IX_SelfEducations_SpecializationId" ON "SelfEducations" ("SpecializationId");


CREATE INDEX "IX_SelfEducations_Type" ON "SelfEducations" ("Type");


CREATE INDEX "IX_SelfEducations_UserId" ON "SelfEducations" ("UserId");


CREATE INDEX "IX_SelfEducations_UserId_SpecializationId" ON "SelfEducations" ("UserId", "SpecializationId");


CREATE INDEX "IX_SelfEducations_Year" ON "SelfEducations" ("Year");


CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");


CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");



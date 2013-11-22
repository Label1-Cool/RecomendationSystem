
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 11/22/2013 15:26:05
-- Generated from EDMX file: e:\Slackerburst\University\Graduate work 2013\RecomendationSystem\DataLayer\RecomendationSystemModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [RecomendationSystemDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserSection_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSection] DROP CONSTRAINT [FK_UserSection_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSection_Section]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSection] DROP CONSTRAINT [FK_UserSection_Section];
GO
IF OBJECT_ID(N'[dbo].[FK_UserHobbie_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserHobbie] DROP CONSTRAINT [FK_UserHobbie_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserHobbie_Hobbie]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserHobbie] DROP CONSTRAINT [FK_UserHobbie_Hobbie];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSchool_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSchool] DROP CONSTRAINT [FK_UserSchool_User];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSchool_School]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UserSchool] DROP CONSTRAINT [FK_UserSchool_School];
GO
IF OBJECT_ID(N'[dbo].[FK_SchoolTypeSchool]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schools] DROP CONSTRAINT [FK_SchoolTypeSchool];
GO
IF OBJECT_ID(N'[dbo].[FK_UserPreference]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Preferences] DROP CONSTRAINT [FK_UserPreference];
GO
IF OBJECT_ID(N'[dbo].[FK_UserParticipationInCompetition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParticipationInCompetitions] DROP CONSTRAINT [FK_UserParticipationInCompetition];
GO
IF OBJECT_ID(N'[dbo].[FK_CompetitionParticipationInCompetition]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ParticipationInCompetitions] DROP CONSTRAINT [FK_CompetitionParticipationInCompetition];
GO
IF OBJECT_ID(N'[dbo].[FK_DisciplineUnitedStateExam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UnitedStateExams] DROP CONSTRAINT [FK_DisciplineUnitedStateExam];
GO
IF OBJECT_ID(N'[dbo].[FK_PreferenceCity_Preference]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PreferenceCity] DROP CONSTRAINT [FK_PreferenceCity_Preference];
GO
IF OBJECT_ID(N'[dbo].[FK_PreferenceCity_City]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PreferenceCity] DROP CONSTRAINT [FK_PreferenceCity_City];
GO
IF OBJECT_ID(N'[dbo].[FK_CityUniversity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[HighSchools] DROP CONSTRAINT [FK_CityUniversity];
GO
IF OBJECT_ID(N'[dbo].[FK_GeneralEducationLineDepartmentEducationLine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DepartmentEducationLines] DROP CONSTRAINT [FK_GeneralEducationLineDepartmentEducationLine];
GO
IF OBJECT_ID(N'[dbo].[FK_UniversityUniversityDepartment]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UniversityDepartments] DROP CONSTRAINT [FK_UniversityUniversityDepartment];
GO
IF OBJECT_ID(N'[dbo].[FK_UniversityDepartmentDepartmentEducationLine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DepartmentEducationLines] DROP CONSTRAINT [FK_UniversityDepartmentDepartmentEducationLine];
GO
IF OBJECT_ID(N'[dbo].[FK_DepartmentEducationLineDepartmentLinesRequirement]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DepartmentLinesRequirements] DROP CONSTRAINT [FK_DepartmentEducationLineDepartmentLinesRequirement];
GO
IF OBJECT_ID(N'[dbo].[FK_DepartmentLinesRequirementDiscipline]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DepartmentLinesRequirements] DROP CONSTRAINT [FK_DepartmentLinesRequirementDiscipline];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUnitedStateExam]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[UnitedStateExams] DROP CONSTRAINT [FK_UserUnitedStateExam];
GO
IF OBJECT_ID(N'[dbo].[FK_UserSchoolMark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SchoolMarks] DROP CONSTRAINT [FK_UserSchoolMark];
GO
IF OBJECT_ID(N'[dbo].[FK_SchoolCity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Schools] DROP CONSTRAINT [FK_SchoolCity];
GO
IF OBJECT_ID(N'[dbo].[FK_SchoolDisciplineSchoolMark]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SchoolMarks] DROP CONSTRAINT [FK_SchoolDisciplineSchoolMark];
GO
IF OBJECT_ID(N'[dbo].[FK_ClusterWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_ClusterWeight];
GO
IF OBJECT_ID(N'[dbo].[FK_SchoolTypeWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_SchoolTypeWeight];
GO
IF OBJECT_ID(N'[dbo].[FK_SchoolDisciplineWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_SchoolDisciplineWeight];
GO
IF OBJECT_ID(N'[dbo].[FK_SectionWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_SectionWeight];
GO
IF OBJECT_ID(N'[dbo].[FK_HobbieWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_HobbieWeight];
GO
IF OBJECT_ID(N'[dbo].[FK_CompetitionWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_CompetitionWeight];
GO
IF OBJECT_ID(N'[dbo].[FK_DisciplineWeight]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Weights] DROP CONSTRAINT [FK_DisciplineWeight];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Sections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sections];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[SchoolTypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SchoolTypes];
GO
IF OBJECT_ID(N'[dbo].[Schools]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Schools];
GO
IF OBJECT_ID(N'[dbo].[Preferences]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Preferences];
GO
IF OBJECT_ID(N'[dbo].[Hobbies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Hobbies];
GO
IF OBJECT_ID(N'[dbo].[ParticipationInCompetitions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ParticipationInCompetitions];
GO
IF OBJECT_ID(N'[dbo].[Competitions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Competitions];
GO
IF OBJECT_ID(N'[dbo].[UnitedStateExams]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UnitedStateExams];
GO
IF OBJECT_ID(N'[dbo].[SchoolMarks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SchoolMarks];
GO
IF OBJECT_ID(N'[dbo].[Disciplines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Disciplines];
GO
IF OBJECT_ID(N'[dbo].[Clusters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Clusters];
GO
IF OBJECT_ID(N'[dbo].[HighSchools]', 'U') IS NOT NULL
    DROP TABLE [dbo].[HighSchools];
GO
IF OBJECT_ID(N'[dbo].[Cities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Cities];
GO
IF OBJECT_ID(N'[dbo].[UniversityDepartments]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UniversityDepartments];
GO
IF OBJECT_ID(N'[dbo].[DepartmentEducationLines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DepartmentEducationLines];
GO
IF OBJECT_ID(N'[dbo].[GeneralEducationLines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[GeneralEducationLines];
GO
IF OBJECT_ID(N'[dbo].[DepartmentLinesRequirements]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DepartmentLinesRequirements];
GO
IF OBJECT_ID(N'[dbo].[SchoolDisciplines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SchoolDisciplines];
GO
IF OBJECT_ID(N'[dbo].[Weights]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Weights];
GO
IF OBJECT_ID(N'[dbo].[UserSection]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSection];
GO
IF OBJECT_ID(N'[dbo].[UserHobbie]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserHobbie];
GO
IF OBJECT_ID(N'[dbo].[UserSchool]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserSchool];
GO
IF OBJECT_ID(N'[dbo].[PreferenceCity]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PreferenceCity];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Sections'
CREATE TABLE [dbo].[Sections] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ActivityType] nvarchar(max)  NOT NULL,
    [Period] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(max)  NOT NULL,
    [LastName] nvarchar(max)  NOT NULL,
    [Gender] nvarchar(max)  NULL,
    [SchoolEducation] nvarchar(max)  NOT NULL,
    [Medal] nvarchar(max)  NULL,
    [Citizenship] nvarchar(max)  NULL,
    [AverageMark] float  NULL
);
GO

-- Creating table 'SchoolTypes'
CREATE TABLE [dbo].[SchoolTypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Schools'
CREATE TABLE [dbo].[Schools] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [EducationQuality] int  NOT NULL,
    [SchoolTypeId] int  NOT NULL,
    [City_Id] int  NOT NULL
);
GO

-- Creating table 'Preferences'
CREATE TABLE [dbo].[Preferences] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Presige] int  NOT NULL,
    [EducationFrom] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'Hobbies'
CREATE TABLE [dbo].[Hobbies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'ParticipationInCompetitions'
CREATE TABLE [dbo].[ParticipationInCompetitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Result] nvarchar(max)  NOT NULL,
    [UserId] int  NOT NULL,
    [CompetitionId] int  NOT NULL
);
GO

-- Creating table 'Competitions'
CREATE TABLE [dbo].[Competitions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'UnitedStateExams'
CREATE TABLE [dbo].[UnitedStateExams] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Result] int  NOT NULL,
    [DisciplineId] int  NOT NULL,
    [UserId] int  NOT NULL
);
GO

-- Creating table 'SchoolMarks'
CREATE TABLE [dbo].[SchoolMarks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Result] smallint  NOT NULL,
    [Respect] int  NULL,
    [UserId] int  NOT NULL,
    [SchoolDisciplineId] int  NOT NULL
);
GO

-- Creating table 'Disciplines'
CREATE TABLE [dbo].[Disciplines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Clusters'
CREATE TABLE [dbo].[Clusters] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'HighSchools'
CREATE TABLE [dbo].[HighSchools] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Prestige] int  NOT NULL,
    [CityId] int  NOT NULL
);
GO

-- Creating table 'Cities'
CREATE TABLE [dbo].[Cities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Prestige] int  NOT NULL
);
GO

-- Creating table 'UniversityDepartments'
CREATE TABLE [dbo].[UniversityDepartments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Prestige] int  NOT NULL,
    [UniversityId] int  NOT NULL
);
GO

-- Creating table 'DepartmentEducationLines'
CREATE TABLE [dbo].[DepartmentEducationLines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [GeneralEducationLineId] int  NULL,
    [UniversityDepartmentId] int  NOT NULL,
    [EducationForm] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NULL,
    [RequiredSum] int  NULL,
    [Actual] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'GeneralEducationLines'
CREATE TABLE [dbo].[GeneralEducationLines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'DepartmentLinesRequirements'
CREATE TABLE [dbo].[DepartmentLinesRequirements] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Requirement] int  NOT NULL,
    [DepartmentEducationLineId] int  NOT NULL,
    [DisciplineId] int  NOT NULL
);
GO

-- Creating table 'SchoolDisciplines'
CREATE TABLE [dbo].[SchoolDisciplines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Weights'
CREATE TABLE [dbo].[Weights] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Coefficient] float  NOT NULL,
    [ClusterId] int  NOT NULL,
    [SchoolTypeId] int  NULL,
    [SchoolDisciplineId] int  NULL,
    [SectionId] int  NULL,
    [HobbieId] int  NULL,
    [CompetitionId] int  NULL,
    [DisciplineId] int  NULL
);
GO

-- Creating table 'UserSection'
CREATE TABLE [dbo].[UserSection] (
    [User_Id] int  NOT NULL,
    [Section_Id] int  NOT NULL
);
GO

-- Creating table 'UserHobbie'
CREATE TABLE [dbo].[UserHobbie] (
    [User_Id] int  NOT NULL,
    [Hobbie_Id] int  NOT NULL
);
GO

-- Creating table 'UserSchool'
CREATE TABLE [dbo].[UserSchool] (
    [User_Id] int  NOT NULL,
    [School_Id] int  NOT NULL
);
GO

-- Creating table 'PreferenceCity'
CREATE TABLE [dbo].[PreferenceCity] (
    [Preference_Id] int  NOT NULL,
    [City_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Sections'
ALTER TABLE [dbo].[Sections]
ADD CONSTRAINT [PK_Sections]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SchoolTypes'
ALTER TABLE [dbo].[SchoolTypes]
ADD CONSTRAINT [PK_SchoolTypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Schools'
ALTER TABLE [dbo].[Schools]
ADD CONSTRAINT [PK_Schools]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Preferences'
ALTER TABLE [dbo].[Preferences]
ADD CONSTRAINT [PK_Preferences]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Hobbies'
ALTER TABLE [dbo].[Hobbies]
ADD CONSTRAINT [PK_Hobbies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ParticipationInCompetitions'
ALTER TABLE [dbo].[ParticipationInCompetitions]
ADD CONSTRAINT [PK_ParticipationInCompetitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Competitions'
ALTER TABLE [dbo].[Competitions]
ADD CONSTRAINT [PK_Competitions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UnitedStateExams'
ALTER TABLE [dbo].[UnitedStateExams]
ADD CONSTRAINT [PK_UnitedStateExams]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SchoolMarks'
ALTER TABLE [dbo].[SchoolMarks]
ADD CONSTRAINT [PK_SchoolMarks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Disciplines'
ALTER TABLE [dbo].[Disciplines]
ADD CONSTRAINT [PK_Disciplines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Clusters'
ALTER TABLE [dbo].[Clusters]
ADD CONSTRAINT [PK_Clusters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'HighSchools'
ALTER TABLE [dbo].[HighSchools]
ADD CONSTRAINT [PK_HighSchools]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Cities'
ALTER TABLE [dbo].[Cities]
ADD CONSTRAINT [PK_Cities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UniversityDepartments'
ALTER TABLE [dbo].[UniversityDepartments]
ADD CONSTRAINT [PK_UniversityDepartments]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DepartmentEducationLines'
ALTER TABLE [dbo].[DepartmentEducationLines]
ADD CONSTRAINT [PK_DepartmentEducationLines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'GeneralEducationLines'
ALTER TABLE [dbo].[GeneralEducationLines]
ADD CONSTRAINT [PK_GeneralEducationLines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DepartmentLinesRequirements'
ALTER TABLE [dbo].[DepartmentLinesRequirements]
ADD CONSTRAINT [PK_DepartmentLinesRequirements]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SchoolDisciplines'
ALTER TABLE [dbo].[SchoolDisciplines]
ADD CONSTRAINT [PK_SchoolDisciplines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [PK_Weights]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [User_Id], [Section_Id] in table 'UserSection'
ALTER TABLE [dbo].[UserSection]
ADD CONSTRAINT [PK_UserSection]
    PRIMARY KEY CLUSTERED ([User_Id], [Section_Id] ASC);
GO

-- Creating primary key on [User_Id], [Hobbie_Id] in table 'UserHobbie'
ALTER TABLE [dbo].[UserHobbie]
ADD CONSTRAINT [PK_UserHobbie]
    PRIMARY KEY CLUSTERED ([User_Id], [Hobbie_Id] ASC);
GO

-- Creating primary key on [User_Id], [School_Id] in table 'UserSchool'
ALTER TABLE [dbo].[UserSchool]
ADD CONSTRAINT [PK_UserSchool]
    PRIMARY KEY CLUSTERED ([User_Id], [School_Id] ASC);
GO

-- Creating primary key on [Preference_Id], [City_Id] in table 'PreferenceCity'
ALTER TABLE [dbo].[PreferenceCity]
ADD CONSTRAINT [PK_PreferenceCity]
    PRIMARY KEY CLUSTERED ([Preference_Id], [City_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'UserSection'
ALTER TABLE [dbo].[UserSection]
ADD CONSTRAINT [FK_UserSection_User]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Section_Id] in table 'UserSection'
ALTER TABLE [dbo].[UserSection]
ADD CONSTRAINT [FK_UserSection_Section]
    FOREIGN KEY ([Section_Id])
    REFERENCES [dbo].[Sections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSection_Section'
CREATE INDEX [IX_FK_UserSection_Section]
ON [dbo].[UserSection]
    ([Section_Id]);
GO

-- Creating foreign key on [User_Id] in table 'UserHobbie'
ALTER TABLE [dbo].[UserHobbie]
ADD CONSTRAINT [FK_UserHobbie_User]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Hobbie_Id] in table 'UserHobbie'
ALTER TABLE [dbo].[UserHobbie]
ADD CONSTRAINT [FK_UserHobbie_Hobbie]
    FOREIGN KEY ([Hobbie_Id])
    REFERENCES [dbo].[Hobbies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserHobbie_Hobbie'
CREATE INDEX [IX_FK_UserHobbie_Hobbie]
ON [dbo].[UserHobbie]
    ([Hobbie_Id]);
GO

-- Creating foreign key on [User_Id] in table 'UserSchool'
ALTER TABLE [dbo].[UserSchool]
ADD CONSTRAINT [FK_UserSchool_User]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [School_Id] in table 'UserSchool'
ALTER TABLE [dbo].[UserSchool]
ADD CONSTRAINT [FK_UserSchool_School]
    FOREIGN KEY ([School_Id])
    REFERENCES [dbo].[Schools]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSchool_School'
CREATE INDEX [IX_FK_UserSchool_School]
ON [dbo].[UserSchool]
    ([School_Id]);
GO

-- Creating foreign key on [SchoolTypeId] in table 'Schools'
ALTER TABLE [dbo].[Schools]
ADD CONSTRAINT [FK_SchoolTypeSchool]
    FOREIGN KEY ([SchoolTypeId])
    REFERENCES [dbo].[SchoolTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchoolTypeSchool'
CREATE INDEX [IX_FK_SchoolTypeSchool]
ON [dbo].[Schools]
    ([SchoolTypeId]);
GO

-- Creating foreign key on [UserId] in table 'Preferences'
ALTER TABLE [dbo].[Preferences]
ADD CONSTRAINT [FK_UserPreference]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserPreference'
CREATE INDEX [IX_FK_UserPreference]
ON [dbo].[Preferences]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'ParticipationInCompetitions'
ALTER TABLE [dbo].[ParticipationInCompetitions]
ADD CONSTRAINT [FK_UserParticipationInCompetition]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserParticipationInCompetition'
CREATE INDEX [IX_FK_UserParticipationInCompetition]
ON [dbo].[ParticipationInCompetitions]
    ([UserId]);
GO

-- Creating foreign key on [CompetitionId] in table 'ParticipationInCompetitions'
ALTER TABLE [dbo].[ParticipationInCompetitions]
ADD CONSTRAINT [FK_CompetitionParticipationInCompetition]
    FOREIGN KEY ([CompetitionId])
    REFERENCES [dbo].[Competitions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompetitionParticipationInCompetition'
CREATE INDEX [IX_FK_CompetitionParticipationInCompetition]
ON [dbo].[ParticipationInCompetitions]
    ([CompetitionId]);
GO

-- Creating foreign key on [DisciplineId] in table 'UnitedStateExams'
ALTER TABLE [dbo].[UnitedStateExams]
ADD CONSTRAINT [FK_DisciplineUnitedStateExam]
    FOREIGN KEY ([DisciplineId])
    REFERENCES [dbo].[Disciplines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DisciplineUnitedStateExam'
CREATE INDEX [IX_FK_DisciplineUnitedStateExam]
ON [dbo].[UnitedStateExams]
    ([DisciplineId]);
GO

-- Creating foreign key on [Preference_Id] in table 'PreferenceCity'
ALTER TABLE [dbo].[PreferenceCity]
ADD CONSTRAINT [FK_PreferenceCity_Preference]
    FOREIGN KEY ([Preference_Id])
    REFERENCES [dbo].[Preferences]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [City_Id] in table 'PreferenceCity'
ALTER TABLE [dbo].[PreferenceCity]
ADD CONSTRAINT [FK_PreferenceCity_City]
    FOREIGN KEY ([City_Id])
    REFERENCES [dbo].[Cities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PreferenceCity_City'
CREATE INDEX [IX_FK_PreferenceCity_City]
ON [dbo].[PreferenceCity]
    ([City_Id]);
GO

-- Creating foreign key on [CityId] in table 'HighSchools'
ALTER TABLE [dbo].[HighSchools]
ADD CONSTRAINT [FK_CityUniversity]
    FOREIGN KEY ([CityId])
    REFERENCES [dbo].[Cities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CityUniversity'
CREATE INDEX [IX_FK_CityUniversity]
ON [dbo].[HighSchools]
    ([CityId]);
GO

-- Creating foreign key on [GeneralEducationLineId] in table 'DepartmentEducationLines'
ALTER TABLE [dbo].[DepartmentEducationLines]
ADD CONSTRAINT [FK_GeneralEducationLineDepartmentEducationLine]
    FOREIGN KEY ([GeneralEducationLineId])
    REFERENCES [dbo].[GeneralEducationLines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_GeneralEducationLineDepartmentEducationLine'
CREATE INDEX [IX_FK_GeneralEducationLineDepartmentEducationLine]
ON [dbo].[DepartmentEducationLines]
    ([GeneralEducationLineId]);
GO

-- Creating foreign key on [UniversityId] in table 'UniversityDepartments'
ALTER TABLE [dbo].[UniversityDepartments]
ADD CONSTRAINT [FK_UniversityUniversityDepartment]
    FOREIGN KEY ([UniversityId])
    REFERENCES [dbo].[HighSchools]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UniversityUniversityDepartment'
CREATE INDEX [IX_FK_UniversityUniversityDepartment]
ON [dbo].[UniversityDepartments]
    ([UniversityId]);
GO

-- Creating foreign key on [UniversityDepartmentId] in table 'DepartmentEducationLines'
ALTER TABLE [dbo].[DepartmentEducationLines]
ADD CONSTRAINT [FK_UniversityDepartmentDepartmentEducationLine]
    FOREIGN KEY ([UniversityDepartmentId])
    REFERENCES [dbo].[UniversityDepartments]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UniversityDepartmentDepartmentEducationLine'
CREATE INDEX [IX_FK_UniversityDepartmentDepartmentEducationLine]
ON [dbo].[DepartmentEducationLines]
    ([UniversityDepartmentId]);
GO

-- Creating foreign key on [DepartmentEducationLineId] in table 'DepartmentLinesRequirements'
ALTER TABLE [dbo].[DepartmentLinesRequirements]
ADD CONSTRAINT [FK_DepartmentEducationLineDepartmentLinesRequirement]
    FOREIGN KEY ([DepartmentEducationLineId])
    REFERENCES [dbo].[DepartmentEducationLines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DepartmentEducationLineDepartmentLinesRequirement'
CREATE INDEX [IX_FK_DepartmentEducationLineDepartmentLinesRequirement]
ON [dbo].[DepartmentLinesRequirements]
    ([DepartmentEducationLineId]);
GO

-- Creating foreign key on [DisciplineId] in table 'DepartmentLinesRequirements'
ALTER TABLE [dbo].[DepartmentLinesRequirements]
ADD CONSTRAINT [FK_DepartmentLinesRequirementDiscipline]
    FOREIGN KEY ([DisciplineId])
    REFERENCES [dbo].[Disciplines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DepartmentLinesRequirementDiscipline'
CREATE INDEX [IX_FK_DepartmentLinesRequirementDiscipline]
ON [dbo].[DepartmentLinesRequirements]
    ([DisciplineId]);
GO

-- Creating foreign key on [UserId] in table 'UnitedStateExams'
ALTER TABLE [dbo].[UnitedStateExams]
ADD CONSTRAINT [FK_UserUnitedStateExam]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUnitedStateExam'
CREATE INDEX [IX_FK_UserUnitedStateExam]
ON [dbo].[UnitedStateExams]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'SchoolMarks'
ALTER TABLE [dbo].[SchoolMarks]
ADD CONSTRAINT [FK_UserSchoolMark]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSchoolMark'
CREATE INDEX [IX_FK_UserSchoolMark]
ON [dbo].[SchoolMarks]
    ([UserId]);
GO

-- Creating foreign key on [City_Id] in table 'Schools'
ALTER TABLE [dbo].[Schools]
ADD CONSTRAINT [FK_SchoolCity]
    FOREIGN KEY ([City_Id])
    REFERENCES [dbo].[Cities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchoolCity'
CREATE INDEX [IX_FK_SchoolCity]
ON [dbo].[Schools]
    ([City_Id]);
GO

-- Creating foreign key on [SchoolDisciplineId] in table 'SchoolMarks'
ALTER TABLE [dbo].[SchoolMarks]
ADD CONSTRAINT [FK_SchoolDisciplineSchoolMark]
    FOREIGN KEY ([SchoolDisciplineId])
    REFERENCES [dbo].[SchoolDisciplines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchoolDisciplineSchoolMark'
CREATE INDEX [IX_FK_SchoolDisciplineSchoolMark]
ON [dbo].[SchoolMarks]
    ([SchoolDisciplineId]);
GO

-- Creating foreign key on [ClusterId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_ClusterWeight]
    FOREIGN KEY ([ClusterId])
    REFERENCES [dbo].[Clusters]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ClusterWeight'
CREATE INDEX [IX_FK_ClusterWeight]
ON [dbo].[Weights]
    ([ClusterId]);
GO

-- Creating foreign key on [SchoolTypeId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_SchoolTypeWeight]
    FOREIGN KEY ([SchoolTypeId])
    REFERENCES [dbo].[SchoolTypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchoolTypeWeight'
CREATE INDEX [IX_FK_SchoolTypeWeight]
ON [dbo].[Weights]
    ([SchoolTypeId]);
GO

-- Creating foreign key on [SchoolDisciplineId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_SchoolDisciplineWeight]
    FOREIGN KEY ([SchoolDisciplineId])
    REFERENCES [dbo].[SchoolDisciplines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SchoolDisciplineWeight'
CREATE INDEX [IX_FK_SchoolDisciplineWeight]
ON [dbo].[Weights]
    ([SchoolDisciplineId]);
GO

-- Creating foreign key on [SectionId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_SectionWeight]
    FOREIGN KEY ([SectionId])
    REFERENCES [dbo].[Sections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_SectionWeight'
CREATE INDEX [IX_FK_SectionWeight]
ON [dbo].[Weights]
    ([SectionId]);
GO

-- Creating foreign key on [HobbieId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_HobbieWeight]
    FOREIGN KEY ([HobbieId])
    REFERENCES [dbo].[Hobbies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_HobbieWeight'
CREATE INDEX [IX_FK_HobbieWeight]
ON [dbo].[Weights]
    ([HobbieId]);
GO

-- Creating foreign key on [CompetitionId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_CompetitionWeight]
    FOREIGN KEY ([CompetitionId])
    REFERENCES [dbo].[Competitions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CompetitionWeight'
CREATE INDEX [IX_FK_CompetitionWeight]
ON [dbo].[Weights]
    ([CompetitionId]);
GO

-- Creating foreign key on [DisciplineId] in table 'Weights'
ALTER TABLE [dbo].[Weights]
ADD CONSTRAINT [FK_DisciplineWeight]
    FOREIGN KEY ([DisciplineId])
    REFERENCES [dbo].[Disciplines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DisciplineWeight'
CREATE INDEX [IX_FK_DisciplineWeight]
ON [dbo].[Weights]
    ([DisciplineId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
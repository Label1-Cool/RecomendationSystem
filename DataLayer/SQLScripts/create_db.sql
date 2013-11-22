PRINT ''
PRINT 'STARTING RECREATION OF DATABASE'
PRINT ''

:On Error exit

PRINT 'CREATING DATABASE:'
IF EXISTS (SELECT 1 FROM SYS.DATABASES WHERE NAME = 'RecomendationSystemDB')
ALTER DATABASE RecomendationSystemDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE
DROP DATABASE RecomendationSystemDB
GO
CREATE DATABASE RecomendationSystemDB
GO
PRINT 'COMPLETE'
PRINT ''

PRINT 'EMPTY TABLES FROM MODEL:'
:r RecomendationSystemModel.edmx.sql
GO

PRINT 'FILLING Users:'
:r SQLScripts\Table(separately)\dbo.Users.data.sql
GO

PRINT 'FILLING Cities:'
:r SQLScripts\Table(separately)\dbo.Cities.data.sql
GO

PRINT 'FILLING Clusters:'
:r SQLScripts\Table(separately)\dbo.Clusters.data.sql
GO

PRINT 'FILLING SchoolTypes:'
:r SQLScripts\Table(separately)\dbo.SchoolTypes.data.sql
GO

PRINT 'FILLING Disciplines:'
:r SQLScripts\Table(separately)\dbo.Disciplines.data.sql
GO

PRINT 'FILLING SchoolDisciplines:'
:r SQLScripts\Table(separately)\dbo.SchoolDisciplines.data.sql
GO

PRINT 'FILLING Schools:'
:r SQLScripts\Table(separately)\dbo.Schools.data.sql
GO

PRINT 'FILLING HighSchools:'
:r SQLScripts\Table(separately)\dbo.HighSchools.data.sql
GO

PRINT 'FILLING UniversityDepartments:'
:r SQLScripts\Table(separately)\dbo.UniversityDepartments.data.sql
GO

PRINT 'FILLING GeneralEducationLines:'
:r SQLScripts\Table(separately)\dbo.GeneralEducationLines.data.sql
GO

PRINT 'FILLING DepartmentEducationLines:'
:r SQLScripts\Table(separately)\dbo.DepartmentEducationLines.data.sql
GO

PRINT 'FILLING DepartmentLinesRequirements:'
:r SQLScripts\Table(separately)\dbo.DepartmentLinesRequirements.data.sql
GO

PRINT 'FILLING SchoolMarks:'
:r SQLScripts\Table(separately)\dbo.SchoolMarks.data.sql
GO

PRINT 'FILLING UnitedStateExams:'
:r SQLScripts\Table(separately)\dbo.UnitedStateExams.data.sql
GO

PRINT 'FILLING Weights:'
:r SQLScripts\Table(separately)\dbo.Weights.data.sql
GO

PRINT ''
PRINT 'DATABASE CREATE IS COMPLETE!'

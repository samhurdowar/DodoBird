/*




<span style='color:red;'>[Name]</span><br>Product Number: [ProductNumber]<br><span style='color:[Color];'>[Color]</span>



<span style='color:red;'>[Name]</span>

                selectColumns = "'<span style=\"color:red;\">' + Name + '</span><br>Product Number: ' + ProductNumber + '<br><span style=\"color:' + ISNULL(Color,'black') + ';\">' + ISNULL(Color,'') + '</span>' AS GridRecord ";

                // add primary key columns
                foreach (var key in tableSchema.PrimaryKeys)
                {
                    selectColumns += "," + key.ColumnName;
                }

Select * from AppDatabase

Select * from AppUser

Select * from Form

update Form set AppDatabaseId = 1, TableName = 'Menu', FormName = 'Manage Menu', PageFile = '~/Views/App/EditMenu.cshtml', TargetType = 'page' where FormId = 1

alter table form add TargetType varchar(50) not null default ''

Select * from FormColumn where FormId = 1

delete  from FormColumn where FormId = 1

Select * from FormSection where FormId = 1

delete  from FormSection where FormId = 1

alter table Grid add Layout varchar(max) not null default ''
alter table Grid add NumOfColumn not null default 0

alter table Grid add GridType int not null default 0


Select * from Menu where ParentId = 1 order by sortorder


update Menu set TargetType = 'form', targetId = 1, PageFile = '' where MenuId = 3

Select * from systypes

Select * from grid

update grid set AppDatabaseId = 3 WHERE GridId > 0 AND GridId < 7

delete from grid where gridid = 5


'image','datetimeoffset','sql_variant','ntext','text','hierarchyid','geometry','geography','varbinary','binary','xml','sysname'


SELECT GridId AS OptionValue, TableName  + ' - ' + GridName AS OptionText FROM Grid ORDER BY TableName, GridName

Select * from Form
SELECT FormId AS OptionValue, TableName  + ' - ' + FormName AS OptionText FROM Form ORDER BY TableName, FormName
*/



                    SELECT c.name AS ColumnName, ISNULL(c.column_id,0) AS ColumnOrder, CAST(ISNULL(c.max_length,0) AS int) AS DataLength,
                    REPLACE(REPLACE(REPLACE(ISNULL(object_definition(c.default_object_id),''), '(', ''), ')', ''), '''', '')   AS DefaultValue,
                    CAST(0 AS Bit) AS IsPrimaryKey,
                    CAST(ISNULL(CASE c.is_identity WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsIdentity,
                    CAST(ISNULL(CASE c.is_nullable WHEN 1 THEN 0 ELSE 1 END, 0) AS Bit) AS IsRequired,
                    CAST(ISNULL(CASE c.is_computed WHEN 1 THEN 1 ELSE 0 END, 0) AS Bit) AS IsComputed,
                    t.name AS DataType
                    FROM sys.columns c 
                    JOIN sys.Types t ON t.system_type_id = c.system_type_id AND t.name <> 'sysname' 
                    JOIN sys.objects o ON o.object_id = c.object_id AND o.type = 'U' AND o.name = 'Menu'
               



SELECT  REPLACE(REPLACE(REPLACE(ISNULL(object_definition(c.default_object_id),''), '(', ''), ')', ''), '''', '')   AS DefaultValue FROM sys.columns c JOIN sys.objects o ON o.object_id = c.object_id WHERE o.type = 'U' 

AND o.name = 'Menu' 


USE DodoBird
GO
IF OBJECT_ID('dbo.AppUser', 'U') IS NOT NULL DROP TABLE dbo.AppUser;
GO
IF OBJECT_ID('dbo.AppDatabase', 'U') IS NOT NULL DROP TABLE dbo.AppDatabase;
GO
IF OBJECT_ID('dbo.Client', 'U') IS NOT NULL DROP TABLE dbo.Client;
GO
IF OBJECT_ID('dbo.Menu', 'U') IS NOT NULL DROP TABLE dbo.Menu;
GO


CREATE TABLE [dbo].[Client](
	[ClientId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ClientName] [varchar](500) NOT NULL default '',
	[DateAdd] datetime NOT NULL default getdate()
)
GO
INSERT INTO Client(ClientName) VALUES('Contoso Retail');
GO

CREATE TABLE [dbo].[AppUser](
	[AppUserId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ClientId] [int] NOT NULL default 0,
	[FirstName] [varchar](250) NOT NULL default '',
	[LastName] [varchar](250) NOT NULL default '',
	[FullName]  AS (([FirstName]+' ')+[LastName]),
	[Email] [varchar](500) NOT NULL default '',
	[PrimaryPhone] [varchar](50) NOT NULL default '',
	[Username] [varchar](250) NOT NULL default '',
	[Password] [varchar](500) NOT NULL default '',
	[DateAdd] [datetime] NOT NULL default getdate()
)

GO
INSERT INTO AppUser(ClientId,FirstName,LastName,Email,Username,Password) VALUES(1,'Bugs','Bunny','Bugs@Warner.com','demo','demo');
GO
CREATE TABLE [dbo].[AppDatabase](
	[AppDatabaseId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ClientId] [int] NOT NULL default 0,
	[DatabaseName] [varchar](500) NOT NULL default '',
	[ConnectionString] [varchar](2000) NOT NULL default ''
)
GO
--INSERT INTO AppDatabase(ClientId, DatabaseName, ConnectionString) VALUES(1,'Dodo Bird Control','data source=localhost;initial catalog=DodoBird;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;');
INSERT INTO AppDatabase(ClientId, DatabaseName, ConnectionString) VALUES(1,'Contoso Retail','data source=localhost\MSSQLSERVER01;initial catalog=ContosoRetail;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;');
GO
INSERT INTO AppDatabase(ClientId, DatabaseName, ConnectionString) VALUES(1,'Adventure Works','data source=localhost\MSSQLSERVER01;initial catalog=AdventureWorks;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;');
GO

CREATE TABLE [dbo].[Menu](
	[MenuId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ClientId] [int] NOT NULL default 0,
	[ParentId] [int] NOT NULL default 0,
	[MenuTitle] [varchar](500) NOT NULL default '',
	[PageFile] [varchar](500) NOT NULL default '',
	[TargetId] [int] NOT NULL default 0,
	[TargetType] [varchar](250) NOT NULL default '',
	[SortOrder] [int] NOT NULL default 0
)
GO
INSERT INTO Menu(ClientId,MenuTitle,SortOrder) VALUES(1,'Administration',1);
GO

INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,1,'Manage Database','~/Views/Home/ManageDatabase.cshtml','page',1);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,1,'Manage Menu','~/Views/Home/ManageMenu.cshtml','page',2);


INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,5,'Account Sub Menu 6','~/Views/Home/Test.cshtml','page',1);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,5,'Account Sub Menu 7','~/Views/Home/Test.cshtml','page',2);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,5,'Account Sub Menu 8','~/Views/Home/Test.cshtml','page',3);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,5,'Account Sub Menu 9','~/Views/Home/Test.cshtml','page',4);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,9,'Sub Menu 9_10','~/Views/Home/Test.cshtml','page',1);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,9,'Sub Menu 9_11','~/Views/Home/Test.cshtml','page',2);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,9,'Sub Menu 9_12','~/Views/Home/Test.cshtml','page',3);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,12,'Sub Menu 12_13','~/Views/Home/Test.cshtml','page',1);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,12,'Sub Menu 12_14','~/Views/Home/Test.cshtml','page',2);


INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,0,'Contacts','~/Views/Home/Test.cshtml','page',3);
INSERT INTO Menu(ClientId,ParentId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,0,'Campaigns','~/Views/Home/Test.cshtml','page',4);




GO

ALTER TABLE AppUser
ADD CONSTRAINT fk_AppUser_Client
    FOREIGN KEY (ClientId)
    REFERENCES Client (ClientId)
    ON DELETE CASCADE;
GO

ALTER TABLE AppDatabase
ADD CONSTRAINT fk_AppDatabase_Client
    FOREIGN KEY (ClientId)
    REFERENCES Client (ClientId)
    ON DELETE CASCADE;
GO



CREATE TABLE [dbo].[Grid](
	[GridId] [int] IDENTITY(1,1) NOT NULL primary key,
	[AppDatabaseId] [int] NOT NULL default 0,
	[TableName] [varchar](500) NOT NULL default '',
	[GridName] [varchar](500) NOT NULL default '',
	[GridFilter] [varchar](2000) NOT NULL default '',
	[GridSort] [varchar](500) NOT NULL default '',
	[DateAdd] datetime NOT NULL default getdate()
)
GO
CREATE TABLE [dbo].[GridColumn](
	[GridColumnId] [int] IDENTITY(1,1) NOT NULL primary key,
	[GridId] [int] NOT NULL default 0,
	[ColumnName] [varchar](500) NOT NULL default '',
	GridOrder int not null default 0,
	[DateAdd] datetime NOT NULL default getdate()
)
GO

ALTER TABLE GridColumn
ADD CONSTRAINT fk_GridColumn_Grid
    FOREIGN KEY (GridId)
    REFERENCES Grid (GridId)
    ON DELETE CASCADE;
GO


drop table [FormColumn]
drop table [Form]


CREATE TABLE [dbo].[Form](
	[FormId] [int] IDENTITY(1,1) NOT NULL primary key,
	[AppDatabaseId] [int] NOT NULL default 0,
	[TableName] [varchar](500) NOT NULL default '',
	[FormName] [varchar](500) NOT NULL default '',
	[DateAdd] datetime NOT NULL default getdate()
)
GO
CREATE TABLE [dbo].[FormColumn](
	[FormColumnId] [int] IDENTITY(1,1) NOT NULL primary key,
	[FormId] [int] NOT NULL default 0,
	[FormSectionId] [int] NOT NULL default 0,
	[SectionColumn] [int] NOT NULL default 0,
	[ColumnName] [varchar](500) NOT NULL default '',
	[ElementType] [varchar](50) NOT NULL default '',
	[DateAdd] datetime NOT NULL default getdate()
)
GO

CREATE TABLE [dbo].[FormSection](
	[FormSectionId] [int] IDENTITY(1,1) NOT NULL primary key,
	[FormId] [int] NOT NULL default 0,
	[ColumnCount] [int] NOT NULL default 0,
	[SectionHeader] [varchar](500) NOT NULL default '',
	[SectionOrder] [int] NOT NULL default 0,
	[DateAdd] datetime NOT NULL default getdate()
)
GO

ALTER TABLE FormSection
ADD CONSTRAINT fk_FormSection_Form
    FOREIGN KEY (FormId)
    REFERENCES Form (FormId)
    ON DELETE CASCADE;
GO
ALTER TABLE FormColumn
ADD CONSTRAINT fk_FormColumn_Form
    FOREIGN KEY (FormId)
    REFERENCES Form (FormId)
    ON DELETE CASCADE;
GO




/*
Add dependent tables and make columns available to grid
Enable images



Add/edit databases
Add/edit tables
Add/edit columns, form elements basic first, textbox, dropdown, checkbox true/false, checkboxes multiple, radios, datetime, picture url

Forms 
	
   Manage form layouts
Go to Form from grid
Grid templates



Add View forms object
Add reports object
Add workflows object

CREATE TABLE DependentTable (
DependentTableId int IDENTITY(1,1) NOT NULL primary key,

AppDatabaseId int NOT NULL default 0,
ParentOwner varchar(250) NOT NULL default '',
ParentTableName varchar(250) NOT NULL default '',
ParentKey varchar(250) NOT NULL default '',
DependentKey varchar(250) NOT NULL default '',
JoinType varchar(250) NOT NULL default '',
Relation varchar(250) NOT NULL default '',
Owner varchar(250) NOT NULL default '',
TableName varchar(250) NOT NULL default ''

)

Production.Product  [ProductID]



[Production].[ProductProductPhoto]   [ProductID]  [ProductPhotoID]

[Production].[ProductPhoto]  [ProductPhotoID]



        public string  { get; set; }
        public string  { get; set; }
        public string  { get; set; }
        public string  { get; set; }
        public string  { get; set; }
        public string  { get; set; }
        public string  { get; set; }


*/
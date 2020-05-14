/**
SELECT * FROM Menu

SELECT * FROM DbEntity

SELECT * FROM PageTemplate
SELECT * FROM ColumnDef

sp_help 'ColumnDef'

SELECT * FROM GridColumn


ALTER TABLE ColumnDef
ADD CONSTRAINT fk_PageTemplate_ColumnDef
    FOREIGN KEY (PageTemplateId)
    REFERENCES PageTemplate (PageTemplateId)
    ON DELETE CASCADE;

GO

ALTER TABLE CHILDTABLE
ADD CONSTRAINT fk_PARENTTABLE_CHILDTABLE
    FOREIGN KEY (PARENTTABLE_ID)
    REFERENCES PARENTTABLE (PARENTTABLE_ID)
    ON DELETE CASCADE;

GO

ALTER TABLE ColumnDef ADD IsIdentity bit not null default 0
ALTER TABLE ColumnDef ADD IsRequired bit not null default 0
ALTER TABLE ColumnDef ADD IsComputed bit not null default 0
ALTER TABLE ColumnDef ADD IsPrimaryKey bit not null default 0
ALTER TABLE ColumnDef ADD DataLength int not null default 0
ALTER TABLE ColumnDef ADD DefaultValue varchar(250) not null default ''
ALTER TABLE ColumnDef ADD DataType varchar(250) not null default ''

UPDATE DbEntity SET ConnectionString = 'data source=localhost;initial catalog=NetworkToolbox;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;' WHERE DbEntityId = 1
GO
UPDATE DbEntity SET ConnectionString = 'data source=localhost;initial catalog=NetworkCafe;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;' WHERE DbEntityId = 2
GO
UPDATE DbEntity SET ConnectionString = 'data source=localhost;initial catalog=f5data;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;' WHERE DbEntityId = 3
GO
UPDATE DbEntity SET ConnectionString = 'data source=localhost;initial catalog=CMDB;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;' WHERE DbEntityId = 6
GO


SELECT * FROM Menu WHERE ParentId = 0 ORDER BY MenuOrder
SELECT * FROM Menu1Item ORDER BY SortOrder
SELECT * FROM Menu2Item ORDER BY Menu1ItemId, SortOrder
SELECT * FROM Menu3Item ORDER BY Menu2ItemId, SortOrder
SELECT * FROM Menu WHERE ParentId = 0

SELECT * FROM AppDatabase
SELECT * FROM AppTable WHERE AppDatabaseId = 4
SELECT * FROM AppColumn
SELECT * FROM Grid WHERE AppTableId = 112

SELECT * FROM GridColumn JOIN AppColumn ON GridColumn.AppColumnId = AppColumn.AppColumnId WHERE GridId = 112 AND IsDisplayed = 1 ORDER BY SortOrder

SELECT * FROM PageTemplate
SELECT * FROM ColumnDef


SELECT (SELECT 'INSERT INTO dbo.AppColumn(' + STUFF((SELECT ', ' + ColumnName FROM (SELECT c.Name AS ColumnName FROM sys.columns c JOIN sys.objects o ON o.object_id = c.object_id 
WHERE o.name = 'AppColumn' AND c.is_computed = 0 )  AS T FoR XML PATH('')), 1, 1, '') + ') VALUES' AS ColumnNames)



UPDATE GridColumn SET IsDisplayed = 1 WHERE GridColumnId IN (SELECT TOP 8 GridColumnId FROM GridColumn WHERE GridId = 2 ORDER BY SortOrder)


SELECT * FROM ViewAppTable
**/




/****** Tables ******/
IF OBJECT_ID('dbo.GridColumn', 'U') IS NOT NULL DROP TABLE dbo.GridColumn;
GO
IF OBJECT_ID('dbo.Grid', 'U') IS NOT NULL DROP TABLE dbo.Grid;
GO
IF OBJECT_ID('dbo.AppColumn', 'U') IS NOT NULL DROP TABLE dbo.AppColumn;
GO
IF OBJECT_ID('dbo.AppTable', 'U') IS NOT NULL DROP TABLE dbo.AppTable;
GO
IF OBJECT_ID('dbo.AppDatabase', 'U') IS NOT NULL DROP TABLE dbo.AppDatabase;
GO


IF OBJECT_ID('dbo.Menu3Item', 'U') IS NOT NULL DROP TABLE dbo.Menu3Item;
GO
IF OBJECT_ID('dbo.Menu2Item', 'U') IS NOT NULL DROP TABLE dbo.Menu2Item;
GO
IF OBJECT_ID('dbo.Menu1Item', 'U') IS NOT NULL DROP TABLE dbo.Menu1Item;
GO
IF OBJECT_ID('GetDisplayNameById') IS NOT NULL DROP FUNCTION GetDisplayNameById
GO
/****** Views ******/
IF OBJECT_ID('dbo.ViewAppTable', 'V') IS NOT NULL DROP VIEW dbo.ViewAppTable;
GO

/****** Stored Procedure ******/

IF OBJECT_ID('SortGridColumn') IS NOT NULL DROP PROCEDURE SortGridColumn
GO


CREATE TABLE AppDatabase (
	AppDatabaseId  int primary key identity(1,1),
	DatabaseName  varchar(500) not null default '',
	ConnectionString  varchar(1000) not null default ''
)
GO

INSERT INTO AppDatabase(DatabaseName,ConnectionString) VALUES
('Source Control','data source=localhost;initial catalog=NetworkToolbox;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;'),
('NetworkCafe','data source=localhost;initial catalog=NetworkCafe;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;'),
('F5 Data','data source=localhost;initial catalog=f5data;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;'),
('CMDB','data source=localhost;initial catalog=CMDB;persist security info=True;Trusted_Connection=Yes;MultipleActiveResultSets=True;')

GO


CREATE TABLE AppTable (
	AppTableId  int primary key identity(1,1),
	AppDatabaseId  int not null  default 0,
	TableName  varchar(500) not null default '',
	PrimaryKey  varchar(500) not null default '',
	PrimaryKeyType  varchar(500) not null default '',
	SystemTypeId int NOT NULL default 0
)
GO

ALTER TABLE AppTable
ADD CONSTRAINT fk_AppDatabase_AppTable
    FOREIGN KEY (AppDatabaseId)
    REFERENCES AppDatabase (AppDatabaseId)
    ON DELETE CASCADE;

GO

--INSERT INTO AppTable(AppDatabaseId,TableName,PrimaryKey) SELECT DISTINCT DbEntityId, TableName, PrimaryKey FROM PageTemplate WHERE DbEntityId <> 6
--GO

CREATE TABLE AppColumn (
	AppColumnId int IDENTITY(1,1) NOT NULL primary key,
	AppTableId int NOT NULL default 0,
	ColumnName varchar(100) NULL,
	DisplayName varchar(300) NULL,
	ElementType varchar(50) NULL,
	ElementWidth int NOT NULL default 0,
	ElementHeight int NOT NULL default 0,

	ColumnOrder int NOT NULL default 0,
	IsIdentity bit NOT NULL default 0,
	IsRequired bit NOT NULL default 0,
	IsComputed bit NOT NULL default 0,
	IsPrimaryKey bit NOT NULL default 0,
	DataLength int NOT NULL default 0,
	DefaultValue varchar(250) NULL,
	DataType varchar(250) NULL,
	SystemTypeId int NOT NULL default 0
)

GO

ALTER TABLE AppColumn
ADD CONSTRAINT fk_AppTable_AppColumn
    FOREIGN KEY (AppTableId)
    REFERENCES AppTable (AppTableId)
    ON DELETE CASCADE;

GO

--INSERT INTO dbo.AppColumn( AppTableId, ColumnName, DisplayName, ElementType, ElementWidth, ElementHeight, ColumnOrder, IsIdentity, IsRequired, IsComputed, IsPrimaryKey, DataLength, DefaultValue, DataType) 
--SELECT T2.AppTableId,ColumnName, DisplayName, ElementType, ElementWidth, ElementHeight, ColumnOrder, IsIdentity, IsRequired, IsComputed, IsPrimaryKey, DataLength, DefaultValue, DataType 
--FROM ColumnDef T1 
--JOIN 
--(SELECT AppTableId, b.* FROM AppTable a JOIN PageTemplate b ON a.TableName = b.TableName) T2 ON T1.PageTemplateId = T2.PageTemplateId 

GO
CREATE VIEW ViewAppTable AS SELECT * FROM AppTable 
GO

CREATE TABLE Menu1Item (
	Menu1ItemId  int primary key identity(1,1),
	MenuTitle  varchar(500) not null default '',
	PageFile   varchar(500) null,
	TsScript   varchar(500) null,
	GridId  int not null default 0,
	FormId  int not null default 0,
	SortOrder  int not null  default 0
)

GO

CREATE TABLE Menu2Item (
	Menu2ItemId  int primary key identity(1,1),
	Menu1ItemId  int not null default 0,
	MenuTitle  varchar(500) not null default '',
	PageFile   varchar(500) null,
	TsScript   varchar(500) null,
	GridId  int not null default 0,
	FormId  int not null default 0,
	SortOrder  int not null  default 0,
	OldParentId  int null
)

GO
CREATE TABLE Menu3Item (
	Menu3ItemId  int primary key identity(1,1),
	Menu2ItemId  int not null default 0,
	MenuTitle  varchar(500) not null default '',
	PageFile   varchar(500) null,
	TsScript   varchar(500) null,
	GridId  int not null default 0,
	FormId  int not null default 0,
	SortOrder  int not null  default 0
)

GO

ALTER TABLE Menu2Item
ADD CONSTRAINT fk_Menu1Item_Menu2Item
    FOREIGN KEY (Menu1ItemId)
    REFERENCES Menu1Item (Menu1ItemId)
    ON DELETE CASCADE;
GO


ALTER TABLE Menu3Item
ADD CONSTRAINT fk_Menu2Item_Menu3Item
    FOREIGN KEY (Menu2ItemId)
    REFERENCES Menu2Item (Menu2ItemId)
    ON DELETE CASCADE;
GO



INSERT INTO Menu1Item(MenuTitle,PageFile,SortOrder) SELECT MenuTitle,PageFile,MenuOrder FROM Menu WHERE ParentId = 0 ORDER BY MenuOrder
GO

INSERT INTO Menu2Item(Menu1ItemId,MenuTitle,PageFile,SortOrder,OldParentId) SELECT 1, MenuTitle,PageFile,MenuOrder,ParentId FROM Menu WHERE ParentId IN (SELECT MenuId FROM Menu WHERE ParentId = 0)
GO


UPDATE T1 SET T1.Menu1ItemId = T2.New_Id FROM Menu2Item T1
JOIN (SELECT a.Menu1ItemId AS New_Id, b.MenuId AS Old_Id FROM Menu1Item a JOIN Menu b ON a.MenuTitle = b.MenuTitle AND b.ParentId = 0) T2 ON T1.OldParentId = T2.Old_Id
GO
INSERT INTO Menu3Item(Menu2ItemId,MenuTitle,PageFile,SortOrder) SELECT (SELECT TOP 1 Menu2ItemId FROM Menu2Item WHERE MenuTitle = 'Admin Tools'), MenuTitle,PageFile,MenuOrder FROM Menu WHERE ParentId IN (SELECT MenuId FROM Menu WHERE MenuTitle = 'Admin Tools')
GO

ALTER TABLE Menu2Item DROP COLUMN OldParentId
GO



/*      Grid   */

CREATE TABLE Grid (
	GridId  int primary key identity(1,1),
	AppTableId  int not null,
	GridName  varchar(500) not null default ''
)

GO

ALTER TABLE Grid
ADD CONSTRAINT fk_AppTable_Grid
    FOREIGN KEY (AppTableId)
    REFERENCES AppTable (AppTableId)
    ON DELETE CASCADE;
GO

--INSERT INTO Grid(AppTableId,GridName) SELECT AppTableId, 'Main for ' + TableName FROM AppTable
GO

CREATE TABLE GridColumn (
	GridColumnId  int primary key identity(1,1),
	GridId  int not null default 0,
	AppColumnId  int not null,
	IsDisplayed  bit not null default 0,
	SortOrder  int not null
)

GO
ALTER TABLE GridColumn
ADD CONSTRAINT fk_Grid_GridColumn
    FOREIGN KEY (GridId)
    REFERENCES Grid (GridId)
    ON DELETE CASCADE;
GO



ALTER TABLE GridColumn
ADD CONSTRAINT fk_GridColumn_AppColumn
    FOREIGN KEY (AppColumnId)
    REFERENCES AppColumn (AppColumnId);
GO

--INSERT INTO GridColumn(GridId, AppColumnId, SortOrder) SELECT GridId, AppColumnId, 1 FROM AppColumn a JOIN Grid b ON a.AppTableId = b.AppTableId 

GO
DELETE FROM ColumnDef WHERE PageTemplateId IN (SELECT PageTemplateId FROM PageTemplate WHERE TemplateName = 'Abstract Table')
GO
DELETE FROM PageTemplate WHERE TemplateName = 'Abstract Table'
GO
DELETE FROM ColumnDef WHERE PageTemplateId NOT IN (SELECT PageTemplateId FROM PageTemplate)
GO

UPDATE Menu2Item SET TsScript = '', PageFile = '~/Views/Home/ManageDatabase.cshtml' WHERE MenuTitle = 'Manage Page Templates'
GO
UPDATE Menu2Item SET TsScript = '', PageFile = '~/Views/Home/ManageMenu.cshtml' WHERE MenuTitle = 'Manage Menu'
GO
UPDATE Menu1Item SET GridId = 83 WHERE Menu1ItemId = 1
GO


-- SortGridColumn(int gridId, int gridColumnId, int isDisplayed, int newOrder)
/****** Object:  StoredProcedure [dbo].[SortLayoutSection]    Script Date: 4/26/2020 12:21:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SortGridColumn]
@gridId int, 
@gridColumnId int, 
@isDisplayed int, 
@newOrder int
AS 
SET NOCOUNT ON;
DECLARE @SequenceOrder int = 0 
DECLARE @OldIsDisplayed int = 0 
DECLARE @OldOrder int = 0 
DECLARE @RecCount int = 0
DECLARE @RecMin int = 0
DECLARE @RecMax int = 0

-- get OldOrder
SELECT @OldIsDisplayed = IsDisplayed, @OldOrder = SortOrder FROM GridColumn WHERE GridColumnId = @gridColumnId

-- update IsDisplayed regardless
UPDATE GridColumn SET IsDisplayed = @isDisplayed WHERE GridColumnId = @gridColumnId


-- Reorder if out of sync
SELECT @RecCount = COUNT(1), @RecMin = MIN(SortOrder), @RecMax = MAX(SortOrder) FROM GridColumn WHERE GridId = @gridId AND IsDisplayed = 1 
IF (@RecMin <> 1) OR (@RecMax <> @RecCount) OR (@OldIsDisplayed <> @isDisplayed)   
BEGIN
	SET @SequenceOrder = 0
	DECLARE SeqCursor CURSOR LOCAL FOR
	SELECT GridColumnId FROM GridColumn WHERE GridId = @gridId AND IsDisplayed = 1 ORDER BY SortOrder
	OPEN SeqCursor
	FETCH SeqCursor
	WHILE (@@FETCH_STATUS = 0)
		BEGIN
			SET @SequenceOrder = @SequenceOrder + 1
			UPDATE GridColumn SET SortOrder = @SequenceOrder WHERE CURRENT OF SeqCursor
			FETCH NEXT FROM SeqCursor
		END
END


-- stop if IsDisplayed = 0 
IF @isDisplayed = 0 RETURN


UPDATE GridColumn SET SortOrder = @newOrder WHERE GridColumnId = @gridColumnId

IF (@OldOrder > @newOrder) OR (@OldIsDisplayed = 0 AND @isDisplayed = 1)
    BEGIN
		SET @SequenceOrder = @newOrder
		DECLARE SeqCursor1 CURSOR LOCAL FOR
		SELECT GridColumnId FROM GridColumn WHERE GridId = @gridId AND IsDisplayed = 1 AND SortOrder >= @newOrder AND GridColumnId <> @gridColumnId ORDER BY SortOrder
		OPEN SeqCursor1
		FETCH SeqCursor1
		WHILE (@@FETCH_STATUS = 0)
			BEGIN
				SET @SequenceOrder = @SequenceOrder + 1
				UPDATE GridColumn SET SortOrder = @SequenceOrder WHERE CURRENT OF SeqCursor1
				FETCH NEXT FROM SeqCursor1
			END
    END
ELSE IF @OldOrder < @newOrder
    BEGIN
		SET @SequenceOrder = 0
		DECLARE SeqCursor1 CURSOR LOCAL FOR
		SELECT GridColumnId FROM GridColumn WHERE GridId = @gridId AND IsDisplayed = 1 AND SortOrder <= @newOrder AND GridColumnId <> @gridColumnId ORDER BY SortOrder
		OPEN SeqCursor1
		FETCH SeqCursor1
		WHILE (@@FETCH_STATUS = 0)
			BEGIN
				SET @SequenceOrder = @SequenceOrder + 1
				UPDATE GridColumn SET SortOrder = @SequenceOrder WHERE CURRENT OF SeqCursor1
				FETCH NEXT FROM SeqCursor1
			END
    END





/*

SELECT * FROM AppTable
SELECT * FROM AppColumn

SELECT * FROM GridColumn WHERE GridId = 13
SELECT * FROM PageTemplate

SELECT * FROM Grid
SELECT * FROM Menu1Item
SELECT * FROM Menu2Item
SELECT * FROM Menu3Item



UPDATE Menu2Item SET TsScript = 'ManageDatabase();', PageFile = '' WHERE MenuTitle = 'Manage Page Templates'


SELECT * FROM Menu


*/
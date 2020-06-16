/*
SELECT * FROM Menu1Item
SELECT * FROM Menu2Item
SELECT * FROM AppDatabase
*/


USE DodoBird
GO
IF OBJECT_ID('dbo.AppUser', 'U') IS NOT NULL DROP TABLE dbo.AppUser;
GO
IF OBJECT_ID('dbo.AppDatabase', 'U') IS NOT NULL DROP TABLE dbo.AppDatabase;
GO
IF OBJECT_ID('dbo.Client', 'U') IS NOT NULL DROP TABLE dbo.Client;
GO
IF OBJECT_ID('dbo.Menu3Item', 'U') IS NOT NULL DROP TABLE dbo.Menu3Item;
GO
IF OBJECT_ID('dbo.Menu2Item', 'U') IS NOT NULL DROP TABLE dbo.Menu2Item;
GO
IF OBJECT_ID('dbo.Menu1Item', 'U') IS NOT NULL DROP TABLE dbo.Menu1Item;
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

CREATE TABLE [dbo].[Menu1Item](
	[Menu1ItemId] [int] IDENTITY(1,1) NOT NULL primary key,
	[ClientId] [int] NOT NULL default 0,
	[MenuTitle] [varchar](500) NOT NULL default '',
	[PageFile] [varchar](500) NOT NULL default '',
	[TargetId] [int] NOT NULL default 0,
	[TargetType] [varchar](250) NOT NULL default '',
	[SortOrder] [int] NOT NULL default 0
)
GO
INSERT INTO Menu1Item(ClientId,MenuTitle,SortOrder) VALUES(1,'Administration',1);
GO
CREATE TABLE [dbo].[Menu2Item](
	[Menu2ItemId] [int] IDENTITY(1,1) NOT NULL primary key,
	[Menu1ItemId] [int] NOT NULL default 0,
	[MenuTitle] [varchar](500) NOT NULL default '',
	[PageFile] [varchar](500) NOT NULL default '',
	[TargetId] [int] NOT NULL default 0,
	[TargetType] [varchar](250) NOT NULL default '',
	[SortOrder] [int] NOT NULL
)
GO
INSERT INTO Menu2Item(Menu1ItemId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,'Manage Database','~/Views/Home/ManageDatabase.cshtml','page',1);
INSERT INTO Menu2Item(Menu1ItemId,MenuTitle,PageFile,TargetType,SortOrder) VALUES(1,'Manage Menu','~/Views/Home/ManageMenu.cshtml','page',2);
GO
CREATE TABLE [dbo].[Menu3Item](
	[Menu3ItemId] [int] IDENTITY(1,1) NOT NULL primary key,
	[Menu2ItemId] [int] NOT NULL default 0,
	[MenuTitle] [varchar](500) NOT NULL default '',
	[PageFile] [varchar](500) NOT NULL default '',
	[TargetId] [int] NOT NULL default 0,
	[TargetType] [varchar](250) NOT NULL default '',
	[SortOrder] [int] NOT NULL
)
GO

ALTER TABLE Menu3Item
ADD CONSTRAINT fk_Menu3Item_Menu2Item
    FOREIGN KEY (Menu2ItemId)
    REFERENCES Menu2Item (Menu2ItemId)
    ON DELETE CASCADE;
GO

ALTER TABLE Menu2Item
ADD CONSTRAINT fk_Menu2Item_Menu1Item
    FOREIGN KEY (Menu1ItemId)
    REFERENCES Menu1Item (Menu1ItemId)
    ON DELETE CASCADE;
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

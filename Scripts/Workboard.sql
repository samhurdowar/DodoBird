--SELECT * FROM DbEntity
--INSERT INTO DbEntity(EntityName,DisplayName,ConnectionString) VALUES ('CmdbEntities','CMDB Database','')

/*
USE NetworkToolbox

TRUNCATE TABLE DebugLog

SELECT * FROM DebugLog

RoutePage()    pageTemplateId=0  pageFile=_PageTemplate  recordId=  refKeyColumnName=  refKeyValue=

SELECT * FROM DbEntity
SELECT * FROM PageTemplate
SELECT * FROM ColumnDef WHERE PageTemplateId = 2125 AND ColumnDefId = 3201

sp_columns 'ColumnDef'

USE NetworkToolbox
GO
DELETE FROM ColumnDef WHERE PageTemplateId IN (SELECT PageTemplateId FROM PageTemplate WHERE PageType = 'abstract')
GO
DELETE FROM PageTemplate WHERE PageType = 'abstract'
GO
ALTER TABLE ColumnDef DROP COLUMN TableName
GO
ALTER TABLE ColumnDef DROP COLUMN PrimaryKey
GO
ALTER TABLE ColumnDef DROP CONSTRAINT DF_ColumnDef_IsRequired
GO
ALTER TABLE ColumnDef DROP COLUMN IsRequired
GO
ALTER TABLE ColumnDef DROP COLUMN DataType
GO
ALTER TABLE ColumnDef DROP COLUMN DataLength
GO
ALTER TABLE ColumnDef DROP COLUMN IsPrimaryKey
GO
ALTER TABLE ColumnDef DROP CONSTRAINT DF_ColumnDef_IsComputed
GO
ALTER TABLE ColumnDef DROP COLUMN IsComputed
GO
ALTER TABLE ColumnDef DROP CONSTRAINT DF_ColumnDef_DefaultValue
GO
ALTER TABLE ColumnDef DROP COLUMN DefaultValue
GO
ALTER TABLE ColumnDef DROP CONSTRAINT DF_ColumnDef_ColumnOrder
GO
ALTER TABLE ColumnDef DROP COLUMN ColumnOrder
GO


ALTER TABLE PageTemplate ADD ViewTabLabel varchar(250) null

ALTER TABLE PageTemplate ADD SearchLayout varchar(max) null

GO
PageTemplate

ALTER TABLE PageTemplate DROP COLUMN LayoutView 
GO
ALTER TABLE PageTemplate ADD ViewLayout varchar(max) null
*/




USE CMDB
GO
ALTER TABLE Device ADD RMA   varchar(250) null
ALTER TABLE Device ADD DOMSSerial   varchar(250) null
ALTER TABLE Device ADD DOMSRack   varchar(250) null
ALTER TABLE Device ADD DOMSRU   varchar(250) null
ALTER TABLE Device ADD SupportStart   date null
ALTER TABLE Device ADD SupportEnd   date null
ALTER TABLE Device ADD SupportContract   varchar(250) null
ALTER TABLE Device ADD EOLDate   date null

ALTER TABLE DeviceModel ADD FrontPicture   text null
ALTER TABLE DeviceModel ADD RearPicture   text null

declare @SQL nvarchar(max)

set @SQL = 
  (
  select 'DROP TABLE  [' + SCHEMA_NAME(schema_id) + '].[' + name +'];'
  from sys.tables --where SCHEMA_NAME(schema_id) not like '%dbo%'
  for xml path('')
  )

exec (@SQL)



CREATE TABLE [dbo].[CustomOption](
	[CustomOptionId] [int] IDENTITY(1,1) NOT NULL,
	[ColumnDefId] [int] NOT NULL,
	[OptionText] [varchar](500) NULL,
	[OptionValue] [varchar](500) NULL,
 CONSTRAINT [PK__CustomOp__A3DD8EF26C77B12A] PRIMARY KEY CLUSTERED 
(
	[CustomOptionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CustomOption] ADD  CONSTRAINT [DF__CustomOpt__Colum__7C6F7215]  DEFAULT ((0)) FOR [ColumnDefId]
GO

ALTER TABLE [dbo].[CustomOption] ADD  CONSTRAINT [DF_CustomOption_OptionValue]  DEFAULT ((0)) FOR [OptionValue]
GO



GO
CREATE TABLE dbo.Device (
	DeviceId       int primary key identity(1,1),
	HostName       varchar(250) null,
	MfgId          int not null default 0,
	Serial         varchar(250) null,
	DeviceSiteId   int not null default 0,
	PoInfoId       int not null default 0,
	DeviceGroups    varchar(500) null,
	MgtIp          varchar(250) null,
	DeviceModelId   int not null default 0,
	DeviceTypeId    int not null default 0,
	RackId         int not null default 0,
	DoB            date null,
	IsVirtual       bit not null default 0,
	Wlp             varchar(250) null,
	DeviceStatusId   int not null default 0,
	RuNumber         int null,
	DoD              date null,
	FismaTag          varchar(500) null,
	ProvisionNnmi      bit not null default 0,
	ProvisionNetworkAutomation   bit not null default 0,
	ProvisionSolarWind           bit not null default 0,
	ProvisionAlgosecFiremon      bit not null default 0,
	ProvisionNetBrain            bit not null default 0,
	DeviceNote                   varchar(500) null,
	RMA   varchar(250) null
)
GO

CREATE TABLE dbo.Mfg (
	MfgId           int primary key identity(1,1),
	MfgName         varchar(250) null
)
GO
INSERT INTO Mfg(MfgName) VALUES 
('Cisco'),
('Juniper'),
('Trend Micro'),
('Citrix'),
('Check Point'),
('Raritan'),
('F5'),
('Gigamon'),
('Viavi'),
('NetOptics'),
('Bluecoat'),
('Safenet'),
('Plixer'),
('Infoblox')


GO
CREATE TABLE dbo.DeviceStatus (
	DeviceStatusId           int primary key identity(1,1),
	Status                 varchar(250) null
)
GO
INSERT INTO DeviceStatus(Status) VALUES
('In Production'),
('Decomissioned'),
('Pre-Production'),
('Lab'),
('RMA''d'),
('Storage')

GO
CREATE TABLE dbo.DeviceType (
	DeviceTypeId           int primary key identity(1,1),
	TypeDesc                 varchar(250) null
)
GO
INSERT INTO DeviceType(TypeDesc) VALUES
('Router'),
('Switch'),
('Firewall'),
('Packet Capture'),
('Packet Broker'),
('DNS'),
('Tools'),
('Tap'),
('HSM')

GO  
CREATE TABLE dbo.DeviceSite (
	DeviceSiteId           int primary key identity(1,1),
	SiteCode               varchar(100) null,
	SiteName               varchar(250) null,
	SiteAddress            varchar(250) null,
	City            varchar(250) null,
	State            varchar(50) null,
	Zip            varchar(50) null,
	SupportPhone            varchar(50) null,
	SupportEmail            varchar(500) null,
	SiteNote   varchar(max) null
)
GO
INSERT INTO DeviceSite(SiteCode,SiteName,SiteAddress,City,State,Zip,SupportPhone,SupportEmail) VALUES
('CDC','Cherokee Data Center','7400 N Lakewood Ave','Tulsa','OK','74117','1.918.600.8780','cdcsitesupport2@dxc.com'),
('CXC','Colorado Springs','311 S. Rockrimmon Blvd','Colorado Springs','CO','80919','1-719-323-3528','dc-facilities-colorado@dxc.com')
GO


CREATE TABLE dbo.PoInfo (
	PoInfoId           int primary key identity(1,1),
	PoNumber               varchar(250) null,
	Entitlement            varchar(250) null,
	PoNote   varchar(max) null
)
GO
INSERT INTO PoInfo(PoNumber,Entitlement) VALUES
('PoNumber One','Entitlement One'),
('PoNumber Two','Entitlement Two')
GO

CREATE TABLE dbo.DeviceGroup (
	DeviceGroupId       int primary key identity(1,1),
	GroupName               varchar(250) null,
	GroupType               varchar(250) null,
	GroupAttribute               varchar(500) null
)

GO

INSERT INTO DeviceGroup(GroupName,GroupType,GroupAttribute) VALUES
('GroupName One','GroupType One','GroupAttribute One'),
('GroupName Two','GroupType Two','GroupAttribute Two'),
('GroupName Three','GroupType Three','GroupAttribute Three')

GO

CREATE TABLE dbo.DeviceGroupAssign (
	DeviceGroupAssignId       int primary key identity(1,1),
	DeviceId          int not null default 0,
	DeviceGroupId         int not null default 0
)

GO

CREATE TABLE dbo.DeviceModel (
	DeviceModelId       int primary key identity(1,1),
	MfgName               varchar(250) null,
	Model               varchar(250) null,
	EolDate    datetime null
)

GO

INSERT INTO DeviceModel(MfgName,Model,EolDate) VALUES
('MfgName One','Model One','4/8/2020'),
('MfgName Two','Model Two','4/8/2020'),
('MfgName Three','Model Three','4/8/2020')

GO

CREATE TABLE dbo.Rack (
	RackId       int primary key identity(1,1),
	DeviceSiteId        int not null default 0,
	Tile               varchar(250) null,
	Ru               varchar(250) null,
	RackType               varchar(250) null
)

GO
INSERT INTO Rack(DeviceSiteId,Tile,Ru,RackType) VALUES
(1,'Tile One','Ru One','RackType One'),
(2,'Tile Two','Ru Two','RackType Two')
GO

--CREATE function dbo.GetDeviceSiteName(@Id int) returns varchar(250)
--AS
--BEGIN
--  DECLARE @ret varchar(250) = ISNULL((SELECT SiteName FROM DeviceSite  WHERE DeviceSiteId = @Id), '')

--  return @ret
--END

--GO
   
--ALTER TABLE Rack ADD SiteName AS dbo.GetDeviceSiteName(DeviceSiteId)
--GO











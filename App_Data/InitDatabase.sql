IF OBJECT_ID('dbo._CustomOption', 'U') IS NULL 
CREATE TABLE dbo._CustomOption (
CustomOptionId int not null primary key identity(1,1), 
AppDatabaseId int not null default 0, 
OptionName varchar(500) not null default ''
);

INTERNAL_GO


IF OBJECT_ID('dbo._CustomOptionItem', 'U') IS NULL 
CREATE TABLE dbo._CustomOptionItem (
CustomOptionItemId int not null primary key identity(1,1), 
CustomOptionId int not null default 0, 
OptionValue varchar(500) not null default '',
OptionText varchar(2000) not null default '',
OptionOrder int not null default 0
);



INTERNAL_GO



IF 'fk_CustomOption_CustomOptionItem' IS NULL 
ALTER TABLE _CustomOptionItem
ADD CONSTRAINT fk_CustomOption_CustomOptionItem
    FOREIGN KEY (CustomOptionId)
    REFERENCES _CustomOption (CustomOptionId)
    ON DELETE CASCADE;

INTERNAL_GO

	CREATE PROCEDURE [dbo].[SortCustomOptionItem]
	@customOptionId int, 
	@formSectionId int,
	@newOrder int
	AS 
	SET NOCOUNT ON;
	DECLARE @SequenceOrder int = 0 
	DECLARE @RecCount int = 0
	DECLARE @RecMin int = 0
	DECLARE @RecMax int = 0
	DECLARE @OldOrder int = 0


	-- Reorder if out of sync
	SELECT @RecCount = COUNT(1), @RecMin = MIN(SectionOrder), @RecMax = MAX(SectionOrder) FROM FormSection WHERE FormId = @formId GROUP BY FormId
	IF (@RecMin <> 1) OR (@RecMax <> @RecCount)  
	BEGIN
		SET @SequenceOrder = 0
		UPDATE FormSection SET SectionOrder = @SequenceOrder, @SequenceOrder = @SequenceOrder + 1 
		WHERE FormSectionId IN (SELECT TOP 100000 FormSectionId FROM FormSection WHERE FormId = @formId ORDER BY SectionOrder)
	END


	BEGIN
		SELECT @OldOrder = SectionOrder FROM FormSection WHERE FormId = @formId AND FormSectionId = @formSectionId
		UPDATE FormSection SET SectionOrder = @newOrder WHERE FormId = @formId AND FormSectionId = @formSectionId

		IF @OldOrder > @newOrder 
			BEGIN
			
				SET @SequenceOrder = @newOrder
				UPDATE FormSection SET SectionOrder = @SequenceOrder, @SequenceOrder = @SequenceOrder + 1 
				WHERE FormSectionId IN 
				(SELECT TOP 100000 FormSectionId FROM FormSection WHERE SectionOrder >= @newOrder AND FormId = @formId AND FormSectionId <> @formSectionId ORDER BY SectionOrder)
			END
		ELSE IF @OldOrder < @newOrder
			BEGIN
				SET @SequenceOrder = 0
				UPDATE FormSection SET SectionOrder = @SequenceOrder, @SequenceOrder = @SequenceOrder + 1 
				WHERE FormSectionId IN 
				(SELECT TOP 100000 FormSectionId FROM FormSection WHERE SectionOrder <= @newOrder AND FormId = @formId AND FormSectionId <> @formSectionId ORDER BY SectionOrder)
			END
	END
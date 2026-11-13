/*
  Our Legacy page

  CMS page name: our-legacy
  Components (component_sequence):
    1 = Founding Family  (Title, Block section Title, Content, Thumbnail Image, Year, Sequence)
    2 = Milestones       (Title, Intro, Year, Thumbnail Image, Sequence)

  Called from AboutUs_DAL.GetLegacyPageData_DAL / AboutUs_BAL.GetLegacyPage_BAL
*/

CREATE OR ALTER PROCEDURE dbo.sp_GetLegacyPageData
    @pagename    NVARCHAR(250) = NULL,
    @LanguageID  INT = NULL,
    @GeographyID INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.sp_GetContentComponentData
        @pagename    = @pagename,
        @LanguageID  = @LanguageID,
        @GeographyID = @GeographyID;
END
GO

-- Smoke test
-- EXEC dbo.sp_GetLegacyPageData @pagename = N'our-legacy', @LanguageID = 1, @GeographyID = 1;

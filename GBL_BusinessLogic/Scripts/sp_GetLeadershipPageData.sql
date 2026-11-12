/*
  Leadership / Board of Directors page

  CMS page name: leadership
  Components (component_sequence):
    1 = Board of Directors (featured chair)
        Title, Block section Title, Intro, Content, Thumbnail Image, popup content, Sequence
    2 = BOD leadership list (grid cards)
        Title, Designation, Thumbnail Image, thumbnail image alt, popup content, Sequence

  Called from AboutUs_DAL.GetLeadershipPageData_DAL / AboutUs_BAL.GetLeadershipPage_BAL
  Route: /about-us/leadership
*/

CREATE OR ALTER PROCEDURE dbo.sp_GetLeadershipPageData
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
-- EXEC dbo.sp_GetLeadershipPageData @pagename = N'leadership', @LanguageID = 1, @GeographyID = 1;

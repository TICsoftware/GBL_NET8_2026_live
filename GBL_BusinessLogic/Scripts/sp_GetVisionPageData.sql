/*
  Our Vision & Mission page

  CMS page name: our-vision
  Route: /about-us/our-vision

  Components (component_sequence):
    1 = Our Guiding Principles
        IsBlock 0: Component Title / Display title, Intro, Component Background image
        IsBlock 1: Title, Intro/Content, Thumbnail Image (icon), Sequence
    2 = Our Mission
        IsBlock 0: Title (Our Mission), Component Background image
        IsBlock 1: Title (each mission line), Sequence

  Page Cont_intro = vision mantra two-column HTML
    (page-intro-mantra__quote + page-intro-mantra__copy)

  Run this, then check result set 2 for component_sequence / is_block / field_name.
*/

CREATE OR ALTER PROCEDURE dbo.sp_GetVisionPageData
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

-- Smoke test — check component_sequence 1 = Guiding Principles, 2 = Mission
EXEC dbo.sp_GetVisionPageData
    @pagename    = N'our-vision',
    @LanguageID  = 1,
    @GeographyID = 1;
GO

/*
  Our Vision & Mission page

  CMS page name: our-vision
  Route: /about-us/our-vision

  Page details (result set 1):
    cont_title          = Content title          → banner H1 "Our Vision"
    cont_hmpg_title     = Landing page Title     → Sanskrit (माता भूमिः…)
    cont_hmpg_intro     = Landing intro          → LHS quote block
    cont_intro          = page intro             → RHS copy column
    BreadcrumPath / masthead_image_path          → banner

  Components (result set 2 — component_sequence):
    1 = Our Guiding Principles
        IsBlock 0: Component Title, Component Intro, Component Background image
        IsBlock 1: Title, Intro, Thumbnail Image (+ alt), Sequence
    2 = Our Mission
        IsBlock 0: Title / Component Title, Component Background image
        IsBlock 1: Title only (mission list lines), Sequence

  Run smoke test below and verify sequences 1 and 2.
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

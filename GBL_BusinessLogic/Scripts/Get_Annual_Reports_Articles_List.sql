/*
  Annual Reports listing / load-more
  Same article shape as Get_Investor_Pdf_Articles_List (subsidiaries):
    thumbnail = content.Hmpg_thumbnail_Media_id -> media.file_path
    PDF       = content.Attach_file_Media_id    -> media.file_path

  CMS page name: annual-reports
  URL: /investor-relations/annual-reports

  App currently reuses Get_Investor_Pdf_Articles_List.
  Run this only if you want a dedicated SP; then point DAL at this name.
*/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Get_Annual_Reports_Articles_List
(
    @ContentId  INT,
    @PageSize   INT = 8,
    @PageNumber INT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    EXEC dbo.Get_Investor_Pdf_Articles_List
        @ContentId  = @ContentId,
        @PageSize   = @PageSize,
        @PageNumber = @PageNumber;
END
GO

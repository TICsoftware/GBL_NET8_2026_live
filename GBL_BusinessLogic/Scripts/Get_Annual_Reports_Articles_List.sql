/*
  Annual Reports listing / load-more
  Ordered by cont_displaydate DESC (newest first).

  Thumbnail = content.Hmpg_thumbnail_Media_id -> media.file_path
  PDF       = content.Attach_file_Media_id    -> media.file_path

  CMS page name: annual-report
  URL: /investor-relations/annual-report

  Example: EXEC dbo.Get_Annual_Reports_Articles_List @ContentId = 111, @PageSize = 8, @PageNumber = 1
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

    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 8;

    SELECT
        c.cont_id,
        c.cont_parent_id,
        c.cont_title,
        c.cont_hmpg_title,
        c.cont_pagename,
        c.language_master_id,
        c.Geography_ID,
        c.cont_intro,
        c.cont_hmpg_intro,
        c.content,

        c.Masthead_image_Media_id,
        m1.file_path AS masthead_image_path,
        c.Masthead_alt_text,

        c.Mobile_Masthead_image_Media_id,
        m2.file_path AS mobile_masthead_image_path,

        c.Template_Master_ID,

        c.Hmpg_thumbnail_Media_id,
        m3.file_path AS Hmpg_thumbnail,
        c.Hmpg_thumbnail_alt_text,

        dbo.GetCanonicalUrl(c.cont_id) AS pageurl,

        c.cont_external_url,
        m4.file_path AS MediafilePath,

        c.cont_sequence,
        c.tag_master_selected_id,
        CAST(NULL AS NVARCHAR(MAX)) AS TagNames,
        c.Cont_Publication,
        c.cont_search_url,
        c.cont_displaydate
    FROM content c
    LEFT JOIN media m1
        ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN media m2
        ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN media m3
        ON c.Hmpg_thumbnail_Media_id = m3.id
    LEFT JOIN media m4
        ON c.Attach_file_Media_id = m4.id
    WHERE
        c.cont_parent_id = @ContentId
        AND c.cont_status = 2
        AND c.content_type_ID = 3
    ORDER BY
        CASE WHEN c.cont_displaydate IS NULL THEN 1 ELSE 0 END,
        c.cont_displaydate DESC,
        ISNULL(c.cont_sequence, 999999),
        c.cont_id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    SELECT
        COUNT(*) AS TotalCount
    FROM content c
    WHERE
        c.cont_parent_id = @ContentId
        AND c.cont_status = 2
        AND c.content_type_ID = 3;
END
GO

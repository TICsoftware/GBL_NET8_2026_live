/*
  Investor Relations PDF listing / load-more
  Used by: InvestorsController.Pdflisting (page 1) and LoadPdfs (page 2+)
           CorporateGovernance_BAL policies / code of conduct lists

  Order: cont_displaydate DESC (newest first), then cont_id DESC.

  CMS page names:
    investor-relations/shareholding-pattern
    investor-relations/subsidiaries
    investor-relations/disclosures
    investor-relations/corporate-governance
    investor-relations/policies
    investor-relations/code-of-conduct
*/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Get_Investor_Pdf_Articles_List
(
    @ContentId  INT,
    @PageSize   INT = 12,
    @PageNumber INT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber IS NULL OR @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize IS NULL OR @PageSize < 1 SET @PageSize = 12;

    /* ==========================================
       Article List (page load + load more)
       Ordered by display date, newest first
       ========================================== */
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
        c.cont_displaydate,
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
        c.cont_search_url
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
        c.cont_displaydate DESC,
        c.cont_id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    /* ==========================================
       Total Count
       ========================================== */
    SELECT
        COUNT(*) AS TotalCount
    FROM content c
    WHERE
        c.cont_parent_id = @ContentId
        AND c.cont_status = 2
        AND c.content_type_ID = 3;
END
GO

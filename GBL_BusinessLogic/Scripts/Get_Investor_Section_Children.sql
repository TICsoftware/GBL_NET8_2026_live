/*
  Child subsections (tabs) under an investor section page.

  Used by CorporateGovernance_BAL for any page routed to
  CorporateGovernance/Index, including:
    investor-relations/corporate-governance
    investor-relations/general-meeting/outcome-of-meeting
    investor-relations/general-meeting/notice-of-meeting

  First 2 children = article tabs (year dropdown, displaydate DESC).
  Remaining children = extra tabs (committees layout when context blocks exist).

  Example: EXEC dbo.Get_Investor_Section_Children @ContentId = 123;
*/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Get_Investor_Section_Children
(
    @ContentId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ContentId IS NULL OR @ContentId < 1
        RETURN;

    SELECT
        c.cont_id,
        c.cont_parent_id,
        c.cont_title,
        c.cont_hmpg_title,
        c.cont_pagename,
        c.cont_intro,
        c.cont_hmpg_intro,
        c.content,
        c.cont_sequence,
        c.cont_displaydate,
        c.cont_external_url,
        CAST(NULL AS NVARCHAR(MAX)) AS MediafilePath,
        CAST(NULL AS NVARCHAR(MAX)) AS pageurl,
        CAST(NULL AS NVARCHAR(MAX)) AS Hmpg_thumbnail,
        CAST(NULL AS NVARCHAR(MAX)) AS Hmpg_thumbnail_alt_text
    FROM content c
    WHERE
        c.cont_parent_id = @ContentId
        AND c.cont_status = 2
    ORDER BY
        ISNULL(c.cont_sequence, 999999),
        c.cont_displaydate DESC,
        c.cont_id DESC;
END
GO

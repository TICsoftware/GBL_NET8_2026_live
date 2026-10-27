/*
  Committees of the Board — context details on Corporate Governance.

  CMS page: corporate-governance  (example cont_id = 449)
  Template: Committees of the Board (context_master / component sequence 1..n)

  Each committee:
    is_block = 1  → Component Title  (committee name, e.g. Audit Committee)
    is_block = 0  → Title (member name), Designation (role), Sequence (member order)

  The live page reads this via sp_GetContentComponentData (table index 1).
*/

-- 1) Page + component rows used by the site
EXEC dbo.sp_GetContentComponentData
    @pagename    = N'corporate-governance',
    @LanguageID  = 1,
    @GeographyID = 1;

-- 2) Direct context_details check (adjust table/column names if your CMS differs)
SELECT
    c.cont_id,
    c.cont_title,
    c.cont_pagename,
    cd.context_group_id,
    cd.is_block,
    cf.name        AS field_name,
    cf.name_key,
    cd.content     AS field_value,
    cd.File_path
FROM dbo.content c
INNER JOIN dbo.context_details cd
    ON cd.cont_id = c.cont_id
LEFT JOIN dbo.context_field cf
    ON cf.id = cd.context_field_id
WHERE LOWER(LTRIM(RTRIM(c.cont_pagename))) IN (
        N'corporate-governance',
        N'investor-relations/corporate-governance'
    )
ORDER BY
    cd.context_group_id,
    ISNULL(cd.is_block, 0) DESC,
    cf.name;

-- 3) PDF children for Policies / Code of Conduct (same SP as subsidiaries)
-- EXEC dbo.Get_Investor_Pdf_Articles_List @ContentId = <policies cont_id>, @PageSize = 100, @PageNumber = 1;
-- EXEC dbo.Get_Investor_Pdf_Articles_List @ContentId = <code-of-conduct cont_id>, @PageSize = 100, @PageNumber = 1;
GO

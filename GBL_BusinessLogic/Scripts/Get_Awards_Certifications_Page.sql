/*
  Awards & Certifications page tree

  Hierarchy:
    Root (e.g. awards / awards-and-certifications)
      ├─ L1 Awards & Accolades
      │    ├─ L2 Individual Honours          → awards slides components
      │    └─ L2 Company & Facility Awards  → awards slides components
      └─ L1 Certifications
           ├─ L2 Sustainability & Responsible Sourcing → certification list
           ├─ L2 Quality & Food Safety
           └─ ...

  Result sets:
    0 = root page content (banner / intro)
    1 = L1 sections (children of root)
    2 = L2 groups (grandchildren)
    3 = soft-template components for all L2 pages

  EXEC dbo.Get_Awards_Certifications_Page
       @PageName = N'awards',
       @LanguageID = 1,
       @GeographyID = 1;
*/
CREATE OR ALTER PROCEDURE dbo.Get_Awards_Certifications_Page
    @PageName    NVARCHAR(500) = NULL,
    @LanguageID  INT = 1,
    @GeographyID INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @RootId INT = NULL;
    DECLARE @NormPage NVARCHAR(500) = LOWER(REPLACE(REPLACE(LTRIM(RTRIM(ISNULL(@PageName, N''))), N' ', N''), N'_', N'-'));

    ;WITH candidates AS
    (
        SELECT
            c.cont_id,
            LOWER(REPLACE(REPLACE(CONVERT(NVARCHAR(500), ISNULL(c.cont_pagename, N'')), N' ', N''), N'_', N'-')) AS page_key,
            ISNULL(TRY_CONVERT(INT, c.cont_sequence), 999999) AS seq_no
        FROM dbo.content c
        WHERE
            c.cont_status = 2
            AND (@LanguageID IS NULL OR c.language_master_id = @LanguageID OR c.language_master_id IS NULL)
            AND (@GeographyID IS NULL OR c.Geography_ID = @GeographyID OR c.Geography_ID IS NULL)
            AND
            (
                @NormPage = N''
                OR LOWER(REPLACE(REPLACE(CONVERT(NVARCHAR(500), ISNULL(c.cont_pagename, N'')), N' ', N''), N'_', N'-')) = @NormPage
                OR LOWER(REPLACE(REPLACE(CONVERT(NVARCHAR(500), ISNULL(c.cont_pagename, N'')), N' ', N''), N'_', N'-')) LIKE N'%/' + @NormPage
                OR LOWER(REPLACE(REPLACE(CONVERT(NVARCHAR(500), ISNULL(c.cont_pagename, N'')), N' ', N''), N'_', N'-')) LIKE @NormPage + N'/%'
                OR LOWER(REPLACE(REPLACE(CONVERT(NVARCHAR(500), ISNULL(c.cont_title, N'')), N' ', N''), N'_', N'-')) LIKE N'%award%certification%'
                OR LOWER(REPLACE(REPLACE(CONVERT(NVARCHAR(500), ISNULL(c.cont_title, N'')), N' ', N''), N'_', N'-')) = N'awards'
            )
    )
    SELECT TOP (1) @RootId = cont_id
    FROM candidates
    ORDER BY
        CASE
            WHEN page_key = @NormPage THEN 0
            WHEN page_key LIKE N'%/' + @NormPage THEN 1
            WHEN page_key LIKE N'%award%certification%' THEN 2
            WHEN page_key = N'awards' THEN 3
            ELSE 9
        END,
        seq_no,
        cont_id;

    IF @RootId IS NULL
    BEGIN
        SELECT TOP (0)
            c.cont_id,
            c.cont_parent_id,
            c.cont_title,
            c.cont_hmpg_title,
            c.cont_pagename,
            c.cont_intro,
            c.cont_hmpg_intro,
            c.content,
            CAST(NULL AS NVARCHAR(MAX)) AS masthead_image_path,
            CAST(NULL AS NVARCHAR(MAX)) AS mobile_masthead_image_path,
            CAST(NULL AS NVARCHAR(MAX)) AS Masthead_alt_text,
            CAST(NULL AS NVARCHAR(MAX)) AS BreadcrumPath,
            CAST(NULL AS NVARCHAR(MAX)) AS cont_window_title,
            CAST(NULL AS NVARCHAR(MAX)) AS cont_metadesc,
            CAST(NULL AS NVARCHAR(MAX)) AS cont_metatag,
            CAST(NULL AS NVARCHAR(MAX)) AS page_schema,
            CAST(NULL AS NVARCHAR(MAX)) AS Hmpg_thumbnail,
            CAST(NULL AS NVARCHAR(MAX)) AS Hmpg_thumbnail_alt_text,
            CAST(NULL AS NVARCHAR(MAX)) AS background_image_path,
            CAST(NULL AS NVARCHAR(MAX)) AS Background_alt_text,
            CAST(NULL AS NVARCHAR(MAX)) AS pageurl,
            CAST(NULL AS NVARCHAR(MAX)) AS parentLink,
            CAST(NULL AS NVARCHAR(MAX)) AS cont_external_url,
            CAST(NULL AS NVARCHAR(MAX)) AS cont_breadcrumb_title,
            CAST(NULL AS INT) AS Template_Master_ID,
            CAST(NULL AS DATETIME) AS cont_displaydate,
            CAST(NULL AS NVARCHAR(MAX)) AS cont_ByLine,
            CAST(NULL AS NVARCHAR(MAX)) AS Cont_Publication
        FROM dbo.content c;

        SELECT TOP (0)
            c.cont_id, c.cont_parent_id, c.cont_title, c.cont_hmpg_title, c.cont_pagename,
            c.cont_intro, c.cont_hmpg_intro, c.content, c.cont_sequence, c.Template_Master_ID
        FROM dbo.content c;

        SELECT TOP (0)
            c.cont_id, c.cont_parent_id, c.cont_title, c.cont_hmpg_title, c.cont_pagename,
            c.cont_intro, c.cont_hmpg_intro, c.content, c.cont_sequence, c.Template_Master_ID
        FROM dbo.content c;

        SELECT TOP (0)
            CAST(NULL AS INT) AS cont_id,
            CAST(NULL AS UNIQUEIDENTIFIER) AS context_group_id,
            CAST(NULL AS NVARCHAR(MAX)) AS field_name,
            CAST(NULL AS NVARCHAR(MAX)) AS name_key,
            CAST(NULL AS INT) AS component_sequence,
            CAST(NULL AS INT) AS is_block,
            CAST(NULL AS NVARCHAR(MAX)) AS field_value,
            CAST(NULL AS NVARCHAR(MAX)) AS component_image_path,
            CAST(NULL AS NVARCHAR(MAX)) AS File_path;

        RETURN;
    END

    /* ==========================================
       0. Root page content
       ========================================== */
    SELECT
        c.cont_id,
        c.cont_parent_id,
        c.cont_title,
        c.cont_hmpg_title,
        c.cont_pagename,
        c.cont_intro,
        c.cont_hmpg_intro,
        c.content,
        m1.file_path AS masthead_image_path,
        m2.file_path AS mobile_masthead_image_path,
        c.Masthead_alt_text,
        CAST(N'' AS NVARCHAR(MAX)) AS BreadcrumPath,
        c.cont_window_title,
        c.cont_metadesc,
        c.cont_metatag,
        c.page_schema,
        m3.file_path AS Hmpg_thumbnail,
        c.Hmpg_thumbnail_alt_text,
        mbg.file_path AS background_image_path,
        c.Background_alt_text,
        dbo.GetCanonicalUrl(c.cont_id) AS pageurl,
        CAST(N'' AS NVARCHAR(MAX)) AS parentLink,
        c.cont_external_url,
        CAST(N'' AS NVARCHAR(MAX)) AS cont_breadcrumb_title,
        c.Template_Master_ID,
        c.cont_displaydate,
        CAST(NULL AS NVARCHAR(MAX)) AS cont_ByLine,
        CAST(NULL AS NVARCHAR(MAX)) AS Cont_Publication
    FROM dbo.content c
    LEFT JOIN dbo.media m1 ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN dbo.media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN dbo.media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
    LEFT JOIN dbo.media mbg ON c.Background_image_Media_id = mbg.id
    WHERE c.cont_id = @RootId;

    /* ==========================================
       1. L1 sections (Awards & Accolades / Certifications)
       ========================================== */
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
        c.Template_Master_ID
    FROM dbo.content c
    WHERE
        c.cont_parent_id = @RootId
        AND c.cont_status = 2
    ORDER BY
        ISNULL(TRY_CONVERT(INT, c.cont_sequence), 999999),
        c.cont_id;

    /* ==========================================
       2. L2 groups under each L1 section
       ========================================== */
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
        c.Template_Master_ID
    FROM dbo.content c
    INNER JOIN dbo.content p
        ON p.cont_id = c.cont_parent_id
       AND p.cont_parent_id = @RootId
       AND p.cont_status = 2
    WHERE c.cont_status = 2
    ORDER BY
        ISNULL(TRY_CONVERT(INT, p.cont_sequence), 999999),
        p.cont_id,
        ISNULL(TRY_CONVERT(INT, c.cont_sequence), 999999),
        c.cont_id;

    /* ==========================================
       3. Components for all L2 pages
          (awards slides / certification list)
       ========================================== */
    SELECT
        cd.cont_id,
        cd.context_group_id,
        CONVERT(NVARCHAR(MAX), ISNULL(cf.name, N'')) AS field_name,
        CONVERT(NVARCHAR(MAX), ISNULL(cf.name_key, N'')) AS name_key,
        ISNULL(
            (
                SELECT TOP (1) TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(CONVERT(NVARCHAR(100), seq.content))), N''))
                FROM dbo.context_details seq
                LEFT JOIN dbo.context_fields seqf ON seqf.id = seq.context_field_id
                WHERE
                    seq.cont_id = cd.cont_id
                    AND seq.context_group_id = cd.context_group_id
                    AND (
                        seq.context_field_id = 10
                        OR LOWER(CONVERT(NVARCHAR(200), ISNULL(seqf.name, N''))) IN (N'sequence', N'component sequence')
                        OR LOWER(CONVERT(NVARCHAR(200), ISNULL(seqf.name_key, N''))) LIKE N'%sequence%'
                    )
            ),
            1
        ) AS component_sequence,
        1 AS is_block,
        CONVERT(NVARCHAR(MAX), ISNULL(cd.content, N'')) AS field_value,
        CONVERT(NVARCHAR(MAX), COALESCE(
            m.file_path,
            CASE
                WHEN CONVERT(NVARCHAR(MAX), ISNULL(cd.File_path, N'')) LIKE N'%/%'
                  OR CONVERT(NVARCHAR(MAX), ISNULL(cd.File_path, N'')) LIKE N'%.%'
                THEN CONVERT(NVARCHAR(MAX), cd.File_path)
                ELSE NULL
            END
        )) AS component_image_path,
        CONVERT(NVARCHAR(MAX), cd.File_path) AS File_path
    FROM dbo.context_details cd
    INNER JOIN dbo.content leaf
        ON leaf.cont_id = cd.cont_id
       AND leaf.cont_status = 2
    INNER JOIN dbo.content section
        ON section.cont_id = leaf.cont_parent_id
       AND section.cont_parent_id = @RootId
       AND section.cont_status = 2
    LEFT JOIN dbo.context_fields cf
        ON cf.id = cd.context_field_id
    OUTER APPLY (
        SELECT TOP (1) mx.file_path
        FROM dbo.media mx
        WHERE (
                TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(CONVERT(NVARCHAR(100), ISNULL(cd.File_path, N'')))), N'')) IS NOT NULL
                AND mx.id = TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(CONVERT(NVARCHAR(100), cd.File_path))), N''))
            )
            OR (
                CONVERT(NVARCHAR(MAX), ISNULL(cd.File_path, N'')) LIKE N'%/%'
                AND (
                    mx.file_path = CONVERT(NVARCHAR(MAX), cd.File_path)
                    OR mx.file_path = N'/' + LTRIM(CONVERT(NVARCHAR(MAX), cd.File_path))
                )
            )
        ORDER BY mx.id
    ) m
    WHERE cd.context_group_id IS NOT NULL
      AND ISNULL(TRY_CONVERT(INT, cd.Status), 2) = 2
    ORDER BY
        leaf.cont_id,
        component_sequence,
        cd.context_group_id,
        cf.id;
END
GO

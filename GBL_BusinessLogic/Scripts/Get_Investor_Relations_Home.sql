/*
  Investor Relations landing page (/investor-relations)

  Source queries:
    SELECT * FROM content WHERE cont_id = @ContentId;                          -- parent (resolved in BAL)
    SELECT * FROM content WHERE cont_parent_id = @ContentId;                   -- section children
    SELECT * FROM content WHERE cont_parent_id IN (
        SELECT cont_id FROM content WHERE cont_parent_id = @ContentId AND cont_status = 2
    ) AND cont_status = 2;                                                     -- nested articles / year pages
    SELECT * FROM content
     WHERE (cont_parent_id = 3 OR cont_parent_id = 7)
       AND cont_section_tagid = @ContentId;                                    -- What's New (press + media)

  Result sets:
    0  Direct children of investor-relations (published sections)
    1  Grandchildren (annual reports, subsidiary PDFs, quarterly years, CG tabs)
    2  What's New: latest 2 press-release + 1 media-coverage (prefer investor-tagged)
    3  Corporate-governance children (Committees, Code of Conduct, Policies)
    4  Investor Contact components (context_details of the 'investors contact' child, e.g. cont_id = 476)
    5  Shareholder Information subsections
         SELECT * FROM content WHERE cont_id = 478
         SELECT * FROM content WHERE cont_parent_id = 478
         SELECT * FROM content WHERE cont_parent_id = 120
    6  Latest Updates item (cont_parent_id = 19) → Announcement card
    7  Subsidiaries documents (cont_parent_id = 114 by default)
         ordered by cont_displaydate DESC (NULLs last)

  Example:
    EXEC dbo.Get_Investor_Relations_Home
         @ContentId = 110, @PressParentId = 3, @MediaParentId = 7,
         @UpdatesParentId = 19, @SubsidiariesParentId = 114;
*/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Get_Investor_Relations_Home
(
    @ContentId                 INT,
    @PressParentId             INT = 3,
    @MediaParentId             INT = 7,
    @ShareholderExtraParentId  INT = 120,
    @UpdatesParentId           INT = 19,
    @SubsidiariesParentId      INT = 114
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ContentId IS NULL OR @ContentId < 1
        RETURN;

    IF @PressParentId IS NULL OR @PressParentId < 1 SET @PressParentId = 3;
    IF @MediaParentId IS NULL OR @MediaParentId < 1 SET @MediaParentId = 7;
    IF @UpdatesParentId IS NULL OR @UpdatesParentId < 1 SET @UpdatesParentId = 19;
    IF @SubsidiariesParentId IS NULL OR @SubsidiariesParentId < 1 SET @SubsidiariesParentId = 114;

    DECLARE @TagNeedle nvarchar(30) = N',' + CAST(@ContentId AS nvarchar(20)) + N',';

    DECLARE @CgId INT =
    (
        SELECT TOP 1 c.cont_id
        FROM content c
        WHERE
            c.cont_status = 2
            AND (
                c.cont_parent_id = @ContentId
                OR c.cont_pagename IN (N'corporate-governance', N'investor-relations/corporate-governance')
            )
            AND (
                LOWER(ISNULL(c.cont_pagename, N'')) LIKE N'%corporate-governance%'
                OR LOWER(ISNULL(c.cont_title, N'')) LIKE N'%corporate governance%'
            )
        ORDER BY
            CASE WHEN c.cont_parent_id = @ContentId THEN 0 ELSE 1 END,
            ISNULL(c.cont_sequence, 999999),
            c.cont_id
    );

    DECLARE @ContactId INT =
    (
        SELECT TOP 1 c.cont_id
        FROM content c
        WHERE
            c.cont_status = 2
            AND c.cont_parent_id = @ContentId
            AND (
                LOWER(REPLACE(ISNULL(c.cont_pagename, N''), N' ', N'')) LIKE N'%investor%contact%'
                OR LOWER(REPLACE(ISNULL(c.cont_title, N''), N' ', N'')) LIKE N'%investor%contact%'
                OR LOWER(REPLACE(ISNULL(c.cont_hmpg_title, N''), N' ', N'')) LIKE N'%investor%contact%'
            )
        ORDER BY
            ISNULL(c.cont_sequence, 999999),
            c.cont_id
    );

    DECLARE @ShareholderId INT =
    (
        SELECT TOP 1 c.cont_id
        FROM content c
        WHERE
            c.cont_status = 2
            AND (
                c.cont_id = 478
                OR (
                    c.cont_parent_id = @ContentId
                    AND (
                        LOWER(REPLACE(ISNULL(c.cont_pagename, N''), N' ', N'')) LIKE N'%shareholder%information%'
                        OR LOWER(REPLACE(ISNULL(c.cont_title, N''), N' ', N'')) LIKE N'%shareholder%information%'
                        OR LOWER(REPLACE(ISNULL(c.cont_hmpg_title, N''), N' ', N'')) LIKE N'%shareholder%information%'
                    )
                )
            )
        ORDER BY
            CASE WHEN c.cont_id = 478 THEN 0 WHEN c.cont_parent_id = @ContentId THEN 1 ELSE 2 END,
            ISNULL(c.cont_sequence, 999999),
            c.cont_id
    );

    IF @ShareholderExtraParentId IS NULL OR @ShareholderExtraParentId < 1
        SET @ShareholderExtraParentId = 120;

    /* Subsidiaries lives under Shareholder Information (e.g. 478 → 114), not always as a direct IR child. */
    DECLARE @SubsidiariesId INT =
    (
        SELECT TOP 1 c.cont_id
        FROM content c
        WHERE
            c.cont_status = 2
            AND (
                c.cont_id = @SubsidiariesParentId
                OR (
                    (
                        c.cont_parent_id = @ContentId
                        OR (@ShareholderId IS NOT NULL AND c.cont_parent_id = @ShareholderId)
                    )
                    AND (
                        LOWER(REPLACE(ISNULL(c.cont_pagename, N''), N' ', N'')) LIKE N'%subsidiar%'
                        OR LOWER(REPLACE(ISNULL(c.cont_title, N''), N' ', N'')) LIKE N'%subsidiar%'
                    )
                )
            )
        ORDER BY
            CASE WHEN c.cont_id = @SubsidiariesParentId THEN 0 ELSE 1 END,
            CASE WHEN @ShareholderId IS NOT NULL AND c.cont_parent_id = @ShareholderId THEN 0 ELSE 1 END,
            ISNULL(c.cont_sequence, 999999),
            c.cont_id
    );

    /* ==========================================
       0. Section children of investor-relations
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
        c.content_type_ID,
        c.Masthead_image_Media_id,
        m1.file_path AS masthead_image_path,
        c.Masthead_alt_text,
        c.Mobile_Masthead_image_Media_id,
        m2.file_path AS mobile_masthead_image_path,
        c.Template_Master_ID,
        c.Hmpg_thumbnail_Media_id,
        m3.file_path AS Hmpg_thumbnail,
        c.Hmpg_thumbnail_alt_text,
        c.Background_image_Media_id,
        mbg.file_path AS background_image_path,
        c.Background_alt_text,
        dbo.GetCanonicalUrl(c.cont_id) AS pageurl,
        c.cont_external_url,
        CAST(NULL AS NVARCHAR(MAX)) AS MediafilePath,
        c.cont_sequence,
        c.tag_master_selected_id,
        CAST(NULL AS NVARCHAR(MAX)) AS TagNames,
        c.Cont_Publication,
        c.cont_search_url
    FROM content c
    LEFT JOIN media m1 ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
    LEFT JOIN media mbg ON c.Background_image_Media_id = mbg.id
    WHERE
        c.cont_parent_id = @ContentId
        AND c.cont_status = 2
    ORDER BY
        ISNULL(c.cont_sequence, 999999),
        c.cont_id;

    /* ==========================================
       1. Nested pages / articles (grandchildren)
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
        c.content_type_ID,
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
    LEFT JOIN media m1 ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
    LEFT JOIN media m4 ON c.Attach_file_Media_id = m4.id
    WHERE
        c.cont_status = 2
        AND c.cont_parent_id IN
        (
            SELECT p.cont_id
            FROM content p
            WHERE p.cont_parent_id = @ContentId
              AND p.cont_status = 2
        )
    ORDER BY
        CASE WHEN c.cont_displaydate IS NULL THEN 1 ELSE 0 END,
        c.cont_displaydate DESC,
        ISNULL(c.cont_sequence, 999999),
        c.cont_id DESC;

    /* ==========================================
       2. What's New — latest 2 press (3) + 1 media (7)
          Prefer investor-tagged rows; fill with absolute latest.
       ========================================== */
    ;WITH NewsPool AS
    (
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
            c.content_type_ID,
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
            CAST(NULL AS NVARCHAR(MAX)) AS MediafilePath,
            c.cont_sequence,
            c.tag_master_selected_id,
            CASE
                WHEN c.cont_parent_id = @MediaParentId
                  OR LOWER(ISNULL(p.cont_pagename, N'')) LIKE N'%media-coverage%'
                    THEN N'Media Coverage'
                ELSE N'Press Release'
            END AS TagNames,
            c.Cont_Publication,
            c.cont_search_url,
            CASE
                WHEN c.cont_parent_id = @MediaParentId
                  OR LOWER(ISNULL(p.cont_pagename, N'')) LIKE N'%media-coverage%'
                    THEN 1
                ELSE 0
            END AS IsMedia,
            CASE
                WHEN TRY_CONVERT(int, CONVERT(nvarchar(50), c.cont_section_tagid)) = @ContentId
                  OR (
                        N',' + REPLACE(REPLACE(ISNULL(CONVERT(nvarchar(200), c.cont_section_tagid), N''), N' ', N''), N';', N',') + N','
                     ) LIKE N'%' + @TagNeedle + N'%'
                    THEN 0
                ELSE 1
            END AS TagRank,
            ROW_NUMBER() OVER (
                PARTITION BY
                    CASE
                        WHEN c.cont_parent_id = @MediaParentId
                          OR LOWER(ISNULL(p.cont_pagename, N'')) LIKE N'%media-coverage%'
                            THEN 1
                        ELSE 0
                    END
                ORDER BY
                    CASE
                        WHEN TRY_CONVERT(int, CONVERT(nvarchar(50), c.cont_section_tagid)) = @ContentId
                          OR (
                                N',' + REPLACE(REPLACE(ISNULL(CONVERT(nvarchar(200), c.cont_section_tagid), N''), N' ', N''), N';', N',') + N','
                             ) LIKE N'%' + @TagNeedle + N'%'
                            THEN 0
                        ELSE 1
                    END,
                    CASE WHEN c.cont_displaydate IS NULL THEN 1 ELSE 0 END,
                    c.cont_displaydate DESC,
                    c.cont_id DESC
            ) AS rn
        FROM content c
        INNER JOIN content p
            ON p.cont_id = c.cont_parent_id
        LEFT JOIN media m1 ON c.Masthead_image_Media_id = m1.id
        LEFT JOIN media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
        LEFT JOIN media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
        WHERE
            c.cont_status = 2
            AND ISNULL(c.content_type_ID, 3) = 3
            AND (
                c.cont_parent_id IN (@PressParentId, @MediaParentId)
                OR LOWER(ISNULL(p.cont_pagename, N'')) LIKE N'%press-release%'
                OR LOWER(ISNULL(p.cont_pagename, N'')) LIKE N'%media-coverage%'
            )
    )
    SELECT
        cont_id,
        cont_parent_id,
        cont_title,
        cont_hmpg_title,
        cont_pagename,
        language_master_id,
        Geography_ID,
        cont_intro,
        cont_hmpg_intro,
        content,
        cont_displaydate,
        content_type_ID,
        Masthead_image_Media_id,
        masthead_image_path,
        Masthead_alt_text,
        Mobile_Masthead_image_Media_id,
        mobile_masthead_image_path,
        Template_Master_ID,
        Hmpg_thumbnail_Media_id,
        Hmpg_thumbnail,
        Hmpg_thumbnail_alt_text,
        pageurl,
        cont_external_url,
        MediafilePath,
        cont_sequence,
        tag_master_selected_id,
        TagNames,
        Cont_Publication,
        cont_search_url
    FROM NewsPool
    WHERE
        (IsMedia = 0 AND rn <= 2)
        OR (IsMedia = 1 AND rn <= 1)
    ORDER BY
        IsMedia,
        rn;

    /* ==========================================
       3. Corporate governance children
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
        c.content_type_ID,
        c.Masthead_image_Media_id,
        CAST(NULL AS NVARCHAR(MAX)) AS masthead_image_path,
        c.Masthead_alt_text,
        c.Mobile_Masthead_image_Media_id,
        CAST(NULL AS NVARCHAR(MAX)) AS mobile_masthead_image_path,
        c.Template_Master_ID,
        c.Hmpg_thumbnail_Media_id,
        CAST(NULL AS NVARCHAR(MAX)) AS Hmpg_thumbnail,
        c.Hmpg_thumbnail_alt_text,
        dbo.GetCanonicalUrl(c.cont_id) AS pageurl,
        c.cont_external_url,
        CAST(NULL AS NVARCHAR(MAX)) AS MediafilePath,
        c.cont_sequence,
        c.tag_master_selected_id,
        CAST(NULL AS NVARCHAR(MAX)) AS TagNames,
        c.Cont_Publication,
        c.cont_search_url
    FROM content c
    WHERE
        @CgId IS NOT NULL
        AND c.cont_parent_id = @CgId
        AND c.cont_status = 2
    ORDER BY
        ISNULL(c.cont_sequence, 999999),
        c.cont_id;

    /* ==========================================
       4. Investor Contact components
          SELECT * FROM context_details WHERE cont_id = @ContactId
          (e.g. 476 — Title + Content + Sequence)
       ========================================== */
    SELECT
        cd.cont_id,
        cd.context_group_id,
        ISNULL(cf.name, N'') AS field_name,
        ISNULL(cf.name_key, N'') AS name_key,
        ISNULL(
            (
                SELECT TOP 1 TRY_CONVERT(int, NULLIF(LTRIM(RTRIM(seq.content)), N''))
                FROM dbo.context_details seq
                LEFT JOIN dbo.context_fields seqf
                    ON seqf.id = seq.context_field_id
                WHERE
                    seq.cont_id = cd.cont_id
                    AND seq.context_group_id = cd.context_group_id
                    AND (
                        seq.context_field_id = 10
                        OR LOWER(ISNULL(seqf.name, N'')) IN (N'sequence', N'component sequence')
                        OR LOWER(ISNULL(seqf.name_key, N'')) LIKE N'%sequence%'
                    )
            ),
            1
        ) AS component_sequence,
        1 AS is_block,
        ISNULL(cd.content, N'') AS field_value,
        COALESCE(
            m.file_path,
            CASE
                WHEN cd.File_path LIKE N'%/%' OR cd.File_path LIKE N'%.%' THEN cd.File_path
                ELSE NULL
            END
        ) AS component_image_path,
        cd.File_path
    FROM dbo.context_details cd
    LEFT JOIN dbo.context_fields cf
        ON cf.id = cd.context_field_id
    OUTER APPLY (
        SELECT TOP (1) mx.file_path
        FROM dbo.media mx
        WHERE (
                TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(ISNULL(cd.File_path, N''))), N'')) IS NOT NULL
                AND mx.id = TRY_CONVERT(INT, NULLIF(LTRIM(RTRIM(cd.File_path)), N''))
            )
            OR (
                cd.File_path LIKE N'%/%'
                AND (
                    mx.file_path = cd.File_path
                    OR mx.file_path = N'/' + LTRIM(cd.File_path)
                )
            )
        ORDER BY mx.id
    ) m
    WHERE
        @ContactId IS NOT NULL
        AND cd.cont_id = @ContactId
        AND cd.context_group_id IS NOT NULL;

    /* ==========================================
       5. Shareholder Information subsections
          SELECT * FROM content WHERE cont_parent_id = 478
          SELECT * FROM content WHERE cont_parent_id = 120
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
        c.content_type_ID,
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
        CAST(NULL AS NVARCHAR(MAX)) AS MediafilePath,
        c.cont_sequence,
        c.tag_master_selected_id,
        CAST(NULL AS NVARCHAR(MAX)) AS TagNames,
        c.Cont_Publication,
        c.cont_search_url
    FROM content c
    LEFT JOIN media m1 ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
    WHERE
        c.cont_status = 2
        AND (
            (@ShareholderId IS NOT NULL AND c.cont_parent_id = @ShareholderId)
            OR c.cont_parent_id = @ShareholderExtraParentId
        )
        AND c.cont_id <> ISNULL(@ShareholderId, 0)
        AND c.cont_id <> @ShareholderExtraParentId
        AND LOWER(REPLACE(ISNULL(c.cont_title, N''), N' ', N'')) NOT LIKE N'%investor%contact%'
        AND LOWER(REPLACE(ISNULL(c.cont_pagename, N''), N' ', N'')) NOT LIKE N'%investor%contact%'
    ORDER BY
        ISNULL(c.cont_sequence, 999999),
        c.cont_id;

    /* ==========================================
       6. Latest Updates item → What's New Announcement
          cont_parent_id = 19 (Updates)
       ========================================== */
    SELECT TOP (1)
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
        c.content_type_ID,
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
        CAST(NULL AS NVARCHAR(MAX)) AS MediafilePath,
        c.cont_sequence,
        c.tag_master_selected_id,
        N'Updates' AS TagNames,
        c.Cont_Publication,
        c.cont_search_url
    FROM content c
    LEFT JOIN media m1 ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
    WHERE
        c.cont_status = 2
        AND (
            c.cont_parent_id = @UpdatesParentId
            OR EXISTS (
                SELECT 1
                FROM content p
                WHERE p.cont_id = c.cont_parent_id
                  AND (
                        LOWER(ISNULL(p.cont_pagename, N'')) LIKE N'%updates%'
                        OR LOWER(ISNULL(p.cont_title, N'')) = N'updates'
                  )
            )
        )
    ORDER BY
        CASE WHEN c.cont_displaydate IS NULL THEN 1 ELSE 0 END,
        c.cont_displaydate DESC,
        c.cont_id DESC;

    /* ==========================================
       7. Subsidiaries documents (latest first)
          Default parent = 114 (under Shareholder Information)
          ORDER BY cont_displaydate DESC (NULLs last)
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
        c.content_type_ID,
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
        N'Subsidiaries' AS TagNames,
        c.Cont_Publication,
        c.cont_search_url,
        p.cont_title AS SectionTitle,
        pmbg.file_path AS section_background_image_path,
        p.Background_alt_text AS section_background_alt_text,
        pm3.file_path AS section_thumbnail_path,
        p.Hmpg_thumbnail_alt_text AS section_thumbnail_alt_text
    FROM content c
    INNER JOIN content p
        ON p.cont_id = c.cont_parent_id
    LEFT JOIN media m1 ON c.Masthead_image_Media_id = m1.id
    LEFT JOIN media m2 ON c.Mobile_Masthead_image_Media_id = m2.id
    LEFT JOIN media m3 ON c.Hmpg_thumbnail_Media_id = m3.id
    LEFT JOIN media m4 ON c.Attach_file_Media_id = m4.id
    LEFT JOIN media pmbg ON p.Background_image_Media_id = pmbg.id
    LEFT JOIN media pm3 ON p.Hmpg_thumbnail_Media_id = pm3.id
    WHERE
        c.cont_status = 2
        AND ISNULL(c.content_type_ID, 3) = 3
        AND (
            (@SubsidiariesId IS NOT NULL AND c.cont_parent_id = @SubsidiariesId)
            OR c.cont_parent_id = @SubsidiariesParentId
        )
    ORDER BY
        CASE WHEN c.cont_displaydate IS NULL THEN 1 ELSE 0 END,
        c.cont_displaydate DESC,
        c.cont_id DESC;
END
GO

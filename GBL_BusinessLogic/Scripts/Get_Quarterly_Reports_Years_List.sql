/*
  Quarterly Reports
  Years = section insides under the parent page (cont_parent_id).
  Quarters = context_details blocks on each year page.

  Fields (context_fields):
    Title
    Block date
    Financial result pdf
    Result presentation pdf
    Earning call transcript
    Earning call audio
    Download all

  Example:
    EXEC dbo.Get_Quarterly_Reports_Years_List @ContentId = 123;
*/
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Get_Quarterly_Reports_Years_List
(
    @ContentId INT
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @ContentId IS NULL OR @ContentId < 1
        RETURN;

    /* ==========================================
       Year pages (section insides)
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

    /* ==========================================
       Quarter blocks for those years
       ========================================== */
    SELECT
        y.cont_id,
        cd.context_group_id,
        ISNULL(cf.name, N'') AS field_name,
        ISNULL(cf.name_key, N'') AS name_key,
        1 AS component_sequence,
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
    FROM content y
    INNER JOIN dbo.context_details cd
        ON cd.cont_id = y.cont_id
       AND cd.context_group_id IS NOT NULL
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
        y.cont_parent_id = @ContentId
        AND y.cont_status = 2;
END
GO

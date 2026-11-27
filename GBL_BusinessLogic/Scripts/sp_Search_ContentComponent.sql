/*
    Site search: pages, components, products and industries,
    plus the most-searched keywords and the pages people open.

    Run this script on the GBL database before using the updated search.

    Procedures
      sp_Search_ContentComponent   keyword search (pages, components, products, industries)
      sp_Search_BrowseSection      sections and articles for a header chip, with no keyword
      sp_Search_Log                record a search or an opened result
      sp_Search_GetPopular         popular keywords and popular pages
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* ============================================================
   Most searched keywords, sections, articles, products, industries
   ============================================================ */
IF OBJECT_ID(N'dbo.Search_Popular', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Search_Popular
    (
        SearchPopularId INT IDENTITY(1,1) NOT NULL,
        Keyword         NVARCHAR(200)  NOT NULL,
        Category        NVARCHAR(100)  NOT NULL CONSTRAINT DF_Search_Popular_Category DEFAULT (N'All'),
        ItemType        NVARCHAR(50)   NOT NULL,   -- Keyword, Page, Section, Article, Product, Industry, PDF
        ItemTitle       NVARCHAR(500)  NULL,
        ItemUrl         NVARCHAR(1000) NOT NULL CONSTRAINT DF_Search_Popular_ItemUrl DEFAULT (N''),
        SectionName     NVARCHAR(300)  NULL,
        SearchCount     INT            NOT NULL CONSTRAINT DF_Search_Popular_SearchCount DEFAULT (1),
        LastSearched    DATETIME       NOT NULL CONSTRAINT DF_Search_Popular_LastSearched DEFAULT (GETDATE()),
        CreatedDate     DATETIME       NOT NULL CONSTRAINT DF_Search_Popular_CreatedDate DEFAULT (GETDATE()),
        MatchKey        VARBINARY(32)  NOT NULL,
        CONSTRAINT PK_Search_Popular PRIMARY KEY CLUSTERED (SearchPopularId)
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_Search_Popular_MatchKey' AND object_id = OBJECT_ID(N'dbo.Search_Popular')
)
BEGIN
    CREATE UNIQUE INDEX UX_Search_Popular_MatchKey
        ON dbo.Search_Popular (MatchKey);
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Search_Popular_Rank' AND object_id = OBJECT_ID(N'dbo.Search_Popular')
)
BEGIN
    CREATE INDEX IX_Search_Popular_Rank
        ON dbo.Search_Popular (ItemType, SearchCount DESC, LastSearched DESC)
        INCLUDE (Keyword, ItemTitle, ItemUrl, Category, SectionName);
END
GO

/* Starter list, used until real searches overtake it. */
IF NOT EXISTS (SELECT 1 FROM dbo.Search_Popular WHERE ItemType = N'Keyword')
BEGIN
    INSERT INTO dbo.Search_Popular
        (Keyword, Category, ItemType, ItemTitle, ItemUrl, SectionName, SearchCount, LastSearched, MatchKey)
    VALUES
        (N'Annual report', N'Investors', N'Keyword', NULL, N'', NULL, 3, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Annual report'), N'|', LOWER(N'Investors'), N'|', LOWER(N'Keyword'), N'|', LOWER(N''))))),
        (N'Subsidiaries', N'Investors', N'Keyword', NULL, N'', NULL, 3, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Subsidiaries'), N'|', LOWER(N'Investors'), N'|', LOWER(N'Keyword'), N'|', LOWER(N''))))),
        (N'Financial statements', N'Investors', N'Keyword', NULL, N'', NULL, 3, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Financial statements'), N'|', LOWER(N'Investors'), N'|', LOWER(N'Keyword'), N'|', LOWER(N''))))),
        (N'Sustainability', N'Sustainability', N'Keyword', NULL, N'', NULL, 3, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Sustainability'), N'|', LOWER(N'Sustainability'), N'|', LOWER(N'Keyword'), N'|', LOWER(N''))))),
        (N'Job openings', N'Careers', N'Keyword', NULL, N'', NULL, 3, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Job openings'), N'|', LOWER(N'Careers'), N'|', LOWER(N'Keyword'), N'|', LOWER(N'')))));
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Search_Popular WHERE ItemType <> N'Keyword')
BEGIN
    INSERT INTO dbo.Search_Popular
        (Keyword, Category, ItemType, ItemTitle, ItemUrl, SectionName, SearchCount, LastSearched, MatchKey)
    VALUES
        (N'Our Manufacturing Ecosystem', N'About Us', N'Article', N'Our Manufacturing Ecosystem', N'/about-us/manufacturing', N'About Us', 2, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Our Manufacturing Ecosystem'), N'|', LOWER(N'About Us'), N'|', LOWER(N'Article'), N'|', LOWER(N'/about-us/manufacturing'))))),
        (N'Shareholder Information', N'Investors', N'Article', N'Shareholder Information', N'/investors/shareholding-pattern', N'Investors', 2, DATEADD(DAY, -2, GETDATE()),
            CONVERT(VARBINARY(32), HASHBYTES('SHA2_256', CONCAT(LOWER(N'Shareholder Information'), N'|', LOWER(N'Investors'), N'|', LOWER(N'Article'), N'|', LOWER(N'/investors/shareholding-pattern')))));
END
GO

/* Shared list of URL slugs for each overlay chip. Caller creates #SearchSlug. */
CREATE OR ALTER PROCEDURE dbo.sp_Search_FillCategorySlugs
(
    @Category NVARCHAR(100)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID(N'tempdb..#SearchSlug') IS NULL
        RETURN;

    DELETE FROM #SearchSlug;

    IF @Category = N'Investors'
        INSERT INTO #SearchSlug (Slug) VALUES
            (N'investors'), (N'investor'), (N'investor-relations'),
            (N'annualreports'), (N'annual-reports'),
            (N'quarterly-reports'), (N'quarterlyreports'),
            (N'financial-highlights'), (N'corporate-governance');
    ELSE IF @Category = N'Businesses'
        INSERT INTO #SearchSlug (Slug) VALUES
            (N'businesses'), (N'products'), (N'industries'),
            (N'industries-we-serve'), (N'industriesinside');
    ELSE IF @Category = N'Sustainability'
        INSERT INTO #SearchSlug (Slug) VALUES
            (N'sustainability'), (N'csr'), (N'resource-circularity');
    ELSE IF @Category = N'Media'
        INSERT INTO #SearchSlug (Slug) VALUES
            (N'media'), (N'awards');
    ELSE IF @Category = N'Careers'
        INSERT INTO #SearchSlug (Slug) VALUES
            (N'careers'), (N'career'), (N'learningdevelopment');
END
GO

/* ============================================================
   Keyword search across content, components, products, industries
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_Search_ContentComponent
(
    @Keyword    NVARCHAR(500),
    @Page       INT = 1,
    @PageCount  INT = 10,
    @RawKeyword NVARCHAR(500) = NULL,
    @Category   NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageCount < 1 SET @PageCount = 10;
    IF @PageCount > 100 SET @PageCount = 100;

    IF @Category IS NULL OR LTRIM(RTRIM(@Category)) IN (N'', N'All')
        SET @Category = N'All';

    SET @RawKeyword = LEFT(LTRIM(RTRIM(ISNULL(@RawKeyword, N''))), 200);
    SET @RawKeyword = REPLACE(REPLACE(REPLACE(REPLACE(@RawKeyword, N'%', N''), N'_', N''), N'[', N''), N']', N'');

    IF OBJECT_ID(N'tempdb..#SearchData') IS NOT NULL
        DROP TABLE #SearchData;

    CREATE TABLE #SearchData
    (
        cont_id            INT,
        cont_title         NVARCHAR(MAX),
        cont_intro         NVARCHAR(MAX),
        cont_hmpg_intro    NVARCHAR(MAX),
        content            NVARCHAR(MAX),
        cont_pagename      NVARCHAR(1000),
        cont_status        INT,
        cont_publishdate   DATETIME,
        PageUrl            NVARCHAR(MAX),
        ComponentName      NVARCHAR(MAX),
        ComponentValue     NVARCHAR(MAX),
        SearchPriority     INT,
        ResultType         NVARCHAR(50),
        SectionPath        NVARCHAR(500)
    );

    CREATE TABLE #SearchSlug (Slug NVARCHAR(100) NOT NULL PRIMARY KEY);
    EXEC dbo.sp_Search_FillCategorySlugs @Category;

    /* Pages */
    IF @Keyword IS NOT NULL AND LTRIM(RTRIM(@Keyword)) NOT IN (N'', N'""')
    BEGIN
        INSERT INTO #SearchData
        (
            cont_id, cont_title, cont_intro, cont_hmpg_intro, content, cont_pagename,
            cont_status, cont_publishdate, PageUrl, ComponentName, ComponentValue,
            SearchPriority, ResultType, SectionPath
        )
        SELECT
            c.cont_id,
            c.cont_title,
            c.cont_intro,
            c.cont_hmpg_intro,
            c.content,
            c.cont_pagename,
            c.cont_status,
            c.cont_publishdate,
            dbo.GetCanonicalUrl(c.cont_id),
            NULL,
            NULL,
            1,
            CASE
                WHEN LOWER(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N'')) LIKE N'%.pdf'
                  OR LOWER(ISNULL(c.cont_pagename, N'')) LIKE N'%.pdf'
                THEN N'PDF'
                ELSE N'Page'
            END,
            NULL
        FROM dbo.content c
        WHERE c.cont_status = 2
          AND CONTAINS
          (
              (c.cont_title, c.cont_intro, c.cont_hmpg_intro, c.content),
              @Keyword
          )
          AND
          (
              @Category = N'All'
              OR EXISTS
              (
                  SELECT 1
                  FROM #SearchSlug s
                  WHERE N'/' + LOWER(REPLACE(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N''), N'\', N'/')) + N'/'
                            LIKE N'%/' + s.Slug + N'/%'
                     OR LOWER(ISNULL(c.cont_pagename, N'')) = s.Slug
                     OR LOWER(ISNULL(c.cont_pagename, N'')) LIKE s.Slug + N'/%'
              )
          );

        /* Component text on those pages */
        INSERT INTO #SearchData
        (
            cont_id, cont_title, cont_intro, cont_hmpg_intro, content, cont_pagename,
            cont_status, cont_publishdate, PageUrl, ComponentName, ComponentValue,
            SearchPriority, ResultType, SectionPath
        )
        SELECT
            c.cont_id,
            c.cont_title,
            c.cont_intro,
            c.cont_hmpg_intro,
            c.content,
            c.cont_pagename,
            c.cont_status,
            c.cont_publishdate,
            dbo.GetCanonicalUrl(c.cont_id),
            cm.Title,
            cd.content,
            2,
            CASE
                WHEN LOWER(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N'')) LIKE N'%.pdf'
                  OR LOWER(ISNULL(c.cont_pagename, N'')) LIKE N'%.pdf'
                THEN N'PDF'
                ELSE N'Page'
            END,
            NULL
        FROM dbo.content c
        INNER JOIN dbo.context_details cd
            ON cd.cont_id = c.cont_id
           AND cd.Status = 2
        INNER JOIN dbo.context_master cm
            ON cm.ID = cd.context_master_id
           AND cm.Status = 1
        WHERE c.cont_status = 2
          AND CONTAINS(cd.content, @Keyword)
          AND
          (
              @Category = N'All'
              OR EXISTS
              (
                  SELECT 1
                  FROM #SearchSlug s
                  WHERE N'/' + LOWER(REPLACE(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N''), N'\', N'/')) + N'/'
                            LIKE N'%/' + s.Slug + N'/%'
                     OR LOWER(ISNULL(c.cont_pagename, N'')) = s.Slug
                     OR LOWER(ISNULL(c.cont_pagename, N'')) LIKE s.Slug + N'/%'
              )
          );
    END

    /* Products and industries belong with Businesses, and also with All. */
    IF LEN(@RawKeyword) >= 2 AND @Category IN (N'All', N'Businesses')
    BEGIN
        DECLARE @Like NVARCHAR(400) = N'%' + REPLACE(@RawKeyword, N' ', N'%') + N'%';

        IF OBJECT_ID(N'dbo.Product_Master', N'U') IS NOT NULL
        BEGIN
            BEGIN TRY
            INSERT INTO #SearchData
            (
                cont_id, cont_title, cont_intro, cont_hmpg_intro, content, cont_pagename,
                cont_status, cont_publishdate, PageUrl, ComponentName, ComponentValue,
                SearchPriority, ResultType, SectionPath
            )
            SELECT
                p.ProductId,
                p.ProductName,
                ISNULL(p.Intro, N''),
                N'',
                ISNULL(p.Content, N''),
                p.Product_pagename,
                ISNULL(p.[status], 0),
                ISNULL(p.ModifiedDate, p.CreatedDate),
                N'/Products/' + p.Product_pagename,
                N'Products',
                NULL,
                CASE
                    WHEN LOWER(p.ProductName) = LOWER(@RawKeyword) THEN 0
                    WHEN p.ProductName LIKE @Like OR ISNULL(p.Product_pagename, N'') LIKE @Like THEN 1
                    ELSE 2
                END,
                N'Product',
                N'Businesses › Products'
            FROM dbo.Product_Master p
            WHERE ISNULL(p.[status], 0) = 2
              AND NULLIF(LTRIM(RTRIM(p.Product_pagename)), N'') IS NOT NULL
              AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = 1)
              AND
              (
                  p.ProductName LIKE @Like
                  OR ISNULL(p.Product_pagename, N'') LIKE @Like
                  OR ISNULL(p.Intro, N'') LIKE @Like
                  OR ISNULL(p.Content, N'') LIKE @Like
                  OR ISNULL(p.Technical_Overview, N'') LIKE @Like
              );
            END TRY
            BEGIN CATCH
            END CATCH
        END

        IF OBJECT_ID(N'dbo.Industry_Master', N'U') IS NOT NULL
        BEGIN
            BEGIN TRY
            INSERT INTO #SearchData
            (
                cont_id, cont_title, cont_intro, cont_hmpg_intro, content, cont_pagename,
                cont_status, cont_publishdate, PageUrl, ComponentName, ComponentValue,
                SearchPriority, ResultType, SectionPath
            )
            SELECT
                i.IndustryId,
                i.IndustryName,
                ISNULL(i.Intro, N''),
                N'',
                ISNULL(i.Content, N''),
                i.Industry_pagename,
                ISNULL(i.[Status], 1),
                ISNULL(i.CreatedDate, GETDATE()),
                N'/IndustriesInside/' + i.Industry_pagename,
                N'Industries',
                NULL,
                CASE
                    WHEN LOWER(i.IndustryName) = LOWER(@RawKeyword) THEN 0
                    WHEN i.IndustryName LIKE @Like OR ISNULL(i.Industry_pagename, N'') LIKE @Like THEN 1
                    ELSE 2
                END,
                N'Industry',
                N'Businesses › Industries'
            FROM dbo.Industry_Master i
            WHERE ISNULL(i.[Status], 1) = 1
              AND NULLIF(LTRIM(RTRIM(i.Industry_pagename)), N'') IS NOT NULL
              AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = 1)
              AND
              (
                  i.IndustryName LIKE @Like
                  OR ISNULL(i.Industry_pagename, N'') LIKE @Like
                  OR ISNULL(i.Intro, N'') LIKE @Like
                  OR ISNULL(i.Content, N'') LIKE @Like
              );
            END TRY
            BEGIN CATCH
            END CATCH
        END
    END

    ;WITH ResultData AS
    (
        SELECT
            ROW_NUMBER() OVER
            (
                PARTITION BY PageUrl
                ORDER BY SearchPriority, cont_publishdate DESC
            ) AS DuplicateRowNo,
            *
        FROM #SearchData
    ),
    FinalData AS
    (
        SELECT
            ROW_NUMBER() OVER
            (
                ORDER BY SearchPriority, cont_publishdate DESC
            ) AS RowNo,
            *
        FROM ResultData
        WHERE DuplicateRowNo = 1
    )
    SELECT *
    FROM FinalData
    WHERE RowNo > ((@Page - 1) * @PageCount)
      AND RowNo <= (@Page * @PageCount);

    SELECT COUNT(*) AS TotalCount
    FROM
    (
        SELECT PageUrl
        FROM #SearchData
        GROUP BY PageUrl
    ) A;
END
GO

/* ============================================================
   Chip with no keyword: sections, articles, and for Businesses
   the products and industries in that section.
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_Search_BrowseSection
(
    @Category NVARCHAR(100),
    @Take     INT = 8
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Take < 1 SET @Take = 8;
    IF @Take > 20 SET @Take = 20;

    IF @Category IS NULL OR LTRIM(RTRIM(@Category)) IN (N'', N'All')
        SET @Category = N'All';

    IF OBJECT_ID(N'tempdb..#Browse') IS NOT NULL
        DROP TABLE #Browse;

    CREATE TABLE #Browse
    (
        cont_title   NVARCHAR(500),
        PageUrl      NVARCHAR(1000),
        ResultType   NVARCHAR(50),
        SectionPath  NVARCHAR(500),
        cont_intro   NVARCHAR(MAX),
        SortGroup    INT,
        SortKey      INT
    );

    IF @Category <> N'All'
    BEGIN
        CREATE TABLE #SearchSlug (Slug NVARCHAR(100) NOT NULL PRIMARY KEY);
        EXEC dbo.sp_Search_FillCategorySlugs @Category;

        INSERT INTO #Browse (cont_title, PageUrl, ResultType, SectionPath, cont_intro, SortGroup, SortKey)
        SELECT
            LEFT(ISNULL(c.cont_title, N''), 500),
            LEFT(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N''), 1000),
            CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.content ch
                    WHERE ch.cont_parent_id = c.cont_id
                      AND ch.cont_status = 2
                )
                THEN N'Section'
                ELSE N'Article'
            END,
            NULL,
            ISNULL(c.cont_intro, N''),
            CASE
                WHEN EXISTS
                (
                    SELECT 1
                    FROM dbo.content ch
                    WHERE ch.cont_parent_id = c.cont_id
                      AND ch.cont_status = 2
                )
                THEN 1
                ELSE 4
            END,
            ROW_NUMBER() OVER (ORDER BY c.cont_publishdate DESC, c.cont_id DESC)
        FROM dbo.content c
        WHERE c.cont_status = 2
          AND NULLIF(LTRIM(RTRIM(ISNULL(c.cont_title, N''))), N'') IS NOT NULL
          AND EXISTS
          (
              SELECT 1
              FROM #SearchSlug s
              WHERE N'/' + LOWER(REPLACE(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N''), N'\', N'/')) + N'/'
                        LIKE N'%/' + s.Slug + N'/%'
                 OR LOWER(ISNULL(c.cont_pagename, N'')) = s.Slug
                 OR LOWER(ISNULL(c.cont_pagename, N'')) LIKE s.Slug + N'/%'
          )
          /* Leave out the section landing page itself; list what sits under it. */
          AND NOT
          (
              EXISTS
              (
                  SELECT 1
                  FROM #SearchSlug s
                  WHERE LOWER(ISNULL(c.cont_pagename, N'')) = s.Slug
              )
              AND NOT EXISTS
              (
                  SELECT 1
                  FROM #SearchSlug s
                  WHERE N'/' + LOWER(REPLACE(ISNULL(dbo.GetCanonicalUrl(c.cont_id), N''), N'\', N'/')) + N'/'
                            LIKE N'%/' + s.Slug + N'/%/%'
              )
          );

        IF @Category = N'Businesses' AND OBJECT_ID(N'dbo.Industry_Master', N'U') IS NOT NULL
        BEGIN
            BEGIN TRY
            INSERT INTO #Browse (cont_title, PageUrl, ResultType, SectionPath, cont_intro, SortGroup, SortKey)
            SELECT
                LEFT(i.IndustryName, 500),
                N'/IndustriesInside/' + i.Industry_pagename,
                N'Industry',
                N'Businesses › Industries',
                ISNULL(i.Intro, N''),
                2,
                ROW_NUMBER() OVER (ORDER BY i.DisplayOrder, i.IndustryName)
            FROM dbo.Industry_Master i
            WHERE ISNULL(i.[Status], 1) = 1
              AND NULLIF(LTRIM(RTRIM(i.Industry_pagename)), N'') IS NOT NULL
              AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = 1);
            END TRY
            BEGIN CATCH
            END CATCH
        END

        IF @Category = N'Businesses' AND OBJECT_ID(N'dbo.Product_Master', N'U') IS NOT NULL
        BEGIN
            BEGIN TRY
            INSERT INTO #Browse (cont_title, PageUrl, ResultType, SectionPath, cont_intro, SortGroup, SortKey)
            SELECT
                LEFT(p.ProductName, 500),
                N'/Products/' + p.Product_pagename,
                N'Product',
                N'Businesses › Products',
                ISNULL(p.Intro, N''),
                3,
                ROW_NUMBER() OVER (ORDER BY p.DisplayOrder, p.ProductName)
            FROM dbo.Product_Master p
            WHERE ISNULL(p.[status], 0) = 2
              AND NULLIF(LTRIM(RTRIM(p.Product_pagename)), N'') IS NOT NULL
              AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = 1);
            END TRY
            BEGIN CATCH
            END CATCH
        END
    END

    ;WITH ranked AS
    (
        SELECT
            cont_title,
            PageUrl,
            ResultType,
            SectionPath,
            cont_intro,
            SortGroup,
            SortKey,
            ROW_NUMBER() OVER (PARTITION BY ResultType ORDER BY SortKey) AS TypeRank
        FROM #Browse
        WHERE NULLIF(LTRIM(RTRIM(ISNULL(PageUrl, N''))), N'') IS NOT NULL
    )
    SELECT TOP (@Take)
        cont_title,
        PageUrl,
        ResultType,
        SectionPath,
        cont_intro
    FROM ranked
    ORDER BY TypeRank, SortGroup, SortKey;

    SELECT COUNT(*) AS TotalCount
    FROM
    (
        SELECT PageUrl
        FROM #Browse
        WHERE NULLIF(LTRIM(RTRIM(ISNULL(PageUrl, N''))), N'') IS NOT NULL
        GROUP BY PageUrl
    ) A;
END
GO

/* ============================================================
   Save a keyword search, or a section / article / product that was opened
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_Search_Log
(
    @Keyword     NVARCHAR(200),
    @Category    NVARCHAR(100) = N'All',
    @ItemType    NVARCHAR(50)  = N'Keyword',
    @ItemTitle   NVARCHAR(500) = NULL,
    @ItemUrl     NVARCHAR(1000) = NULL,
    @SectionName NVARCHAR(300) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Keyword = LEFT(LTRIM(RTRIM(ISNULL(@Keyword, N''))), 200);
    SET @Category = LEFT(LTRIM(RTRIM(ISNULL(@Category, N'All'))), 100);
    SET @ItemType = LEFT(LTRIM(RTRIM(ISNULL(@ItemType, N'Keyword'))), 50);
    SET @ItemUrl = LEFT(LTRIM(RTRIM(ISNULL(@ItemUrl, N''))), 1000);
    SET @ItemTitle = NULLIF(LEFT(LTRIM(RTRIM(ISNULL(@ItemTitle, N''))), 500), N'');
    SET @SectionName = NULLIF(LEFT(LTRIM(RTRIM(ISNULL(@SectionName, N''))), 300), N'');

    IF @Category = N'' SET @Category = N'All';
    IF @Keyword = N'' RETURN;

    IF @ItemType NOT IN (N'Keyword', N'Page', N'Section', N'Article', N'Product', N'Industry', N'PDF')
        SET @ItemType = N'Page';

    DECLARE @MatchKey VARBINARY(32) = CONVERT(VARBINARY(32), HASHBYTES(
        'SHA2_256',
        CONCAT(LOWER(@Keyword), N'|', LOWER(@Category), N'|', LOWER(@ItemType), N'|', LOWER(@ItemUrl))
    ));

    UPDATE dbo.Search_Popular
    SET SearchCount = SearchCount + 1,
        LastSearched = GETDATE(),
        ItemTitle = COALESCE(@ItemTitle, ItemTitle),
        SectionName = COALESCE(@SectionName, SectionName)
    WHERE MatchKey = @MatchKey;

    IF @@ROWCOUNT = 0
    BEGIN
        BEGIN TRY
            INSERT INTO dbo.Search_Popular
                (Keyword, Category, ItemType, ItemTitle, ItemUrl, SectionName, SearchCount, LastSearched, CreatedDate, MatchKey)
            VALUES
                (@Keyword, @Category, @ItemType, @ItemTitle, @ItemUrl, @SectionName, 1, GETDATE(), GETDATE(), @MatchKey);
        END TRY
        BEGIN CATCH
            IF ERROR_NUMBER() IN (2601, 2627)
            BEGIN
                UPDATE dbo.Search_Popular
                SET SearchCount = SearchCount + 1,
                    LastSearched = GETDATE(),
                    ItemTitle = COALESCE(@ItemTitle, ItemTitle),
                    SectionName = COALESCE(@SectionName, SectionName)
                WHERE MatchKey = @MatchKey;
            END
            ELSE
                THROW;
        END CATCH
    END
END
GO

/* ============================================================
   Popular keywords, then the sections and articles people open
   ============================================================ */
CREATE OR ALTER PROCEDURE dbo.sp_Search_GetPopular
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP (8)
        Keyword
    FROM dbo.Search_Popular
    WHERE ItemType = N'Keyword'
    ORDER BY SearchCount DESC, LastSearched DESC;

    SELECT TOP (4)
        ItemTitle,
        ItemUrl,
        Category,
        ItemType,
        SectionName
    FROM dbo.Search_Popular
    WHERE ItemType <> N'Keyword'
      AND ItemUrl <> N''
    ORDER BY SearchCount DESC, LastSearched DESC;
END
GO

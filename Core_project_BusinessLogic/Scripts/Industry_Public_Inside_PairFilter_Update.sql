/*
  Industry inside products — filter by exact industry + subcategory pair.

  CMS tags products as pairs (e.g. Adhesives+Solvent, Electronics+Intermediate)
  in product_subcategory_Mapping (IndustryId + Category_Master_Id).

  Before: product showed under a subcategory tab if it had that subcategory
  under ANY industry (shared Category_Master_Id across industries).
  After: product shows only when tagged for THIS industry AND THIS subcategory.

  Run on SQL Server, then hard-refresh /IndustriesInside/{page}.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.product_subcategory_Mapping', 'IndustryId') IS NULL
    ALTER TABLE dbo.product_subcategory_Mapping ADD IndustryId INT NULL;
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Public_GetInside
    @PageName NVARCHAR(250),
    @LanguageId INT = 1,
    @CategoryId INT = NULL,
    @Page INT = 1,
    @PageSize INT = 6
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 6;
    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @IndustryId INT = NULL;
    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT TOP (1)
        i.IndustryId,
        i.IndustryName,
        i.Industry_pagename,
        i.Intro,
        mb.file_path AS BannerUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(i.Banner_Image_Alt)), N''), i.IndustryName) AS BannerAlt,
        i.Window_Title,
        i.Meta_Title,
        i.Meta_Description
    FROM dbo.Industry_Master i
    LEFT JOIN dbo.media mb ON mb.ID = i.Banner_Image_media_id
    WHERE ISNULL(i.[Status], 1) = 1
      AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
      AND LOWER(LTRIM(RTRIM(i.Industry_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
    ORDER BY i.DisplayOrder, i.IndustryId;

    SET @IndustryId = (
        SELECT TOP (1) IndustryId
        FROM dbo.Industry_Master
        WHERE ISNULL([Status], 1) = 1
          AND (Language_Master_Id IS NULL OR Language_Master_Id = @LanguageId)
          AND LOWER(LTRIM(RTRIM(Industry_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
        ORDER BY DisplayOrder, IndustryId
    );

    -- Subcategory tabs for this industry only (Industry_Subcategory_Mapping)
    SELECT
        c.SubcategoryId AS Id,
        c.SubcategoryName AS Name
    FROM dbo.Industry_Subcategory_Mapping m
    INNER JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = m.Category_Master_Id
    WHERE m.IndustryId = @IndustryId
      AND ISNULL(c.[Status], 1) = 1
    ORDER BY m.DisplayOrder, c.SubcategoryName;

    IF (@CategoryId IS NULL OR @CategoryId = 0) AND @IndustryId IS NOT NULL
    BEGIN
        SET @CategoryId = (
            SELECT TOP (1) c.SubcategoryId
            FROM dbo.Industry_Subcategory_Mapping m
            INNER JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = m.Category_Master_Id
            WHERE m.IndustryId = @IndustryId
              AND ISNULL(c.[Status], 1) = 1
            ORDER BY m.DisplayOrder, c.SubcategoryName
        );
    END

    -- Invalid category for this industry → fall back to first mapped subcategory
    IF @CategoryId IS NOT NULL AND @CategoryId <> 0 AND @IndustryId IS NOT NULL
       AND NOT EXISTS (
            SELECT 1
            FROM dbo.Industry_Subcategory_Mapping m
            WHERE m.IndustryId = @IndustryId
              AND m.Category_Master_Id = @CategoryId
       )
    BEGIN
        SET @CategoryId = (
            SELECT TOP (1) c.SubcategoryId
            FROM dbo.Industry_Subcategory_Mapping m
            INNER JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = m.Category_Master_Id
            WHERE m.IndustryId = @IndustryId
              AND ISNULL(c.[Status], 1) = 1
            ORDER BY m.DisplayOrder, c.SubcategoryName
        );
    END

    DECLARE @Paged TABLE (ProductId INT NOT NULL PRIMARY KEY);

    INSERT INTO @Paged (ProductId)
    SELECT p.ProductId
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND @CategoryId IS NOT NULL
      AND @CategoryId <> 0
      AND EXISTS (
            SELECT 1
            FROM dbo.product_subcategory_Mapping psm
            WHERE psm.ProductId = p.ProductId
              AND psm.Category_Master_Id = @CategoryId
              AND (
                    -- Exact CMS pair: this industry + this subcategory
                    psm.IndustryId = @IndustryId
                    OR (
                        -- Legacy rows (IndustryId NULL) only if product has no pair rows yet
                        psm.IndustryId IS NULL
                        AND NOT EXISTS (
                            SELECT 1
                            FROM dbo.product_subcategory_Mapping px
                            WHERE px.ProductId = p.ProductId
                              AND px.IndustryId IS NOT NULL
                        )
                    )
                  )
      )
    ORDER BY p.DisplayOrder, p.ProductName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        p.Intro,
        COALESCE(NULLIF(LTRIM(RTRIM(mt.file_path)), N''), mb.file_path) AS ThumbnailUrl,
        CASE
            WHEN NULLIF(LTRIM(RTRIM(mt.file_path)), N'') IS NOT NULL
                THEN ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName)
            ELSE ISNULL(NULLIF(LTRIM(RTRIM(p.Banner_Image_Alt)), N''), p.ProductName)
        END AS ThumbnailAlt,
        mb.file_path AS BannerUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Banner_Image_Alt)), N''), p.ProductName) AS BannerAlt
    FROM @Paged x
    INNER JOIN dbo.Product_Master p ON p.ProductId = x.ProductId
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    ORDER BY p.DisplayOrder, p.ProductName;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND @CategoryId IS NOT NULL
      AND @CategoryId <> 0
      AND EXISTS (
            SELECT 1
            FROM dbo.product_subcategory_Mapping psm
            WHERE psm.ProductId = p.ProductId
              AND psm.Category_Master_Id = @CategoryId
              AND (
                    psm.IndustryId = @IndustryId
                    OR (
                        psm.IndustryId IS NULL
                        AND NOT EXISTS (
                            SELECT 1
                            FROM dbo.product_subcategory_Mapping px
                            WHERE px.ProductId = p.ProductId
                              AND px.IndustryId IS NOT NULL
                        )
                    )
                  )
      );

    SELECT
        pam.ProductId,
        am.ApplicationId,
        am.ApplicationName
    FROM dbo.product_application_Mapping pam
    INNER JOIN dbo.Application_Master am ON am.ApplicationId = pam.ApplicationId
    INNER JOIN @Paged x ON x.ProductId = pam.ProductId
    WHERE ISNULL(am.[Status], 1) = 1
    ORDER BY am.DisplayOrder, am.ApplicationName;

    SELECT ISNULL(@CategoryId, 0) AS ActiveCategoryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Public_GetInsideProducts
    @IndustryId INT,
    @CategoryId INT = NULL,
    @Page INT = 1,
    @PageSize INT = 6,
    @LanguageId INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 6;
    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @CategoryOk BIT = 1;

    -- Tab AJAX must pass a category that belongs to this industry
    IF @CategoryId IS NULL OR @CategoryId = 0
        SET @CategoryOk = 0;
    ELSE IF @IndustryId IS NULL OR @IndustryId = 0
        SET @CategoryOk = 0;
    ELSE IF NOT EXISTS (
            SELECT 1
            FROM dbo.Industry_Subcategory_Mapping m
            WHERE m.IndustryId = @IndustryId
              AND m.Category_Master_Id = @CategoryId
       )
        SET @CategoryOk = 0;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    DECLARE @Paged TABLE (ProductId INT NOT NULL PRIMARY KEY);

    INSERT INTO @Paged (ProductId)
    SELECT p.ProductId
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE @CategoryOk = 1
      AND ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND EXISTS (
            SELECT 1
            FROM dbo.product_subcategory_Mapping psm
            WHERE psm.ProductId = p.ProductId
              AND psm.Category_Master_Id = @CategoryId
              AND (
                    psm.IndustryId = @IndustryId
                    OR (
                        psm.IndustryId IS NULL
                        AND NOT EXISTS (
                            SELECT 1
                            FROM dbo.product_subcategory_Mapping px
                            WHERE px.ProductId = p.ProductId
                              AND px.IndustryId IS NOT NULL
                        )
                    )
                  )
      )
    ORDER BY p.DisplayOrder, p.ProductName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        p.Intro,
        COALESCE(NULLIF(LTRIM(RTRIM(mt.file_path)), N''), mb.file_path) AS ThumbnailUrl,
        CASE
            WHEN NULLIF(LTRIM(RTRIM(mt.file_path)), N'') IS NOT NULL
                THEN ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName)
            ELSE ISNULL(NULLIF(LTRIM(RTRIM(p.Banner_Image_Alt)), N''), p.ProductName)
        END AS ThumbnailAlt,
        mb.file_path AS BannerUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Banner_Image_Alt)), N''), p.ProductName) AS BannerAlt
    FROM @Paged x
    INNER JOIN dbo.Product_Master p ON p.ProductId = x.ProductId
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    ORDER BY p.DisplayOrder, p.ProductName;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE @CategoryOk = 1
      AND ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND EXISTS (
            SELECT 1
            FROM dbo.product_subcategory_Mapping psm
            WHERE psm.ProductId = p.ProductId
              AND psm.Category_Master_Id = @CategoryId
              AND (
                    psm.IndustryId = @IndustryId
                    OR (
                        psm.IndustryId IS NULL
                        AND NOT EXISTS (
                            SELECT 1
                            FROM dbo.product_subcategory_Mapping px
                            WHERE px.ProductId = p.ProductId
                              AND px.IndustryId IS NOT NULL
                        )
                    )
                  )
      );

    SELECT
        pam.ProductId,
        am.ApplicationId,
        am.ApplicationName
    FROM dbo.product_application_Mapping pam
    INNER JOIN dbo.Application_Master am ON am.ApplicationId = pam.ApplicationId
    INNER JOIN @Paged x ON x.ProductId = pam.ProductId
    WHERE ISNULL(am.[Status], 1) = 1
    ORDER BY am.DisplayOrder, am.ApplicationName;
END
GO

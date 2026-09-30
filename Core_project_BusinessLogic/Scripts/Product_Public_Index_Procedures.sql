/*
  Run this file for the public Products listing page.
  Banner / title / intro come from dbo.content (pagename = products).
  Cards come from Product_Master.
  Filters are multi-select Industry / Application / Category with product counts.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Product_Public_GetIndex
    @PageName NVARCHAR(2000) = NULL,
    @LanguageId INT = 1,
    @IndustryIds NVARCHAR(MAX) = NULL,
    @ApplicationIds NVARCHAR(MAX) = NULL,
    @CategoryIds NVARCHAR(MAX) = NULL,
    @Page INT = 1,
    @PageSize INT = 9
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 9;
    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    DECLARE @Ind TABLE (Id INT NOT NULL PRIMARY KEY);
    DECLARE @App TABLE (Id INT NOT NULL PRIMARY KEY);
    DECLARE @Cat TABLE (Id INT NOT NULL PRIMARY KEY);

    INSERT INTO @Ind (Id)
    SELECT DISTINCT TRY_CONVERT(INT, LTRIM(RTRIM(value)))
    FROM STRING_SPLIT(ISNULL(@IndustryIds, N''), N',')
    WHERE TRY_CONVERT(INT, LTRIM(RTRIM(value))) > 0;

    INSERT INTO @App (Id)
    SELECT DISTINCT TRY_CONVERT(INT, LTRIM(RTRIM(value)))
    FROM STRING_SPLIT(ISNULL(@ApplicationIds, N''), N',')
    WHERE TRY_CONVERT(INT, LTRIM(RTRIM(value))) > 0;

    INSERT INTO @Cat (Id)
    SELECT DISTINCT TRY_CONVERT(INT, LTRIM(RTRIM(value)))
    FROM STRING_SPLIT(ISNULL(@CategoryIds, N''), N',')
    WHERE TRY_CONVERT(INT, LTRIM(RTRIM(value))) > 0;

    SELECT TOP (1)
        c.cont_id,
        c.cont_title,
        c.cont_intro,
        c.cont_breadcrumb_title,
        c.cont_window_title,
        c.cont_metatag,
        c.cont_metadesc,
        c.cont_pagename,
        c.Masthead_alt_text,
        md.file_path AS MastheadImage,
        COALESCE(mm.file_path, md.file_path) AS MobileMastheadImage
    FROM dbo.content c
    LEFT JOIN dbo.media md ON md.ID = c.Masthead_image_Media_id
    LEFT JOIN dbo.media mm ON mm.ID = c.Mobile_Masthead_image_Media_id
    WHERE @PageName IS NOT NULL
      AND LTRIM(RTRIM(@PageName)) <> N''
      AND LOWER(LTRIM(RTRIM(c.cont_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
      AND ISNULL(c.cont_status, 0) = 2
      AND (c.language_master_id IS NULL OR c.language_master_id = @LanguageId)
    ORDER BY c.cont_id DESC;

    ;WITH matched AS
    (
        SELECT
            p.ProductId,
            p.ProductName,
            p.Product_pagename,
            p.DisplayOrder,
            COALESCE(mt.file_path, mb.file_path) AS ThumbnailUrl,
            ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName) AS ThumbnailAlt
        FROM dbo.Product_Master p
        LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
        LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
        WHERE ISNULL(p.[status], 1) = 1
          AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
          AND (
                NOT EXISTS (SELECT 1 FROM @Ind)
                OR EXISTS (
                    SELECT 1
                    FROM dbo.product_Industry_Mapping pim
                    INNER JOIN @Ind x ON x.Id = pim.IndustryId
                    WHERE pim.ProductId = p.ProductId
                )
          )
          AND (
                NOT EXISTS (SELECT 1 FROM @App)
                OR EXISTS (
                    SELECT 1
                    FROM dbo.product_application_Mapping pam
                    INNER JOIN @App x ON x.Id = pam.ApplicationId
                    WHERE pam.ProductId = p.ProductId
                )
          )
          AND (
                NOT EXISTS (SELECT 1 FROM @Cat)
                OR EXISTS (
                    SELECT 1
                    FROM dbo.product_subcategory_Mapping psm
                    INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                    WHERE psm.ProductId = p.ProductId
                )
          )
    )
    SELECT
        ProductId,
        ProductName,
        Product_pagename,
        ThumbnailUrl,
        ThumbnailAlt,
        DisplayOrder
    FROM matched
    ORDER BY DisplayOrder, ProductName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    WHERE ISNULL(p.[status], 1) = 1
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
      )
      AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
      )
      AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
      );

    SELECT
        i.IndustryId AS Id,
        i.IndustryName AS Name,
        COUNT(DISTINCT p.ProductId) AS [Count]
    FROM dbo.Industry_Master i
    LEFT JOIN dbo.product_Industry_Mapping pim ON pim.IndustryId = i.IndustryId
    LEFT JOIN dbo.Product_Master p
        ON p.ProductId = pim.ProductId
       AND ISNULL(p.[status], 1) = 1
       AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
       AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
       )
       AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
       )
    WHERE ISNULL(i.[Status], 1) = 1
      AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
    GROUP BY i.IndustryId, i.IndustryName, i.DisplayOrder
    ORDER BY i.DisplayOrder, i.IndustryName;

    SELECT
        a.ApplicationId AS Id,
        a.ApplicationName AS Name,
        COUNT(DISTINCT p.ProductId) AS [Count]
    FROM dbo.Application_Master a
    LEFT JOIN dbo.product_application_Mapping pam ON pam.ApplicationId = a.ApplicationId
    LEFT JOIN dbo.Product_Master p
        ON p.ProductId = pam.ProductId
       AND ISNULL(p.[status], 1) = 1
       AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
       AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
       )
       AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
       )
    WHERE ISNULL(a.[Status], 1) = 1
      AND (a.Language_Master_Id IS NULL OR a.Language_Master_Id = @LanguageId)
    GROUP BY a.ApplicationId, a.ApplicationName, a.DisplayOrder
    ORDER BY a.DisplayOrder, a.ApplicationName;

    SELECT
        c.SubcategoryId AS Id,
        c.SubcategoryName AS Name,
        COUNT(DISTINCT p.ProductId) AS [Count]
    FROM dbo.Industry_Subcategory_Master c
    LEFT JOIN dbo.product_subcategory_Mapping psm ON psm.Category_Master_Id = c.SubcategoryId
    LEFT JOIN dbo.Product_Master p
        ON p.ProductId = psm.ProductId
       AND ISNULL(p.[status], 1) = 1
       AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
       AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
       )
       AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
       )
    WHERE ISNULL(c.[Status], 1) = 1
      AND (c.Language_Master_Id IS NULL OR c.Language_Master_Id = @LanguageId)
    GROUP BY c.SubcategoryId, c.SubcategoryName, c.DisplayOrder
    ORDER BY c.DisplayOrder, c.SubcategoryName;

    SELECT
        pim.ProductId,
        i.IndustryName
    FROM dbo.product_Industry_Mapping pim
    INNER JOIN dbo.Industry_Master i ON i.IndustryId = pim.IndustryId
    INNER JOIN
    (
        SELECT p.ProductId
        FROM dbo.Product_Master p
        WHERE ISNULL(p.[status], 1) = 1
          AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
          AND (
                NOT EXISTS (SELECT 1 FROM @Ind)
                OR EXISTS (
                    SELECT 1 FROM dbo.product_Industry_Mapping m
                    INNER JOIN @Ind x ON x.Id = m.IndustryId
                    WHERE m.ProductId = p.ProductId
                )
          )
          AND (
                NOT EXISTS (SELECT 1 FROM @App)
                OR EXISTS (
                    SELECT 1 FROM dbo.product_application_Mapping m
                    INNER JOIN @App x ON x.Id = m.ApplicationId
                    WHERE m.ProductId = p.ProductId
                )
          )
          AND (
                NOT EXISTS (SELECT 1 FROM @Cat)
                OR EXISTS (
                    SELECT 1 FROM dbo.product_subcategory_Mapping m
                    INNER JOIN @Cat x ON x.Id = m.Category_Master_Id
                    WHERE m.ProductId = p.ProductId
                )
          )
        ORDER BY p.DisplayOrder, p.ProductName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
    ) pg ON pg.ProductId = pim.ProductId
    WHERE ISNULL(i.[Status], 1) = 1
    ORDER BY pim.DisplayOrder, i.IndustryName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Public_GetPaged
    @IndustryIds NVARCHAR(MAX) = NULL,
    @ApplicationIds NVARCHAR(MAX) = NULL,
    @CategoryIds NVARCHAR(MAX) = NULL,
    @Page INT = 1,
    @PageSize INT = 9,
    @LanguageId INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 9;
    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    DECLARE @Ind TABLE (Id INT NOT NULL PRIMARY KEY);
    DECLARE @App TABLE (Id INT NOT NULL PRIMARY KEY);
    DECLARE @Cat TABLE (Id INT NOT NULL PRIMARY KEY);

    INSERT INTO @Ind (Id)
    SELECT DISTINCT TRY_CONVERT(INT, LTRIM(RTRIM(value)))
    FROM STRING_SPLIT(ISNULL(@IndustryIds, N''), N',')
    WHERE TRY_CONVERT(INT, LTRIM(RTRIM(value))) > 0;

    INSERT INTO @App (Id)
    SELECT DISTINCT TRY_CONVERT(INT, LTRIM(RTRIM(value)))
    FROM STRING_SPLIT(ISNULL(@ApplicationIds, N''), N',')
    WHERE TRY_CONVERT(INT, LTRIM(RTRIM(value))) > 0;

    INSERT INTO @Cat (Id)
    SELECT DISTINCT TRY_CONVERT(INT, LTRIM(RTRIM(value)))
    FROM STRING_SPLIT(ISNULL(@CategoryIds, N''), N',')
    WHERE TRY_CONVERT(INT, LTRIM(RTRIM(value))) > 0;

    SELECT
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        COALESCE(mt.file_path, mb.file_path) AS ThumbnailUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName) AS ThumbnailAlt,
        p.DisplayOrder
    FROM dbo.Product_Master p
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    WHERE ISNULL(p.[status], 1) = 1
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
      )
      AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
      )
      AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
      )
    ORDER BY p.DisplayOrder, p.ProductName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    WHERE ISNULL(p.[status], 1) = 1
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
      )
      AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
      )
      AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
      );

    SELECT
        pim.ProductId,
        i.IndustryName
    FROM dbo.product_Industry_Mapping pim
    INNER JOIN dbo.Industry_Master i ON i.IndustryId = pim.IndustryId
    INNER JOIN
    (
        SELECT p.ProductId
        FROM dbo.Product_Master p
        WHERE ISNULL(p.[status], 1) = 1
          AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
          AND (
                NOT EXISTS (SELECT 1 FROM @Ind)
                OR EXISTS (
                    SELECT 1 FROM dbo.product_Industry_Mapping m
                    INNER JOIN @Ind x ON x.Id = m.IndustryId
                    WHERE m.ProductId = p.ProductId
                )
          )
          AND (
                NOT EXISTS (SELECT 1 FROM @App)
                OR EXISTS (
                    SELECT 1 FROM dbo.product_application_Mapping m
                    INNER JOIN @App x ON x.Id = m.ApplicationId
                    WHERE m.ProductId = p.ProductId
                )
          )
          AND (
                NOT EXISTS (SELECT 1 FROM @Cat)
                OR EXISTS (
                    SELECT 1 FROM dbo.product_subcategory_Mapping m
                    INNER JOIN @Cat x ON x.Id = m.Category_Master_Id
                    WHERE m.ProductId = p.ProductId
                )
          )
        ORDER BY p.DisplayOrder, p.ProductName
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
    ) pg ON pg.ProductId = pim.ProductId
    WHERE ISNULL(i.[Status], 1) = 1
    ORDER BY pim.DisplayOrder, i.IndustryName;

    SELECT
        i.IndustryId AS Id,
        i.IndustryName AS Name,
        COUNT(DISTINCT p.ProductId) AS [Count]
    FROM dbo.Industry_Master i
    LEFT JOIN dbo.product_Industry_Mapping pim ON pim.IndustryId = i.IndustryId
    LEFT JOIN dbo.Product_Master p
        ON p.ProductId = pim.ProductId
       AND ISNULL(p.[status], 1) = 1
       AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
       AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
       )
       AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
       )
    WHERE ISNULL(i.[Status], 1) = 1
      AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
    GROUP BY i.IndustryId, i.IndustryName, i.DisplayOrder
    ORDER BY i.DisplayOrder, i.IndustryName;

    SELECT
        a.ApplicationId AS Id,
        a.ApplicationName AS Name,
        COUNT(DISTINCT p.ProductId) AS [Count]
    FROM dbo.Application_Master a
    LEFT JOIN dbo.product_application_Mapping pam ON pam.ApplicationId = a.ApplicationId
    LEFT JOIN dbo.Product_Master p
        ON p.ProductId = pam.ProductId
       AND ISNULL(p.[status], 1) = 1
       AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
       AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
       )
       AND (
            NOT EXISTS (SELECT 1 FROM @Cat)
            OR EXISTS (
                SELECT 1 FROM dbo.product_subcategory_Mapping psm
                INNER JOIN @Cat x ON x.Id = psm.Category_Master_Id
                WHERE psm.ProductId = p.ProductId
            )
       )
    WHERE ISNULL(a.[Status], 1) = 1
      AND (a.Language_Master_Id IS NULL OR a.Language_Master_Id = @LanguageId)
    GROUP BY a.ApplicationId, a.ApplicationName, a.DisplayOrder
    ORDER BY a.DisplayOrder, a.ApplicationName;

    SELECT
        c.SubcategoryId AS Id,
        c.SubcategoryName AS Name,
        COUNT(DISTINCT p.ProductId) AS [Count]
    FROM dbo.Industry_Subcategory_Master c
    LEFT JOIN dbo.product_subcategory_Mapping psm ON psm.Category_Master_Id = c.SubcategoryId
    LEFT JOIN dbo.Product_Master p
        ON p.ProductId = psm.ProductId
       AND ISNULL(p.[status], 1) = 1
       AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
       AND (
            NOT EXISTS (SELECT 1 FROM @Ind)
            OR EXISTS (
                SELECT 1 FROM dbo.product_Industry_Mapping pim
                INNER JOIN @Ind x ON x.Id = pim.IndustryId
                WHERE pim.ProductId = p.ProductId
            )
       )
       AND (
            NOT EXISTS (SELECT 1 FROM @App)
            OR EXISTS (
                SELECT 1 FROM dbo.product_application_Mapping pam
                INNER JOIN @App x ON x.Id = pam.ApplicationId
                WHERE pam.ProductId = p.ProductId
            )
       )
    WHERE ISNULL(c.[Status], 1) = 1
      AND (c.Language_Master_Id IS NULL OR c.Language_Master_Id = @LanguageId)
    GROUP BY c.SubcategoryId, c.SubcategoryName, c.DisplayOrder
    ORDER BY c.DisplayOrder, c.SubcategoryName;
END
GO

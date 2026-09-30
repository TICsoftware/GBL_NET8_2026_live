/*
  Run this file for the public Industries listing page.
  Content banner / title / intro / breadcrumb come from dbo.content.
  Industry cards come from dbo.Industry_Master.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Public_GetIndex
    @PageName NVARCHAR(2000) = NULL,
    @LanguageId INT = 1,
    @IndustryId INT = NULL,
    @ApplicationId INT = NULL,
    @CategoryId INT = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;
    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

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

    ;WITH filtered AS
    (
        SELECT
            i.IndustryId,
            i.IndustryName,
            i.Industry_pagename,
            i.Intro,
            i.DisplayOrder,
            mt.file_path AS ThumbnailUrl,
            ISNULL(NULLIF(LTRIM(RTRIM(i.Landing_Thumbnail_Image_Alt)), N''), i.IndustryName) AS ThumbnailAlt
        FROM dbo.Industry_Master i
        LEFT JOIN dbo.media mt ON mt.ID = i.Landing_Thumbnail_Image_media_id
        WHERE ISNULL(i.[Status], 1) = 1
          AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
          AND (@IndustryId IS NULL OR @IndustryId = 0 OR i.IndustryId = @IndustryId)
          AND (
                @ApplicationId IS NULL OR @ApplicationId = 0 OR EXISTS (
                    SELECT 1
                    FROM dbo.product_Industry_Mapping pim
                    INNER JOIN dbo.product_application_Mapping pam ON pam.ProductId = pim.ProductId
                    WHERE pim.IndustryId = i.IndustryId
                      AND pam.ApplicationId = @ApplicationId
                )
          )
          AND (
                @CategoryId IS NULL OR @CategoryId = 0 OR EXISTS (
                    SELECT 1
                    FROM dbo.Industry_Subcategory_Mapping ism
                    WHERE ism.IndustryId = i.IndustryId
                      AND ism.Category_Master_Id = @CategoryId
                )
          )
    )
    SELECT
        IndustryId,
        IndustryName,
        Industry_pagename,
        Intro,
        ThumbnailUrl,
        ThumbnailAlt,
        DisplayOrder
    FROM filtered
    ORDER BY DisplayOrder, IndustryName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Industry_Master i
    WHERE ISNULL(i.[Status], 1) = 1
      AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
      AND (@IndustryId IS NULL OR @IndustryId = 0 OR i.IndustryId = @IndustryId)
      AND (
            @ApplicationId IS NULL OR @ApplicationId = 0 OR EXISTS (
                SELECT 1
                FROM dbo.product_Industry_Mapping pim
                INNER JOIN dbo.product_application_Mapping pam ON pam.ProductId = pim.ProductId
                WHERE pim.IndustryId = i.IndustryId
                  AND pam.ApplicationId = @ApplicationId
            )
      )
      AND (
            @CategoryId IS NULL OR @CategoryId = 0 OR EXISTS (
                SELECT 1
                FROM dbo.Industry_Subcategory_Mapping ism
                WHERE ism.IndustryId = i.IndustryId
                  AND ism.Category_Master_Id = @CategoryId
            )
      );

    SELECT IndustryId AS Id, IndustryName AS Name
    FROM dbo.Industry_Master
    WHERE ISNULL([Status], 1) = 1
      AND (Language_Master_Id IS NULL OR Language_Master_Id = @LanguageId)
    ORDER BY DisplayOrder, IndustryName;

    SELECT ApplicationId AS Id, ApplicationName AS Name
    FROM dbo.Application_Master
    WHERE ISNULL([Status], 1) = 1
      AND (Language_Master_Id IS NULL OR Language_Master_Id = @LanguageId)
    ORDER BY DisplayOrder, ApplicationName;

    SELECT SubcategoryId AS Id, SubcategoryName AS Name
    FROM dbo.Industry_Subcategory_Master
    WHERE ISNULL([Status], 1) = 1
      AND (Language_Master_Id IS NULL OR Language_Master_Id = @LanguageId)
    ORDER BY DisplayOrder, SubcategoryName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Public_GetPaged
    @IndustryId INT = NULL,
    @ApplicationId INT = NULL,
    @CategoryId INT = NULL,
    @Page INT = 1,
    @PageSize INT = 10,
    @LanguageId INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;
    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    SELECT
        i.IndustryId,
        i.IndustryName,
        i.Industry_pagename,
        i.Intro,
        i.DisplayOrder,
        mt.file_path AS ThumbnailUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(i.Landing_Thumbnail_Image_Alt)), N''), i.IndustryName) AS ThumbnailAlt
    FROM dbo.Industry_Master i
    LEFT JOIN dbo.media mt ON mt.ID = i.Landing_Thumbnail_Image_media_id
    WHERE ISNULL(i.[Status], 1) = 1
      AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
      AND (@IndustryId IS NULL OR @IndustryId = 0 OR i.IndustryId = @IndustryId)
      AND (
            @ApplicationId IS NULL OR @ApplicationId = 0 OR EXISTS (
                SELECT 1
                FROM dbo.product_Industry_Mapping pim
                INNER JOIN dbo.product_application_Mapping pam ON pam.ProductId = pim.ProductId
                WHERE pim.IndustryId = i.IndustryId
                  AND pam.ApplicationId = @ApplicationId
            )
      )
      AND (
            @CategoryId IS NULL OR @CategoryId = 0 OR EXISTS (
                SELECT 1
                FROM dbo.Industry_Subcategory_Mapping ism
                WHERE ism.IndustryId = i.IndustryId
                  AND ism.Category_Master_Id = @CategoryId
            )
      )
    ORDER BY i.DisplayOrder, i.IndustryName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Industry_Master i
    WHERE ISNULL(i.[Status], 1) = 1
      AND (i.Language_Master_Id IS NULL OR i.Language_Master_Id = @LanguageId)
      AND (@IndustryId IS NULL OR @IndustryId = 0 OR i.IndustryId = @IndustryId)
      AND (
            @ApplicationId IS NULL OR @ApplicationId = 0 OR EXISTS (
                SELECT 1
                FROM dbo.product_Industry_Mapping pim
                INNER JOIN dbo.product_application_Mapping pam ON pam.ProductId = pim.ProductId
                WHERE pim.IndustryId = i.IndustryId
                  AND pam.ApplicationId = @ApplicationId
            )
      )
      AND (
            @CategoryId IS NULL OR @CategoryId = 0 OR EXISTS (
                SELECT 1
                FROM dbo.Industry_Subcategory_Mapping ism
                WHERE ism.IndustryId = i.IndustryId
                  AND ism.Category_Master_Id = @CategoryId
            )
      );
END
GO

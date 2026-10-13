/*
  Product Master - Save / Save and Publish
  Status: 0 = Inactive, 1 = Saved, 2 = Published

  Run this script on SQL Server.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* Existing active products -> Published so public pages keep working */
UPDATE dbo.Product_Master
SET [status] = 2
WHERE ISNULL([status], 1) = 1;
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_Insert
    @ProductName NVARCHAR(500),
    @Product_pagename NVARCHAR(300) = NULL,
    @Intro NVARCHAR(MAX) = NULL,
    @Content NVARCHAR(MAX) = NULL,
    @Technical_Overview NVARCHAR(MAX) = NULL,
    @Main_Application NVARCHAR(MAX) = NULL,
    @Thumbnail_Image_media_id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @SafetyDataSheet_media_id INT = NULL,
    @Certification_Image_media_id INT = NULL,
    @Banner_Image_Alt NVARCHAR(500) = NULL,
    @Thumbnail_Image_Alt NVARCHAR(500) = NULL,
    @Certification_Image_Alt NVARCHAR(500) = NULL,
    @Language_Master_Id INT = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Create_UserId INT = NULL,
    @NewID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewID = 0;
    IF @Status IS NULL OR @Status NOT IN (1, 2) SET @Status = 1;

    IF EXISTS (
        SELECT 1 FROM dbo.Product_Master
        WHERE LOWER(LTRIM(RTRIM(ProductName))) = LOWER(LTRIM(RTRIM(@ProductName)))
          AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0))
        THROW 50020, 'This name already exists for the selected language.', 1;

    IF @Product_pagename IS NOT NULL AND LTRIM(RTRIM(@Product_pagename)) <> N''
       AND EXISTS (
            SELECT 1 FROM dbo.Product_Master
            WHERE LOWER(LTRIM(RTRIM(Product_pagename))) = LOWER(LTRIM(RTRIM(@Product_pagename))))
        THROW 50021, 'This page name already exists.', 1;

    INSERT INTO dbo.Product_Master
    (
        ProductName, Product_pagename, Intro, Content, Technical_Overview, Main_Application,
        Thumbnail_Image_media_id, Banner_Image_media_id, SafetyDataSheet_media_id, Certification_Image_media_id,
        Banner_Image_Alt, Thumbnail_Image_Alt, Certification_Image_Alt,
        Language_Master_Id, DisplayOrder, [status], Create_UserId, CreatedDate
    )
    VALUES
    (
        @ProductName, @Product_pagename, @Intro, @Content, @Technical_Overview, @Main_Application,
        @Thumbnail_Image_media_id, @Banner_Image_media_id, @SafetyDataSheet_media_id, @Certification_Image_media_id,
        @Banner_Image_Alt, @Thumbnail_Image_Alt, @Certification_Image_Alt,
        @Language_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME()
    );
    SET @NewID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_Update
    @ProductId INT,
    @ProductName NVARCHAR(500),
    @Product_pagename NVARCHAR(300) = NULL,
    @Intro NVARCHAR(MAX) = NULL,
    @Content NVARCHAR(MAX) = NULL,
    @Technical_Overview NVARCHAR(MAX) = NULL,
    @Main_Application NVARCHAR(MAX) = NULL,
    @Thumbnail_Image_media_id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @SafetyDataSheet_media_id INT = NULL,
    @Certification_Image_media_id INT = NULL,
    @Banner_Image_Alt NVARCHAR(500) = NULL,
    @Thumbnail_Image_Alt NVARCHAR(500) = NULL,
    @Certification_Image_Alt NVARCHAR(500) = NULL,
    @Language_Master_Id INT = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Status IS NULL OR @Status NOT IN (1, 2) SET @Status = 1;

    IF EXISTS (
        SELECT 1 FROM dbo.Product_Master
        WHERE LOWER(LTRIM(RTRIM(ProductName))) = LOWER(LTRIM(RTRIM(@ProductName)))
          AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0)
          AND ProductId <> @ProductId)
        THROW 50020, 'This name already exists for the selected language.', 1;

    IF @Product_pagename IS NOT NULL AND LTRIM(RTRIM(@Product_pagename)) <> N''
       AND EXISTS (
            SELECT 1 FROM dbo.Product_Master
            WHERE LOWER(LTRIM(RTRIM(Product_pagename))) = LOWER(LTRIM(RTRIM(@Product_pagename)))
              AND ProductId <> @ProductId)
        THROW 50021, 'This page name already exists.', 1;

    UPDATE dbo.Product_Master
    SET ProductName = @ProductName,
        Product_pagename = @Product_pagename,
        Intro = @Intro,
        Content = @Content,
        Technical_Overview = @Technical_Overview,
        Main_Application = @Main_Application,
        Thumbnail_Image_media_id = @Thumbnail_Image_media_id,
        Banner_Image_media_id = @Banner_Image_media_id,
        SafetyDataSheet_media_id = @SafetyDataSheet_media_id,
        Certification_Image_media_id = @Certification_Image_media_id,
        Banner_Image_Alt = @Banner_Image_Alt,
        Thumbnail_Image_Alt = @Thumbnail_Image_Alt,
        Certification_Image_Alt = @Certification_Image_Alt,
        Language_Master_Id = @Language_Master_Id,
        DisplayOrder = @Sequence,
        [status] = @Status,
        Update_UserId = @Update_UserId,
        ModifiedDate = SYSUTCDATETIME()
    WHERE ProductId = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Public_GetInside
    @PageName NVARCHAR(300),
    @LanguageId INT = 1
AS
BEGIN
    SET NOCOUNT ON;

    IF @LanguageId IS NULL OR @LanguageId < 1 SET @LanguageId = 1;

    DECLARE @ProductId INT = NULL;

    SELECT TOP (1)
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        p.Intro,
        p.Content,
        p.Technical_Overview,
        p.Main_Application,
        mb.file_path AS BannerUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Banner_Image_Alt)), N''), p.ProductName) AS BannerAlt,
        mt.file_path AS ThumbnailUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName) AS ThumbnailAlt,
        ms.file_path AS SafetyDataSheetUrl
    FROM dbo.Product_Master p
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media ms ON ms.ID = p.SafetyDataSheet_media_id
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND LOWER(LTRIM(RTRIM(p.Product_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
    ORDER BY p.DisplayOrder, p.ProductId;

    SELECT TOP (1) @ProductId = ProductId
    FROM dbo.Product_Master
    WHERE ISNULL([status], 0) = 2
      AND (Language_Master_Id IS NULL OR Language_Master_Id = @LanguageId)
      AND LOWER(LTRIM(RTRIM(Product_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
    ORDER BY DisplayOrder, ProductId;

    SELECT
        pk.product_packaging_MasterId AS Id,
        pk.[Name],
        md.file_path AS ThumbnailUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(pk.Thumbnailimage_alt)), N''), pk.[Name]) AS ThumbnailAlt
    FROM dbo.product_packaging_Mapping m
    INNER JOIN dbo.product_packaging_Master pk ON pk.product_packaging_MasterId = m.product_packaging_MasterId
    LEFT JOIN dbo.media md ON TRY_CONVERT(INT, pk.Thumbnailimage_Id) = md.ID
    WHERE m.ProductId = @ProductId
      AND ISNULL(pk.[Status], 1) = 1
    ORDER BY m.DisplayOrder, pk.[Name];

    SELECT
        m.CertificateTitle AS Title,
        md.file_path AS Url
    FROM dbo.product_certificate_Mapping m
    LEFT JOIN dbo.media md ON md.ID = m.Certificate_media_id
    WHERE m.ProductId = @ProductId
    ORDER BY m.DisplayOrder, m.product_certificate_MappingID;

    SELECT
        c.SubcategoryId AS Id,
        c.SubcategoryName AS Name
    FROM dbo.product_subcategory_Mapping m
    INNER JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = m.Category_Master_Id
    WHERE m.ProductId = @ProductId
      AND ISNULL(c.[Status], 1) = 1
    ORDER BY m.DisplayOrder, c.SubcategoryName;
END
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
        WHERE ISNULL(p.[status], 0) = 2
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
    WHERE ISNULL(p.[status], 0) = 2
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
       AND ISNULL(p.[status], 0) = 2
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
       AND ISNULL(p.[status], 0) = 2
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
       AND ISNULL(p.[status], 0) = 2
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
        WHERE ISNULL(p.[status], 0) = 2
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
    WHERE ISNULL(p.[status], 0) = 2
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
    WHERE ISNULL(p.[status], 0) = 2
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
        WHERE ISNULL(p.[status], 0) = 2
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
       AND ISNULL(p.[status], 0) = 2
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
       AND ISNULL(p.[status], 0) = 2
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
       AND ISNULL(p.[status], 0) = 2
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

    SELECT TOP (1) @IndustryId = IndustryId
    FROM dbo.Industry_Master
    WHERE ISNULL([Status], 1) = 1
      AND (Language_Master_Id IS NULL OR Language_Master_Id = @LanguageId)
      AND LOWER(LTRIM(RTRIM(Industry_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
    ORDER BY DisplayOrder, IndustryId;

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
        SELECT TOP (1) @CategoryId = c.SubcategoryId
        FROM dbo.Industry_Subcategory_Mapping m
        INNER JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = m.Category_Master_Id
        WHERE m.IndustryId = @IndustryId
          AND ISNULL(c.[Status], 1) = 1
        ORDER BY m.DisplayOrder, c.SubcategoryName;
    END

    DECLARE @Paged TABLE (ProductId INT NOT NULL PRIMARY KEY);

    INSERT INTO @Paged (ProductId)
    SELECT p.ProductId
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            @CategoryId IS NULL OR @CategoryId = 0
            OR EXISTS (
                SELECT 1
                FROM dbo.product_subcategory_Mapping psm
                WHERE psm.ProductId = p.ProductId
                  AND psm.Category_Master_Id = @CategoryId
            )
      )
    ORDER BY p.DisplayOrder, p.ProductName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        p.Intro,
        mt.file_path AS ThumbnailUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName) AS ThumbnailAlt
    FROM @Paged x
    INNER JOIN dbo.Product_Master p ON p.ProductId = x.ProductId
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    ORDER BY p.DisplayOrder, p.ProductName;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            @CategoryId IS NULL OR @CategoryId = 0
            OR EXISTS (
                SELECT 1
                FROM dbo.product_subcategory_Mapping psm
                WHERE psm.ProductId = p.ProductId
                  AND psm.Category_Master_Id = @CategoryId
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

    DECLARE @Offset INT = (@Page - 1) * @PageSize;
    DECLARE @Paged TABLE (ProductId INT NOT NULL PRIMARY KEY);

    INSERT INTO @Paged (ProductId)
    SELECT p.ProductId
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            @CategoryId IS NULL OR @CategoryId = 0
            OR EXISTS (
                SELECT 1
                FROM dbo.product_subcategory_Mapping psm
                WHERE psm.ProductId = p.ProductId
                  AND psm.Category_Master_Id = @CategoryId
            )
      )
    ORDER BY p.DisplayOrder, p.ProductName
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        p.Intro,
        mt.file_path AS ThumbnailUrl,
        ISNULL(NULLIF(LTRIM(RTRIM(p.Thumbnail_Image_Alt)), N''), p.ProductName) AS ThumbnailAlt
    FROM @Paged x
    INNER JOIN dbo.Product_Master p ON p.ProductId = x.ProductId
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    ORDER BY p.DisplayOrder, p.ProductName;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = p.ProductId AND pim.IndustryId = @IndustryId
    WHERE ISNULL(p.[status], 0) = 2
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND (
            @CategoryId IS NULL OR @CategoryId = 0
            OR EXISTS (
                SELECT 1
                FROM dbo.product_subcategory_Mapping psm
                WHERE psm.ProductId = p.ProductId
                  AND psm.Category_Master_Id = @CategoryId
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

/*
  Run this file for the public Product inside page.
  Banner / title / intro / technical overview / SDS / main application come from Product_Master.
  Packaging comes from product_packaging_Mapping + product_packaging_Master.
  Certificates come from product_certificate_Mapping.
  Breadcrumb category comes from product_subcategory_Mapping + Industry_Subcategory_Master.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
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
    WHERE ISNULL(p.[status], 1) = 1
      AND (p.Language_Master_Id IS NULL OR p.Language_Master_Id = @LanguageId)
      AND LOWER(LTRIM(RTRIM(p.Product_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
    ORDER BY p.DisplayOrder, p.ProductId;

    SELECT TOP (1) @ProductId = ProductId
    FROM dbo.Product_Master
    WHERE ISNULL([status], 1) = 1
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

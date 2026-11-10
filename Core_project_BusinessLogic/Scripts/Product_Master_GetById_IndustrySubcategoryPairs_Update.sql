-- Updated SP only: Product_Master_GetById
-- Adds industry + subcategory pair result set for Product Master grid load.
CREATE OR ALTER PROCEDURE dbo.Product_Master_GetById
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.ProductId,
        p.ProductName,
        p.Product_pagename,
        p.DisplayOrder AS [Sequence],
        ISNULL(p.[status], 1) AS [Status],
        p.Language_Master_Id,
        lm.Language_Name AS LanguageName,
        p.Thumbnail_Image_media_id,
        p.Banner_Image_media_id,
        p.SafetyDataSheet_media_id,
        p.Certification_Image_media_id,
        mt.file_path AS Thumbnail_Image_Url,
        mb.file_path AS Banner_Image_Url,
        ms.file_path AS SafetyDataSheet_Url,
        mc.file_path AS Certification_Image_Url,
        p.Banner_Image_Alt,
        p.Thumbnail_Image_Alt,
        p.Certification_Image_Alt,
        p.Intro,
        p.Content,
        p.Technical_Overview,
        p.Main_Application
    FROM dbo.Product_Master p
    LEFT JOIN dbo.Language_Master lm ON lm.ID = p.Language_Master_Id
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    LEFT JOIN dbo.media ms ON ms.ID = p.SafetyDataSheet_media_id
    LEFT JOIN dbo.media mc ON mc.ID = p.Certification_Image_media_id
    WHERE p.ProductId = @ID;

    SELECT IndustryId AS Id
    FROM dbo.product_Industry_Mapping
    WHERE ProductId = @ID
    ORDER BY DisplayOrder, IndustryId;

    SELECT ApplicationId AS Id
    FROM dbo.product_application_Mapping
    WHERE ProductId = @ID
    ORDER BY DisplayOrder, ApplicationId;

    SELECT Category_Master_Id AS Id
    FROM dbo.product_subcategory_Mapping
    WHERE ProductId = @ID
    ORDER BY DisplayOrder, Category_Master_Id;

    SELECT product_packaging_MasterId AS Id
    FROM dbo.product_packaging_Mapping
    WHERE ProductId = @ID
    ORDER BY DisplayOrder, product_packaging_MasterId;

    SELECT
        m.product_certificate_MappingID,
        m.ProductId,
        m.CertificateTitle,
        m.Certificate_media_id,
        md.file_path AS Certificate_Url,
        m.DisplayOrder
    FROM dbo.product_certificate_Mapping m
    LEFT JOIN dbo.media md ON md.ID = m.Certificate_media_id
    WHERE m.ProductId = @ID
    ORDER BY m.DisplayOrder, m.product_certificate_MappingID;

    SELECT DISTINCT
        ism.IndustryId,
        im.IndustryName,
        psm.Category_Master_Id AS SubcategoryId,
        c.SubcategoryName
    FROM dbo.product_subcategory_Mapping psm
    INNER JOIN dbo.Industry_Subcategory_Mapping ism
        ON ism.Category_Master_Id = psm.Category_Master_Id
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = psm.ProductId
       AND pim.IndustryId = ism.IndustryId
    LEFT JOIN dbo.Industry_Master im ON im.IndustryId = ism.IndustryId
    LEFT JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = psm.Category_Master_Id
    WHERE psm.ProductId = @ID
    ORDER BY im.IndustryName, c.SubcategoryName;
END
GO

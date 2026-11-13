-- Product Master: industry + subcategory pair save/load (updated SPs only + one column).
-- Run on the manage DB used by Product_Master.

IF COL_LENGTH('dbo.product_subcategory_Mapping', 'IndustryId') IS NULL
    ALTER TABLE dbo.product_subcategory_Mapping ADD IndustryId INT NULL;
GO

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

    -- New rows (IndustryId stored) + legacy rows (IndustryId NULL) reconstructed.
    SELECT
        psm.IndustryId,
        im.IndustryName,
        psm.Category_Master_Id AS SubcategoryId,
        c.SubcategoryName
    FROM dbo.product_subcategory_Mapping psm
    LEFT JOIN dbo.Industry_Master im ON im.IndustryId = psm.IndustryId
    LEFT JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = psm.Category_Master_Id
    WHERE psm.ProductId = @ID
      AND psm.IndustryId IS NOT NULL
      AND psm.Category_Master_Id IS NOT NULL

    UNION

    SELECT
        pim.IndustryId,
        im.IndustryName,
        psm.Category_Master_Id AS SubcategoryId,
        c.SubcategoryName
    FROM dbo.product_subcategory_Mapping psm
    INNER JOIN dbo.product_Industry_Mapping pim
        ON pim.ProductId = psm.ProductId
    LEFT JOIN dbo.Industry_Subcategory_Mapping ism
        ON ism.IndustryId = pim.IndustryId
       AND ism.Category_Master_Id = psm.Category_Master_Id
    LEFT JOIN dbo.Industry_Master im ON im.IndustryId = pim.IndustryId
    LEFT JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = psm.Category_Master_Id
    WHERE psm.ProductId = @ID
      AND psm.IndustryId IS NULL
      AND psm.Category_Master_Id IS NOT NULL
      AND (
            ism.IndustrySubcategoryId IS NOT NULL
            OR (
                SELECT COUNT(1)
                FROM dbo.product_Industry_Mapping x
                WHERE x.ProductId = psm.ProductId
            ) = 1
          )
    ORDER BY IndustryName, SubcategoryName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_SaveMappings
    @ProductId INT,
    @IndustryIds NVARCHAR(MAX) = NULL,
    @ApplicationIds NVARCHAR(MAX) = NULL,
    @SubcategoryIds NVARCHAR(MAX) = NULL,
    @PackagingIds NVARCHAR(MAX) = NULL,
    @IndustrySubcategoryPairs NVARCHAR(MAX) = NULL, -- format: IndustryId:SubcategoryId,IndustryId:SubcategoryId
    @Create_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Industries TABLE (IndustryId INT NOT NULL PRIMARY KEY);
    DECLARE @Applications TABLE (ApplicationId INT NOT NULL PRIMARY KEY);
    DECLARE @Packaging TABLE (product_packaging_MasterId INT NOT NULL PRIMARY KEY);
    DECLARE @Pairs TABLE
    (
        IndustryId INT NOT NULL,
        SubcategoryId INT NOT NULL,
        PRIMARY KEY (IndustryId, SubcategoryId)
    );

    -- Only exact pairs are saved. Never expand to all subcategories under an industry.
    IF @IndustrySubcategoryPairs IS NOT NULL AND LTRIM(RTRIM(@IndustrySubcategoryPairs)) <> N''
    BEGIN
        INSERT INTO @Pairs (IndustryId, SubcategoryId)
        SELECT DISTINCT
            TRY_CAST(LEFT(LTRIM(RTRIM(value)), CHARINDEX(':', LTRIM(RTRIM(value))) - 1) AS INT),
            TRY_CAST(SUBSTRING(LTRIM(RTRIM(value)), CHARINDEX(':', LTRIM(RTRIM(value))) + 1, 50) AS INT)
        FROM STRING_SPLIT(@IndustrySubcategoryPairs, ',')
        WHERE CHARINDEX(':', LTRIM(RTRIM(value))) > 1
          AND TRY_CAST(LEFT(LTRIM(RTRIM(value)), CHARINDEX(':', LTRIM(RTRIM(value))) - 1) AS INT) IS NOT NULL
          AND TRY_CAST(SUBSTRING(LTRIM(RTRIM(value)), CHARINDEX(':', LTRIM(RTRIM(value))) + 1, 50) AS INT) IS NOT NULL;
    END

    INSERT INTO @Industries (IndustryId)
    SELECT DISTINCT IndustryId FROM @Pairs;

    IF @ApplicationIds IS NOT NULL AND LTRIM(RTRIM(@ApplicationIds)) <> N''
        INSERT INTO @Applications (ApplicationId)
        SELECT DISTINCT TRY_CAST(value AS INT)
        FROM STRING_SPLIT(@ApplicationIds, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL;

    IF @PackagingIds IS NOT NULL AND LTRIM(RTRIM(@PackagingIds)) <> N''
        INSERT INTO @Packaging (product_packaging_MasterId)
        SELECT DISTINCT TRY_CAST(value AS INT)
        FROM STRING_SPLIT(@PackagingIds, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM dbo.product_Industry_Mapping WHERE ProductId = @ProductId;
        DELETE FROM dbo.product_application_Mapping WHERE ProductId = @ProductId;
        DELETE FROM dbo.product_subcategory_Mapping WHERE ProductId = @ProductId;
        DELETE FROM dbo.product_packaging_Mapping WHERE ProductId = @ProductId;

        INSERT INTO dbo.product_Industry_Mapping (ProductId, IndustryId, DisplayOrder, Create_UserId, CreatedDate)
        SELECT @ProductId, i.IndustryId,
               ROW_NUMBER() OVER (ORDER BY ISNULL(im.IndustryName, N''), i.IndustryId),
               @Create_UserId, SYSUTCDATETIME()
        FROM @Industries i
        LEFT JOIN dbo.Industry_Master im ON im.IndustryId = i.IndustryId;

        INSERT INTO dbo.product_application_Mapping (ProductId, ApplicationId, DisplayOrder, Create_UserId, CreatedDate)
        SELECT @ProductId, a.ApplicationId,
               ROW_NUMBER() OVER (ORDER BY ISNULL(am.ApplicationName, N''), a.ApplicationId),
               @Create_UserId, SYSUTCDATETIME()
        FROM @Applications a
        LEFT JOIN dbo.Application_Master am ON am.ApplicationId = a.ApplicationId;

        INSERT INTO dbo.product_subcategory_Mapping
        (
            ProductId, IndustryId, Category_Master_Id, DisplayOrder, Create_UserId, CreatedDate
        )
        SELECT
            @ProductId,
            p.IndustryId,
            p.SubcategoryId,
            ROW_NUMBER() OVER (ORDER BY ISNULL(im.IndustryName, N''), ISNULL(cm.SubcategoryName, N''), p.IndustryId, p.SubcategoryId),
            @Create_UserId,
            SYSUTCDATETIME()
        FROM @Pairs p
        LEFT JOIN dbo.Industry_Master im ON im.IndustryId = p.IndustryId
        LEFT JOIN dbo.Industry_Subcategory_Master cm ON cm.SubcategoryId = p.SubcategoryId;

        INSERT INTO dbo.product_packaging_Mapping (ProductId, product_packaging_MasterId, DisplayOrder, Create_UserId, CreatedDate)
        SELECT @ProductId, pk.product_packaging_MasterId,
               ROW_NUMBER() OVER (ORDER BY ISNULL(pm.[Name], N''), pk.product_packaging_MasterId),
               @Create_UserId, SYSUTCDATETIME()
        FROM @Packaging pk
        LEFT JOIN dbo.product_packaging_Master pm ON pm.product_packaging_MasterId = pk.product_packaging_MasterId;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

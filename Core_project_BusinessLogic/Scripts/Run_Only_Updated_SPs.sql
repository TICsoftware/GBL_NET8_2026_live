/*
  Run this file only.
  Creates / updates the stored procedures needed for:
    - unique name + language (Industry / Application / Industry Category / Product)
    - unique page name across all languages (Industry + Product)
    - Product masthead / thumbnail alt fields
    - unique IndustryId + Category mapping (no duplicate tagging)

  Also adds Product_Master.Banner_Image_Alt and Thumbnail_Image_Alt if missing.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.Product_Master', 'Banner_Image_Alt') IS NULL
    ALTER TABLE dbo.Product_Master ADD Banner_Image_Alt NVARCHAR(500) NULL;
GO

IF COL_LENGTH('dbo.Product_Master', 'Thumbnail_Image_Alt') IS NULL
    ALTER TABLE dbo.Product_Master ADD Thumbnail_Image_Alt NVARCHAR(500) NULL;
GO

/* ========== INDUSTRY ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Insert
    @Name NVARCHAR(500),
    @PageName NVARCHAR(300) = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Language_Master_Id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @Landing_Thumbnail_Image_media_id INT = NULL,
    @Banner_Image_Alt NVARCHAR(500) = NULL,
    @Landing_Thumbnail_Image_Alt NVARCHAR(500) = NULL,
    @Window_Title NVARCHAR(2000) = NULL,
    @Meta_Title NVARCHAR(2000) = NULL,
    @Meta_Description NVARCHAR(2000) = NULL,
    @Intro NVARCHAR(MAX) = NULL,
    @Content NVARCHAR(MAX) = NULL,
    @Create_UserId INT = NULL,
    @MasterType NVARCHAR(50),
    @NewID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewID = 0;
    IF @Status IS NULL SET @Status = 1;

    IF @MasterType = N'Industry'
       AND EXISTS (
            SELECT 1 FROM dbo.Industry_Master
            WHERE LOWER(LTRIM(RTRIM(IndustryName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0))
        THROW 50020, 'This name already exists for the selected language.', 1;
    ELSE IF @MasterType = N'Application'
       AND EXISTS (
            SELECT 1 FROM dbo.Application_Master
            WHERE LOWER(LTRIM(RTRIM(ApplicationName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0))
        THROW 50020, 'This name already exists for the selected language.', 1;
    ELSE IF @MasterType = N'IndustryCategory'
       AND EXISTS (
            SELECT 1 FROM dbo.Industry_Subcategory_Master
            WHERE LOWER(LTRIM(RTRIM(SubcategoryName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0))
        THROW 50020, 'This name already exists for the selected language.', 1;

    IF @MasterType = N'Industry'
       AND @PageName IS NOT NULL AND LTRIM(RTRIM(@PageName)) <> N''
       AND EXISTS (
            SELECT 1 FROM dbo.Industry_Master
            WHERE LOWER(LTRIM(RTRIM(Industry_pagename))) = LOWER(LTRIM(RTRIM(@PageName))))
        THROW 50021, 'This page name already exists.', 1;

    IF @MasterType = N'Industry'
    BEGIN
        INSERT INTO dbo.Industry_Master
        (
            IndustryName, Industry_pagename,
            Banner_Image_media_id, Landing_Thumbnail_Image_media_id,
            Banner_Image_Alt, Landing_Thumbnail_Image_Alt,
            Window_Title, Meta_Title, Meta_Description,
            Intro, Content,
            Language_Master_Id, DisplayOrder, [Status],
            Create_UserId, CreatedDate
        )
        VALUES
        (
            @Name, @PageName,
            @Banner_Image_media_id, @Landing_Thumbnail_Image_media_id,
            @Banner_Image_Alt, @Landing_Thumbnail_Image_Alt,
            @Window_Title, @Meta_Title, @Meta_Description,
            @Intro, @Content,
            @Language_Master_Id, @Sequence, @Status,
            @Create_UserId, SYSUTCDATETIME()
        );
        SET @NewID = SCOPE_IDENTITY();
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        INSERT INTO dbo.Application_Master
            (ApplicationName, Language_Master_Id, DisplayOrder, [Status], Create_UserId, CreatedDate)
        VALUES
            (@Name, @Language_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME());
        SET @NewID = SCOPE_IDENTITY();
    END
    ELSE IF @MasterType = N'IndustryCategory'
    BEGIN
        INSERT INTO dbo.Industry_Subcategory_Master
            (SubcategoryName, Language_Master_Id, DisplayOrder, [Status], Create_UserId, CreatedDate)
        VALUES
            (@Name, @Language_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME());
        SET @NewID = SCOPE_IDENTITY();
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Update
    @ID INT,
    @Name NVARCHAR(500),
    @PageName NVARCHAR(300) = NULL,
    @Sequence INT,
    @Language_Master_Id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @Landing_Thumbnail_Image_media_id INT = NULL,
    @Banner_Image_Alt NVARCHAR(500) = NULL,
    @Landing_Thumbnail_Image_Alt NVARCHAR(500) = NULL,
    @Window_Title NVARCHAR(2000) = NULL,
    @Meta_Title NVARCHAR(2000) = NULL,
    @Meta_Description NVARCHAR(2000) = NULL,
    @Intro NVARCHAR(MAX) = NULL,
    @Content NVARCHAR(MAX) = NULL,
    @Update_UserId INT = NULL,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Lang INT = @Language_Master_Id;
    IF @Lang IS NULL AND @MasterType = N'Application'
        SELECT @Lang = Language_Master_Id FROM dbo.Application_Master WHERE ApplicationId = @ID;
    ELSE IF @Lang IS NULL AND @MasterType = N'IndustryCategory'
        SELECT @Lang = Language_Master_Id FROM dbo.Industry_Subcategory_Master WHERE SubcategoryId = @ID;

    IF @MasterType = N'Industry'
       AND EXISTS (
            SELECT 1 FROM dbo.Industry_Master
            WHERE LOWER(LTRIM(RTRIM(IndustryName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Lang, 0)
              AND IndustryId <> @ID)
        THROW 50020, 'This name already exists for the selected language.', 1;
    ELSE IF @MasterType = N'Application'
       AND EXISTS (
            SELECT 1 FROM dbo.Application_Master
            WHERE LOWER(LTRIM(RTRIM(ApplicationName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Lang, 0)
              AND ApplicationId <> @ID)
        THROW 50020, 'This name already exists for the selected language.', 1;
    ELSE IF @MasterType = N'IndustryCategory'
       AND EXISTS (
            SELECT 1 FROM dbo.Industry_Subcategory_Master
            WHERE LOWER(LTRIM(RTRIM(SubcategoryName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Lang, 0)
              AND SubcategoryId <> @ID)
        THROW 50020, 'This name already exists for the selected language.', 1;

    IF @MasterType = N'Industry'
       AND @PageName IS NOT NULL AND LTRIM(RTRIM(@PageName)) <> N''
       AND EXISTS (
            SELECT 1 FROM dbo.Industry_Master
            WHERE LOWER(LTRIM(RTRIM(Industry_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
              AND IndustryId <> @ID)
        THROW 50021, 'This page name already exists.', 1;

    IF @MasterType = N'Industry'
    BEGIN
        UPDATE dbo.Industry_Master
        SET IndustryName = @Name,
            Industry_pagename = @PageName,
            Banner_Image_media_id = @Banner_Image_media_id,
            Landing_Thumbnail_Image_media_id = @Landing_Thumbnail_Image_media_id,
            Banner_Image_Alt = @Banner_Image_Alt,
            Landing_Thumbnail_Image_Alt = @Landing_Thumbnail_Image_Alt,
            Window_Title = @Window_Title,
            Meta_Title = @Meta_Title,
            Meta_Description = @Meta_Description,
            Intro = @Intro,
            Content = @Content,
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE IndustryId = @ID;
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        UPDATE dbo.Application_Master
        SET ApplicationName = @Name,
            Language_Master_Id = ISNULL(@Language_Master_Id, Language_Master_Id),
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE ApplicationId = @ID;
    END
    ELSE IF @MasterType = N'IndustryCategory'
    BEGIN
        UPDATE dbo.Industry_Subcategory_Master
        SET SubcategoryName = @Name,
            Language_Master_Id = ISNULL(@Language_Master_Id, Language_Master_Id),
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE SubcategoryId = @ID;
    END
END
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_NameExists
    @Name NVARCHAR(500),
    @Language_Master_Id INT = NULL,
    @ID INT = 0,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM dbo.Industry_Master
            WHERE LOWER(LTRIM(RTRIM(IndustryName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0)
              AND IndustryId <> ISNULL(@ID, 0)
        ) THEN 1 ELSE 0 END AS IsExists;
    ELSE IF @MasterType = N'Application'
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM dbo.Application_Master
            WHERE LOWER(LTRIM(RTRIM(ApplicationName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0)
              AND ApplicationId <> ISNULL(@ID, 0)
        ) THEN 1 ELSE 0 END AS IsExists;
    ELSE IF @MasterType = N'IndustryCategory'
        SELECT CASE WHEN EXISTS (
            SELECT 1 FROM dbo.Industry_Subcategory_Master
            WHERE LOWER(LTRIM(RTRIM(SubcategoryName))) = LOWER(LTRIM(RTRIM(@Name)))
              AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0)
              AND SubcategoryId <> ISNULL(@ID, 0)
        ) THEN 1 ELSE 0 END AS IsExists;
    ELSE
        SELECT 0 AS IsExists;
END
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_PageNameExists
    @PageName NVARCHAR(300),
    @ID INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM dbo.Industry_Master
        WHERE @PageName IS NOT NULL
          AND LTRIM(RTRIM(@PageName)) <> N''
          AND LOWER(LTRIM(RTRIM(Industry_pagename))) = LOWER(LTRIM(RTRIM(@PageName)))
          AND IndustryId <> ISNULL(@ID, 0)
    ) THEN 1 ELSE 0 END AS IsExists;
END
GO

/* ========== PRODUCT ========== */

CREATE OR ALTER PROCEDURE dbo.Product_Master_GetPaged
    @Search NVARCHAR(500) = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;
    DECLARE @Offset INT = (@Page - 1) * @PageSize;

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
        mt.file_path AS Thumbnail_Image_Url,
        mb.file_path AS Banner_Image_Url,
        ms.file_path AS SafetyDataSheet_Url,
        p.Banner_Image_Alt,
        p.Thumbnail_Image_Alt,
        p.Intro,
        p.Content,
        p.Technical_Overview
    FROM dbo.Product_Master p
    LEFT JOIN dbo.Language_Master lm ON lm.ID = p.Language_Master_Id
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    LEFT JOIN dbo.media ms ON ms.ID = p.SafetyDataSheet_media_id
    WHERE (@Search IS NULL OR p.ProductName LIKE N'%' + @Search + N'%')
    ORDER BY p.DisplayOrder, p.ProductId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.Product_Master p
    WHERE (@Search IS NULL OR p.ProductName LIKE N'%' + @Search + N'%');
END
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
        mt.file_path AS Thumbnail_Image_Url,
        mb.file_path AS Banner_Image_Url,
        ms.file_path AS SafetyDataSheet_Url,
        p.Banner_Image_Alt,
        p.Thumbnail_Image_Alt,
        p.Intro,
        p.Content,
        p.Technical_Overview
    FROM dbo.Product_Master p
    LEFT JOIN dbo.Language_Master lm ON lm.ID = p.Language_Master_Id
    LEFT JOIN dbo.media mt ON mt.ID = p.Thumbnail_Image_media_id
    LEFT JOIN dbo.media mb ON mb.ID = p.Banner_Image_media_id
    LEFT JOIN dbo.media ms ON ms.ID = p.SafetyDataSheet_media_id
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
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_Insert
    @ProductName NVARCHAR(500),
    @Product_pagename NVARCHAR(300) = NULL,
    @Intro NVARCHAR(MAX) = NULL,
    @Content NVARCHAR(MAX) = NULL,
    @Technical_Overview NVARCHAR(MAX) = NULL,
    @Thumbnail_Image_media_id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @SafetyDataSheet_media_id INT = NULL,
    @Banner_Image_Alt NVARCHAR(500) = NULL,
    @Thumbnail_Image_Alt NVARCHAR(500) = NULL,
    @Language_Master_Id INT = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Create_UserId INT = NULL,
    @NewID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewID = 0;
    IF @Status IS NULL SET @Status = 1;

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
        ProductName, Product_pagename, Intro, Content, Technical_Overview,
        Thumbnail_Image_media_id, Banner_Image_media_id, SafetyDataSheet_media_id,
        Banner_Image_Alt, Thumbnail_Image_Alt,
        Language_Master_Id, DisplayOrder, [status], Create_UserId, CreatedDate
    )
    VALUES
    (
        @ProductName, @Product_pagename, @Intro, @Content, @Technical_Overview,
        @Thumbnail_Image_media_id, @Banner_Image_media_id, @SafetyDataSheet_media_id,
        @Banner_Image_Alt, @Thumbnail_Image_Alt,
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
    @Thumbnail_Image_media_id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @SafetyDataSheet_media_id INT = NULL,
    @Banner_Image_Alt NVARCHAR(500) = NULL,
    @Thumbnail_Image_Alt NVARCHAR(500) = NULL,
    @Language_Master_Id INT = NULL,
    @Sequence INT,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

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
        Thumbnail_Image_media_id = @Thumbnail_Image_media_id,
        Banner_Image_media_id = @Banner_Image_media_id,
        SafetyDataSheet_media_id = @SafetyDataSheet_media_id,
        Banner_Image_Alt = @Banner_Image_Alt,
        Thumbnail_Image_Alt = @Thumbnail_Image_Alt,
        Language_Master_Id = @Language_Master_Id,
        DisplayOrder = @Sequence,
        Update_UserId = @Update_UserId,
        ModifiedDate = SYSUTCDATETIME()
    WHERE ProductId = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_NameExists
    @ProductName NVARCHAR(500),
    @Language_Master_Id INT = NULL,
    @ProductId INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM dbo.Product_Master
        WHERE LOWER(LTRIM(RTRIM(ProductName))) = LOWER(LTRIM(RTRIM(@ProductName)))
          AND ISNULL(Language_Master_Id, 0) = ISNULL(@Language_Master_Id, 0)
          AND ProductId <> ISNULL(@ProductId, 0)
    ) THEN 1 ELSE 0 END AS IsExists;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_PageNameExists
    @Product_pagename NVARCHAR(300),
    @ProductId INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CASE WHEN EXISTS (
        SELECT 1 FROM dbo.Product_Master
        WHERE @Product_pagename IS NOT NULL
          AND LTRIM(RTRIM(@Product_pagename)) <> N''
          AND LOWER(LTRIM(RTRIM(Product_pagename))) = LOWER(LTRIM(RTRIM(@Product_pagename)))
          AND ProductId <> ISNULL(@ProductId, 0)
    ) THEN 1 ELSE 0 END AS IsExists;
END
GO

/* ========== INDUSTRY / CATEGORY TAGGING ========== */

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'UX_Industry_Subcategory_Mapping_Industry_Category'
      AND object_id = OBJECT_ID(N'dbo.Industry_Subcategory_Mapping')
)
BEGIN
    ;WITH dups AS
    (
        SELECT IndustrySubcategoryId,
               ROW_NUMBER() OVER (
                   PARTITION BY IndustryId, Category_Master_Id
                   ORDER BY IndustrySubcategoryId
               ) AS rn
        FROM dbo.Industry_Subcategory_Mapping
    )
    DELETE FROM dups WHERE rn > 1;

    CREATE UNIQUE NONCLUSTERED INDEX UX_Industry_Subcategory_Mapping_Industry_Category
        ON dbo.Industry_Subcategory_Mapping (IndustryId, Category_Master_Id);
END
GO

CREATE OR ALTER PROCEDURE dbo.Industry_Category_Mapping_Save
    @IndustryIds NVARCHAR(MAX),
    @CategoryIds NVARCHAR(MAX),
    @Create_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @IndustryIds IS NULL OR LTRIM(RTRIM(@IndustryIds)) = N''
        THROW 50001, 'Select at least one industry.', 1;
    IF @CategoryIds IS NULL OR LTRIM(RTRIM(@CategoryIds)) = N''
        THROW 50002, 'Select at least one category.', 1;

    DECLARE @Industries TABLE (IndustryId INT NOT NULL PRIMARY KEY);
    DECLARE @Categories TABLE (Category_Master_Id INT NOT NULL PRIMARY KEY);

    INSERT INTO @Industries (IndustryId)
    SELECT DISTINCT TRY_CAST(value AS INT)
    FROM STRING_SPLIT(@IndustryIds, ',')
    WHERE TRY_CAST(value AS INT) IS NOT NULL;

    INSERT INTO @Categories (Category_Master_Id)
    SELECT DISTINCT TRY_CAST(value AS INT)
    FROM STRING_SPLIT(@CategoryIds, ',')
    WHERE TRY_CAST(value AS INT) IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM @Industries)
        THROW 50001, 'Select at least one industry.', 1;
    IF NOT EXISTS (SELECT 1 FROM @Categories)
        THROW 50002, 'Select at least one category.', 1;

    IF EXISTS (
        SELECT 1
        FROM dbo.Industry_Subcategory_Mapping m
        INNER JOIN @Industries i ON i.IndustryId = m.IndustryId
        INNER JOIN @Categories c ON c.Category_Master_Id = m.Category_Master_Id
    )
        THROW 50003, 'This industry and category mapping already exists.', 1;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @MaxOrder INT =
            ISNULL((SELECT MAX(DisplayOrder) FROM dbo.Industry_Subcategory_Mapping), 0);

        ;WITH pairs AS
        (
            SELECT
                i.IndustryId,
                c.Category_Master_Id,
                ROW_NUMBER() OVER (
                    ORDER BY ISNULL(im.IndustryName, N''), ISNULL(cm.SubcategoryName, N''), i.IndustryId, c.Category_Master_Id
                ) AS RowNum
            FROM @Industries i
            CROSS JOIN @Categories c
            LEFT JOIN dbo.Industry_Master im ON im.IndustryId = i.IndustryId
            LEFT JOIN dbo.Industry_Subcategory_Master cm ON cm.SubcategoryId = c.Category_Master_Id
            WHERE NOT EXISTS (
                SELECT 1
                FROM dbo.Industry_Subcategory_Mapping x
                WHERE x.IndustryId = i.IndustryId
                  AND x.Category_Master_Id = c.Category_Master_Id
            )
        )
        INSERT INTO dbo.Industry_Subcategory_Mapping
        (
            IndustryId, Category_Master_Id, DisplayOrder,
            Create_UserId, CreatedDate
        )
        SELECT
            p.IndustryId, p.Category_Master_Id, @MaxOrder + p.RowNum,
            @Create_UserId, SYSUTCDATETIME()
        FROM pairs p;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

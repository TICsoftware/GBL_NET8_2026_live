/*
  Product Master + industry / application / subcategory / packaging mappings
  Status: 1 = Active, 0 = Inactive
  Media preview joins dbo.media (ID, file_path).
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.Product_Master', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Product_Master
    (
        ProductId INT IDENTITY(1,1) NOT NULL,
        ProductName NVARCHAR(500) NOT NULL,
        Product_pagename NVARCHAR(300) NULL,
        Intro NVARCHAR(MAX) NULL,
        Content NVARCHAR(MAX) NULL,
        Thumbnail_Image_media_id INT NULL,
        Banner_Image_media_id INT NULL,
        Technical_Overview NVARCHAR(MAX) NULL,
        SafetyDataSheet_media_id INT NULL,
        Language_Master_Id INT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_Product_Master_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_Product_Master_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        [status] INT NULL,
        CONSTRAINT PK_Product_Master PRIMARY KEY CLUSTERED (ProductId ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.product_Industry_Mapping', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_Industry_Mapping
    (
        product_Industry_MappingID INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        IndustryId INT NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_product_Industry_Mapping_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_product_Industry_Mapping_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        CONSTRAINT PK_product_Industry_Mapping PRIMARY KEY CLUSTERED (product_Industry_MappingID ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.product_application_Mapping', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_application_Mapping
    (
        product_application_MappingID INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        ApplicationId INT NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_product_application_Mapping_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_product_application_Mapping_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        CONSTRAINT PK_product_application_Mapping PRIMARY KEY CLUSTERED (product_application_MappingID ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.product_subcategory_Mapping', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_subcategory_Mapping
    (
        product_subcategory_MappingID INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        Category_Master_Id INT NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_product_subcategory_Mapping_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_product_subcategory_Mapping_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        CONSTRAINT PK_product_subcategory_Mapping PRIMARY KEY CLUSTERED (product_subcategory_MappingID ASC)
    );
END
GO

IF OBJECT_ID(N'dbo.product_packaging_Mapping', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_packaging_Mapping
    (
        product_packaging_MappingID INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        product_packaging_MasterId INT NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_product_packaging_Mapping_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_product_packaging_Mapping_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        CONSTRAINT PK_product_packaging_Mapping PRIMARY KEY CLUSTERED (product_packaging_MappingID ASC)
    );
END
GO

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

    INSERT INTO dbo.Product_Master
    (
        ProductName, Product_pagename, Intro, Content, Technical_Overview,
        Thumbnail_Image_media_id, Banner_Image_media_id, SafetyDataSheet_media_id,
        Language_Master_Id, DisplayOrder, [status], Create_UserId, CreatedDate
    )
    VALUES
    (
        @ProductName, @Product_pagename, @Intro, @Content, @Technical_Overview,
        @Thumbnail_Image_media_id, @Banner_Image_media_id, @SafetyDataSheet_media_id,
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

    UPDATE dbo.Product_Master
    SET ProductName = @ProductName,
        Product_pagename = @Product_pagename,
        Intro = @Intro,
        Content = @Content,
        Technical_Overview = @Technical_Overview,
        Thumbnail_Image_media_id = @Thumbnail_Image_media_id,
        Banner_Image_media_id = @Banner_Image_media_id,
        SafetyDataSheet_media_id = @SafetyDataSheet_media_id,
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

CREATE OR ALTER PROCEDURE dbo.Product_Master_SaveMappings
    @ProductId INT,
    @IndustryIds NVARCHAR(MAX) = NULL,
    @ApplicationIds NVARCHAR(MAX) = NULL,
    @SubcategoryIds NVARCHAR(MAX) = NULL,
    @PackagingIds NVARCHAR(MAX) = NULL,
    @Create_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Industries TABLE (IndustryId INT NOT NULL PRIMARY KEY);
    DECLARE @Applications TABLE (ApplicationId INT NOT NULL PRIMARY KEY);
    DECLARE @Subcategories TABLE (Category_Master_Id INT NOT NULL PRIMARY KEY);
    DECLARE @Packaging TABLE (product_packaging_MasterId INT NOT NULL PRIMARY KEY);

    IF @IndustryIds IS NOT NULL AND LTRIM(RTRIM(@IndustryIds)) <> N''
        INSERT INTO @Industries (IndustryId)
        SELECT DISTINCT TRY_CAST(value AS INT)
        FROM STRING_SPLIT(@IndustryIds, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL;

    IF @ApplicationIds IS NOT NULL AND LTRIM(RTRIM(@ApplicationIds)) <> N''
        INSERT INTO @Applications (ApplicationId)
        SELECT DISTINCT TRY_CAST(value AS INT)
        FROM STRING_SPLIT(@ApplicationIds, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL;

    IF @SubcategoryIds IS NOT NULL AND LTRIM(RTRIM(@SubcategoryIds)) <> N''
        INSERT INTO @Subcategories (Category_Master_Id)
        SELECT DISTINCT TRY_CAST(value AS INT)
        FROM STRING_SPLIT(@SubcategoryIds, ',')
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

        INSERT INTO dbo.product_subcategory_Mapping (ProductId, Category_Master_Id, DisplayOrder, Create_UserId, CreatedDate)
        SELECT @ProductId, s.Category_Master_Id,
               ROW_NUMBER() OVER (ORDER BY ISNULL(cm.SubcategoryName, N''), s.Category_Master_Id),
               @Create_UserId, SYSUTCDATETIME()
        FROM @Subcategories s
        LEFT JOIN dbo.Industry_Subcategory_Master cm ON cm.SubcategoryId = s.Category_Master_Id;

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

CREATE OR ALTER PROCEDURE dbo.Product_Master_Activate
    @ID INT,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Product_Master
    SET [status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME()
    WHERE ProductId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_Deactivate
    @ID INT,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Product_Master
    SET [status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME()
    WHERE ProductId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_UpdateSequence
    @ID INT,
    @Sequence INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Product_Master
    SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME()
    WHERE ProductId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_Delete
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;
        DELETE FROM dbo.product_Industry_Mapping WHERE ProductId = @ID;
        DELETE FROM dbo.product_application_Mapping WHERE ProductId = @ID;
        DELETE FROM dbo.product_subcategory_Mapping WHERE ProductId = @ID;
        DELETE FROM dbo.product_packaging_Mapping WHERE ProductId = @ID;
        DELETE FROM dbo.Product_Master WHERE ProductId = @ID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_GetIndustries
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IndustryId AS Id, IndustryName AS Name
    FROM dbo.Industry_Master
    WHERE ISNULL([Status], 1) = 1
    ORDER BY DisplayOrder, IndustryName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_GetApplications
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ApplicationId AS Id, ApplicationName AS Name
    FROM dbo.Application_Master
    WHERE ISNULL([Status], 1) = 1
    ORDER BY DisplayOrder, ApplicationName;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_GetSubcategoriesByIndustries
    @IndustryIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Industries TABLE (IndustryId INT NOT NULL PRIMARY KEY);

    INSERT INTO @Industries (IndustryId)
    SELECT DISTINCT TRY_CAST(value AS INT)
    FROM STRING_SPLIT(ISNULL(@IndustryIds, N''), ',')
    WHERE TRY_CAST(value AS INT) IS NOT NULL;

    SELECT DISTINCT
        c.SubcategoryId AS Id,
        c.SubcategoryName AS Name
    FROM dbo.Industry_Subcategory_Mapping m
    INNER JOIN @Industries i ON i.IndustryId = m.IndustryId
    INNER JOIN dbo.Industry_Subcategory_Master c ON c.SubcategoryId = m.Category_Master_Id
    WHERE ISNULL(c.[Status], 1) = 1
    ORDER BY c.SubcategoryName;
END
GO

/*
  Run this file only for Product Master:
    - Main_Application column
    - product_certificate_Mapping table
    - updated GetById / Insert / Update / Delete
    - certificate clear + insert SPs
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF COL_LENGTH('dbo.Product_Master', 'Main_Application') IS NULL
    ALTER TABLE dbo.Product_Master ADD Main_Application NVARCHAR(MAX) NULL;
GO

IF OBJECT_ID(N'dbo.product_certificate_Mapping', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_certificate_Mapping
    (
        product_certificate_MappingID INT IDENTITY(1,1) NOT NULL,
        ProductId INT NOT NULL,
        CertificateTitle NVARCHAR(500) NOT NULL,
        Certificate_media_id INT NOT NULL,
        DisplayOrder INT NOT NULL CONSTRAINT DF_product_certificate_Mapping_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_product_certificate_Mapping_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        CONSTRAINT PK_product_certificate_Mapping PRIMARY KEY CLUSTERED (product_certificate_MappingID ASC)
    );
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
        p.Technical_Overview,
        p.Main_Application
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
END
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
        ProductName, Product_pagename, Intro, Content, Technical_Overview, Main_Application,
        Thumbnail_Image_media_id, Banner_Image_media_id, SafetyDataSheet_media_id,
        Banner_Image_Alt, Thumbnail_Image_Alt,
        Language_Master_Id, DisplayOrder, [status], Create_UserId, CreatedDate
    )
    VALUES
    (
        @ProductName, @Product_pagename, @Intro, @Content, @Technical_Overview, @Main_Application,
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
    @Main_Application NVARCHAR(MAX) = NULL,
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
        Main_Application = @Main_Application,
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
        DELETE FROM dbo.product_certificate_Mapping WHERE ProductId = @ID;
        DELETE FROM dbo.Product_Master WHERE ProductId = @ID;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_ClearCertificates
    @ProductId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.product_certificate_Mapping WHERE ProductId = @ProductId;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Master_InsertCertificate
    @ProductId INT,
    @CertificateTitle NVARCHAR(500),
    @Certificate_media_id INT,
    @DisplayOrder INT,
    @Create_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.product_certificate_Mapping
    (
        ProductId, CertificateTitle, Certificate_media_id, DisplayOrder,
        Create_UserId, CreatedDate
    )
    VALUES
    (
        @ProductId, @CertificateTitle, @Certificate_media_id, @DisplayOrder,
        @Create_UserId, SYSUTCDATETIME()
    );
END
GO

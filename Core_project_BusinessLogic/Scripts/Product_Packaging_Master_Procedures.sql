/*
  Product Packaging Master
  Status: 1 = Active, 0 = Inactive
  Thumbnailimage_Id stores media.ID as nvarchar to match table schema.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.product_packaging_Master', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.product_packaging_Master
    (
        product_packaging_MasterId INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(250) NOT NULL,
        Language_Master_Id INT NULL,
        Thumbnailimage_Id NVARCHAR(250) NOT NULL CONSTRAINT DF_product_packaging_Master_Thumb DEFAULT (N''),
        Thumbnailimage_alt NVARCHAR(250) NOT NULL CONSTRAINT DF_product_packaging_Master_ThumbAlt DEFAULT (N''),
        DisplayOrder INT NOT NULL CONSTRAINT DF_product_packaging_Master_DisplayOrder DEFAULT (0),
        Create_UserId INT NULL,
        Update_UserId INT NULL,
        CreatedDate DATETIME2(7) NOT NULL CONSTRAINT DF_product_packaging_Master_CreatedDate DEFAULT (SYSUTCDATETIME()),
        ModifiedDate DATETIME2(7) NULL,
        [Status] INT NOT NULL CONSTRAINT DF_product_packaging_Master_Status DEFAULT (1),
        CONSTRAINT PK_product_packaging_Master PRIMARY KEY CLUSTERED (product_packaging_MasterId ASC)
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_GetPaged
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
        p.product_packaging_MasterId,
        p.[Name],
        p.Language_Master_Id,
        lm.Language_Name AS LanguageName,
        p.Thumbnailimage_Id,
        mt.file_path AS Thumbnail_Image_Url,
        p.Thumbnailimage_alt,
        p.DisplayOrder AS [Sequence],
        p.[Status]
    FROM dbo.product_packaging_Master p
    LEFT JOIN dbo.Language_Master lm ON lm.ID = p.Language_Master_Id
    LEFT JOIN dbo.media mt ON TRY_CONVERT(INT, p.Thumbnailimage_Id) = mt.ID
    WHERE (@Search IS NULL OR p.[Name] LIKE N'%' + @Search + N'%')
    ORDER BY p.DisplayOrder, p.product_packaging_MasterId
    OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

    SELECT COUNT(1) AS TotalCount
    FROM dbo.product_packaging_Master p
    WHERE (@Search IS NULL OR p.[Name] LIKE N'%' + @Search + N'%');
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_GetById
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.product_packaging_MasterId,
        p.[Name],
        p.Language_Master_Id,
        lm.Language_Name AS LanguageName,
        p.Thumbnailimage_Id,
        mt.file_path AS Thumbnail_Image_Url,
        p.Thumbnailimage_alt,
        p.DisplayOrder AS [Sequence],
        p.[Status]
    FROM dbo.product_packaging_Master p
    LEFT JOIN dbo.Language_Master lm ON lm.ID = p.Language_Master_Id
    LEFT JOIN dbo.media mt ON TRY_CONVERT(INT, p.Thumbnailimage_Id) = mt.ID
    WHERE p.product_packaging_MasterId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_Insert
    @Name NVARCHAR(250),
    @Language_Master_Id INT = NULL,
    @Thumbnailimage_Id NVARCHAR(250) = NULL,
    @Thumbnailimage_alt NVARCHAR(250) = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Create_UserId INT = NULL,
    @NewID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewID = 0;
    IF @Status IS NULL SET @Status = 1;
    IF @Thumbnailimage_Id IS NULL SET @Thumbnailimage_Id = N'';
    IF @Thumbnailimage_alt IS NULL SET @Thumbnailimage_alt = N'';

    INSERT INTO dbo.product_packaging_Master
    (
        [Name], Language_Master_Id, Thumbnailimage_Id, Thumbnailimage_alt,
        DisplayOrder, [Status], Create_UserId, CreatedDate
    )
    VALUES
    (
        @Name, @Language_Master_Id, @Thumbnailimage_Id, @Thumbnailimage_alt,
        @Sequence, @Status, @Create_UserId, SYSUTCDATETIME()
    );
    SET @NewID = SCOPE_IDENTITY();
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_Update
    @ID INT,
    @Name NVARCHAR(250),
    @Language_Master_Id INT = NULL,
    @Thumbnailimage_Id NVARCHAR(250) = NULL,
    @Thumbnailimage_alt NVARCHAR(250) = NULL,
    @Sequence INT,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @Thumbnailimage_Id IS NULL SET @Thumbnailimage_Id = N'';
    IF @Thumbnailimage_alt IS NULL SET @Thumbnailimage_alt = N'';

    UPDATE dbo.product_packaging_Master
    SET [Name] = @Name,
        Language_Master_Id = @Language_Master_Id,
        Thumbnailimage_Id = @Thumbnailimage_Id,
        Thumbnailimage_alt = @Thumbnailimage_alt,
        DisplayOrder = @Sequence,
        Update_UserId = @Update_UserId,
        ModifiedDate = SYSUTCDATETIME()
    WHERE product_packaging_MasterId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_Activate
    @ID INT,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.product_packaging_Master
    SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME()
    WHERE product_packaging_MasterId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_Deactivate
    @ID INT,
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.product_packaging_Master
    SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME()
    WHERE product_packaging_MasterId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_UpdateSequence
    @ID INT,
    @Sequence INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.product_packaging_Master
    SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME()
    WHERE product_packaging_MasterId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_Delete
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF OBJECT_ID(N'dbo.product_packaging_Mapping', N'U') IS NOT NULL
       AND EXISTS (SELECT 1 FROM dbo.product_packaging_Mapping WHERE product_packaging_MasterId = @ID)
        THROW 50010, 'This packaging is mapped to one or more products.', 1;

    DELETE FROM dbo.product_packaging_Master WHERE product_packaging_MasterId = @ID;
END
GO

CREATE OR ALTER PROCEDURE dbo.Product_Packaging_Master_GetActive
AS
BEGIN
    SET NOCOUNT ON;
    SELECT product_packaging_MasterId AS Id, [Name]
    FROM dbo.product_packaging_Master
    WHERE ISNULL([Status], 1) = 1
    ORDER BY DisplayOrder, [Name];
END
GO

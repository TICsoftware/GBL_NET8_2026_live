/*
  Catalog Master - Status column + stored procedures
  Tables:
    Industry_Master
    Industry_Subcategory_Master
    Application_Master
    Product_Master
    Industry_Subcategory_Mapping

  Status: 1 = Active, 0 = Inactive
  Run once against the GBL database.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* ========================= ALTER TABLES - ADD Status ========================= */

IF COL_LENGTH('dbo.Industry_Master', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Industry_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Industry_Master_Status DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.Industry_Subcategory_Master', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Industry_Subcategory_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Industry_Subcategory_Master_Status DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.Application_Master', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Application_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Application_Master_Status DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.Product_Master', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Product_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Product_Master_Status DEFAULT (1);
END
GO

IF COL_LENGTH('dbo.Industry_Subcategory_Mapping', 'Status') IS NULL
BEGIN
    ALTER TABLE dbo.Industry_Subcategory_Mapping ADD [Status] INT NOT NULL CONSTRAINT DF_Industry_Subcategory_Mapping_Status DEFAULT (1);
END
GO

/* ========================= GetPaged ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_GetPaged
    @MasterType NVARCHAR(50),
    @Search NVARCHAR(500) = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 SET @PageSize = 10;

    DECLARE @Offset INT = (@Page - 1) * @PageSize;

    IF @MasterType = N'Industry'
    BEGIN
        SELECT
            IndustryId AS ID,
            IndustryName AS Name,
            Industry_pagename AS PageName,
            DisplayOrder AS [Sequence],
            [Status],
            Language_Master_Id
        FROM dbo.Industry_Master
        WHERE (@Search IS NULL OR IndustryName LIKE N'%' + @Search + N'%')
        ORDER BY DisplayOrder, IndustryId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Industry_Master
        WHERE (@Search IS NULL OR IndustryName LIKE N'%' + @Search + N'%');
    END
    ELSE IF @MasterType = N'Subcategory'
    BEGIN
        SELECT
            SubcategoryId AS ID,
            SubcategoryName AS Name,
            CAST(NULL AS NVARCHAR(300)) AS PageName,
            DisplayOrder AS [Sequence],
            [Status],
            Language_Master_Id
        FROM dbo.Industry_Subcategory_Master
        WHERE (@Search IS NULL OR SubcategoryName LIKE N'%' + @Search + N'%')
        ORDER BY DisplayOrder, SubcategoryId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Industry_Subcategory_Master
        WHERE (@Search IS NULL OR SubcategoryName LIKE N'%' + @Search + N'%');
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        SELECT
            ApplicationId AS ID,
            ApplicationName AS Name,
            CAST(NULL AS NVARCHAR(300)) AS PageName,
            DisplayOrder AS [Sequence],
            [Status],
            Language_Master_Id
        FROM dbo.Application_Master
        WHERE (@Search IS NULL OR ApplicationName LIKE N'%' + @Search + N'%')
        ORDER BY DisplayOrder, ApplicationId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Application_Master
        WHERE (@Search IS NULL OR ApplicationName LIKE N'%' + @Search + N'%');
    END
    ELSE IF @MasterType = N'Product'
    BEGIN
        SELECT
            ProductId AS ID,
            ProductName AS Name,
            Product_pagename AS PageName,
            DisplayOrder AS [Sequence],
            [Status],
            Language_Master_Id
        FROM dbo.Product_Master
        WHERE (@Search IS NULL OR ProductName LIKE N'%' + @Search + N'%')
        ORDER BY DisplayOrder, ProductId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Product_Master
        WHERE (@Search IS NULL OR ProductName LIKE N'%' + @Search + N'%');
    END
    ELSE IF @MasterType = N'Mapping'
    BEGIN
        SELECT
            m.IndustrySubcategoryId AS ID,
            i.IndustryName AS Name,
            CAST(NULL AS NVARCHAR(300)) AS PageName,
            m.DisplayOrder AS [Sequence],
            m.[Status],
            m.IndustryId,
            m.Category_Master_Id,
            s.SubcategoryName AS RelatedName
        FROM dbo.Industry_Subcategory_Mapping m
        INNER JOIN dbo.Industry_Master i ON i.IndustryId = m.IndustryId
        INNER JOIN dbo.Industry_Subcategory_Master s ON s.SubcategoryId = m.Category_Master_Id
        WHERE (
            @Search IS NULL
            OR i.IndustryName LIKE N'%' + @Search + N'%'
            OR s.SubcategoryName LIKE N'%' + @Search + N'%'
        )
        ORDER BY m.DisplayOrder, m.IndustrySubcategoryId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Industry_Subcategory_Mapping m
        INNER JOIN dbo.Industry_Master i ON i.IndustryId = m.IndustryId
        INNER JOIN dbo.Industry_Subcategory_Master s ON s.SubcategoryId = m.Category_Master_Id
        WHERE (
            @Search IS NULL
            OR i.IndustryName LIKE N'%' + @Search + N'%'
            OR s.SubcategoryName LIKE N'%' + @Search + N'%'
        );
    END
END
GO

/* ========================= GetById ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_GetById
    @ID INT,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
    BEGIN
        SELECT IndustryId AS ID, IndustryName AS Name, Industry_pagename AS PageName,
               DisplayOrder AS [Sequence], [Status], Language_Master_Id
        FROM dbo.Industry_Master WHERE IndustryId = @ID;
    END
    ELSE IF @MasterType = N'Subcategory'
    BEGIN
        SELECT SubcategoryId AS ID, SubcategoryName AS Name, CAST(NULL AS NVARCHAR(300)) AS PageName,
               DisplayOrder AS [Sequence], [Status], Language_Master_Id
        FROM dbo.Industry_Subcategory_Master WHERE SubcategoryId = @ID;
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        SELECT ApplicationId AS ID, ApplicationName AS Name, CAST(NULL AS NVARCHAR(300)) AS PageName,
               DisplayOrder AS [Sequence], [Status], Language_Master_Id
        FROM dbo.Application_Master WHERE ApplicationId = @ID;
    END
    ELSE IF @MasterType = N'Product'
    BEGIN
        SELECT ProductId AS ID, ProductName AS Name, Product_pagename AS PageName,
               DisplayOrder AS [Sequence], [Status], Language_Master_Id
        FROM dbo.Product_Master WHERE ProductId = @ID;
    END
    ELSE IF @MasterType = N'Mapping'
    BEGIN
        SELECT
            m.IndustrySubcategoryId AS ID,
            i.IndustryName AS Name,
            CAST(NULL AS NVARCHAR(300)) AS PageName,
            m.DisplayOrder AS [Sequence],
            m.[Status],
            m.IndustryId,
            m.Category_Master_Id,
            s.SubcategoryName AS RelatedName
        FROM dbo.Industry_Subcategory_Mapping m
        INNER JOIN dbo.Industry_Master i ON i.IndustryId = m.IndustryId
        INNER JOIN dbo.Industry_Subcategory_Master s ON s.SubcategoryId = m.Category_Master_Id
        WHERE m.IndustrySubcategoryId = @ID;
    END
END
GO

/* ========================= Insert ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_Insert
    @Name NVARCHAR(500) = NULL,
    @PageName NVARCHAR(300) = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Language_Master_Id INT = NULL,
    @IndustryId INT = NULL,
    @Category_Master_Id INT = NULL,
    @Create_UserId INT = NULL,
    @MasterType NVARCHAR(50),
    @NewID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET @NewID = 0;
    IF @Status IS NULL SET @Status = 1;

    IF @MasterType = N'Industry'
    BEGIN
        INSERT INTO dbo.Industry_Master
            (IndustryName, Industry_pagename, Language_Master_Id, DisplayOrder, [Status], Create_UserId, CreatedDate)
        VALUES
            (@Name, @PageName, @Language_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME());
        SET @NewID = SCOPE_IDENTITY();
    END
    ELSE IF @MasterType = N'Subcategory'
    BEGIN
        INSERT INTO dbo.Industry_Subcategory_Master
            (SubcategoryName, Language_Master_Id, DisplayOrder, [Status], Create_UserId, CreatedDate)
        VALUES
            (@Name, @Language_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME());
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
    ELSE IF @MasterType = N'Product'
    BEGIN
        INSERT INTO dbo.Product_Master
            (ProductName, Product_pagename, Language_Master_Id, DisplayOrder, [Status], Create_UserId, CreatedDate)
        VALUES
            (@Name, @PageName, @Language_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME());
        SET @NewID = SCOPE_IDENTITY();
    END
    ELSE IF @MasterType = N'Mapping'
    BEGIN
        INSERT INTO dbo.Industry_Subcategory_Mapping
            (IndustryId, Category_Master_Id, DisplayOrder, [Status], Create_UserId, CreatedDate)
        VALUES
            (@IndustryId, @Category_Master_Id, @Sequence, @Status, @Create_UserId, SYSUTCDATETIME());
        SET @NewID = SCOPE_IDENTITY();
    END
END
GO

/* ========================= Update ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_Update
    @ID INT,
    @Name NVARCHAR(500) = NULL,
    @PageName NVARCHAR(300) = NULL,
    @Sequence INT,
    @Language_Master_Id INT = NULL,
    @IndustryId INT = NULL,
    @Category_Master_Id INT = NULL,
    @Update_UserId INT = NULL,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
    BEGIN
        UPDATE dbo.Industry_Master
        SET IndustryName = @Name,
            Industry_pagename = @PageName,
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE IndustryId = @ID;
    END
    ELSE IF @MasterType = N'Subcategory'
    BEGIN
        UPDATE dbo.Industry_Subcategory_Master
        SET SubcategoryName = @Name,
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE SubcategoryId = @ID;
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        UPDATE dbo.Application_Master
        SET ApplicationName = @Name,
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE ApplicationId = @ID;
    END
    ELSE IF @MasterType = N'Product'
    BEGIN
        UPDATE dbo.Product_Master
        SET ProductName = @Name,
            Product_pagename = @PageName,
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE ProductId = @ID;
    END
    ELSE IF @MasterType = N'Mapping'
    BEGIN
        UPDATE dbo.Industry_Subcategory_Mapping
        SET IndustryId = @IndustryId,
            Category_Master_Id = @Category_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE IndustrySubcategoryId = @ID;
    END
END
GO

/* ========================= Activate ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_Activate
    @ID INT,
    @MasterType NVARCHAR(50),
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
        UPDATE dbo.Industry_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Subcategory'
        UPDATE dbo.Industry_Subcategory_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE SubcategoryId = @ID;
    ELSE IF @MasterType = N'Application'
        UPDATE dbo.Application_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'Product'
        UPDATE dbo.Product_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE ProductId = @ID;
    ELSE IF @MasterType = N'Mapping'
        UPDATE dbo.Industry_Subcategory_Mapping SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE IndustrySubcategoryId = @ID;
END
GO

/* ========================= Deactivate ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_Deactivate
    @ID INT,
    @MasterType NVARCHAR(50),
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
        UPDATE dbo.Industry_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Subcategory'
        UPDATE dbo.Industry_Subcategory_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE SubcategoryId = @ID;
    ELSE IF @MasterType = N'Application'
        UPDATE dbo.Application_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'Product'
        UPDATE dbo.Product_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE ProductId = @ID;
    ELSE IF @MasterType = N'Mapping'
        UPDATE dbo.Industry_Subcategory_Mapping SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE IndustrySubcategoryId = @ID;
END
GO

/* ========================= UpdateSequence ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_UpdateSequence
    @ID INT,
    @Sequence INT,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
        UPDATE dbo.Industry_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Subcategory'
        UPDATE dbo.Industry_Subcategory_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE SubcategoryId = @ID;
    ELSE IF @MasterType = N'Application'
        UPDATE dbo.Application_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'Product'
        UPDATE dbo.Product_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE ProductId = @ID;
    ELSE IF @MasterType = N'Mapping'
        UPDATE dbo.Industry_Subcategory_Mapping SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE IndustrySubcategoryId = @ID;
END
GO

/* ========================= Delete (hard delete, optional) ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_Delete
    @ID INT,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
        DELETE FROM dbo.Industry_Master WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Subcategory'
        DELETE FROM dbo.Industry_Subcategory_Master WHERE SubcategoryId = @ID;
    ELSE IF @MasterType = N'Application'
        DELETE FROM dbo.Application_Master WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'Product'
        DELETE FROM dbo.Product_Master WHERE ProductId = @ID;
    ELSE IF @MasterType = N'Mapping'
        DELETE FROM dbo.Industry_Subcategory_Mapping WHERE IndustrySubcategoryId = @ID;
END
GO

/* ========================= Lookup (active only) ========================= */

CREATE OR ALTER PROCEDURE dbo.Catalog_master_GetLookup
    @LookupType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @LookupType = N'Industry'
    BEGIN
        SELECT IndustryId AS Id, IndustryName AS Name
        FROM dbo.Industry_Master
        WHERE [Status] = 1
        ORDER BY DisplayOrder, IndustryName;
    END
    ELSE IF @LookupType = N'Subcategory'
    BEGIN
        SELECT SubcategoryId AS Id, SubcategoryName AS Name
        FROM dbo.Industry_Subcategory_Master
        WHERE [Status] = 1
        ORDER BY DisplayOrder, SubcategoryName;
    END
END
GO

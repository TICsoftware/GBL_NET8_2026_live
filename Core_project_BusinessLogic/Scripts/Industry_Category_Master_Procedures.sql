/*
  Industry Category Master
  Industry / Application / IndustryCategory

  Industry also persists:
    Banner_Image_media_id, Landing_Thumbnail_Image_media_id,
    Intro, Content, Language_Master_Id

  Status: 1 = Active, 0 = Inactive
  Media preview joins dbo.media (ID, file_path).
  Language name joins Language_Master (ID, Language_Name).
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/* ========== Ensure Status column ========== */

IF COL_LENGTH('dbo.Industry_Master', 'Status') IS NULL
    ALTER TABLE dbo.Industry_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Industry_Master_Status DEFAULT (1);
GO

IF COL_LENGTH('dbo.Application_Master', 'Status') IS NULL
    ALTER TABLE dbo.Application_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Application_Master_Status DEFAULT (1);
GO

IF COL_LENGTH('dbo.Industry_Subcategory_Master', 'Status') IS NULL
    ALTER TABLE dbo.Industry_Subcategory_Master ADD [Status] INT NOT NULL CONSTRAINT DF_Industry_Subcategory_Master_Status DEFAULT (1);
GO

/* ========== GetPaged ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_GetPaged
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
            i.IndustryId AS ID,
            i.IndustryName AS Name,
            i.Industry_pagename AS PageName,
            i.DisplayOrder AS [Sequence],
            i.[Status],
            i.Language_Master_Id,
            lm.Language_Name AS LanguageName,
            i.Banner_Image_media_id,
            i.Landing_Thumbnail_Image_media_id,
            mb.file_path AS Banner_Image_Url,
            mt.file_path AS Landing_Thumbnail_Image_Url,
            i.Intro,
            i.Content
        FROM dbo.Industry_Master i
        LEFT JOIN dbo.Language_Master lm ON lm.ID = i.Language_Master_Id
        LEFT JOIN dbo.media mb ON mb.ID = i.Banner_Image_media_id
        LEFT JOIN dbo.media mt ON mt.ID = i.Landing_Thumbnail_Image_media_id
        WHERE (@Search IS NULL OR i.IndustryName LIKE N'%' + @Search + N'%')
        ORDER BY i.DisplayOrder, i.IndustryId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Industry_Master i
        WHERE (@Search IS NULL OR i.IndustryName LIKE N'%' + @Search + N'%');
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        SELECT
            ApplicationId AS ID,
            ApplicationName AS Name,
            CAST(NULL AS NVARCHAR(300)) AS PageName,
            DisplayOrder AS [Sequence],
            [Status],
            Language_Master_Id,
            CAST(NULL AS NVARCHAR(100)) AS LanguageName,
            CAST(NULL AS INT) AS Banner_Image_media_id,
            CAST(NULL AS INT) AS Landing_Thumbnail_Image_media_id,
            CAST(NULL AS NVARCHAR(500)) AS Banner_Image_Url,
            CAST(NULL AS NVARCHAR(500)) AS Landing_Thumbnail_Image_Url,
            CAST(NULL AS NVARCHAR(MAX)) AS Intro,
            CAST(NULL AS NVARCHAR(MAX)) AS Content
        FROM dbo.Application_Master
        WHERE (@Search IS NULL OR ApplicationName LIKE N'%' + @Search + N'%')
        ORDER BY DisplayOrder, ApplicationId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Application_Master
        WHERE (@Search IS NULL OR ApplicationName LIKE N'%' + @Search + N'%');
    END
    ELSE IF @MasterType = N'IndustryCategory'
    BEGIN
        SELECT
            SubcategoryId AS ID,
            SubcategoryName AS Name,
            CAST(NULL AS NVARCHAR(300)) AS PageName,
            DisplayOrder AS [Sequence],
            [Status],
            Language_Master_Id,
            CAST(NULL AS NVARCHAR(100)) AS LanguageName,
            CAST(NULL AS INT) AS Banner_Image_media_id,
            CAST(NULL AS INT) AS Landing_Thumbnail_Image_media_id,
            CAST(NULL AS NVARCHAR(500)) AS Banner_Image_Url,
            CAST(NULL AS NVARCHAR(500)) AS Landing_Thumbnail_Image_Url,
            CAST(NULL AS NVARCHAR(MAX)) AS Intro,
            CAST(NULL AS NVARCHAR(MAX)) AS Content
        FROM dbo.Industry_Subcategory_Master
        WHERE (@Search IS NULL OR SubcategoryName LIKE N'%' + @Search + N'%')
        ORDER BY DisplayOrder, SubcategoryId
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

        SELECT COUNT(1) AS TotalCount
        FROM dbo.Industry_Subcategory_Master
        WHERE (@Search IS NULL OR SubcategoryName LIKE N'%' + @Search + N'%');
    END
END
GO

/* ========== GetById ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_GetById
    @ID INT,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @MasterType = N'Industry'
    BEGIN
        SELECT
            i.IndustryId AS ID,
            i.IndustryName AS Name,
            i.Industry_pagename AS PageName,
            i.DisplayOrder AS [Sequence],
            i.[Status],
            i.Language_Master_Id,
            lm.Language_Name AS LanguageName,
            i.Banner_Image_media_id,
            i.Landing_Thumbnail_Image_media_id,
            mb.file_path AS Banner_Image_Url,
            mt.file_path AS Landing_Thumbnail_Image_Url,
            i.Intro,
            i.Content
        FROM dbo.Industry_Master i
        LEFT JOIN dbo.Language_Master lm ON lm.ID = i.Language_Master_Id
        LEFT JOIN dbo.media mb ON mb.ID = i.Banner_Image_media_id
        LEFT JOIN dbo.media mt ON mt.ID = i.Landing_Thumbnail_Image_media_id
        WHERE i.IndustryId = @ID;
    END
    ELSE IF @MasterType = N'Application'
    BEGIN
        SELECT ApplicationId AS ID, ApplicationName AS Name, CAST(NULL AS NVARCHAR(300)) AS PageName,
               DisplayOrder AS [Sequence], [Status], Language_Master_Id,
               CAST(NULL AS NVARCHAR(100)) AS LanguageName,
               CAST(NULL AS INT) AS Banner_Image_media_id,
               CAST(NULL AS INT) AS Landing_Thumbnail_Image_media_id,
               CAST(NULL AS NVARCHAR(500)) AS Banner_Image_Url,
               CAST(NULL AS NVARCHAR(500)) AS Landing_Thumbnail_Image_Url,
               CAST(NULL AS NVARCHAR(MAX)) AS Intro,
               CAST(NULL AS NVARCHAR(MAX)) AS Content
        FROM dbo.Application_Master WHERE ApplicationId = @ID;
    END
    ELSE IF @MasterType = N'IndustryCategory'
    BEGIN
        SELECT SubcategoryId AS ID, SubcategoryName AS Name, CAST(NULL AS NVARCHAR(300)) AS PageName,
               DisplayOrder AS [Sequence], [Status], Language_Master_Id,
               CAST(NULL AS NVARCHAR(100)) AS LanguageName,
               CAST(NULL AS INT) AS Banner_Image_media_id,
               CAST(NULL AS INT) AS Landing_Thumbnail_Image_media_id,
               CAST(NULL AS NVARCHAR(500)) AS Banner_Image_Url,
               CAST(NULL AS NVARCHAR(500)) AS Landing_Thumbnail_Image_Url,
               CAST(NULL AS NVARCHAR(MAX)) AS Intro,
               CAST(NULL AS NVARCHAR(MAX)) AS Content
        FROM dbo.Industry_Subcategory_Master WHERE SubcategoryId = @ID;
    END
END
GO

/* ========== Insert ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Insert
    @Name NVARCHAR(500),
    @PageName NVARCHAR(300) = NULL,
    @Sequence INT,
    @Status INT = 1,
    @Language_Master_Id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @Landing_Thumbnail_Image_media_id INT = NULL,
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
    BEGIN
        INSERT INTO dbo.Industry_Master
        (
            IndustryName, Industry_pagename,
            Banner_Image_media_id, Landing_Thumbnail_Image_media_id,
            Intro, Content,
            Language_Master_Id, DisplayOrder, [Status],
            Create_UserId, CreatedDate
        )
        VALUES
        (
            @Name, @PageName,
            @Banner_Image_media_id, @Landing_Thumbnail_Image_media_id,
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

/* ========== Update ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Update
    @ID INT,
    @Name NVARCHAR(500),
    @PageName NVARCHAR(300) = NULL,
    @Sequence INT,
    @Language_Master_Id INT = NULL,
    @Banner_Image_media_id INT = NULL,
    @Landing_Thumbnail_Image_media_id INT = NULL,
    @Intro NVARCHAR(MAX) = NULL,
    @Content NVARCHAR(MAX) = NULL,
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
            Banner_Image_media_id = @Banner_Image_media_id,
            Landing_Thumbnail_Image_media_id = @Landing_Thumbnail_Image_media_id,
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
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE ApplicationId = @ID;
    END
    ELSE IF @MasterType = N'IndustryCategory'
    BEGIN
        UPDATE dbo.Industry_Subcategory_Master
        SET SubcategoryName = @Name,
            Language_Master_Id = @Language_Master_Id,
            DisplayOrder = @Sequence,
            Update_UserId = @Update_UserId,
            ModifiedDate = SYSUTCDATETIME()
        WHERE SubcategoryId = @ID;
    END
END
GO

/* ========== Activate ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Activate
    @ID INT,
    @MasterType NVARCHAR(50),
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @MasterType = N'Industry'
        UPDATE dbo.Industry_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Application'
        UPDATE dbo.Application_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'IndustryCategory'
        UPDATE dbo.Industry_Subcategory_Master SET [Status] = 1, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE SubcategoryId = @ID;
END
GO

/* ========== Deactivate ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Deactivate
    @ID INT,
    @MasterType NVARCHAR(50),
    @Update_UserId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    IF @MasterType = N'Industry'
        UPDATE dbo.Industry_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Application'
        UPDATE dbo.Application_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'IndustryCategory'
        UPDATE dbo.Industry_Subcategory_Master SET [Status] = 0, Update_UserId = @Update_UserId, ModifiedDate = SYSUTCDATETIME() WHERE SubcategoryId = @ID;
END
GO

/* ========== UpdateSequence ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_UpdateSequence
    @ID INT,
    @Sequence INT,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF @MasterType = N'Industry'
        UPDATE dbo.Industry_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Application'
        UPDATE dbo.Application_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'IndustryCategory'
        UPDATE dbo.Industry_Subcategory_Master SET DisplayOrder = @Sequence, ModifiedDate = SYSUTCDATETIME() WHERE SubcategoryId = @ID;
END
GO

/* ========== Delete ========== */

CREATE OR ALTER PROCEDURE dbo.Industry_Category_master_Delete
    @ID INT,
    @MasterType NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    IF @MasterType = N'Industry'
        DELETE FROM dbo.Industry_Master WHERE IndustryId = @ID;
    ELSE IF @MasterType = N'Application'
        DELETE FROM dbo.Application_Master WHERE ApplicationId = @ID;
    ELSE IF @MasterType = N'IndustryCategory'
        DELETE FROM dbo.Industry_Subcategory_Master WHERE SubcategoryId = @ID;
END
GO

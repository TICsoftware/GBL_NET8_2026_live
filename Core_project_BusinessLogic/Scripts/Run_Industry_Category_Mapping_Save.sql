/*
  Run this file only.
  Updates Industry_Category_Mapping_Save so the same IndustryId + CategoryId
  cannot be mapped again.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

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

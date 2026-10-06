/* Run on DB (local + UAT) before using drag-drop sequence reorder */
CREATE OR ALTER PROCEDURE dbo.Context_Detail_Update_Block_Sequence
(
    @context_group_id UNIQUEIDENTIFIER,
    @sequence INT,
    @mode NVARCHAR(20) = N'main', -- main | temp | reprocess
    @Updated_UserID INT = 1
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @mode = N'temp'
    BEGIN
        UPDATE dbo.context_details_temp
        SET content = CAST(@sequence AS NVARCHAR(50)),
            Updated_UserID = @Updated_UserID,
            Updated_Date = SYSUTCDATETIME()
        WHERE context_group_id = @context_group_id
          AND context_field_id IN (10, 47);
        RETURN;
    END

    IF @mode = N'reprocess'
    BEGIN
        UPDATE dbo.context_details_reprocess
        SET content = CAST(@sequence AS NVARCHAR(50)),
            Updated_UserID = @Updated_UserID,
            Updated_Date = SYSUTCDATETIME()
        WHERE context_group_id = @context_group_id
          AND context_field_id IN (10, 47);
        RETURN;
    END

    UPDATE dbo.context_details
    SET content = CAST(@sequence AS NVARCHAR(50)),
        Updated_UserID = @Updated_UserID,
        Updated_Date = SYSUTCDATETIME()
    WHERE context_group_id = @context_group_id
      AND context_field_id IN (10, 47);
END
GO

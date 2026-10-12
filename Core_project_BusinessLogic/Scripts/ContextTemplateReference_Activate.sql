/*
  Activate a soft-deleted / inactive context_template_reference mapping.
  Sets Status = 1 for the given ID.
*/

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE dbo.sp_ContextTemplateReference_Activate
    @ID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF @ID IS NULL OR @ID <= 0
        THROW 50001, 'Invalid mapping id.', 1;

    IF NOT EXISTS (SELECT 1 FROM dbo.context_template_reference WHERE ID = @ID)
        THROW 50002, 'Mapping not found.', 1;

    UPDATE dbo.context_template_reference
    SET Status = 1
    WHERE ID = @ID;
END
GO

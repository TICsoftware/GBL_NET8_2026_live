/*
  Committees of the Board

  CMS page name: committees-of-the-board
  Called from CorporateGovernance_BAL as:
    EXEC dbo.sp_GetContentComponentData
         @pagename    = N'committees-of-the-board',
         @LanguageID  = 1,
         @GeographyID = 1;

  Result set 2 (table index 1) = context details:
    is_block = 1  → Component Title (committee name)
    is_block = 0  → Title (member name), Designation (role), Sequence (order)
*/

EXEC dbo.sp_GetContentComponentData
    @pagename    = N'committees-of-the-board',
    @LanguageID  = 1,
    @GeographyID = 1;
GO

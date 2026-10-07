/*
    Schema for the Work With Us career enquiry feature.

    Entry point:
      GBL_MVC/Views/Career/workwithus.cshtml
         -> CareerController.SubmitWorkWithUs
         -> WorkWithUsEnquiry_BAL/DAL

    Rate limit: same IP may not submit more than 3 times within 5 minutes.
    SP returns a single row/column result set consumed as dt.Rows[0][0]:
      'updated'  -> insert succeeded
      'exceeds'  -> IP exceeded 3 submissions in last 5 minutes
      otherwise  -> treated as an error message
*/

-- ============================================================
-- Table: WorkWithUsEnquiry
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WorkWithUsEnquiry')
BEGIN
    CREATE TABLE dbo.WorkWithUsEnquiry
    (
        EnquiryId       INT IDENTITY(1,1) PRIMARY KEY,
        FullName        NVARCHAR(100)   NOT NULL,
        Email           NVARCHAR(500)   NOT NULL,
        Expertise       NVARCHAR(1000)  NULL,
        Address         NVARCHAR(2000)  NULL,
        Designation     NVARCHAR(1000)  NULL,
        ResumePath      NVARCHAR(1000)  NULL,
        Message         NVARCHAR(MAX)   NULL,
        NotRobot        BIT             NOT NULL DEFAULT (0),
        IPAddress       NVARCHAR(50)    NULL,
        CreatedDate     DATETIME        NOT NULL DEFAULT (GETDATE())
    );
END
ELSE
BEGIN
    -- Align existing table columns to current sizes
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN FullName NVARCHAR(100) NOT NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN Email NVARCHAR(500) NOT NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN Expertise NVARCHAR(1000) NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN Address NVARCHAR(2000) NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN Designation NVARCHAR(1000) NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN ResumePath NVARCHAR(1000) NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN Message NVARCHAR(MAX) NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN NotRobot BIT NOT NULL;
    ALTER TABLE dbo.WorkWithUsEnquiry ALTER COLUMN IPAddress NVARCHAR(50) NULL;
END
GO

-- Index to support IP rate-limit lookups
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_WorkWithUsEnquiry_IP_CreatedDate'
      AND object_id = OBJECT_ID('dbo.WorkWithUsEnquiry')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_WorkWithUsEnquiry_IP_CreatedDate
        ON dbo.WorkWithUsEnquiry (IPAddress, CreatedDate DESC);
END
GO

-- ============================================================
-- SP: sp_AddWorkWithUsEnquiry
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.sp_AddWorkWithUsEnquiry
    @FullName       NVARCHAR(100),
    @Email          NVARCHAR(500),
    @Expertise      NVARCHAR(1000)  = NULL,
    @Address        NVARCHAR(2000)  = NULL,
    @Designation    NVARCHAR(1000)  = NULL,
    @ResumePath     NVARCHAR(1000)  = NULL,
    @Message        NVARCHAR(MAX)   = NULL,
    @NotRobot       BIT,
    @IPAddress      NVARCHAR(50)    = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Result NVARCHAR(200);
    DECLARE @MaxSubmissions INT = 3;
    DECLARE @WindowMinutes INT = 5;

    IF (
        SELECT COUNT(1)
        FROM dbo.WorkWithUsEnquiry
        WHERE IPAddress = @IPAddress
          AND CreatedDate >= DATEADD(MINUTE, -@WindowMinutes, GETDATE())
    ) >= @MaxSubmissions
    BEGIN
        SET @Result = 'exceeds';
    END
    ELSE
    BEGIN
        BEGIN TRY
            INSERT INTO dbo.WorkWithUsEnquiry
                (FullName, Email, Expertise, Address, Designation, ResumePath, Message, NotRobot, IPAddress)
            VALUES
                (@FullName, @Email, @Expertise, @Address, @Designation, @ResumePath, @Message, @NotRobot, @IPAddress);

            SET @Result = 'updated';
        END TRY
        BEGIN CATCH
            SET @Result = ERROR_MESSAGE();
        END CATCH
    END

    SELECT @Result AS Result;
END
GO

/*
    Schema for the Contact Us enquiry feature.

    Shared by BOTH entry points in the application:
      1) GBL_MVC/Views/CommonComponent/_contactus_enquiry_form.cshtml
         -> ContactusController.SubmitEnquiry
      2) GBL_MVC/Views/CommonComponent/_home_contact_modal_form.cshtml (site-wide modal)
         -> ContactusController.SubmitHomeEnquiry

    Both actions call the same ContactusEnquiry_BAL/DAL, so they read/write
    the same table and stored procedures below.
*/

-- ============================================================
-- Table: ContactUsEnquiry
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ContactUsEnquiry')
BEGIN
    CREATE TABLE dbo.ContactUsEnquiry
    (
        EnquiryId       INT IDENTITY(1,1) PRIMARY KEY,
        FullName        NVARCHAR(100)   NOT NULL,
        Designation     NVARCHAR(100)   NULL,
        Organisation    NVARCHAR(200)   NOT NULL,
        Email           NVARCHAR(150)   NOT NULL,
        Phone           NVARCHAR(20)    NOT NULL,
        CityId          INT             NOT NULL,
        InterestId      INT             NOT NULL,
        Query           NVARCHAR(1000)  NULL,
        Consent         BIT             NOT NULL DEFAULT (0),
        IPAddress       NVARCHAR(50)    NULL,
        CreatedDate     DATETIME        NOT NULL DEFAULT (GETDATE())
    );
END
GO

-- ============================================================
-- Master: CityMaster
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'CityMaster')
BEGIN
    CREATE TABLE dbo.CityMaster
    (
        CityId          INT IDENTITY(1,1) PRIMARY KEY,
        CityName        NVARCHAR(100)   NOT NULL,
        Sequence        INT             NOT NULL DEFAULT (0),
        Status          BIT             NOT NULL DEFAULT (1),
        CreatedDate     DATETIME        NOT NULL DEFAULT (GETDATE())
    );
END
GO

-- ============================================================
-- Master: AreaOfInterestMaster
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AreaOfInterestMaster')
BEGIN
    CREATE TABLE dbo.AreaOfInterestMaster
    (
        InterestId      INT IDENTITY(1,1) PRIMARY KEY,
        InterestName    NVARCHAR(150)   NOT NULL,
        Sequence        INT             NOT NULL DEFAULT (0),
        Status          BIT             NOT NULL DEFAULT (1),
        CreatedDate     DATETIME        NOT NULL DEFAULT (GETDATE())
    );
END
GO

-- ============================================================
-- SP: sp_AddContactUsEnquiry
-- Called by ContactusEnquiry_DAL.AddContactUsEnquiry_DAL(...)
-- Returns a single row/column result set consumed as dt.Rows[0][0]:
--   'updated'  -> insert succeeded
--   'exceeds'  -> caller already submitted too many enquiries today (per IP)
--   otherwise  -> treated as an error message
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.sp_AddContactUsEnquiry
    @FullName       NVARCHAR(100),
    @Designation    NVARCHAR(100)   = NULL,
    @Organisation   NVARCHAR(200),
    @Email          NVARCHAR(150),
    @Phone          NVARCHAR(20),
    @CityId         INT,
    @InterestId     INT,
    @Query          NVARCHAR(1000)  = NULL,
    @Consent        BIT,
    @IPAddress      NVARCHAR(50)    = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Result NVARCHAR(200);
    DECLARE @DailyLimit INT = 5;

    IF (SELECT COUNT(1)
        FROM dbo.ContactUsEnquiry
        WHERE IPAddress = @IPAddress
          AND CreatedDate >= CAST(GETDATE() AS DATE)) >= @DailyLimit
    BEGIN
        SET @Result = 'exceeds';
    END
    ELSE
    BEGIN
        BEGIN TRY
            INSERT INTO dbo.ContactUsEnquiry
                (FullName, Designation, Organisation, Email, Phone, CityId, InterestId, Query, Consent, IPAddress)
            VALUES
                (@FullName, @Designation, @Organisation, @Email, @Phone, @CityId, @InterestId, @Query, @Consent, @IPAddress);

            SET @Result = 'updated';
        END TRY
        BEGIN CATCH
            SET @Result = ERROR_MESSAGE();
        END CATCH
    END

    SELECT @Result AS Result;
END
GO

-- ============================================================
-- SP: GetCityMaster
-- Called by ContactusEnquiry_DAL.GetCityList_DAL()
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.GetCityMaster
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CityId, CityName
    FROM dbo.CityMaster
    WHERE Status = 1
    ORDER BY Sequence, CityName;
END
GO

-- ============================================================
-- SP: GetAreaOfInterestMaster
-- Called by ContactusEnquiry_DAL.GetAreaOfInterestList_DAL()
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.GetAreaOfInterestMaster
AS
BEGIN
    SET NOCOUNT ON;

    SELECT InterestId, InterestName
    FROM dbo.AreaOfInterestMaster
    WHERE Status = 1
    ORDER BY Sequence, InterestName;
END
GO

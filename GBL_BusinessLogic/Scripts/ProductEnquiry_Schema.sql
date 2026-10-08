/*
    Schema for the Product Order Sample / Enquiry feature.

    Entry point:
      GBL_MVC/Views/Enquiry/Index.cshtml
         -> EnquiryController.SubmitEnquiry
         -> ProductEnquiry_BAL/DAL

    URL: /enquiry/{product-pagename}

    Rate limit: same IP may not submit more than 3 times within 5 minutes.
    SP returns a single row/column result set consumed as dt.Rows[0][0]:
      'updated'  -> insert succeeded
      'exceeds'  -> IP exceeded 3 submissions in last 5 minutes
      otherwise  -> treated as an error message
*/

-- ============================================================
-- Table: ProductEnquiry
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ProductEnquiry')
BEGIN
    CREATE TABLE dbo.ProductEnquiry
    (
        EnquiryId           INT IDENTITY(1,1) PRIMARY KEY,
        ProductName         NVARCHAR(500)   NOT NULL,
        ProductPageName     NVARCHAR(300)   NULL,
        FullName            NVARCHAR(100)   NOT NULL,
        CompanyName         NVARCHAR(200)   NOT NULL,
        StreetAddress       NVARCHAR(500)   NOT NULL,
        City                NVARCHAR(100)   NOT NULL,
        State               NVARCHAR(100)   NOT NULL,
        Country             NVARCHAR(100)   NOT NULL,
        Phone               NVARCHAR(20)    NULL,
        CountryCode         NVARCHAR(10)    NULL,
        Mobile              NVARCHAR(20)    NOT NULL,
        Fax                 NVARCHAR(20)    NULL,
        Email               NVARCHAR(500)   NOT NULL,
        BusinessType        NVARCHAR(200)   NOT NULL,
        EnquiryDetails      NVARCHAR(MAX)   NULL,
        AcceptTerms         BIT             NOT NULL DEFAULT (0),
        NotRobot            BIT             NOT NULL DEFAULT (0),
        IPAddress           NVARCHAR(50)    NULL,
        CreatedDate         DATETIME        NOT NULL DEFAULT (GETDATE())
    );
END
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_ProductEnquiry_IP_CreatedDate'
      AND object_id = OBJECT_ID('dbo.ProductEnquiry')
)
BEGIN
    CREATE NONCLUSTERED INDEX IX_ProductEnquiry_IP_CreatedDate
        ON dbo.ProductEnquiry (IPAddress, CreatedDate DESC);
END
GO

-- ============================================================
-- SP: sp_AddProductEnquiry
-- ============================================================
CREATE OR ALTER PROCEDURE dbo.sp_AddProductEnquiry
    @ProductName        NVARCHAR(500),
    @ProductPageName    NVARCHAR(300)   = NULL,
    @FullName           NVARCHAR(100),
    @CompanyName        NVARCHAR(200),
    @StreetAddress      NVARCHAR(500),
    @City               NVARCHAR(100),
    @State              NVARCHAR(100),
    @Country            NVARCHAR(100),
    @Phone              NVARCHAR(20)    = NULL,
    @CountryCode        NVARCHAR(10)    = NULL,
    @Mobile             NVARCHAR(20),
    @Fax                NVARCHAR(20)    = NULL,
    @Email              NVARCHAR(500),
    @BusinessType       NVARCHAR(200),
    @EnquiryDetails     NVARCHAR(MAX)   = NULL,
    @AcceptTerms        BIT,
    @NotRobot           BIT,
    @IPAddress          NVARCHAR(50)    = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Result NVARCHAR(200);
    DECLARE @MaxSubmissions INT = 3;
    DECLARE @WindowMinutes INT = 5;

    IF (
        SELECT COUNT(1)
        FROM dbo.ProductEnquiry
        WHERE IPAddress = @IPAddress
          AND CreatedDate >= DATEADD(MINUTE, -@WindowMinutes, GETDATE())
    ) >= @MaxSubmissions
    BEGIN
        SET @Result = 'exceeds';
    END
    ELSE
    BEGIN
        BEGIN TRY
            INSERT INTO dbo.ProductEnquiry
            (
                ProductName, ProductPageName, FullName, CompanyName, StreetAddress,
                City, State, Country, Phone, CountryCode, Mobile, Fax, Email,
                BusinessType, EnquiryDetails, AcceptTerms, NotRobot, IPAddress
            )
            VALUES
            (
                @ProductName, @ProductPageName, @FullName, @CompanyName, @StreetAddress,
                @City, @State, @Country, @Phone, @CountryCode, @Mobile, @Fax, @Email,
                @BusinessType, @EnquiryDetails, @AcceptTerms, @NotRobot, @IPAddress
            );

            SET @Result = 'updated';
        END TRY
        BEGIN CATCH
            SET @Result = ERROR_MESSAGE();
        END CATCH
    END

    SELECT @Result AS Result;
END
GO

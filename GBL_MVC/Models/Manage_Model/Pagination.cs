namespace GBL_MVC;


public class PagedResult 
{   
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
}
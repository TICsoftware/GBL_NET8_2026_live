using Core_project_BusinessLogic.BAL;
using Core_project_BusinessLogic.Entity;
using GBL_MVC;

namespace GBL_MVC.Services
{
    /// <summary>
    /// Loads industry, application, and category options for the products finder.
    /// Scoped so a request that renders the partial more than once hits the database once.
    /// </summary>
    public class ProductFinderService
    {
        private readonly IConfiguration _configuration;
        private Industry_Public_Page? _page;

        public ProductFinderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Industry_Public_Page GetPage()
        {
            if (_page != null)
                return _page;

            try
            {
                var bal = new Industry_Public_BAL(_configuration);
                _page = bal.GetIndex("industries", 1, new Industry_Public_Filter
                {
                    PageNumber = 1,
                    PageSize = 1,
                    LanguageId = 1
                }) ?? new Industry_Public_Page();
            }
            catch (Exception ex)
            {
                FileLogger.LogError("/CommonComponent/_products_Finder :", ex);
                _page = new Industry_Public_Page();
            }

            return _page;
        }
    }
}

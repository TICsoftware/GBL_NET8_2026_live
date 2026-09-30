using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.BAL
{
    public class Product_Public_BAL
    {
        private readonly Product_Public_DAL _dal;

        public Product_Public_BAL(IConfiguration config)
        {
            _dal = new Product_Public_DAL(config);
        }

        public Product_Public_Page GetIndex(string pageName, int languageId, Product_Public_Filter filter)
        {
            var value = (pageName ?? string.Empty).Trim().Trim('/');
            if (string.IsNullOrWhiteSpace(value) ||
                string.Equals(value, "Products", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(value, "Products/Index", StringComparison.OrdinalIgnoreCase))
            {
                value = "products";
            }
            return _dal.GetIndex(value, languageId, filter ?? new Product_Public_Filter());
        }

        public Product_Public_Page GetPaged(Product_Public_Filter filter)
        {
            return _dal.GetPaged(filter ?? new Product_Public_Filter());
        }

        public Product_Inside_Page GetInside(string pageName, int languageId)
        {
            return _dal.GetInside((pageName ?? string.Empty).Trim().Trim('/'), languageId);
        }
    }
}

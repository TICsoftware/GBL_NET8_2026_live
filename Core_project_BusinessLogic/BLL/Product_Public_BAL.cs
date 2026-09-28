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

        public Product_Inside_Page GetInside(string pageName, int languageId)
        {
            return _dal.GetInside((pageName ?? string.Empty).Trim().Trim('/'), languageId);
        }
    }
}

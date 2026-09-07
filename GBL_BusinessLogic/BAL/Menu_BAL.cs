using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class Menu_BAL : Menu_DAL
    {
        private readonly IConfiguration _configuration;
        public Menu_BAL(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }



        public List<MenuHeader> GetFrontendHeaderMenus()
        {
            return GetFrontendHeaderMenus_dal();
        }


          public List<MenuFooter> GetFrontendFooterMenus()
        {
            return GetFrontendFooterMenus_dal();
        }

    }
}
using System.Collections.Generic;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.BAL
{
    public class MenuHeader_BAL : MenuHeader_DAL
    {
        //private readonly MenuHeader_DAL _dal;

        public MenuHeader_BAL(IConfiguration config) : base(config)
        {
            //_dal = new MenuHeader_DAL(config);
        }

        public MenuHeader GetMenuById(int id)
        {
            return GetMenuById_dal(id);
        }

        public void SaveMenu(MenuHeader model)
        {
            SaveMenu_dal(model);
        }


        public (List<MenuHeader> Data, int Total) GetPagedMenus(string search, int? isActive, int page, int pageSize)
        {
            return GetPagedMenus_dal(search, isActive, page, pageSize);
        }

        public List<MenuHeader> GetMenus()
        {
            return GetMenus_dal();
        }

        public List<MenuHeader> GetParentMenus()
        {
            return GetParentMenus_dal();
        }


        public void Deactivate(int id, int userId)
        {
            Deactivate_dal(id, userId); ;
        }



    }
}
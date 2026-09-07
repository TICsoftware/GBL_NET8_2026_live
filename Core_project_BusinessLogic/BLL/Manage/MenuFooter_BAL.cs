using System.Collections.Generic;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.BAL
{
    public class MenuFooter_BAL : MenuFooter_DAL
    {
        //private readonly MenuHeader_DAL _dal;

        public MenuFooter_BAL(IConfiguration config) : base(config)
        {
            //_dal = new MenuHeader_DAL(config);
        }

        public MenuFooter GetFooterById(int id)
        {
            return GetFooterById_dal(id);
        }

        public void SaveFooter(MenuFooter model)
        {
            SaveFooter_dal(model);
        }

        public void UpdateFooter(MenuFooter model)
        {
            UpdateFooter_dal(model);
        }

        public (List<MenuFooter> Data, int Total) GetPagedFooters(
            string search,
            int? status,
            int page,
            int pageSize)
        {
            return GetPagedFooters_dal(search, status, page, pageSize);
        }

        public List<MenuFooter> GetFooters()
        {
            return GetFooters_dal();
        }

        public List<MenuFooter> GetParentFooters()
        {
            return GetParentFooters_dal();
        }

        public void Deactivate(int id, int userId)
        {
            Deactivate_dal(id, userId); ;
        }


    }
}
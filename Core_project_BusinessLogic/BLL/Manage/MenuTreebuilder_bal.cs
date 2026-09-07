
using System;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Core_project_BusinessLogic.BAL
{
    public class MenuTreebuilder_bal
    {
        private readonly CmsMenu_DAL _dal;

        private readonly IConfiguration _config;

        public MenuTreebuilder_bal(IConfiguration config)
        {
            _config = config;
            _dal = new CmsMenu_DAL(config);
        }


        // 👉 Get Templates
        public Dictionary<int, string> GetTemplates()
        {
            return _dal.GetTemplates();
        }

        // 👉 Get Menu Items
        public List<CmsMenu> GetMenus()
        {
            return _dal.GetMenus();
        }
        public List<MenuTemplate> GetTemplateList()
        {
            return _dal.GetTemplateList();
        }
        // public void GenerateMenuHtml()
        // {
        //     var menus = _dal.GetMenus(); // existing DAL

        //     MenuRenderer renderer = new MenuRenderer();

        //     string html = renderer.RenderMenu(menus);

        //     _dal.SaveGeneratedMenuHtml("MainMenu", html);
        // }

        public void GenerateMenuHtml()
        {
            var menus = _dal.GetMenus();

            var templates = _dal.GetTemplates();

            MenuRenderer renderer = new MenuRenderer();

            string html = renderer.RenderMenu(menus, templates);

            _dal.SaveGeneratedMenuHtml("MainMenu", html);
        }


        public string GetMenuHtml()
        {
            return _dal.GetGeneratedMenuHtml("MainMenu");
        }

        // 👉 Build Tree Structure
        public List<CmsMenu> BuildTree(List<CmsMenu> all)
        {
            var roots = all.Where(x => x.ParentId == null)
                           .OrderBy(x => x.DisplayOrder)
                           .ToList();

            foreach (var r in roots)
                r.Children = GetChildren(r.MenuId, all);

            return roots;
        }

        private List<CmsMenu> GetChildren(int id, List<CmsMenu> all)
        {
            var children = all.Where(x => x.ParentId == id)
                              .OrderBy(x => x.DisplayOrder)
                              .ToList();

            foreach (var c in children)
                c.Children = GetChildren(c.MenuId, all);

            return children;
        }

        // 👉 Save Generated HTML
        public void SaveMenuHtml(string name, string html)
        {
            _dal.SaveMenuHtml(name, html);
        }

        // 👉 Load HTML
        public string GetMenuHtml(string name)
        {
            return _dal.GetMenuHtml(name);
        }
        public CmsMenu GetMenuById(int id)
        {
            return _dal.GetMenuById(id);
        }

        public void SaveMenu(CmsMenu model)
        {
            _dal.SaveMenu(model);
        }

    }
}

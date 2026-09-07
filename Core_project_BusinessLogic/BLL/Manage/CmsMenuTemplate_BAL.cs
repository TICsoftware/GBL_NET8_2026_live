
using System;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;

namespace Core_project_BusinessLogic.BAL
{

    public class CmsMenuTemplate_BAL
    {
        private readonly CmsMenuTemplate_DAL _dal;


        public CmsMenuTemplate_BAL(IConfiguration config)
        {
            _dal = new CmsMenuTemplate_DAL(config);
        }


        public CmsMenuTemplate GetTemplateById(int id)
        {
            return _dal.GetTemplateById(id);
        }

        public void SaveTemplate(CmsMenuTemplate model)
        {
            _dal.SaveTemplate(model);
        }
        public List<CmsMenuTemplate> GetTemplates()
        {
            return _dal.GetTemplates();
        }
    }
}
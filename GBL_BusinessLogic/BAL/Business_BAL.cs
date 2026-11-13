using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.DAL;
using GBL_BusinessLogic.Entity;
using GBL_BusinessLogic;

namespace GBL_BusinessLogic.BAL
{
    public class Business_BAL : BasePageBAL
    {
        public Business_BAL(IConfiguration configuration) : base(configuration)
        {
        }


        public BusinessModel GetBusiness_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new BusinessModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);

                var ds_subsection = GetSubSectionFirstComponentData_DAL(model.Content.ContId, languageId);

                if (ds_subsection.Tables.Count >= 1 && ds_subsection.Tables[0].Rows.Count > 0)
                {
                    model.Section_List = Config_Application_Website.MapArticleList(ds_subsection.Tables[0]);

                    if (ds_subsection.Tables.Count >= 2 && ds_subsection.Tables[1].Rows.Count > 0)
                    {
                        var groupedData = GetGroupedComponents(ds_subsection.Tables[1]);
                        // model.Components = groupedData; 

                        model.Subsections_Components = MapComponents(groupedData, 1);
                    }
                }
            }

            return model;
        }

        public BusinessModel GetInsideBusiness_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new BusinessModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }
            if (ds.Tables.Count >= 2 && ds.Tables[1].Rows.Count > 0)
            {
                ComponentModel objModel=new();
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                foreach(var item in groupedData.Where( m => m.Sequence > 1))
                {

                }
                model.Subsections_Components = MapComponents(groupedData, 1);
            } 
            return model;
        }

    }
}
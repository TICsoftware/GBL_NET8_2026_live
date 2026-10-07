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
    public class Careers_BAL : BasePageBAL
    {
        public Careers_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public CareersModel GetLearningDevelopment_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new CareersModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                model.Learning_Development_List = MapComponents(groupedData, 1);
            }

            return model;
        }


        public CareersModel GetCareers_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new CareersModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                model.Why_Join_Godavari_Biorefineries_List = MapComponents(groupedData, 1);
                model.Life_at_Godavari_List = MapComponents(groupedData, 2);
                model.Career_CTA_List = MapComponents(groupedData, 3);
            }

            return model;
        }

        public CareersModel GetWorkWithUs_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new CareersModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                model.Components = GetGroupedComponents(ds.Tables[1]);
            }

            return model;
        }
    }
}

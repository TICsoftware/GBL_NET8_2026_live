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
    public class Media_BAL : BasePageBAL
    {
        public Media_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public MediaModel GetPressRelease_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new MediaModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            // Components
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;
            }

            return model;
        }


        public MediaModel GetPressRelease_Inside_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new MediaModel();

            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                model.Related_Articles_List = Config_Application_Website.MapArticleList(ds.Tables[2]);
            }


            return model;
        }






    }
}
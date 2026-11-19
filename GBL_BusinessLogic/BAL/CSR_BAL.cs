using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;
using GBL_BusinessLogic;

namespace GBL_BusinessLogic.BAL
{
    public class CSR_BAL : BasePageBAL
    {
        public CSR_BAL(IConfiguration configuration) : base(configuration)
        {
        }
        public CSRModel GetPageData_GroupedComponents_BAL(string pagename, int languageId, int geographyId)
        {

            var model = new CSRModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count >= 1 && ds.Tables[1].Rows.Count > 0)
            {
                model.Components = GetGroupedComponents(ds.Tables[1]);
                model.Component_Dictionary = MapComponentsByContextMasterId(model.Components);

                // Flat ordered list (all sequences) for views that still use Subsections_Components
                model.ListComponents = model.Component_Dictionary
                    .OrderBy(kv => kv.Key)
                    .SelectMany(kv => kv.Value)
                    .ToList();
            }

            return model;
    }
        
    }
}
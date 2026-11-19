using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Configuration;
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
            Dictionary<int, List<ComponentModel>> Component_Lists = [];
            var model = new BusinessModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

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
                        Component_Lists = MapComponentsBySequence(groupedData);
                        model.Subsections_Components = Component_Lists.TryGetValue(1, out var seq1)
                            ? seq1
                            : MapComponents(groupedData, 1);
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
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Component_Lists = MapComponentsBySequence(groupedData);

                // Flat ordered list (all sequences) for views that still use Subsections_Components
                model.Subsections_Components = model.Component_Lists
                    .OrderBy(kv => kv.Key)
                    .SelectMany(kv => kv.Value)
                    .ToList();
            }
            Dictionary<int, List<ComponentModel>> dictionary_Componentlists = [];
            var ds_parent_component = GetParentComponentData_DAL(pagename, languageId, geographyId);
            if (ds_parent_component.Tables.Count >= 1 && ds_parent_component.Tables[0].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds_parent_component.Tables[0]);
                dictionary_Componentlists = MapComponentsBySequence(groupedData);
                model.Parent_section_Components = dictionary_Componentlists.TryGetValue(1, out var seq1)
                    ? seq1
                    : MapComponents(groupedData, 1);

            }
            return model;
        }

       


    }
}

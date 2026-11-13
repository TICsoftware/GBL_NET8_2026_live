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
                        model.Component_Lists = MapComponentsBySequence(groupedData);
                        model.Subsections_Components = model.Component_Lists.TryGetValue(1, out var seq1)
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

            return model;
        }

        /// <summary>
        /// Builds a separate ComponentModel list for every distinct component_sequence.
        /// </summary>
        private Dictionary<int, List<ComponentModel>> MapComponentsBySequence(List<ComponentGroup> groupedData)
        {
            var result = new Dictionary<int, List<ComponentModel>>();
            if (groupedData == null || groupedData.Count == 0)
                return result;

            foreach (var seq in groupedData.Select(g => g.Sequence).Distinct().OrderBy(s => s))
            {
                var list = MapComponents(groupedData, seq);
                foreach (var item in list)
                {
                    if (item.Sequence <= 0)
                        item.Sequence = seq;
                }
                result[seq] = list;
            }

            return result;
        }
    }
}

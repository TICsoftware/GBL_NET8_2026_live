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
            DataSet? ds = null;

            foreach (var name in GetLearningDevelopmentPageNames(pagename))
            {
                ds = GetContentComponentData_DAL(name, languageId, geographyId);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    model.Content = MapContent(ds.Tables[0].Rows[0]);
                    if ((model.Content?.ContId ?? 0) > 0)
                    {
                        break;
                    }
                }
            }

            if (ds != null && ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                // Legacy / shared career inside pages (social, diversity, etc.)
                model.Learning_Development_List = MapComponents(groupedData, 1);

                // New Learning & Development layout (/careers/learning-and-development)
                // 1 Empowering Growth (intro block + cards)
                // 2 Knowledge Sharing
                // 3 Expanding Professional Horizons
                // 4 Our Learning Philosophy
                model.Empowering_Growth_List = MapComponents(groupedData, 1);
                model.Knowledge_Sharing_List = MapComponents(groupedData, 2);
                model.Expanding_Horizons_List = MapComponents(groupedData, 3);
                model.Learning_Philosophy_List = MapComponents(groupedData, 4);
            }

            return model;
        }

        private static IEnumerable<string> GetLearningDevelopmentPageNames(string? pagename)
        {
            var requested = (pagename ?? "").Trim().TrimStart('/');
            var names = new List<string>();

            void Add(string? value)
            {
                var name = (value ?? "").Trim().TrimStart('/');
                if (string.IsNullOrWhiteSpace(name)) return;
                if (!names.Exists(x => x.Equals(name, StringComparison.OrdinalIgnoreCase)))
                {
                    names.Add(name);
                }
            }

            Add(requested);

            var last = requested.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? "";
            Add(last);
            if (!string.IsNullOrWhiteSpace(last))
            {
                Add("careers/" + last);
                Add("sustainability/" + last);
            }

            // Only for the Learning & Development page — do not bleed into wellbeing/social/etc.
            var isLearningDevelopment = last.Equals("learning-and-development", StringComparison.OrdinalIgnoreCase)
                || last.Equals("learning-development", StringComparison.OrdinalIgnoreCase)
                || requested.IndexOf("learning-and-development", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isLearningDevelopment)
            {
                Add("learning-and-development");
                Add("careers/learning-and-development");
                Add("learning-development");
                Add("LearningDevelopment");
            }

            return names;
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

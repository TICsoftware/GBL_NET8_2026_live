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



        public SustainabilityModel GetCarbonLightCircularity_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new SustainabilityModel();
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

                model.Carbon_light_circularity_Section_List = MapComponents(groupedData, 1);
            }


            return model;
        }



        public SustainabilityModel GetSustainabilityReports_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new SustainabilityModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            // Content
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }


            if (ds.Tables.Count > 2 && ds.Tables[2].Rows.Count > 0)
            {
                model.SectionArticles_List = Config_Application_Website.MapArticleList(ds.Tables[2]);
            }

            if (ds.Tables.Count > 3 && ds.Tables[3].Rows.Count > 0)
            {
                model.TotalCount = Convert.ToInt32(ds.Tables[3].Rows[0]["TotalCount"]);
            }


            return model;
        }




        public SustainabilityModel GetSustainabilityReports_page_wise_BAL(int cont_id, int page, int pageSize)
        {
            var model = new SustainabilityModel();

            var ds = Get_SustainabilityReports_page_wise_DAL(cont_id, page, pageSize);

            if (ds == null || ds.Tables.Count == 0)
            {
                return model;
            }

            // Article list
            if (ds.Tables[0].Rows.Count > 0)
            {
                model.SectionArticles_List = Config_Application_Website.MapMediaArticleList(ds.Tables[0]);
            }

            // Total record count
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                model.TotalCount = Convert.ToInt32(
                    ds.Tables[1].Rows[0]["TotalCount"]
                );
            }

            return model;
        }

        /// <summary>
        /// /sustainability/governance — banner + child subsections as PDF lists
        /// (default: Key Governance Policies / key-governance-policies).
        /// Uses the same child/PDF SPs as investor corporate-governance.
        /// </summary>
        public SustainabilityModel GetGovernance_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new SustainabilityModel();
            var pageName = string.IsNullOrWhiteSpace(pagename)
                ? "sustainability/governance"
                : pagename.Trim().TrimStart('/');

            using var cgBal = new CorporateGovernance_BAL(_configuration);
            var cg = cgBal.GetCorporateGovernance_BAL(pageName, languageId, geographyId);

            model.Content = cg.Content;
            model.Sections = cg.Sections ?? new List<GovernanceSectionModel>();
            return model;
        }

    }
}
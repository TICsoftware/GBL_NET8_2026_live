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
    public class Sustainability_BAL : BasePageBAL
    {
        public Sustainability_BAL(IConfiguration configuration) : base(configuration)
        {
        }


        public SustainabilityModel GetSustainability_BAL(string pagename, int languageId, int geographyId)
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

                model.Sustainability_intro_List = MapComponents(groupedData, 1);
                model.Creating_Towards_beautiful_world_List = MapComponents(groupedData, 2);
                model.Turning_commitment_into_action_List = MapComponents(groupedData, 3);
                model.Awards_Certifications_Reports_List = MapComponents(groupedData, 4);
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
        /// No year dropdown. PDFs come from Get_Sustainability_Governance_Articles_List
        /// (not Get_Investor_Pdf_Articles_List used by investor pages).
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

            foreach (var section in model.Sections)
            {
                section.ShowYearFilter = false;
                section.SelectedYear = "";
                section.Years = new List<DropdownModel>();
                section.Articles = LoadGovernanceArticlesByDateDesc(section.ContId);
            }

            return model;
        }

        private List<ArticleModel> LoadGovernanceArticlesByDateDesc(int contentId)
        {
            if (contentId <= 0)
                return new List<ArticleModel>();

            try
            {
                var ds = Get_Sustainability_Governance_Articles_DAL(
                    contentId,
                    1,
                    CorporateGovernance_BAL.PdfPageSize);

                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    return Config_Application_Website.MapArticleList(ds.Tables[0]);
                }
            }
            catch
            {
                // SP may not be deployed yet for this environment.
            }

            return new List<ArticleModel>();
        }

    }
}
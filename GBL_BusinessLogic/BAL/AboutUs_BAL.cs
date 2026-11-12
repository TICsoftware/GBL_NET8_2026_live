using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class AboutUs_BAL : BasePageBAL
    {
        public AboutUs_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public AboutModel GetLegacyPage_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            var pageName = string.IsNullOrWhiteSpace(pagename) ? "our-legacy" : pagename.Trim();
            var ds = GetLegacyPageData_DAL(pageName, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                // component_sequence 1 = Founding Family, 2 = Milestones
                model.Founding_Family_List = MapComponents(groupedData, 1);
                model.Milestones_List = MapComponents(groupedData, 2);
            }

            return model;
        }

        public AboutModel GetLeadershipPage_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            var pageName = string.IsNullOrWhiteSpace(pagename) ? "leadership" : pagename.Trim();
            var ds = GetLeadershipPageData_DAL(pageName, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                // component_sequence 1 = Board of Directors (featured), 2 = BOD leadership list
                model.Board_Of_Directors_List = MapComponents(groupedData, 1);
                model.Leadership_List = MapComponents(groupedData, 2);
            }

            return model;
        }

        public AboutModel GetAboutUs_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new AboutModel();
            var pageName = string.IsNullOrWhiteSpace(pagename) ? "about-us" : pagename.Trim();
            var ds = GetContentComponentData_DAL(pageName, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                model.Four_Decades_Of_Building_List = MapComponents(groupedData, 1);
                model.Infographic_List = MapComponents(groupedData, 2);
                model.What_We_Stand_For_List = MapComponents(groupedData, 3);
                model.A_Word_From_Our_Leadership_List = MapComponents(groupedData, 4);
                model.Values_That_Define_How_We_Work_List = MapComponents(groupedData, 5);
                model.Built_On_Trust_Proven_At_Scale_List = MapComponents(groupedData, 6);
                model.Legacy_Built_One_Year_At_A_Time_List = MapComponents(groupedData, 7);
                model.Ready_To_Build_With_Priya_Cement_List = MapComponents(groupedData, 8);
            }

            return model;
        }
    }
}

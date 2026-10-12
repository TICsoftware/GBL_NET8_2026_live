using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using GBL_BusinessLogic.Entity;

namespace GBL_BusinessLogic.BAL
{
    public class ResourceCircularity_BAL : BasePageBAL
    {
        public ResourceCircularity_BAL(IConfiguration configuration) : base(configuration)
        {
        }

        public ResourceCircularityModel GetResourceCircularity_BAL(string pagename, int languageId, int geographyId)
        {
            var model = new ResourceCircularityModel();
            var ds = GetContentComponentData_DAL(pagename, languageId, geographyId);

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                model.Content = MapContent(ds.Tables[0].Rows[0]);
            }

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                var groupedData = GetGroupedComponents(ds.Tables[1]);
                model.Components = groupedData;

                model.Infographic_List = MapComponents(groupedData, 1);
                model.Circular_Energy_Model_List = MapComponents(groupedData, 2);
                model.Energy_Chart_World_List = MapComponents(groupedData, 3);
                model.Energy_Chart_India_List = MapComponents(groupedData, 4);
                model.Energy_Chart_Gbl_List = MapComponents(groupedData, 5);
                model.Fossil_Fuels_List = MapComponents(groupedData, 6);
                model.Low_Emission_Operations_List = MapComponents(groupedData, 7);
                model.Conserving_Water_List = MapComponents(groupedData, 8);
            }

            return model;
        }
    }
}

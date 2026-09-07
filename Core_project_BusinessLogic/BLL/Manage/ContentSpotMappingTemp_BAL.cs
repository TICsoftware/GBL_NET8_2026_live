using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core_project_BusinessLogic.BAL
{
public class ContentSpotMappingTemp_BAL
{
    private readonly ContentSpotMappingTemp_DAL _dal;
        private readonly ContextReference_BAL _bal_CR;
          private readonly Main_Spot_Template_Details_BAL _mainsoptbal;


    public ContentSpotMappingTemp_BAL(IConfiguration config)
    {
        _dal = new ContentSpotMappingTemp_DAL(config);
    }

    public void Add(int contId, int spotId, int spotType, int userId)
        => _dal.Insert(contId, spotId, spotType, userId);

    public DataTable GetAll(int contId,string conttempid)
        => _dal.GetAll(contId,conttempid);

    public void MoveUp(int currentId, int swapId)
        => _dal.Swap(currentId, swapId);

        public void UpdateSequence(int contSpotId, int newSequence)
{
    _dal.UpdateSequence(contSpotId, newSequence);
}
public void AddSpot(int contId, string tempid,int spotId, int spotType)
{
    _dal.AddSpot(contId, tempid, spotId, spotType);
}
public void DeleteMapping(int contSpotId, int contId)
{
    _dal.DeleteMapping(contSpotId, contId);
}


public ContentSpotMappingTemp_BAL(
        ContextReference_BAL balCR,
        Main_Spot_Template_Details_BAL mainSpotBal)
    {
        _bal_CR = balCR;
        _mainsoptbal = mainSpotBal;
    }


    public string BuildPreviewHtml(List<SpotPreviewVM> spots)
    {
        var sb = new StringBuilder();

        foreach (var s in spots)
        {
            if (s.SpotType == 1)
            {
                // Dynamic Context
                string html = _bal_CR.BuildFinalLayout(s.SpotId);
                sb.Append(html);
            }
            else
            {
                DataSet ds = _mainsoptbal.GetSpotLayoutById(s.SpotId);
                string finalLayout = BuildFinalLayout(ds);
                sb.Append(finalLayout);
            }
        }

        return sb.ToString();
    }

    public void Delete_all(string contidEnc, string tempid)
    {
        int? contId = null;

        if (!string.IsNullOrEmpty(contidEnc) && contidEnc != "0")
        {
            contId = Convert.ToInt32(
                CryptoEngine.Decrypt(contidEnc)
            );
        }

        _dal.Deleteall(contId, tempid);
    }

        public string BuildFinalLayout(DataSet ds)
        {
            // ============================
            // TABLE REFERENCES
            // ============================
            DataTable mainLayoutTable = ds.Tables[0];
            DataTable mainContentTable = ds.Tables[1];
            DataTable spotLayoutTable = ds.Tables[2];
            DataTable spotContentTable = ds.Tables[3];

            if (mainLayoutTable.Rows.Count == 0 || spotLayoutTable.Rows.Count == 0)
                return null;

            // ============================
            // MAIN LAYOUT
            // ============================
            string finalLayout = mainLayoutTable.Rows[0]["Design_Layout"]?.ToString();

            if (string.IsNullOrWhiteSpace(finalLayout))
                return null;

            if (mainContentTable.Rows.Count > 0)
            {
                DataRow main = mainContentTable.Rows[0];

                finalLayout = finalLayout
                    .Replace("#maintemplatetitle#", main["Title"]?.ToString() ?? string.Empty)
                    .Replace("#maintemplateintro#", main["Intro"]?.ToString() ?? string.Empty);
            }

            // ============================
            // RHS SPOT LAYOUT
            // ============================
            string spotTemplate = spotLayoutTable.Rows[0]["Design_Layout"]?.ToString();

            if (string.IsNullOrWhiteSpace(spotTemplate))
                return finalLayout;

            var rhsHtml = new StringBuilder();
            int index = 1;

            foreach (DataRow row in spotContentTable.Rows)
            {
                string spotHtml = spotTemplate
                    .Replace("#title#", row["Title"]?.ToString() ?? string.Empty)
                    .Replace("#intro#", row["Description"]?.ToString() ?? string.Empty)
                    .Replace("#thumbnail_image#", row["Thumbnail_Img"]?.ToString() ?? string.Empty)
                    .Replace("#thumbnail_image_alt#", row["Thumbnail_Alt_Text"]?.ToString() ?? string.Empty);

                finalLayout = finalLayout.Replace($"#rhsspot{index}#", spotHtml);
                index++;
            }

            return finalLayout;
        }
}

}
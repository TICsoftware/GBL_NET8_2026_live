using System.Data;
using System.Xml;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic;

public class TagMaster_BAL : TagMaster_DAL
{
    private readonly TagMaster_DAL _dal;
    public TagMaster_BAL(IConfiguration configuration)
  : base(configuration)
    {
        _dal = new TagMaster_DAL(configuration);

    }

    public List<TagMaster> GetAllTags_BAL(int status, int language_id, out List<Options_List> languages)
    {
        languages = [];
        List<TagMaster> list = new List<TagMaster>();
        DataSet ds = _dal.GetAll(status, language_id);
        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                languages.Add(new Options_List
                {
                    id = Convert.ToInt32(row["ID"]),
                    title = row["Language_Name"] as string
                });
            }
        }
        if (ds.Tables[1].Rows.Count > 0)
        {
            foreach (DataRow row in ds.Tables[1].Rows)
            {
                list.Add(Map(row));
            }
        }

        return list;
    }

    public TagMaster GetTagById_BAL(int id)
    {
        TagMaster obj = new();
        DataTable dt = _dal.GetById(id);

        if (dt.Rows.Count > 0)
        {
            obj = Map(dt.Rows[0]);
        }

        return obj;
    }

    public int AddTag(TagMaster obj, int userid, out int result)
    {
        result = 0;
        DataTable dt = _dal.Add(obj, userid);

        if (dt.Rows.Count > 0)//dt.Columns.Contains("NewID")
        {
            result = Convert.ToInt32(dt.Rows[0]["result"]);
            return dt.Rows[0]["ID"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["ID"]);
        }
        else
        {
            return 0;
        }

    }

    public void UpdateTag(TagMaster obj, int userid)
    {
        _dal.Update(obj, userid);
    }

    private TagMaster Map(DataRow row)
    {

        return new TagMaster
        {
            ID = Convert.ToInt32(row["ID"]),
            Tag_name = row["Tag_Name"] as string,
            Language_Master_ID = row["Language_MasterID"] == DBNull.Value ? null : Convert.ToInt32(row["Language_MasterID"]),
            Languauge_Name = row.Table.Columns.Contains("Language_Name") ? row["Language_Name"]?.ToString() : null,
            Status = Convert.ToInt32(row["Status"]),
            Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : (DateTime?)null,
            Updated_Date = row["Updated_Date"] != DBNull.Value ? Convert.ToDateTime(row["Updated_Date"]) : (DateTime?)null,
        };
    }

    public void UpdateTagStatus(int id, int status, int userid)
    {
        _dal.UpdateTagStatus_DAL(id,status, userid);
    }
}
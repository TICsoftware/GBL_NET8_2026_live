using System.Collections.Generic;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Core_project_BusinessLogic.BAL
{
    public class ContextReference_BAL
    {
        private readonly ContextReference_DAL dal;

        public ContextReference_BAL(IConfiguration config)
        {
            dal = new ContextReference_DAL(config);
        }

        public List<ContextReference> GetAll() => dal.GetAll();


        public List<ContextReference> GetAll_bytemplate(int template_id) => dal.GetAll_bytemplateid(template_id);
        public ContextReference GetById(int id) => dal.GetById(id);

        public int Add(ContextReference model)
        {
           
            return dal.Add(model);
        }
          public bool HasData(int referenceId)
      {
         return dal.CheckIfDataExists(referenceId);
       }
        public void Update(ContextReference model)
        {
            
            dal.Update(model);
        }

        public string BuildFinalLayout(int referenceId)
{
    DataTable dt = dal.GetLayoutData(referenceId);

    if (dt.Rows.Count == 0)
        return string.Empty;

    string layout = dt.Rows[0]["design_layout"].ToString();

    foreach (DataRow row in dt.Rows)
    {
        string key = row["name_key"].ToString();

        // prefer File_path for image fields
        string value = row["thumbnail_image"] != DBNull.Value && !string.IsNullOrEmpty(row["thumbnail_image"].ToString())
            ? row["thumbnail_image"].ToString()
            : row["content"]?.ToString();

        layout = layout.Replace($"#{key}#", value ?? "");
    }

    return layout;
}

        public string GetHtmlLayout(int referenceId)
{
    return dal.GetHtmlLayoutByReferenceId(referenceId);
}


        public void Deactivate(int id, int userId = 1) => dal.Deactivate(id, userId);
    }
}

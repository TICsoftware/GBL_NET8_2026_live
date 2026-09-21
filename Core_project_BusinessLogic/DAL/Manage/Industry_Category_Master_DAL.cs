using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;

namespace Core_project_BusinessLogic.DAL
{
    public class Industry_Category_Master_DAL : DBHelper
    {
        public Industry_Category_Master_DAL(IConfiguration configuration) : base(configuration) { }

        public (List<Industry_Category_Master_Entity> Data, int Total) GetPaged(string type, string search, int page, int pageSize)
        {
            SqlParameter[] p =
            {
                new("@MasterType", type),
                new("@Search", string.IsNullOrEmpty(search) ? DBNull.Value : search),
                new("@Page", page),
                new("@PageSize", pageSize)
            };

            DataSet ds = GetDataSet("Industry_Category_master_GetPaged", p);
            List<Industry_Category_Master_Entity> list = new();

            foreach (DataRow r in ds.Tables[0].Rows)
            {
                list.Add(MapRow(r, type));
            }

            int total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            return (list, total);
        }

        public Industry_Category_Master_Entity GetById(int id, string type)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type)
            };

            DataTable dt = GetDataSet("Industry_Category_master_GetById", p).Tables[0];
            if (dt.Rows.Count == 0)
                return null!;

            return MapRow(dt.Rows[0], type);
        }

        public int Insert(Industry_Category_Master_Entity m)
        {
            SqlParameter[] p =
            {
                new("@Name", (object?)m.Name ?? DBNull.Value),
                new("@PageName", (object?)m.PageName ?? DBNull.Value),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Status", m.Status ?? 1),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@Banner_Image_media_id", m.Banner_Image_media_id.HasValue ? m.Banner_Image_media_id.Value : DBNull.Value),
                new("@Landing_Thumbnail_Image_media_id", m.Landing_Thumbnail_Image_media_id.HasValue ? m.Landing_Thumbnail_Image_media_id.Value : DBNull.Value),
                new("@Intro", (object?)m.Intro ?? DBNull.Value),
                new("@Content", (object?)m.Content ?? DBNull.Value),
                new("@Create_UserId", m.Create_UserId.HasValue ? m.Create_UserId.Value : DBNull.Value),
                new("@MasterType", m.MasterType)
            };

            return SqlInsertReturnIdentity_withSP("Industry_Category_master_Insert", "@NewID", p);
        }

        public void Update(Industry_Category_Master_Entity m)
        {
            SqlParameter[] p =
            {
                new("@ID", m.ID),
                new("@Name", (object?)m.Name ?? DBNull.Value),
                new("@PageName", (object?)m.PageName ?? DBNull.Value),
                new("@Sequence", m.Sequence.HasValue ? m.Sequence.Value : DBNull.Value),
                new("@Language_Master_Id", m.Language_Master_Id.HasValue ? m.Language_Master_Id.Value : DBNull.Value),
                new("@Banner_Image_media_id", m.Banner_Image_media_id.HasValue ? m.Banner_Image_media_id.Value : DBNull.Value),
                new("@Landing_Thumbnail_Image_media_id", m.Landing_Thumbnail_Image_media_id.HasValue ? m.Landing_Thumbnail_Image_media_id.Value : DBNull.Value),
                new("@Intro", (object?)m.Intro ?? DBNull.Value),
                new("@Content", (object?)m.Content ?? DBNull.Value),
                new("@Update_UserId", m.Update_UserId.HasValue ? m.Update_UserId.Value : DBNull.Value),
                new("@MasterType", m.MasterType)
            };

            SQLInsert_Update_Delete_Data("Industry_Category_master_Update", p);
        }

        public void Activate(int id, string type, int? updateUserId)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type),
                new("@Update_UserId", updateUserId.HasValue ? updateUserId.Value : DBNull.Value)
            };
            SQLInsert_Update_Delete_Data("Industry_Category_master_Activate", p);
        }

        public void Deactivate(int id, string type, int? updateUserId)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type),
                new("@Update_UserId", updateUserId.HasValue ? updateUserId.Value : DBNull.Value)
            };
            SQLInsert_Update_Delete_Data("Industry_Category_master_Deactivate", p);
        }

        public void ChangeStatus(int id, string type, int status, int? updateUserId)
        {
            if (status == 1)
                Activate(id, type, updateUserId);
            else
                Deactivate(id, type, updateUserId);
        }

        public void Delete(int id, string type)
        {
            SqlParameter[] p =
            {
                new("@ID", id),
                new("@MasterType", type)
            };
            SQLInsert_Update_Delete_Data("Industry_Category_master_Delete", p);
        }

        public void UpdateSequence(List<Industry_Category_Master_Entity> list, string type)
        {
            foreach (var item in list)
            {
                SqlParameter[] p =
                {
                    new("@ID", item.ID),
                    new("@Sequence", item.Sequence),
                    new("@MasterType", type)
                };
                SQLInsert_Update_Delete_Data("Industry_Category_master_UpdateSequence", p);
            }
        }

        private static Industry_Category_Master_Entity MapRow(DataRow r, string type)
        {
            return new Industry_Category_Master_Entity
            {
                ID = Convert.ToInt32(r["ID"]),
                Name = ColStr(r, "Name"),
                PageName = ColStr(r, "PageName"),
                Sequence = ColInt(r, "Sequence"),
                Status = ColInt(r, "Status") ?? 1,
                Language_Master_Id = ColInt(r, "Language_Master_Id"),
                LanguageName = ColStr(r, "LanguageName"),
                Banner_Image_media_id = ColInt(r, "Banner_Image_media_id"),
                Landing_Thumbnail_Image_media_id = ColInt(r, "Landing_Thumbnail_Image_media_id"),
                Banner_Image_Url = ColStr(r, "Banner_Image_Url"),
                Landing_Thumbnail_Image_Url = ColStr(r, "Landing_Thumbnail_Image_Url"),
                Intro = ColStr(r, "Intro"),
                Content = ColStr(r, "Content"),
                MasterType = type
            };
        }

        private static string? ColStr(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? r[col].ToString() : null;

        private static int? ColInt(DataRow r, string col) =>
            r.Table.Columns.Contains(col) && r[col] != DBNull.Value ? Convert.ToInt32(r[col]) : null;
    }
}

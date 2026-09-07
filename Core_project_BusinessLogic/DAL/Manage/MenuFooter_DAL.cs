using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;
using Core_project_BusinessLogic.Entity.Manage;

namespace Core_project_BusinessLogic.DAL
{
    public class MenuFooter_DAL : DBHelper
    {
        public MenuFooter_DAL(IConfiguration config) : base(config)
        {
        }

        // ============================================
        // GET ALL MENUS
        // ============================================


        public (List<MenuFooter> Data, int Total) GetPagedFooters_dal(
    string search,
    int? status,
    int page,
    int pageSize)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@Search",
            string.IsNullOrWhiteSpace(search) ? DBNull.Value : search),

        new SqlParameter("@Status",
            status.HasValue ? status.Value : DBNull.Value),

        new SqlParameter("@Page", page),
        new SqlParameter("@PageSize", pageSize)
    };

            DataSet ds = GetDataSet("MenuFooter_GetList", p);

            List<MenuFooter> list = new();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(MapFooter(r));
                }
            }

            int total = 0;

            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            }

            return (list, total);
        }


        // ============================================
        // GET ALL FOOTERS
        // ============================================
        public List<MenuFooter> GetFooters_dal()
        {
            SqlParameter[] p =
            {
        new SqlParameter("@Search", DBNull.Value),
        new SqlParameter("@Status", DBNull.Value),
        new SqlParameter("@Page", 1),
        new SqlParameter("@PageSize", 1000)
    };

            DataSet ds = GetDataSet("MenuFooter_GetList", p);

            List<MenuFooter> list = new();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(MapFooter(r));
                }
            }

            return list;
        }


        // ============================================
        // GET FOOTER BY ID
        // ============================================
        public MenuFooter GetFooterById_dal(int id)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@FooterId", id)
    };

            DataTable dt =
                GetDataSet("MenuFooter_GetById", p).Tables[0];

            if (dt.Rows.Count == 0)
                return null;

            return MapFooter(dt.Rows[0]);
        }


        public void Deactivate_dal(int id, int userId)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@DeActivated_UserID", userId)
            };
            SQLInsert_Update_Delete_Data("MenuFooter_Deactivate", p);
        }


        // ============================================
        // GET PARENT FOOTERS
        // ============================================
        public List<MenuFooter> GetParentFooters_dal()
        {
            DataTable dt =
                GetDataSet("GetParentMenuFooter").Tables[0];

            List<MenuFooter> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new MenuFooter
                {
                    FooterId = r["FooterId"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(r["FooterId"]),

                    Title = r["Title"] == DBNull.Value
                        ? null
                        : r["Title"].ToString()
                });
            }

            return list;
        }


        // ============================================
        // INSERT FOOTER
        // ============================================
        public void SaveFooter_dal(MenuFooter model)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@ParentFooterId",
            (object?)model.ParentFooterId ?? DBNull.Value),

        new SqlParameter("@FooterType",
            model.FooterType ?? "Menu"),

        new SqlParameter("@Title",
            (object?)model.Title ?? DBNull.Value),

        new SqlParameter("@Description",
            (object?)model.Description ?? DBNull.Value),

        new SqlParameter("@Url",
            (object?)model.Url ?? DBNull.Value),

        new SqlParameter("@Target",
            model.Target ?? "_self"),

        new SqlParameter("@FooterImageId",
            model.FooterImageId > 0
                ? model.FooterImageId
                : DBNull.Value),

        new SqlParameter("@FooterThumbImageId",
            model.FooterThumbImageId > 0
                ? model.FooterThumbImageId
                : DBNull.Value),

        new SqlParameter("@IconClass",
            (object?)model.IconClass ?? DBNull.Value),

        new SqlParameter("@ColumnNo",
            (object?)model.ColumnNo ?? DBNull.Value),

        new SqlParameter("@Sequence", model.Sequence),

        new SqlParameter("@Status", model.Status),

        new SqlParameter("@CreatedBy", model.CreatedBy)
    };

            ExecuteNonQuery("MenuFooter_Insert", p);
        }


        // ============================================
        // UPDATE FOOTER
        // ============================================
        public void UpdateFooter_dal(MenuFooter model)
        {
            SqlParameter[] p =
            {
        new SqlParameter("@FooterId", model.FooterId),

        new SqlParameter("@ParentFooterId",
            (object?)model.ParentFooterId ?? DBNull.Value),

        new SqlParameter("@FooterType",
            model.FooterType ?? "Menu"),

        new SqlParameter("@Title",
            (object?)model.Title ?? DBNull.Value),

        new SqlParameter("@Description",
            (object?)model.Description ?? DBNull.Value),

        new SqlParameter("@Url",
            (object?)model.Url ?? DBNull.Value),

        new SqlParameter("@Target",
            model.Target ?? "_self"),

        new SqlParameter("@FooterImageId",
            model.FooterImageId > 0
                ? model.FooterImageId
                : DBNull.Value),

        new SqlParameter("@FooterThumbImageId",
            model.FooterThumbImageId > 0
                ? model.FooterThumbImageId
                : DBNull.Value),

        new SqlParameter("@IconClass",
            (object?)model.IconClass ?? DBNull.Value),

        new SqlParameter("@ColumnNo",
            (object?)model.ColumnNo ?? DBNull.Value),

        new SqlParameter("@Sequence", model.Sequence),

        new SqlParameter("@Status", model.Status),

        new SqlParameter("@ModifiedBy",
            model.ModifiedBy.HasValue
                ? model.ModifiedBy.Value
                : DBNull.Value)
    };

            ExecuteNonQuery("MenuFooter_Update", p);
        }


        // ============================================
        // MAP DATA ROW TO FOOTER MODEL
        // ============================================
        private MenuFooter MapFooter(DataRow r)
        {
            return new MenuFooter
            {
                // Basic Information
                FooterId = r["FooterId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["FooterId"]),

                ParentFooterId = r["ParentFooterId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(r["ParentFooterId"]),

                    ParentFooterTitle = r["ParentFooterTitle"] == DBNull.Value
                    ? "Main Footer"
                    : r["ParentFooterTitle"].ToString(),

                FooterType = r["FooterType"] == DBNull.Value
                    ? "Menu"
                    : r["FooterType"].ToString(),

                Title = r["Title"] == DBNull.Value
                    ? null
                    : r["Title"].ToString(),

                Description = r["Description"] == DBNull.Value
                    ? null
                    : r["Description"].ToString(),

                Url = r["Url"] == DBNull.Value
                    ? null
                    : r["Url"].ToString(),

                Target = r["Target"] == DBNull.Value
                    ? "_self"
                    : r["Target"].ToString(),

                // Images
                FooterImageId = r["FooterImageId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["FooterImageId"]),

                FooterImage = r.Table.Columns.Contains("FooterImage")
                    && r["FooterImage"] != DBNull.Value
                        ? r["FooterImage"].ToString()
                        : null,

                FooterThumbImageId = r["FooterThumbImageId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["FooterThumbImageId"]),

                FooterThumbImage = r.Table.Columns.Contains("FooterThumbImage")
                    && r["FooterThumbImage"] != DBNull.Value
                        ? r["FooterThumbImage"].ToString()
                        : null,

                // Display Settings
                IconClass = r["IconClass"] == DBNull.Value
                    ? null
                    : r["IconClass"].ToString(),

                ColumnNo = r["ColumnNo"] == DBNull.Value
                    ? null
                    : r["ColumnNo"].ToString(),

                Sequence = r["Sequence"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["Sequence"]),

                Status = r["Status"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["Status"]),

                // Audit Information
                CreatedBy = r.Table.Columns.Contains("CreatedBy")
                            && r["CreatedBy"] != DBNull.Value
                    ? Convert.ToInt32(r["CreatedBy"])
                    : 0,

                CreatedDate = r["CreatedDate"] == DBNull.Value
                    ? DateTime.Now
                    : Convert.ToDateTime(r["CreatedDate"]),

                ModifiedBy = r.Table.Columns.Contains("ModifiedBy")
                             && r["ModifiedBy"] != DBNull.Value
                    ? Convert.ToInt32(r["ModifiedBy"])
                    : null,

                ModifiedDate = r["ModifiedDate"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(r["ModifiedDate"])
            };
        }
    }
}
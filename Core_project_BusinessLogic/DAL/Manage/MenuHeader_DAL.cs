using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Core_project_BusinessLogic.Entity;
using Core_project_BusinessLogic.Entity.Manage;

namespace Core_project_BusinessLogic.DAL
{
    public class MenuHeader_DAL : DBHelper
    {
        public MenuHeader_DAL(IConfiguration config) : base(config)
        {
        }

        // ============================================
        // GET ALL MENUS
        // ============================================


        public (List<MenuHeader> Data, int Total) GetPagedMenus_dal(string search, int? isActive, int page, int pageSize)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@Search", string.IsNullOrWhiteSpace(search) ? DBNull.Value : search),
                new SqlParameter("@Status", isActive.HasValue ? isActive.Value : DBNull.Value),
                new SqlParameter("@Page", page), new SqlParameter("@PageSize", pageSize)
            };

            DataSet ds = GetDataSet("MenuHeader_GetList", p);

            List<MenuHeader> list = new();

            if (ds.Tables.Count > 0)
            {
                foreach (DataRow r in ds.Tables[0].Rows)
                {
                    list.Add(MapMenu(r));
                }
            }

            int total = 0;

            if (ds.Tables.Count > 1 &&
                ds.Tables[1].Rows.Count > 0)
            {
                total = Convert.ToInt32(ds.Tables[1].Rows[0]["TotalCount"]);
            }

            return (list, total);
        }

        public List<MenuHeader> GetMenus_dal()
        {
            SqlParameter[] p = { };

            DataTable dt =
                GetDataSet("MenuHeader_GetList", p).Tables[0];

            List<MenuHeader> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(MapMenu(r));
            }

            return list;
        }


        // ============================================
        // GET MENU BY ID
        // ============================================
        public MenuHeader GetMenuById_dal(int id)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@MenuId", id)
            };

            DataTable dt =
                GetDataSet("MenuHeader_GetById", p).Tables[0];

            if (dt.Rows.Count == 0)
                return null;

            return MapMenu(dt.Rows[0]);
        }


        // ============================================
        // GET PARENT MENUS
        // ============================================

        public List<MenuHeader> GetParentMenus_dal(int currentMenuId = 0)
        {
            DataTable dt = GetDataSet("GetParentMenuheader").Tables[0];

            List<MenuHeader> list = new();

            foreach (DataRow r in dt.Rows)
            {
                int menuId = r["MenuId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["MenuId"]);

                // Exclude current menu from becoming its own parent
                if (currentMenuId > 0 && menuId == currentMenuId)
                    continue;

                list.Add(new MenuHeader
                {
                    MenuId = menuId,

                    ParentMenuId = r["ParentMenuId"] == DBNull.Value
                        ? null
                        : Convert.ToInt32(r["ParentMenuId"]),

                    MenuName = r["MenuName"] == DBNull.Value
                        ? null
                        : r["MenuName"].ToString(),

                    MenuLevel = r["MenuLevel"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(r["MenuLevel"]),

                    DisplayMenuName = r["DisplayMenuName"] == DBNull.Value
                        ? null
                        : r["DisplayMenuName"].ToString()
                });
            }

            return list;
        }

        // public List<MenuHeader> GetParentMenus_dal(int currentMenuId = 0)
        // {

        //     DataTable dt = GetDataSet("GetParentMenuheader").Tables[0];

        //     List<MenuHeader> list = new();

        //     foreach (DataRow r in dt.Rows)
        //     {
        //         list.Add(new MenuHeader
        //         {
        //             MenuId = Convert.ToInt32(r["MenuId"]),
        //             MenuName = r["MenuName"]?.ToString()
        //         });
        //     }

        //     return list;
        // }


        // ============================================
        // SAVE MENU
        // INSERT / UPDATE
        // ============================================
        public void SaveMenu_dal(MenuHeader model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ParentMenuId", (object?)model.ParentMenuId ?? DBNull.Value),
                new SqlParameter("@MenuCategory", model.MenuCategory),
                new SqlParameter("@MenuName", (object?)model.MenuName ?? DBNull.Value),
                new SqlParameter("@Url", (object?)model.Url ?? DBNull.Value),
                new SqlParameter("@Target", model.Target ?? "_self"),
                new SqlParameter("@MenuType", model.MenuType ?? "Normal"),
                new SqlParameter("@Sequence", model.Sequence),
                new SqlParameter("@FeatureImageId", (object?)model.FeatureImageId ?? DBNull.Value),
                new SqlParameter("@ThumbImageId", (object?)model.ThumbImageId ?? DBNull.Value),
                new SqlParameter("@FeatureTitle", (object?)model.FeatureTitle ?? DBNull.Value),
                new SqlParameter("@FeatureDescription", (object?)model.FeatureDescription ?? DBNull.Value),
                new SqlParameter("@FeatureButtonText", (object?)model.FeatureButtonText ?? DBNull.Value),
                new SqlParameter("@FeatureButtonUrl", (object?)model.FeatureButtonUrl ?? DBNull.Value),
                new SqlParameter("@FeatureButtonTarget", model.FeatureButtonTarget ?? "_self"),
                new SqlParameter("@CssClass", (object?)model.CssClass ?? DBNull.Value),
                new SqlParameter("@Status", model.Status)
            };

            ExecuteNonQuery("MenuHeader_Insert", p);
        }


        public void UpdateMenu_dal(MenuHeader model)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@MenuId", model.MenuId),
                new SqlParameter("@ParentMenuId", (object?)model.ParentMenuId ?? DBNull.Value),
                new SqlParameter("@MenuCategory", model.MenuCategory),
                new SqlParameter("@MenuName", (object?)model.MenuName ?? DBNull.Value),
                new SqlParameter("@Url", (object?)model.Url ?? DBNull.Value),
                new SqlParameter("@Target", model.Target ?? "_self"),
                new SqlParameter("@MenuType", model.MenuType ?? "Normal"),
                new SqlParameter("@Sequence", model.Sequence),

                new SqlParameter("@FeatureImageId",
                    model.FeatureImageId > 0 ? model.FeatureImageId : DBNull.Value),

                new SqlParameter("@ThumbImageId",
                    model.ThumbImageId > 0 ? model.ThumbImageId : DBNull.Value),

                new SqlParameter("@FeatureTitle", (object?)model.FeatureTitle ?? DBNull.Value),
                new SqlParameter("@FeatureDescription", (object?)model.FeatureDescription ?? DBNull.Value),
                new SqlParameter("@FeatureButtonText", (object?)model.FeatureButtonText ?? DBNull.Value),
                new SqlParameter("@FeatureButtonUrl", (object?)model.FeatureButtonUrl ?? DBNull.Value),
                new SqlParameter("@FeatureButtonTarget", model.FeatureButtonTarget ?? "_self"),
                new SqlParameter("@CssClass", (object?)model.CssClass ?? DBNull.Value),
                new SqlParameter("@Status", model.Status)
            };

            ExecuteNonQuery("MenuHeader_Update", p);
        }


        public void Deactivate_dal(int id, int userId)
        {
            SqlParameter[] p =
            {
                new SqlParameter("@ID", id),
                new SqlParameter("@DeActivated_UserID", userId)
            };
            SQLInsert_Update_Delete_Data("MenuHeader_Deactivate", p);
        }

        // ============================================
        // MAP DATA ROW TO MODEL
        // ============================================
        private MenuHeader MapMenu(DataRow r)
        {
            return new MenuHeader
            {
                // Basic Information
                MenuId = r["MenuId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["MenuId"]),

                ParentMenuId = r["ParentMenuId"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(r["ParentMenuId"]),

                ParentMenuName = r["ParentMenuName"] == DBNull.Value
                    ? null
                    : r["ParentMenuName"].ToString(),


                MenuCategory = r["MenuCategory"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["MenuCategory"]),

                MenuName = r["MenuName"] == DBNull.Value
                    ? null
                    : r["MenuName"].ToString(),

                Url = r["Url"] == DBNull.Value
                    ? null
                    : r["Url"].ToString(),

                Target = r["Target"] == DBNull.Value
                    ? "_self"
                    : r["Target"].ToString(),

                MenuType = r["MenuType"] == DBNull.Value
                    ? "Normal"
                    : r["MenuType"].ToString(),

                // Display Settings
                Sequence = r["Sequence"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["Sequence"]),

                // Images
                FeatureImageId = r["FeatureImageId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["FeatureImageId"]),

                FeatureImage = r["FeatureImage"] == DBNull.Value
                    ? null
                    : r["FeatureImage"].ToString(),

                ThumbImageId = r["ThumbImageId"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["ThumbImageId"]),

                ThumbImage = r["ThumbImage"] == DBNull.Value
                    ? null
                    : r["ThumbImage"].ToString(),

                // Feature Content
                FeatureTitle = r["FeatureTitle"] == DBNull.Value
                    ? null
                    : r["FeatureTitle"].ToString(),

                FeatureDescription = r["FeatureDescription"] == DBNull.Value
                    ? null
                    : r["FeatureDescription"].ToString(),

                FeatureButtonText = r["FeatureButtonText"] == DBNull.Value
                    ? null
                    : r["FeatureButtonText"].ToString(),

                FeatureButtonUrl = r["FeatureButtonUrl"] == DBNull.Value
                    ? null
                    : r["FeatureButtonUrl"].ToString(),

                FeatureButtonTarget = r["FeatureButtonTarget"] == DBNull.Value
                    ? "_self"
                    : r["FeatureButtonTarget"].ToString(),

                // Other Settings
                CssClass = r["CssClass"] == DBNull.Value
                    ? null
                    : r["CssClass"].ToString(),

                Status = r["Status"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(r["Status"]),

                // Audit Information
                CreatedDate = r["CreatedDate"] == DBNull.Value
                    ? DateTime.Now
                    : Convert.ToDateTime(r["CreatedDate"]),

                ModifiedDate = r["ModifiedDate"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(r["ModifiedDate"])
            };
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core_project_BusinessLogic.DAL;
using Core_project_BusinessLogic.Entity.Manage;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Core_project_BusinessLogic.BLL.Manage
{
    public class MediaManager_bal
    {
        private readonly MediaManager_dal _dal;
        public MediaManager_bal(IConfiguration configuration)
        {
            _dal = new MediaManager_dal(configuration);
        }

        public List<MediaItem> GetAll_Media_bal(string sort, string type, string search,int page, int pageSize)
        {
            List<MediaItem> list = new List<MediaItem>();
            DataTable dt = _dal.GetAll_Media_DAL(sort, type, search, page,  pageSize);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapMedia(row));
            }

            return list;
        }


        public int Add_Media_bal(MediaItem media)
        {
            DataTable dt = _dal.Add_Media_DAL(media);

            if (dt.Rows.Count > 0 && dt.Columns.Contains("result"))
            {
                return Convert.ToInt32(dt.Rows[0]["result"]);
            }

            return 0;
        }


        public int Update_Media_bal(MediaTemp media)
        {
            DataTable dt = _dal.update_media_DAL(media);

            if (dt.Rows.Count > 0 && dt.Columns.Contains("result"))
            {
                return Convert.ToInt32(dt.Rows[0]["result"]);
            }

            return 0;
        }


        public MediaTemp? Add_Media_temp_bal(MediaTemp media)
        {
            DataTable dt = _dal.Add_Media_temp_DAL(media);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                media.ID = Convert.ToInt32(row["ID"]);
                media.media_id = Convert.ToInt32(row["media_id"]);
                media.file_alt_text = row["file_alt_text"]?.ToString();
                media.FileUrl = row["file_path"]?.ToString();

                return media;
            }

            return null;
        }


        public MediaItem Get_MediaById_bal(int mediaId)
        {
            List<MediaItem> list = new List<MediaItem>();
            DataTable dt = _dal.Get_MediaById_DAL(mediaId);

            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapMedia(row));
            }

            return list[0];
        }

        public void DeleteMedia_bal(int media_id, int updatedBy, string file_path)
        {
            _dal.DeleteMedia_DAL(media_id, updatedBy, file_path);
        }


        //  public int Add_Media_temp_bal(MediaTemp media)
        // {
        //     DataTable dt = _dal.Add_Media_temp_DAL(media);

        //     if (dt.Rows.Count > 0 && dt.Columns.Contains("result"))
        //     {
        //         return Convert.ToInt32(dt.Rows[0]["result"]);
        //     }

        //     return 0;
        // }


        private MediaItem MapMedia(DataRow row)
        {
            return new MediaItem
            {
                ID = Convert.ToInt32(row["ID"]),
                media_file_name = row["media_file_name"]?.ToString(),
                file_path = row["file_path"]?.ToString(),
                file_type = row["file_type"]?.ToString(),
                file_size = row["file_size"]?.ToString(),
                status = row["status"] != DBNull.Value ? Convert.ToInt32(row["status"]) : (int?)null,
                Created_UserID = row["Created_UserID"] != DBNull.Value ? Convert.ToInt32(row["Created_UserID"]) : (int?)null,
                Created_Date = row["Created_Date"] != DBNull.Value ? Convert.ToDateTime(row["Created_Date"]) : (DateTime?)null,
            };
        }






    }
}
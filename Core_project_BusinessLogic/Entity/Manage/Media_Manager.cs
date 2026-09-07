using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic
{
    public class MediaItem
    {
        public int ID { get; set; }
        public string media_file_name { get; set; }
        public string file_column_name { get; set; }
        public string file_path { get; set; }
        public string file_type { get; set; }
        public string file_alt_text { get; set; }
        public string file_size { get; set; }

        public int? status { get; set; }
        public int? Language_Master_ID { get; set; }

        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }
    }


    public class MediaManagerViewModel
    {
        public string SortOrder { get; set; } = "DateDesc";
        public string FileType { get; set; } = "All";
        public string SearchTerm { get; set; } = "";

        public List<MediaItem> Items { get; set; } = new();

        // ✅ ADD THESE
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public bool HasMore { get; set; } = false;
    }


    public class MediaTemp
    {
        public int ID { get; set; }

        public int media_id { get; set; }
        public string FileName { get; set; }
        public string file_alt_text { get; set; }
        public string FileUrl { get; set; }

        public int Created_UserID { get; set; }
    }

}
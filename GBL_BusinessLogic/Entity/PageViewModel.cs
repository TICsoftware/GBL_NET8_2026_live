using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class PageViewModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new List<ComponentGroup>();
    }



    public class ComponentGroup
    {
        public string GroupId { get; set; }
        public int Sequence { get; set; }
        public List<ComponentField> Fields { get; set; } = new List<ComponentField>();
    }

    public class ComponentField
    {
        public string GroupId { get; set; }   // ADD THIS
        public string FieldName { get; set; }
        public string FieldKey { get; set; }
        public string FieldValue { get; set; }
        public string ImagePath { get; set; }
        public string popup_intro { get; set; }
        public string popup_content { get; set; }
        public int sequence { get; set; }
        public int IsBlock { get; set; }
    }

    public class ContentViewModel
    {
        public int ContId { get; set; }
        public string ContTitle { get; set; }
        public string Cont_intro { get; set; }
        public string Cont_hmpg_intro { get; set; }
        public string PageName { get; set; }
        public string MastheadImage { get; set; }
        public string MobileMastheadImage { get; set; }


        // Optional (from SP)
        public string ContIntro { get; set; }
        public string ContHomeIntro { get; set; }
        public string Content { get; set; }

        public string BreadcrumPath { get; set; }
        public int Template_Master_ID { get; set; }

        public string cont_metadesc { get; set; }
        public string cont_metatag { get; set; }
        public string cont_window_title { get; set; }
        public string page_schema { get; set; }

        public string Hmpg_thumbnail { get; set; }
        public string Hmpg_thumbnail_alt_text { get; set; }
        public string Masthead_image_Alt_text { get; set; }
        public string CanonicalUrl { get; set; }

        public string cont_meta_image { get; set; }

        public DateTime? cont_displaydate { get; set; }
        public string ByLine { get; set; }
        public string Publication { get; set; }
        public string ParentLink { get; set; }

    }





}
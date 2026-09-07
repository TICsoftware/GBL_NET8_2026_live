using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_BusinessLogic.Entity
{
    public class ComponentModel
    {
        public string GroupId { get; set; }

        public string Title { get; set; }
        public string Intro { get; set; }
        public string HmpgIntro { get; set; }
        public string DisplayTitle { get; set; }
         public string BlockDisplayTitle { get; set; }
        public string Content { get; set; }

        public string ComponentThumbnail { get; set; }
        public string ComponentThumbnailAltText { get; set; }

        public string Componentbackground { get; set; }
        public string ComponentbackgroundAltText { get; set; }

        public string ThumbnailImage { get; set; }
        public string ThumbnailAltText { get; set; }

        public string Url { get; set; }
        public string Url_Text { get; set; }
        public string component_Url2 { get; set; }
        public string component_Url_Text2 { get; set; }

        public string MediafilePath { get; set; }

        public string Component_FileUpload1 { get; set; }
        public string Component_FileUpload2 { get; set; }
        public string FileUploadTitle1 { get; set; }
        public string FileUploadTitle2 { get; set; }

        public string Component_Button_Title1 { get; set; }
        public string Component_Button_Title2 { get; set; }

        public int Sequence { get; set; }
        public int IsBlock { get; set; }

        public string Video_path { get; set; }
        public string Video_poster { get; set; }

         public string Component_Video_path { get; set; }


        public string Icon_Image { get; set; }
        public string Component_Icon_Image { get; set; }
        public string Component_Icon2_Image { get; set; }
        public string Component_Icon3_Image { get; set; }
        public string Component_Icon_alt_Image { get; set; }
        public string Popup_Content { get; set; }
        public string Popup_Display_Title { get; set; }

        public string Section_title { get; set; }

        public string Component_right_image { get; set; }
        public string Component_Right_image_alt { get; set; }
        public string Component_Designation { get; set; }
        public string Component_Designation2 { get; set; }
        public string Designation { get; set; }
        public string Thumbnail_color_image { get; set; }
        public string Component_LHS_thumbnail { get; set; }
        public string Component_LHS_thumbnail_image_alt { get; set; }
        public string Component_LHS_icon1 { get; set; }
        public string Component_LHS_icon2 { get; set; }

        public string Component_RHS_icon1 { get; set; }
        public string Component_RHS_icon2 { get; set; }
        public string bg_class { get; set; }
        public string year { get; set; }
    }


    public class ArticleModel
    {
        public int ContId { get; set; }
        public int ContParentId { get; set; }

        public string Title { get; set; }
        public string Intro { get; set; }
        public string HmpgIntro { get; set; }

        public string PageName { get; set; }

        public string ThumbnailImage { get; set; }
        public string ThumbnailAltText { get; set; }

        public string ExternalUrl { get; set; }
        public string MediafilePath { get; set; }

        public string Url { get; set; }
        public string UrlTarget { get; set; }

        public DateTime? DisplayDate { get; set; }
        public int Sequence { get; set; }
    }

}
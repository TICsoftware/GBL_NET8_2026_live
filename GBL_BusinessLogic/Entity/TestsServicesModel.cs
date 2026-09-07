using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Priya_Cement_BusinessLogic.Entity
{
    public class TestsServicesModel
    {
        public ContentViewModel? Content { get; set; }
        public List<ComponentGroup> Components { get; set; } = new();

        public List<ContentCommonModel> FindTest_List { get; set; } = new();
        public List<ContentCommonModel> Services_List { get; set; } = new();
        public List<ContentCommonModel> TestDirectory_List { get; set; } = new();
        public List<ContentCommonModel> ReferenceResources_List { get; set; } = new();
        public List<ContentCommonModel> DiagnosticRequest_List { get; set; } = new();


        // for patients page


        public List<ContentCommonModel> BookATest_List { get; set; } = new();
        public List<ContentCommonModel> WHyTrust_List { get; set; } = new();
        public List<ContentCommonModel> StepToSchedule_List { get; set; } = new();
        public List<ContentCommonModel> FAQ_List { get; set; } = new();
        public List<ContentCommonModel> Visitlab_List { get; set; } = new();
        public List<ContentCommonModel> needhelp_List { get; set; } = new();

    }



    public class ContentCommonModel
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
        public string ThumbnailImage { get; set; }
        public string ThumbnailAltText { get; set; }
        public string Url { get; set; }
        public string Url_Text { get; set; }
        public int Sequence { get; set; }
        public int IsBlock { get; set; }
        public string Video_path { get; set; }
        public string Video_poster { get; set; }
        public string Icon_Image { get; set; }
        public string Popup_Content { get; set; }
        public string Popup_Display_Title { get; set; }
        public string Section_title { get; set; }
        public string File_path { get; set; }
    }




}
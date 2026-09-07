using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class MenuHeaderModel
    {
        public int MenuId { get; set; }

        public int? ParentMenuId { get; set; }

        public int MenuCategory { get; set; }

        public string? MenuName { get; set; }

        public string? Url { get; set; }

        public string Target { get; set; } = "_self";

        public string MenuType { get; set; } = "Normal";

        public int Sequence { get; set; } = 0;

        // Mega Menu Feature/Card
        public string? FeatureImage { get; set; }

        public int? FeatureImageId { get; set; }
        public bool Feature_Image_IsDelete { get; set; }

        public string? FeatureTitle { get; set; }

        public string? FeatureDescription { get; set; }

        public string? FeatureButtonText { get; set; }

        public string? FeatureButtonUrl { get; set; }

        public string FeatureButtonTarget { get; set; } = "_self";

        public string? CssClass { get; set; }

        public int Status { get; set; } = 0;

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }


        public string? Thumb_image { get; set; }
        public int? Thumb_image_id { get; set; }
        public bool Thumb_image_IsDelete { get; set; }

        [RegularExpression(@"^[^`\^~#<>{}?]+", ErrorMessage = @"Please enter valid characters. The following characters are not accepted `^~#<> in the mouse over text for masthead")]
        public string? Thumb_image_alttext { get; set; }


    }
}
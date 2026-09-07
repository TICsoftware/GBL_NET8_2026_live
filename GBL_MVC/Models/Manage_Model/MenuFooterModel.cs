using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class MenuFooterModel
    {
        // Basic Information
        public int FooterId { get; set; }

        public int? ParentFooterId { get; set; }

        public string? FooterType { get; set; } = "Menu";

        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? Url { get; set; }

        public string Target { get; set; } = "_self";


        // Images
        public string? FooterImage { get; set; }

        public int? FooterImageId { get; set; }

        public bool FooterImageIsDelete { get; set; }

        public string? FooterThumbImage { get; set; }

        public int? FooterThumbImageId { get; set; }

        public bool FooterThumbImageIsDelete { get; set; }

        [RegularExpression(
            @"^[^`\^~#<>{}?]+$",
            ErrorMessage = "Please enter valid characters. The following characters are not accepted: ` ^ ~ # < >"
        )]
        public string? FooterImageAltText { get; set; }


        // Display Settings
        public string? IconClass { get; set; }

        public string? ColumnNo { get; set; }

        public int Sequence { get; set; } = 0;

        public int Status { get; set; } = 2;


        // Audit Information
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
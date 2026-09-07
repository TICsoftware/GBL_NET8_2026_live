using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GBL_MVC.Models.Manage_Model
{
    public class Model_Template_Master
    {
        public int ID { get; set; }

        [Display(Name = "Language")]
        //[Required(ErrorMessage = "Please select a language.")]
        public int? Language_Master_ID { get; set; }

        [Display(Name = "Template Name")]
        //[Required(ErrorMessage = "Please enter template name.")]
        [StringLength(120, ErrorMessage = "Template name should not exceed 120 characters.")]
        [RegularExpression(@"^(?! )[A-Za-z0-9 .'\-&()]+(?<! )$", ErrorMessage = "Please enter only valid characters (letters, numbers, spaces, and .'-&()).")]
        public string Name { get; set; }

        [Display(Name = "Status")]
        //[Required(ErrorMessage = "Please select status.")]
        public int? Status { get; set; }

        [Display(Name = "Created By")]
        public string?  Created_UserID { get; set; }

     //public string  Updated_UserID { get; set; }
        // Control action indicator (like save/update)
       public int? Updated_UserID { get; set; }
 // Control action indicator (like save/update)
        public int Template_Master_Save_Action { get; set; }
        // Additional property for result message
       // public string Result { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GBL_MVC
{
    public class Model_Tag_Master
    {
        public int ID { get; set; }

        [Display(Name = "Language")]
        [Required(ErrorMessage = "Please select a language.")]
        public int? Language_Master_ID { get; set; }

        [Display(Name = "Tag Name")]
        [Required(ErrorMessage = "Please enter tag name.")]
        [StringLength(120, ErrorMessage = "Tag name should not exceed 120 characters.")]
        [RegularExpression(@"^(?! )[A-Za-z0-9 .'\-&()]+(?<! )$", ErrorMessage = "Please enter only valid characters (letters, numbers, spaces, and .'-&()).")]
        public string? Tag_Name { get; set; }
        public string? Language_Name { get; set; }
        public string? Created_Date { get; set; }
        public string? Updated_Date { get; set; }
        public int Status { get; set; }

    }

    public class List_Model_Tags
    {
        public List<Model_Tag_Master>? List_tags { get; set; }
        public int Status { get; set; }
        public List<SelectListItem>? Languages { get; set; }
        public int language_id { get; set; }


    }

}
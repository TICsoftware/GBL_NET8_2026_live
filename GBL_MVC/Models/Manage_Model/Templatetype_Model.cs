using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GBL_MVC.Models.Manage_Model
{
    public class Templatetype_Model
    {

        public int ID { get; set; }

        [Required(ErrorMessage = "Please enter template type")]
        [Display(Name = "Block Layout Title")]
        public string Template_Type { get; set; }

        [Display(Name = "Status")]
        //[Required(ErrorMessage = "Please select status.")]
        public int? Status { get; set; }


        [Display(Name = "Design Layout")]
        public string Design_Layout { get; set; }


        [Display(Name = "Created By")]
        public string? Created_UserID { get; set; }


        public int? Updated_UserID { get; set; }
        // Control action indicator (like save/update)

        public List<int> SelectedComponentsIds { get; set; } = new();
        public string? ComponentsNames { get; set; }


        public int Template_Master_Save_Action { get; set; }

    }
}
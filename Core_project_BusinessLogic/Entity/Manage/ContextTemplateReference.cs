using System;
using System.ComponentModel.DataAnnotations;

namespace Core_project_BusinessLogic.Entity
{
    public class ContextTemplateReference
    {
        public int ID { get; set; }

        public int? Status { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a component layout.")]
        public int Context_Master_ID { get; set; }

        [Required(ErrorMessage = "Template is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a template.")]
        public int? Template_Master_ID { get; set; }

        [Required(ErrorMessage = "Sequence is required.")]
        [Range(1, 9999, ErrorMessage = "Sequence must be between 1 and 9999.")]
        public int? Sequence { get; set; }

        // Optional audit fields (add if needed later)
        public int? Created_UserID { get; set; }
        public DateTime? Created_Date { get; set; }

        public int? Updated_UserID { get; set; }
        public DateTime? Updated_Date { get; set; }

        public string? Context_Title { get; set; }

        [Required(ErrorMessage = "Label is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Label must be 2-100 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\s\-_#]+$", ErrorMessage = "Label contains invalid characters.")]
        public string? Component_Label { get; set; }
        public string? Template_Name { get; set; }

        public int field_id { get; set; }
        public string? group_id { get; set; }

        public bool HasData { get; set; }

        public int? is_block { get; set; }   // NEW COLUMN

         public int? Component_field_count { get; set; } 

    }

    public class Context_Component_Details
    {
        public List<ContextTemplateReference>? _context { get; set; }

        //Id, GUID, title , sequence
        public List<Tuple<int, int, string, string, int>>? _context_Details { get; set; }
    }
    public class ComponentOrderModel
    {
        public int id { get; set; }
        public int sequence { get; set; }
    }

}

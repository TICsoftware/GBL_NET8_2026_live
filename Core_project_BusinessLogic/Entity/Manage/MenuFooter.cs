using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class MenuFooter
    {
        // Basic Information
        public int FooterId { get; set; }

        public int? ParentFooterId { get; set; }

        public string ParentFooterTitle { get; set; }

        public string FooterType { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public string Target { get; set; }

        // Images
        public int FooterImageId { get; set; }

        public string FooterImage { get; set; }

        public int FooterThumbImageId { get; set; }

        public string FooterThumbImage { get; set; }

        // Display Settings
        public string IconClass { get; set; }

        public string ColumnNo { get; set; }

        public int Sequence { get; set; }

        // Status
        public int Status { get; set; }

        // Audit Information
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
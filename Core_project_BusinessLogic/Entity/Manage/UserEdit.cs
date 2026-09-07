using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_project_BusinessLogic.Entity.Manage
{
    public class UserEdit
    {
        public int user_id { get; set; }

        public string? password { get; set; }
        public byte? usr_status { get; set; }
        public string username { get; set; }
        public DateTime? usr_createdate { get; set; }
        public DateTime? usr_updatedate { get; set; }

        public string? secret { get; set; }

        public bool Is2FAEnabled { get; set; }

        public DateTime LastLogin { get; set; }

        public int UserType_Id { get; set; }

        public string Email { get; set; }

        public string PhoneNo { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

         public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDate { get; set; }

    }

}
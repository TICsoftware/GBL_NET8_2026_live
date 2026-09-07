using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIC_CMS_BusinessLogic.Entity
{
    public class User_Master
    {
        public int usr_id { get; set; }
        public int? usr_roleid { get; set; }
        public string usr_login { get; set; }
        public string usr_password { get; set; }
        public byte? usr_status { get; set; }
        public string usr_firstname { get; set; }
        public string usr_lastname { get; set; }
        public string usr_emailid { get; set; }
        public DateTime? usr_createdate { get; set; }
        public DateTime? usr_updatedate { get; set; }
        public string role_taskids { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace GBL_BusinessLogic.DAL
{
    public class ContentRepository : DBHelper
    {
        public ContentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public DataTable GetContentForSitemap()
        {
            return GetDataSet("GetContentForSitemap").Tables[0];
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace TestO2DDLL
{
    class Cls_Connection
    {
        public string GetConnectionString()
        {
            string conStr = string.Empty;
            conStr = "Data Source=203.154.67.28,1433;Initial Catalog=O2D_Dup;Timeout=30;User Id=O2DDUPLEX;Password=4Ward;MultipleActiveResultSets=True;Application Name=O2D;";


            return conStr;
        }
    }
}

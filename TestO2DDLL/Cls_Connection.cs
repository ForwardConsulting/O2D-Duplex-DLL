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
            conStr = "Data Source=203.150.244.14,1433;Initial Catalog=O2D_Dup;Timeout=30;User Id=rmuser;Password=scgrawmat;MultipleActiveResultSets=True;Application Name=O2D;";


            return conStr;
        }
    }
}

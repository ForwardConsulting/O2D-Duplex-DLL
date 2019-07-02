using System;
using System.Collections.Generic;
using System.Text;

namespace Duplex.Tool
{
    class clsConversion
    {
        internal DateTime GetDateFromSAPDate(string SAPDate)
        {
            try
            {
                //SAPDate should be in format 20190615 for example
                string yeartmp;
                string monthtmp;
                string datetmp;
                string resultstr;
                DateTime result;
                yeartmp = SAPDate.Substring(0, 4);
                monthtmp = SAPDate.Substring(4, 2);
                datetmp = SAPDate.Substring(6, 2);
                resultstr = ($"{yeartmp}/{monthtmp}/{datetmp}");
                result = DateTime.Parse(resultstr);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception ($"Error on GetDateFromSAPDate:{ex.Message}");
            }

        }
    }
}

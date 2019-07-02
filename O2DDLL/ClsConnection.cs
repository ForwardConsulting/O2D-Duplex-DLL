using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace O2DDLL
{
    class ClsConnection
    {
        private string _conStr;
        public ClsConnection( string connectionString)
        {
            _conStr = connectionString;
        }
        public DataSet FillDataProcedure(ref DataSet ds, string procName, string[] paramName,object[] paramValue,string[] tableName=null)
        {
            if (paramName!=null)
            {
                if (paramName.Length != paramValue.Length )
                {
                    throw new Exception("ParameterName and Value must be equal length of array");
                }
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand ())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;
                    cmd.CommandTimeout = 30;
                    for (int j=0;j<=paramName.GetUpperBound(0);j+=1)
                    {
                        cmd.Parameters.AddWithValue(paramName[j], paramValue[j]);
                    }
                        
                    using (SqlDataAdapter sd = new SqlDataAdapter(cmd))
                    {
                        sd.Fill(ds);
                    }

                }

            }
            if (tableName != null)
            {
                if(ds.Tables.Count !=tableName.Length)
                {
                    throw new Exception("TableName must be equal to Datatable amount tin Dataset");
                }
                else
                {
                    for (int j=0;j<=tableName.GetUpperBound(0); j += 1)
                    {
                        ds.Tables[j].TableName = tableName[j];
                    }
                }
            }
            /* Mark records to be added state */
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr.SetAdded();
                }
            }
            return ds;
        }
    }
}

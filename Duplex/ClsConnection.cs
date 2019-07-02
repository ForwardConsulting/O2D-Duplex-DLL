using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Duplex
{
    class ClsConnection
    {
        private readonly string _conStr;

        public ClsConnection(string connectionString)
        {
            this._conStr = connectionString;
        }

        internal string GetSqlFormat(object str)
        {
            try
            {
                string result = string.Empty;
                DateTime tmpDate;
                Double tmpNum;
                string yrtmp;
                string mthtmp;
                string daytmp;
                string hourtmp;
                string minutetmp;
                string secondtmp;
                if (str==null)
                {
                    result = "Null";
                    goto FinalStep;
                }

                if (DateTime.TryParse(Convert.ToString(str), out tmpDate) == true)
                {
                    yrtmp = Convert.ToString(tmpDate.Year);
                    mthtmp = Convert.ToString(tmpDate.Month);
                    daytmp = Convert.ToString(tmpDate.Day);
                    hourtmp = Convert.ToString(tmpDate.Hour);
                    minutetmp = Convert.ToString(tmpDate.Minute);
                    secondtmp = Convert.ToString(tmpDate.Second);
                    result = $"'{yrtmp}/{mthtmp}/{daytmp} {hourtmp}:{minutetmp}:{secondtmp}'";
                    goto FinalStep;
                }

                if (Double.TryParse (Convert.ToString(str),out tmpNum))
                {
                    result = Convert.ToString(tmpNum);
                    goto FinalStep;
                }

                result = $"'{Convert.ToString(str)}'";

                FinalStep:
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on GetSqlFormat:{ex.Message}");
            }
        }
        internal int GetNextID(string Table)
        {
            try
            {
                int result;
                string procName = "X10_SP_GetNextID";
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure ;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 60;
                        cmd.Parameters.AddWithValue("@TableName", Table);
                        cmd.Parameters.AddWithValue("@UserID", 0);
                        cmd.Parameters.Add("@NextID", SqlDbType.Int);
                        cmd.Parameters["@NextID"].Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result = Convert.ToInt32 (cmd.Parameters["@NextID"].Value);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on GetSqlFormat:{ex.Message}");
            }
        }
        internal DataTable FillDataQuery(string strSql)
        {
            DataTable DT = new DataTable();
            using (SqlConnection con = new SqlConnection(_conStr))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strSql;
                    cmd.CommandTimeout = 60;
                    using (SqlDataAdapter sd = new SqlDataAdapter(cmd))
                    {
                        sd.Fill(DT);
                    }
                }
            }
            return DT;
        }
        public DataSet FillDataProcedure(ref DataSet ds, string procName, string[] paramName, object[] paramValue, string[] tableName = null)
        {
            if (paramName != null)
            {
                if (paramName.Length != paramValue.Length)
                {
                    throw new Exception("ParameterName and Value must be equal length of array");
                }
            }

            using (SqlConnection con = new SqlConnection(_conStr))
            {
                con.Open();
                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = procName;
                    cmd.CommandTimeout = 30;
                    for (int j = 0; j <= paramName.GetUpperBound(0); j += 1)
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
                if (ds.Tables.Count != tableName.Length)
                {
                    throw new Exception("TableName must be equal to Datatable amount tin Dataset");
                }
                else
                {
                    for (int j = 0; j <= tableName.GetUpperBound(0); j += 1)
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

        public void ExecuteNonQuery(string strSql)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 60;
                        cmd.CommandText = strSql;
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error on ExecuteNonQuery:" + ex.Message);
            }

        }

        public string GetSqlDateFormatFromSAPFormat(string DateSAPFormat)
        {
            //SAP Date format such as 20190629 
            string yearTmp;
            string monthTmp;
            string dateTmp;
            string result;
            yearTmp = DateSAPFormat.Substring(0, 4);
            monthTmp = DateSAPFormat.Substring(4, 2);
            dateTmp = DateSAPFormat.Substring(6, 2);
            result = yearTmp + "/" + monthTmp + "/" + dateTmp;
            return result;
        }
    }
}

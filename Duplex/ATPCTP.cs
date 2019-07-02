using Duplex.Model;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Duplex
{
    public class ATPCTP
    {
        private string _conStr;
        private DataSet _ds;
        public clsDTOrder DTOrder=new clsDTOrder();
        #region "Property"
        public string ConStr
        {
            get
            {
                return _conStr;
            }
            set
            {
                _conStr = value;
            }
        }
       
        #endregion
        //public string ConStr { get => _conStr; set => _conStr = value; }


        public ATPCTP(String connectionString)
        {
            ConStr = connectionString;
        }
        /// <summary>
        /// For caller to call for ATPCTP result.
        /// </summary>
        /// <param name="materialID"></param>
        /// <param name=""></param>
        /// <returns></returns>
        //public DataSet Request(DataTable dtOrder, DataTable dtOrderItem, int atpCtpLogicId)
        public DataSet Request(clsDTOrder dtOrder, clsDTOrderItem dtOrderItem, int atpCtpLogicId)
        {
            try
            {
                _ds = new DataSet();
                string procName = "proc_DUP_ATPCTPRequestSelect_4096";
                DataTable dt1;
                DataTable dt2;
                dt1 = dtOrder.DT;
                dt2 = dtOrderItem.DT;
                //ClsConnection con = new ClsConnection(_conStr);

                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@DTOrder", dt1);
                        cmd.Parameters.AddWithValue("@DTOrderItem", dt2);
                        cmd.Parameters.AddWithValue("@ATPCTPLogicID", atpCtpLogicId);


                        using (SqlDataAdapter sd = new SqlDataAdapter(cmd))
                        {
                            sd.Fill(_ds);
                        }

                    }

                }
                _ds.Tables[0].TableName = "DTOrderItem";
                _ds.Tables[1].TableName = "DTOrderItemOperation";
                _ds.Tables[2].TableName = "DTOrderItemOperationDetail";

                return _ds;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool Confirm(string ConfirmListID, int UserID)
        {
            try
            {
                string procName = "proc_DUP_ConfirmATPCTP_4322";
                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@ConfirmList", ConfirmListID);
                        cmd.Parameters.AddWithValue("@UserID", UserID);

                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }


        }
    }
}

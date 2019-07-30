using Duplex.Model;
using Duplex.Tool;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Duplex
{
    public class ATPCTP
    {
        private static clsLog log;
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


        public ATPCTP(String customDBconnectionString,String StdDBConnectionString)
        {
            ConStr = customDBconnectionString;
            log = new clsLog(StdDBConnectionString);
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
                Console.WriteLine("Start Request ...");
                int logID;
                logID = log.LogProcessInsert(clsLog.Logger.ATPCTP, clsLog.ProcessCategory.RequestATPCTP, "ATPCTP Request", DateTime.Now);
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
                log.LogProcessUpdate(logID, DateTime.Now);
                Console.WriteLine("End ATPCTPRequest");
                return _ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error found on Request:{ex.Message}");
                throw ex;
            }

        }
        public bool Confirm(clsDTConfirm DTConfirm, int UserID)
        {
            try
            {
                Console.WriteLine("Start Confirm ... ");
                int logID;
                logID = log.LogProcessInsert(clsLog.Logger.ATPCTP, clsLog.ProcessCategory.ConfirmATPCTP, "ATPCTP Confrim", DateTime.Now);
                string procName = "proc_DUP_ConfirmATPCTP_4322";
                DataTable DT1 = DTConfirm.DT;

                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@DTConfirm", DT1);
                        cmd.Parameters.AddWithValue("@UserID", UserID);

                        cmd.ExecuteNonQuery();
                    }
                }
                log.LogProcessUpdate(logID, DateTime.Now);
                Console.WriteLine("End Confirm");
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    log.LogAlert(clsLog.Logger.ATPCTP, clsLog.ErrorLevel.CriticalImapact, clsLog.ProcessCategory.ConfirmATPCTP, $"Unable to Confirm ATCTP {ex.Message}");
                }
                catch (Exception)
                {
                    //nothing
                }
                finally
                {
                    Console.WriteLine($"Error found on Confirm {ex.Message}");
                    throw ex;
                }
                
            }


        }
    }
}

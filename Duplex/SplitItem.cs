using Duplex.Model;
using Duplex.Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Duplex
{
    public class SplitItem
    {
        private static clsLog log;
        private string _conStr;
        private DataSet _ds;

        public SplitItem(String customDBconnectionString, String StdDBConnectionString)
        {
            ConStr = customDBconnectionString;
            log = new clsLog(StdDBConnectionString);
        }

        public SplitItem()
        {
        }

        public string ConStr { get => _conStr; set => _conStr = value; }

        public DataSet Request(clsDTSplitItem DtSplitItem, int UserID, ref string WarningMsg)
        {
            try
            {
                WarningMsg = string.Empty;
                Console.WriteLine("Start Request ...");
                int logID;
                logID = log.LogProcessInsert(clsLog.Logger.Order, clsLog.ProcessCategory.RequestSplitItem, "SplitItem Request", DateTime.Now);
                _ds = new DataSet();
                string procName = "proc_DUP_SplitItemRequest_4627";
                DataTable dt1;
                dt1 = DtSplitItem.DT;

                //ClsConnection con = new ClsConnection(_conStr);

                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 180;
                        cmd.Parameters.AddWithValue("@DTOrderSplit", dt1);
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        using (SqlDataAdapter sd = new SqlDataAdapter(cmd))
                        {
                            sd.Fill(_ds);
                        }

                    }

                }
                _ds.Tables[0].TableName = "DTSplitItem";

                log.LogProcessUpdate(logID, DateTime.Now);
                WarningMsg = string.Empty;
                Console.WriteLine("End SplitItem Request");
                return _ds;
            }
            catch (Exception ex)
            {
                string errmsg = ex.Message;
                string showerrmsg = string.Empty;
                if (_ds != null)
                {
                    _ds.Tables[0].TableName = "DTSplitItem";
                }
                if (IsRealError(errmsg, ref showerrmsg) == true)
                {
                    WarningMsg = string.Empty;
                    Console.WriteLine($"Error found on Request:{ex.Message}");
                    throw ex;
                }
                else
                {
                    WarningMsg = showerrmsg;
                    return _ds;
                }

            }

        }
        bool IsRealError(string ErrorMessage, ref string output)
        {
            string[] errmsg;
            bool result = false;
            errmsg = ErrorMessage.Split("\r");
            if (errmsg.Length > 0)
            { ErrorMessage = errmsg[0]; }
            else { errmsg = ErrorMessage.Split(":"); }


            if (ErrorMessage.ToLower().Contains("material") || ErrorMessage.ToLower().Contains("order")
                         || ErrorMessage.ToLower().Contains("request") || ErrorMessage.ToLower().Contains("logic") || ErrorMessage.ToLower().Contains("route") || ErrorMessage.ToLower().Contains("invent")
                         || ErrorMessage.ToLower().Contains("item"))
            {
                result = false;
            }
            else
            {
                result = true;
            }
            output = ErrorMessage;
            return result;

        }


        public bool Confirm(int UserID, ref string WarningMsg)
        {
            try
            {
                Console.WriteLine("Start Splititem Confirm ... ");
                int logID;
                logID = log.LogProcessInsert(clsLog.Logger.Order, clsLog.ProcessCategory.ConfirmSplitItem, "Split Confirm", DateTime.Now);
                string procName = "proc_DUP_SplitItemConfirm_4628";

                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 180;
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.ExecuteNonQuery(); //nothing return to GUI
                    }
                }

                log.LogProcessUpdate(logID, DateTime.Now);
                Console.WriteLine("End Confirm SplitItem");
                return true;
            }
            catch (Exception ex)
            {
                string errmsg = ex.Message;
                string showerrmsg = string.Empty;
                WarningMsg = errmsg;
                try
                {
                    log.LogAlert(clsLog.Logger.Order, clsLog.ErrorLevel.CriticalImapact, clsLog.ProcessCategory.ConfirmSplitItem, $"Unable to Confirm SplitItem {errmsg}");
                    if (IsRealError(errmsg, ref showerrmsg) == true)
                    {
                        WarningMsg = errmsg;
                        Console.WriteLine($"Error found on Confirm SplitItem:{errmsg}");
                        return false;
                    }
                    else
                    {
                        WarningMsg = showerrmsg;
                        return true;
                    }
                }
                catch (Exception)
                {
                    //nothing
                }
                finally
                {
                    Console.WriteLine($"Error found on Confirm SplitItem {ex.Message}");
                    throw ex;
                }

            }

        }
    }
}


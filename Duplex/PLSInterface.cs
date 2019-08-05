using Duplex.Tool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Duplex
{
    /// <summary>
    /// Task#4425:Caller should submit id of DT_OrderItemOperation which user tick and press button expect to send those to PLS system. At here, we will write data direct into PLS system only.
    /// </summary>
    public class PLSInterface
    {
        private DataTable _DTOrderItemOpr;
        private string _conStr;
        private int _operationID;
        private int _logID;

        private static clsLog log;

        public PLSInterface(String customDBconnectionString, String StdDBConnectionString)
        {
            _conStr = customDBconnectionString;
            log = new clsLog(StdDBConnectionString);
        }


        public DataTable DTOrderItemOpr { get => _DTOrderItemOpr; set => _DTOrderItemOpr = value; }
        public string ConStr { get => _conStr; set => _conStr = value; }
        public int OperationID { get => _operationID; set => _operationID = value; }

        public bool ExportData(ref string WarningMsg)
        {
            WarningMsg = string.Empty;
            Console.WriteLine($"Start Export Data to PLS");
            string procName = "proc_O2D_PLSInterface_4475";
            if (_DTOrderItemOpr == null)
            {
                throw new Exception($"Please add OperationID and call method Addrow to create internal table first");
            }
            try
            {
                _logID = log.LogProcessInsert(clsLog.Logger.PLSInterface, clsLog.ProcessCategory.Interface, "Export Plan to PLS", DateTime.Now);
                using (SqlConnection con = new SqlConnection(ConStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@DTRefID", _DTOrderItemOpr);
                        cmd.ExecuteNonQuery();
                    }

                }
                WarningMsg = string.Empty;
                log.LogProcessUpdate(_logID, DateTime.Now);
                Console.WriteLine($"End of Export Data to PLS");
                return true; 
            }
            catch (SqlException ex)
            {
                string errmsg = ex.Message;
                string showerrmsg = string.Empty;
                try
                {
                    Console.WriteLine($"Error found on ExportData:{errmsg}");
                    log.LogAlert(clsLog.Logger.PLSInterface, clsLog.ErrorLevel.CriticalImapact, clsLog.ProcessCategory.Interface, $"Unable to export plan to PLS {errmsg}");
                    if (IsRealError(errmsg, ref showerrmsg) == true)
                    {
                        WarningMsg = string.Empty;
                        throw ex;
                    }
                    else
                    {
                        WarningMsg = showerrmsg;
                        return false;
                    }
                    
                }
                catch (Exception ex1)
                {
                    Console.WriteLine($"writing log has an error {ex1.Message}");
                    return false;
                    //nothing to handle here
                }

            }
        }


        bool IsRealError(string ErrorMessage, ref string output)
        {
            string[] errmsg;
            bool result = false;
            //errmsg = ErrorMessage.Split(":");
            errmsg = ErrorMessage.Split("\r");
            if (errmsg.Length > 0) { ErrorMessage = errmsg[0]; }
            errmsg = ErrorMessage.Split(":");
            if (errmsg.Length < 2)
            {
                ErrorMessage = errmsg[0];
            }
            else
            {
                ErrorMessage = errmsg[1];
            }

            

            if (ErrorMessage.ToLower().Contains("material") || ErrorMessage.ToLower().Contains("order")
                || ErrorMessage.ToLower().Contains("request") || ErrorMessage.ToLower().Contains("logic") || ErrorMessage.ToLower().Contains("route") || ErrorMessage.ToLower().Contains("invent") || ErrorMessage.ToLower().Contains("equipment") || ErrorMessage.ToLower().Contains ("operation"))
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
        public void AddRow()
        {
            try
            {
                if (_DTOrderItemOpr == null)
                {
                    _DTOrderItemOpr = new DataTable();
                    _DTOrderItemOpr.Columns.Add("ID", typeof(int));
                }

                DataRow DR = _DTOrderItemOpr.NewRow();
                DR["ID"] = _operationID;
                _DTOrderItemOpr.Rows.Add(DR);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on AddRow (PLSInterface):{ex.Message}");
                throw new Exception($"Error on AddRow:{ex.Message}");
            }
        }

    }
}

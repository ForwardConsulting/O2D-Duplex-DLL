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

        public PLSInterface(string ConnectionString)
        {
            _conStr = ConnectionString;
            log = new clsLog(_conStr);
        }


        public DataTable DTOrderItemOpr { get => _DTOrderItemOpr; set => _DTOrderItemOpr = value; }
        public string ConStr { get => _conStr; set => _conStr = value; }
        public int OperationID { get => _operationID; set => _operationID = value; }

        public void ExportData()
        {
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
                log.LogProcessUpdate(_logID, DateTime.Now);
                Console.WriteLine($"End of Export Data to PLS");
            }
            catch (Exception ex)
            {
                try
                {
                    log.LogAlert(clsLog.Logger.PLSInterface, clsLog.ErrorLevel.CriticalImapact, clsLog.ProcessCategory.Interface, $"Unable to export plan to PLS {ex.Message}");
                }
                catch (Exception ex1)
                {
                    Console.WriteLine($"writing log has an error {ex1.Message}");
                    //nothing to handle here
                }
                finally
                {
                    Console.WriteLine($"Error found on ExportData:{ex.Message}");
                    throw new Exception($"Error on ExportData:{ex.Message }");
                }
                
            }
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

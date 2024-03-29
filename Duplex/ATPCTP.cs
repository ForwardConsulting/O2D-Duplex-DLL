﻿using Duplex.Model;
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
        public clsDTOrder DTOrder = new clsDTOrder();
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


        public ATPCTP(String customDBconnectionString, String StdDBConnectionString)
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
        public DataSet Request(clsDTOrder dtOrder, clsDTOrderItem dtOrderItem, int atpCtpLogicId, ref string WarningMsg)
        {
            try
            {
                WarningMsg = string.Empty;
                Console.WriteLine("Start Request ...");
                int logID;
                logID = log.LogProcessInsert(clsLog.Logger.ATPCTP, clsLog.ProcessCategory.RequestATPCTP, "ATPCTP Request", DateTime.Now);
                _ds = new DataSet();
                LogInputParameter(dtOrder, dtOrderItem, atpCtpLogicId);
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
                        cmd.CommandTimeout = 180;
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
                WarningMsg = string.Empty;
                Console.WriteLine("End ATPCTPRequest");
                if (_ds.Tables.Count > 0)
                {
                    if (_ds.Tables[0].Rows.Count == 0 && WarningMsg == string.Empty)
                    {
                        WarningMsg = "No WIP found either in Balance inventory or Material Master";
                    }
                }
                LogOutputResult(_ds);
                return _ds;
            }
            catch (Exception ex)
            {
                string errmsg = ex.Message;
                string showerrmsg = string.Empty;
                if (_ds != null)
                {
                    _ds.Tables[0].TableName = "DTOrderItem";
                    _ds.Tables[1].TableName = "DTOrderItemOperation";
                    _ds.Tables[2].TableName = "DTOrderItemOperationDetail";
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
                    if (_ds.Tables.Count > 0)
                    {
                        if (_ds.Tables[0].Rows.Count == 0 && WarningMsg == string.Empty)
                        {
                            WarningMsg = "No WIP found either in Balance inventory or Material Master";
                        }
                    }
                    return _ds;
                }

            }

        }

        private void LogOutputResult(DataSet ds)
        {
            try
            {
                string tmpStr = string.Empty;
                string headerMsg = string.Empty;
                foreach (DataTable DT in _ds.Tables)
                {
                    headerMsg = $@"Table {DT.TableName} => ";
                    foreach (DataRow DR in DT.Rows)
                    {
                        tmpStr = string.Empty;
                        foreach (DataColumn DC in DT.Columns)
                        {
                            tmpStr += $@" {DC.ColumnName}:{DR[DC.ColumnName]},";
                        }
                        if (tmpStr.Length > 2000)
                        {
                            tmpStr = tmpStr.Substring(0, 1900);
                        }
                        log.LogAlert(clsLog.Logger.ATPCTP, clsLog.ErrorLevel.NoImpact, clsLog.ProcessCategory.RequestATPCTP, $@"{headerMsg}{tmpStr}");
                    }

                }

            }
            catch (Exception ex)
            {
                throw new Exception($@"Error on LogOutputResult:{ex.Message}");
            }
        }

        private void LogInputParameter(clsDTOrder dtOrder, clsDTOrderItem dtOrderItem, int atpCtpLogicId)
        {
            try
            {
                string tmpLog = string.Empty;
                tmpLog = "Order Level => ";
                foreach (DataRow DR in dtOrder.DT.Rows)
                {
                    foreach (DataColumn DC in dtOrder.DT.Columns)
                    {
                        tmpLog += $@" {DC.ColumnName}: {DR[DC.ColumnName]},";
                    }
                }
                log.LogAlert(clsLog.Logger.ATPCTP, clsLog.ErrorLevel.NoImpact, clsLog.ProcessCategory.RequestATPCTP, tmpLog);

                tmpLog = "OrderItem Level => ";
                foreach (DataRow DR in dtOrderItem.DT.Rows)
                {
                    foreach (DataColumn DC in dtOrderItem.DT.Columns)
                    {
                        tmpLog += $@" {DC.ColumnName}: {DR[DC.ColumnName]},";
                    }
                }
                log.LogAlert(clsLog.Logger.ATPCTP, clsLog.ErrorLevel.NoImpact, clsLog.ProcessCategory.RequestATPCTP, tmpLog);


            }
            catch (Exception ex)
            {
                throw new Exception($@"Error on LogInputParameter:{ex.Message}");
            }
        }

        bool IsRealError(string ErrorMessage, ref string output)
        {
            string[] errmsg;
            bool result = false;
            //errmsg = ErrorMessage.Split(":");
            errmsg = ErrorMessage.Split("\r");
            if (errmsg.Length > 0)
            { ErrorMessage = errmsg[0]; }
            else { errmsg = ErrorMessage.Split(":"); }

            if (ErrorMessage.Contains(":"))
            {
                errmsg = ErrorMessage.Split(":");
            }
            if (errmsg.Length < 2)
            {
                output = string.Empty;
                return false;
            }
            else { ErrorMessage = errmsg[1]; }


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
        public bool Confirm(clsDTConfirm DTConfirm, int UserID, ref string WarningMsg)
        {
            try
            {
                Console.WriteLine("Start Confirm ... ");
                int logID;
                logID = log.LogProcessInsert(clsLog.Logger.ATPCTP, clsLog.ProcessCategory.ConfirmATPCTP, "ATPCTP Confirm", DateTime.Now);
                string procName = "proc_DUP_ConfirmATPCTP_4322";
                DataTable DT1 = DTConfirm.DT;

                using (SqlConnection con = new SqlConnection(_conStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 180;
                        SqlParameter dtc;
                        dtc = cmd.Parameters.Add("@DTConfirm", SqlDbType.Structured);
                        dtc.Value = DT1;
                        dtc.TypeName = "dbo.DT_ConfirmATPCTP";

                        cmd.Parameters.AddWithValue("@UserID", UserID);

                        cmd.ExecuteNonQuery();
                    }
                }

                log.LogProcessUpdate(logID, DateTime.Now);
                Console.WriteLine("End Confirm");
                return true;
            }
            catch (SqlException ex)
            {
                string errmsg = ex.Message;
                string showerrmsg = string.Empty;
                WarningMsg = errmsg;
                try
                {
                    log.LogAlert(clsLog.Logger.ATPCTP, clsLog.ErrorLevel.CriticalImapact, clsLog.ProcessCategory.ConfirmATPCTP, $"Unable to Confirm ATCTP {errmsg}");
                    if (IsRealError(errmsg, ref showerrmsg) == true)
                    {
                        WarningMsg = errmsg;
                        Console.WriteLine($"Error found on Confirm ATPCTP:{errmsg}");
                        return false;
                    }
                    else
                    {
                        WarningMsg = showerrmsg;
                        return false;
                    }
                }
                catch (Exception)
                {
                    //nothing
                    throw ex;
                }


            }


        }

        public void LogConfirmInput(DataTable DT)
        {
            try
            {
                string tmpLog = string.Empty;
                tmpLog = "Confirm Param => ";
                foreach (DataRow DR in DT.Rows)
                {
                    foreach (DataColumn DC in DT.Columns)
                    {
                        tmpLog += $@" {DC.ColumnName}: {DR[DC.ColumnName]},";
                    }
                }
                log.LogAlert(clsLog.Logger.ATPCTP, clsLog.ErrorLevel.NoImpact, clsLog.ProcessCategory.ConfirmATPCTP, tmpLog);
            }
            catch (Exception ex)
            {

            }
        }

    }
}

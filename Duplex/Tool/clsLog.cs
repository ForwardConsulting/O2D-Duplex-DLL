﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Duplex.Tool
{
    class clsLog
    {
        public enum Logger
        {
            Unspecified,Inventory, Order, ProductionPlan, Delivery
        }
        public enum ErrorLevel
        {
            NoImpact = 0,
            SoftImpact = 1,
            MiddleImpact = 2,
            HardImpact = 3,
            CriticalImapact = 4,
            VeryCriticalImpact = 5
        }
        public enum ProcessCategory
        {
            Unspecified, Inventory, Order, ProductionPlan, Delivery
        }


        private string _conStr;

        public string ConStr { get => _conStr; set => _conStr = value; }


        public clsLog(string ConnectionString)
        {
            ConStr = ConnectionString;
        }

        internal void LogAlert(Logger LoggerModule, ErrorLevel CriticalLevel, ProcessCategory Category, string Message)
        {
            try
            {
                string strSql;
                ClsConnection con = new ClsConnection(_conStr);
                strSql = $"Insert into DT_LogAlert(LoggerModule,CriticalLevel,Category,Description" +
                    $",InsertDate,UpdateDate,InsertUserID,UpdateUserID)" +
                    $" Values({con.GetSqlFormat(Convert.ToString(LoggerModule))},{(int)CriticalLevel},{con.GetSqlFormat(Convert.ToString(Category))},{con.GetSqlFormat(Message)},GetDate(),GetDate(),0,0)";
                con.ExecuteNonQuery(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on LogAlert:{ex.Message}");
            }
        }
        /// <summary>
        /// Log process will return ID to caller. Caller can remember and keep update end date or something else on LogProcessUpdate function.
        /// </summary>
        /// <param name="LoggerModule"></param>
        /// <param name="Category"></param>
        /// <param name="Message"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        internal int LogProcessInsert(Logger LoggerModule, ProcessCategory Category, string Message, DateTime StartDate)
        {
            try
            {
                int LogID;
                string strSql;
                ClsConnection con = new ClsConnection(_conStr);
          
                LogID = con.GetNextID("DT_LogProcess");
                strSql = $"Insert into DT_LogProcess(ID,LoggerModule,Category,Description,StartDate" +
                    $",InsertDate,UpdateDate,InsertUserID,UpdateUserID)" +
                    $" Values({LogID},{con.GetSqlFormat(Convert.ToString(LoggerModule))},{con.GetSqlFormat(Convert.ToString(Category))},{con.GetSqlFormat(Message)}" +
                    $",{con.GetSqlFormat(StartDate)}" +
                    $",GetDate(),GetDate(),0,0)";


                con.ExecuteNonQuery(strSql);
                return LogID;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on LogProcessInsert:{ex.Message}");
            }
        }
        internal void LogProcessUpdate(int LogID,DateTime EndDate )
        {
            try
            {
                string strSql;
                ClsConnection con = new ClsConnection(_conStr);
                strSql = $"Update DT_LogProcess Set EndDate={con.GetSqlFormat(EndDate)},UpdateDate=GetDate(),UpdateUserID=0" +
                    $" where ID={LogID}";
                con.ExecuteNonQuery(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on LogProcessUpdate:{ex.Message}");
            }
        }
    }
}

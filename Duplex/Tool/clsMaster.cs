using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Duplex.Tool
{
    public class clsMaster
    {
        private string _conStr;
        private clsLog.Logger _logger;
        private clsLog.ProcessCategory _logProcess;

        private clsLog Log;
        private ClsConnection con;

        internal string ConStr { get => _conStr; set => _conStr = value; }
        internal clsLog.Logger Logger { get => _logger; set => _logger = value; }
        internal clsLog.ProcessCategory LogProcess { get => _logProcess; set => _logProcess = value; }

        public clsMaster(string connectionString)
        {
            ConStr = connectionString;
            con = new ClsConnection(ConStr);
            Log = new clsLog(ConStr);
            Logger = clsLog.Logger.Unspecified; //default.. caller have to change it on new instance
            LogProcess = clsLog.ProcessCategory.Unspecified; //default
        }

        internal int GetUnitID(string UnitCode)
        {
            string strSql;
            DataTable dt = new DataTable();
            strSql = $"Select ID From [O2D].dbo.MT_Unit m where m.[Code]='{UnitCode}'";
            dt = con.FillDataQuery(strSql);
            if (dt.Rows.Count == 0)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(dt.Rows[0][0]);
            }
        }
        /// <summary>
        /// Look for current data opening inventory
        /// </summary>
        /// <returns></returns>
        internal DateTime GetOpeningInventoryDate()
        {
            try
            {
                DateTime datechk = DateTime.Parse("2019/1/1");
                string strSql;
                DataTable dt;
                strSql = "Select top 1 OpeningDate From DT_InventoryOpening d";
                dt = con.FillDataQuery(strSql);
                if (dt.Rows.Count > 0)
                {
                    datechk = Convert.ToDateTime(dt.Rows[0][0]);
                }
                return datechk;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on GEtOpeningInventoryDate:{ex.Message }");
            }
        }
        internal int GetMaterialId(string MaterialCode, Boolean IsLogError = false)
        {
            DataTable DT = new DataTable();
            string strSql = string.Empty;
            int materialID = 0;
            strSql = "Select ID From [O2D].dbo.MT_Material m" +
                    " where m.[Code]='" + MaterialCode + "';";
            DT = con.FillDataQuery(strSql);
            if (DT.Rows.Count > 0)
            {
                materialID = Convert.ToInt32(DT.Rows[0]["ID"]);
                if (materialID == 0 && IsLogError == true)
                {
                    Log.LogAlert(Logger, clsLog.ErrorLevel.HardImpact, LogProcess, $"Material not found in O2D system({MaterialCode})");
                }
                return materialID;
            }
            else
            {
                if (materialID == 0 && IsLogError == true)
                {
                    Log.LogAlert(Logger, clsLog.ErrorLevel.HardImpact, LogProcess, $"Material not found in O2D system({MaterialCode})");
                }
                return 0;
            }
        }
        internal int GetWarehouseID(string WareHouseCode, Boolean IsLogError = false)
        {
            DataTable DT = new DataTable();
            string strSql = string.Empty;
            int warehouseID = 0;
            strSql = "Select ID From [O2D].dbo.MT_Warehouse m" +
                    " where m.[Code]='" + WareHouseCode + "';";
            DT = con.FillDataQuery(strSql);
            if (DT.Rows.Count > 0)
            {
                warehouseID = Convert.ToInt32(DT.Rows[0]["ID"]);
                if (warehouseID == 0 && IsLogError == true)
                {
                    Log.LogAlert(Logger, clsLog.ErrorLevel.HardImpact, LogProcess, $"Warehouse not found in O2D system({WareHouseCode})");
                }
                return warehouseID;
            }
            else
            {
                if (warehouseID == 0 && IsLogError == true)
                {
                    Log.LogAlert(Logger, clsLog.ErrorLevel.HardImpact, LogProcess, $"Warehouse not found in O2D system({WareHouseCode})");
                }
                return 0;
            }
        }
    }
}

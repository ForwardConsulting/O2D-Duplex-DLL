using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Duplex.Tool;
using System.Data;

namespace Duplex
{
    public class Inventory
    {
        private System.IO.FileInfo _textOPFile; //Opening snapshot file
        private string[] _textOPContent;
        private string _conStr;
        private DataTable _dttxt; // raw data from text file
        private DataTable _dtconv; //after convert to code id
        private DateTime _currentInvDate; //Date in database
        private DateTime _txtInvDate; // Date in text file
        private int _LogprocessID; //To update EndDate later

        public string ConStr { get => _conStr; set => _conStr = value; }

        public Inventory(string ConnectionString)
        {
            ConStr = ConnectionString;
        }
        public Boolean ImportOpening(System.IO.FileInfo textFile)
        {
            //try
            //{
            string msg = string.Empty;
            _textOPFile = textFile;
            if (IsValidOpeningInventoryFile(ref msg) == false)
            {
                throw new Exception(msg);
            }
            if (StartImportOpeningProcess(ref msg) == false)
            {
                throw new Exception(msg);
            }
            return true;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception ($"Error on IportOpening:{ex.Message}");
            //}

        }
        private void InitialDT()
        {
            try
            {
                _dttxt = new DataTable("RAW");
                _dttxt.Columns.Add("MaterialNumber", typeof(string));
                _dttxt.Columns.Add("Plant", typeof(string));
                _dttxt.Columns.Add("weightkg", typeof(string));
                _dttxt.Columns.Add("BaseUnit", typeof(string));
                _dttxt.Columns.Add("RollReamQty", typeof(string));
                _dttxt.Columns.Add("StockDate", typeof(string));

                _dtconv = new DataTable("CONV");
                _dtconv.Columns.Add("MaterialID", typeof(int));
                _dtconv.Columns.Add("WarehouseID", typeof(int));
                _dtconv.Columns.Add("weightkg", typeof(decimal));
                _dtconv.Columns.Add("UnitID", typeof(int));
                _dtconv.Columns.Add("RollReamQty", typeof(decimal));
                _dtconv.Columns.Add("StockDate", typeof(DateTime));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on InitialDT:{ex.Message}");
            }

        }
        private void FillDTContent()
        {
            try
            {
                string strSql;
                string[] txt;
                string matCode = string.Empty, wareHouse = string.Empty, unit = string.Empty, stockDate = string.Empty;
                decimal weightkg = 0.0M, Qty = 0.0M;
                DataRow dr;
                clsMaster cm = new clsMaster(ConStr);
                cm.Logger = clsLog.Logger.Inventory;
                cm.LogProcess = clsLog.ProcessCategory.Inventory;
                clsConversion conv = new clsConversion();

                strSql = string.Empty;
                for (int i = 0; i <= _textOPContent.GetUpperBound(0); i++)
                {
                    txt = _textOPContent[i].Split("||", StringSplitOptions.RemoveEmptyEntries);
                    if (txt.Length != 6)
                    {
                        throw new Exception("Content of line " + i + " should have 6 parts after split(exculde empty head and tail).");
                    }
                    for (int j = 0; j <= txt.GetUpperBound(0); j++)
                    {
                        //ignore j=0 and j=txt.GetUpperBound(0) .. begin and end is nothing to use.
                        switch (j)
                        {
                            case 0:
                                matCode = txt[j];
                                break;
                            case 1:
                                wareHouse = txt[j];
                                break;
                            case 2:
                                weightkg = Convert.ToDecimal(txt[j]);
                                break;
                            case 3:
                                unit = txt[j];
                                break;
                            case 4:
                                Qty = Convert.ToDecimal(txt[j]);
                                break;
                            case 5:
                                stockDate = txt[j];
                                break;
                            default:
                                throw new Exception("text format is wrong");
                        }

                    }
                    dr = _dttxt.NewRow();
                    dr["MaterialNumber"] = matCode;
                    dr["Plant"] = wareHouse;
                    dr["weightkg"] = weightkg;
                    dr["BaseUnit"] = unit;
                    dr["RollReamQty"] = Qty;
                    dr["StockDate"] = stockDate;
                    _dttxt.Rows.Add(dr);

                    dr = _dtconv.NewRow();
                    dr["MaterialID"] = cm.GetMaterialId(matCode, true);
                    dr["WarehouseID"] = cm.GetWarehouseID(wareHouse, true);
                    dr["weightkg"] = weightkg;
                    dr["UnitID"] = cm.GetUnitID(unit);
                    dr["RollReamQty"] = Qty;
                    dr["StockDate"] = conv.GetDateFromSAPDate(stockDate);
                    _dtconv.Rows.Add(dr);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on FillDTContent:{ex.Message}");
            }
        }
        /// <summary>
        /// Check text file content to each field and row level if it correct in valid range to import.
        /// </summary>
        /// <returns></returns>
        private Boolean IsValidDataContent(ref string Output)
        {
            //Check 1 date in file
            DateTime BaseDate = DateTime.Parse("2019/1/1");
            DateTime workDate = BaseDate;
            DateTime oldDate = BaseDate;
            clsMaster cm = new clsMaster(_conStr);
            _currentInvDate = cm.GetOpeningInventoryDate();
            foreach (DataRow dr in _dtconv.Rows)
            {
                workDate = Convert.ToDateTime(dr["StockDate"]);
                if (oldDate == BaseDate)
                {
                    oldDate = workDate;
                    continue;
                }
                else
                {
                    if (oldDate != workDate)
                    {
                        Output = $"StockDate has more than 1 value({oldDate},{workDate}";
                        return false;
                    }
                }
            }
            // Check value of date.. must be 1. <=Today 2. >=current opening inventory date
            _txtInvDate = workDate;
            if (_txtInvDate > DateTime.Today)
            {
                Output = $"Inventory Stock Date is in future which is Impossible. Please check Text file";
                return false;
            }
            if (_txtInvDate < _currentInvDate)
            {
                Output = $"Inventory Stock Date is older than current opening Inventory date in database now ({_currentInvDate})";
                return false;
            }


            Output = "ok";
            return true;
        }
        /// <summary>
        /// Archive Current opening inventory to archive table and delete current inventory.
        /// </summary>
        private void ArchiveCurrentOpeningInventory()
        {
            try
            {
                ClsConnection con = new ClsConnection(_conStr);
                string strSql = string.Empty;
                if (_currentInvDate < _txtInvDate)
                {
                    strSql = $"Insert into DT_InventoryOpeningArchive" +
                            $" (ID,WarehouseID,MaterialID,OpeningDate,OpenQty_InvU" +
                            $",InsertDate,UpdateDate,InsertUserID,UpdateUserID)" +
                            $" Select ID,WarehouseID,MaterialID,OpeningDate,OpenQty_InvU" +
                            $",GetDate(),GetDate(),InsertUserID,UpdateUserID" +
                            $" From DT_InventoryOpening;";
                }

                strSql += $"Delete From DT_InventoryOpening;";
                con.ExecuteNonQuery(strSql);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on ArchiveCurrentOpeningInventory:{ex.Message}");
            }
        }
        private void ImportOpeningInvFromDT()
        {
            try
            {
                string strSql = string.Empty;
                int newID;
                DataRow[] dr;
                ClsConnection con = new ClsConnection(_conStr);
                dr = _dtconv.Select($"MaterialID<>0 and WarehouseID<>0");
                for (int i = 0; i < dr.GetUpperBound(0); i++)
                {
                    newID = con.GetNextID("DT_InventoryOpening");
                    strSql = $"Insert into DT_InventoryOpening" +
                            $"(ID,WarehouseID,MaterialID,OpeningDate,OpenQty_InvU" +
                            $",InsertDate,UpdateDate,InsertUserID,UpdateUserID)" +
                            $" Values({newID},{con.GetSqlFormat(dr[i]["WarehouseID"])}" +
                            $",{con.GetSqlFormat(dr[i]["MaterialID"])},{con.GetSqlFormat(dr[i]["StockDate"])}" +
                            $",{con.GetSqlFormat(dr[i]["RollReamQty"])}" +
                            $",Getdate(),Getdate(),0,0);";
                    con.ExecuteNonQuery(strSql);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on ImportOpeningInvFromDT:{ex.Message}");
            }
        }
        private Boolean StartImportOpeningProcess(ref string Output)
        {
            //suppose to have 8 string after spllit double pipe but first and last won't be used
            /*Example of text file
             BOF
            ||MaterialNumber||Plant||Quantity||BaseUnit||RollReamQty||StockDate||
            ||Z02DW450X10920127P||7581||15000.000||KG||5.000||20190625||
            ||Z02DW450X10920127P||7583||10000.000||KG||1.000||20190625||
            EOF
              
             */

            string msg = string.Empty;
            clsLog log = new clsLog(_conStr);
            _LogprocessID = log.LogProcessInsert(clsLog.Logger.Inventory, clsLog.ProcessCategory.Inventory,$"Import Inventory from SAP", DateTime.Now);
            InitialDT();
            FillDTContent();
            if (IsValidDataContent(ref msg) == false)
            {
                Output = $"Invalid DataContent:{msg}";
                return false;
            }
            ArchiveCurrentOpeningInventory();
            ImportOpeningInvFromDT();
            log.LogProcessUpdate(_LogprocessID, DateTime.Now);

            Output = "ok";
            return true;
        }


        internal Boolean IsValidOpeningInventoryFile(ref string Output)
        {
            //try
            //{
            if (_textOPFile.Exists == false)
            {
                Output = "Text file not found";
                return false;
            }

            string tmpStr = string.Empty;
            int lineCount = 0;
            int lineContent = 0;


            using (StreamReader reader = File.OpenText(_textOPFile.FullName))
            {
                while (reader.Peek() >= 0)
                {
                    lineCount += 1;
                    tmpStr = reader.ReadLine();
                    if (lineCount == 1 && tmpStr != "BOF")
                    {
                        Output = "BOF word not found at beginning";
                        return false;
                    }
                    //Line2 is column name. So not use
                    if (lineCount > 2)
                    {
                        if (tmpStr != "EOF")
                        {
                            lineContent += 1;
                            Array.Resize(ref _textOPContent, lineContent);
                            _textOPContent[lineContent - 1] = tmpStr;
                        }

                    }
                }
                //At the last point of reading text, last word shold be EOF always
                if (tmpStr != "EOF")
                {
                    Output = "EOF is not found at the end of file";
                    return false;
                }

                if (_textOPContent.Length == 0)
                {
                    Output = "No content found in text file";
                    return false;
                }
                return true;
            }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"Error on IsValidOpeningInventoryFile:{ex.Message}");
            //}




        }
    }
}

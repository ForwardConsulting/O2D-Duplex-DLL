using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace Duplex.Model
{
    public class clsDTOrder
    {
        private int _id;
        private string _ordernumber;
        private int _orderHierarchyID;
        private int _billToID;
        private string _poNumber;
        private int? _shipToID;
        private int? _contractID;
        private string _remark;
        private string _propertyCaption;
        private string _propertyValue;
        private DateTime _insertDate;
        private DateTime _updateDate;
        private int _userID;
        private DataTable _dt;


        public int Id { get => _id; set => _id = value; }

        public string Ordernumber { get => _ordernumber; set => _ordernumber = value; }
        public int OrderHierarchyID { get => _orderHierarchyID; set => _orderHierarchyID = value; }
        public int BillToID { get => _billToID; set => _billToID = value; }
        public string PoNumber { get => _poNumber; set => _poNumber = value; }
        public int? ShipToID { get => _shipToID; set => _shipToID = value; }
        public int? ContractID { get => _contractID; set => _contractID = value; }
        public string Remark { get => _remark; set => _remark = value; }
        public string PropertyCaption { get => _propertyCaption; set => _propertyCaption = value; }
        public string PropertyValue { get => _propertyValue; set => _propertyValue = value; }
        public DateTime InsertDate { get => _insertDate; set => _insertDate = value; }
        public DateTime UpdateDate { get => _updateDate; set => _updateDate = value; }
        public int UserID { get => _userID; set => _userID = value; }
        public DataTable DT { get => _dt; }

        public void AddRow()
        {
            if (DT == null) 
            {
                CreateSchema();
            }
            InsertData();
        }
        void InsertData()
        {
            string filterStr;
            filterStr = "ID=" + _id.ToString ();
            DataRow[] drs;
            drs = _dt.Select(filterStr);
            if (drs.Length >0)
            {
                throw new Exception("ID exist found");
            }
            DataRow dr;
            dr = _dt.NewRow();
            dr["ID"] = _id ;dr["Ordernumber"] = _ordernumber ;dr["OrderHierarchyID"] =_orderHierarchyID ;
            dr["BillToID"] =_billToID ; dr["PoNumber"] =_poNumber ; dr["ShipToID"] =_shipToID ;
            dr["ContractID"] =_contractID ; dr["Remark"] =_remark ; dr["PropertyCaption"] =_propertyCaption ;
            dr["PropertyValue"] =_propertyValue ; dr["InsertDate"] =DateTime.Now; dr["UpdateDate"] = DateTime.Now;
            dr["InsertUserID"] = _userID; dr["UpdateUserID"] = _userID;


            _dt.Rows.Add(dr);
        }
        void CreateSchema()
        {
            _dt = new DataTable();
            DataColumn[] dc = new DataColumn[]
            {
                new DataColumn("ID",typeof(int)),
                new DataColumn("Ordernumber",typeof(string)),
                new DataColumn("OrderHierarchyID",typeof(int)),
                new DataColumn("BillToID",typeof(int)),
                new DataColumn("PoNumber",typeof(string)),
                new DataColumn("ShipToID",typeof(int)),
                new DataColumn("ContractID",typeof(int)),
                new DataColumn("Remark",typeof(string)),
                new DataColumn("PropertyCaption",typeof(string)),
                new DataColumn("PropertyValue",typeof(string)),
                new DataColumn("InsertDate",typeof(DateTime)),
                new DataColumn("UpdateDate",typeof(DateTime)),
                new DataColumn("InsertUserID",typeof(string)),
                new DataColumn("UpdateUserID",typeof(string))
            };

            _dt.Columns.AddRange(dc);
        }


    }
}

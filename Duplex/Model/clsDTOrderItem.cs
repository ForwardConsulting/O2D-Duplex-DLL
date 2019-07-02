using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Duplex.Model
{
    public class clsDTOrderItem
    {
        private int _id;
        private int _orderID;
        private int _routeID;
        private int _bomID;
        private int _itemNumber;
        private int _materialID;
        private int _shiptoID;
        private int _contractID;
        private decimal _requestQTY_OrdU;
        private decimal _confirmQTY_OrdU;
        private DateTime _requestDate;
        private DateTime _confirmDate;
        private int _statusID;
        private string _remark;
        private DateTime _insertDate;
        private DateTime _updateDate;
        private int _userID;
        private DataTable _dt;

        public int Id { get => _id; set => _id = value; }
        public int OrderID { get => _orderID; set => _orderID = value; }
        public int RouteID { get => _routeID; set => _routeID = value; }
        public int BomID { get => _bomID; set => _bomID = value; }
        public int ItemNumber { get => _itemNumber; set => _itemNumber = value; }
        public int MaterialID { get => _materialID; set => _materialID = value; }
        public int ShiptoID { get => _shiptoID; set => _shiptoID = value; }
        public int ContractID { get => _contractID; set => _contractID = value; }
        public decimal RequestQTY_OrdU { get => _requestQTY_OrdU; set => _requestQTY_OrdU = value; }
        public decimal ConfirmQTY_OrdU { get => _confirmQTY_OrdU; set => _confirmQTY_OrdU = value; }
        public DateTime RequestDate { get => _requestDate; set => _requestDate = value; }
        public DateTime ConfirmDate { get => _confirmDate; set => _confirmDate = value; }
        public int StatusID { get => _statusID; set => _statusID = value; }
        public string Remark { get => _remark; set => _remark = value; }
        public DateTime InsertDate { get => _insertDate; set => _insertDate = value; }
        public DateTime UpdateDate { get => _updateDate; set => _updateDate = value; }
        public int InsertUserID { get => _userID; set => _userID = value; }
        public DataTable DT { get => _dt; }

        public void AddRow()
        {
            if (_dt == null)
            {
                CreateSchema();
            }
            InsertData();
        }
        void InsertData()
        {
            string filterStr;
            filterStr = "ID=" + _id.ToString();
            DataRow[] drs;
            drs = _dt.Select(filterStr);
            if (drs.Length > 0)
            {
                throw new Exception("ID exist found");
            }
            DataRow dr;
            dr = _dt.NewRow();
            dr["ID"] = _id; dr["OrderID"] = _orderID; dr["RouteID"] = _routeID;
            dr["BOMID"] = _bomID; dr["ItemNumber"] = _itemNumber; dr["MaterialID"] = _materialID;
            dr["ShipToID"] = _shiptoID; dr["ContractID"] = _contractID; dr["RequestQTY_OrdU"] = _requestQTY_OrdU;
            dr["ConfirmQTY_OrdU"] = _confirmQTY_OrdU; dr["RequestDate"] = _requestDate; dr["ConfirmDate"] = _confirmDate; dr["StatusID"] = _statusID; dr["Remark"] = _remark;
            dr["InsertDate"] = DateTime.Now; dr["UpdateDate"] = DateTime.Now;
            dr["InsertUserID"] = _userID; dr["UpdateUserID"] = _userID;


            _dt.Rows.Add(dr);
        }
        void CreateSchema()
        {
            _dt = new DataTable();
            DataColumn[] dc = new DataColumn[]
            {
                new DataColumn("ID",typeof(int)),
                new DataColumn("OrderID",typeof(int)),
                new DataColumn("RouteID",typeof(int)),
                new DataColumn("BOMID",typeof(int)),
                new DataColumn("ItemNumber",typeof(int)),
                new DataColumn("MaterialID",typeof(int)),
                new DataColumn("ShipToID",typeof(int)),
                new DataColumn("ContractID",typeof(int)),
                new DataColumn("RequestQTY_OrdU",typeof(decimal)),
                new DataColumn("ConfirmQTY_OrdU",typeof(decimal)),
                new DataColumn("RequestDate",typeof(DateTime)),
                new DataColumn("ConfirmDate",typeof(DateTime)),
                new DataColumn("StatusID",typeof(int)),
                new DataColumn("Remark",typeof(string)),
                new DataColumn("InsertDate",typeof(DateTime)),
                new DataColumn("UpdateDate",typeof(DateTime)),
                new DataColumn("InsertUserID",typeof(string)),
                new DataColumn("UpdateUserID",typeof(string))
            };

            _dt.Columns.AddRange(dc);
        }


    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Duplex.Model
{
    public class clsDTSplitItem
    {
        private int _orderItemId;
        private int? _itemNo;
        private decimal _requestReamQty;
        private DateTime? _requestDate;
        private DataTable _dt;
        /*_requestDate is Nullable type but user must not input null, just skip it. If caller try to input Null, the error will show up because dataset is not allow null.
         */

        public int OrderItemId { get => _orderItemId; set => _orderItemId = value; }
        public int? ItemNo { get => _itemNo; set => _itemNo = value; }
        public decimal RequestReamQty { get => _requestReamQty; set => _requestReamQty = value; }
        public DateTime? RequestDate { get => _requestDate; set => _requestDate = value; }
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

            DataRow dr;
            dr = _dt.NewRow();
            dr["OrderItemID"] = _orderItemId; dr["ItemNo"] = _itemNo; dr["RequestReamQty"] = _requestReamQty;
            if (_requestDate != null)
            {
                dr["RequestDate"] = _requestDate;
            }


            _dt.Rows.Add(dr);
        }

        void CreateSchema()
        {
            _dt = new DataTable();
            DataColumn[] dc = new DataColumn[]
            {
                new DataColumn("OrderItemID",typeof(int)),
                new DataColumn("ItemNo",typeof(int)),
                new DataColumn("RequestReamQty",typeof(decimal)),
                new DataColumn("RequestDate",typeof(DateTime)),
                new DataColumn("ConfirmDate",typeof(DateTime)),
            };

            _dt.Columns.AddRange(dc);
        }
    }
}

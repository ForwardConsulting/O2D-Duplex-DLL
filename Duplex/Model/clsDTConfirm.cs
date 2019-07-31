using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Duplex.Model
{
    public class clsDTConfirm
    {
        private int _solId;
        private decimal _quantity;
        private DataTable _dt;
        private int _idCount=0;
        public int SolutionId { get => _solId; set => _solId = value; }
        public decimal Quantity { get => _quantity; set => _quantity = value; }
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
            _idCount += 1;
             dr["SolutionID"] = _solId; dr["Quantity"] = _quantity;

            _dt.Rows.Add(dr);
        }
        void CreateSchema()
        {
            _dt = new DataTable();
            DataColumn[] dc = new DataColumn[]
            {
                //new DataColumn ("ID",typeof(int)),
                new DataColumn("SolutionID",typeof(int)),
                new DataColumn("Quantity",typeof(decimal)),
            };

            _dt.Columns.AddRange(dc);
        }
    }
}

using Duplex;
using System;
using System.Data;
using Duplex.Model;
namespace TestO2DDLL
{
    class Program
    {

        static DataSet ds;
        static ATPCTP d;
        /*Prepare parameter
         */
        static DataTable DTOrder = new DataTable();
        static DataTable DTOrderItem = new DataTable();
        static void Main(string[] args)
        {
            String ConStr;


            Cls_Connection t = new Cls_Connection();
            ConStr = t.GetConnectionString();
            
            d = new ATPCTP(ConStr);
            clsDTOrder dtOrder = new clsDTOrder { Id = -1, Ordernumber = "Order-xxx", OrderHierarchyID = 2, BillToID = 111,PoNumber ="PO-xxx",ShipToID =222,ContractID =123,Remark ="Test Remark" ,UserID =9};
            dtOrder.AddRow();

            clsDTOrderItem dtOrderItem = new clsDTOrderItem { Id = -1, OrderID = -1, ItemNumber = 10, MaterialID = 1000000034, ShiptoID = 111, ContractID = 222, RequestQTY_OrdU = 20, RequestDate = Convert.ToDateTime("2019/10/10") };
            dtOrderItem.AddRow();

            //ds = d.Request(1, 10, new DateTime (2019,6/7), 1, 0);
            try
            {
               // bool result;
                //PrepareParam();
                //ds = d.Request(DTOrder, DTOrderItem, 1000000002);
                ds = d.Request(dtOrder, dtOrderItem, 1000000002);


                //confirm case
                //result = d.Confirm("86", 9);

                Console.WriteLine("done for " + ConStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error found:" + ex.Message );
            }


           
        }
        static void PrepareParam()
        {
            DTOrder.Columns.Add("ID", typeof(int));
            DTOrder.Columns.Add("OrderNumber", typeof(string));
            DTOrder.Columns.Add("OrderHierarchyID", typeof(int));
            DTOrder.Columns.Add("BillToID", typeof(int));
            DTOrder.Columns.Add("PONumber", typeof(string));
            DTOrder.Columns.Add("ShipToID", typeof(int));
            DTOrder.Columns.Add("ContractID", typeof(int));
            DTOrder.Columns.Add("Remark", typeof(string));
            DTOrder.Columns.Add("PropertyCaption", typeof(string));
            DTOrder.Columns.Add("PropertyValue", typeof(string));
            DTOrder.Columns.Add("InsertDate", typeof(DateTime));
            DTOrder.Columns.Add("UpdateDate", typeof(DateTime));
            DTOrder.Columns.Add("InsertUserID", typeof(int));
            DTOrder.Columns.Add("UpdateUserID", typeof(int));

            DTOrderItem.Columns.Add("ID", typeof(int));
            DTOrderItem.Columns.Add("OrderID", typeof(int));
            DTOrderItem.Columns.Add("RouteID", typeof(int));
            DTOrderItem.Columns.Add("BOMID", typeof(int));
            DTOrderItem.Columns.Add("ItemNumber", typeof(int));
            DTOrderItem.Columns.Add("MaterialID", typeof(int));
            DTOrderItem.Columns.Add("ShipToID", typeof(int));
            DTOrderItem.Columns.Add("ContractID", typeof(int));
            DTOrderItem.Columns.Add("RequestQTY_OrdU", typeof(int));
            DTOrderItem.Columns.Add("ConfirmQTY_OrdU", typeof(int));
            DTOrderItem.Columns.Add("RequestDate", typeof(DateTime));
            DTOrderItem.Columns.Add("ConfirmDate", typeof(DateTime));
            DTOrderItem.Columns.Add("StatusID", typeof(int));
            DTOrderItem.Columns.Add("Remark", typeof(string));
            DTOrderItem.Columns.Add("InsertDate", typeof(DateTime));
            DTOrderItem.Columns.Add("UpdateDate", typeof(DateTime));
            DTOrderItem.Columns.Add("InsertUserID", typeof(int));
            DTOrderItem.Columns.Add("UpdateUserID", typeof(int));

            DTOrder.Rows.Add(new object[] { -1, "Order-xx", 2, 111, "PO-xxx", 222, 123, "Test remark", "Caption1|Caption2", "Value1|Value2", DateTime.Today, DateTime.Today, 9, 9 });
            DTOrderItem.Rows.Add(new object[] { -1, -1, null, null, 10, 1000000034, 111, null, 22, null, "2019/6/11", null, 1, "remark", DateTime.Today, DateTime.Today, 9, 9 });
        }


    }
}

using Duplex;
using System;
using System.Data;
using Duplex.Model;
using Duplex.Tool;

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
            //testSFTP();
            //Main_TestPLSRelease();
            Main_TestATPCTPRequest();

        }
        static void testSFTP()
        {
            clsSFTP sftp = new clsSFTP();
            sftp.sftpIP = "ftptest.scg.com";
            sftp.Sftpuser = "skico2dtest02";
            sftp.Sftppwd = "kr0EtvFC";

            sftp.DownloadLatestFile();
        }
        static void Main_TestATPCTPRequest()
        {
            String ConStr;


            Cls_Connection t = new Cls_Connection();
            ConStr = t.GetConnectionString();
            DateTime OrderreqDate = DateTime.Today.AddDays(1d);

            d = new ATPCTP(ConStr);


            clsDTOrder dtOrder = new clsDTOrder { Id = -1, Ordernumber = "Order-xxx", OrderHierarchyID = 2, BillToID = 1000036, PoNumber ="PO-xxx",ShipToID =222,ContractID =123,Remark ="Test Remark" ,UserID =9};
            dtOrder.AddRow();

            clsDTOrderItem dtOrderItem = new clsDTOrderItem { Id = -1, OrderID = -1, ItemNumber = 10, MaterialID = 1000000034, ShiptoID = 111, ContractID = 222, RequestQTY_OrdU = 20, RequestDate = OrderreqDate };
            dtOrderItem.AddRow();

            //ds = d.Request(1, 10, new DateTime (2019,6/7), 1, 0);
            try
            {
                ds = d.Request(dtOrder, dtOrderItem, 1000000002);

                Console.WriteLine("done for " + ConStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error found:" + ex.Message );
            }


           
        }

        static void Main_TestATPCTPConfirm()
        {
            String ConStr;

            Cls_Connection t = new Cls_Connection();
            ConStr = t.GetConnectionString();

            d = new ATPCTP(ConStr);
            clsDTConfirm DTConfirm = new clsDTConfirm { SolutionId = 198,Quantity=25 };
            DTConfirm.AddRow();

            d.Confirm(DTConfirm,UserID:99);

        }

        static void Main_TestPLSRelease()
        {
            String ConStr;


            Cls_Connection t = new Cls_Connection();
            ConStr = t.GetConnectionString();
            PLSInterface d = new PLSInterface(ConStr);

            d.OperationID = 1000000022;d.AddRow();
            d.OperationID = 1000000023; d.AddRow();
            d.ExportData();
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

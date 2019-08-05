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
            Main_TestPLSRelease();
            //Main_TestATPCTPRequest();
            //Main_TestATPCTPRequest2();
            //Main_TestATPCTPConfirm();
            t1();
        }

        static void t1()
        {
            clsDTOrder xx = new clsDTOrder();

        }

        static void testSFTP()
        {
            clsSFTP sftp = new clsSFTP();
            sftp.sftpIP = "ftptest.scg.com";
            sftp.Sftpuser = "skico2dtest02";
            sftp.Sftppwd = "kr0EtvFC";

            sftp.DownloadLatestFile();
        }

        static void Main_TestATPCTPRequest2()
        {
            String ConStrCustom;
            String ConstrStd;
            int UserID = 0;

            Cls_Connection t = new Cls_Connection();
            ConStrCustom = t.GetConnectionString_Custom();
            ConstrStd = t.GetConnectionString_Std();
            DateTime OrderreqDate = DateTime.Today.AddDays(1d);

            d = new ATPCTP(ConStrCustom, ConstrStd);


            //clsDTOrder dtOrder = new clsDTOrder { Id = 1000000070, Ordernumber = "OD-190727-0004", OrderHierarchyID = 1000000065, BillToID = 5, PoNumber = "PO-xxx", ShipToID = 222, ContractID = 123, Remark = "Test Remark", UserID = 99 };
            //dtOrder.AddRow();

            //clsDTOrderItem dtOrderItem = new clsDTOrderItem { Id = -1, OrderID = 1000000070, ItemNumber = 10, MaterialID = 1000000034, ShiptoID = 111, ContractID = 222, RequestQTY_OrdU = 20, RequestDate = OrderreqDate };
            //dtOrderItem.AddRow();

            //ds = d.Request(1, 10, new DateTime (2019,6/7), 1, 0);

            //clsDTOrder dtOrder = new clsDTOrder
            //{
            //    Id = 1000000070,
            //    Ordernumber = "OD-190727-0004",
            //    OrderHierarchyID = 1000000065,
            //    BillToID = 5,
            //    PoNumber = "po-ppppp",
            //    ShipToID = 0,
            //    ContractID = 0,
            //    Remark = "ssss",
            //    UserID = UserID
            //};
            //dtOrder.AddRow();

            //clsDTOrderItem dtOrderItem = new clsDTOrderItem
            //{
            //    Id = -1,
            //    OrderID = 1000000070,
            //    ItemNumber = 10,
            //    MaterialID = 1000000034,
            //    ShiptoID = 111,
            //    ContractID = 222,
            //    RequestQTY_OrdU = 20,
            //    RequestDate = OrderreqDate,
            //    InsertUserID = UserID

            //};
            //dtOrderItem.AddRow();

            clsDTOrder dtOrder = new clsDTOrder
            {
                Id = 1000000147,
                Ordernumber = "OD-190731-0014",
                OrderHierarchyID = 1000000065,
                BillToID = 3,
                PoNumber = "po-ssss",
                ShipToID = 0,
                ContractID = 0,
                Remark = "Test Remark",
                UserID = UserID
            };
            dtOrder.AddRow();

            clsDTOrderItem dtOrderItem = new clsDTOrderItem
            {
                Id = -1,
                OrderID = 1000000128,
                ItemNumber = 0,
                MaterialID = 1000000034,
                ShiptoID = 0,
                ContractID = 0,
                RequestQTY_OrdU = 10,
                RequestDate = new DateTime(2019, 8, 9),
                InsertUserID = UserID
            };
            dtOrderItem.AddRow();


            try
            {
                string ErrMsg = string.Empty;
                ds = d.Request(dtOrder, dtOrderItem, 1000000002, ref ErrMsg);
                if (ErrMsg.Length > 0)
                {
                    Console.WriteLine($"Warning:{ErrMsg}");
                }
                else
                {
                    Console.WriteLine("done for " + ConStrCustom);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error found:" + ex.Message);
            }



        }

        static void Main_TestATPCTPRequest()
        {
            String ConStrCustom;
            String ConstrStd;

            Cls_Connection t = new Cls_Connection();
            ConStrCustom = t.GetConnectionString_Custom();
            ConstrStd = t.GetConnectionString_Std();
            DateTime OrderreqDate = DateTime.Today.AddDays(1d);

            d = new ATPCTP(ConStrCustom, ConstrStd);


            clsDTOrder dtOrder = new clsDTOrder { Id = -1, Ordernumber = "Order-xxx", OrderHierarchyID = 2, BillToID = 1000036, PoNumber = "PO-xxx", ShipToID = 222, ContractID = 123, Remark = "Test Remark", UserID = 9 };
            dtOrder.AddRow();

            clsDTOrderItem dtOrderItem = new clsDTOrderItem { Id = -1, OrderID = -1, ItemNumber = 10, MaterialID = 1000000034, ShiptoID = 111, ContractID = 222, RequestQTY_OrdU = 20, RequestDate = OrderreqDate };
            dtOrderItem.AddRow();

            //ds = d.Request(1, 10, new DateTime (2019,6/7), 1, 0);
            try
            {
                string ErrMsg = String.Empty;
                ds = d.Request(dtOrder, dtOrderItem, 1000000002, ref ErrMsg);

                Console.WriteLine("done for " + ConStrCustom);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error found:" + ex.Message);
            }



        }

        static void Main_TestATPCTPConfirm()
        {
            String ConStrCustom;
            String ConstrStd;
            string WarningMsg=string.Empty;

            Cls_Connection t = new Cls_Connection();
            ConStrCustom = t.GetConnectionString_Custom();
            ConstrStd = t.GetConnectionString_Std();

            d = new ATPCTP(ConStrCustom, ConstrStd);
            clsDTConfirm DTConfirm = new clsDTConfirm { SolutionId = 2480, Quantity = 11 };
            DTConfirm.AddRow();

            if (d.Confirm(DTConfirm, UserID: 99, ref WarningMsg)==true)
            {
                Console.WriteLine($"Confirm process is success");
            }
            else
            {
                Console.WriteLine($"WarningMsg:{WarningMsg}");
            }
            

        }

        static void Main_TestPLSRelease()
        {
            String ConStrCustom;
            String ConstrStd;
            string WarningMsg = string.Empty;


            Cls_Connection t = new Cls_Connection();
            ConStrCustom = t.GetConnectionString_Custom();
            ConstrStd = t.GetConnectionString_Std();
            PLSInterface d = new PLSInterface(ConStrCustom,ConstrStd);

            d.OperationID = 1000000048; d.AddRow();
            d.OperationID = 1000000049; d.AddRow();
            if (d.ExportData(ref WarningMsg) == false)
            {
                Console.WriteLine($"Export problem with reason:{WarningMsg}");
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

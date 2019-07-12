using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Duplex
{
    /// <summary>
    /// Task#4425:Caller should submit id of DT_OrderItemOperation which user tick and press button expect to send those to PLS system. At here, we will write data direct into PLS system only.
    /// </summary>
    class PLSInterface
    {
        private DataTable _DTOrderItemOpr;
        private string _conStr;
        public PLSInterface(string ConnectionString)
        {
            _conStr = ConnectionString;
        }


        public DataTable DTOrderItemOpr { get => _DTOrderItemOpr; set => _DTOrderItemOpr = value; }
        public string ConStr { get => _conStr; set => _conStr = value; }

        public void ExportData(DataTable DTOrderItemOperation)
        {
            string procName = "proc_O2D_PLSInterface_4475";
            try
            {
                DTOrderItemOpr = DTOrderItemOperation;
                using (SqlConnection con = new SqlConnection(ConStr))
                {
                    con.Open();
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        cmd.CommandTimeout = 30;
                        cmd.Parameters.AddWithValue("@DTRefID", DTOrderItemOpr);
                        cmd.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error on ExportData:{ex.Message }");
            }
        }

    }
}

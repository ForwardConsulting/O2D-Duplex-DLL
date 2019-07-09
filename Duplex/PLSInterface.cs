using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Duplex
{
    /// <summary>
    /// Task#4425:Caller should submit id of DT_OrderItemOperation which user tick and press button expect to send those to PLS system. At here, we will write data direct into PLS system only.
    /// </summary>
    class PLSInterface
    {
        private DataTable _DTOrderItemOpr;

        public PLSInterface(DataTable DTOrderItemOperation)
        {
            DTOrderItemOpr = DTOrderItemOperation;
        }

        public DataTable DTOrderItemOpr { get => _DTOrderItemOpr; set => _DTOrderItemOpr = value; }

        public void ExportData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                throw new Exception($"Error on ExportData:{ex.Message }");
            }
        }

    }
}

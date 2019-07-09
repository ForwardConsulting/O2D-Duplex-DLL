using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Duplex
{
    class PLSInterface
    {
        private DataTable _DTOrderItemOpr;

        public PLSInterface(DataTable DTOrderItemOperation )
        {
            DTOrderItemOpr = DTOrderItemOperation;
        }

        public DataTable DTOrderItemOpr{ get => _DTOrderItemOpr; set => _DTOrderItemOpr = value; }

        
    }
}

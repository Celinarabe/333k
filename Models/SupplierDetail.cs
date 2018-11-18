using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Rabe_Celina_HW6.Models
{
    public class SupplierDetail
    {
        //navigational properties
        public Int32 SupplierDetailID { get; set; }

        public Supplier Supplier { get; set; }
        public Product Product { get; set; }

    }
}
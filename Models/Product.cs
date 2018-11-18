using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Rabe_Celina_HW6.Models
{
    public class Product
    {
        public Int32 ProductID { get; set; }


        //RANGE STARTS AT 5001
        public Int32 SKU { get; set; }

        [Required(ErrorMessage = "Product name is required!")]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

 
        [Display(Name = "Product Price")]
        [DisplayFormat(DataFormatString = "${0:N}")]
        public Decimal Price { get; set; }

        public string Description { get; set; }
        [Display(Name = "Product Description")]

        public List <OrderDetail> OrderDetails { get; set; }
        public List<SupplierDetail> SupplierDetails { get; set; }

		public Product()
        {
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierDetail>();
            }

            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
namespace Rabe_Celina_HW6.Models
{
    public class OrderDetail
    {
        public Int32 OrderDetailID { get; set; }

        [Display(Name = "Quantity Ordered")]
        [Range(1,1000, ErrorMessage = "Quantity must be greater than zero")]
        public Int32 QuantityOrdered { get; set; }

        [Display(Name = "Product Price")]
        [DisplayFormat(DataFormatString = "${0:N}")]
        public Decimal ProductPrice { get; set; }

        [Display(Name = "Extended Price")]
        [DisplayFormat(DataFormatString = "${0:N}")]
        public Decimal ExtendedPrice { get; set; }


        //navigational properties
        public Order Order {get; set;}
        public Product Product { get; set; }

    }
}


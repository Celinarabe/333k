using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Rabe_Celina_HW6.Models
{
    public class Order
    {
        private const Decimal SALES_TAX = 0.0825m;

        public Int32 OrderID { get; set; }

        //RANGE STARTS AT 10001
        public Int32 OrderNumber { get; set; }

        //ORDER DATE IS TODAY
        [Display(Name = "Order Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime OrderDate { get; set; }


        [Display(Name = "Order Notes")]
        public string Notes { get; set; }

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "${0:N}")]
        public Decimal OrderSubtotal
        {
           get { return OrderDetails.Sum(rd => rd.ExtendedPrice); }
        }

        [Display(Name = "Sales Tax")]
        [DisplayFormat(DataFormatString = "${0:N}")]
        public Decimal SalesTax
        {
            get { return OrderSubtotal * SALES_TAX; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "${0:N}")]
        public Decimal OrderTotal
        {
            get { return OrderSubtotal + SalesTax; }
        }


        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }

    }
}

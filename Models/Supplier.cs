using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Rabe_Celina_HW6.Models
{
    public class Supplier
    {
        public Int32 SupplierID { get; set; }

        [Required(ErrorMessage = "Please include the supplier's name")]
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Please enter the supplier's email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the supplier's phone number")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please enter the established date")]
        [Display(Name = "Established Date:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0: MM.dd.yyyy}", ApplyFormatInEditMode = true)]
        public string Date { get; set; }

        public bool Preferred { get; set; }

        public string Notes { get; set; }

        public List<SupplierDetail> SupplierDetails { get; set; }


        public Supplier()
        {
            if (SupplierDetails == null)
            {
                SupplierDetails = new List<SupplierDetail>();
            }
        }
    }
}


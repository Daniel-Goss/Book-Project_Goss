using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Book_Project_Goss.Models
{
    public class InvoiceUpsertModel
    {
        public Invoice Invoice { get; set; }
        public List<Customer> CustomerList { get; set; }
    }
}
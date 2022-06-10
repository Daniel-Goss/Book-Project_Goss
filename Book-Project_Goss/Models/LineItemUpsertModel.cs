using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Book_Project_Goss.Models
{
    public class LineItemUpsertModel
    {
        public List<Product> ProductList { get; set; }
        public List<Invoice> InvoiceList { get; set; }
        public InvoiceLineItem LineItem { get; set; }
    }
}
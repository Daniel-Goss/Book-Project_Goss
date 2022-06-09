using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Book_Project_Goss.Models
{
    public class CustomerUpsertModel
    {

        public List<State> StateList { get; set; }
        public Customer Customer { get; set; }
    }
}
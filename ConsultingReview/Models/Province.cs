using System;
using System.Collections.Generic;

namespace ConsultingReview.Models
{
    public partial class Province
    {
        public Province()
        {
            Customer = new HashSet<Customer>();
        }

        public string ProvinceCode { get; set; }
        public string Name { get; set; }
        public double ProvincialTax { get; set; }

        public virtual ICollection<Customer> Customer { get; set; }
    }
}

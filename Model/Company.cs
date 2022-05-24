using System;
using System.Collections.Generic;

namespace PhoneCostApp.Model
{
    public partial class Company
    {
        public Company()
        {
            PhoneCosts = new HashSet<PhoneCost>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<PhoneCost> PhoneCosts { get; set; }
    }
}

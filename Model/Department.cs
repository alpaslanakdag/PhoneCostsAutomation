using System;
using System.Collections.Generic;

namespace PhoneCostApp.Model
{
    public partial class Department
    {
        public Department()
        {
            PhoneCosts = new HashSet<PhoneCost>();
        }

        public int Id { get; set; }
        public string? Org1 { get; set; }
        public int? ParentId { get; set; }

        public virtual ICollection<PhoneCost> PhoneCosts { get; set; }
    }
}

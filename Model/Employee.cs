using System;
using System.Collections.Generic;

namespace PhoneCostApp.Model
{
    public partial class Employee
    {
        public Employee()
        {
            PhoneCosts = new HashSet<PhoneCost>();
        }

        public int Id { get; set; }
        public string? EmployeeName { get; set; }
        public int? PhoneNumber { get; set; }

        public virtual ICollection<PhoneCost> PhoneCosts { get; set; }
    }
}

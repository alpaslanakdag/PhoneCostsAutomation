using System;
using System.Collections.Generic;

namespace PhoneCostApp.Model
{
    public partial class PhoneCost
    {
        public int Id { get; set; }
        public string? CustomerCostCenter { get; set; }
        public int? EmployeeId { get; set; }
        public int? CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public decimal? Total { get; set; }
        public decimal? MobileConnection { get; set; }
        public decimal? MobileCalls { get; set; }
        public string? Debtor { get; set; }
        public DateTime? Date { get; set; }
        public string? ReferencePeriod { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Company? Company { get; set; }
        public virtual Department? Department { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}

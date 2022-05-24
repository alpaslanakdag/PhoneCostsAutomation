using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneCostApp.Model
{
     public partial class LogEntry
    {
        public int Id { get; set; }

        public string? FileName { get; set; }  
        
        public string? Comment { get; set; } 

        public DateTime? CreatedDate { get; set; }

    }
}

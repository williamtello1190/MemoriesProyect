using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.StoredProcedure
{
    public class MemoryPersonDetail
    {
        public Int32 IdMemoryPersonDet { get; set; }
        public Int32 IdMemoryPerson { get; set; }
        public Int32 IdAttachment { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string PhysicalName { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public string IsMain { get; set; }
    }
}

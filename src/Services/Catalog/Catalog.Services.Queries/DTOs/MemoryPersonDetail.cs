using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Services.Queries.DTOs
{
    public class MemoryPersonDetailDto
    {
        public Int64 IdMemoryPersonDet { get; set; }
        public Int64 IdMemoryPerson { get; set; }
        public Int64 IdAttachment { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string PhysicalName { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public string IsMain { get; set; }
        public string Base64File { get; set; }
        public string FilseServer { get; set; }
    }
}

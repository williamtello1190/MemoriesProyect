using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Domain.StoredProcedure
{
    public class MemoryPerson
    {
        public Int32 IdMemoryPerson { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public string CodeQR { get; set; }
    }
}

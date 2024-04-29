using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Services.Queries.DTOs
{
    public class MemoryPersonByUserIdDto
    {
        public Int32 IdMemoryPerson { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string FilePath { get; set; }
        public string Base64File { get; set; }
    }
}

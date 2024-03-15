using MediatR;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Services.EventHandlers.Commands.StoredProcedure
{
    public class MemoryPersonUpdateCommand : IRequest<DataResponse>
    {
        public Int64 MemoryPersonId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
    }
}

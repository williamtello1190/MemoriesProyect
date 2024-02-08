using MediatR;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Services.EventHandlers.Commands.StoredProcedure
{
    public class MemoryPersonCreateCommand : IRequest<DataResponse>
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Description { get; set; }
        public Int32 UserId { get; set; }
        public string CodeQR { get; set; }
    }
}

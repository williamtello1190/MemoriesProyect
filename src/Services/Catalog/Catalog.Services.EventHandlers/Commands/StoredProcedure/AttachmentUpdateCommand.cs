using MediatR;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Services.EventHandlers.Commands.StoredProcedure
{
    public class AttachmentUpdateCommand : IRequest<DataResponse>
    {
        public Int64? MemoryPersonDetId { get; set; }
        public Int64 ? AttachmentId { get; set; }
        public string User { get; set; }
    }
}

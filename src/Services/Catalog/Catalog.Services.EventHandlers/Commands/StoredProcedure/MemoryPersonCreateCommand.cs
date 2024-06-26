﻿using MediatR;
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
        public string DocumentNumber { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string DeathDate { get; set; }
        public string Description { get; set; }
        public string CodeQR { get; set; }
        public string UserRegister { get; set; }
        public MemoryPersonUserCommand User { get; set; }
        public List<MemoryPersonAttachmentCommand> Attachment { get; set; }
    }

    public class MemoryPersonUserCommand
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; } 
    }

    public class MemoryPersonAttachmentCommand : IRequest<DataResponse>
    {
        public Int64 MemoryPersonId { get; set; } 
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string PhysicalName { get; set; }
        public string Extension { get; set; }
        public string Description { get; set; }
        public string IsMain { get; set; }
        public string FileServer { get; set; }
        public string FileBase64 { get; set; }
        public string Option { get; set; }
        public string UserRegister { get; set; }

    }
}

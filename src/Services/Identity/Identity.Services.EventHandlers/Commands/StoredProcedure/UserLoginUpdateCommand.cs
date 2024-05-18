using MediatR;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.EventHandlers.Commands.StoredProcedure
{
    public class UserLoginUpdateCommand : IRequest<DataResponse>
    {
        public Int32 UserId { get; set; }
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
    }
}

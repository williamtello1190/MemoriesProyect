using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.Queries.DTOs.RequestFilter
{
    public class RequestGetUser : INotification
    {
        public RequestGetUser() { }
        public string code { get; set; }
        public string password { get; set; }

    }
}

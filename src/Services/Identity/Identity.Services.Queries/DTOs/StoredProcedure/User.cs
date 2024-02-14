using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.Queries.DTOs.StoredProcedure
{
    public class UserDto
    {
        public int UserId { get; set; }
        public int RolId { get; set; }
        public string Code { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string DocumentNumber { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string Status { get; set; }
    }
}

using Identity.Persistence.Database;
using Identity.Services.Queries.DTOs.StoredProcedure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Service.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Services.Queries.StoredProcedure
{
    public interface IGetUserQueryService
    {
        Task<UserDto> GetUser(string code, string password);
    }
    public class GetUserQueryService : IGetUserQueryService
    {
        private readonly ApplicationDbContext _context;

        public GetUserQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> GetUser(string code, string password)
        {
            SqlParameter pCode = new() { ParameterName = "@code", SqlDbType = SqlDbType.VarChar, Value = code };
            SqlParameter pPassword = new() { ParameterName = "@Password", SqlDbType = SqlDbType.VarChar, Value = password };
            var collection = await _context.Users.FromSqlRaw("EXEC [dbo].[uspGetUser] @Code, @Password", pCode, pPassword).ToListAsync();

            if (collection.Count > 0)
            {
                return collection.First().MapTo<UserDto>();
            }
            else
            {
                return null;
            }
        }
    }
}

using Catalog.Persistence.Database;
using Catalog.Services.Queries.DTOs;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Service.Common.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Services.Queries.StoredProcedure
{
    public interface IGetMemoryPersonByUserIdQueryService
    {
        Task<List<MemoryPersonByUserIdDto>> GetMemoryPersonByUserId(Int32 userId);
    }
    public class GetMemoryPersonByUserIdQueryService : IGetMemoryPersonByUserIdQueryService
    {
        private readonly ApplicationDbContext _context;

        public GetMemoryPersonByUserIdQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MemoryPersonByUserIdDto>> GetMemoryPersonByUserId(Int32 userId)
        {
            SqlParameter pUserId = new() { ParameterName = "@userId", SqlDbType = SqlDbType.Int, Value = userId };
            var collection = await _context.MemoryPersonByUserId.FromSqlRaw("EXEC [dbo].[uspGetMemoryPersonByIdUser] @userId", pUserId).ToListAsync();

            return collection.MapTo<List<MemoryPersonByUserIdDto>>();

        }
    }
}

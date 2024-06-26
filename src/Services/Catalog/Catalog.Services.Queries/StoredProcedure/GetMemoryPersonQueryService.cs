﻿using Catalog.Persistence.Database;
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
    public interface IGetMemoryPersonQueryService
    {
        Task<MemoryPersonDto> GetMemoryPersonById(Int32 MemoryPersonId);
        Task<MemoryPersonDto> GetMemoryPersonByCodeQR(string CodeQR);
    }
    public class GetMemoryPersonQueryService : IGetMemoryPersonQueryService
    {
        private readonly ApplicationDbContext _context;

        public GetMemoryPersonQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MemoryPersonDto> GetMemoryPersonById(Int32 MemoryPersonId)
        {
            SqlParameter pMemoryPersonId = new() { ParameterName = "@MemoryPersonId", SqlDbType = SqlDbType.Int, Value = MemoryPersonId };
            var collection = await _context.MemoryPerson.FromSqlRaw("EXEC [dbo].[uspGetMemoryPerson] @MemoryPersonId", pMemoryPersonId).ToListAsync();
            if (collection.Count > 0)
            {
                return collection.First().MapTo<MemoryPersonDto>();
            }
            else
            {
                return null;
            }
        }

        public async Task<MemoryPersonDto> GetMemoryPersonByCodeQR(string CodeQR)
        {
            SqlParameter pCodeQR = new() { ParameterName = "@codeQR", SqlDbType = SqlDbType.VarChar, Value = CodeQR };
            var collection = await _context.MemoryPerson.FromSqlRaw("EXEC [dbo].[uspGetMemoryPersonByCodeQR] @codeQR", pCodeQR).ToListAsync();
            if (collection.Count > 0)
            {
                return collection.First().MapTo<MemoryPersonDto>();
            }
            else
            {
                return null;
            }
        }
    }
}

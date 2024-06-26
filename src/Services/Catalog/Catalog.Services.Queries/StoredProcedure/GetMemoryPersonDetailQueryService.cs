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
    public interface IGetMemoryPersonDetailQueryService
    {
        Task<List<MemoryPersonDetailDto>> GetMemoryPersonDetailById(Int32 MemoryPersonId);
        Task<List<MemoryPersonDetailDto>> GetMemoryPersonDetailByCodeQR(string CodeQR);
    }
    public class GetMemoryPersonDetailQueryService : IGetMemoryPersonDetailQueryService
    {
        private readonly ApplicationDbContext _context;

        public GetMemoryPersonDetailQueryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MemoryPersonDetailDto>> GetMemoryPersonDetailById(Int32 MemoryPersonId)
        {
            SqlParameter pMemoryPersonId = new() { ParameterName = "@MemoryPersonId", SqlDbType = SqlDbType.Int, Value = MemoryPersonId };
            var collection = await _context.MemoryPersonDetail.FromSqlRaw("EXEC [dbo].[uspGetMemoryPersonDetail] @MemoryPersonId", pMemoryPersonId).ToListAsync();

            return collection.MapTo<List<MemoryPersonDetailDto>>();

        }

        public async Task<List<MemoryPersonDetailDto>> GetMemoryPersonDetailByCodeQR(string CodeQR)
        {
            SqlParameter pCodeQR = new() { ParameterName = "@CodeQR", SqlDbType = SqlDbType.VarChar, Value = CodeQR };
            var collection = await _context.MemoryPersonDetail.FromSqlRaw("EXEC [dbo].[uspGetMemoryPersonDetailByCodeQR] @CodeQR", pCodeQR).ToListAsync();

            return collection.MapTo<List<MemoryPersonDetailDto>>();

        }
    }
}

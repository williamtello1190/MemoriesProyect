using Catalog.Persistence.Database;
using Catalog.Services.EventHandlers.Commands.StoredProcedure;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog.Services.EventHandlers.StoredProcedure
{
    public class MemoryPersonCreateEventHandler : IRequestHandler<MemoryPersonCreateCommand, DataResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MemoryPersonCreateEventHandler> _logger;

        public MemoryPersonCreateEventHandler(ApplicationDbContext context, ILogger<MemoryPersonCreateEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DataResponse> Handle(MemoryPersonCreateCommand command, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();
            DataResponse resp = new();

            try
            {
                _context.Database.OpenConnection();
                SqlParameter pName = new() { ParameterName = "@name", SqlDbType = SqlDbType.VarChar, Value = command.Name };
                SqlParameter pLastName = new() { ParameterName = "@lastName", SqlDbType = SqlDbType.VarChar, Value = command.LastName };
                SqlParameter pBirthDate = new() { ParameterName = "@birthDate", SqlDbType = SqlDbType.VarChar, Value = command.BirthDate };
                SqlParameter pDeathDate = new() { ParameterName = "@deathDate", SqlDbType = SqlDbType.VarChar, Value = command.DeathDate };
                SqlParameter pDescription = new() { ParameterName = "@description", SqlDbType = SqlDbType.VarChar, Value = command.Description };
                SqlParameter pUserId = new() { ParameterName = "@userId", SqlDbType = SqlDbType.Int, Value = command.UserId };
                SqlParameter pCodeQR = new() { ParameterName = "@codeQR", SqlDbType = SqlDbType.VarChar, Value = command.CodeQR };
                SqlParameter oCode = new() { ParameterName = "@code", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspRegisterMemoryPerson]  @studentId, @name, @lastName, @birthDate, @deathDate, @description, @userId, @codeQR, @code OUTPUT", pName, pLastName, pBirthDate, pDeathDate, pDescription, pUserId, pCodeQR, oCode);

                int respId = string.IsNullOrWhiteSpace(oCode.Value.ToString()) ? 0 : int.Parse(oCode.Value.ToString());

                if (respId > 0)
                {
                    resp.Code = DataResponse.STATUS_CREADO;
                    resp.Status = true;
                    resp.IDbdGenerado = respId;
                    await transaction.CommitAsync();
                }
                else
                {
                    resp.Code = DataResponse.STATUS_ERROR;
                    resp.Status = false;
                    resp.IDbdGenerado = -1;
                    await transaction.RollbackAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                resp.Code = DataResponse.STATUS_EXCEPTION;
                resp.Message = ex.Message;
                resp.Status = false;
                await transaction.RollbackAsync();
            }
            finally
            {
                _context.Database.CloseConnection();
            }

            return resp;
        }
    
    }
}

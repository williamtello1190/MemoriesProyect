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
    public class MemoryPersonUpdateEventHandler : IRequestHandler<MemoryPersonUpdateCommand, DataResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MemoryPersonUpdateEventHandler> _logger;

        public MemoryPersonUpdateEventHandler(ApplicationDbContext context, ILogger<MemoryPersonUpdateEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DataResponse> Handle(MemoryPersonUpdateCommand command, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();
            DataResponse resp = new();

            try
            {
                _context.Database.OpenConnection();
                SqlParameter pMemoryPersonId = new() { ParameterName = "@memoryPersonId", SqlDbType = SqlDbType.Int, Value = command.MemoryPersonId };
                SqlParameter pName = new() { ParameterName = "@name", SqlDbType = SqlDbType.VarChar, Value = command.Name };
                SqlParameter pLastName = new() { ParameterName = "@lastName", SqlDbType = SqlDbType.VarChar, Value = command.LastName };
                SqlParameter pBirthDate = new() { ParameterName = "@birthDate", SqlDbType = SqlDbType.VarChar, Value = command.BirthDate };
                SqlParameter pDeathDate = new() { ParameterName = "@deathDate", SqlDbType = SqlDbType.VarChar, Value = command.DeathDate };
                SqlParameter pDescription = new() { ParameterName = "@description", SqlDbType = SqlDbType.VarChar, Value = command.Description };
                SqlParameter pUser = new() { ParameterName = "@user", SqlDbType = SqlDbType.Int, Value = command.User };
                SqlParameter oCode = new() { ParameterName = "@code", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspUpdateMemoryPerson] @memoryPersonId, @name, @lastName, @birthDate, @deathDate, @description, @user, @code OUTPUT", pMemoryPersonId, pName, pLastName, pBirthDate, pDeathDate, pDescription, pUser, oCode);

                int respIdMemory = string.IsNullOrWhiteSpace(oCode.Value.ToString()) ? 0 : int.Parse(oCode.Value.ToString());
                if (respIdMemory <= 0)
                {
                    await transaction.CommitAsync();
                    return messageError("No se actualizó la memoria");
                }

                resp.Code = DataResponse.STATUS_CREADO;
                resp.Status = true;
                resp.IDbdGenerado = respIdMemory;
                resp.Message = "Se actualizó Correctamente";
                await transaction.CommitAsync();
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

        public DataResponse messageError(string message)
        {
            var resp = new DataResponse();
            resp.Code = DataResponse.STATUS_ERROR;
            resp.Message = message;
            resp.Status = false;
            return resp;
        }
    }
}

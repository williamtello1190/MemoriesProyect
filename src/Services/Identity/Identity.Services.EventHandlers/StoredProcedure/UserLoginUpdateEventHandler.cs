using Identity.Persistence.Database;
using Identity.Services.EventHandlers.Commands.StoredProcedure;
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

namespace Identity.Services.EventHandlers.StoredProcedure
{
    public class UserLoginUpdateEventHandler : IRequestHandler<UserLoginUpdateCommand, DataResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserLoginUpdateEventHandler> _logger;

        public UserLoginUpdateEventHandler(ApplicationDbContext context, ILogger<UserLoginUpdateEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DataResponse> Handle(UserLoginUpdateCommand command, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();
            DataResponse resp = new();

            try
            {
                _context.Database.OpenConnection();
                SqlParameter pUserId = new() { ParameterName = "@userId", SqlDbType = SqlDbType.Int, Value = command.UserId };
                SqlParameter pPasswordOld = new() { ParameterName = "@passwordOld", SqlDbType = SqlDbType.VarChar, Value = command.PasswordOld };
                SqlParameter pPasswordNew = new() { ParameterName = "@passwordNew", SqlDbType = SqlDbType.VarChar, Value = command.PasswordNew };
                SqlParameter oCode = new() { ParameterName = "@code", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspUpdatePasswordUser] @userId, @passwordOld, @passwordNew, @code OUTPUT", pUserId, pPasswordOld, pPasswordNew, oCode);

                int respIdMemory = string.IsNullOrWhiteSpace(oCode.Value.ToString()) ? 0 : int.Parse(oCode.Value.ToString());
                if (respIdMemory <= 0)
                {
                    await transaction.CommitAsync();
                    return messageError("No se actualizó la clave");
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

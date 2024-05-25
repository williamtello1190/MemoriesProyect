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
    public class AttachmentUpdateEventHandler : IRequestHandler<AttachmentUpdateCommand, DataResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttachmentUpdateEventHandler> _logger;

        public AttachmentUpdateEventHandler(ApplicationDbContext context, ILogger<AttachmentUpdateEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DataResponse> Handle(AttachmentUpdateCommand command, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();
            DataResponse resp = new();

            try
            {
                _context.Database.OpenConnection();
                SqlParameter pMemoryPersonIdDet = new() { ParameterName = "@memoryPersonIdDet", SqlDbType = SqlDbType.Int, Value = command.MemoryPersonDetId };
                SqlParameter pMemoryPersonId = new() { ParameterName = "@attachmentId", SqlDbType = SqlDbType.Int, Value = command.AttachmentId };
                SqlParameter pUser = new() { ParameterName = "@user", SqlDbType = SqlDbType.VarChar, Value = command.User };
                SqlParameter oCodeAttach = new() { ParameterName = "@codeAttach", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspUpdateAttachment]  @memoryPersonIdDet, @attachmentId, @user, @codeAttach OUTPUT", pMemoryPersonIdDet, pMemoryPersonId, pUser, oCodeAttach);

                int respIdAttachment = string.IsNullOrWhiteSpace(oCodeAttach.Value.ToString()) ? 0 : int.Parse(oCodeAttach.Value.ToString());
                if (respIdAttachment <= 0)
                {
                    await transaction.RollbackAsync();
                    return messageError("No se Actualizó");
                }

                resp.Code = DataResponse.STATUS_CREADO;
                resp.Status = true;
                resp.IDbdGenerado = respIdAttachment;
                resp.Message = "Se Actualizó Correctamente";
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

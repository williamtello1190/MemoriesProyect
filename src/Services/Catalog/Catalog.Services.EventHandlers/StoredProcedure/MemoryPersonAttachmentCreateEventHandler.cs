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
    public class MemoryPersonAttachmentCreateEventHandler : IRequestHandler<MemoryPersonAttachmentCommand, DataResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MemoryPersonAttachmentCreateEventHandler> _logger;

        public MemoryPersonAttachmentCreateEventHandler(ApplicationDbContext context, ILogger<MemoryPersonAttachmentCreateEventHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DataResponse> Handle(MemoryPersonAttachmentCommand command, CancellationToken cancellationToken)
        {
            using var transaction = _context.Database.BeginTransaction();
            DataResponse resp = new();

            try
            {
                _context.Database.OpenConnection();
                SqlParameter pFileName = new() { ParameterName = "@fileName", SqlDbType = SqlDbType.VarChar, Value = command.FileName };
                SqlParameter pFilePath = new() { ParameterName = "@filePath", SqlDbType = SqlDbType.VarChar, Value = command.FilePath };
                SqlParameter pPhysicalName = new() { ParameterName = "@physicalName", SqlDbType = SqlDbType.VarChar, Value = command.PhysicalName };
                SqlParameter pExtension = new() { ParameterName = "@extension", SqlDbType = SqlDbType.VarChar, Value = command.Extension };
                SqlParameter pDescriptionAttach = new() { ParameterName = "@descriptionAttach", SqlDbType = SqlDbType.VarChar, Value = "" };
                SqlParameter pMemoryPersonId = new() { ParameterName = "@memoryPersonId", SqlDbType = SqlDbType.Int, Value = command.MemoryPersonId };
                SqlParameter pIsMain = new() { ParameterName = "@isMain", SqlDbType = SqlDbType.VarChar, Value = "" };
                SqlParameter pUserRegister = new() { ParameterName = "@userRegister", SqlDbType = SqlDbType.VarChar, Value = command.UserRegister };
                SqlParameter oCodeAttach = new() { ParameterName = "@codeAttach", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspRegisterAttachment]  @fileName, @filePath, @physicalName, @extension, @descriptionAttach, @memoryPersonId, @isMain, @userRegister, @codeAttach OUTPUT", pFileName, pFilePath, pPhysicalName, pExtension, pDescriptionAttach, pMemoryPersonId, pIsMain, pUserRegister, oCodeAttach);

                int respIdAttachment = string.IsNullOrWhiteSpace(oCodeAttach.Value.ToString()) ? 0 : int.Parse(oCodeAttach.Value.ToString());
                if (respIdAttachment <= 0)
                {
                    await transaction.CommitAsync();
                    return messageError("No se grabo el adjunto");
                }

                resp.Code = DataResponse.STATUS_CREADO;
                resp.Status = true;
                resp.IDbdGenerado = respIdAttachment;
                resp.Message = "Se grabo Correctamente";
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

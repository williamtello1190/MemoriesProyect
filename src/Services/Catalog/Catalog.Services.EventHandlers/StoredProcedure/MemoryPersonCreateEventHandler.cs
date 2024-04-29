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

                #region usuario
                SqlParameter pNameUser = new() { ParameterName = "@nameUser", SqlDbType = SqlDbType.VarChar, Value = command.User.Name };
                SqlParameter pLastNameUser = new() { ParameterName = "@lastNameUser", SqlDbType = SqlDbType.VarChar, Value = command.User.LastName };
                SqlParameter pDocumentNumber = new() { ParameterName = "@documentNumber", SqlDbType = SqlDbType.VarChar, Value = command.User.DocumentNumber };
                SqlParameter pEmail = new() { ParameterName = "@email", SqlDbType = SqlDbType.VarChar, Value = command.User.Email };
                SqlParameter pUserName = new() { ParameterName = "@userName", SqlDbType = SqlDbType.VarChar, Value = command.User.UserName };
                SqlParameter pPassword = new() { ParameterName = "@password", SqlDbType = SqlDbType.VarChar, Value = command.User.Password };
                SqlParameter pUserRegister = new() { ParameterName = "@userRegister", SqlDbType = SqlDbType.VarChar, Value = command.UserRegister };
                SqlParameter oCodeUser = new() { ParameterName = "@codeUser", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspRegisterUser]  @nameUser, @lastNameUser, @documentNumber, @email, @userName, @password, @userRegister, @codeUser OUTPUT", pNameUser, pLastNameUser, pDocumentNumber, pEmail, pUserName, pPassword, pUserRegister, oCodeUser);

                int respIdUser = string.IsNullOrWhiteSpace(oCodeUser.Value.ToString()) ? 0 : int.Parse(oCodeUser.Value.ToString());
                if (respIdUser <= 0)
                {
                    await transaction.CommitAsync();
                    return messageError("No se grabo al usuario");
                }
                #endregion

                #region memoria
                SqlParameter pNumberDocument = new() { ParameterName = "@numberDocument", SqlDbType = SqlDbType.VarChar, Value = command.DocumentNumber };
                SqlParameter pName = new() { ParameterName = "@name", SqlDbType = SqlDbType.VarChar, Value = command.Name };
                SqlParameter pLastName = new() { ParameterName = "@lastName", SqlDbType = SqlDbType.VarChar, Value = command.LastName };
                SqlParameter pBirthDate = new() { ParameterName = "@birthDate", SqlDbType = SqlDbType.VarChar, Value = command.BirthDate };
                SqlParameter pDeathDate = new() { ParameterName = "@deathDate", SqlDbType = SqlDbType.VarChar, Value = command.DeathDate };
                SqlParameter pDescription = new() { ParameterName = "@description", SqlDbType = SqlDbType.VarChar, Value = command.Description };
                SqlParameter pUserId = new() { ParameterName = "@userId", SqlDbType = SqlDbType.Int, Value = respIdUser };
                //SqlParameter pCodeQR = new() { ParameterName = "@codeQR", SqlDbType = SqlDbType.VarChar, Value = command.CodeQR };
                SqlParameter oCode = new() { ParameterName = "@code", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspRegisterMemoryPerson] @numberDocument, @name, @lastName, @birthDate, @deathDate, @description, @userId, @userRegister, @code OUTPUT", pNumberDocument, pName, pLastName, pBirthDate, pDeathDate, pDescription, pUserId, pUserRegister, oCode);

                int respIdMemory = string.IsNullOrWhiteSpace(oCode.Value.ToString()) ? 0 : int.Parse(oCode.Value.ToString());
                if (respIdMemory <= 0)
                {
                    await transaction.CommitAsync();
                    return messageError("No se grabo la memoria");
                }
                #endregion

                #region adjuntos
                if (command.Attachment.Count() > 0) 
                {
                    foreach (var item in command.Attachment) 
                    {
                        SqlParameter pFileName = new() { ParameterName = "@fileName", SqlDbType = SqlDbType.VarChar, Value = item.FileName };
                        SqlParameter pFilePath = new() { ParameterName = "@filePath", SqlDbType = SqlDbType.VarChar, Value = item.FilePath };
                        SqlParameter pPhysicalName = new() { ParameterName = "@physicalName", SqlDbType = SqlDbType.VarChar, Value = item.PhysicalName };
                        SqlParameter pExtension = new() { ParameterName = "@extension", SqlDbType = SqlDbType.VarChar, Value = item.Extension };
                        SqlParameter pDescriptionAttach = new() { ParameterName = "@descriptionAttach", SqlDbType = SqlDbType.VarChar, Value = item.Description };
                        SqlParameter pMemoryPersonId = new() { ParameterName = "@memoryPersonId", SqlDbType = SqlDbType.Int, Value = respIdMemory };
                        SqlParameter pIsMain = new() { ParameterName = "@isMain", SqlDbType = SqlDbType.VarChar, Value = item.IsMain };
                        SqlParameter pFileServer = new() { ParameterName = "@fileServer", SqlDbType = SqlDbType.VarChar, Value = item.FileServer };
                        SqlParameter pOption = new() { ParameterName = "@option", SqlDbType = SqlDbType.VarChar, Value = item.Option};
                        SqlParameter oCodeAttach = new() { ParameterName = "@codeAttach", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                        await _context.Database.ExecuteSqlRawAsync("EXEC [dbo].[uspRegisterAttachment]  @fileName, @filePath, @physicalName, @extension, @descriptionAttach, @memoryPersonId, @isMain, @fileServer, @option, @userRegister, @codeAttach OUTPUT", pFileName, pFilePath, pPhysicalName, pExtension, pDescriptionAttach, pMemoryPersonId, pIsMain, pFileServer, pOption, pUserRegister, oCodeAttach);

                        int respIdAttachment = string.IsNullOrWhiteSpace(oCodeAttach.Value.ToString()) ? 0 : int.Parse(oCodeAttach.Value.ToString());
                        if (respIdAttachment <= 0)
                        {
                            await transaction.CommitAsync();
                            return messageError("No se grabo el adjunto");
                        }
                    }
                }
                #endregion

                resp.Code = DataResponse.STATUS_CREADO;
                resp.Status = true;
                resp.IDbdGenerado = respIdMemory;
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

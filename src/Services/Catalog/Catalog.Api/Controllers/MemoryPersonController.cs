using Catalog.Services.EventHandlers.Commands.StoredProcedure;
using Catalog.Services.Queries.DTOs;
using Catalog.Services.Queries.StoredProcedure;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Service.Common.Collection;
using Service.Common.Util;
using Service.Common.Util.GenerarPlantilla;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Catalog.Api.Controllers
{
    [ApiController]
    [Route("catalog")]
    [EnableCors("AllowOrigin")]
    public class MemoryPersonController : ControllerBase
    {
        private readonly ILogger<MemoryPersonController> _logger;
        private readonly IMediator _mediator;
        private readonly string routeRoot;
        private readonly string _defaultConnection;
        private IConfiguration _configuration { get; }
        private readonly IGetMemoryPersonQueryService _IGetMemoryPersonQueryService;
        private readonly IGetMemoryPersonDetailQueryService _IGetMemoryPersonDetailQueryService;
        private Funciones funciones = new Funciones();
        private readonly string _templateArchivoQR;
        private readonly string _saveArchivoQR;
        private readonly string _saveImageMemory;
        public MemoryPersonController(
          ILogger<MemoryPersonController> logger,
          IMediator mediator,
          IConfiguration configuration,
          IGetMemoryPersonQueryService IGetMemoryPersonQueryService,
          IGetMemoryPersonDetailQueryService IGetMemoryPersonDetailQueryService
          )
        {
            _logger = logger;
            _mediator = mediator;
            _configuration = configuration;
            routeRoot = _configuration.GetSection("ConfigDocument").GetSection("RouteRoot").Value;
            _defaultConnection = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            _IGetMemoryPersonQueryService = IGetMemoryPersonQueryService;
            _IGetMemoryPersonDetailQueryService = IGetMemoryPersonDetailQueryService;
            _templateArchivoQR = _configuration.GetSection("ConfigDocument").GetSection("TemplateArchivoQR").Value;
            _saveArchivoQR = _configuration.GetSection("ConfigDocument").GetSection("SaveArchivoQR").Value;
            _saveImageMemory = _configuration.GetSection("ConfigDocument").GetSection("SaveImageMemory").Value;
        }

        [HttpGet("memoryPerson/{MemoryPersonId}")]
        public async Task<MemoryPersonDto> GetMemoryPerson(Int32 MemoryPersonId)
        {
            return await _IGetMemoryPersonQueryService.GetMemoryPerson(MemoryPersonId);
        }

        [HttpGet("memoryPersonDetail/{MemoryPersonId}")]
        public async Task<List<MemoryPersonDetailDto>>GetMemoryPersonDetail(Int32 MemoryPersonId)
        {
            return await _IGetMemoryPersonDetailQueryService.GetMemoryPersonDetail(MemoryPersonId);
        }

        [HttpPost("createMemoryPerson")]
        public async Task<DataResponse> CreateMemoryPerson(MemoryPersonCreateCommand command)
        {
            var resp = new DataResponse();
            DataResponse<BEFileResponse> respAdjunto = new DataResponse<BEFileResponse>();
            resp.Status = false;
            resp.Code = -1;
            string encriptado = string.Empty;
            DateTime fecha = DateTime.Now;
            string filePath = string.Empty;
            string rutaDinamica = string.Empty;
            try
            {
                string filePathRoot = routeRoot;
                filePath = _saveImageMemory;
                string strMes = fecha.Month.ToString().PadLeft(2, '0');
                string strDia = fecha.Day.ToString().PadLeft(2, '0');
                rutaDinamica += fecha.Year.ToString() + @"\" + strMes + @"\" + strDia + @"\";
                List<MemoryPersonAttachmentCommand> lstdocAttachment = new List<MemoryPersonAttachmentCommand>();
                if (command.Attachment != null && command.Attachment.Count > 0)
                {
                    foreach (MemoryPersonAttachmentCommand doc in command.Attachment)
                    {
                        var respSaveFile = SaveDocument(filePathRoot + filePath + rutaDinamica, doc.FileBase64, doc.Extension, _defaultConnection);
                        if (respSaveFile.Status)
                        {
                            doc.FileName = respSaveFile.Data.FileName;
                            doc.PhysicalName = respSaveFile.Data.FileName;
                            doc.FilePath = filePath + rutaDinamica;
                            doc.FileServer = filePathRoot;
                            doc.Option = "I";
                            lstdocAttachment.Add(doc);
                        }
                        else
                        {
                            resp.Message = respSaveFile.Message + " archivo " + doc.FileName;
                            resp = respSaveFile;
                            return resp;
                        }
                    }
                }

                command.Attachment = lstdocAttachment;
                var result = await _mediator.Send(command);
                if (result.IDbdGenerado > 0)
                {
                    encriptado = funciones.Encriptar(result.IDbdGenerado.ToString());
                    GenerarArchivoQR doc = new GenerarArchivoQR(_templateArchivoQR, _saveArchivoQR, routeRoot);
                    respAdjunto = doc.GenerarArchivo(encriptado, _defaultConnection);
                    if (!respAdjunto.Status)
                    {
                        resp.Message = respAdjunto.Message;
                        return resp;
                    }

                    MemoryPersonAttachmentCommand commandAttach = new MemoryPersonAttachmentCommand();
                    commandAttach.MemoryPersonId = result.IDbdGenerado;
                    commandAttach.UserRegister = command.UserRegister;
                    commandAttach.FileName = respAdjunto.Data.FileName;
                    commandAttach.FilePath = respAdjunto.Data.FileRuta;
                    commandAttach.PhysicalName = respAdjunto.Data.FileName;
                    commandAttach.Extension = ".pdf";
                    commandAttach.FileServer = routeRoot;
                    commandAttach.Description = encriptado;
                    commandAttach.Option = "P";
                    var respAttach = await _mediator.Send(commandAttach);
                    if (respAttach.IDbdGenerado <= 0)
                    {
                        resp.Message = respAttach.Message;
                        return resp;
                    }

                    resp.Message = "Guardado Correctamente";
                    resp.Code = DataResponse.STATUS_CREADO;
                    resp.Status = true;
                    resp.IDbdGenerado = result.IDbdGenerado;
                    resp.File = respAdjunto.File;
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                resp.Message = ex.Message;
                resp.Code = DataResponse.STATUS_EXCEPTION;
            }
            return resp;
        }

        public DataResponse<BEFileResponse> SaveDocument(string filePath, string docCode64, string fileType, string compilado)
        {
            var resp = new DataResponse<BEFileResponse>();
            var Fileresp = new BEFileResponse();
            try
            {

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                if (!Directory.Exists(filePath))
                {
                    resp.Status = false;
                    resp.Code = DataResponse.STATUS_ERROR;
                    resp.Message = "No se pudo crear la ruta";
                    return resp;
                }

                string nameGenPDF = Guid.NewGuid().ToString("N");
                string sNameGenPdf = nameGenPDF + fileType;
                string ambiente = compilado.Contains("/PRUEBAS") ? "pruebas_" : "";
                //grabamos el archivo
                byte[] data = System.Convert.FromBase64String(docCode64);
                System.IO.File.WriteAllBytes(filePath + ambiente + sNameGenPdf, data);

                if (System.IO.File.Exists(filePath + ambiente + sNameGenPdf))
                {
                    Fileresp.FileName = ambiente + sNameGenPdf;
                    resp.Status = true;
                    resp.Data = Fileresp;
                    resp.Code = DataResponse.STATUS_CREADO;
                }
                else
                {
                    resp.Status = false;
                    resp.Code = DataResponse.STATUS_ERROR;
                    resp.Message = "No se pudo almacenar el archivo";
                }


            }
            catch (Exception ex)
            {
                resp.Status = false;
                resp.Code = DataResponse.STATUS_EXCEPTION;
                resp.Message = "Error Inesperado: " + ex.Message;
            }

            return resp;

        }
    }
}

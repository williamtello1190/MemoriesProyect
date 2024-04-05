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
        private readonly string _mail;
        private readonly string _password;
        private readonly string _host;
        private readonly int _port;
        private readonly string _urlWeb;
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
            _mail = _configuration.GetSection("MailSettings").GetSection("Mail").Value;
            _password = _configuration.GetSection("MailSettings").GetSection("Password").Value;
            _host = _configuration.GetSection("MailSettings").GetSection("Host").Value;
            _port = int.Parse(_configuration.GetSection("MailSettings").GetSection("Port").Value);
            _urlWeb = _configuration.GetSection("ConfigDocument").GetSection("UrlWeb").Value;
        }

        [HttpGet("memoryPersonById/{MemoryPersonId}")]
        public async Task<MemoryPersonDto> GetMemoryPersonById(Int32 MemoryPersonId)
        {
            return await _IGetMemoryPersonQueryService.GetMemoryPersonById(MemoryPersonId);
        }

        [HttpGet("memoryPersonDetailById/{MemoryPersonId}")]
        public async Task<List<MemoryPersonDetailDto>> GetMemoryPersonDetailById(Int32 MemoryPersonId)
        {
            return await _IGetMemoryPersonDetailQueryService.GetMemoryPersonDetailById(MemoryPersonId);
        }
        [HttpGet("memoryPersonByCodeQR/{CodeQR}")]
        public async Task<MemoryPersonDto> GetMemoryPersonByCodeQR(string CodeQR)
        {
            return await _IGetMemoryPersonQueryService.GetMemoryPersonByCodeQR(CodeQR);
        }

        [HttpGet("memoryPersonDetailByCodeQR/{CodeQR}")]
        public async Task<List<MemoryPersonDetailDto>> GetMemoryPersonDetailByCodeQR(string CodeQR)
        {
            var resp = new List<MemoryPersonDetailDto>();
            try
            {
                resp = await _IGetMemoryPersonDetailQueryService.GetMemoryPersonDetailByCodeQR(CodeQR);
                if (resp.Count > 0)
                {
                    resp.ForEach(x => x.Base64File = Convert.ToBase64String(System.IO.File.ReadAllBytes(x.FileServer + x.FilePath + x.PhysicalName)));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return resp;
        }

        [HttpPost("createMemoryPerson")]
        public async Task<DataResponse> CreateMemoryPerson(MemoryPersonCreateCommand command)
        {
            var resp = new DataResponse();
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
                command.BirthDate = string.IsNullOrEmpty(command.BirthDate) ? null : DateTime.Parse(command.BirthDate).ToString("dd/MM/yyyy");
                command.DeathDate = string.IsNullOrEmpty(command.DeathDate) ? null : DateTime.Parse(command.DeathDate).ToString("dd/MM/yyyy");
                List<MemoryPersonAttachmentCommand> lstdocAttachment = new List<MemoryPersonAttachmentCommand>();
                if (command.Attachment != null && command.Attachment.Count > 0)
                {
                    foreach (MemoryPersonAttachmentCommand doc in command.Attachment)
                    {
                        SaveFile docSave = new SaveFile(filePathRoot + filePath);
                        var respSaveFile = docSave.SaveDocument(doc.FileBase64, doc.Extension, _defaultConnection);
                        if (respSaveFile.Status)
                        {
                            doc.PhysicalName = respSaveFile.Data.FileName;
                            //doc.FilePath = respSaveFile.Data.FileRuta.Replace(filePathRoot, "");
                            doc.FilePath = respSaveFile.Data.FileRuta;
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
                    string urlPdf = _urlWeb + encriptado;
                    //string urlPdf = _urlWeb;
                    GenerarArchivoQR doc = new GenerarArchivoQR(_saveArchivoQR, routeRoot);
                    var respAdjunto = doc.GenerarArchivo(urlPdf, command.Name + " " + command.LastName, command.BirthDate + " - " + command.DeathDate, _defaultConnection);
                    if (!respAdjunto.Status)
                    {
                        resp.Message = respAdjunto.Message;
                        return resp;
                    }

                    var respAttach = await RegisterAttachment(respAdjunto, result.IDbdGenerado, command.UserRegister, ".pdf", encriptado, "P", string.Empty);
                    if (respAttach.IDbdGenerado <= 0)
                    {
                        resp.Message = respAttach.Message;
                        return resp;
                    }

                    var respEnvio = sendEmail(result.IDbdGenerado, command.User.Email, urlPdf, respAdjunto.Data.ByteFile, command.User.Name + " " + command.User.LastName, command.Name + " " + command.LastName);

                    resp.Message = "Guardado Correctamente - " + respEnvio.Message;
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

        public DataResponse sendEmail(Int64 MemoryPersonId, string correo, string linkUrl, byte[] file, string nameRegister, string nameMemory)
        {
            DataResponse resp = new DataResponse();
            string titulo = string.Empty;
            string msgBody = string.Empty;
            string tituloCorreo = string.Empty;
            DateTime fechaEnvio = DateTime.Now;

            try
            {
                tituloCorreo = "Solicitud de registro de Memoria";
                titulo = "SOLICITUD MEMORIAS";
                string modulo = linkUrl;
                string fechaEnvioEmail = fechaEnvio.ToString("dd ") + "de" + fechaEnvio.ToString(" MMMM ") + "del" + fechaEnvio.ToString(" yyyy");
                #region :: mensaje
                msgBody += @"<tr>
                                    <td colspan = '3'>
                                        <p style = 'text-align:justify'> Estimado(a) Sr(a): " + nameRegister + @"</p>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan = '3'>
                                        <p style = 'text-align:justify'> Se le comunica a usted que su solicitud ha sido registrado." + @" </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan = '3'>
                                        <p style = 'text-align:justify'> " + "Por lo tanto, se le envía el pdf adjunto del Codigo QR generado y el link de la web  " +
                                                                    @"<a href ='" + modulo + @"' target='_BLANK'>Click aquí, para ver el registro.</a></p>
									</td>
								</tr> 
                                <tr>
									<td colspan='3'> 
                                        <p style='text-align:justify'>Lima, " + fechaEnvioEmail + @"</p>
                                    </td>
								</tr>
                                ";
                #endregion


                string logo = "https://dcassetcdn.com/design_img/3401269/726756/726756_18638460_3401269_7de38b77_image.jpg";
                string body = @"
                            <center>
								<table width='90%' style='font-family:calibri; margin:30px; color:#222;'>
									<tr>
										<td><img src='" + logo + @"' width=200 heigth=50/></td>
										<td></td>
									</tr>
									<tr>
										<td colspan='3' align='center'><b>" + titulo + @"</b></td>
									</tr>" + msgBody + @"
									<tr>
										<td colspan='3'>&nbsp;</td>
									</tr>
									<tr>
										<td colspan='3' align='left'>
											<stroing>Nota:</strong><br/>
											Mensaje Automático, por favor no responder.<br/>
											<span style='color:#7EA953'>Imprime este correo electrónico sólo si es necesario. Cuidar el ambiente es responsabilidad de todos.</span><br/>
										</td>
									</tr>
									<tr>
										<td colspan='3' align='center'>&nbsp;</td>
									</tr>
								</table>
							</center>";

                MailRequestDto dataEmail = new MailRequestDto()
                {
                    Body = body,
                    Subject = tituloCorreo,
                    ToEmail = correo
                };

                SendEmail doc = new SendEmail(_host, _port, _mail, _password);
                var respEnvio = doc.EnviarEmail(dataEmail, file, nameMemory);
                if (respEnvio.IDbdGenerado > 0)
                {
                    resp.IDbdGenerado = 1;
                    resp.Status = true;
                    resp.Message = "Se envio correo electronico";
                }
                else
                {
                    resp.Status = false;
                    resp.IDbdGenerado = -1;
                    resp = respEnvio;
                    resp.Message = "No se envio correo electronico";
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                resp.Message = "No se pudo enviar el mensaje";
                resp.Status = false;
                resp.IDbdGenerado = -1;
                resp.Code = DataResponse.STATUS_EXCEPTION;

            }

            return resp;
        }

        [HttpPut("attachmentUpdate")]
        public async Task<DataResponse> AttachmentUpdate(AttachmentUpdateCommand command)
        {
            var resp = new DataResponse();
            try
            {
                resp = await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                resp.Message = ex.Message;
                resp.Code = DataResponse.STATUS_EXCEPTION;
            }
            return resp;
        }

        [HttpPost("addAttachmentMemory")]
        public async Task<DataResponse> AddAttachmentMemory(List<MemoryPersonAttachmentCommand> listCommand)
        {
            var resp = new DataResponse();
            resp.IDbdGenerado = -1;
            string filePathRoot = routeRoot;
            string filePath = _saveImageMemory;
            try
            {
                foreach (MemoryPersonAttachmentCommand doc in listCommand)
                {
                    SaveFile docSave = new SaveFile(filePathRoot + filePath);
                    var respSaveFile = docSave.SaveDocument(doc.FileBase64, doc.Extension, _defaultConnection);
                    if (respSaveFile.Status)
                    {
                        resp = await RegisterAttachment(respSaveFile, doc.MemoryPersonId, doc.UserRegister, doc.Extension, doc.Description, "I", string.Empty);
                    }
                    else
                    {
                        resp.Message = respSaveFile.Message + " archivo " + doc.FileName;
                        resp = respSaveFile;
                        return resp;
                    }
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

        public async Task<DataResponse> RegisterAttachment(DataResponse<BEFileResponse> request, Int64 id, string user, string extension, string description, string option, string isMain)
        {
            var resp = new DataResponse();
            resp.IDbdGenerado = -1;
            try
            {
                MemoryPersonAttachmentCommand commandAttach = new MemoryPersonAttachmentCommand();
                commandAttach.MemoryPersonId = id;
                commandAttach.UserRegister = user;
                commandAttach.FileName = request.Data.FileName;
                commandAttach.FilePath = request.Data.FileRuta;
                commandAttach.PhysicalName = request.Data.FileName;
                commandAttach.Extension = extension;
                commandAttach.FileServer = routeRoot;
                commandAttach.Description = description;
                commandAttach.Option = option;
                commandAttach.IsMain = isMain;
                resp = await _mediator.Send(commandAttach);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                resp.Message = ex.Message;
                resp.Code = DataResponse.STATUS_EXCEPTION;
            }
            return resp;
        }

        [HttpPut("memoryPersonUpdate")]
        public async Task<DataResponse> memoryPersonUpdate(MemoryPersonUpdateCommand command)
        {
            var resp = new DataResponse();
            try
            {
                resp = await _mediator.Send(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + " " + ex.StackTrace);
                resp.Message = ex.Message;
                resp.Code = DataResponse.STATUS_EXCEPTION;
            }
            return resp;
        }
    }
}

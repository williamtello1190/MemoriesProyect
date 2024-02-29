using Catalog.Services.Queries.DTOs;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Util
{
    public class SendEmail
    {
        private readonly string server;
        private readonly int port;
        private readonly string from;
        private readonly string password;

        public SendEmail(string server, int port, string from, string password)
        {
            this.port = port;
            this.server = server;
            this.from = from;
            this.password = password;
        }

        public DataResponse EnviarEmail(MailRequestDto request, byte[] respFile, string nombre)
        {
            var resp = new DataResponse();
            try
            {
                if (string.IsNullOrEmpty(request.ToEmail))
                {
                    resp.Status = false;
                    resp.Message = "No tiene destinatario";
                    resp.Code = DataResponse.STATUS_ERROR;
                    return resp;
                }

                MailMessage message = new MailMessage();
                message.From = new MailAddress(from, "Memorias");
                message.To.Add(new MailAddress(request.ToEmail));

                if (respFile != null)
                {

                    Attachment data = new Attachment(new MemoryStream(respFile, false), nombre, "application/pdf");
                    message.Attachments.Add(data);
                }

                SmtpClient smtp = new SmtpClient()
                {
                    Port = port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = true,
                    Host = server,
                    Credentials = new System.Net.NetworkCredential(from, password)
                };

                message.Subject = request.Subject;
                message.Body = request.Body;
                message.IsBodyHtml = true;
                smtp.Send(message);
                message.Dispose();

                resp.IDbdGenerado = 1;
                resp.Status = true;
                resp.Code = DataResponse.STATUS_CREADO;

            }
            catch (Exception e)
            {
                resp.Status = false;
                resp.Message = "Error Inesperado: " + e.Message;
                resp.Code = DataResponse.STATUS_EXCEPTION;
            }

            return resp;

        }

    }
}

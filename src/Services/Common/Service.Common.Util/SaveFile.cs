using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Util
{
    public class SaveFile
    {
        private readonly string _rutaFileServer;

        public SaveFile(string rutaFileServer)
        {
            _rutaFileServer = rutaFileServer;
        }

        public DataResponse<BEFileResponse> SaveDocument(string FileCode64, string FileType, string ServerPath)
        {
            var resp = new DataResponse<BEFileResponse>();
            resp.Status = false; resp.Code = 0;
            var Fileresp = new BEFileResponse();
            try
            {
                string pathSavePDF = "";
                string rutaDinamica = "";
                rutaDinamica += _rutaFileServer;
                var fecha = DateTime.Now;
                string strMes = fecha.Month.ToString().PadLeft(2, '0');
                string strDia = fecha.Day.ToString().PadLeft(2, '0');
                rutaDinamica += fecha.Year.ToString() + "\\" + strMes + "\\" + strDia + "\\";

                if (!Directory.Exists(rutaDinamica))
                {
                    Directory.CreateDirectory(rutaDinamica);
                }

                string sNameGenPdf = Guid.NewGuid().ToString("N") + "." + FileType;
                string ambiente = ServerPath.Contains("/PRUEBAS") ? "pruebas_" : "";
                sNameGenPdf = ambiente + sNameGenPdf;
                pathSavePDF = rutaDinamica + sNameGenPdf;

                //grabamos el archivo
                byte[] data = System.Convert.FromBase64String(FileCode64);
                File.WriteAllBytes(pathSavePDF, data);

                if (File.Exists(pathSavePDF))
                {
                    Fileresp.FileName = sNameGenPdf;
                    Fileresp.FileRuta = rutaDinamica;
                    resp.Data = Fileresp;
                    resp.Code = DataResponse.STATUS_CREADO;
                    resp.Status = true;
                }

            }
            catch (Exception ex)
            {
                resp.Code = DataResponse.STATUS_EXCEPTION;
                resp.Status = false;
                resp.Message = "Error Inesperado: " + ex.Message;
            }
            return resp;
        }

    }
}

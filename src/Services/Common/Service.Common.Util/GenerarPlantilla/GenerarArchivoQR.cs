using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;
using Service.Common.Collection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static QRCoder.PayloadGenerator;

namespace Service.Common.Util.GenerarPlantilla
{
    public class GenerarArchivoQR
    {
        private readonly string _rutaPlantillaArchivo, _rutaGuardarArchivo, _rutaServer;
        public GenerarArchivoQR() { }
        public GenerarArchivoQR(string rutaPlantillaArchivo, string rutaGuardarArchivo, string rutaServer)
        {
            _rutaPlantillaArchivo = rutaPlantillaArchivo;
            _rutaGuardarArchivo = rutaGuardarArchivo;
            _rutaServer = rutaServer;
        }

        public DataResponse<BEFileResponse> GenerarArchivo(string encriptado, string compilado)
        {
            DataResponse<BEFileResponse> resp = new DataResponse<BEFileResponse>();
            var respAddFile = new DataResponse<BEFileResponse>();
            resp.Status = false;
            resp.Code = -1;
            resp.Data = new BEFileResponse();
            try
            {

                string rutaFile = "";
                string rutaDinamica = "";

                string plantillaPDF = _rutaServer + _rutaPlantillaArchivo + "ConstanciaVerificacionPYM.pdf";
                string pathSavePDF = _rutaGuardarArchivo;

                if (!File.Exists(plantillaPDF))
                {
                    resp.Code = DataResponse.STATUS_ERROR;
                    resp.Message = "No existe plantilla principal o ruta";
                    return resp;
                }

                var fecha = DateTime.Now;
                string strMes = fecha.Month.ToString().PadLeft(2, '0');
                string strDia = fecha.Day.ToString().PadLeft(2, '0');
                string ambiente = compilado.Contains("/PRUEBAS") ? "pruebas_" : "";
                string sNameGenPdf = ambiente + Guid.NewGuid().ToString("N") + ".pdf";
                rutaFile += fecha.Year.ToString() + "\\" + strMes + "\\" + strDia + "\\";
                rutaDinamica += pathSavePDF + rutaFile;
                pathSavePDF = _rutaServer + rutaDinamica + sNameGenPdf;
                respAddFile = GenerarDocumentoQR(plantillaPDF, pathSavePDF, encriptado);

                if (!Directory.Exists(_rutaServer + rutaDinamica))
                {
                    Directory.CreateDirectory(_rutaServer + rutaDinamica);
                }

                if (!respAddFile.Status)
                {
                    resp.Code = DataResponse.STATUS_ERROR;
                    resp.Message = respAddFile.Message;
                    return resp;
                }

                File.WriteAllBytes(pathSavePDF, respAddFile.Data.ByteFile);

                if (!File.Exists(pathSavePDF))
                {
                    resp.Code = DataResponse.STATUS_ERROR;
                    resp.Message = "No se pudo almacenar el PDF";
                    return resp;
                }

                resp.Data.FileRuta = rutaDinamica;
                resp.Data.FileName = sNameGenPdf;
                resp.File = Convert.ToBase64String(respAddFile.Data.ByteFile);
                resp.Status = true;
                resp.Code = DataResponse.STATUS_CREADO;

            }
            catch (Exception ex)
            {
                resp.Status = false;
                resp.Message = "Error Inesperado: " + ex.Message;
            }

            return resp;

        }



        private DataResponse<BEFileResponse> GenerarDocumentoQR(string PlantillaDocx, string pathSavePDF, string encriptado)
        {
            var resp = new DataResponse<BEFileResponse>();

            resp.Code = DataResponse.STATUS_ERROR;
            try
            {
                string opcionBtn = string.Empty;
                String path = string.Empty;

                FileInfo sourceFilePlantillaUnica = new FileInfo(PlantillaDocx);
                PdfReader reader = new PdfReader(sourceFilePlantillaUnica.FullName);

                Document document = new Document();
                MemoryStream output = new MemoryStream();

                PdfWriter writer = PdfWriter.GetInstance(document, output);

                document.Open();
                BaseFont _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                BaseFont _baseFontBold = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                writer.StrictImageSequence = true;
                PdfContentByte content = writer.DirectContent;
                content = writer.DirectContent;
                content.BeginText();

                string urlPDF = @"https://webexterno.sutran.gob.pe/WebExterno" + encriptado;

                Uri myUri = new Uri(urlPDF, UriKind.Absolute);
                Url generator = new Url(myUri.OriginalString);
                string payload = generator.ToString();

                cargarCodigoQR(document, payload);

                content.SetFontAndSize(_baseFontBold, 12);
                content.ShowTextAligned(Element.ALIGN_LEFT, "", 390, 723, 0);

                //foreach (DocumentoGeneradoConstanciaParametrosPDF item in parametros)
                //{
                //    string cadena = item.valor;

                //    if (cadena.Length > item.cantidadPermitido)
                //    {
                //        content.SetFontAndSize(_baseFontBold, 7);

                //        cadena = item.valor.Substring(0, item.cantidadPermitido);
                //        content.ShowTextAligned(Element.ALIGN_LEFT, cadena, item.posicion_x, item.posicion_y + 4, 0);
                //        cadena = item.valor.Substring(item.cantidadPermitido);
                //        content.ShowTextAligned(Element.ALIGN_LEFT, cadena, item.posicion_x, item.posicion_y - 4, 0);
                //    }
                //    else
                //    {
                //        content.SetFontAndSize(_baseFont, 8);
                //        content.ShowTextAligned(Element.ALIGN_LEFT, cadena, item.posicion_x, item.posicion_y, 0);
                //    }

                //}

                content.EndText();

                PdfImportedPage page = writer.GetImportedPage(reader, 1);
                content.AddTemplate(page, 0, 0);

                document.Close();
                reader.Close();
                var file = new BEFileResponse();
                file.ByteFile = output.ToArray();
                resp.Data = file;
                resp.IDbdGenerado = 1;
                resp.Status = true;
                resp.Code = 1;
                resp.Message = "Se genero archivo pdf correctamente";

            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
                resp.Status = false;
            }
            return resp;
        }

        private void cargarCodigoQR(iTextSharp.text.Document document, string codigo)
        {

            QRCoder.QRCodeGenerator qr = new QRCodeGenerator();
            var data = qr.CreateQrCode(codigo, QRCoder.QRCodeGenerator.ECCLevel.L);
            var code = new QRCoder.QRCode(data);
            Bitmap img = code.GetGraphic(5);

            System.IO.MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Jpeg);

            iTextSharp.text.Image imagendemo;

            imagendemo = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Bmp);
            imagendemo.SetAbsolutePosition(30, 6);//'Posicion en el eje cartesiano
            imagendemo.ScaleAbsoluteWidth(100);//'Ancho de la imagen
            imagendemo.ScaleAbsoluteHeight(100);// 'Altura de la imagen
            document.Add(imagendemo);//' Agrega la imagen al documento

        }
    }
}

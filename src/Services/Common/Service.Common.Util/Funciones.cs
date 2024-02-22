using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Service.Common.Collection;
using Spire.Xls;


namespace Service.Common.Util
{
    public class Funciones
    {
        public static string decimalToStringSoles(decimal monto)
        {
            string montoFormateado = "";

            CultureInfo myCI = new CultureInfo("es-PE");
            decimal _monto = 0.00M;
            _monto = monto;
            montoFormateado = _monto.ToString("C2", myCI);
            return montoFormateado;
        }
        public static string ConvertirDobleAMontoEnLetras(double cantidad, string moneda)
        {
            string res, dec = "";
            Int64 entero;
            Int64 decimales;
            entero = Convert.ToInt64(Math.Truncate(cantidad));
            decimales = Convert.ToInt64(Math.Round((cantidad - entero) * 100, 2));
            dec = "CON " + RellenarCadenaPorLaIzquierda(decimales.ToString(), '0', 2) + "/100";
            res = ToText(Convert.ToDouble(entero)) + " " + dec + " " + moneda.ToUpper().Trim();
            return res;
        }

        public static string RellenarCadenaPorLaIzquierda(string cadena, char relleno, int tamanio)
        {
            string cadenaRellenada = cadena;
            while (cadenaRellenada.Length < tamanio)
            {
                cadenaRellenada = relleno + cadenaRellenada;
            }
            return cadenaRellenada;
        }

        public static string ToText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + ToText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30)
            {
                string segundaCifra;
                if ((value % 20).Equals(1))
                {
                    segundaCifra = "UN";
                }
                else
                {
                    segundaCifra = ToText(value % 20);
                }
                Num2Text = "VEINTI" + segundaCifra;
            }
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100)
            {
                string segundaCifra;
                if ((value % 10).Equals(1))
                {
                    segundaCifra = "UN";
                }
                else
                {
                    segundaCifra = ToText(value % 10);
                }
                Num2Text = ToText(Math.Truncate(value / 10) * 10) + " Y " + segundaCifra;
            }
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + ToText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800))
                Num2Text = ToText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = ToText(Math.Truncate(value / 100) * 100) + " " + ToText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + ToText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = ToText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0)
                {
                    Num2Text = Num2Text + " " + ToText(value % 1000);
                }
            }
            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + ToText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = ToText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                {
                    Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000) * 1000000);
                }
            }
            else if (value == 1000000000000)
            {
                Num2Text = "UN BILLON";
            }
            else if (value < 2000000000000)
            {
                Num2Text = "UN BILLON " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            else
            {
                Num2Text = ToText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                {
                    Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
            }

            return Num2Text;
        }

        public static string InicialesNombreEncargado(string nombre)
        {
            string resp = string.Empty;
            if (!string.IsNullOrEmpty(nombre))
            {
                var datos = (from c in nombre.Split(' ') where Char.IsUpper(Convert.ToChar(c.Substring(0, 1))) select c.Substring(0, 1));
                foreach (string caracter in datos)
                {
                    resp = resp + caracter;
                }
            }
            return resp;
        }


        public Byte[] ArrayUnirPdf(List<BEFileResponse> files)
        {

            // Create document object  
            iTextSharp.text.Document PDFdoc = new iTextSharp.text.Document();
            // Create a object of FileStream which will be disposed at the end  
            using (System.IO.MemoryStream FileStream = new System.IO.MemoryStream())
            {
                // Create a PDFwriter that is listens to the Pdf document  
                iTextSharp.text.pdf.PdfCopy PDFwriter = new iTextSharp.text.pdf.PdfCopy(PDFdoc, FileStream);

                // Open the PDFdocument  
                PDFdoc.Open();
                foreach (BEFileResponse item in files)
                {
                    // Create a PDFreader for a certain PDFdocument  
                    PdfReader PDFreader = new PdfReader(item.ByteFile.ToArray());

                    PDFreader.ConsolidateNamedDestinations();
                    // Add content  
                    for (int i = 1; i <= PDFreader.NumberOfPages; i++)
                    {
                        iTextSharp.text.pdf.PdfImportedPage page = PDFwriter.GetImportedPage(PDFreader, i);
                        PDFwriter.AddPage(page);
                    }
                    // iTextSharp.text.pdf.PRAcroForm form = PDFreader.AcroForm;
                    //if (form != null)
                    //{
                    //    PDFwriter.CopyAcroForm(PDFreader);
                    //}

                    // Close PDFreader  
                    PDFreader.Close();

                }
                // Close the PDFdocument and PDFwriter  
                PDFwriter.Close();
                PDFdoc.Close();
                byte[] doc = FileStream.ToArray();
                FileStream.Close();

                return doc;
            }
        }

        public Byte[] ExportTableExcel(DataTable data)
        {
            byte[] array = null;
            try
            {
                DataTable table = data;


                Workbook book = new Workbook();
                Worksheet sheet = book.Worksheets[0];


                sheet.InsertDataTable(table, true, 1, 1);

                sheet.AllocatedRange.Rows[0].Style.Color = System.Drawing.Color.FromArgb(0, 47, 118);
                sheet.AllocatedRange.Rows[0].Style.Font.Color = Color.White;
                sheet.AllocatedRange.Rows[0].Style.Font.Size = 12;
                sheet.AllocatedRange.Style.Font.FontName = "Arial";


                ////book.SaveToFile("insertTableToExcel.xls", ExcelVersion.Version2013);

                using (MemoryStream ms = new MemoryStream())
                {
                    book.SaveToStream(ms);
                    array = ms.ToArray();
                    ms.Close();
                }

            }
            catch (Exception ex)
            {

            }
            return array;
        }

        public Byte[] ConvertImageToPdf(string urlImage)
        {
            byte[] array = null;
            try
            {
                iTextSharp.text.Rectangle pageSize = null;

                //-----------------------------------------------------------------------------
                FileInfo file = new FileInfo(urlImage);
                //-----------------------------------------------------------------------------

                using (var srcImage = new Bitmap(urlImage))
                {

                    if (file.Extension.ToLower() == ".tif" || file.Extension.ToLower() == ".tiff")
                    {
                        pageSize = new iTextSharp.text.Rectangle(iTextSharp.text.PageSize.A4);
                    }
                    else
                    {
                        pageSize = new iTextSharp.text.Rectangle(0, 0, srcImage.Width, srcImage.Height);
                    }
                }
                using (var ms = new MemoryStream())
                {
                    var document = new iTextSharp.text.Document(pageSize, 0, 0, 0, 0);
                    iTextSharp.text.pdf.PdfWriter.GetInstance(document, ms).SetFullCompression();
                    document.Open();
                    var image = iTextSharp.text.Image.GetInstance(urlImage);

                    //-----------------------------------------------------------------------------
                    if (file.Extension.ToLower() == ".tif" || file.Extension.ToLower() == ".tiff")
                    {
                        float percentage = 0.0f;
                        percentage = 540 / image.Width;
                        image.ScalePercent(percentage * 100);
                    }//-----------------------------------------------------------------------------

                    document.Add(image);
                    document.Close();

                    array = ms.ToArray();
                }
            }
            catch (Exception ex)
            {

            }
            return array;
        }

        public DataResponse CopiarArchivoYearMothDay(string rutaOrigen, string rutaDestino, string name)
        {
            var resp = new DataResponse();
            resp.Code = -1;
            resp.Status = false;
            try
            {
                if (!File.Exists(rutaOrigen) || !Directory.Exists(rutaDestino))
                {
                    resp.Message = "No se tiene acceso";
                    return resp;
                }

                var fecha = DateTime.Now;
                string strMes = fecha.Month.ToString().PadLeft(2, '0');
                string strDia = fecha.Day.ToString().PadLeft(2, '0');

                rutaDestino += fecha.Year.ToString() + "\\" + strMes + "\\" + strDia + "\\";

                if (!Directory.Exists(rutaDestino))
                {
                    Directory.CreateDirectory(rutaDestino);
                }

                if (!File.Exists(Path.Combine(rutaDestino, name)))
                {
                    File.Copy(rutaOrigen, Path.Combine(rutaDestino, name));
                }

                if (File.Exists(Path.Combine(rutaDestino, name)))
                {
                    resp.Code = 1;
                    resp.Status = true;
                    resp.Message = rutaDestino;
                }

            }
            catch (Exception ex)
            {
                resp.Message = ex.Message;
            }

            return resp;
        }

        public string ClearTextEspeciales(string texto)
        {
            string value = "";
            string[] caracteres = { "$", "<", "%", "/", "'", "-", "+", "::", ">", "@", ",", "ñ", "Ñ" };

            foreach (string item in caracteres)
            {
                if (texto.Contains(item) && texto != "")
                {
                    texto = texto.Replace(item, "");
                }
            }
            if (texto == "")
            {
                texto = DateTime.Now.ToString("ddmmmyyyyhhmm");
            }
            value = texto;
            return value;
        }

        public string Encriptar(string _cadenaAencriptar)
        {
            string result = string.Empty;
            byte[] encryted = System.Text.Encoding.Unicode.GetBytes(_cadenaAencriptar);
            result = Convert.ToBase64String(encryted);
            return result;
        }

        public string DesEncriptar(string _cadenaAdesencriptar)
        {
            string result = string.Empty;
            byte[] decryted = Convert.FromBase64String(_cadenaAdesencriptar);
            result = System.Text.Encoding.Unicode.GetString(decryted);
            return result;
        }
        public DataResponse<BEFileResponse> HtmlToPdf(string html, string ServerPath, string rutaFileServer)
        {
            var resp = new DataResponse<BEFileResponse>();
            var Fileresp = new BEFileResponse();
            try
            {
                resp.Status = false;
                resp.Code = DataResponse.STATUS_ERROR;
                MemoryStream msOutput = new MemoryStream();
                TextReader reader = new StringReader(html);
                var document = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 50, 50, 50, 50);
                var writer = PdfWriter.GetInstance(document, msOutput);
                var worker = new iTextSharp.text.html.simpleparser.HTMLWorker(document);
                document.Open();
                worker.StartDocument();
                worker.Parse(reader);
                worker.EndDocument();
                worker.Close();
                document.Close();

                string pathSavePDF = "";
                string rutaDinamica = "";
                rutaDinamica += rutaFileServer;
                var fecha = DateTime.Now;
                string strMes = fecha.Month.ToString().PadLeft(2, '0');
                string strDia = fecha.Day.ToString().PadLeft(2, '0');
                rutaDinamica += fecha.Year.ToString() + "\\" + strMes + "\\" + strDia + "\\";

                if (!Directory.Exists(rutaDinamica))
                {
                    Directory.CreateDirectory(rutaDinamica);
                }

                string sNameGenPdf = Guid.NewGuid().ToString("N") + ".pdf";
                string archivotest = "";
                archivotest = ServerPath.Contains("/PRUEBAS") ? "pruebas_" : "";
                sNameGenPdf = archivotest + sNameGenPdf;
                pathSavePDF = rutaDinamica + sNameGenPdf;
                var respAddFile = new DataResponse();
                //grabamos el archivo
                byte[] data = System.Convert.FromBase64String(Convert.ToBase64String(msOutput.ToArray()));
                File.WriteAllBytes(pathSavePDF, data);

                if (File.Exists(pathSavePDF))
                {
                    Fileresp.FileName = sNameGenPdf;
                    Fileresp.FileRuta = rutaDinamica;
                    resp.Status = true;
                    resp.Data = Fileresp;
                    resp.Code = DataResponse.STATUS_CREADO;
                }
            }
            catch (Exception ex)
            {
                resp.Status = false;
                resp.Message = "Error Inesperado: " + ex.Message;
                resp.Code = DataResponse.STATUS_EXCEPTION;
            }
            return resp;
        }

        public string NumberInText(double value)
        {
            string Num2Text = "";
            value = Math.Truncate(value);
            if (value == 0) Num2Text = "CERO";
            else if (value == 1) Num2Text = "UNO";
            else if (value == 2) Num2Text = "DOS";
            else if (value == 3) Num2Text = "TRES";
            else if (value == 4) Num2Text = "CUATRO";
            else if (value == 5) Num2Text = "CINCO";
            else if (value == 6) Num2Text = "SEIS";
            else if (value == 7) Num2Text = "SIETE";
            else if (value == 8) Num2Text = "OCHO";
            else if (value == 9) Num2Text = "NUEVE";
            else if (value == 10) Num2Text = "DIEZ";
            else if (value == 11) Num2Text = "ONCE";
            else if (value == 12) Num2Text = "DOCE";
            else if (value == 13) Num2Text = "TRECE";
            else if (value == 14) Num2Text = "CATORCE";
            else if (value == 15) Num2Text = "QUINCE";
            else if (value < 20) Num2Text = "DIECI" + ToText(value - 10);
            else if (value == 20) Num2Text = "VEINTE";
            else if (value < 30)
            {
                string segundaCifra;
                if ((value % 20).Equals(1))
                {
                    segundaCifra = "UN";
                }
                else
                {
                    segundaCifra = ToText(value % 20);
                }
                Num2Text = "VEINTI" + segundaCifra;
            }
            else if (value == 30) Num2Text = "TREINTA";
            else if (value == 40) Num2Text = "CUARENTA";
            else if (value == 50) Num2Text = "CINCUENTA";
            else if (value == 60) Num2Text = "SESENTA";
            else if (value == 70) Num2Text = "SETENTA";
            else if (value == 80) Num2Text = "OCHENTA";
            else if (value == 90) Num2Text = "NOVENTA";
            else if (value < 100)
            {
                string segundaCifra;
                if ((value % 10).Equals(1))
                {
                    segundaCifra = "UN";
                }
                else
                {
                    segundaCifra = ToText(value % 10);
                }
                Num2Text = ToText(Math.Truncate(value / 10) * 10) + " Y " + segundaCifra;
            }
            else if (value == 100) Num2Text = "CIEN";
            else if (value < 200) Num2Text = "CIENTO " + ToText(value - 100);
            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800))
                Num2Text = ToText(Math.Truncate(value / 100)) + "CIENTOS";
            else if (value == 500) Num2Text = "QUINIENTOS";
            else if (value == 700) Num2Text = "SETECIENTOS";
            else if (value == 900) Num2Text = "NOVECIENTOS";
            else if (value < 1000) Num2Text = ToText(Math.Truncate(value / 100) * 100) + " " + ToText(value % 100);
            else if (value == 1000) Num2Text = "MIL";
            else if (value < 2000) Num2Text = "MIL " + ToText(value % 1000);
            else if (value < 1000000)
            {
                Num2Text = ToText(Math.Truncate(value / 1000)) + " MIL";
                if ((value % 1000) > 0)
                {
                    Num2Text = Num2Text + " " + ToText(value % 1000);
                }
            }
            else if (value == 1000000) Num2Text = "UN MILLON";
            else if (value < 2000000) Num2Text = "UN MILLON " + ToText(value % 1000000);
            else if (value < 1000000000000)
            {
                Num2Text = ToText(Math.Truncate(value / 1000000)) + " MILLONES ";
                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0)
                {
                    Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000) * 1000000);
                }
            }
            else if (value == 1000000000000)
            {
                Num2Text = "UN BILLON";
            }
            else if (value < 2000000000000)
            {
                Num2Text = "UN BILLON " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
            }
            else
            {
                Num2Text = ToText(Math.Truncate(value / 1000000000000)) + " BILLONES";
                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0)
                {
                    Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);
                }
            }

            return Num2Text;
        }

        public static string ReplaceSpecialCharacter(string caracter)
        {
            string valor = caracter.ToUpper();
            valor = Regex.Replace(valor, @"[¿]+", "Ñ");
            valor = Regex.Replace(valor, @"&apos;", "'");
            valor = Regex.Replace(valor, @"&amp;", "&");
            valor = Regex.Replace(valor, @"N[?]", "Nº");
            return valor;
        }

        public static Byte[] ExportarExcelSinFiltros(System.Data.DataTable dataTable)
        {
            byte[] array = null;
            try
            {
                using (ExcelPackage pck = new ExcelPackage())
                {
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Resultado");

                    ws.PrinterSettings.Orientation = eOrientation.Landscape;
                    ws.PrinterSettings.PaperSize = ePaperSize.A4;
                    ws.PrinterSettings.RightMargin = 0;
                    ws.PrinterSettings.LeftMargin = 0;
                    ws.PrinterSettings.Scale = 60;

                    //Load Data
                    ws.Cells["A1"].LoadFromDataTable(dataTable, true);

                    //Set Style Header
                    using (ExcelRange rng = ws.Cells[1, 1, 1, dataTable.Columns.Count])
                    {
                        rng.Style.Font.Bold = true;
                        rng.Style.Font.Size = 11;
                        rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rng.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#003366"));
                        rng.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    }

                    ws.PrinterSettings.RepeatRows = ws.Cells["1:1"];
                    ws.PrinterSettings.RepeatColumns = ws.Cells["A:A"];

                    using (ExcelRange col = ws.Cells[2, 1, dataTable.Rows.Count + 1, dataTable.Columns.Count])
                    {
                        col.Style.Font.Size = 10;
                        col.Style.WrapText = false;
                        col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        col.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        col.AutoFitColumns();
                    }

                    array = pck.GetAsByteArray();
                }
            }
            catch (Exception)
            {
                array = null;
            }
            return array;
        }
    }
}

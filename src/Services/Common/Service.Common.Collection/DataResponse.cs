using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.Collection
{
    public class DataResponse
    {
        public const Int32 STATUS_OK = 0;
        public const Int32 STATUS_CREADO = 1;
        public const Int32 STATUS_MODIFICADO = 2;
        public const Int32 STATUS_ELIMINADO = 3;
        public const Int32 STATUS_ERROR = -1;
        public const Int32 STATUS_EXCEPTION = -2;

        public Int32 Code { get; set; }
        public String Message { get; set; }
        public Int64 IDbdGenerado { get; set; }
        public Boolean Status { get; set; }
        public String File { get; set; }
        public String NroGenerado { get; set; }

        public Byte[] ByteFile { get; set; }


        public DataResponse()
        {
            this.Code = STATUS_ERROR;
            this.Message = String.Empty;
            this.IDbdGenerado = 0;
            this.Status = true;
            this.File = String.Empty;
            this.NroGenerado = String.Empty;

        }

        public DataResponse(Int32 status, String mensaje, Int64 idgenerado)
        {
            this.Code = status;
            this.Message = mensaje;
            this.IDbdGenerado = idgenerado;
        }
    }
    public class DataResponse<T> : DataResponse
    {
        public T Data { get; set; }

    }

    public class BEFileResponse
    {
        public string FileRuta { get; set; }
        public string FileName { get; set; }
        public Byte[] ByteFile { get; set; }
        public string FileBase64 { get; set; }
    }
}

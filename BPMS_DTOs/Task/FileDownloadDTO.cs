using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task
{
    public class FileDownloadDTO
    {
        public string FileName { get; set; } = string.Empty;
        public string MIMEType { get; set; } = string.Empty;
        public byte[] Data { get; set; } = new byte[1];
    }
}

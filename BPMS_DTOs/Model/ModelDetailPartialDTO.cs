using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelDetailPartialDTO : ModelDetailInfoDTO
    {
        public string SVG { get; set; } = string.Empty;
    }
}

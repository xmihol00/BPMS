using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ShareTypes
{
    public class RecieveEventShareDTO : BlockModelShareDTO
    {
        public Guid? SenderId { get; set; }
        public bool Editable { get; set; }
    }
}

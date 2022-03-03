using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemDetailDTO : SystemDetailPartialDTO
    {
        public List<SystemAllDTO> OtherSystems { get; set; } = new List<SystemAllDTO>();
        public SystemAllDTO SelectedSystem { get; set; } = new SystemAllDTO();
    }
}

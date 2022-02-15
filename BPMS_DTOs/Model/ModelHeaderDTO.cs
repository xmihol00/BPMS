using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelHeaderDTO : ModelInfoDTO
    {
        public Guid Id { get; set; }
    }
}

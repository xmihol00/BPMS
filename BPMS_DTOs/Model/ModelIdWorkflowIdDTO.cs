using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelIdWorkflowDTO
    {
        public Guid ModelId { get; set; }
        public Guid WorkflowId { get; set; }
    }
}

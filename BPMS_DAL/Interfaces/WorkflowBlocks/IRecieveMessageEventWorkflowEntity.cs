using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.WorkflowBlocks
{
    public interface IRecieveMessageEventWorkflowEntity : IBlockWorkflowEntity
    {
        public bool Delivered { get; set; }
    }
}

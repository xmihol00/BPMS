using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum BlockTypeEnum
    {
        Unset,
        UserTask,
        ServiceTask,
        ReceiveEvent,
        SendEvent,
        InclusiveGateway,
        ExclusiveGateway,
    }
}

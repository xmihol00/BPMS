using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;

namespace BPMS_Common.Interfaces
{
    public interface IAddressAuth : IAuth
    {
        public string DestinationURL { get; set; }
    }
}

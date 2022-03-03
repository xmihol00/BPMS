using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Interfaces
{
    public interface IAuthInfo
    {
        public byte[]? Key { get; set; }
        public Guid SystemId { get; set; }
    }
}

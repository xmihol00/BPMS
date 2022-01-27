using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class SystemPoolEntity
    {
        public Guid SystemId { get; set; }
        public Guid PoolId { get; set; }
        public SystemEntity System { get; set; } = new SystemEntity();
        public PoolEntity Pool { get; set; } = new PoolEntity();
    }
}

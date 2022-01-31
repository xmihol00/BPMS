using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class StartEventModelEntity : BlockModelEntity, IStartEventModelEntity
    {
        public StartEventModelEntity() : base() {}
        public StartEventModelEntity(PoolEntity pool) : base(pool) { }
    }
}

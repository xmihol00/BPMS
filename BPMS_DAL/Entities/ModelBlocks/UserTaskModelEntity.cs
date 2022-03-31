using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class UserTaskModelEntity : BlockModelEntity, IUserTaskModelEntity, IAttributes
    {
        public UserTaskModelEntity() : base() {}
        public UserTaskModelEntity(PoolEntity pool) : base(pool) { }
        public int Difficulty { get; set; }
    }
}

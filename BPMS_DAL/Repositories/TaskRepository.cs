using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_Common.Enums;

namespace BPMS_DAL.Repositories
{
    public class TaskRepository : BaseRepository<BlockWorkflowEntity>
    {
        public TaskRepository(BpmsDbContext context) : base(context) {}

    }
}

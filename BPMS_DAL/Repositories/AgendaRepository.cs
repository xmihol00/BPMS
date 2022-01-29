using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Repositories
{
    public class AgendaRepository : BaseRepository<AgendaEntity>
    {
        public AgendaRepository(BpmsDbContext context) : base(context) {} 
    }
}

using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Role;
using BPMS_Common.Enums;
using BPMS_DTOs.User;

namespace BPMS_DAL.Repositories
{
    public class SystemAgendaRepository : BaseRepository<SystemAgendaEntity>
    {
        public SystemAgendaRepository(BpmsDbContext context) : base(context) {}
    }
}

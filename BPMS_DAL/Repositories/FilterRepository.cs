using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Repositories
{
    public class FilterRepository : BaseRepository<FilterEntity>
    {
        public FilterRepository(BpmsDbContext context) : base(context) {}
    }
}

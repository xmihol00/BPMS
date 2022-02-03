using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Model;

namespace BPMS_DAL.Repositories
{
    public class ModelRepository : BaseRepository<ModelEntity>
    {
        public ModelRepository(BpmsDbContext context) : base(context) {}

        public Task<List<AllModelDTO>> OfAgenda(Guid id)
        {
            return _dbSet.Where(x => x.AgendaId == id)
                         .Select(x => new AllModelDTO
                         {
                             ModelId = x.Id,
                             Name = x.Name,
                             SVG = x.SVG
                         })
                         .ToListAsync();
        }
    }
}

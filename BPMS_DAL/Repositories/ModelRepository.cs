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
                             Id = x.Id,
                             Name = x.Name,
                             SVG = x.SVG
                         })
                         .ToListAsync();
        }

        public Task<ModelDetailDTO> Detail(Guid id)
        {
            return _dbSet.Select(x => new ModelDetailDTO
                         {
                             Id = x.Id,
                             Description = x.Description,
                             Name = x.Name,
                             SVG = x.SVG
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<ModelHeaderDTO> Header(Guid id)
        {
            return _dbSet.Select(x => new ModelHeaderDTO
                         {
                             Id = x.Id,
                             Description = x.Description,
                             Name = x.Name
                         })
                         .FirstAsync(x => x.Id == id);
        }
    }
}

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
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Repositories
{
    public class ModelRepository : BaseRepository<ModelEntity>
    {
        public ModelRepository(BpmsDbContext context) : base(context) {}

        public Task<List<ModelAllDTO>> OfAgenda(Guid id)
        {
            return _dbSet.Where(x => x.AgendaId == id)
                         .Select(x => new ModelAllDTO
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
                             SVG = x.SVG,
                             State = x.State
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<ModelEntity> DetailRaw(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<ModelShareDTO> Share(Guid id)
        {
            return _dbSet.Select(x => new ModelShareDTO
                         {
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             State = x.State,
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
                             Name = x.Name,
                             State = x.State
                         })
                         .FirstAsync(x => x.Id == id);
        }
    }
}

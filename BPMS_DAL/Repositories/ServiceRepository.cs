using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Service;
using Microsoft.Extensions.Primitives;

namespace BPMS_DAL.Repositories
{
    public class ServiceRepository : BaseRepository<ServiceEntity>
    {
        public ServiceRepository(BpmsDbContext context) : base(context) {} 

        public Task<List<ServiceAllDTO>> All()
        {
            return _dbSet.Select(x => new ServiceAllDTO 
                         {
                             HttpMethod = x.HttpMethod,
                             Id = x.Id,
                             Name = x.Name,
                             Serialization = x.Serialization,
                             Type = x.Type,
                             URL = x.URL
                         })
                         .ToListAsync();
        }

        public Task<ServiceEditPageDTO> Edit(Guid id)
        {
            return _dbSet.Select(x => new ServiceEditPageDTO
                         {
                            Description = x.Description,
                            HttpMethod = x.HttpMethod,
                            Id = x.Id,
                            Name = x.Name,
                            Serialization = x.Serialization,
                            Type = x.Type,
                            URL = x.URL,
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<ServiceEntity> Detail(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}

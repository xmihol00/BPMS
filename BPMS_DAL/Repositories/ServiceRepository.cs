using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Service;
using Microsoft.Extensions.Primitives;
using BPMS_DTOs.Header;

namespace BPMS_DAL.Repositories
{
    public class ServiceRepository : BaseRepository<ServiceEntity>
    {
        public ServiceRepository(BpmsDbContext context) : base(context) {} 

        public Task<List<ServiceAllDTO>> All(Guid? apartFromId = null)
        {
            return _dbSet.Where(x => x.Id != apartFromId)
                         .Select(x => new ServiceAllDTO 
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

        public Task<ServiceRequestDTO> ForRequest(Guid id)
        {
            return _dbSet.Include(x => x.Headers)
                         .Select(x => new ServiceRequestDTO 
                         {
                             Id = x.Id,
                             HttpMethod = x.HttpMethod,
                             Type = x.Type,
                             Serialization = x.Serialization,
                             URL = x.URL,
                             Headers = x.Headers.Select(y => new HeaderRequestDTO
                                                {
                                                    Name = y.Name,
                                                    Value = y.Value
                                                })
                                                .ToList()
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<ServiceIdNameDTO>> AllIdNames()
        {
            return _dbSet.Select(x => new ServiceIdNameDTO 
                         {
                             Id = x.Id,
                             Name = x.Name
                         })
                         .ToListAsync();
        }

        public Task<ServiceAllDTO> Selected(Guid id)
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
                         .FirstAsync(x => x.Id == id);
        }
    }
}

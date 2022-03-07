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
                             URL = x.URL,
                             AuthType = x.AuthType
                         })
                         .ToListAsync();
        }

        public Task<ServiceDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Headers)
                         .Select(x => new ServiceDetailDTO
                         {
                            Description = x.Description,
                            HttpMethod = x.HttpMethod,
                            Id = x.Id,
                            Name = x.Name,
                            Serialization = x.Serialization,
                            Type = x.Type,
                            URL = x.URL,
                            AppId = x.AppId,
                            AuthType = x.AuthType,
                            Headers = x.Headers.Select(y => new HeaderAllDTO
                                               {
                                                   Id = y.Id,
                                                   Name = y.Name,
                                                   Value = y.Value
                                               })
                                               .ToList()
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
                             AppId = x.AppId,
                             AppSecret = x.AppSecret,
                             AuthType = x.AuthType,
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
                             URL = x.URL,
                             AuthType = x.AuthType
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<ServiceEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}

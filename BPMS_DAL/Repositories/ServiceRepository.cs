using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Service;
using Microsoft.Extensions.Primitives;
using BPMS_DTOs.Header;
using BPMS_Common.Enums;

namespace BPMS_DAL.Repositories
{
    public class ServiceRepository : BaseRepository<ServiceEntity>
    {
        public ServiceRepository(BpmsDbContext context) : base(context) {} 

        public Task<List<ServiceAllDTO>> All(Guid? id = null)
        {
            IQueryable<ServiceEntity> query = _dbSet.Where(x => x.Id != id);

            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.ServiceDELETE)] || Filters[((int)FilterTypeEnum.ServiceGET)] ||
                    Filters[((int)FilterTypeEnum.ServicePOST)] || Filters[((int)FilterTypeEnum.ServicePUT)] ||
                    Filters[((int)FilterTypeEnum.ServicePATCH)])
                {
                    query = query.Where(x => (Filters[((int)FilterTypeEnum.ServiceDELETE)] && x.HttpMethod == HttpMethodEnum.DELETE) ||
                                             (Filters[((int)FilterTypeEnum.ServiceGET)] && x.HttpMethod == HttpMethodEnum.GET) ||
                                             (Filters[((int)FilterTypeEnum.ServicePATCH)] && x.HttpMethod == HttpMethodEnum.PATCH) ||
                                             (Filters[((int)FilterTypeEnum.ServicePUT)] && x.HttpMethod == HttpMethodEnum.PUT) ||
                                             (Filters[((int)FilterTypeEnum.ServicePOST)] && x.HttpMethod == HttpMethodEnum.POST));
                }

                if (Filters[((int)FilterTypeEnum.ServiceJSON)] || Filters[((int)FilterTypeEnum.ServiceXML)] ||
                    Filters[((int)FilterTypeEnum.ServiceURL)] || Filters[((int)FilterTypeEnum.ServiceReplace)])
                {
                    query = query.Where(x => (Filters[((int)FilterTypeEnum.ServiceJSON)] && x.Serialization == SerializationEnum.JSON) ||
                                             (Filters[((int)FilterTypeEnum.ServiceXML)] && (x.Serialization == SerializationEnum.XMLMarks || x.Serialization == SerializationEnum.XMLAttributes)) ||
                                             (Filters[((int)FilterTypeEnum.ServiceURL)] && x.Serialization == SerializationEnum.URL) ||
                                             (Filters[((int)FilterTypeEnum.ServiceReplace)] && x.Serialization == SerializationEnum.Replace));
                }

                if (Filters[((int)FilterTypeEnum.ServiceREST)])
                {
                    query = query.Where(x => x.Type == ServiceTypeEnum.REST);
                }
            }

            return query.Select(x => new ServiceAllDTO 
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

        public Task<ServiceRequestDTO?> ForRequest(Guid? id)
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
                         .FirstOrDefaultAsync(x => x.Id == id);
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

using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_Common.Enums;

namespace BPMS_DAL.Repositories
{
    public class ServiceDataSchemaRepository : BaseRepository<ServiceDataSchemaEntity>
    {
        public ServiceDataSchemaRepository(BpmsDbContext context) : base(context) {}

        public Task<ServiceDataSchemaDetailDTO> Detail(Guid id)
        {
            return _dbSet.Select(x => new ServiceDataSchemaDetailDTO 
                         {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Alias = x.Alias,
                            Name = x.Name,
                            Compulsory = x.Compulsory,
                            Type = x.Type,
                            Order = x.Order,
                            ServiceId = x.ServiceId,
                            StaticData = x.StaticData
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<ServiceDataSchemaNodeDTO>> DataSchemas(Guid serviceId, DirectionEnum direction)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId && x.Direction == direction)
                         .Select(x => new ServiceDataSchemaNodeDTO 
                         {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Alias = x.Alias,
                            Name = x.Name,
                            Type = x.Type,
                            Compulsory = x.Compulsory,
                            StaticData = x.StaticData
                         })
                         .ToListAsync();
        }

        public Task<List<ServiceDataSchemaDataDTO>> DataSchemasTest(Guid serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId && x.Direction == DirectionEnum.Input)
                         .Select(x => new ServiceDataSchemaDataDTO 
                         {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Alias = x.Alias,
                            Name = x.Name,
                            Type = x.Type,
                            StaticData = x.StaticData
                         })
                         .ToListAsync();
        }

        public Task<List<ServiceDataSchemaEntity>> SchemasForRemoval(Guid id)
        {
            return _dbSet.AsNoTracking()
                         .Include(x => x.Service)
                            .ThenInclude(x => x.DataSchemas)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Service.DataSchemas)
                         .ToListAsync();
        }

        public Task<List<ServiceDataSchemaAllDTO>> Test(Guid serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId)
                         .Select(x => new ServiceDataSchemaAllDTO 
                         {
                             Alias = x.Alias,
                             Compulsory = x.Compulsory,
                             Id = x.Id,
                             Name = x.Name,
                             StaticData = x.StaticData,
                             Type = x.Type
                         })
                         .ToListAsync();
        }
    }
}

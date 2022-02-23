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

        public Task<DataSchemaDetailDTO> Detail(Guid id)
        {
            return _dbSet.Select(x => new DataSchemaDetailDTO 
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

        public Task<List<DataSchemaNodeDTO>> DataSchemas(Guid serviceId, DirectionEnum direction)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId && x.Direction == direction)
                         .Select(x => new DataSchemaNodeDTO 
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

        public Task<List<DataSchemaDataDTO>> DataSchemasTest(Guid serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId && x.Direction == DirectionEnum.Input)
                         .Select(x => new DataSchemaDataDTO 
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

        public Task<List<DataSchemaAllDTO>> Test(Guid serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId && x.Direction == DirectionEnum.Input)
                         .Select(x => new DataSchemaAllDTO 
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

        public Task<ServiceDataSchemaEntity?> Find(Guid serviceId, string alias, Guid? parentId, DirectionEnum direction)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.ServiceId == serviceId && x.Alias == alias && 
                                                   x.ParentId == parentId && x.Direction == direction);
        }

        public Task<List<DataSchemaAttributeDTO>> AsAttributes(Guid? serviceId, DirectionEnum direction)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId && x.Type != DataTypeEnum.Object && 
                                x.Direction == direction && x.StaticData == null)
                         .Select(x => new DataSchemaAttributeDTO
                         {
                             Compulsory = x.Compulsory,
                             Name = x.Name,
                             Type = x.Type
                         })
                         .ToListAsync();
        }

        public Task<List<DataSchemaBareDTO>> All(Guid? serviceId)
        {
            return _dbSet.Where(x => x.ServiceId == serviceId)
                         .Select(x => new DataSchemaBareDTO
                         {
                             Id = x.Id,
                             Type = x.Type
                         })
                         .ToListAsync();
        }

        public Task<List<ServiceDataSchemaEntity>> AllWithMaps(Guid? serviceId)
        {
            return _dbSet.Include(x => x.Blocks)
                         .Where(x => x.ServiceId == serviceId)
                         .Select(x => new ServiceDataSchemaEntity 
                         {
                             Type = x.Type,
                             Blocks = x.Blocks,
                             Id = x.Id
                         })
                         .ToListAsync();
        }
    }
}

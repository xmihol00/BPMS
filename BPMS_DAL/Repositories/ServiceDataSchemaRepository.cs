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
                         })
                         .ToListAsync();
        }
    }
}

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

namespace BPMS_DAL.Repositories
{
    public class ServiceDataSchemaRepository : BaseRepository<ServiceDataSchemaEntity>
    {
        public ServiceDataSchemaRepository(BpmsDbContext context) : base(context) {}

        public Task<List<ServiceDataSchemaNodeDTO>> DataSchemas(Guid blockId)
        {
            return _dbSet.Where(x => x.ServiceId == blockId)
                         .Select(x => new ServiceDataSchemaNodeDTO 
                         {
                            Id = x.Id,
                            ParentId = x.ParentId,
                            Alias = x.Alias,
                            Name = x.Name,
                            Compulsory = x.Compulsory,
                            DataType = x.DataType,
                         })
                         .ToListAsync();
        }
    }
}

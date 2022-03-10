using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.DataSchema;

namespace BPMS_DAL.Repositories
{
    public class DataSchemaMapRepository : BaseRepository<DataSchemaMapEntity>
    {
        public DataSchemaMapRepository(BpmsDbContext context) : base(context) {}

        public Task<List<DataSchemaMappedDTO>> Mapped(Guid serviceTaskId)
        {
            return _dbSet.Include(x => x.Source)
                         .Include(x => x.Target)
                         .Where(x => x.ServiceTaskId == serviceTaskId)
                         .Select(x => new DataSchemaMappedDTO
                         {
                             ServiceTaskId = x.ServiceTaskId,
                             Source = new DataSchemaMapDTO
                             {
                                 Alias = x.Source.Alias,
                                 Id = x.Source.Id,
                                 Name = x.Source.Name,
                                 Type = x.Source.Type
                             },
                             Target = new DataSchemaMapDTO
                             {
                                 Alias = x.Target.Alias,
                                 Id = x.Target.Id,
                                 Name = x.Target.Name,
                                 Type = x.Target.Type
                             }
                         })
                         .ToListAsync();
        }

        public Task<bool> Any(Guid serviceTaskId, Guid sourceId, Guid targetId)
        {
            return _dbSet.AnyAsync(x => x.ServiceTaskId == serviceTaskId && x.SourceId == sourceId && x.TargetId == targetId);
        }
    }
}

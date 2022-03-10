using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Notification;
using BPMS_Common.Enums;

namespace BPMS_DAL.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationEntity>
    {
        public NotificationRepository(BpmsDbContext context) : base(context) {}

        public Task<List<NotificationAllDTO>> All(Guid userId)
        {
            return _dbSet.Where(x => x.UserId == userId && (x.State == NotificationStateEnum.Marked || x.State == NotificationStateEnum.Unread))
                         .Select(x => new NotificationAllDTO
                         {
                             Date = x.Date,
                             Id = x.Id,
                             TargetId = x.TargetId,
                             State = x.State,
                             Type = x.Type,
                             Info = x.Info,
                         })
                         .OrderBy(x => x.State)
                            .ThenBy(x => x.Date)
                         .ToListAsync();
        }

        public void ChangeState(Guid id, NotificationStateEnum state)
        {
            _dbSet.Attach(new NotificationEntity
            {
                Id = id,
                State = state
            })
            .Property(x => x.State).IsModified = true;
        }
    }
}

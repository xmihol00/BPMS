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

        public Task<List<NotificationAllDTO>> All()
        {
            IQueryable<NotificationEntity> query = _dbSet.Where(x => x.UserId == UserId);

            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.NotificationMarked)] || Filters[((int)FilterTypeEnum.NotificationRead)])
                {
                    query = query.Where(x => x.State == NotificationStateEnum.Unread ||
                                            (Filters[((int)FilterTypeEnum.NotificationMarked)] && x.State == NotificationStateEnum.Marked) ||
                                            (Filters[((int)FilterTypeEnum.NotificationRead)] && x.State == NotificationStateEnum.Read));
                }
                else
                {
                    query = query.Where(x => x.State == NotificationStateEnum.Unread);
                }
            }

            return query.Select(x => new NotificationAllDTO
                        {
                            Date = x.Date,
                            Id = x.Id,
                            TargetId = x.TargetId,
                            State = x.State,
                            Type = x.Type,
                            Info = x.Info,
                        })
                        .OrderBy(x => x.State)
                           .ThenByDescending(x => x.Date)
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

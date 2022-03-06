using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class FilterEntity
    {
        public Guid UserId { get; set; }
        public UserEntity? User { get; set; }
        public FilterTypeEnum Filter { get; set; }
    }
}

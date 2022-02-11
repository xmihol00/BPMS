using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.ServiceDataSchema
{
    public interface IServiceDataSchema
    {
        public IEnumerable<ServiceDataSchema>? Children { get; set; }
        public Guid? ParentId { get; set; }
        public Guid Id { get; set; }
        public DataTypeEnum Type { get; set; }
    }
}

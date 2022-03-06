using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Task.IDataTypes
{
    public interface ITaskData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Compulsory { get; set; }
        public string? Description { get; set; }
        public string BlockName { get; set; }
    }
}

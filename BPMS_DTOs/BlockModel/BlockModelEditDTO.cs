using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel
{
    public class BlockModelEditDTO
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? RoleId { get; set; }
        public int Difficulty { get; set; }
    }
}

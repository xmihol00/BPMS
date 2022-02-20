using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockAttribute
{
    public class BlockAttributeAllDTO
    {
        public Guid Id { get; set; }
        public AttributeTypeEnum Type { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.ConnectionRequest
{
    public class LastConnectionRequestDTO
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
    }
}
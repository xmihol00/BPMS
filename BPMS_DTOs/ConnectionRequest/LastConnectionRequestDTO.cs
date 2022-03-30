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
        public string URL { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string? UserPhone { get; set; }
        public DateTime Date { get; set; }
    }
}

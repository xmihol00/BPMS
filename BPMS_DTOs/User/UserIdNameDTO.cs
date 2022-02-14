using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserIdNameDTO
    {
        public Guid? Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserOverviewDTO
    {
        public List<UserAllDTO> Users { get; set; } = new List<UserAllDTO>();
    }
}

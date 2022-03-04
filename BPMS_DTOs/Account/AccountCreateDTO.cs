using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DTOs.Account
{
    public class AccountCreateDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PasswordCheck { get; set; } = string.Empty;
        public string ReturnURL { get; set; } = string.Empty;
    }
}

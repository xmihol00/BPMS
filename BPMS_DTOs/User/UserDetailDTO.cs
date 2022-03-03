using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Task;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserDetailDTO : UserDetailPartialDTO
    {
        public List<UserAllDTO> OtherUsers { get; set; } = new List<UserAllDTO>();
        public UserAllDTO SelectedUser { get; set; } = new UserAllDTO();
    }
}

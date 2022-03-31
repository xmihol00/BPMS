
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.DataSchema;

namespace BPMS_BL.Helpers
{
    public static class RoleHelper
    {
        public static async Task AssignRole(LaneEntity lane, Guid agendaId, string poolName, AgendaRepository agendaRepository, 
                                             SolvingRoleRepository solvingRoleRepository, AgendaRoleRepository agendaRoleRepository)
        {
            string name = lane.Name ?? poolName;
            lane.RoleId = await agendaRepository.RoleByName(name, agendaId);
            if (lane.RoleId == Guid.Empty)
            {
                lane.RoleId = await solvingRoleRepository.RoleByName(name);
                if (lane.RoleId == Guid.Empty)
                {
                    lane.RoleId = null;
                    lane.Role = new SolvingRoleEntity
                    {
                        Name = name,
                        AgendaRoles = new List<AgendaRoleEntity>
                        {
                            new AgendaRoleEntity
                            {
                                AgendaId = agendaId
                            }
                        }
                    };
                }
                else
                {
                    await agendaRoleRepository.Create(new AgendaRoleEntity
                    {
                        AgendaId = agendaId,
                        RoleId = lane.RoleId.Value
                    });
                }
            }
            lane.Pool = null;
        }        
    }
}

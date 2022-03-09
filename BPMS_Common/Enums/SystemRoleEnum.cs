using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common.Enums
{
    public enum SystemRoleEnum
    {
        Admin,
        AgendaKeeper,
        WorkflowKeeper,
        ServiceKeeper,
        TaskSolver,
        ModelKeeper
    }

    public static class SystemRole
    {
        public static string ToLabel(this SystemRoleEnum value)
        {
            switch (value)
            {
                case SystemRoleEnum.Admin:
                    return "Administrátor";
                
                case SystemRoleEnum.AgendaKeeper:
                    return "Správce agendy";
                
                case SystemRoleEnum.ServiceKeeper:
                    return "Správce webových služeb";
                
                case SystemRoleEnum.TaskSolver:
                    return "Řešitel úkolů";

                case SystemRoleEnum.WorkflowKeeper:
                    return "Správce workflow";

                case SystemRoleEnum.ModelKeeper:
                    return "Správce modelů";
                
                default:
                    return "";
            }
        }
    }
}

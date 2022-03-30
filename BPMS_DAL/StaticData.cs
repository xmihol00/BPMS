using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL;
using BPMS_DAL.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS_DAL
{
    public static class StaticData
    {
        public static Guid ThisSystemId { get; set; }
        public static string ThisSystemURL { get; set; } = "";
        public static string FileStore { get; set; } = string.Empty;
        public static IServiceProvider? ServiceProvider { get; set; }

        public static void Load(BpmsDbContext? context)
        {
            SystemEntity system = context?.Systems?.First(x => x.State == SystemStateEnum.ThisSystem);
            ThisSystemId = system.Id;
            ThisSystemURL = system.URL;
        }
    }
}

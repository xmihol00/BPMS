using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS_Common
{
    public static class StaticData
    {
        public static Guid ThisSystemId = Guid.Parse("26ea2c26-f4c9-43b0-8607-f7de1dad9fcd");
        public static string ThisSystemURL = "";
        public static string FileStore { get; set; } = string.Empty;
        public static IServiceProvider? ServiceProvider { get; set; }
    }
}

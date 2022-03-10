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
        public static byte[] Key = Encoding.ASCII.GetBytes("2s5v8y/B?E(H+MbQeThVmYq3t6w9z$C&");
        public static byte[] IV = new byte[16] { 0x70, 0x15, 0x92, 0x40, 0x6a, 0xef, 0x68, 0x8c, 0x02, 0x1f, 0x5e, 0xc8, 0xeb, 0x0c, 0x6f, 0x35 };
        public static string FileStore { get; set; } = string.Empty;
        public static int KeySize = 256;
        public static int SaltSize = 32;
        public static IServiceProvider? ServiceProvider { get; set; }
    }
}

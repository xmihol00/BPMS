using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_Common
{
    public static class StaticData
    {
        public static Guid ThisSystemId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static string ThisSystemURL = "https://localhost:5001/";
        public static string SymetricKey = "!A%D*G-KaPdSgVkYp3s6v9y$B?E(H+MbQeThWmZq4t7w!z%C*F)J@NcRfUjXn2r5";
        public static byte[] IV = new byte[16] { 0x70, 0x15, 0x92, 0x40, 0x6a, 0xef, 0x68, 0x8c, 0x02, 0x1f, 0x5e, 0xc8, 0xeb, 0x0c, 0x6f, 0x35 };
    }
}


using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_BL.Helpers
{
    public static class CookieHelper
    {
        public static void SetCookie(this FilterTypeEnum value, bool removed, HttpResponse response)
        {
            if (removed)
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddHours(-1);
                response.Cookies.Append(value.ToString(), "0", options);
            }
            else
            {
                response.Cookies.Append(value.ToString(), "1");
            }
        }
    }
}

using Microsoft.EntityFrameworkCore;
using BPMS_DAL;
using BPMS_DTOs.Account;
using System;

namespace BPMS_Tests.Factories
{
    public static class AddressAuthFactory
    {
        /*public static AddressDTO CreateAuthDTO()
        {
            AddressDTO dto = new AddressDTO
            {
                Encryption = 
            }

            return dto;
        }*/

        private static AddressDTO CreateDTO()
        {
            return new AddressDTO
            {
                MessageId = Guid.Parse("5e250b64-ea22-4990-86d2-94d523b2e1b4"),
                SystemId = Guid.Parse("ab250bcd-ea22-4990-86d2-94d523b2e1b4"),
                
            };
        }
    }
}
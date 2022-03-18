using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMS_DAL.Seeds
{
    public static class SystemSeed
    {
        private static readonly List<SystemEntity> _systemSeeds = new List<SystemEntity>()
        {
            new SystemEntity()
            {
                Id = StaticData.ThisSystemId,
                Name = "Tento systém",
                Key = SymetricCipherHelper.EncryptKey().Result,
                URL = "https://localhost:5001/",
                State = SystemStateEnum.ThisSystem,
                Encryption = EncryptionLevelEnum.Encrypted,
            },
        };

        public static void SeedSystems(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemEntity>(entity => entity.HasData(_systemSeeds));
        }
    }
}

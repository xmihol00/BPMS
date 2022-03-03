using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMS_DAL.Seeds
{
    public static class SystemSeedTemplate
    {
        private static readonly List<SystemEntity> _systemSeeds = new List<SystemEntity>()
        {
            new SystemEntity()
            {
                Id = StaticData.ThisSystemId,
                Name = "Tento systém",
            },
            new SystemEntity()
            {
                Id = Guid.Parse("90074a51-95a3-48a9-be3a-93b8ad3109d6"),
                Name = "Test systém 1",
                Key = Encoding.ASCII.GetBytes("PeShVmYq3t6w9y$B&E)H@McQfTjWnZr4"),
                URL = "https://localhost:5010/"
            },
            new SystemEntity()
            {
                Id = Guid.Parse("ac0706e2-c282-49e0-99c7-5322e3235e62"),
                Name = "Test systém 2",
                Key = Encoding.ASCII.GetBytes("%D*F-JaNdRgUkXp2s5v8y/B?E(H+KbPe"),
                URL = "https://localhost:5011/"
            },
            new SystemEntity()
            {
                Id = Guid.Parse("87e156a0-6762-42b1-b67e-af105ed9a811"),
                Name = "Test systém 3",
                Key = Encoding.ASCII.GetBytes("eShVmYq3t6w9z$C&F)J@NcQfTjWnZr4u"),
                URL = "https://localhost:5012/"
            }
        };

        private static void SeedSystemsTemplate(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemEntity>(entity => entity.HasData(_systemSeeds));
        }
    }
}

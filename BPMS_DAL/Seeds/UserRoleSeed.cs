using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMS_DAL.Seeds
{
    public static class SystemRoleSeed
    {
        private static readonly List<SystemRoleEntity> _systemRoleSeeds = new List<SystemRoleEntity>()
        {
            new SystemRoleEntity()
            {
                UserId = Guid.Parse("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                Role = SystemRoleEnum.Admin
            },
            new SystemRoleEntity()
            {
                UserId = Guid.Parse("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                Role = SystemRoleEnum.AgendaKeeper
            },
            new SystemRoleEntity()
            {
                UserId = Guid.Parse("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                Role = SystemRoleEnum.WorkflowKeeper
            },
        };

        public static void SeedSystemRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemRoleEntity>(entity => entity.HasData(_systemRoleSeeds));
        }
    }
}

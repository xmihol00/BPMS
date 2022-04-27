using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMS_DAL.Seeds
{
    public static class UserSeed
    {
        private static readonly List<UserEntity> _userSeeds = new List<UserEntity>()
        {
            new UserEntity()
            {
                Id = Guid.Parse("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                Email = "admin.system@test.cz",
                Name = "Admin",
                Surname = "System",
                Title = "Ing.",
                UserName = "admin"
            }
        };

        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity => entity.HasData(_userSeeds));
        }
    }
}

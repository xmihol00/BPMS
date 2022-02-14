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
                UserName = "admin",
            },
            new UserEntity()
            {
                Id = Guid.Parse("442c2de7-eb92-44f9-acf1-41d5dade854a"),
                Email = "spravce.system@test.cz",
                Name = "Správce",
                Surname = "System",
                UserName = "spravce",
            },
            new UserEntity()
            {
                Id = Guid.Parse("6e250b64-ea22-4880-86d2-94d547b2e1b5"),
                Email = "karel@test.cz",
                Name = "Karel",
                Surname = "Stavitel",
                UserName = "kaja",
            },
            new UserEntity()
            {
                Id = Guid.Parse("342c2de7-eb92-44f9-acf1-41d5dade854b"),
                Email = "pavel@test.cz",
                Name = "Pavel",
                Surname = "Svoboda",
                UserName = "paja",
            }
        };

        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(entity => entity.HasData(_userSeeds));
        }
    }
}

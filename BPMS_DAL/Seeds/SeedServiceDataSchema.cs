using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMS_DAL.Seeds
{
    public static class ServiceDataSchemaSeed
    {
        private static readonly List<ServiceDataSchemaEntity> _serviceDataSchemaSeeds = new List<ServiceDataSchemaEntity>()
        {
            new ServiceDataSchemaEntity()
            {
                Id = Guid.Parse("ec2873d3-4806-40f6-b4a4-a35380ebd838"),
                Alias = "q",
                Compulsory = true,
                StaticData = null,
                Type = DataTypeEnum.String,
                Name = "Město",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-58513ade014a")
            },
            new ServiceDataSchemaEntity()
            {
                Id = Guid.Parse("ec2873d3-4806-40f6-b454-b35380ebd838"),
                Alias = "appid",
                Compulsory = true,
                StaticData = "7622a0a6b0f63a523986e6021e727f81",
                Type = DataTypeEnum.String,
                Name = "Klíč",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-58513ade014a")
            },
            new ServiceDataSchemaEntity()
            {
                Id = Guid.Parse("ec2873d3-4877-40f6-b4a4-b35380ebd838"),
                Alias = "lon",
                Compulsory = true,
                StaticData = null,
                Type = DataTypeEnum.Number,
                Name = "Zeměpisná délka",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("ec2873d3-4806-40f6-b4a4-b35380ebd828")
            },
            new ServiceDataSchemaEntity()
            {
                Id = Guid.Parse("ec2873d3-4806-40f6-b4a4-b45380ebd838"),
                Alias = "lat",
                Compulsory = true,
                StaticData = null,
                Type = DataTypeEnum.Number,
                Name = "Zeměpisná šířka",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("ec2873d3-4806-40f6-b4a4-b35380ebd828")
            },
            new ServiceDataSchemaEntity()
            {
                Id = Guid.Parse("ec2873d3-4806-40f6-b4a4-b35380ebd838"),
                Alias = "appid",
                Compulsory = true,
                StaticData = "7622a0a6b0f63a523986e6021e727f81",
                Type = DataTypeEnum.String,
                Name = "Klíč",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("ec2873d3-4806-40f6-b4a4-b35380ebd828")
            }
        };

        public static void SeedServiceDataSchemas(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceDataSchemaEntity>(entity => entity.HasData(_serviceDataSchemaSeeds));
        }
    }
}

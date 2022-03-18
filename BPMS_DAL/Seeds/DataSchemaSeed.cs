using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BPMS_DAL.Seeds
{
    public static class DataSchemaSeed
    {
        private static readonly List<DataSchemaEntity> _dataSchemaSeeds = new List<DataSchemaEntity>()
        {
            new DataSchemaEntity()
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
            new DataSchemaEntity()
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
            new DataSchemaEntity()
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
            new DataSchemaEntity()
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
            new DataSchemaEntity()
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
            },
            new DataSchemaEntity()
            {
                Id = Guid.Parse("ab2873d3-4806-40f6-b4a4-a35380ebd838"),
                Alias = "q",
                Compulsory = true,
                StaticData = null,
                Type = DataTypeEnum.String,
                Name = "Město",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade014a")
            },
            new DataSchemaEntity()
            {
                Id = Guid.Parse("ba2873d3-4806-40f6-b454-b35380ebd838"),
                Alias = "appid",
                Compulsory = true,
                StaticData = "7622a0a6b0f63a523986e6021e727f81",
                Type = DataTypeEnum.String,
                Name = "Klíč",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade014a")
            },
            new DataSchemaEntity()
            {
                Id = Guid.Parse("aab873d3-4806-40f6-b454-b35380ebd838"),
                Alias = "mode",
                Compulsory = true,
                StaticData = "xml",
                Type = DataTypeEnum.String,
                Name = "Mód",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade014a")
            },
            new DataSchemaEntity()
            {
                Id = Guid.Parse("aab873d3-4806-40f6-b454-b35380eb5566"),
                Alias = "ico",
                Compulsory = true,
                Type = DataTypeEnum.String,
                StaticData = null,
                Name = "IČO",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade1234")
            },
            new DataSchemaEntity()
            {
                Id = Guid.Parse("aab873d3-4806-40f6-b454-b35380eb6677"),
                Alias = "obchodni_firma",
                Compulsory = true,
                Type = DataTypeEnum.String,
                StaticData = null,
                Name = "Obchodní firma",
                Direction = DirectionEnum.Input,
                ParentId = null,
                ServiceId = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade4567")
            },
        };

        public static void SeedDataSchemas(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataSchemaEntity>(entity => entity.HasData(_dataSchemaSeeds));
        }
    }
}

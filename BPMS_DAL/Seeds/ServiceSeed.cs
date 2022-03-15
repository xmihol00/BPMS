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
    public static class ServiceSeed
    {
        private static readonly List<ServiceEntity> _serviceSeeds = new List<ServiceEntity>()
        {
            new ServiceEntity() {
                Description = "Počasí zadávané názvem města. Sereializace odpovědi v JSON.",
                Id = Guid.Parse("23bdf847-0e87-4eeb-92c3-58513ade014a"),
                HttpMethod = HttpMethodEnum.GET,
                Serialization = SerializationEnum.URL,
                Name = "Počasí - město (JSON)",
                Type = ServiceTypeEnum.REST,
                URL = "http://api.openweathermap.org/data/2.5/weather",
            },
            new ServiceEntity() {
                Description = "Počasí zadávané názvem zeměpisnou délkou a zeměpisnou šířkou.",
                Id = Guid.Parse("ec2873d3-4806-40f6-b4a4-b35380ebd828"),
                HttpMethod = HttpMethodEnum.GET,
                Serialization = SerializationEnum.URL,
                Name = "Počasí - zeměpisná délka a šířka",
                Type = ServiceTypeEnum.REST,
                URL = "http://api.openweathermap.org/data/2.5/weather",
            },
            new ServiceEntity() {
                Description = "Počasí zadávané názvem města. Sereializace odpovědi v XML.",
                Id = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade014a"),
                HttpMethod = HttpMethodEnum.GET,
                Serialization = SerializationEnum.URL,
                Name = "Počasí - město (XML)",
                Type = ServiceTypeEnum.REST,
                URL = "http://api.openweathermap.org/data/2.5/weather",
            },
            new ServiceEntity() {
                Description = "Ozvěna vstupu serializovaného do JSON    .",
                Id = Guid.Parse("23bdf844-8587-4ccb-92c3-66513ade014a"),
                HttpMethod = HttpMethodEnum.POST,
                Serialization = SerializationEnum.JSON,
                Name = "Echo API - JSON",
                Type = ServiceTypeEnum.REST,
                URL = "TODO",
                AuthType = ServiceAuthEnum.Basic,
                AppId = "UserName",
                AppSecret = "Password"
            },
            new ServiceEntity() {
                Description = "Ozvěna vstupu serializovaného do XML atributů.",
                Id = Guid.Parse("45adf844-8587-4bbb-92c3-66513ade014a"),
                HttpMethod = HttpMethodEnum.PUT,
                Serialization = SerializationEnum.XMLAttributes,
                Name = "Echo API - XML atributy",
                Type = ServiceTypeEnum.REST,
                URL = "TODO",
                AuthType = ServiceAuthEnum.Bearer,
                AppSecret = "AppSecret"
            },
            new ServiceEntity() {
                Description = "Ozvěna vstupu serializovaného do XML tagů.",
                Id = Guid.Parse("45adf844-8587-4ffb-92c3-66513ade014a"),
                HttpMethod = HttpMethodEnum.PUT,
                Serialization = SerializationEnum.XMLMarks,
                Name = "Echo API - XML tagy",
                Type = ServiceTypeEnum.REST,
                URL = "TODO",
            },
        };

        public static void SeedServices(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceEntity>(entity => entity.HasData(_serviceSeeds));
        }
    }
}

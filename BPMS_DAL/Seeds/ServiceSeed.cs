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
                Description = "Počasí zadávané názvem zeměpisnou délkou a zeměpisnou šířkou. Sereializace odpovědi v JSON.",
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
                Description = "Administrativní registr ekonomických subjektů - XML API. Vyhledávání pomocí IČO.",
                Id = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade1234"),
                HttpMethod = HttpMethodEnum.GET,
                Serialization = SerializationEnum.URL,
                Name = "ARES - IČO",
                Type = ServiceTypeEnum.REST,
                URL = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_std.cgi",
            },
            new ServiceEntity() {
                Description = "Administrativní registr ekonomických subjektů - XML API. Vyhledávání pomocí názvu obchodní firmy.",
                Id = Guid.Parse("23bdf847-0e87-4eeb-92c3-66513ade4567"),
                HttpMethod = HttpMethodEnum.GET,
                Serialization = SerializationEnum.URL,
                Name = "ARES - Obchodní firma",
                Type = ServiceTypeEnum.REST,
                URL = "http://wwwinfo.mfcr.cz/cgi-bin/ares/darv_std.cgi",
            },
        };

        public static void SeedServices(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ServiceEntity>(entity => entity.HasData(_serviceSeeds));
        }
    }
}

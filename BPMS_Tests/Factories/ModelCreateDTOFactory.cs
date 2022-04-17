using Microsoft.EntityFrameworkCore;
using BPMS_DAL;
using BPMS_DTOs.Model;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Mime;

namespace BPMS_Tests.Factories
{
    public static class ModelCreateDTOFactory
    {
        public static ModelCreateDTO Create(Guid agendaId, string name, string description, string bpmnFileName, string svgFileName)
        {
            ModelCreateDTO dto = new ModelCreateDTO
            {
                AgendaId = agendaId,
                Description = description,
                Name = name
            };

            FileStream bpmnStream = File.OpenRead("../../../../Models/" + bpmnFileName);
            dto.BPMN = new FormFile(bpmnStream, 0, bpmnStream.Length, "bpmnFile", "bpmnFile");
            
            FileStream svgStream = File.OpenRead("../../../../Models/" + svgFileName);
            dto.SVG = new FormFile(svgStream, 0, svgStream.Length, "svgFile", "svgFile");

            return dto;
        }
    }
}
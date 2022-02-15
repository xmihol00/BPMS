using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class PoolFacade
    {
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly IMapper _mapper;

        public PoolFacade(PoolRepository PoolRepository, SystemRepository systemRepository, IMapper mapper)
        {
            _poolRepository = PoolRepository;
            _systemRepository = systemRepository;
            _mapper = mapper;
        }

        public async Task Edit(PoolEditDTO dto)
        {
            PoolEntity entity = await _poolRepository.Detail(dto.Id);
            entity.Name = dto.Name;
            entity.Description = dto.Description ?? "";

            if (dto.SystemId != entity.SystemId)
            {
                XDocument svg = XDocument.Parse(entity.Model.SVG);
                XElement svgPool = svg.Descendants().First(x => x.Attribute("id")?.Value == dto.Id.ToString());
                XAttribute attribute = svgPool.Attribute("class");
                if (dto.SystemId == StaticData.ThisSystemId)
                {
                    attribute.SetValue("djs-group bpmn-pool bpmn-this-sys");
                }
                else
                {
                    attribute.SetValue("djs-group bpmn-pool");
                }
                entity.Model.SVG = svg.ToString();
            }

            entity.SystemId = dto.SystemId;

            await _poolRepository.Save();
        }

        public async Task<PoolConfigDTO> Config(Guid id)
        {
            PoolConfigDTO dto = await _poolRepository.Config(id);
            dto.Systems.Add(new SystemIdNameDTO
            {
                Id = null,
                Name = "Nevybrán"
            });
            dto.Systems.AddRange(await _systemRepository.IdNames());

            return dto;
        }
    }
}

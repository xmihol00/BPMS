using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class PoolFacade
    {
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly ModelRepository _modelRepository;
        private readonly IMapper _mapper;

        public PoolFacade(PoolRepository PoolRepository, SystemRepository systemRepository, ModelRepository modelRepository,
                          IMapper mapper)
        {
            _poolRepository = PoolRepository;
            _systemRepository = systemRepository;
            _modelRepository = modelRepository;
            _mapper = mapper;
        }

        public async Task<string> Edit(PoolEditDTO dto)
        {
            PoolEntity entity = await _poolRepository.DetailForEdit(dto.Id);
            entity.Name = dto.Name ?? "";
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
            entity.Model.State = ModelStateEnum.Shareable;
            ModelStateEnum state = ModelStateEnum.New;
            foreach (PoolEntity pool in entity.Model.Pools)
            {
                if (pool.SystemId == null)
                {
                    entity.Model.State = ModelStateEnum.New;
                }

                if (pool.SystemId == StaticData.ThisSystemId)
                {
                    if (state == ModelStateEnum.Shareable)
                    {
                        state = ModelStateEnum.Incorrect;
                        break;
                    }
                    else
                    {
                        state = ModelStateEnum.Shareable;
                    }
                }
            }

            entity.Model.State = state == ModelStateEnum.Shareable ? entity.Model.State : state;

            await _poolRepository.Save();
            return await _modelRepository.Svg(entity.ModelId);
        }

        public async Task<PoolConfigDTO> Config(Guid id)
        {
            PoolConfigDTO dto = await _poolRepository.Config(id);
            List<Guid?> assignedSystems = await _poolRepository.AssignedSystems(id);
            dto.Systems.Add(new SystemPickerDTO
            {
                Id = null,
                Name = "Nevybrán",
            });
            dto.Systems.AddRange(await _poolRepository.OfAgenda(id));
            dto.Systems.Add(await _systemRepository.ThisSystem());
            dto.Systems = dto.Systems.Where(x => !assignedSystems.Contains(x.Id)).ToList();

            return dto;
        }
    }
}

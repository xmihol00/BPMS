using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DTOs.System;
using BPMS_DTOs.User;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;

namespace BPMS_BL.Facades
{
    public class CommunicationFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly UserRepository _userRepository;
        private readonly FlowRepository _flowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly PoolRepository _poolRepository;
        private readonly SystemRepository _systemRepository;
        private readonly SystemAgendaRepository _systemAgendaRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly BlockAttributeRepository _blockAttributeRepository;
        private readonly ServiceDataSchemaRepository _serviceDataSchemaRepository;
        private readonly IMapper _mapper;

        public CommunicationFacade(UserRepository userRepository, ModelRepository modelRepository, FlowRepository flowRepository,
                                   BlockModelRepository blockModelRepository, PoolRepository poolRepository, SystemRepository systemRepository, 
                                   SystemAgendaRepository systemAgendaRepository, WorkflowRepository workflowRepository,
                                   AgendaRoleRepository agendaRoleRepository, BlockAttributeRepository blockAttributeRepository,
                                   ServiceDataSchemaRepository serviceDataSchemaRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _flowRepository = flowRepository;
            _blockModelRepository = blockModelRepository;
            _poolRepository = poolRepository;
            _systemRepository = systemRepository;
            _systemAgendaRepository = systemAgendaRepository;
            _workflowRepository = workflowRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _blockAttributeRepository = blockAttributeRepository;
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
            _mapper = mapper;
        }

        public async Task<string> RemoveRecieverAttribute(Guid id)
        {
            _blockAttributeRepository.Remove(new BlockAttributeEntity
            {
                Id = id
            });
            await _blockAttributeRepository.Save();

            return "";
        }

        public async Task<string> ToggleRecieverAttribute(BlockAttributeEntity attribute)
        {
            if (await _blockAttributeRepository.Any(attribute.Id))
            {
                _blockAttributeRepository.Remove(attribute);
            }
            else
            {
                await _blockAttributeRepository.Create(attribute);
            }

            await _blockAttributeRepository.Save();
            return "";
        }

        public async Task<string> ShareModel(ModelDetailShare dto)
        {
            if (await _modelRepository.Any(dto.Id))
            {
                return "";
            }

            ModelEntity model = _mapper.Map<ModelEntity>(dto);
            model.State = ModelStateEnum.Shared;
            XDocument svg = XDocument.Parse(dto.SVG, LoadOptions.PreserveWhitespace);
            
            List<Guid> targetAgendas = await _systemRepository.Agendas(dto.SenderURL);
            if (targetAgendas.Count == 0)
            {
                throw new Exception();
            }
            else if (targetAgendas.Count == 1)
            {
                model.AgendaId = targetAgendas.First();
            }
            else
            {
                model.AgendaId = null;
            }

            foreach (PoolShareDTO poolDTO in dto.Pools)
            {
                PoolEntity poolEntity = _mapper.Map<PoolEntity>(poolDTO);
                XElement element = svg.Descendants().First(x => x.Attribute("id")?.Value == poolEntity.Id.ToString());

                if (poolDTO.SystemURL == StaticData.ThisSystemURL)
                {
                    poolEntity.SystemId = StaticData.ThisSystemId;
                    element.Attribute("class").SetValue("djs-group bpmn-pool bpmn-this-sys");
                }
                else
                {
                    poolEntity.SystemId = await _systemRepository.IdFromUrl(poolDTO.SystemURL);
                    if (poolEntity.SystemId == Guid.Empty)
                    {
                        poolEntity.SystemId = null;
                    }

                    element.Attribute("class").SetValue("djs-group bpmn-pool");
                }

                await _poolRepository.Create(poolEntity);
            }

            model.SVG = svg.ToString(SaveOptions.DisableFormatting);
            await _modelRepository.Create(model);

            await _blockModelRepository.CreateRange(dto.EndEvents);
            await _blockModelRepository.CreateRange(dto.StartEvents);
            await _blockModelRepository.CreateRange(dto.UserTasks);
            await _blockModelRepository.CreateRange(dto.ServiceTasks);
            await _blockModelRepository.CreateRange(dto.ParallelGateways);
            await _blockModelRepository.CreateRange(dto.ExclusiveGateways);
            await _blockModelRepository.CreateRange(dto.SendEvents);
            await _blockModelRepository.CreateRange(dto.RecieveEvents);

            await _flowRepository.CreateRange(dto.Flows);
           
            await _modelRepository.Save();

            return "";
        }

        public async Task<string> IsModelRunable(ModelIdDTO dto)
        {
            if (!await _modelRepository.CheckState(dto.Id, ModelStateEnum.Waiting))
            {
                throw new NotImplementedException();
            }
            return "";
        }

        public async Task<string> RunModel(ModelIdDTO dto)
        {
            

            await new WorkflowHelper(_modelRepository, _workflowRepository, _agendaRoleRepository, 
                                     _blockAttributeRepository, _serviceDataSchemaRepository)
                                    .CreateWorkflow(dto.Id);

            await _modelRepository.Save();
            return "";   
        }

        public void AuthorizeSystem(string auth)
        {
            SystemUrlKeyDTO system = SymetricCypherHelper.JsonDecrypt<SystemUrlKeyDTO>(auth["Bearer ".Length..]);

            if (!_systemRepository.Authorize(system.URL, system.Key))
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}

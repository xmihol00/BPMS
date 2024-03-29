using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_Common;
using BPMS_DAL;
using BPMS_BL.Helpers;

namespace BPMS_BL.Facades
{
    public class ModelUploadFacade : BaseFacade
    {
        private readonly ModelRepository _modelRepository;
        private readonly PoolRepository _poolRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly LaneRepository _laneRepository;
        private readonly AgendaRepository _agendaRepository;
        private readonly SolvingRoleRepository _solvingRoleRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;

        private XDocument _svg = new XDocument();
        private Dictionary<string, BlockModelEntity> _blocksDict = new Dictionary<string, BlockModelEntity>();
        private Dictionary<string, PoolEntity>? _poolsDict = null;
        private PoolEntity _currentPool = new PoolEntity();
        private IEnumerable<(string? source, string? destination)> _messageFlows = new List<(string?, string?)>();
        private IEnumerable<BlockModelEntity> _startEvents = new List<BlockModelEntity>();
        private Dictionary<Guid, ExecutabilityStateEnum> _solvedGateways = new Dictionary<Guid, ExecutabilityStateEnum>();
        private ModelEntity _model = new ModelEntity();
        private uint _order = 0;

        public ModelUploadFacade(ModelRepository modelRepository, PoolRepository poolRepository, BlockModelRepository blockModelRepository,
                                 LaneRepository laneRepository, AgendaRepository agendaRepository, SolvingRoleRepository solvingRoleRepository, 
                                 AgendaRoleRepository agendaRoleRepository, FilterRepository filterRepository)
        : base(filterRepository)
        {
            _modelRepository = modelRepository;
            _poolRepository = poolRepository;
            _blockModelRepository = blockModelRepository;
            _laneRepository = laneRepository;
            _solvingRoleRepository = solvingRoleRepository;
            _agendaRepository = agendaRepository;
            _agendaRoleRepository = agendaRoleRepository;
        }

        public async Task<Guid> Upload(ModelCreateDTO dto)
        {
            XDocument bpmn = new XDocument();
            if (dto.BPMN != null)
            {
                try
                {
                    bpmn = XDocument.Load(new StreamReader(dto.BPMN.OpenReadStream()), LoadOptions.PreserveWhitespace);   
                }
                catch
                {
                    throw new ParsingException("BPMN soubor je ve špatném formátu.");
                }
            }
            else
            {
                throw new ParsingException("BPMN soubor nebyl nahrán.");
            }

            if (dto.SVG != null)
            {
                try
                {
                    _svg = XDocument.Load(new StreamReader(dto.SVG.OpenReadStream()), LoadOptions.PreserveWhitespace);
                }
                catch
                {
                    throw new ParsingException("SVG soubor je ve špatném formátu.");
                }
            }
            else
            {
                throw new ParsingException("SVG soubor nebyl nahrán.");
            }

            (XElement? collaboration, IEnumerable<XElement> processes) = ParseRoot(bpmn.Root);

            ParseCollaboration(collaboration);

            foreach (XElement process in processes)
            {
                (IEnumerable<XElement> blocks, IEnumerable<XElement> flows) = ParseProcess(process);
                foreach (XElement block in blocks)
                {
                    ParseBlock(block);
                }

                foreach (XElement flow in flows)
                {
                    ParseFlow(flow);
                }

                await ParseLane(process, dto.AgendaId);
            }

            foreach (var messageFlow in _messageFlows)
            {
                ParseMessageFlow(messageFlow.source, messageFlow.destination);
            }
            
            CheckExecutability();

            _svg.Root?.Attribute("width")?.Remove();
            _svg.Root?.Attribute("height")?.Remove();

            if (_poolsDict is not null)
            {
                List<PoolEntity> pools = _poolsDict.Select(x => x.Value).ToList();
                if (pools.Count == 1)
                {
                    _currentPool = pools[0];
                    _currentPool.SystemId = StaticData.ThisSystemId;
                    _model.State = ModelStateEnum.Executable;
                }
                
                foreach (PoolEntity pool in pools)
                {
                    await _poolRepository.Create(pool);
                }
            }
            else
            {
                _currentPool.SystemId = StaticData.ThisSystemId;
                _model.State = ModelStateEnum.Executable;
                await _poolRepository.Create(_currentPool);
            }

            foreach (BlockModelEntity blockModel in _blocksDict.Select(x => x.Value))
            {
                await _blockModelRepository.Create(blockModel);
            }

            if (_model.State == ModelStateEnum.Executable)
            {
                try
                {
                    XElement svgPool = _svg.Descendants().First(x => x.Attribute("id")?.Value == _currentPool.Id.ToString());
                    XAttribute attribute = svgPool.Attribute("class");
                    attribute.SetValue("djs-group bpmn-pool bpmn-this-sys");
                }
                catch
                {
                    throw new ParsingException("Model obsahuje elementy mimo element 'Pool'.");
                }
            }
            _model.Name = dto.Name;
            _model.SVG = _svg.ToString(SaveOptions.DisableFormatting);
            _model.AgendaId = dto.AgendaId;

            await _modelRepository.Create(_model);
            await _blockModelRepository.Save();
            return _model.Id;
        }

        private void ChangeSvg(string? elementId, Guid newId, string? className = null, bool assignParent = false)
        {
            try
            {
                XElement svgElement = _svg.Descendants().First(x => x.Attribute("data-element-id")?.Value == elementId);
                svgElement.Attribute("data-element-id")?.Remove();
                if (assignParent)
                {
                    svgElement = svgElement.Parent;
                }
                svgElement.SetAttributeValue("id", newId);

                if (className != null)
                {
                    var attribute = svgElement.Attribute("class");
                    string currentVal = attribute?.Value ?? "";
                    attribute?.SetValue(currentVal + " " + className);
                }
            }
            catch
            {
                throw new ParsingException("SVG soubor neodpovídá BPMN souboru.");
            }
        }

        private void RemoveSvgId(string elementId)
        {
            if (elementId == "")
            {
                throw new ParsingException("Soubor BPMN je poškozený, blok neobsauje ID.");
            }

            try
            {
                XElement svgElement = _svg.Descendants().First(x => x.Attribute("data-element-id")?.Value == elementId);
                svgElement.Attribute("data-element-id")?.Remove();
            }
            catch
            {
                throw new ParsingException("SVG soubor neodpovídá BPMN souboru.");
            }
        }

        private (XElement? collaboration, IEnumerable<XElement> processes) ParseRoot(XElement element)
        {
            return 
            (
                element.Elements().Where(x => x.Name.LocalName == "collaboration").FirstOrDefault(),
                element.Elements().Where(x => x.Name.LocalName == "process")
            );
        }

        private async Task ParseLane(XElement element, Guid agendaId)
        {
            XElement? laneSet = element.Elements().Where(x => x.Name.LocalName == "laneSet").FirstOrDefault();
            if (laneSet == null)
            {
                LaneEntity lane = new LaneEntity();
                lane.PoolId = _currentPool.Id;
                foreach (BlockModelEntity block in _blocksDict.Where(x => x.Value.PoolId == _currentPool.Id).Select(x => x.Value))
                {
                    block.LaneId = lane.Id;
                }

                await RoleHelper.AssignRole(lane, agendaId, _currentPool.Name, _agendaRepository, _solvingRoleRepository, _agendaRoleRepository);
                await _laneRepository.Create(lane);
            }
            else
            {
                foreach (XElement xLane in laneSet.Elements().Where(x => x.Name.LocalName == "lane"))
                {
                    LaneEntity lane = new LaneEntity();
                    lane.PoolId = _currentPool.Id;
                    lane.Name = xLane.Attribute("name")?.Value;
                    if (lane.Name == null)
                    {
                        throw new ParsingException("Model obsahuje nepojmenovanou dráhu.");
                    }
                    await RoleHelper.AssignRole(lane, agendaId, _currentPool.Name, _agendaRepository, _solvingRoleRepository, _agendaRoleRepository);

                    ChangeSvg(xLane.Attribute("id").Value, lane.Id, "bpmn-lane");

                    foreach(XElement block in xLane.Elements())
                    {
                        _blocksDict[block.Value].LaneId = lane.Id;
                    }

                    await _laneRepository.Create(lane);
                }
            }
        }

        private void ParseCollaboration(XElement? collaboration)
        {
            if (collaboration is null)
            {
                _currentPool.ModelId = _model.Id;
                return;
            }

            _poolsDict = new Dictionary<string, PoolEntity>();

            foreach (XElement element in collaboration.Elements().Where(x => x.Name.LocalName == "participant"))
            {
                PoolEntity pool = new PoolEntity(_model);
                pool.Name = element.Attribute("name")?.Value;
                if (pool.Name == null)
                {
                    throw new ParsingException("Model obsahuje nepojmenovaný bazén.");
                }
                ChangeSvg(element.Attribute("id")?.Value, pool.Id, "bpmn-pool bpmn-not-config", true);
                _poolsDict[element.Attribute("processRef")?.Value] = pool;
            }

            foreach (XElement element in collaboration.Elements().Where(x => x.Name.LocalName == "messageFlow"))
            {
                _messageFlows = _messageFlows.Append((element.Attribute("sourceRef")?.Value, element.Attribute("targetRef")?.Value));
            }
        }

        private (IEnumerable<XElement> blocks, IEnumerable<XElement> flows) ParseProcess(XElement process)
        {
            if (_poolsDict is not null)
            {
                _currentPool = _poolsDict[process.Attribute("id")?.Value ?? ""];
            }

            return
            (
                process.Elements().Where(x => x.Name.LocalName != "sequenceFlow"),
                process.Elements().Where(x => x.Name.LocalName == "sequenceFlow")
            );
        }

        private void ParseBlock(XElement block)
        {
            BlockModelEntity blockModel;
            switch (block.Name.LocalName)
            {
                case "startEvent":
                    if (!ParseSingleOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'Start Event' musí mít právě jeden odchozí řídící tok a žadné jiné.");
                    }
                    if (block.Elements().Any(x => x.Name.LocalName.Contains("Definition")))
                    {
                        throw new ParsingException("Nepodporaovaný BPMN prvek.");
                    }
                    blockModel = new StartEventModelEntity(_currentPool);
                    _startEvents = _startEvents.Append(blockModel);
                    break;

                case "endEvent":
                    if (!ParseSingleIncomingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'End Event' musí mít právě jeden příchozí řídící tok a žadné jiné.");
                    }
                    if (block.Elements().Any(x => x.Name.LocalName.Contains("Definition")))
                    {
                        throw new ParsingException("Nepodporaovaný BPMN prvek.");
                    }
                    blockModel = new EndEventModelEntity(_currentPool);
                    break;
                
                case "serviceTask":
                    if (!ParseSingleIncomingOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'Service Task' musí mít právě jeden příchozí a maximálně jeden odchozí řídící tok.");
                    }
                    blockModel = new ServiceTaskModelEntity(_currentPool);
                    break;

                case "userTask":
                    if (!ParseSingleIncomingOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'User Task' musí mít právě jeden příchozí a maximálně jeden maximálně jeden odchozí řídící tok.");
                    }
                    blockModel = new UserTaskModelEntity(_currentPool);
                    break;
                
                case "intermediateThrowEvent":
                    if (!ParseSingleIncomingOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'Intermediate Throw Event' musí mít právě jeden příchozí a maximálně jeden odchozí řídící tok.");
                    }

                    if (block.Elements().Any(x => x.Name.LocalName == "messageEventDefinition"))
                    {
                        blockModel = new SendMessageEventModelEntity(_currentPool);
                    }
                    else if (block.Elements().Any(x => x.Name.LocalName == "signalEventDefinition"))
                    {
                        blockModel = new SendSignalEventModelEntity(_currentPool);
                    }
                    else
                    {
                        goto default;
                    }
                    break;

                case "intermediateCatchEvent":
                    if (!ParseSingleIncomingOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'Intermediate Catch Event' musí mít právě jeden příchozí a maximálně jeden odchozí řídící tok.");
                    }

                    if (block.Elements().Any(x => x.Name.LocalName == "messageEventDefinition"))
                    {
                        blockModel = new RecieveMessageEventModelEntity(_currentPool);
                    }
                    else if (block.Elements().Any(x => x.Name.LocalName == "signalEventDefinition"))
                    {
                        blockModel = new RecieveSignalEventModelEntity(_currentPool);
                    }
                    else
                    {
                        goto default;
                    }
                    break;
                
                case "exclusiveGateway":
                    if (!ParseMultipleIncomingOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'Exclusive Gateway' musí mít alespoň jeden příchozí a odchozí řídící tok.");
                    }
                    blockModel = new ExclusiveGatewayModelEntity(_currentPool);
                    break;

                case "parallelGateway":
                    if (!ParseMultipleIncomingOutgoingBlock(block))
                    {
                        throw new ParsingException("Blok typu 'Parallel Gateway' musí mít alespoň jeden příchozí a odchozí řídící tok.");
                    }
                    blockModel = new ParallelGatewayModelEntity(_currentPool);
                    break;
                
                case "laneSet":
                    return;

                default:
                    throw new ParsingException("Nepodporaovaný BPMN prvek.");
            }

            blockModel.Name = block.Attribute("name")?.Value;
            if (blockModel.Name == null)
            {
                throw new ParsingException("Model obsahuje nepojmenovaný blok.");
            }
            string? id = block.Attribute("id")?.Value;
            ChangeSvg(id, blockModel.Id, "bpmn-block");
            _blocksDict[id] = blockModel; 
        }

        private bool ParseSingleIncomingBlock(XElement block)
        {
            return block.Elements().Where(x => x.Name.LocalName == "incoming").Count() == 1 &&
                   block.Elements().Where(x => x.Name.LocalName == "outgoing").Count() == 0;
        }

        private bool ParseSingleOutgoingBlock(XElement block)
        {
            return block.Elements().Where(x => x.Name.LocalName == "incoming").Count() == 0 &&
                   block.Elements().Where(x => x.Name.LocalName == "outgoing").Count() == 1;
        }

        private bool ParseSingleIncomingOutgoingBlock(XElement block)
        {
            return block.Elements().Where(x => x.Name.LocalName == "incoming").Count() == 1 &&
                   block.Elements().Where(x => x.Name.LocalName == "outgoing").Count() <= 1;
        }

        private bool ParseMultipleIncomingOutgoingBlock(XElement block)
        {
            return block.Elements().Where(x => x.Name.LocalName == "incoming").Count() > 0 &&
                   block.Elements().Where(x => x.Name.LocalName == "outgoing").Count() > 0;
        }

        private void ParseFlow(XElement flow)
        {
            RemoveSvgId(flow.Attribute("id")?.Value ?? "");

            BlockModelEntity outgoingBlock;
            BlockModelEntity incomingBlock;
            try
            {
                outgoingBlock = _blocksDict[flow.Attribute("sourceRef")?.Value ?? ""];
                incomingBlock = _blocksDict[flow.Attribute("targetRef")?.Value ?? ""];
            }
            catch
            {
                throw new ParsingException("Řídící tok spojující neexistující bloky.");
            }

            FlowEntity flowModel = new FlowEntity();
            flowModel.OutBlockId = outgoingBlock.Id;
            flowModel.OutBlock = outgoingBlock;
            flowModel.InBlockId = incomingBlock.Id;
            flowModel.InBlock = incomingBlock;

            outgoingBlock.OutFlows = outgoingBlock.OutFlows.Append(flowModel).ToList();
            incomingBlock.InFlows = incomingBlock.InFlows.Append(flowModel).ToList();
        }

        private void ParseMessageFlow(string? source, string? destination)
        {
            if (source is not null && destination is not null)    
            {
                BlockModelEntity? sourceBlock = _blocksDict[source];
                BlockModelEntity? destinationBlock = _blocksDict[destination];
                if (_blocksDict.TryGetValue(source, out sourceBlock) && _blocksDict.TryGetValue(destination, out destinationBlock) && 
                    sourceBlock is ISendMessageEventModelEntity && destinationBlock is IRecieveMessageEventModelEntity)
                {
                    (sourceBlock as ISendMessageEventModelEntity).RecieverId = destinationBlock.Id;
                    return;
                }
            }

            throw new ParsingException("Spojení typu 'Message Flow' musí spojovat blok typu 'Intermediate Message Throw Event' s blokem typu 'Intermediate Message Catch Event'.");
        }

        private void CheckExecutability()
        {   
            if (_blocksDict.Any(x => x.Value is ISendMessageEventModelEntity && (x.Value as ISendMessageEventModelEntity).RecieverId == Guid.Empty) ||
                !_blocksDict.Where(x => x.Value is IRecieveMessageEventModelEntity)
                            .All(x => _blocksDict.Any(y => y.Value is ISendMessageEventModelEntity && (y.Value as ISendMessageEventModelEntity).RecieverId == x.Value.Id)))
            {
                throw new ParsingException("Události typu 'Message Event' musí být spojeny elementem 'Message Flow'.");
            }

            foreach (BlockModelEntity branch in _startEvents)
            {
                if (CheckBranchExecutability(branch) != ExecutabilityStateEnum.Executable)
                {
                    throw new ParsingException("Větev není možné úspěšně vykonat.");
                }
            }
        }

        private ExecutabilityStateEnum CheckBranchExecutability(BlockModelEntity? branch)
        {
            while (branch?.OutFlows.Count == 1)
            {
                branch.Order = _order++;
                branch = branch.OutFlows[0].InBlock;   
            }

            if (branch?.OutFlows.Count > 1)
            {
                if (branch is IParallelGatewayModelEntity)
                {
                    return CheckParallelGatewayExecutability(branch);
                }
                else if (branch is IExclusiveGatewayModelEntity)
                {
                    return CheckExclusiveGatewayExecutability(branch);
                }
                else
                {
                    return ExecutabilityStateEnum.NotExecutable;
                }
            }
            else if (branch is IEndEventModelEntity)
            {
                branch.Order = _order++;
                return ExecutabilityStateEnum.Executable;
            }
            else
            {
                return ExecutabilityStateEnum.NotExecutable;
            }
        }

        private ExecutabilityStateEnum CheckExclusiveGatewayExecutability(BlockModelEntity gateway)
        {
            if (_solvedGateways.ContainsKey(gateway.Id))
            {
                return _solvedGateways[gateway.Id];
            }
            else
            {
                _solvedGateways[gateway.Id] = ExecutabilityStateEnum.Loop;
            }

            gateway.Order = _order++;
            ExecutabilityStateEnum result = ExecutabilityStateEnum.NotExecutable;
            foreach (BlockModelEntity? branch in gateway.OutFlows.Select(x => x.InBlock))
            {
                ExecutabilityStateEnum state = CheckBranchExecutability(branch);
                if (state == ExecutabilityStateEnum.NotExecutable)
                {
                    return ExecutabilityStateEnum.NotExecutable;
                }
                else if (state == ExecutabilityStateEnum.Executable)
                {
                    result = ExecutabilityStateEnum.Executable;
                }
            }

            _solvedGateways[gateway.Id] = result;

            return result;
        }

        private ExecutabilityStateEnum CheckParallelGatewayExecutability(BlockModelEntity gateway)
        {
            if (_solvedGateways.ContainsKey(gateway.Id))
            {
                return _solvedGateways[gateway.Id];
            }
            else
            {
                _solvedGateways[gateway.Id] = ExecutabilityStateEnum.Loop;
            }

            gateway.Order = _order++;
            foreach (BlockModelEntity? branch in gateway.OutFlows.Select(x => x.InBlock))
            {
                if (CheckBranchExecutability(branch) == ExecutabilityStateEnum.Executable)
                {
                    _solvedGateways[gateway.Id] = ExecutabilityStateEnum.Executable;
                    return ExecutabilityStateEnum.Executable;
                }
            }

            _solvedGateways[gateway.Id] = ExecutabilityStateEnum.NotExecutable;
            return ExecutabilityStateEnum.NotExecutable;
        }
    }
}

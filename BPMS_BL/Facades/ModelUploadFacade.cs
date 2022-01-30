using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;

namespace BPMS_BL.Facades
{
    public class ModelUploadFacade
    {
        private readonly AgendaRepository _agendaRepository;
        private XDocument _svg = new XDocument();
        private Dictionary<string, BlockModelEntity> _blocksDict = new Dictionary<string, BlockModelEntity>();
        private Dictionary<string, PoolEntity>? _poolsDict = null;
        private PoolEntity _currentPool = new PoolEntity();
        private List<(string? source, string? destination)> _messageFlows = new List<(string?, string?)>();

        public ModelUploadFacade(AgendaRepository agendaRepository)
        {
            _agendaRepository = agendaRepository;
        }

        public async Task Upload(CreateModelDTO dto)
        {
            XDocument bpmn = new XDocument();
            if (dto.BPMN is not null)
            {
                bpmn = XDocument.Load(new StreamReader(dto.BPMN.OpenReadStream()));   
            }
            else
            {
                return;
            }

            if (dto.SVG is not null)
            {
                _svg = XDocument.Load(new StreamReader(dto.SVG.OpenReadStream()));
            }
            else
            {
                return;
            }

            (XElement? collaboration, IEnumerable<XElement> processes) = ParseRoot(bpmn.Root ?? new XElement(""));

            ParseCollaboration(collaboration);

            foreach (XElement process in processes)
            {
                (IEnumerable<XElement> blocks, IEnumerable<XElement> flows) = ParseProcess(process);
                foreach (XElement block in blocks)
                {
                    ParseBlock(block);
                }
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

        private void ParseCollaboration(XElement? collaboration)
        {
            if (collaboration is null)
            {
                return;
            }

            _poolsDict = new Dictionary<string, PoolEntity>();

            foreach (XElement element in collaboration.Elements().Where(x => x.Name.LocalName == "participant"))
            {
                PoolEntity pool = new PoolEntity();
                pool.Name = ((string?)element.Attribute("name")) ?? "";
                _poolsDict[((string?)element.Attribute("processRef")) ?? ""] = pool;
            }

            foreach (XElement element in collaboration.Elements().Where(x => x.Name.LocalName == "messageFlow"))
            {
                _messageFlows.Append((((string?)element.Attribute("sourceRef")), ((string?)element.Attribute("targetRef"))));
            }
        }

        private (IEnumerable<XElement> blocks, IEnumerable<XElement> flows) ParseProcess(XElement process)
        {
            if (_poolsDict is not null)
            {
                _currentPool = _poolsDict[((string?)process.Attribute("id")) ?? ""];
            }

            return
            (
                process.Elements().Where(x => x.Name.LocalName != "sequenceFlow"),
                process.Elements().Where(x => x.Name.LocalName == "sequenceFlow")
            );
        }

        private void ParseBlock(XElement block)
        {

        }
    }
}

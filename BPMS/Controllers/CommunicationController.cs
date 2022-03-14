using System.Text.Json;
using BPMS_BL.Facades;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    public class CommunicationController : Controller
    {
        private readonly CommunicationFacade _communicationFacade;

        public CommunicationController(CommunicationFacade communicationFacade)
        {
            _communicationFacade = communicationFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            string action = context.RouteData.Values["action"].ToString();
             _communicationFacade.AuthorizeSystem(Request.Headers.Authorization, Request.Path, action, Request.Method);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ShareModel([FromBody] ModelDetailShare dto)
        {
            return Ok(await _communicationFacade.ShareModel(dto));
        }

        [HttpPost]
        public async Task<IActionResult> IsModelRunable([FromBody] WorkflowShare dto)
        {
            return Ok(await _communicationFacade.IsModelRunable(dto));
        }

        [HttpPost]
        public async Task<IActionResult> RunModel([FromBody] ModelIdWorkflowDTO dto)
        {
            return Ok(await _communicationFacade.RunModel(dto));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRecieverAttribute([FromBody] AttributeEntity attribute)
        {
            return Ok(await _communicationFacade.ToggleRecieverAttribute(attribute));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleForeignRecieverAttribute([FromBody] AttributeEntity attribute)
        {
            return Ok(await _communicationFacade.ToggleForeignRecieverAttribute(attribute));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRecieverAttribute(Guid id)
        {
            return Ok(await _communicationFacade.RemoveRecieverAttribute(id));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveForeignRecieverAttribute(Guid id)
        {
            return Ok(await _communicationFacade.RemoveForeignRecieverAttribute(id));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Message([FromBody] MessageShare message)
        {
            return Ok(await _communicationFacade.Message(message));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ForeignMessage([FromBody] MessageShare message)
        {
            return Ok(await _communicationFacade.ForeignMessage(message));
        }

        [HttpPost]
        public async Task<IActionResult> BlockActivity([FromBody] List<BlockWorkflowActivityDTO> blocks)
        {
            return Ok(await _communicationFacade.BlockActivity(blocks));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystem([FromBody] SystemEntity system)
        {
            return Ok(await _communicationFacade.CreateSystem(system));
        }

        [HttpPost]
        public async Task<IActionResult> ActivateSystem()
        {
            return Ok(await _communicationFacade.ActivateSystem());
        }

        [HttpGet] 
        public async Task<IActionResult> SenderInfo(Guid id)
        {
            return Ok(await _communicationFacade.SenderInfo(id));
        }
        
        [HttpGet] 
        public async Task<IActionResult> ForeignRecieverInfo(Guid id)
        {
            return Ok(await _communicationFacade.ForeignRecieverInfo(id));
        }

        [HttpGet] 
        public async Task<IActionResult> Agendas()
        {
            return Ok(await _communicationFacade.Agendas());
        }

        [HttpGet] 
        public async Task<IActionResult> Models(Guid id)
        {
            return Ok(await _communicationFacade.Models(id));
        }

        [HttpGet]
        public async Task<IActionResult> Pools(Guid id)
        {
            return Ok(await _communicationFacade.Pools(id));
        }

        [HttpGet]
        public async Task<IActionResult> SenderBlocks(Guid id)
        {
            return Ok(await _communicationFacade.SenderBlocks(id));
        }

        [HttpPost]
        [Route("/Communication/RemoveReciever/{foreignBlockId}/{senderId}")]
        public async Task<IActionResult> RemoveReciever(Guid foreignBlockId, Guid senderId)
        {
            return Ok(await _communicationFacade.RemoveReciever(foreignBlockId, senderId));
        }

        [HttpPost]
        [Route("/Communication/AddReciever/{foreignBlockId}/{senderId}")]
        public async Task<IActionResult> AddReciever(Guid foreignBlockId, Guid senderId)
        {
            return Ok(await _communicationFacade.AddReciever(foreignBlockId, senderId));
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateSystem([FromBody] string test)
        {
            return Ok(await _communicationFacade.DeactivateSystem());
        }

        [HttpPost]
        public async Task<IActionResult> ReactivateSystem([FromBody] ConnectionRequestEntity request)
        {
            return Ok(await _communicationFacade.ReactivateSystem(request));
        }
    }
}
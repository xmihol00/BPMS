using System.Text;
using System.Text.Json;
using BPMS_BL.Facades;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.Model;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BPMS.Controllers
{
    public class CommunicationController : Controller
    {
        private readonly CommunicationFacade _communicationFacade;
        private string _data { get; set; } = string.Empty;

        public CommunicationController(CommunicationFacade communicationFacade)
        {
            _communicationFacade = communicationFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            HttpContext.Request.EnableBuffering();
            string action = context.RouteData.Values["action"].ToString();
            _data = _communicationFacade.AuthorizeSystem(Request.Headers.Authorization, Request, action).Result;

            base.OnActionExecuting(context);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ShareModel()
        {
            return Ok(await _communicationFacade.ShareModel(JsonConvert.DeserializeObject<ModelDetailShare>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> IsModelRunable()
        {
            return Ok(await _communicationFacade.IsModelRunable(JsonConvert.DeserializeObject<WorkflowShare>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> RunModel()
        {
            return Ok(await _communicationFacade.RunModel(JsonConvert.DeserializeObject<ModelIdWorkflowDTO>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleRecieverAttribute()
        {
            return Ok(await _communicationFacade.ToggleRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleForeignRecieverAttribute()
        {
            return Ok(await _communicationFacade.ToggleForeignRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRecieverAttribute()
        {
            return Ok(await _communicationFacade.RemoveRecieverAttribute(JsonConvert.DeserializeObject<Guid>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveForeignRecieverAttribute()
        {
            return Ok(await _communicationFacade.RemoveForeignRecieverAttribute(JsonConvert.DeserializeObject<Guid>(_data)));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Message()
        {
            return Ok(await _communicationFacade.Message(JsonConvert.DeserializeObject<MessageShare>(_data)));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ForeignMessage()
        {
            return Ok(await _communicationFacade.ForeignMessage(JsonConvert.DeserializeObject<MessageShare>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> BlockActivity()
        {
            return Ok(await _communicationFacade.BlockActivity(JsonConvert.DeserializeObject<List<BlockWorkflowActivityDTO>>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystem()
        {
            return Ok(await _communicationFacade.CreateSystem(JsonConvert.DeserializeObject<SystemEntity>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> ActivateSystem()
        {
            return Ok(await _communicationFacade.ActivateSystem());
        }

        [HttpPut] 
        public async Task<IActionResult> SenderInfo()
        {
            return Ok(await _communicationFacade.SenderInfo(JsonConvert.DeserializeObject<Guid>(_data)));
        }
        
        [HttpPut] 
        public async Task<IActionResult> ForeignRecieverInfo()
        {
            return Ok(await _communicationFacade.ForeignRecieverInfo(JsonConvert.DeserializeObject<Guid>(_data)));
        }

        [HttpPut] 
        public async Task<IActionResult> Agendas()
        {
            return Ok(await _communicationFacade.Agendas());
        }

        [HttpPut] 
        public async Task<IActionResult> Models()
        {
            return Ok(await _communicationFacade.Models(JsonConvert.DeserializeObject<Guid>(_data)));
        }

        [HttpPut]
        public async Task<IActionResult> Pools()
        {
            return Ok(await _communicationFacade.Pools(JsonConvert.DeserializeObject<Guid>(_data)));
        }

        [HttpPut]
        public async Task<IActionResult> SenderBlocks()
        {
            return Ok(await _communicationFacade.SenderBlocks(JsonConvert.DeserializeObject<Guid>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveReciever()
        {
            return Ok(await _communicationFacade.RemoveReciever(JsonConvert.DeserializeObject<BlockIdSenderIdDTO>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> AddReciever()
        {
            return Ok(await _communicationFacade.AddReciever(JsonConvert.DeserializeObject<BlockIdSenderIdDTO>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateSystem()
        {
            return Ok(await _communicationFacade.DeactivateSystem());
        }

        [HttpPost]
        public async Task<IActionResult> ReactivateSystem()
        {
            return Ok(await _communicationFacade.ReactivateSystem(JsonConvert.DeserializeObject<ConnectionRequestEntity>(_data)));
        }
    }
}
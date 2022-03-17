using System.Text;
using System.Text.Json;
using BPMS_BL.Facades;
using BPMS_Common.Enums;
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
            await _communicationFacade.ShareModel(JsonConvert.DeserializeObject<ModelDetailShare>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> IsModelRunable()
        {
            await _communicationFacade.IsModelRunable(JsonConvert.DeserializeObject<WorkflowShare>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RunModel()
        {
            await _communicationFacade.RunModel(JsonConvert.DeserializeObject<ModelIdWorkflowDTO>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecieverAttribute()
        {
            await _communicationFacade.CreateRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateForeignRecieverAttribute()
        {
            await _communicationFacade.CreateForeignRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRecieverAttribute()
        {
            await _communicationFacade.UpdateRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateForeignRecieverAttribute()
        {
            await _communicationFacade.UpdateForeignRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRecieverAttribute()
        {
            await _communicationFacade.RemoveRecieverAttribute(JsonConvert.DeserializeObject<Guid>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveForeignRecieverAttribute()
        {
            await _communicationFacade.RemoveForeignRecieverAttribute(JsonConvert.DeserializeObject<Guid>(_data));
            return Ok();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Message()
        {
            await _communicationFacade.Message(JsonConvert.DeserializeObject<MessageShare>(_data));
            return Ok();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ForeignMessage()
        {
            await _communicationFacade.ForeignMessage(JsonConvert.DeserializeObject<MessageShare>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BlockActivity()
        {
            await _communicationFacade.BlockActivity(JsonConvert.DeserializeObject<List<BlockWorkflowActivityDTO>>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystem()
        {
            await _communicationFacade.CreateSystem(JsonConvert.DeserializeObject<SystemEntity>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateSystem()
        {
            await _communicationFacade.ActivateSystem();
            return Ok();
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
            await _communicationFacade.RemoveReciever(JsonConvert.DeserializeObject<BlockIdSenderIdDTO>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddReciever()
        {
            return Ok(await _communicationFacade.AddReciever(JsonConvert.DeserializeObject<BlockIdSenderIdDTO>(_data)));
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateSystem()
        {
            await _communicationFacade.DeactivateSystem();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ReactivateSystem()
        {
            await _communicationFacade.ReactivateSystem(JsonConvert.DeserializeObject<ConnectionRequestEntity>(_data));
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEncryption()
        {
            await _communicationFacade.ChangeEncryption(Enum.Parse<EncryptionLevelEnum>(_data));
            return Ok();
        }
    }
}
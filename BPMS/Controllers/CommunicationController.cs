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
            _data = _communicationFacade.AuthorizeSystem(Request.Headers.Authorization, Request, action, Response).Result;

            base.OnActionExecuting(context);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ShareModel()
        {
            return await _communicationFacade.ShareModel(JsonConvert.DeserializeObject<ModelDetailShare>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> IsModelRunable()
        {
            return await _communicationFacade.IsModelRunable(JsonConvert.DeserializeObject<WorkflowShare>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> RunModel()
        {
            return await _communicationFacade.RunModel(JsonConvert.DeserializeObject<ModelIdWorkflowDTO>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecieverAttribute()
        {
            return await _communicationFacade.CreateRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> CreateForeignRecieverAttribute()
        {
            return await _communicationFacade.CreateForeignRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRecieverAttribute()
        {
            return await _communicationFacade.UpdateRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateForeignRecieverAttribute()
        {
            return await _communicationFacade.UpdateForeignRecieverAttribute(JsonConvert.DeserializeObject<AttributeEntity>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRecieverAttribute()
        {
            return await _communicationFacade.RemoveRecieverAttribute(JsonConvert.DeserializeObject<Guid>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveForeignRecieverAttribute()
        {
            return await _communicationFacade.RemoveForeignRecieverAttribute(JsonConvert.DeserializeObject<Guid>(_data));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Message()
        {
            return await _communicationFacade.Message(JsonConvert.DeserializeObject<MessageShare>(_data));
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> ForeignMessage()
        {
            return await _communicationFacade.ForeignMessage(JsonConvert.DeserializeObject<MessageShare>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> BlockActivity()
        {
            return await _communicationFacade.BlockActivity(JsonConvert.DeserializeObject<List<BlockWorkflowActivityDTO>>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystem()
        {
            return await _communicationFacade.CreateSystem(JsonConvert.DeserializeObject<SystemEntity>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> ActivateSystem()
        {
            return await _communicationFacade.ActivateSystem();
        }

        [HttpPut] 
        public async Task<IActionResult> SenderInfo()
        {
            return await _communicationFacade.SenderInfo(JsonConvert.DeserializeObject<Guid>(_data));
        }
        
        [HttpPut] 
        public async Task<IActionResult> ForeignRecieverInfo()
        {
            return await _communicationFacade.ForeignRecieverInfo(JsonConvert.DeserializeObject<Guid>(_data));
        }

        [HttpPut] 
        public async Task<IActionResult> Agendas()
        {
            return await _communicationFacade.Agendas();
        }

        [HttpPut] 
        public async Task<IActionResult> Models()
        {
            return await _communicationFacade.Models(JsonConvert.DeserializeObject<Guid>(_data));
        }

        [HttpPut]
        public async Task<IActionResult> Pools()
        {
            return await _communicationFacade.Pools(JsonConvert.DeserializeObject<Guid>(_data));
        }

        [HttpPut]
        public async Task<IActionResult> SenderBlocks()
        {
            return await _communicationFacade.SenderBlocks(JsonConvert.DeserializeObject<Guid>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveReciever()
        {
            return await _communicationFacade.RemoveReciever(JsonConvert.DeserializeObject<BlockIdSenderIdDTO>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> AddReciever()
        {
            return await _communicationFacade.AddReciever(JsonConvert.DeserializeObject<BlockIdSenderIdDTO>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> DeactivateSystem()
        {
            return await _communicationFacade.DeactivateSystem();
        }

        [HttpPost]
        public async Task<IActionResult> ReactivateSystem()
        {
            return await _communicationFacade.ReactivateSystem(JsonConvert.DeserializeObject<ConnectionRequestEntity>(_data));
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEncryption()
        {
            return await _communicationFacade.ChangeEncryption(Enum.Parse<EncryptionLevelEnum>(_data));
        }
    }
}
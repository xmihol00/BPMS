using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_BL.Helpers
{
    public static class AuditMessageTextHelper
    {
        public static async Task CreateAuditMessage(AuditMessageRepository auditMessageRepository, Guid messageId, Guid systemId, string? action, string method)
        {
            AuditMessageEntity message = new AuditMessageEntity
            {
                Date = DateTime.Now,
                Text = "",
                Id = messageId,
                SystemId = systemId
            };

            switch (action)
            {
                case "ShareModel":
                    message.Text = "Sdílení modelu";
                    break;

                case "IsModelRunable":
                    message.Text = "Připravenost modelu";
                    break;

                case "RunModel":
                    message.Text = "Spuštění workflow";
                    break;

                case "ToggleRecieverAttribute":
                    message.Text = "Změna atributu příjemce";
                    break;

                case "ToggleForeignRecieverAttribute":
                    message.Text = "Změna atributu cizího příjemce";
                    break;

                case "RemoveRecieverAttribute":
                    message.Text = "Odstranění atributu příjemce";
                    break;

                case "RemoveForeignRecieverAttribute":
                    message.Text = "Odstranění atributu cizího příjemce";
                    break;

                case "Message":
                    message.Text = "Zaslání zprávy";
                    break;

                case "ForeignMessage":
                    message.Text = "Zaslání cizí zprávy";
                    break;

                case "BlockActivity":
                    message.Text = "Změna aktivity bloků";
                    break;

                case "CreateSystem":
                    return;

                case "ActivateSystem":
                    message.Text = "Aktivace systému";
                    break;

                case "SenderInfo":
                    message.Text = "Informace o odesílateli";
                    break;

                case "ForeignRecieverInfo":
                    message.Text = "Informace o příjemci";
                    break;

                case "Agendas":
                    message.Text = "Informace o agendách";
                    break;

                case "Models":
                    message.Text = "Informace o modelech";
                    break;

                case "Pools":
                    message.Text = "Informace o bazénech";
                    break;

                case "SenderBlocks":
                    message.Text = "Informace o odesílatelích";
                    break;

                case "RemoveReciever":
                    message.Text = "Odebrání příjemce";
                    break;

                case "AddReciever":
                    message.Text = "Přidání příjemce";
                    break;

                case "DeactivateSystem":
                    message.Text = "Deaktivace systému";
                    break;

                case "ReactivateSystem":
                    message.Text = "Reaktivace systému";
                    break;
            }

            await auditMessageRepository.Create(message);
            if (method == "get" || method == "GET")
            {
                await auditMessageRepository.Save();
            }
        }
    }
}

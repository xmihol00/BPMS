using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelInfoCardDTO : ModelDetailInfoDTO
    {
        public ModelAllDTO SelectedModel { get; set; } = new ModelAllDTO(); 
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelDetailDTO : ModelDetailPartialDTO
    {
        public List<ModelAllDTO> OtherModels { get; set; } = new List<ModelAllDTO>();
        public ModelAllDTO SelectedModel { get; set; } = new ModelAllDTO();
    }
}

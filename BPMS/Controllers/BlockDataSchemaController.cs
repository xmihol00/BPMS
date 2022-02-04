using BPMS_BL.Facades;
using BPMS_DTOs.BlockDataSchema;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class BlockDataSchemaController : Controller
    {
        private readonly BlockDataSchemaFacade _dataSchemaFacade;

        public BlockDataSchemaController(BlockDataSchemaFacade dataSchemaFacade)
        {
            _dataSchemaFacade = dataSchemaFacade;
        }
    }
}

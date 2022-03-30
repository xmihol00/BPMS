
var BlockId = "";
var PoolId = "";

window.addEventListener('DOMContentLoaded', () => 
{
    AddEventListeners();
});

function AddEventListeners(result = null)
{
    let model = document.querySelector("[id='WorkflowModelId']");
    if (model)
    {
        let workflowId = document.getElementById("WorkflowIdId").value;
        if (RoleForEdit && (!result || result.editable))
        {
            for (let pool of model.getElementsByClassName("bpmn-pool"))
            {
                if (pool.classList.contains("bpmn-this-sys"))
                {
                    for (let block of pool.getElementsByClassName("bpmn-block"))
                    {
                        block.addEventListener("click", (event) => ShowBlockDetail(event, workflowId, block.id));
                    }
                }
                else
                {
                    for (let block of pool.getElementsByClassName("bpmn-block"))
                    {
                        block.addEventListener("click", (event) => event.stopPropagation());
                    }
                }

                pool.addEventListener("click", () => ShowPoolDetail(pool.id));
            }
        }
        else
        {
            for (let pool of model.getElementsByClassName("bpmn-pool"))
            {
                pool.classList.remove("bpmn-this-sys");
                pool.classList.remove("bpmn-pool");
            }
        }
    }
}

function ShowBlockDetail(event, workflowId, blockId)
{
    event.stopPropagation();
    ShowModal("BlockConfigId", `/BlockWorkflow/Config/${blockId}/${workflowId}`, "BlockConfigTargetId", false, HideModelHeader)
    BlockId = blockId;
}

function ShowPoolDetail(poolId)
{
    ShowModal("PoolConfigId", "/Pool/Config/" + poolId, "PoolConfigTargetId", false, HideModelHeader)
    PoolId = poolId;
}
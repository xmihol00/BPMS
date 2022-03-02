var BlockId = "";
var PoolId = "";

window.addEventListener('DOMContentLoaded', () => 
{
    AddEventListeners();
});

function AddEventListeners()
{
    let model = document.querySelector("[id='WorkflowModelId']");
    if (model)
    {
        let workflowId = document.getElementById("WorkflowIdId").value;
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
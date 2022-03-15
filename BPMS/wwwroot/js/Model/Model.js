
var BlockId = "";
var PoolId = "";
var DragTarget = null;

window.addEventListener('DOMContentLoaded', () => 
{
    AddEventListeners();
    document.addEventListener("dragover", (event) => event.preventDefault());
    if (typeof activePools != "undefined")
    {
        DisplayActivePools(ActivePools);
    }
});

function DisplayActivePools(activePools)
{
    let model = document.querySelector(`[id=ModelSvgId]`);
    for (let blockId of activePools)
    {
        let block = model.querySelector(`[id='${blockId}']`);
        let target = block.children[0].children[0];
        target.style.stroke = "#5400db";
        target.style.fill = "#0054db";
    }
}

function AddEventListeners()
{
    let model = document.querySelector("[id='ModelSvgId']");
    if (model)
    {
        if (RoleForEdit)
        {
            for (let pool of model.getElementsByClassName("bpmn-pool"))
            {
                if (pool.classList.contains("bpmn-this-sys"))
                {
                    for (let block of pool.getElementsByClassName("bpmn-block"))
                    {
                        block.addEventListener("click", (event) => ShowBlockDetail(event, block.id));
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

function ShowBlockDetail(event, blockId)
{
    event.stopPropagation();
    ShowModal("BlockConfigId", "/BlockModel/Config/" + blockId, "BlockConfigTargetId", false, HideModelHeader)
    BlockId = blockId;
}

function ShowPoolDetail(poolId)
{
    ShowModal("PoolConfigId", "/Pool/Config/" + poolId, "PoolConfigTargetId", false, HideModelHeader)
    PoolId = poolId;
}

function ShowAddAttrib()
{
    document.getElementById("AttribCreateFormId").classList.remove("d-none");
    document.getElementById("NewAttBtnId").classList.add("d-none");
    document.getElementById("EditBtnId").classList.add("d-none");
    let senderBtn = document.getElementById("ChangeSenderBtnId");
    if (senderBtn)
    {
        senderBtn.classList.add("d-none");
    }
}

function CancelAddAttrib()
{
    document.getElementById("AttribCreateFormId").classList.add("d-none");
    document.getElementById("NewAttBtnId").classList.remove("d-none");
    document.getElementById("EditBtnId").classList.remove("d-none");
    let senderBtn = document.getElementById("ChangeSenderBtnId");
    if (senderBtn)
    {
        senderBtn.classList.remove("d-none");
    }
}

function EditAttribute(button)
{
    let oldForm = document.getElementById("AttribEitFormId");
    if (oldForm)
    {
        oldForm.nextSibling.classList.remove("d-none");
        oldForm.remove();
    }

    let element = button.parentNode.parentNode;
    let form = document.getElementById("AttribCreateFormId").cloneNode(true);
    form.id = "AttribEitFormId";

    form.querySelector("#IdId").value = element.id;
    form.querySelector("#HeadlineId").remove();
    form.querySelector("#NameId").value = element.children[1].innerText;
    let checked = element.children[3].children[0].classList.contains("bg-primary");
    form.querySelector("#CompulsoryId").checked = checked;
    form.querySelector("#DescrId").value = element.children[4].children[0] ? "" : element.children[4].innerText;
    for (let opt of form.querySelector("#TypeId").options)
    {
        if (opt.innerText == element.children[2].innerText)
        {
            opt.selected = true;
            if (opt.innerText == "soubor" || opt.innerText == "výběr")
            {
                MakeSpecEditable(opt.innerText == "soubor" ? "File" : "Selection", element.children[5].children[0], form.querySelector("#SpecDivId"));
            }
            else
            {
                form.querySelector("#SpecDivId").innerHTML = NoSpecInput();
            }
            break;
        }
        else
        {
            opt.selected = false;
        }
    }
    
    let submitBtn = form.querySelector("#CreateAttBtnId");
    submitBtn.id = "EditAttBtnId";
    submitBtn.setAttribute("form", "AttribEitFormId");
    submitBtn.disabled = false;

    let cancelBtn = form.querySelector("#CancelAttBtnId");
    cancelBtn.id = "";
    cancelBtn.setAttribute("onclick", "CancelEdit(this)");

    element.parentNode.insertBefore(form, element);
    element.classList.add("d-none");
    form.addEventListener("submit", (event) => AjaxFormSubmit(event, 'AttributesConfigId', false, false, null, CancelAddAttrib));
    form.classList.add("border-bottom");
    form.classList.remove("d-none");
    InputValidator(form);
}

function CancelEdit(element)
{
    let form = element.parentNode.parentNode;
    form.nextSibling.classList.remove("d-none");
    form.remove();
}

function AttribTypeChange(element)
{
    let specDiv = element.parentNode.parentNode.parentNode.children[4];
    if (element.value == "File" || element.value == "Select")
    {
        specDiv.innerHTML = CreateSpecInput(0, element.value) + AddSpecBtn(element.value);
        specDiv.children[0].children[2].remove();
    }
    else
    {
        specDiv.innerHTML = NoSpecInput();
    }

    InputValidator(specDiv.parentNode.parentNode);
}

function NoSpecInput()
{
    return `<div class="vertical-justify-center">
                Tento typ atributu nemá specifikaci.
            </div>`;
}

function AddSpecBtn(type)
{
    return `<div class="d-flex justify-content-center">
    <button class="butn btn-p mt-2 mb-2" type="button" onclick="AddSpecInput(this, '${type}')">Přidat další ${type == "File" ? "typ souboru" : "hodnotu výběru"}</button></div>`
}

function CreateSpecInput(index, type, value = "")
{
    return `<label class="input mt-3">
        <input required name="Specification[${index}]" class="input-field" type="text" placeholder=" " value="${value}" />
        <span id="S${index}LableId" class="input-label">${index + 1}. ${type == "File" ? "typ souboru" : "hodnota výběru"}</span>
        <button class="btn spec-remove" onclick="RemoveSpec(this, '${type}')" type="button"><i class="fas fa-times"></i></button>
    </label>`
}

function AddSpecInput(button, type)
{
    let specDiv = button.parentNode.parentNode;
    let index = specDiv.children.length - 1;
    let div = document.createElement("div");
    div.innerHTML = CreateSpecInput(index, type);
    specDiv.insertBefore(div.children[0], button.parentNode);
    
    if (specDiv.children.length - 1 == 2)
    {
        div.innerHTML = `<button class="btn spec-remove" onclick="RemoveSpec(this, '${type}')" type="button"><i class="fas fa-times"></i></button>`;
        specDiv.children[0].appendChild(div.children[0]);
    }

    InputValidator(specDiv.parentNode.parentNode);
}

function RemoveSpec(button, type)
{
    let specDiv = button.parentNode.parentNode;
    button.parentNode.remove();
    for (let i = 0; i < specDiv.children.length - 1; i++)
    {
        let inputBlock = specDiv.children[i];
        inputBlock.children[0].setAttribute("name", `Specification[${i}]`);
        inputBlock.children[1].innerText = `${i + 1}. ${type == "File" ? "typ souboru" : "hodnota výběru"}`
    }

    if (specDiv.children.length - 1 == 1)
    {
        specDiv.children[0].children[2].remove();
    }

    InputValidator(specDiv.parentNode.parentNode);
}

function MakeSpecEditable(type, list, target)
{
    let i = 0;
    target.innerHTML = "";
    for (let element of list.querySelectorAll("li"))
    {
        target.innerHTML += CreateSpecInput(i++, type, element.children[0].innerText);
    }

    if (i == 1)
    {
        target.children[0].children[2].remove();
    }

    target.innerHTML += AddSpecBtn(type);
}

function ToggleTaskMap(attribId, button)
{
    button.innerHTML = "<i class='fas fa-spinner'></i>"
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/ToggleTaskMap/${BlockId}/${attribId}`
    })
    .done(() => 
    {
        button.innerHTML = "<i class='fas fa-check-circle'></i>"
        if (button.classList.contains("bg-success"))
        {
            button.classList.remove("bg-success");
            button.classList.add("bg-secondary");
        }
        else
        {
            button.classList.add("bg-success");
            button.classList.remove("bg-secondary");
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function ToggleSendMap(attribId, button)
{
    button.innerHTML = "<i class='fas fa-spinner'></i>"
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/ToggleSendMap/${BlockId}/${attribId}`
    })
    .done((result) => 
    {
        button.innerHTML = "<i class='fas fa-check-circle'></i>"
        if (result)
        {
            if (button.classList.contains("bg-success"))
            {
                button.classList.remove("bg-success");
                button.classList.add("bg-secondary");
            }
            else
            {
                button.classList.add("bg-success");
                button.classList.remove("bg-secondary");
            }
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function ToggleServiceMap(button, dataSchemaId, serviceTaskId)
{
    button.innerHTML = "<i class='fas fa-spinner'></i>"
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/ToggleServiceMap/${BlockId}/${dataSchemaId}/${serviceTaskId}`
    })
    .done(() => 
    {
        button.innerHTML = "<i class='fas fa-check-circle'></i>"
        if (button.classList.contains("bg-success"))
        {
            button.classList.remove("bg-success");
            button.classList.add("bg-secondary");
        }
        else
        {
            button.classList.add("bg-success");
            button.classList.remove("bg-secondary");
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function RemoveAttribute(element)
{
    element.innerHTML = "<i class='fas fa-spinner'></i>";
    let attrib = element.parentNode.parentNode;
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/Remove/${attrib.id}`
    })
    .done(() => 
    {
        attrib.remove();
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function ShareModel()
{
    let modelId = document.getElementById("ModelIdId").value;

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Model/Share/${modelId}`
    })
    .done((result) => 
    {
        InfoCardUpdate(result);
        document.getElementById("PageNavId").innerHTML = result.header;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function ModelRunCB(result)
{   
    if (typeof(result) == "object")
    {
        PoolConfigCB(result);
    }
    else
    {
        window.location.replace("/Workflow/Detail/" + result);
    }
}

function RemoveModel()
{
    let modelId = document.getElementById("ModelIdId").value;

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Model/Remove/${modelId}`
    })
    .done(() => 
    {
        window.location.replace(`/Model/Overview/`);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });    
}

function ValidateStart()
{
    InputValidator(document.getElementById("RunModelId"));
}

function ShowSenderChange()
{
    document.getElementById("NewAttBtnId").classList.add("d-none");
    document.getElementById("EditBtnId").classList.add("d-none");
    document.getElementById("ChangeSenderBtnId").classList.add("d-none");   
}

function ChangeSender(id)
{
    let modelId = document.getElementById("ModelIdId").value;
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/ChangeSender/${modelId}/${id}`
    })
    .done((result) => 
    {
        let form = document.getElementById("SenderChangeFormId");
        form.innerHTML = result;
        form.classList.remove("d-none");
        ShowSenderChange();
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function SystemChange(select)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/Agendas/${select.value}`
    })
    .done((result) => 
    {
        document.getElementById("AgendaPickerId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });   
}

function AgendaChange(select)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/Models/${document.getElementById("SystemIdId").value}/${select.value}`
    })
    .done((result) => 
    {
        document.getElementById("ModelPickerId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });   
}

function ModelChange(select)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/Pools/${document.getElementById("SystemIdId").value}/${select.value}`
    })
    .done((result) => 
    {
        document.getElementById("PoolPickerId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });   
}

function PoolChange(select)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/SenderBlocks/${document.getElementById("SystemIdId").value}/${select.value}`
    })
    .done((result) => 
    {
        document.getElementById("BlockPickerId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });   
}

function SchemaDragStart(event)
{
    DragTarget = event.target;
    let type = DragTarget.getAttribute("data-type");

    for (let ele of document.getElementsByClassName("target-map"))
    {
        if (type == ele.getAttribute("data-type"))
        {
            ele.classList.add("map-ok");
        }
        else
        {
            ele.classList.add("map-bad");
        }
    }
}

function SchemaDragEnd()
{
    for (let ele of document.getElementsByClassName("target-map"))
    {
        ele.classList.remove("map-bad");
        ele.classList.remove("map-ok");
    }
}

function SchemaDragEnter(div)
{
    div.classList.add("map-drag-over");
}

function SchemaDragLeave(div)
{
    div.classList.remove("map-drag-over");
}

function SchemaDragDrop(event)
{
    if (event.target.getAttribute("data-type") != DragTarget.getAttribute("data-type"))
    {
        return;
    }

    let target = event.target;
    target.classList.remove("text-center");
    target.innerHTML = DragTarget.innerHTML;
    DragTarget.remove();

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/AddMap/${document.getElementById("BlockIdId").value}/${DragTarget.id}/${target.id}`
    })
    .done((result) => 
    {
        document.getElementById("ServiceMapConfigId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function RemoveMap(btn, serviceTaskId)
{
    let parent = btn.parentNode;

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/BlockModel/RemoveMap/${serviceTaskId}/${parent.children[0].id}/${parent.children[2].id}`
    })
    .done((result) => 
    {
        document.getElementById("ServiceMapConfigId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function PoolConfigCB(result)
{
    setTimeout(() => document.getElementById("PageNavId").innerHTML = result.header, 850);
    InfoCardUpdate(result);

    if (result.model)
    {
        document.getElementById("ModelSvgId").innerHTML = result.model;
        AddEventListeners();
    }
}

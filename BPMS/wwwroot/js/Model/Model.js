
var BlockId = "";
var PoolId = "";

window.addEventListener('DOMContentLoaded', () => 
{
    if (window.location.pathname.startsWith("/Model/Detail"))
    {
        ModelAddEventListeners();
    }
});

function ModelAddEventListeners()
{
    for (let pool of document.getElementsByClassName("bpmn-pool"))
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

function HideModelHeader()
{
    setTimeout(() => 
    {
        document.getElementById("BlockConfigTargetId").innerHTML = LoadingImageHtml();
    }, HideDelay);
}

function ShowAddAttrib()
{
    document.getElementById("AttribCreateFormId").classList.remove("d-none");
    document.getElementById("NewAttBtnId").classList.add("d-none");
    document.getElementById("EditBtnId").classList.add("d-none");
}

function CancelAddAttrib()
{
    document.getElementById("AttribCreateFormId").classList.add("d-none");

    document.getElementById("NewAttBtnId").classList.remove("d-none");
    document.getElementById("EditBtnId").classList.remove("d-none");
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
    form.querySelector("#DescrId").value = element.children[4].innerText;
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
    form.addEventListener("submit", (event) => AjaxFormSubmit(event, 'BlockConfigTargetId'));
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
    if (element.value == "File" || element.value == "Selection")
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
    <button class="butn btn-p mb-2" type="button" onclick="AddSpecInput(this, '${type}')">Přidat další ${type == "File" ? "typ souboru" : "hodnotu výběru"}</button></div>`
}

function CreateSpecInput(index, type, value = "")
{
    return `<label class="input mb-1">
        <input required name="Specification[${index}]" class="input-field" type="text" placeholder=" " value="${value}" oninput="" />
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

function ShareModel(btn)
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
        btn.innerText = "Spustit";
        btn.onclick = () => RunModel(btn);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function RunModel(btn)
{
    let modelId = document.getElementById("ModelIdId").value;

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Model/Run/${modelId}`
    })
    .done((result) => 
    {
        if (!result)
        {
            btn.innerText = "Spustit";
            btn.disabled = true;
            btn.setAttribute("title", "Vytvoření workflow je již spuštěno, čeká se na spolupracující systémy.")
            btn.removeAttribute("onclick");
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
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
    .done((result) => 
    {
        if (result)
        {
            window.location.replace(`/Agenda/Detail/${result}`);
        }
        else
        {
            window.location.replace(`/Agenda/Overview/`);
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });    
}


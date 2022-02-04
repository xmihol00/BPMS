
var LoadingImage = null;

window.addEventListener('DOMContentLoaded', () => 
{
    if (window.location.pathname.startsWith("/Model/Detail"))
    {
        LoadingImage = document.getElementById("BlockConfigTargetId").children[0];
        for (let ele of document.getElementsByClassName("bpmn-block"))
        {
            ele.addEventListener("click", () => ShowModal("BlockConfigId", "/BlockModel/Config/" + ele.id, 
                                 "BlockConfigTargetId", null, false, HideModelHeader));
        }
    }
});

function HideModelHeader()
{
    setTimeout(() => 
    {
        document.getElementById("BlockConfigTargetId").innerHTML = LoadingImage.innerHTML;
    }, 350);
}

function BlockConfigSubmit(event, targetId)
{
    document.getElementById("BlockConfigTargetId").innerHTML = LoadingImage.innerHTML;
    AjaxFormSubmit(event, targetId);
}

function ShowAddAttrib()
{
    document.getElementById("AttribCreateFormId").classList.remove("d-none");
    document.getElementById("CreateAttBtnId").classList.remove("d-none");
    document.getElementById("CancelAttBtnId").classList.remove("d-none");

    document.getElementById("NewAttBtnId").classList.add("d-none");
    document.getElementById("EditBtnId").classList.add("d-none");

    document.addEventListener("input", ValidateAttribute);
}

function CancelAddAttrib()
{
    document.getElementById("AttribCreateFormId").classList.add("d-none");
    document.getElementById("CreateAttBtnId").classList.add("d-none");
    document.getElementById("CancelAttBtnId").classList.add("d-none");

    document.getElementById("NewAttBtnId").classList.remove("d-none");
    document.getElementById("EditBtnId").classList.remove("d-none");

    document.removeEventListener("input", ValidateAttribute);
}

function ValidateAttribute()
{
    let disabled = false;
    let name = document.getElementById("AttribNameId");

    if (!name.value || !name.value.trim())
    {
        disabled = true;
        name.parentNode.children[1].classList.add("color-required");
    }
    else
    {
        name.parentNode.children[1].classList.remove("color-required");
    }

    let compusory = document.getElementById("CompulsoryId");

    if (compusory.checked)
    {
        compusory.parentNode.children[0].classList.add("label-checkbox-checked");
    }
    else
    {
        compusory.parentNode.children[0].classList.remove("label-checkbox-checked");
    }

    document.getElementById("CreateAttBtnId").disabled = disabled;
}

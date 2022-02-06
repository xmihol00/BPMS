
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
    document.getElementById(targetId).innerHTML = LoadingImage.innerHTML;
    AjaxFormSubmit(event, targetId);
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
        compusory.parentNode.children[0].classList
    }
    else
    {
        compusory.parentNode.children[0].classList.remove("label-checkbox-checked");
    }

    document.getElementById("CreateAttBtnId").disabled = disabled;
}

function EditAttribute(button)
{
    let oldForm = document.getElementById("AttribEitFormId");
    if (oldForm)
    {
        oldForm.nextSibling.classList.remove("d-none");
        oldForm.remove();
    }

    let element = button.parentNode;
    let form = document.getElementById("AttribCreateFormId").cloneNode(true);
    form.id = "AttribEitFormId";

    form.querySelector("#IdId").value = element.id;
    form.querySelector("#HeadlineId").innerText = "Editce atributu";
    form.querySelector("#NameId").value = element.children[1].innerText;
    form.querySelector("#AttribNameLabelId").classList.remove("color-required");
    let checked = element.children[3].children[0].classList.contains("bg-primary");
    form.querySelector("#CompulsoryId").checked = checked;
    if (checked)
    {
        form.querySelector("#CompulsoryLabelId").classList.add("label-checkbox-checked");
    }
    else
    {
        form.querySelector("#CompulsoryLabelId").classList.remove("label-checkbox-checked");
    }
    form.querySelector("#DescrId").value = element.children[4].innerText;
    for (let opt of form.querySelector("#TypeId").options)
    {
        if (opt.innerText == element.children[2].innerText)
        {
            opt.selected = true;
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
    form.classList.add("border-bottom");
    form.classList.remove("d-none");
}

function CancelEdit(element)
{
    let form = element.parentNode.parentNode;
    form.nextSibling.classList.remove("d-none");
    form.remove();
}
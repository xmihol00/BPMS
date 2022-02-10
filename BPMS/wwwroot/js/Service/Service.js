var ServiceId = "";
var HiddenButtons = null;

function ResetForm(form, direction, btn, parentId = "") 
{
    if (HiddenButtons)
    {
        HiddenButtons.classList.remove("d-none");
    }

    if (direction == "Input") 
    {
        document.getElementById("HeadlineId").innerText = "Tvorba vstupního atributu";
    }
    else 
    {
        document.getElementById("HeadlineId").innerText = "Tvorba výstupního atributu";
    }
    
    for (let select of form.getElementsByTagName("select")) 
    {
        select.options[0].selected = true;
    }
    
    document.getElementById("ParentId").value = parentId;
    document.getElementById("DirectionId").value = direction;
    document.getElementById("IdId").value = "";
    document.getElementById("CompulsoryId").checked = true;
    document.getElementById("NameId").value = "";
    document.getElementById("AliasId").value = "";
    document.getElementById("StaticDataId").classList.add("d-none");
    document.getElementById("DataChId").checked = false;
    document.getElementById("CrateEditBtnId").value = "Vytvořit";

    btn.parentNode.after(form);
    form.classList.remove("d-none");
    btn.classList.add("d-none");
    HiddenButtons = btn;
    form.onsubmit = (event) => DataSchemaSubmit(event, `${direction}SchemaId`);
    InputValidator(form);
}

function InputAttribAdd(btn)
{
    let form = document.getElementById("AttribServiceFormId");
    ResetForm(form, "Input", btn);
}

function AttribCancel()
{
    document.getElementById("AddInputBtnId").classList.remove("d-none");
    document.getElementById("AddOutputBtnId").classList.remove("d-none");
    document.getElementById("AttribServiceFormId").classList.add("d-none");
    if (HiddenButtons)
    {
        HiddenButtons.classList.remove("d-none");
    }
}

function OutputAttribAdd(btn)
{
    document.getElementById("HeadlineId").innerText = "Tvorba výstupního atributu";
    let form = document.getElementById("AttribServiceFormId");
    form.classList.remove("d-none");
    form.onsubmit = (event) => DataSchemaSubmit(event, 'InputSchemaId');
    btn.classList.add("d-none");
    InputValidator(form);
}

function DataSchemaSubmit(event, targetId)
{
    event.preventDefault();
    document.getElementById("AddOutputBtnId").classList.remove("d-none");
    document.getElementById("AddInputBtnId").classList.remove("d-none");
    if (HiddenButtons)
    {
        HiddenButtons.classList.remove("d-none");
    }
    event.target.classList.add("d-none");
    document.getElementById("CreateFormId").after(event.target);
    AjaxFormSubmit(event, targetId);
}

function ToggleStaticData(checkbox)
{
    let input = document.getElementById("StaticDataId");
    if (checkbox.checked)
    {
        input.classList.remove("d-none");
    }
    else
    {
        input.classList.add("d-none");
    }
}

function CreateNestedAtt(btn)
{
    let form = document.getElementById("AttribServiceFormId");
    let btnParent = btn.parentNode;
    let direction = btnParent.parentNode.parentNode.parentNode.getAttribute("data-direction");

    ResetForm(form, direction, btnParent, btnParent.id);
}

function EditAttribute(btn)
{
    if (HiddenButtons)
    {
        HiddenButtons.classList.remove("d-none");
    }

    let form = document.getElementById("AttribServiceFormId");
    let btnParent = btn.parentNode;
    let direction = btnParent.parentNode.parentNode.parentNode.getAttribute("data-direction");
    let type = btnParent.parentNode.getElementsByClassName("text-code")[0].innerText;
    let staticData = btn.getAttribute("data-data");
    
    document.getElementById("IdId").value = btnParent.id;
    document.getElementById("ParentId").value = btn.id;
    
    document.getElementById("DirectionId").value = direction;
    if (direction == "Input")
    {
        document.getElementById("HeadlineId").innerText = "Editace vstupního atributu";
    }
    else
    {
        document.getElementById("HeadlineId").innerText = "Editace výstupního atributu";
    }
    for (let select of form.getElementsByTagName("select"))
    {
        for (let i = 0; i < select.options.length; i++)
        {
            if (select.options[i].innerText == type)
            {
                select.options[i].selected = true;
                break;
            }
        }
    }
    
    let indputDiv = document.getElementById("StaticDataId");
    if (staticData)
    {
        indputDiv.classList.remove("d-none");
        indputDiv.children[0].value = staticData;
        document.getElementById("DataChId").checked = true;
    }
    else
    {
        indputDiv.classList.add("d-none");
        document.getElementById("DataChId").checked = false;
    }
    
    document.getElementById("CompulsoryId").checked = btnParent.children[0].classList.contains("bg-primary");
    let idAlias = btnParent.parentNode.children[0];
    document.getElementById("NameId").value = idAlias.children[0].innerText;
    document.getElementById("AliasId").value = idAlias.children[1].innerText.replace(/^\(|\)$/g, '');
    document.getElementById("CrateEditBtnId").value = "Uložit";
    
    btnParent.parentNode.after(form);
    form.onsubmit = (event) => DataSchemaSubmit(event, `${direction}SchemaId`);
    form.classList.remove("d-none");
    btnParent.classList.add("d-none");
    HiddenButtons = btnParent;
    InputValidator(form);
}

function RemoveAttribute(btn)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Service/RemoveSchema/${btn.parentNode.id}`
    })
    .done(() => 
    {
        btn.parentNode.parentNode.parentNode.remove();
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    }); 
}

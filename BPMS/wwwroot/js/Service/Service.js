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
        select.disabled = false;
    }

    form.classList.remove("mb-2");
    document.getElementById("ParentId").value = parentId;
    document.getElementById("DirectionId").value = direction;
    document.getElementById("IdId").value = "";
    document.getElementById("NameId").value = "";
    let alias = document.getElementById("AliasId");
    alias.value = "";
    alias.readOnly = false;
    document.getElementById("StaticDataId").classList.add("d-none");
    let compulsory = document.getElementById("CompulsoryId");
    compulsory.disabled = false;
    compulsory.checked = true;
    let dataCh = document.getElementById("DataChId");
    dataCh.checked = false;
    dataCh.disabled = false;
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
    ResetForm(form, "Input", btn.parentNode);
    form.classList.add("mb-2");
}

function AttribCancel()
{
    document.getElementById("AddInputBtnId").classList.remove("d-none");
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

    ResetForm(form, "Input", btnParent, btnParent.id);
}

function EditAtrribute(form, btn, disabled)
{
    if (HiddenButtons)
    {
        HiddenButtons.classList.remove("d-none");
    }
    
    let btnParent = btn.parentNode;
    let type = btnParent.parentNode.getElementsByClassName("text-code")[0].innerText;
    let staticData = btn.getAttribute("data-data");
    document.getElementById("IdId").value = btnParent.id;
    document.getElementById("ParentId").value = btn.id;    

    for (let select of form.getElementsByTagName("select"))
    {
        for (let i = 0; i < select.options.length; i++)
        {
            if (select.options[i].innerText == type)
            {
                select.options[i].selected = true;
                select.disabled = disabled;
                break;
            }
        }
    }
    
    let indputDiv = document.getElementById("StaticDataId");
    let dataCh = document.getElementById("DataChId");
    dataCh.disabled = disabled;
    if (staticData)
    {
        indputDiv.classList.remove("d-none");
        indputDiv.children[0].value = staticData;
        dataCh.checked = true;
    }
    else
    {
        indputDiv.classList.add("d-none");
        dataCh.checked = false;
    }
    
    let compulsory = document.getElementById("CompulsoryId");
    compulsory.disabled = disabled;
    compulsory.checked = btnParent.children[0].classList.contains("bg-primary");
    let idAlias = btnParent.parentNode.children[0];
    document.getElementById("NameId").value = idAlias.children[0].innerText;
    let alias = document.getElementById("AliasId");
    alias.value = idAlias.children[1].innerText.replace(/^\(|\)$/g, '');
    alias.readOnly = disabled;
    document.getElementById("CrateEditBtnId").value = "Uložit";
    
    btnParent.parentNode.after(form);
    form.classList.remove("d-none");
    btnParent.classList.add("d-none");
    HiddenButtons = btnParent;
    InputValidator(form);
}

function EditInputAttribute(btn)
{
    let form = document.getElementById("AttribServiceFormId");
    EditAtrribute(form, btn, false);
    document.getElementById("DirectionId").value = "Input";
    document.getElementById("HeadlineId").innerText = "Editace vstupního atributu";
    form.onsubmit = (event) => DataSchemaSubmit(event, 'InputSchemaId');
}

function EditOutputAttribute(btn)
{
    let form = document.getElementById("AttribServiceFormId");
    EditAtrribute(form, btn, true);
    document.getElementById("DirectionId").value = "Output";
    document.getElementById("HeadlineId").innerText = "Editace výtupního atributu";
    form.onsubmit = (event) => DataSchemaSubmit(event, 'OutputSchemaId');
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

function SetFormAction(input, action)
{
    document.getElementById(input.getAttribute("form")).setAttribute("action", action);
}

function TestService(btn, id)
{
    HiddenButtons = btn.parentNode;
    HiddenButtons.classList.add("d-none");
    GetAjaxRequest(`/Service/Test/${id}`, 'TestInputId');
}

function CancelServiceTest()
{
    if (HiddenButtons)
    {
        HiddenButtons.classList.remove("d-none");
        document.getElementById("TestInputId").innerHTML = "";
    }
}

function GenerateOutAttributes(btn)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: "/Service/GenerateAttributes/",
        data: { 
                RecievedData: document.getElementById("ResponseTextId").innerText,
                ServiceId: btn.getAttribute("data-id"),
                Serialization: btn.getAttribute("data-type")
             }
    })
    .done((result) => 
    {
        document.getElementById("OutputSchemaId").innerHTML = result;
        document.getElementById("InputSchemaId").scrollIntoView({ behavior: "smooth", block: "start" });
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });    
}

function ShowCreateHeaderModal()
{
    let form = document.getElementById("HeaderModalId");
    for (let input of form.getElementsByClassName("input-field"))
    {
        input.value = "";
    }
    InputValidator(form);
    ShowModal("HeaderModalId");
}

function RemoveHeader(btn)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: "/Service/RemoveHeader/" + btn.parentNode.id,
    })
    .done(() => 
    {
        btn.parentNode.parentNode.remove();
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    }); 
}

function ScrollGenerate()
{
    document.getElementById("GenerateBtnId").scrollIntoView();
}


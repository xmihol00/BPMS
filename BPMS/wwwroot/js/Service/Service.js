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
    document.getElementById("DataToogleId").classList.remove("d-none");
    document.getElementById("CompulsoryDivId").classList.remove("d-none");
    document.getElementById("DescriptionId").value = "";
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
    document.getElementById("EditFormId").after(event.target);
    AjaxFormSubmit(event, targetId);
}

function ToggleStaticData(checkbox)
{
    let input = document.getElementById("StaticDataId");
    let type = document.getElementById("SchemaTypeId");
    let compulsory = document.getElementById("CompulsoryDivId");
    if (checkbox.checked)
    {
        input.classList.remove("d-none");
        type.disabled = true;
        compulsory.classList.add("d-none");
    }
    else
    {
        input.classList.add("d-none");
        compulsory.classList.remove("d-none");
        type.disabled = false;
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
    document.getElementById("DescriptionId").value = btn.parentNode.parentNode.children[0].getAttribute("data-title");
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
   
    let select = document.getElementById("SchemaTypeId");
    staticDataDiv = document.getElementById("DataToogleId");
    compulsoryDiv = document.getElementById("CompulsoryDivId");
    if (select.value == "Object" || select.value.startsWith("Array"))
    {
        staticDataDiv.classList.add("d-none");
        compulsoryDiv.classList.add("d-none");
    }
    else
    {
        staticDataDiv.classList.remove("d-none");
        compulsoryDiv.classList.remove("d-none");
    }
    
    let indputDiv = document.getElementById("StaticDataId");
    let dataCh = document.getElementById("DataChId");
    dataCh.disabled = disabled;
    if (staticData)
    {
        indputDiv.classList.remove("d-none");
        indputDiv.children[0].value = staticData;
        dataCh.checked = true;
        document.getElementById("CompulsoryDivId").classList.add("d-none");
    }
    else
    {
        indputDiv.classList.add("d-none");
        document.getElementById("CompulsoryDivId").classList.remove("d-none");
        dataCh.checked = false;
    }
    
    let compulsory = document.getElementById("CompulsoryId");
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
        url: `/Service/RemoveSchema/${btn.parentNode.id}`,
        statusCode: { 401: HandleRedirect }
    })
    .done(() => 
    {
        btn.parentNode.parentNode.parentNode.remove();
    })
    .fail((result) => 
    {
        ErrorAlert(result);
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
             },
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        document.getElementById("OutputSchemaId").innerHTML = result;
        document.getElementById("InputSchemaId").scrollIntoView({ behavior: "smooth", block: "end" });
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });    
}

function ShowCreateHeaderModal()
{
    document.getElementById("HeaderHeadId").innerText = "Tvorba hlavičky webové služby";
    document.getElementById("HeaderId").value = null;
    document.getElementById("CreateEditBtnId").innerText = "Vytvořit";
    document.getElementById("HeaderNameId").value = "";
    document.getElementById("HeaderValueId").value = "";

    InputValidator(document.getElementById("HeaderFormId"));
    ShowModal("HeaderFormId");
}

function EditHeader(btn)
{
    let row = btn.parentNode.parentNode;
    document.getElementById("HeaderHeadId").innerText = "Editace hlavičky webové služby";
    document.getElementById("HeaderId").value = row.id;
    document.getElementById("HeaderNameId").value = row.children[0].innerText;
    document.getElementById("HeaderValueId").value = row.children[1].innerText;
    document.getElementById("CreateEditBtnId").innerText = "Upravit";
    InputValidator(document.getElementById("HeaderFormId"));
    ShowModal("HeaderFormId");
}

function RemoveHeader(btn)
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: "/Service/RemoveHeader/" + btn.parentNode.id,
        statusCode: { 401: HandleRedirect }
    })
    .done(() => 
    {
        btn.parentNode.parentNode.remove();
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    }); 
}

function ScrollGenerate()
{
    document.getElementById("GenerateBtnId").scrollIntoView();
}

function AuthChange(select)
{
    if (select.value == "None")
    {
        document.getElementById("AppIdId").classList.add("d-none");
        document.getElementById("AppSecretId").classList.add("d-none");
    }
    else if (select.value == "Basic")
    {
        document.getElementById("AppIdId").classList.remove("d-none");
        let secret = document.getElementById("AppSecretId");
        secret.classList.remove("d-none");
        secret.classList.add("col-md-6");
    }
    else
    {
        document.getElementById("AppIdId").classList.add("d-none");
        let secret = document.getElementById("AppSecretId");
        secret.classList.remove("d-none");
        secret.classList.remove("col-md-6");
    }
}

function SchemaTypeChange(select)
{
    let staticData = document.getElementById("DataToogleId");
    let compulsory = document.getElementById("CompulsoryDivId");
    if (select.value == "Object" || select.value.startsWith("Array"))
    {
        staticData.classList.add("d-none");
        compulsory.classList.add("d-none");
    }
    else
    {
        staticData.classList.remove("d-none");
        compulsory.classList.remove("d-none");
    }
}

function GenerateArrays(form)
{
    for (let array of form.querySelectorAll("[name='array']"))
    {
        i = 0;
        for (let input of array.children)
        {

            input.children[0].name = `${array.id}_${++i}`;
        }
    }
}

function AddArrayInput(btn, type)
{
    let target = btn.parentNode.children[3];
    let tmp = document.createElement("div");
    tmp.innerHTML = CreateArrayInput(target.children.length + 1, type, btn.parentNode.children[1].innerText);
    target.appendChild(tmp.children[0]);
}

function CreateArrayInput(index, type, text)
{
    if (type == "ArrayString")
    {
        type = "text";
    }
    else if (type == "ArrayNumber")
    {
        type = "number"
    }
    else
    {
        if (type == "ArrayBool")
        {
            return `<label class="input mt-3">
                        <select class="input-select"> 
                            <option value="true">ano (true)</option>
                            <option value="false">ne (false)</option>
                        </select>
                        <span class="input-label">${text} - ${index}. hodnota</span>
                        <button class="btn array-remove" onclick="RemoveArrayInput(this)" type="button"><i class="fas fa-times"></i></button>
                    </label>`
        }
        return "";
    }

    return `<label class="input mt-3">
                <input class="input-field" type="${type}" placeholder=" "/>
                <span class="input-label">${text} - ${index}. hodnota</span>
                <button class="btn array-remove" onclick="RemoveArrayInput(this)" type="button"><i class="fas fa-times"></i></button>
            </label>`
}

function RemoveArrayInput(btn)
{
    let parent = btn.parentNode;
    let parentParent = parent.parentNode;
    let name = parentParent.parentNode.children[1].innerText;
    i = 0;
    parent.remove();
    for (let input of parentParent.children)
    {
        input.children[1].innerText = `${name} - ${++i}. hodnota`;
    }
}

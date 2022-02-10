var ServiceId = "";
var HiddenButtons = null;

function InputAttribAdd(btn)
{
    document.getElementById("HeadlineId").innerText = "Tvorba vstupního atributu";
    document.getElementById("DirectionId").value = "Input";
    let form = document.getElementById("AttribServiceFormId");
    form.classList.remove("d-none");
    btn.classList.add("d-none");
    btn.parentNode.after(form);
    form.onsubmit = (event) => DataSchemaSubmit(event, 'InputSchemaId');
    InputValidator(form);
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

    document.getElementById("ParentId").value = btnParent.id;
    if (direction == "Input")
    {
        document.getElementById("HeadlineId").innerText = "Tvorba vnořeného vstupního atributu";
    }
    else
    {
        document.getElementById("HeadlineId").innerText = "Tvorba vnořeného výstupního atributu";
    }
    document.getElementById("DirectionId").value = direction;
    for (let select of form.getElementsByTagName("select"))
    {
        select.options[0].selected = true;
    }
    
    btnParent.parentNode.after(form);
    form.onsubmit = (event) => DataSchemaSubmit(event, 'InputSchemaId');
    form.classList.remove("d-none");
    btnParent.classList.add("d-none");
    HiddenButtons = btnParent;
}

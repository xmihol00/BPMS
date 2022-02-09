
function InputAttribAdd()
{
    document.getElementById("HeadlineId").innerText = "Tvorba vstupního atributu";
    document.getElementById("AttribServiceFormId").classList.remove("d-none");
}

function OutputAttribAdd()
{
    document.getElementById("HeadlineId").innerText = "Tvorba výstupního atributu";
}

function DataSchemaSubmit(event, targetId)
{
    event.preventDefault();
    document.getElementById(targetId).innerHTML = LoadingImage.innerHTML;
    AjaxFormSubmit(event, targetId);
}

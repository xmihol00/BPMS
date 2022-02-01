
function ValidateAgendaCreate()
{
    let value = document.getElementById("AgendaNameId").value;
    let disabled = false;

    if (!value || !value.trim())
    {
        document.getElementById("AgendaNameLabelId").classList.add("color-required");
        disabled = true;
    }
    else
    {
        document.getElementById("AgendaNameLabelId").classList.remove("color-required");
    }

    document.getElementById("CreateBtnId").disabled = disabled;
}

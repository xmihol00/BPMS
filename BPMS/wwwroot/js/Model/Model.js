
function BlockConfig(id)
{
    ShowModal("BlockConfigId", "/BlockModel/Config/" + id, "BlockConfigTargetId", null, false);
}

function ShowAddAttrib()
{
    document.getElementById("NewAttribRowId").classList.remove("d-none");
    document.getElementById("CreateAttId").classList.remove("d-none");
    document.getElementById("CancelAttId").classList.remove("d-none");

    document.getElementById("NewBlockId").classList.add("d-none");
    document.getElementById("CreateBtnId").classList.add("d-none");
}

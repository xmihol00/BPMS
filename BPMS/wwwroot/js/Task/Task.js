
window.addEventListener('DOMContentLoaded', () => 
{
    if (document.location.pathname.startsWith("/Task/UserDetail/"))
    {
        InputValidator(document.getElementById("TaskDataFormId"));
    }
});

function SaveTaskData()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/SaveData");
    AjaxFormSubmit(form, "DetailDivId");
}

function SolveTask()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/SolveUserTask");
}

function RemoveFile(btn)
{
    let parent = btn.parentNode.parentNode.parentNode;
    let fileLabel = parent.children[1];
    fileLabel.classList.remove("d-none");
    fileLabel.children[0].name = parent.id;
    fileLabel.setAttribute("data-title", parent.children[0].getAttribute("data-title"));
    parent.children[0].remove();
}

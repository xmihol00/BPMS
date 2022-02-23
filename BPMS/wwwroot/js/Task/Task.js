
window.addEventListener('DOMContentLoaded', () => 
{
    InputValidator(document.getElementById("TaskDataFormId"));
});

function SaveTaskData()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/SaveData");
    AjaxFormSubmit(form);
}

function SolveTask()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/Solve");
}

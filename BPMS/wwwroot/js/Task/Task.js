
window.addEventListener('DOMContentLoaded', () => 
{  
    ValidateTask();
});

function ValidateTask()
{
    let form = document.getElementById("TaskDataFormId")
    if (form)
    {
        InputValidator(form);
    }
    form = document.getElementById("SeviceCallFormId");
    if (form)
    {
        InputValidator(form);
    }
}

function SaveUserTask()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/SaveUserTask");
    AjaxFormSubmit(form, "DetailDivId");
}

function SaveServiceTask()
{
    document.getElementById("TaskIdId").remove();
    let formData1 = new FormData(document.getElementById("SeviceCallFormId"));
    let formData2 = new FormData(document.getElementById("TaskDataFormId"));
    for (var pair of formData2.entries()) 
    {
        formData1.append(pair[0], pair[1]);
    }

    $.ajax(
    {
        async: true,
        type: "POST",
        url: "/Task/SaveServiceTask",
        data: formData1,
        contentType: false,
        processData: false,
    })
    .done((result) => 
    {
        document.getElementById("DetailDivId").innerHTML = result;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function SolveUserTask()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/SolveUserTask");
}

function SolveServiceTask()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/SolveServiceTask");
}

function CallService()
{
    let form = document.getElementById("TaskDataFormId");
    form.setAttribute("action", "/Task/CallService");
    AjaxFormSubmit(form, "DetailDivId");
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

function AddArrayInput(btn, type)
{
    let target = btn.parentNode.parentNode.children[1];

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Task/AddToArray/${target.id}/${type}`,
    })
    .done((result) => 
    {
        let tmp = document.createElement("div");
        tmp.innerHTML = result;
        tmp.children[0].children[0].children[1].innerText += ` - ${target.children.length + 1}. hodnota`;
        target.appendChild(tmp.children[0]);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    }); 
}


function ValidateServiceCall()
{
    InputValidator(document.getElementById("TaskDataFormId"));
}

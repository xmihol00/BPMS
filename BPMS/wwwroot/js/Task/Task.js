
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

function TaskDetail(element)
{
    for (let card of document.getElementsByClassName("selected-card"))
    {
        card.classList.remove("selected-card");
    }

    let detailDiv = document.getElementById("DetailDivId");
    let topEle = element.parentNode.parentNode;
    let overviewDiv = document.getElementById("OverviewNavId");
    topEle.classList.add("side-overview");
    detailDiv.classList.add("container-lg");
    detailDiv.classList.remove("d-none");
    if (overviewDiv)
    {
        overviewDiv.classList.add("overview-nav-hide");
    }
    
    element.children[0].classList.add("selected-card");
    element.parentElement.prepend(element);

    $.ajax(
    {
        async: true,
        type: "GET",
        url: "/Task/UserDetailPartial/" + element.id
    })
    .done((result) => 
    {
        document.getElementById("PageNavId").innerHTML = result.header;
        detailDiv.innerHTML = result.detail;
        window.history.pushState({}, '', "/Task/UserDetail/" + element.id);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}
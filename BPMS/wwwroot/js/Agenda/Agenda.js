
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

function ValidateModelUplaod()
{
    let disable = false;
    let name = document.getElementById("ModelNameId");
    let bpmnFile = document.getElementById("BpmnFileId");
    let svgFile = document.getElementById("SvgFileId");
    let btn = document.getElementById("UploadModelBtnId");

    
    if (!name.value || !name.value.trim())
    {
        disable = true;
        name.parentElement.children[1].classList.add("color-required");
    }
    else
    {
        name.parentElement.children[1].classList.remove("color-required");
    }

    if (!bpmnFile.files[0])
    {
        disable = true;
        bpmnFile.parentElement.children[1].classList.add("color-required");
    }
    else
    {
        bpmnFile.parentElement.children[1].classList.remove("color-required");
    }

    if (!svgFile.files[0])
    {
        disable = true;
        svgFile.parentElement.children[1].classList.add("color-required");
    }
    else
    {
        svgFile.parentElement.children[1].classList.remove("color-required");
    }

    btn.disabled = disable;
}

function AgendaDetail(element)
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
    detailDiv.classList.remove("w-0");
    overviewDiv.classList.add("overview-nav-hide");
    element.children[0].classList.add("selected-card");

    element.parentElement.prepend(element);

    $.ajax(
    {
        async: true,
        type: "GET",
        url: "/Agenda/DetailPartial/" + element.id
    })
    .done((result) => 
    {
        document.getElementById("PageNavId").innerHTML = result.header;
        detailDiv.innerHTML = result.detail;
        window.history.pushState({}, '', "/Agenda/Detail/" + element.id);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function ShowModelModal(element)
{
    let content = document.getElementById("ModelModalId");
    content.children[0].children[0].innerHTML = element.innerHTML;
    content.classList.remove("d-none");
    let navbar = document.getElementById("PageNavId");
    navbar.classList.add("page-navbar-modal-large");
    let page = document.getElementById("PageContentId");
    page.innerHTML = "";
    page.classList.remove("d-flex");

    setTimeout(() => 
    {
        page.innerHTML = content.innerHTML;
        content.innerHTML = "";
        navbar.classList.remove("page-navbar-modal-large");
    }, 690);
}

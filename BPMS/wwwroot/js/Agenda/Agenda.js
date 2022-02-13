var ModalHeaderLoaded = false;
var ModalHeader = null;


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
    let h1 = document.createElement("h1");
    h1.classList.add("border-bottom");
    h1.classList.add("text-center");
    h1.classList.add("text-font");
    h1.innerText = element.children[0].innerText;
    element.children[0].remove();
    element.prepend(h1);
    let content = document.getElementById("ModelModalId");
    content.children[0].children[0].innerHTML = element.innerHTML;
    content.classList.remove("d-none");

    let navbar = document.getElementById("PageNavId");
    navbar.classList.add("page-navbar-transition");

    let page = document.getElementById("PageContentId");
    page.innerHTML = "";
    page.classList.remove("d-flex");

    $.ajax(
    {
        async: true,
        type: "GET",
        url: "/Model/Header/" + element.id
    })
    .done((result) => 
    {
        if (ModalHeaderLoaded)
        {
            navbar.innerHTML = result;
            navbar.classList.remove("page-navbar-transition");
        }
        else
        {
            ModalHeader = result;
        }
        window.history.pushState({}, '', "/Model/Detail/" + element.id);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });

    setTimeout(() => 
    {
        navbar.children[0].innerHTML = "";
    }, 350);

    setTimeout(() => 
    {
        ModalHeaderLoaded = true;
        if (ModalHeader)
        {
            navbar.innerHTML = ModalHeader;
            navbar.classList.remove("page-navbar-transition");
        }
        page.innerHTML = content.innerHTML;
        content.innerHTML = "";
        
        for (let ele of document.getElementsByClassName("bpmn-block"))
        {
            ele.addEventListener("click", () => ShowBlockDetail(ele.id));
        }

        LoadingImage = document.getElementById("BlockConfigTargetId").children[0];
    }, 700);
}

function RoleChanged(select)
{
    let addBtn = document.getElementById("AddRoleBtnId");
    let createBtn = document.getElementById("CreateRoleBtnId");
    let nameDiv = document.getElementById("NewRoleNameId");
    let descrDiv = document.getElementById("NewRoleDescId");
    
    if (select.value == "Nevybrána")
    {
        addBtn.classList.add("d-none");
        createBtn.classList.remove("d-none");
        nameDiv.classList.remove("d-none");
        descrDiv.classList.remove("d-none");
    }
    else
    {
        addBtn.classList.remove("d-none");
        createBtn.classList.add("d-none");
        nameDiv.classList.add("d-none");
        descrDiv.classList.add("d-none");
    }
}

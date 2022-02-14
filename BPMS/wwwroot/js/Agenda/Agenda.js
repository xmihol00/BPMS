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
    }, 700);
}

function RoleChanged(select)
{
    let addBtn = document.getElementById("AddRoleBtnId");
    let createBtn = document.getElementById("CreateRoleBtnId");
    let nameDiv = document.getElementById("NewRoleNameId");
    let descrArea = document.getElementById("RoleDescId");
    let option = select.options[select.selectedIndex];
    
    if (!option.getAttribute("value"))
    {
        addBtn.classList.add("d-none");
        createBtn.classList.remove("d-none");
        nameDiv.classList.remove("d-none");
        descrArea.value = "";
        descrArea.readOnly = false;
        ResizeTextArea(descrArea);
    }
    else
    {
        addBtn.classList.remove("d-none");
        createBtn.classList.add("d-none");
        nameDiv.classList.add("d-none");
        descrArea.value = option.getAttribute("data-desc");
        descrArea.readOnly = true;
        ResizeTextArea(descrArea);
    }
}

function EditRole(btn)
{
    let agendaId = document.getElementById("AgendaIdId").value;
    let target = btn.parentNode;
    let roleId = target.id;

    if (btn.classList.contains("text-prim"))
    {
        btn.classList.remove("text-prim");
        target.lastChild.remove();
        for (let ele of target.getElementsByClassName("role-remove-btn"))
        {
            ele.classList.add("d-none");
        }
        target.children[1].classList.add("d-none");
    }
    else
    {
        btn.classList.add("text-prim");
        $.ajax(
        {
            async: true,
            type: "GET",
            url: `/Agenda/MissingInRole/${agendaId}/${roleId}`
        })
        .done((result) => 
        {
            let div = document.createElement("div");
            div.innerHTML = result;
            target.appendChild(div);
        })
        .fail(() => 
        {
            // TODO
            //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
        });
    
        for (let ele of target.getElementsByClassName("role-remove-btn"))
        {
            ele.classList.remove("d-none");
        }
        target.children[1].classList.remove("d-none");
    }
}

function AddUserToRole(btn)
{
    let parent = btn.parentNode.parentNode;
    let select = parent.getElementsByTagName("select")[0]
    let userId = select.value;
    parent = parent.parentNode;
    let roleId = parent.parentNode.id;
    let agendaId = document.getElementById("AgendaIdId").value;

    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Agenda/AddUserRole/${userId}/${agendaId}/${roleId}`
    })
    .done(() => 
    {
        let div = document.createElement("div");
        div.classList.add("d-flex");
        div.classList.add("justify-content-between");
        div.innerHTML = `<b>${select.options[select.selectedIndex].innerText}</b><button id="${userId}" type="button" class="btn btn-sm role-remove-btn" onclick="RemoveUser(this)"><i class="fas fa-times"></i></button>`
        parent.before(div);
        select.options[select.selectedIndex].remove();
        if (!select.options.length)
        {
            btn.disabled = true;
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

function RemoveRole(btn)
{
    let agendaId = document.getElementById("AgendaIdId").value;
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Agenda/RemoveRole/${agendaId}/${btn.parentNode.id}`
    })
    .done(() => 
    {
        btn.parentNode.remove();
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });    
}

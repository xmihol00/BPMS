var AlertTimeout = null;
var AlertInterval = null;
var ModalContentId = null;
var LoadedElements = [];
var Callback = null;
var HideDelay = 850;
var Notifications = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.None).withUrl("/Notification").build();

Notifications.on("Notification", (result) =>
{
    for (let div of document.getElementsByClassName("message-div"))
    {
        document.removeEventListener("click", HideNotifications);
        div.innerHTML =
        `<h4 class="text-font border-bottom text-center mx-3 my-2 pb-1">Nové upozornění</h4>
        <div class="notif-div ${result.state} d-flex justify-content-between">
            <div class="my-auto">
                <span class="text-code text-small">${result.date}:</span><br>
                ${result.text}<b class="notif-info">${result.info}</b>.
            </div>
            <div id="${result.id}" class="my-auto notif-btns">
                <button class="btn text-prim-edit px-1" onclick="NotificationSeen(this)"><i class="fas fa-eye-slash"></i></button>
                <button class="btn text-prim-edit px-1" onclick="NotificationMark(this)"><i class="fas fa-highlighter"></i></button>
                <a class="btn text-prim-edit px-1" href="${result.href}"><i class="fas fa-angle-double-right"></i></a>
            </div>
        </div>
        <div class="d-flex justify-content-center mb-2">
            <button class="butn btn-p py-2" onclick=ShowNotifications(event)>Zobrazit vše</button>
            <button class="butn btn-d py-2" onclick="HideNotificationsListener()">Zavřít</button>
        </div>`;
        div.classList.add("message-div-active");
    }
});

Notifications.start();

document.addEventListener("keydown", KeyDownHandler);
window.addEventListener("popstate", () => window.location.reload())

window.addEventListener('DOMContentLoaded', () => 
{
    if (typeof ActiveBlocks != "undefined")
    {
        DisplayActiveBlocks(ActiveBlocks);
    }

    if (typeof ActiveBlock != "undefined")
    {
        DisplayActiveBlocks(ActiveBlock);
    }

    ResizeTextAreas(document);
    ActivateIcons();
});

function ShowAlert(message, error = false) 
{
    let alertDiv = document.getElementById("AlertDivId");
    alertDiv.style.display = "flex";
    alertDiv.style.opacity = "1";
    
    let alert = alertDiv.firstChild;
    alert.firstChild.innerHTML = message;

    if (AlertTimeout != null)
    {
        clearTimeout(AlertTimeout);
        clearInterval(AlertInterval);
        AlertTimeout = null;
    }
    
    if (error) 
    {
        alert.classList.remove("alert-success");
        alert.classList.add("alert-danger");
    } 
    else 
    {
        alert.classList.add("alert-success");
        alert.classList.remove("alert-danger");
    }

    AlertTimeout = setTimeout(() => HideStart(alertDiv), error ? 6000 : 3000);
}

function ErrorAlert(result)
{
    HideModal();
    if (result.responseText)
    {
        ShowAlert(result.responseText, true);    
    }
    else
    {
        ShowAlert("Operaci se nepodařilo provést, zkontrolujte připojení k internetu.", true);
    }
}

function HideStart(element)
{
    AlertInterval = setInterval(() => { element.style.opacity -= 0.01; }, 30);
    AlertTimeout = setTimeout(HideAlert, 3000);
}

function HideAlert()
{
    let alert = document.getElementById("AlertDivId");
    alert.style.display = "none";

    clearTimeout(AlertTimeout);
    clearInterval(AlertInterval);
}

function ActivateIcons()
{
    document.addEventListener("click", HideNotifications);
    for (let icon of document.getElementsByClassName("fa-info-circle"))
    {
        icon.innerHTML = '<div class="message-div" onclick="event.stopPropagation()"></div>';
        icon.parentNode.addEventListener("click", (event) => ShowNotifications(event));
    }
}

function DisplayActiveBlocks(activeBlocks)
{
    if (activeBlocks)
    {
        if (Array.isArray(activeBlocks))
        {
            for (let ele of activeBlocks)
            {
                let workflow = document.querySelector(`[id='${ele.id}']`);
                for (let blockId of ele.blockIds)
                {
                    let block = workflow.querySelector(`[id='${blockId}']`);
                    let target = block.children[0].children[0];
                    target.style.stroke = "green";
                    target.style.fill = "#abfffd";
                }
            }
        }
        else
        {
            let workflow = document.querySelector(`[id='WorkflowModelId']`);
            for (let blockId of activeBlocks.blockIds)
            {
                let block = workflow.querySelector(`[id='${blockId}']`);
                let target = block.children[0].children[0];
                target.style.stroke = "green";
                target.style.fill = "#abfffd";
            }
        }
    }
}

function ResizeTextAreas(element, modal = false)
{
    for (let textarea of element.getElementsByTagName("textarea"))
    {
        ResizeTextArea(textarea, modal);
        textarea.addEventListener("input", ResizeTextArea);
    }
}

function ResizeTextArea(event, modal = false)
{
    let aditional = modal ? 50 : 0;
    let textarea = event.target || event;
    if (textarea.scrollHeight < 60)
    {
        textarea.style.height = "60px";
    }
    else if (textarea.scrollHeight > textarea.clientHeight)
    {

        textarea.style.height = `${textarea.scrollHeight + aditional}px`;
    }
}

function HistoryBack()
{
    history.back();
}

function HandleRedirect()
{
    window.location.href = `/Account/SignIn?ReturnUrl=${encodeURIComponent(window.location.pathname)}`;
}

function KeyDownHandler(event)
{
    if (event.key == "Escape")
    {
        HideModal();
    }
}

function ToggleSideBar()
{
    let sideBar = document.getElementById("MainSideBarId");
    if (sideBar.classList.contains("side-menu-bar-small"))
    {
        sideBar.classList.remove("side-menu-bar-small");
        document.getElementById("Main").classList.remove("main-content-large");
        document.getElementById("Footer").classList.remove("footer-large");
    }
    else
    {
        sideBar.classList.add("side-menu-bar-small");
        document.getElementById("Footer").classList.add("footer-large");
        document.getElementById("Main").classList.add("main-content-large");
    }
}

function ShowModalElement(contentId)
{
    document.getElementById("ModalBackgroundId").classList.add("modal-background-show");
    let modal = document.getElementById(contentId);
    modal.classList.remove("d-none");
    ResizeTextAreas(modal, true);
    document.getElementById("PageNavId").classList.add("page-navbar-modal");
    ModalContentId = contentId;
}

function ShowModal(contentId, url = null, targetId = null, remember = true, hideCallback = null, succesCallback = null)
{
    ShowModalElement(contentId);
    if (targetId)
    {
        let target = document.getElementById(targetId);

        if (url && !LoadedElements.includes(targetId))
        {
            $.ajax(
            {
                async: true,
                type: "GET",
                url: url,
                statusCode: { 401: HandleRedirect }
            })
            .done((result) => 
            {
                target.innerHTML = result;
                ResizeTextAreas(target, true);
                if (remember)
                {
                    LoadedElements.push(targetId);
                }

                if (succesCallback)
                {
                    succesCallback();
                }
            })
            .fail((result) => 
            {
                ErrorAlert(result);
            });
        }
    }
    
    Callback = hideCallback;
}

function HideModal()
{
    let modal = document.getElementById("PageNavId");
    modal.scroll(0, 0);
    modal.classList.remove("page-navbar-modal");
    document.getElementById("ModalBackgroundId").classList.remove("modal-background-show");
    
    if (ModalContentId)
    {
        if (Callback)
        {
            Callback();
        }

        setTimeout(() => 
        {
            document.getElementById(ModalContentId).classList.add("d-none");
            ModalContentId = null;
        }, HideDelay);
    }
}

function FileSelected(element)
{
    let label = element.parentNode.children[1];
    if (element.files[0])
    {
        label.classList.add("input-file-chosen");
        element.style.color = "#000";
    }
    else
    {
        label.classList.remove("input-file-chosen");
        element.style.color = "#fff";
    }
}

function AjaxFormSubmit(event, targetId = null, hide = false, delay = false, callback = null, successCallback = null, failCallback = null)
{
    let form;
    if (event.target)
    {
        event.preventDefault();
        form = event.target;
    }
    else
    {
        form = event;
    }

    if (callback)
    {
        callback();
    }

    const dto = new FormData(form);
    $.ajax(
    {
        async: true,
        type: "POST",
        url: form.getAttribute("action"),
        data: dto,
        contentType: false,
        processData: false,
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        if (hide)
        {
            HideModal();
        }
        
        if (result && targetId)
        {
            if (delay)
            {
                setTimeout(() => DisplayResult(targetId, result, successCallback), HideDelay);
            }
            else
            {
                DisplayResult(targetId, result, successCallback);
            }
        }
        else if (successCallback)
        {
            successCallback(result);
        }
    })
    .fail((result) => 
    {
        if (failCallback)
        {
            failCallback();
        }
        else
        {
            ErrorAlert(result);
        }
    });        
}

function InfoCardUpdate(result)
{
    document.getElementById("DetailInfoId").innerHTML = result.info;
    let sideOverview = document.getElementById("OverviewDivId").children[0];
    let div = document.createElement("div");
    div.innerHTML = result.card;
    sideOverview.children[0].innerHTML = div.children[0].innerHTML;
}

function DisplayResult(targetId, result, successCallback)
{
    let target = document.getElementById(targetId)
    target.innerHTML = result;
    ResizeTextAreas(target);
    if (successCallback)
    {
        successCallback();
    }
}

function InputValidator(form, select = false)
{
    let disabled = false;

    for (let input of form.querySelectorAll("[required]"))
    {
        if (input.type == "file")
        {
            if (!input.files[0] && input.name)
            {
                disabled = true;
            }
        }
        else if (input.classList.contains("input-select"))
        {
            if (input.options.length == 0 || (select && !input.value))
            {
                disabled = true;
                break;
            }
        }
        else if (!input.value || !input.value.trim())
        {
            disabled = true;
            break;
        }
    }

    for (let input of form.querySelectorAll("[data-url]"))
    {
        try 
        {
            new URL(input.value);
            document.getElementById(input.getAttribute("data-url")).classList.remove("color-required");  
        }
        catch (_) 
        {
            disabled = true;
            document.getElementById(input.getAttribute("data-url")).classList.add("color-required");
        }
    }

    for (let button of document.querySelectorAll(`[form=${form.id}]`))
    {
        button.disabled = disabled;
    }
}

function PasswordValidator(form)
{
    InputValidator(form);
    let pwd = document.getElementById("PasswordId").value;
    let check = document.getElementById("PasswordCheckId").value;
    if (check && check != pwd)
    {
        document.getElementById("PasswordMismatchId").classList.remove("d-none");
        document.querySelector("[form=AccountCreateId]").disabled = true;
    }
    else
    {
        document.getElementById("PasswordMismatchId").classList.add("d-none");
    }

    if (pwd && !document.getElementById("PasswordId").value.match(/(?=.*[A-Z])(?=.*[!@#$&._%()*-/+$^&{}:])(?=.*[0-9])(?=.*[a-z]).{10}/))
    {
        document.getElementById("PasswordWeakId").classList.remove("d-none");
        document.querySelector("[form=AccountCreateId]").disabled = true;
    }
    else
    {
        document.getElementById("PasswordWeakId").classList.add("d-none");
    }
}

function GetAjaxRequest(url, targetId)
{
    $.ajax(
    {
        async: true,
        type: "GET",
        url: url,
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        let target = document.getElementById(targetId);
        target.innerHTML = result;
        ResizeTextAreas(target);
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    }); 
}

function LoadingImageHtml()
{
    return "<div class='loading-content'></div>"
}

function OverviewTransition(path)
{
    let detailDiv = document.getElementById("DetailDivId");
    let overviewDiv = document.getElementById("OverviewDivId");
    let overviewNav = document.getElementById("OverviewNavId");

    detailDiv.classList.remove("container-lg");
    detailDiv.innerHTML = LoadingImageHtml();

    overviewDiv.classList.remove("side-overview");
    overviewDiv.children[0].children[0].children[0].classList.remove("selected-card");

    overviewNav.classList.remove("overview-nav-hide");

    $.ajax(
    {
        async: true,
        type: "GET",
        url: path,
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        document.getElementById("PageNavId").innerHTML = result.header;
        overviewNav.innerHTML = result.filters;

        window.history.pushState({}, '', path.replace("Partial", ""));
        ActivateIcons();
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

function DetailTransition(element, path, blocks = false, succesCallback = null, pools = false)
{
    let detailDiv = document.getElementById("DetailDivId");

    if (document.location.pathname.match(/(User|System)\/Detail/g) && element.parentNode.parentNode.id != "OverviewDivId")
    {
        window.location.href = path.replace("Partial", "") + element.id;
        return;
    }
    else if (document.location.pathname.includes("/Detail/"))
    {
        detailDiv.innerHTML = LoadingImageHtml();
    }

    let overviewDiv = document.getElementById("OverviewDivId");
    overviewDiv.children[0].children[0].children[0].classList.remove("selected-card");
        
    overviewDiv.classList.add("side-overview");
    detailDiv.classList.add("container-lg");
    detailDiv.classList.remove("d-none");
    
    let overviewNav = document.getElementById("OverviewNavId");
    overviewNav.classList.add("overview-nav-hide");
        
    element.children[0].classList.add("selected-card");
    element.parentElement.prepend(element);
    window.scrollTo({ top: 0, behavior: 'smooth' });
    
    $.ajax(
    {
        async: true,
        type: "GET",
        url: path + element.id,
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        document.getElementById("PageNavId").innerHTML = result.header;
        detailDiv.innerHTML = result.detail;
        if (blocks)
        {
            DisplayActiveBlocks(result.activeBlocks || result.activeBlock);
        }
        if (pools)
        {
            DisplayActivePools(result.activePools);
        }
        window.history.pushState({}, '', path.replace("Partial", "") + element.id);
        
        if (succesCallback)
        {
            succesCallback(result);
        }

        ActivateIcons();
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

function HideModelHeader()
{
    setTimeout(() => 
    {
        document.getElementById("BlockConfigTargetId").innerHTML = LoadingImageHtml();
    }, HideDelay);
}

function CreateInput(value, name, form) 
{
    let element = document.createElement("input");
    element.value = value
    element.name = name;
    element.type = "hidden";
    element.setAttribute("data-gen", "");
    form.appendChild(element);
}

function FilterChanges(event, path, blocks = false)
{
    let target = event.target;
    if (target.classList.contains("filter-div"))
    {
        let dto = {};
        dto.Filter = target.id;
        dto.Removed = target.classList.contains("filter-div-sel");

        $.ajax(
        {
            async: true,
            type: "POST",
            url: path,
            data: dto,
            statusCode: { 401: HandleRedirect }
        })
        .done((result) => 
        {
            if (blocks)
            {
                document.getElementById("OverviewDivId").innerHTML = result.overview;
                DisplayActiveBlocks(result.activeBlocks);
            }
            else
            {
                document.getElementById("OverviewDivId").innerHTML = result;
            }

            if (dto.Removed)
            {
                target.classList.remove("filter-div-sel");
            }
            else
            {
                target.classList.add("filter-div-sel");
            }
        })
        .fail((result) => 
        {
            ErrorAlert(result);
        });
    }
}

function ShowNotifications(event)
{
    event.stopPropagation();
    document.addEventListener("click", HideNotifications);

    $.ajax(
    {
        async: true,
        type: "GET",
        url: "/Notification/All",
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        for (let div of document.getElementsByClassName("message-div"))
        {
            div.innerHTML = result;
            div.classList.add("message-div-active");
        } 
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

function NotificationSeen(btn)
{
    document.addEventListener("click", HideNotifications);
    $.ajax(
    {
        async: true,
        type: "POST",
        url: "/Notification/Seen/" + btn.parentNode.id,
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        for (let div of document.getElementsByClassName("message-div"))
        {
            div.innerHTML = result;
            div.classList.add("message-div-active");
        } 
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

function NotificationMark(btn)
{
    document.addEventListener("click", HideNotifications);
    let marked = btn.classList.contains("text-prim");
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Notification/Mark/${btn.parentNode.id}/${marked}`,
        statusCode: { 401: HandleRedirect }
    })
    .done(() => 
    {
        if (marked)
        {
            btn.classList.remove("text-prim");
            btn.parentNode.parentNode.classList.remove("Marked");
        }
        else
        {
            btn.classList.add("text-prim");
            btn.parentNode.parentNode.classList.add("Marked");
            if (btn.parentNode.children.length == 3)
            {
                btn.parentNode.children[0].remove();
            }
        }
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

function HideNotifications()
{
    for (let div of document.getElementsByClassName("message-div"))
    {
        div.classList.remove("message-div-active");
    }   
}

function HideNotificationsListener()
{
    document.addEventListener("click", HideNotifications);
    HideNotifications();
}

function NotifFilterChange(btn)
{
    let dto = {};
    dto.Filter = btn.id;
    dto.Removed = btn.classList.contains("text-prim");

    $.ajax(
    {
        async: true,
        type: "POST",
        url: "/Notification/Filter",
        data: dto,
        statusCode: { 401: HandleRedirect }
    })
    .done((result) => 
    {
        for (let messages of document.getElementsByClassName("message-div"))
        {
            messages.innerHTML = result;
            messages.classList.add("message-div-active");
        }
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

function NotificationRemove(btn)
{
    let parent = btn.parentNode;
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/Notification/Remove/${parent.id}`,
        statusCode: { 401: HandleRedirect }
    })
    .done(() => 
    {
        parent.parentNode.remove();
    })
    .fail((result) => 
    {
        ErrorAlert(result);
    });
}

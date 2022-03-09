
var ModalContentId = null;
var LoadedElements = [];
var Callback = null;
var HideDelay = 850;
var Notifications = new signalR.HubConnectionBuilder().configureLogging(signalR.LogLevel.None).withUrl("/Notification").build();

Notifications.on("Notification", (result) =>
{
    console.log(result);
});

Notifications.hub.qs = "test=123";

Notifications.start();

document.addEventListener("keydown", KeyDownHandler);

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
});

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
                url: url
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
            .fail(() => 
            {
                // TODO
                //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
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
    .fail(() => 
    {
        if (failCallback)
        {
            failCallback();
        }
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
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
    if (document.getElementById("PasswordCheckId").value != document.getElementById("PasswordId").value)
    {
        document.getElementById("PasswordMismatchId").classList.remove("d-none");
        document.querySelector("[form=AccountCreateId]").disabled = true;
    }
    else
    {
        document.getElementById("PasswordMismatchId").classList.add("d-none");
    }
}

function GetAjaxRequest(url, targetId)
{
    $.ajax(
    {
        async: true,
        type: "GET",
        url: url
    })
    .done((result) => 
    {
        let target = document.getElementById(targetId);
        target.innerHTML = result;
        ResizeTextAreas(target);
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    }); 
}

function LoadingImageHtml()
{
    return "<div class='loading-content'></div>"
}

function DetailTransition(element, path, blocks = false, succesCallback = null)
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

    for (let card of document.getElementsByClassName("selected-card"))
    {
        card.classList.remove("selected-card");
    }
    
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
    window.scrollTo({ top: 0, behavior: 'smooth' });
    
    $.ajax(
    {
        async: true,
        type: "GET",
        url: path + element.id
    })
    .done((result) => 
    {
        document.getElementById("PageNavId").innerHTML = result.header;
        detailDiv.innerHTML = result.detail;
        if (blocks)
        {
            DisplayActiveBlocks(result.activeBlocks || result.activeBlock);
        }
        window.history.pushState({}, '', path.replace("Partial", "") + element.id);
        
        if (succesCallback)
        {
            succesCallback(result);
        }
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
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
            data: dto
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
        .fail(() => 
        {
            // TODO
            //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
        });
    }
}

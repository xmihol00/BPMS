
var ModalContentId = null;
var LoadedElements = [];
var Callback = null;
var HideDelay = 850;

document.addEventListener("keydown", KeyDownHandler);

window.addEventListener('DOMContentLoaded', () => 
{
    for (let icon of document.getElementsByClassName("fa-chevron-circle-left"))
    {
        icon.parentNode.addEventListener("click", HistoryBack);
    }

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
    document.getElementById("PageNavId").classList.remove("page-navbar-modal");
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

function InputValidator(form)
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
            if (input.options.length == 0)
            {
                disabled = true;
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
        window.scrollTo({ top: 0, behavior: 'smooth' });

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

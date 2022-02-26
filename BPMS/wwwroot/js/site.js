
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

    ResizeTextAreas(document);
});

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
    let aditional = modal ? 26 : 0;
    let textarea = event.target || event;
    if (textarea.scrollHeight < 60)
    {
        textarea.style.height = "60px";
    }
    else
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
                succesCallback();
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
            if (!input.files[0])
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

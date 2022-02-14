
var ModalContentId = null;
var LoadedElements = [];
var Callback = null;

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

function ShowModal(contentId, url = null, targetId = null, remember = true, callback = null)
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
            })
            .fail(() => 
            {
                // TODO
                //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
            });
        }
    }
    
    Callback = callback;
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
            Callback = null;
        }

        setTimeout(() => 
        {
            document.getElementById(ModalContentId).classList.add("d-none");
            ModalContentId = null;
        }, 850);
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

function AjaxFormSubmit(event, targetId = null, callback = null, successCallback = null, failCallback = null, hide = false)
{
    event.preventDefault();
    let form = event.target;
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
        if (result && targetId)
        {
            let target = document.getElementById(targetId)
            target.innerHTML = result;
            ResizeTextAreas(target);
        }

        if (hide)
        {
            HideModal();
        }

        if (successCallback)
        {
            successCallback();
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

function InputValidator(form)
{
    let disabled = false;

    for (let input of form.querySelectorAll("[data-reqi]"))
    {
        if (!input.value || !input.value.trim())
        {
            disabled = true;
            document.getElementById(input.getAttribute("data-reqi")).classList.add("color-required");
        }
        else
        {
            document.getElementById(input.getAttribute("data-reqi")).classList.remove("color-required");
        }
    }

    for (let input of form.querySelectorAll("[data-reqf]"))
    {
        if (!input.files[0])
        {
            disabled = true;
            document.getElementById(input.getAttribute("data-reqf")).classList.add("color-required");
        }
        else
        {
            document.getElementById(input.getAttribute("data-reqf")).classList.remove("color-required");
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

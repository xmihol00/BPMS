
var ModalContentId = null;
var Validator = null;
var LoadedElements = [];
var Callback = null;

document.addEventListener("keydown", KeyDownHandler);

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

function ShowModalElement(contentId, validator = null)
{
    document.getElementById("ModalBackgroundId").classList.add("modal-background-show");
    document.getElementById(contentId).classList.remove("d-none");
    document.getElementById("PageNavId").classList.add("page-navbar-modal");
    //document.getElementById("PageContentId").classList.add("page-content-modal");
    ModalContentId = contentId;
    if (validator)
    {
        document.addEventListener("input", validator);
        Validator = validator;
    }
}

function ShowModal(contentId, url = null, targetId = null, validator = null, remember = true, callback = null)
{
    ShowModalElement(contentId, validator);
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
    let modal = document.getElementById("PageNavId");
    modal.classList.remove("page-navbar-modal");
    modal.classList.remove("page-navbar-modal-large");
    document.getElementById("ModalBackgroundId").classList.remove("modal-background-show");
    
    if (Validator)
    {
        document.removeEventListener("input", Validator);
        Validator = null;
    }

    if (ModalContentId)
    {
        setTimeout(() => 
        {
            document.getElementById(ModalContentId).classList.add("d-none");
            ModalContentId = null;
            if (Callback)
            {
                Callback();
                Callback = null;
            }
        }, 350);
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

function AjaxFormSubmit(event, targetId)
{
    event.preventDefault();
    let form = event.target;

    const dto = new FormData(form);
    console.log(dto, form.getAttribute("action"));
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
        document.getElementById(targetId).innerHTML = result;
        
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
        
}

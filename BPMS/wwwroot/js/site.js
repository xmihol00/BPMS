
var ModalContentId = null;

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

function ShowModalElement(contentId, inputValidator = null)
{
    document.getElementById("ModalBackgroundId").classList.add("modal-background-show");
    document.getElementById(contentId).classList.remove("d-none");
    document.getElementById("PageNavId").classList.add("page-navbar-modal");
    document.getElementById("PageContentId").classList.add("page-content-modal");
    ModalContentId = contentId;
    if (inputValidator)
    {
        document.addEventListener("input", inputValidator);
    }
}

function ShowModal(contentId, url = null, targetId = null, inputValidator = null)
{
    let target = document.getElementById(targetId);
    if (url && !target.innerHTML)
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
            ShowModalElement(contentId, inputValidator);
        })
        .fail(() => 
        {
            // TODO
            //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
        });
    }
    else
    {
        ShowModalElement(contentId, inputValidator);
    }
}

function HideModal()
{
    document.getElementById("PageNavId").classList.remove("page-navbar-modal");
    document.getElementById("ModalBackgroundId").classList.remove("modal-background-show");
    if (ModalContentId)
    {
        setTimeout(() => 
        {
            document.getElementById(ModalContentId).classList.add("d-none");
            ModalContentId = null;
        }, 400);
    }
    setTimeout(() => document.getElementById("PageContentId").classList.remove("page-content-modal"), 700);
}

function FileSelected(element)
{
    if (element.files[0])
    {
        element.parentNode.children[1].classList.add("input-file-chosen");
        element.style.color = "#000";
    }
    else
    {
        element.parentNode.children[1].classList.remove("input-file-chosen");
        element.style.color = "#fff";
    }
}

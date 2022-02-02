
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

function AgendaDetail(element)
{
    for (let card of document.getElementsByClassName("selected-card"))
    {
        card.classList.remove("selected-card");
    }

    let detailDiv = document.getElementById("DetailDivId");
    let topEle = element.parentNode.parentNode;
    let overviewDiv = document.getElementById("OverviewNavId");

    $.ajax(
    {
        async: true,
        type: "GET",
        url: "/Agenda/DetailPartial/" + element.id
    })
    .done((result) => 
    {
        detailDiv.innerHTML = result;
        topEle.classList.add("side-overview");
        detailDiv.classList.remove("d-none");
        detailDiv.classList.add("container-lg");
        overviewDiv.classList.add("overview-nav-hide");
        element.children[0].classList.add("selected-card");
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

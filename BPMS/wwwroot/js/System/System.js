
function SystemActivate(result)
{
    InfoCardUpdate(result);

    let buttonDiv = document.getElementById("SystemBtnsId");
    buttonDiv.children[0].remove();
    buttonDiv.innerHTML = `<button class="butn btn-d" onclick="Deactive()">Deaktivovat</button>` + buttonDiv.innerHTML;
}

function SystemReactivate(result)
{
    InfoCardUpdate(result);

    let buttonDiv = document.getElementById("SystemBtnsId");
    buttonDiv.children[0].remove();
    buttonDiv.innerHTML = `<button class="butn btn-s" disabled title="Žádost o navázání spojení již byla odeslána.">Aktivovat</button>` + buttonDiv.innerHTML;
}

function Deactive()
{
    $.ajax(
    {
        async: true,
        type: "POST",
        url: `/System/Deactivate/${document.getElementById("IdId").value}`,
    })
    .done((result) => 
    {
        InfoCardUpdate(result);

        let buttonDiv = document.getElementById("SystemBtnsId");
        buttonDiv.children[0].remove();
        buttonDiv.innerHTML = `<button class="butn btn-s" onclick="ShowModal('ReactivateFormId')">Aktivovat</button>` + buttonDiv.innerHTML;
    })
    .fail(() => 
    {
        // TODO
        //ShowAlert("Nepodařilo se získat potřebná data, zkontrolujte připojení k internetu.", true);
    });
}

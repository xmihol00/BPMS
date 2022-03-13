
function SystemUpdate(result)
{
    InfoCardUpdate(result);

    let buttonDiv = document.getElementById("SystemBtnsId");
    buttonDiv.children[0].remove();
    buttonDiv.innerHTML = <button class="butn btn-d" onclick="Deactive()">Deaktivovat</button> + buttonDiv.innerHTML;
}
